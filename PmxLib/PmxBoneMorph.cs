using System;
using System.IO;

namespace PmxLib;

internal class PmxBoneMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public int Index;

	public Vector3 Translation;

	public Quaternion Rotaion;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.BoneMorph;

	public override int BaseIndex
	{
		get
		{
			return Index;
		}
		set
		{
			Index = value;
		}
	}

	public PmxBone RefBone { get; set; }

	public PmxBoneMorph()
	{
	}

	public PmxBoneMorph(int index, Vector3 t, Quaternion r)
		: this()
	{
		Index = index;
		Translation = t;
		Rotaion = r;
	}

	public PmxBoneMorph(PmxBoneMorph sv)
		: this()
	{
		FromPmxBoneMorph(sv);
	}

	public void FromPmxBoneMorph(PmxBoneMorph sv)
	{
		Index = sv.Index;
		Translation = sv.Translation;
		Rotaion = sv.Rotaion;
	}

	public void Clear()
	{
		Translation = Vector3.zero;
		Rotaion = Quaternion.identity;
	}

	public override void FromStreamEx(Stream s, PmxElementFormat size)
	{
		Index = PmxStreamHelper.ReadElement_Int32(s, size.BoneSize, signed: true);
		Translation = V3_BytesConvert.FromStream(s);
		Vector4 vector = V4_BytesConvert.FromStream(s);
		Rotaion = new Quaternion(vector.x, vector.y, vector.z, vector.w);
	}

	public override void ToStreamEx(Stream s, PmxElementFormat size)
	{
		PmxStreamHelper.WriteElement_Int32(s, Index, size.BoneSize, signed: true);
		V3_BytesConvert.ToStream(s, Translation);
		V4_BytesConvert.ToStream(s, new Vector4(Rotaion.x, Rotaion.y, Rotaion.z, Rotaion.w));
	}

	object ICloneable.Clone()
	{
		return new PmxBoneMorph(this);
	}

	public override PmxBaseMorph Clone()
	{
		return new PmxBoneMorph(this);
	}
}
