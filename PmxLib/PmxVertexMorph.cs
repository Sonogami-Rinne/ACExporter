using System;
using System.IO;

namespace PmxLib;

internal class PmxVertexMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public int Index;

	public Vector3 Offset;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.VertexMorph;

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

	public PmxVertex RefVertex { get; set; }

	public PmxVertexMorph()
	{
		Index = -1;
	}

	public PmxVertexMorph(int index, Vector3 offset)
		: this()
	{
		Index = index;
		Offset = offset;
	}

	public PmxVertexMorph(PmxVertexMorph sv)
		: this()
	{
		FromPmxVertexMorph(sv);
	}

	public void FromPmxVertexMorph(PmxVertexMorph sv)
	{
		Index = sv.Index;
		Offset = sv.Offset;
	}

	public override void FromStreamEx(Stream s, PmxElementFormat size)
	{
		Index = PmxStreamHelper.ReadElement_Int32(s, size.VertexSize, signed: false);
		Offset = V3_BytesConvert.FromStream(s);
	}

	public override void ToStreamEx(Stream s, PmxElementFormat size)
	{
		PmxStreamHelper.WriteElement_Int32(s, Index, size.VertexSize, signed: false);
		V3_BytesConvert.ToStream(s, Offset);
	}

	object ICloneable.Clone()
	{
		return new PmxVertexMorph(this);
	}

	public override PmxBaseMorph Clone()
	{
		return new PmxVertexMorph(this);
	}
}
