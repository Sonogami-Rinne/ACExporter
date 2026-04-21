using System;
using System.IO;

namespace PmxLib;

internal class PmxGroupMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public int Index;

	public float Ratio;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.GroupMorph;

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

	public PmxMorph RefMorph { get; set; }

	public PmxGroupMorph()
	{
		Ratio = 1f;
	}

	public PmxGroupMorph(int index, float r)
		: this()
	{
		Index = index;
		Ratio = r;
	}

	public PmxGroupMorph(PmxGroupMorph sv)
		: this()
	{
		FromPmxGroupMorph(sv);
	}

	public void FromPmxGroupMorph(PmxGroupMorph sv)
	{
		Index = sv.Index;
		Ratio = sv.Ratio;
	}

	public override void FromStreamEx(Stream s, PmxElementFormat size)
	{
		Index = PmxStreamHelper.ReadElement_Int32(s, size.MorphSize, signed: true);
		Ratio = PmxStreamHelper.ReadElement_Float(s);
	}

	public override void ToStreamEx(Stream s, PmxElementFormat size)
	{
		PmxStreamHelper.WriteElement_Int32(s, Index, size.MorphSize, signed: true);
		PmxStreamHelper.WriteElement_Float(s, Ratio);
	}

	object ICloneable.Clone()
	{
		return new PmxGroupMorph(this);
	}

	public override PmxBaseMorph Clone()
	{
		return new PmxGroupMorph(this);
	}
}
