using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class DynamicBoneData
{
	public int CoordinateType = -1;

	public string Root = "";

	public float Damping;

	public string DampingDistrib = "";

	public float Elasticity;

	public string ElasticityDistrib = "";

	public float Stiffness;

	public string StiffnessDistrib = "";

	public float Inert;

	public string InertDistrib = "";

	public float Radius;

	public string RadiusDistrib = "";

	public float EndLength;

	public List<float> EndOffset;

	public List<float> Gravity;

	public List<float> Force;

	public List<string> Colliders = new List<string>();

	public List<string> Exclusions = new List<string>();

	public List<string> NotRolls = new List<string>();

	public float ObjectScale;

	public float Weight;

	public List<string> ParticleBones = new List<string>();

	public List<float> ParticleBoneLength = new List<float>();

	public List<float> ParticleDamping = new List<float>();

	public List<float> ParticleElasticity = new List<float>();

	public List<List<float>> ParticleEndOffset = new List<List<float>>();

	public List<float> ParticleInert = new List<float>();

	public List<float> ParticleRadius = new List<float>();

	public List<float> ParticleStiffness = new List<float>();

	public DynamicBoneData(DynamicBone dynamicBone)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		if (dynamicBone.m_Root != null)
		{
			Root = PmxBuilder.GetAltBoneName(dynamicBone.m_Root);
		}
		Damping = dynamicBone.m_Damping;
		if (dynamicBone.m_DampingDistrib != null)
		{
			DampingDistrib = PmxBuilder.AnimationCurveToJSON(dynamicBone.m_DampingDistrib);
		}
		Elasticity = dynamicBone.m_Elasticity;
		if (dynamicBone.m_ElasticityDistrib != null)
		{
			ElasticityDistrib = PmxBuilder.AnimationCurveToJSON(dynamicBone.m_ElasticityDistrib);
		}
		Stiffness = dynamicBone.m_Stiffness;
		if (dynamicBone.m_StiffnessDistrib != null)
		{
			StiffnessDistrib = PmxBuilder.AnimationCurveToJSON(dynamicBone.m_StiffnessDistrib);
		}
		Inert = dynamicBone.m_Inert;
		if (dynamicBone.m_InertDistrib != null)
		{
			InertDistrib = PmxBuilder.AnimationCurveToJSON(dynamicBone.m_InertDistrib);
		}
		Radius = dynamicBone.m_Radius;
		if (dynamicBone.m_RadiusDistrib != null)
		{
			RadiusDistrib = PmxBuilder.AnimationCurveToJSON(dynamicBone.m_RadiusDistrib);
		}
		EndLength = dynamicBone.m_EndLength;
		EndOffset = new List<float>() { dynamicBone.m_EndOffset.x, dynamicBone.m_EndOffset.y, dynamicBone.m_EndOffset.z };
		Gravity = new List<float>() { dynamicBone.m_Gravity .x, dynamicBone.m_Gravity .y, dynamicBone.m_Gravity .z};
		Force = new List<float>() { dynamicBone.m_Force .x, dynamicBone.m_Force .y, dynamicBone.m_Force .z};
		foreach (var collider in dynamicBone.m_Colliders)
		{
			if (!(collider == null))
			{
				Colliders.Add(collider.name);
			}
		}
		foreach (Transform exclusion in dynamicBone.m_Exclusions)
		{
			if (exclusion != null)
			{
				Exclusions.Add(PmxBuilder.GetAltBoneName(exclusion));
			}
		}
		foreach (Transform notRoll in dynamicBone.m_notRolls)
		{
			if (notRoll != null)
			{
				NotRolls.Add(PmxBuilder.GetAltBoneName(notRoll));
			}
		}
		ObjectScale = dynamicBone.m_ObjectScale;
		Weight = dynamicBone.m_Weight;
		foreach (var particleTree in dynamicBone.m_ParticleTrees)
		{
			foreach (var particle in particleTree.m_Particles)
			{
				if (particle.m_Transform != null)
				{
					ParticleBones.Add(PmxBuilder.GetAltBoneName(particle.m_Transform));
				}
				else
				{
					ParticleBones.Add("NULL");
				}
				ParticleBoneLength.Add(particle.m_BoneLength);
				ParticleDamping.Add(particle.m_Damping);
				ParticleElasticity.Add(particle.m_Elasticity);
				if (particle.m_ParentIndex >= 1)
				{
					Transform transform =particleTree.m_Particles[particle.m_ParentIndex].m_Transform;
					var tmp = transform.rotation * particle.m_EndOffset;
					ParticleEndOffset.Add(new List<float>() { tmp.x, tmp.y, tmp.z });
				}
				else
				{
					ParticleEndOffset.Add(new List<float>() { particle.m_EndOffset.x, particle.m_EndOffset.y, particle.m_EndOffset.z });
				}
				ParticleInert.Add(particle.m_Inert);
				ParticleRadius.Add(particle.m_Radius);
				ParticleStiffness.Add(particle.m_Stiffness);
			}
        }
	}

	//public DynamicBoneData(DynamicBone_Ver01 bone)
	//{
	//	if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
	//	{
	//		CoordinateType = PmxBuilder.nowCoordinate;
	//	}
	//	if (bone.m_Root != null)
	//	{
	//		Root = PmxBuilder.GetAltBoneName(bone.m_Root);
	//	}
	//}

	public DynamicBoneData(DynamicBone_Ver02 bone)
	{
		if (PmxBuilder.nowCoordinate < PmxBuilder.maxCoord)
		{
			CoordinateType = PmxBuilder.nowCoordinate;
		}
		if (bone.Root != null)
		{
			Root = PmxBuilder.GetAltBoneName(bone.Root);
		}
		foreach (Transform bone2 in bone.Bones)
		{
			if (bone2 != null)
			{
				ParticleBones.Add(PmxBuilder.GetAltBoneName(bone2));
			}
		}
	}
}
