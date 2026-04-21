using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class DynamicBoneColliderData
{
	public int CoordinateType = -1;

	public string Name = "";

	public List<float> OffsetRotation;

	public List<float> Center;

	public float Radius;

	public List<float> LossyScale;

	public float Height;

	public int Direction;

	public int Bound;

	public DynamicBoneColliderData(DynamicBoneCollider dynamicBoneCollider)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		Name = dynamicBoneCollider.name;
		Vector3 eulerAngles = dynamicBoneCollider.transform.rotation.eulerAngles;
		//OffsetRotation = new Vector3(0f - eulerAngles.x, eulerAngles.y, 0f - eulerAngles.z);
		OffsetRotation = new List<float>() { -eulerAngles.x, eulerAngles.y, -eulerAngles.z };
		Center = new List<float>() { dynamicBoneCollider.m_Center.x, dynamicBoneCollider.m_Center.y, dynamicBoneCollider.m_Center.z };
		Radius = dynamicBoneCollider.m_Radius;
		LossyScale = new List<float>() { dynamicBoneCollider.transform.lossyScale.x, dynamicBoneCollider.transform.lossyScale.y, dynamicBoneCollider.transform.lossyScale.z };
		Height = dynamicBoneCollider.m_Height;
		Direction = (int)dynamicBoneCollider.m_Direction;
		Bound = (int)dynamicBoneCollider.m_Bound;
	}
}
