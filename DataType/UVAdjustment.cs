using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
internal class UVAdjustment
{
    public string SMRName;
    public string SMRPath;
    public int xOffset;
    public int yOffset;
    public int xScale;
    public int yScale;
    public List<string> materials;
    public UVAdjustment(string SMRName, string SMRPath, int xOffset, int yOffset, int xScale, int yScale, List<string> materials)
    {
        this.SMRName = SMRName;
        this.SMRPath = SMRPath;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.xScale = xScale;
        this.yScale = yScale;
        this.materials = materials;
    }
}
