using System;
using UnityEngine;

[Serializable]
internal class ReferenceInfoData
{
	public int CoordinateType = -1;

	public int ChaReference_RefObjKey = -1;

	public string GameObjectPath = "";

	public ReferenceInfoData(int index, GameObject gameObject)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		ChaReference_RefObjKey = index;
		if ((bool)gameObject)
		{
			GameObjectPath = PmxBuilder.GetGameObjectPath(gameObject);
		}
	}
}
