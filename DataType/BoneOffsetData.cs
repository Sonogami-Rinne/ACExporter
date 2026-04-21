using System;
using System.Collections.Generic;
using PmxLib;
using UnityEngine;

[Serializable]
internal class BoneOffsetData
{
	public int CoordinateType = -1;

	public string BoneName = "";

	public List<float> Offset;

	public BoneOffsetData(string boneName, PmxLib.Vector3 offset)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		BoneName = boneName;
		Offset = new List<float>() { offset.x, offset.y, offset.z};
	}
}
