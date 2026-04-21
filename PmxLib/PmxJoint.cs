using System;
using System.IO;

namespace PmxLib;

public class PmxJoint : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
{
	public enum JointKind
	{
		Sp6DOF,
		G6DOF,
		P2P,
		ConeTwist,
		Slider,
		Hinge
	}

	public JointKind Kind;

	public int BodyA;

	public int BodyB;

	public Vector3 Position;

	public Vector3 Rotation;

	public Vector3 Limit_MoveLow;

	public Vector3 Limit_MoveHigh;

	public Vector3 Limit_AngleLow;

	public Vector3 Limit_AngleHigh;

	public Vector3 SpConst_Move;

	public Vector3 SpConst_Rotate;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Joint;

	public string Name { get; set; }

	public string NameE { get; set; }

	public PmxBody RefBodyA { get; set; }

	public PmxBody RefBodyB { get; set; }

	public string NXName
	{
		get
		{
			return Name;
		}
		set
		{
			Name = value;
		}
	}

	public PmxJoint()
	{
		Name = "";
		NameE = "";
		BodyA = -1;
		BodyB = -1;
	}

	public PmxJoint(PmxJoint joint, bool nonStr)
	{
		FromPmxJoint(joint, nonStr);
	}

	public void FromPmxJoint(PmxJoint joint, bool nonStr)
	{
		if (!nonStr)
		{
			Name = joint.Name;
			NameE = joint.NameE;
		}
		Kind = joint.Kind;
		BodyA = joint.BodyA;
		BodyB = joint.BodyB;
		Position = joint.Position;
		Rotation = joint.Rotation;
		Limit_MoveLow = joint.Limit_MoveLow;
		Limit_MoveHigh = joint.Limit_MoveHigh;
		Limit_AngleLow = joint.Limit_AngleLow;
		Limit_AngleHigh = joint.Limit_AngleHigh;
		SpConst_Move = joint.SpConst_Move;
		SpConst_Rotate = joint.SpConst_Rotate;
	}

	public void ClearLimit()
	{
		Limit_AngleLow = Vector3.zero;
		Limit_AngleHigh = Vector3.zero;
		if (Kind == JointKind.ConeTwist)
		{
			Limit_MoveLow.y = 0f;
			Limit_MoveHigh.y = 0f;
		}
		else
		{
			Limit_MoveLow = Vector3.zero;
			Limit_MoveHigh = Vector3.zero;
		}
	}

	public void ClearParameter()
	{
		switch (Kind)
		{
		case JointKind.Sp6DOF:
		case JointKind.G6DOF:
		case JointKind.P2P:
		case JointKind.Slider:
			SpConst_Move = Vector3.zero;
			SpConst_Rotate = Vector3.zero;
			break;
		case JointKind.ConeTwist:
			Limit_MoveLow.x = 0f;
			Limit_MoveHigh.x = 1f;
			Limit_MoveLow.z = 0f;
			Limit_MoveHigh.z = 0f;
			SpConst_Move = new Vector3(1f, 0.3f, 1f);
			SpConst_Rotate = Vector3.zero;
			break;
		case JointKind.Hinge:
			SpConst_Move = new Vector3(0.9f, 0.3f, 1f);
			SpConst_Rotate = Vector3.zero;
			break;
		}
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		Kind = (JointKind)s.ReadByte();
		BodyA = PmxStreamHelper.ReadElement_Int32(s, f.BodySize, signed: true);
		BodyB = PmxStreamHelper.ReadElement_Int32(s, f.BodySize, signed: true);
		Position = V3_BytesConvert.FromStream(s);
		Rotation = V3_BytesConvert.FromStream(s);
		Limit_MoveLow = V3_BytesConvert.FromStream(s);
		Limit_MoveHigh = V3_BytesConvert.FromStream(s);
		Limit_AngleLow = V3_BytesConvert.FromStream(s);
		Limit_AngleHigh = V3_BytesConvert.FromStream(s);
		SpConst_Move = V3_BytesConvert.FromStream(s);
		SpConst_Rotate = V3_BytesConvert.FromStream(s);
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		if (Kind != 0 && (double)f.Ver < 2.09999990463257)
		{
			s.WriteByte(0);
		}
		else
		{
			s.WriteByte((byte)Kind);
		}
		PmxStreamHelper.WriteElement_Int32(s, BodyA, f.BodySize, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, BodyB, f.BodySize, signed: true);
		V3_BytesConvert.ToStream(s, Position);
		V3_BytesConvert.ToStream(s, Rotation);
		V3_BytesConvert.ToStream(s, Limit_MoveLow);
		V3_BytesConvert.ToStream(s, Limit_MoveHigh);
		V3_BytesConvert.ToStream(s, Limit_AngleLow);
		V3_BytesConvert.ToStream(s, Limit_AngleHigh);
		V3_BytesConvert.ToStream(s, SpConst_Move);
		V3_BytesConvert.ToStream(s, SpConst_Rotate);
	}

	object ICloneable.Clone()
	{
		return new PmxJoint(this, nonStr: false);
	}

	public PmxJoint Clone()
	{
		return new PmxJoint(this, nonStr: false);
	}
}
