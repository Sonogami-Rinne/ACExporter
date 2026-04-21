using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
internal class BlendShapeinfo
{
    public string patternName;
    public string type;
    public List<string> blendShapeName;
    public List<float> blendShapeWeight;

    public BlendShapeinfo(string patternName, string type)
    {
        this.patternName = patternName;
        this.blendShapeName = new List<string>();
        this.blendShapeWeight = new List<float>();
        this.type = type;
    }
    public void Add(string blendShapeName, float blendShapeWeight)
    {
        this.blendShapeWeight.Add(blendShapeWeight);
        this.blendShapeName.Add(blendShapeName);
    }
}
