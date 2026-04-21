using System;
using System.Collections.Generic;
using ACExporter.Utils;
using UnityEngine;

[Serializable]
internal class MaterialInfo
{
	public string MaterialName;
	public string ShaderName;
    public bool isHair = false;

	public List<string> ShaderPropNames = new List<string>();
	public List<string> ShaderPropTextureValues = new List<string>();
	public List<List<float>> ShaderPropColorValues = new List<List<float>>();
	public List<float> ShaderPropFloatValues = new List<float>();
	public List<List<float>> ShaderPropVectorValues = new List<List<float>>();

    public MaterialInfo(Material material, string _materialName)
	{
		MaterialName = _materialName;
		ShaderName = material.shader.name;
		//var properties = material.GetTexturePropertyNames();
        isHair = material.shader.name.Contains("hair", System.StringComparison.OrdinalIgnoreCase);

		Shader shader = material.shader;
		
		for(int i = 0; i < shader.GetPropertyCount(); i++)
		{
			string name = shader.GetPropertyName(i);
			switch (shader.GetPropertyType(i))
			{
				case UnityEngine.Rendering.ShaderPropertyType.Color:
					ShaderPropNames.Add(name + " " + "Color" + ShaderPropColorValues.Count);
					ShaderPropColorValues.Add(ConvertColor(material.GetColor(name)));
					break;
				case UnityEngine.Rendering.ShaderPropertyType.Range:
				case UnityEngine.Rendering.ShaderPropertyType.Float:
					ShaderPropNames.Add(name + " " + "Float" + ShaderPropFloatValues.Count);
					ShaderPropFloatValues.Add(material.GetFloat(name));
					break;
				case UnityEngine.Rendering.ShaderPropertyType.Int:
                    ShaderPropNames.Add(name + " " + "Float" + ShaderPropFloatValues.Count);
                    ShaderPropFloatValues.Add(material.GetInt(name));
					break;
				case UnityEngine.Rendering.ShaderPropertyType.Texture:
                    ShaderPropNames.Add(name + " " + "Texture" + " " + ShaderPropTextureValues.Count);
                    Texture texture = material.GetTexture(name);
					if (texture != null)
					{
                        ShaderPropTextureValues.Add(name);
						TextureSaver.SaveTexture(texture, PmxBuilder.currentSavePath + MaterialName + name + ".png");
                    }
					else
					{
                        ShaderPropTextureValues.Add(null);
                    }
					break;
				case UnityEngine.Rendering.ShaderPropertyType.Vector:
					ShaderPropNames.Add(name + " " + "Vector" + " " + ShaderPropVectorValues.Count);
					var vec = material.GetVector(name);
					ShaderPropVectorValues.Add(new List<float>() { vec.x, vec.y, vec.z, vec.w});
					break;
				default:
					Console.WriteLine("Ignored shader property: " + name);
					break;
            }
		}
		//MaterialShader materialShader2 = MaterialShaders.materialShaders.Find((MaterialShader materialShader) => string.CompareOrdinal(materialShader.shaderName, ShaderName) == 0);
		//if (materialShader2 == null)
		//{
		//	return;
		//}
		//foreach (MaterialShaderItem property in materialShader2.properties)
		//{
		//	// Add the color data
  //          string text = "_" + property.name;
		//	if (property.type == "Color")
		//	{
		//		ShaderPropNames.Add(text + " " + property.type + " " + ShaderPropColorValues.Count());
		//		ShaderPropColorValues.Add(ConvertColor(material.GetColor(text)));
  //          }

		//	// check the names of each texture. if there is a texture with "_HGLS" in the name, mark it as a hair shader.
  //          if (property.type == "Texture")
  //          {
  //              Dictionary<string, string> dictionary = PmxBuilder.typeMap.ToDictionary((KeyValuePair<string, string> x) => x.Value, (KeyValuePair<string, string> x) => x.Key);
  //              if (dictionary.ContainsKey(text))
  //              {
  //                  string text2 = dictionary[text];
		//			if (text2 == "_HGLS") {
		//				isHair = true;
  //                  }
  //              }
  //          }

  //      }
    }
	private static List<float> ConvertColor(Color color)
	{
		return new List<float>() { color.r, color.g, color.b, color.a };
	}
}
