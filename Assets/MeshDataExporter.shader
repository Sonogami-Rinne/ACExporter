Shader "Hidden/MeshDataExporter"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Name "BasicInfo"
            ZWrite Off
            ZTest Always
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5
            #pragma require geometry
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                uint _TextureWidth;
                uint _TextureHeight;
            CBUFFER_END
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
                uint   vertexID   : SV_VertexID;
            };
            
            struct v2g
            {
                float3 posOS    : TEXCOORD0;
                float3 normalOS : TEXCOORD1;
                float2 uv       : TEXCOORD2;
                float4 color    : TEXCOORD3;
                uint   vertexID : TEXCOORD4;
            };
            
            struct g2f
            {
                float4 positionCS : SV_POSITION;
                nointerpolation uint value : TEXCOORD0;
            };
            
            float4 PackUInt(uint bits)
            {
                return float4(
                    ((bits >> 0)  & 0xFF) / 256.0,
                    ((bits >> 8)  & 0xFF) / 256.0,
                    ((bits >> 16) & 0xFF) / 256.0,
                    ((bits >> 24) & 0xFF) / 256.0
                );
            }
            
            v2g vert(Attributes IN)
            {
                v2g OUT;
                OUT.posOS    = IN.positionOS;
                OUT.normalOS = IN.normalOS;
                OUT.uv       = IN.uv;
                OUT.color    = IN.color;
                OUT.vertexID = IN.vertexID;
                return OUT;
            }
            
            [maxvertexcount(156)]
            void geom(triangle v2g input[3], uint primitiveID : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
            {
                for (uint vtx = 0; vtx < 3; vtx++)
                {                    
                    uint data[13] = {
                        input[vtx].vertexID,
                        asuint(input[vtx].posOS.x),
                        asuint(input[vtx].posOS.y),
                        asuint(input[vtx].posOS.z),
                        asuint(input[vtx].normalOS.x),
                        asuint(input[vtx].normalOS.y),
                        asuint(input[vtx].normalOS.z),
                        asuint(input[vtx].uv.x),
                        asuint(input[vtx].uv.y),
                        asuint(input[vtx].color.r),
                        asuint(input[vtx].color.g),
                        asuint(input[vtx].color.b),
                        asuint(input[vtx].color.a)
                    };
                    
                    uint startPixel = primitiveID * 48u + vtx * 16u;
                    
                    for (uint i = 0; i < 13; i++)
                    {
                        uint pixelIndex = startPixel + i;
                        float px = pixelIndex % _TextureWidth;
                        float py = pixelIndex / _TextureWidth;
                        py = _TextureHeight - 1 - py;
                        
                        float left   = (px / (float)_TextureWidth) * 2.0 - 1.0;
                        float right  = ((px + 1.0) / (float)_TextureWidth) * 2.0 - 1.0;
                        float bottom = (py / (float)_TextureHeight) * 2.0 - 1.0;
                        float top    = ((py + 1.0) / (float)_TextureHeight) * 2.0 - 1.0;
                        
                        g2f v;
                        v.value = data[i];
                        
                        v.positionCS = float4(left,  top, 0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(right, top, 0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(left,  bottom,    0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(right, bottom,    0.0, 1.0);
                        outStream.Append(v);
                        
                        outStream.RestartStrip();
                    }
                }
            }
            
            float4 frag(g2f IN) : SV_Target
            {
                return PackUInt(IN.value);
            }
            
            ENDHLSL
        }
        Pass
        {
            Name "AdditionalUVs"
            ZWrite Off
            ZTest Always
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5
            #pragma require geometry
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                uint _TextureWidth;
                uint _TextureHeight;
                uint _UVMask;
            CBUFFER_END
            
            struct Attributes
            {
                float4 uv1         : TEXCOORD1;
                float4 uv2         : TEXCOORD2;
                float4 uv3         : TEXCOORD3;
                float4 uv4         : TEXCOORD4;
                float4 uv5         : TEXCOORD5;
                float4 uv6         : TEXCOORD6;
                float4 uv7         : TEXCOORD7;
                uint   vertexID   : SV_VertexID;
            };
            
            struct v2g
            {
                float4 uv1      : TEXCOORD0;
                float4 uv2      : TEXCOORD1;
                float4 uv3      : TEXCOORD2;
                uint   vertexID : TEXCOORD3;
            };
            
            struct g2f
            {
                float4 positionCS : SV_POSITION;
                nointerpolation uint value : TEXCOORD0;
            };
            
            float4 PackUInt(uint bits)
            {
                return float4(
                    ((bits >> 0)  & 0xFF) / 256.0,
                    ((bits >> 8)  & 0xFF) / 256.0,
                    ((bits >> 16) & 0xFF) / 256.0,
                    ((bits >> 24) & 0xFF) / 256.0
                );
            }
            
            v2g vert(Attributes IN)
            {
                v2g OUT;
                OUT.vertexID = IN.vertexID;
                float4 uvs[3] = {{0,0,0,0}, {0,0,0,0},{0,0,0,0}};
                uint count = 0;
                uint uvMask = _UVMask;

                for(uint i = 0; count < 3 && uvMask > 0; i++)
                {
                    if (uvMask & 0x1 > 0)
                    {
                        switch (i)
                        {
                            case 0: uvs[count] = IN.uv1;
                                break;
                            case 1: uvs[count] = IN.uv2;
                                break;
                            case 2: uvs[count] = IN.uv3;
                                break;
                            case 3: uvs[count] = IN.uv4;
                                break;
                            case 4: uvs[count] = IN.uv5;
                                break;
                            case 5: uvs[count] = IN.uv6;
                                break;
                            default: uvs[count] = IN.uv7;
                                break;
                        }
                        count = count + 1;
                    }
                    uvMask = uvMask >> 1;
                }
                OUT.uv1 = uvs[0];
                OUT.uv2 = uvs[1];
                OUT.uv3 = uvs[2];

                return OUT;
            }
            
            [maxvertexcount(156)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> outStream)
            {
                for(uint vtx = 0; vtx < 3; vtx++)
                {
                    uint data[13] = {
                        input[vtx].vertexID,
                        asuint(input[vtx].uv1.x),
                        asuint(input[vtx].uv1.y),
                        asuint(input[vtx].uv1.z),
                        asuint(input[vtx].uv1.w),
                        asuint(input[vtx].uv2.x),
                        asuint(input[vtx].uv2.y),
                        asuint(input[vtx].uv2.z),
                        asuint(input[vtx].uv2.w),
                        asuint(input[vtx].uv3.x),
                        asuint(input[vtx].uv3.y),
                        asuint(input[vtx].uv3.z),
                        asuint(input[vtx].uv3.w)
                    };
                    
                    uint startPixel = input[vtx].vertexID * 16u;
                    
                    for (uint i = 0; i < 13; i++)
                    {
                        uint pixelIndex = startPixel + i;
                        float px = pixelIndex % _TextureWidth;
                        float py = pixelIndex / _TextureWidth;
                        py = _TextureHeight - 1 - py;
                        
                        float left   = (px / (float)_TextureWidth) * 2.0 - 1.0;
                        float right  = ((px + 1.0) / (float)_TextureWidth) * 2.0 - 1.0;
                        float bottom = (py / (float)_TextureHeight) * 2.0 - 1.0;
                        float top    = ((py + 1.0) / (float)_TextureHeight) * 2.0 - 1.0;
                        
                        g2f v;
                        v.value = data[i];
                        
                        v.positionCS = float4(left,  top, 0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(right, top, 0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(left,  bottom,    0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(right, bottom,    0.0, 1.0);
                        outStream.Append(v);
                        
                        outStream.RestartStrip();
                    }
                }
            }
            
            float4 frag(g2f IN) : SV_Target
            {
                return PackUInt(IN.value);
            }
            
            ENDHLSL
        }
        Pass
        {
            Name "VertexColorOnly"
            ZWrite Off
            ZTest Always
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5
            #pragma require geometry
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                uint _TextureWidth;
                uint _TextureHeight;
            CBUFFER_END
            
            struct Attributes
            {
                float4 color      : COLOR;
                uint   vertexID   : SV_VertexID;
            };
            
            struct v2g
            {
                float4 color    : TEXCOORD0;
                uint   vertexID : TEXCOORD1;
            };
            
            struct g2f
            {
                float4 positionCS : SV_POSITION;
                nointerpolation uint value : TEXCOORD0;
            };
            
            float4 PackUInt(uint bits)
            {
                return float4(
                    ((bits >> 0)  & 0xFF) / 256.0,
                    ((bits >> 8)  & 0xFF) / 256.0,
                    ((bits >> 16) & 0xFF) / 256.0,
                    ((bits >> 24) & 0xFF) / 256.0
                );
            }
            
            v2g vert(Attributes IN)
            {
                v2g OUT;
                OUT.color    = IN.color;
                OUT.vertexID = IN.vertexID;
                return OUT;
            }
            
            [maxvertexcount(60)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> outStream)
            {
                for(uint vtx = 0; vtx < 3; vtx++)
                {
                    uint data[5] = {
                        input[vtx].vertexID,
                        asuint(input[vtx].color.r),
                        asuint(input[vtx].color.g),
                        asuint(input[vtx].color.b),
                        asuint(input[vtx].color.a)
                    };
                                        
                    uint startPixel = input[vtx].vertexID * 8u;              

                    for (uint i = 0; i < 5; i++)
                    {
                        uint pixelIndex = startPixel + i;
                        float px = pixelIndex % _TextureWidth;
                        float py = pixelIndex / _TextureWidth;
                        py = _TextureHeight - 1 - py;
                        
                        float left   = (px / (float)_TextureWidth) * 2.0 - 1.0;
                        float right  = ((px + 1.0) / (float)_TextureWidth) * 2.0 - 1.0;
                        float bottom = (py / (float)_TextureHeight) * 2.0 - 1.0;
                        float top    = ((py + 1.0) / (float)_TextureHeight) * 2.0 - 1.0;
                        
                        g2f v;
                        v.value = data[i];
                        
                        v.positionCS = float4(left,  top, 0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(right, top, 0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(left,  bottom,    0.0, 1.0);
                        outStream.Append(v);
                        v.positionCS = float4(right, bottom,    0.0, 1.0);
                        outStream.Append(v);
                        
                        outStream.RestartStrip();
                    }
                }
            }
            
            float4 frag(g2f IN) : SV_Target
            {
                return PackUInt(IN.value);
            }
            
            ENDHLSL
        }
    }
}