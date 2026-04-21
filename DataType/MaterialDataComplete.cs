using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
internal class MaterialDataComplete
{
    public string SMRPath;
    public int CoordinateType = -1;
    public int EnumIndex = -1;
    public string SMRName;

    public List<MaterialInfo> MaterialInformation = new List<MaterialInfo>();

    public MaterialDataComplete(PmxBuilder pmxBuilder, Renderer smr)
    {
        if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
        {
            CoordinateType = PmxBuilder.nowCoordinate;
        }
        SMRName = smr.name;
        //SMRName = ((pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal) && smr.sharedMaterials.Count() > 0 && pmxBuilder.ignoreList.Contains(PmxBuilder.CleanUpName(smr.sharedMaterial.name), StringComparer.Ordinal)) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
        SMRName = ((pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal)) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
        SMRPath = PmxBuilder.GetGameObjectPath(smr.gameObject);
        for (int i = 0; i < smr.materials.Count(); i++)
        {
            if ((bool)smr.materials[i])
            {
                string name = smr.materials[i].name;
                name = PmxBuilder.CleanUpName(name);
                name = ((!pmxBuilder.ignoreList.Contains(name, StringComparer.Ordinal) || !pmxBuilder.ignoreList.Contains(smr.name, StringComparer.Ordinal)) ? (name + " " + PmxBuilder.GetAltInstanceID(smr.transform.parent.gameObject)) : ((!name.Contains(pmxBuilder.EyeMatName)) ? name : (name + "_" + smr.name)));
                //name = PmxBuilder.GetAltMaterialName(pmxBuilder, name);
                MaterialInformation.Add(new MaterialInfo(smr.materials[i], name));
            }
        }

    }
}