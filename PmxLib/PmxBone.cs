using System;
using System.IO;

namespace PmxLib;

public class PmxBone : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
{
	[Flags]
	public enum BoneFlags
	{
		None = 0,
		ToBone = 1,
		Rotation = 2,
		Translation = 4,
		Visible = 8,
		Enable = 0x10,
		IK = 0x20,
		AppendLocal = 0x80,
		AppendRotation = 0x100,
		AppendTranslation = 0x200,
		FixAxis = 0x400,
		LocalFrame = 0x800,
		AfterPhysics = 0x1000,
		ExtParent = 0x2000
	}

	public enum IKKindType
	{
		None,
		IK,
		Target,
		Link
	}

	public BoneFlags Flags;

	public int Parent;

	public int To_Bone;

	public Vector3 To_Offset;

	public Vector3 Position;

	public int Level;

	public int AppendParent;

	public float AppendRatio;

	public Vector3 Axis;

	public Vector3 LocalX;

	public Vector3 LocalY;

	public Vector3 LocalZ;

	public int ExtKey;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Bone;

	public string Name { get; set; }

	public string NameE { get; set; }

	public PmxBone RefParent { get; set; }

	public PmxBone RefTo_Bone { get; set; }

	public PmxBone RefAppendParent { get; set; }

	public PmxIK IK { get; private set; }

	public IKKindType IKKind { get; set; }

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

	public void ClearFlags()
	{
		Flags = BoneFlags.Rotation | BoneFlags.Visible | BoneFlags.Enable;
	}

	public bool GetFlag(BoneFlags f)
	{
		return (f & Flags) == f;
	}

	public void SetFlag(BoneFlags f, bool val)
	{
		if (val)
		{
			Flags |= f;
		}
		else
		{
			Flags &= ~f;
		}
	}

	public void ClearLocal()
	{
		LocalX = new Vector3(1f, 0f, 0f);
		LocalY = new Vector3(0f, 1f, 0f);
		LocalZ = new Vector3(0f, 0f, 1f);
	}

	public void NormalizeLocal()
	{
		LocalZ.Normalize();
		LocalX.Normalize();
		LocalY = Vector3.Cross(LocalZ, LocalX);
		LocalZ = Vector3.Cross(LocalX, LocalY);
		LocalY.Normalize();
		LocalZ.Normalize();
	}

	public PmxBone()
	{
		Name = "";
		NameE = "";
		ClearFlags();
		Parent = -1;
		To_Bone = -1;
		To_Offset = Vector3.zero;
		AppendParent = -1;
		AppendRatio = 1f;
		Level = 0;
		ClearLocal();
		IK = new PmxIK();
		IKKind = IKKindType.None;
	}

	public PmxBone(PmxBone bone, bool nonStr)
	{
		FromPmxBone(bone, nonStr);
	}

	public void FromPmxBone(PmxBone bone, bool nonStr)
	{
		if (!nonStr)
		{
			Name = bone.Name;
			NameE = bone.NameE;
		}
		Flags = bone.Flags;
		Parent = bone.Parent;
		To_Bone = bone.To_Bone;
		To_Offset = bone.To_Offset;
		Position = bone.Position;
		Level = bone.Level;
		AppendParent = bone.AppendParent;
		AppendRatio = bone.AppendRatio;
		Axis = bone.Axis;
		LocalX = bone.LocalX;
		LocalY = bone.LocalY;
		LocalZ = bone.LocalZ;
		ExtKey = bone.ExtKey;
		IK = bone.IK.Clone();
		IKKind = bone.IKKind;
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		Position = V3_BytesConvert.FromStream(s);
		Parent = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
		Level = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		Flags = (BoneFlags)PmxStreamHelper.ReadElement_Int32(s, 2, signed: false);
		if (GetFlag(BoneFlags.ToBone))
		{
			To_Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
		}
		else
		{
			To_Offset = V3_BytesConvert.FromStream(s);
		}
		if (GetFlag(BoneFlags.AppendRotation) || GetFlag(BoneFlags.AppendTranslation))
		{
			AppendParent = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
			AppendRatio = PmxStreamHelper.ReadElement_Float(s);
		}
		if (GetFlag(BoneFlags.FixAxis))
		{
			Axis = V3_BytesConvert.FromStream(s);
		}
		if (GetFlag(BoneFlags.LocalFrame))
		{
			LocalX = V3_BytesConvert.FromStream(s);
			LocalZ = V3_BytesConvert.FromStream(s);
			NormalizeLocal();
		}
		if (GetFlag(BoneFlags.ExtParent))
		{
			ExtKey = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		}
		if (GetFlag(BoneFlags.IK))
		{
			IK.FromStreamEx(s, f);
		}
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		V3_BytesConvert.ToStream(s, Position);
		PmxStreamHelper.WriteElement_Int32(s, Parent, f.BoneSize, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, Level, 4, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, (int)Flags, 2, signed: false);
		if (GetFlag(BoneFlags.ToBone))
		{
			PmxStreamHelper.WriteElement_Int32(s, To_Bone, f.BoneSize, signed: true);
		}
		else
		{
			V3_BytesConvert.ToStream(s, To_Offset);
		}
		if (GetFlag(BoneFlags.AppendRotation) || GetFlag(BoneFlags.AppendTranslation))
		{
			PmxStreamHelper.WriteElement_Int32(s, AppendParent, f.BoneSize, signed: true);
			PmxStreamHelper.WriteElement_Float(s, AppendRatio);
		}
		if (GetFlag(BoneFlags.FixAxis))
		{
			V3_BytesConvert.ToStream(s, Axis);
		}
		if (GetFlag(BoneFlags.LocalFrame))
		{
			NormalizeLocal();
			V3_BytesConvert.ToStream(s, LocalX);
			V3_BytesConvert.ToStream(s, LocalZ);
		}
		if (GetFlag(BoneFlags.ExtParent))
		{
			PmxStreamHelper.WriteElement_Int32(s, ExtKey, 4, signed: true);
		}
		if (GetFlag(BoneFlags.IK))
		{
			IK.ToStreamEx(s, f);
		}
	}

	object ICloneable.Clone()
	{
		return new PmxBone(this, nonStr: false);
	}

	public PmxBone Clone()
	{
		return new PmxBone(this, nonStr: false);
	}
}
