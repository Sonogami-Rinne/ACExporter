using System;
using System.IO;

namespace PmxLib;

public class PmxBody : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
{
	public enum BoxKind
	{
		Sphere,
		Box,
		Capsule
	}

	public enum ModeType
	{
		Static,
		Dynamic,
		DynamicWithBone
	}

	public int Bone;

	public static string NullBoneName = "-";

	public int Group;

	public PmxBodyPassGroup PassGroup;

	public Vector3 BoxSize;

	public Vector3 Position;

	public Vector3 Rotation;

	public float Mass;

	public float PositionDamping;

	public float RotationDamping;

	public float Restitution;

	public float Friction;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Body;

	public string Name { get; set; }

	public string NameE { get; set; }

	public PmxBone RefBone { get; set; }

	public BoxKind BoxType { get; set; }

	public ModeType Mode { get; set; }

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

	public PmxBody()
	{
		Name = "";
		NameE = "";
		PassGroup = new PmxBodyPassGroup();
		InitializeParameter();
		BoxSize = new Vector3(2f, 2f, 2f);
	}

	public PmxBody(PmxBody body, bool nonStr)
	{
		FromPmxBody(body, nonStr);
	}

	public void FromPmxBody(PmxBody body, bool nonStr)
	{
		if (!nonStr)
		{
			Name = body.Name;
			NameE = body.NameE;
		}
		Bone = body.Bone;
		Group = body.Group;
		PassGroup = body.PassGroup.Clone();
		BoxType = body.BoxType;
		BoxSize = body.BoxSize;
		Position = body.Position;
		Rotation = body.Rotation;
		Mass = body.Mass;
		PositionDamping = body.PositionDamping;
		RotationDamping = body.RotationDamping;
		Restitution = body.Restitution;
		Friction = body.Friction;
		Mode = body.Mode;
	}

	public void InitializeParameter()
	{
		Mass = 1f;
		PositionDamping = 0.5f;
		RotationDamping = 0.5f;
		Restitution = 0f;
		Friction = 0.5f;
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
		Group = PmxStreamHelper.ReadElement_Int32(s, 1, signed: true);
		PassGroup.FromFlagBits((ushort)PmxStreamHelper.ReadElement_Int32(s, 2, signed: false));
		BoxType = (BoxKind)s.ReadByte();
		BoxSize = V3_BytesConvert.FromStream(s);
		Position = V3_BytesConvert.FromStream(s);
		Rotation = V3_BytesConvert.FromStream(s);
		Mass = PmxStreamHelper.ReadElement_Float(s);
		Vector4 vector = V4_BytesConvert.FromStream(s);
		PositionDamping = vector.x;
		RotationDamping = vector.y;
		Restitution = vector.z;
		Friction = vector.w;
		Mode = (ModeType)s.ReadByte();
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		PmxStreamHelper.WriteElement_Int32(s, Bone, f.BoneSize, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, Group, 1, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, PassGroup.ToFlagBits(), 2, signed: false);
		s.WriteByte((byte)BoxType);
		V3_BytesConvert.ToStream(s, BoxSize);
		V3_BytesConvert.ToStream(s, Position);
		V3_BytesConvert.ToStream(s, Rotation);
		PmxStreamHelper.WriteElement_Float(s, Mass);
		V4_BytesConvert.ToStream(s, new Vector4(PositionDamping, RotationDamping, Restitution, Friction));
		s.WriteByte((byte)Mode);
	}

	object ICloneable.Clone()
	{
		return new PmxBody(this, nonStr: false);
	}

	public PmxBody Clone()
	{
		return new PmxBody(this, nonStr: false);
	}
}
