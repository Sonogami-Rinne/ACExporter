using System;
using System.IO;

namespace PmxLib;

internal class PmxImpulseMorph : PmxBaseMorph, IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public int Index;

	public bool Local;

	public Vector3 Velocity;

	public Vector3 Torque;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.ImpulseMorph;

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

	public PmxBody RefBody { get; set; }

	public bool ZeroFlag { get; set; }

	public PmxImpulseMorph()
	{
		Local = false;
		Velocity = Vector3.zero;
		Torque = Vector3.zero;
	}

	public PmxImpulseMorph(int index, bool local, Vector3 t, Vector3 r)
	{
		Index = index;
		Local = local;
		Velocity = t;
		Torque = r;
	}

	public PmxImpulseMorph(PmxImpulseMorph sv)
		: this()
	{
		FromPmxImpulseMorph(sv);
	}

	public void FromPmxImpulseMorph(PmxImpulseMorph sv)
	{
		Index = sv.Index;
		Local = sv.Local;
		Velocity = sv.Velocity;
		Torque = sv.Torque;
		ZeroFlag = sv.ZeroFlag;
	}

	public bool UpdateZeroFlag()
	{
		ZeroFlag = Velocity == Vector3.zero && Torque == Vector3.zero;
		return ZeroFlag;
	}

	public void Clear()
	{
		Velocity = Vector3.zero;
		Torque = Vector3.zero;
	}

	public override void FromStreamEx(Stream s, PmxElementFormat size)
	{
		Index = PmxStreamHelper.ReadElement_Int32(s, size.BodySize, signed: true);
		Local = s.ReadByte() != 0;
		Velocity = V3_BytesConvert.FromStream(s);
		Torque = V3_BytesConvert.FromStream(s);
	}

	public override void ToStreamEx(Stream s, PmxElementFormat size)
	{
		PmxStreamHelper.WriteElement_Int32(s, Index, size.BodySize, signed: true);
		s.WriteByte((byte)(Local ? 1 : 0));
		V3_BytesConvert.ToStream(s, Velocity);
		V3_BytesConvert.ToStream(s, Torque);
	}

	object ICloneable.Clone()
	{
		return new PmxImpulseMorph(this);
	}

	public override PmxBaseMorph Clone()
	{
		return new PmxImpulseMorph(this);
	}
}
