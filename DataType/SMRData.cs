using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
internal class SMRData
{
	public int CoordinateType = -1;

	public string SMRName;

	public string SMRPath;

	public List<string> SMRBoneNames = new List<string>();

	public List<string> SMRMaterialNames = new List<string>();

	public SMRData(PmxBuilder pmxBuilder, SkinnedMeshRenderer smr)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		SMRName = PmxBuilder.CleanUpName(smr.name);
		//SMRName = ((pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal) && smr.sharedMaterials.Count() > 0 && pmxBuilder.ignoreList.Contains(PmxBuilder.CleanUpName(smr.sharedMaterial.name), StringComparer.Ordinal)) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
		SMRName = ((pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal)) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
		SMRPath = PmxBuilder.GetGameObjectPath(smr.gameObject);
		for (int i = 0; i < smr.bones.Count(); i++)
		{
			if ((bool)smr.bones[i])
			{
				SMRBoneNames.Add(PmxBuilder.GetAltBoneName(smr.bones[i]));
			}
		}
		for (int j = 0; j < smr.materials.Count(); j++)
		{
			if ((bool)smr.materials[j])
			{
				string name = smr.materials[j].name;
				name = PmxBuilder.CleanUpName(name);
				name = ((!pmxBuilder.ignoreList.Contains(name, StringComparer.Ordinal) || !pmxBuilder.ignoreList.Contains(smr.name, StringComparer.Ordinal)) ? (name + " " + PmxBuilder.GetAltInstanceID(smr.transform.parent.gameObject)) : ((!name.Contains(pmxBuilder.EyeMatName)) ? name : (name + "_" + smr.name)));
				name = PmxBuilder.GetAltMaterialName(pmxBuilder, name);
				SMRMaterialNames.Add(name);
			}
		}
	}

	public SMRData(PmxBuilder pmxBuilder, MeshRenderer smr)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		SMRName = smr.name;
		SMRName = (pmxBuilder.ignoreList.Contains(SMRName, StringComparer.Ordinal) ? SMRName : (SMRName + " " + PmxBuilder.GetAltInstanceID(smr)));
		SMRPath = PmxBuilder.GetGameObjectPath(smr.gameObject);
		for (int i = 0; i < smr.materials.Count(); i++)
		{
			string name = smr.materials[i].name;
			name = PmxBuilder.CleanUpName(name);
			name = ((pmxBuilder.ignoreList.Contains(name, StringComparer.Ordinal) && pmxBuilder.ignoreList.Contains(smr.name, StringComparer.Ordinal)) ? ((!name.Contains(pmxBuilder.EyeMatName)) ? name : (name + "_" + smr.name)) : (name + " " + PmxBuilder.GetAltInstanceID(smr.transform.parent.gameObject)));
			name = PmxBuilder.GetAltMaterialName(pmxBuilder, name);
			SMRMaterialNames.Add(name);
		}
	}
}
