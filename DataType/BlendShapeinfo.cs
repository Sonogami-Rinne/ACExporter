using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
internal class BlendShapeinfo
{
    public string patternName;
    public bool isDefault;
    public string type;
    public List<string> blendShapeName;
    public List<float> blendShapeWeight;
    public int gagEye;
    public string gagMaterialTex;
    public string gagMaterialTexR;
    public List<float> vector3TileAnimation;
    public List<float> vector3TileAnimationR;
    public float sizeSpeed;
    public float sizeSpeedR;
    public float sizeWidth;
    public float sizeWidthR;
    public float angleSpeed;
    public float angleSpeedR;
    public float yurayura;
    public bool isExpression;
    public string expressionMaterialTex;

    public BlendShapeinfo(string patternName, string type, bool isDefault)
    {
        this.patternName = patternName;
        this.blendShapeName = new List<string>();
        this.blendShapeWeight = new List<float>();
        this.type = type;
        this.gagEye = -1;
        this.gagMaterialTex = "";
        this.gagMaterialTexR = "";
        this.vector3TileAnimation = new List<float>();
        this.vector3TileAnimationR = new List<float>();
        this.sizeSpeed = 0;
        this.sizeSpeedR = 0;
        this.sizeWidth = 0;
        this.sizeWidthR = 0;
        this.angleSpeed = 0;
        this.angleSpeedR = 0;
        this.yurayura = 0;
        this.isDefault = isDefault;
        this.isExpression = false;
        this.expressionMaterialTex = "";
    }
    public void Add(string blendShapeName, float blendShapeWeight)
    {
        this.blendShapeWeight.Add(blendShapeWeight);
        this.blendShapeName.Add(blendShapeName);
    }
}
