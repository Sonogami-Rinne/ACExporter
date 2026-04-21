using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib;

public class PmxIK : IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public class IKLink : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public enum EulerType
		{
			ZXY,
			XYZ,
			YZX
		}

		public enum FixAxisType
		{
			None,
			Fix,
			X,
			Y,
			Z
		}

		public int Bone;

		public bool IsLimit;

		public Vector3 Low;

		public Vector3 High;

		public EulerType Euler;

		public FixAxisType FixAxis;

		PmxObject IPmxObjectKey.ObjectKey => PmxObject.IKLink;

		public PmxBone RefBone { get; set; }

		public IKLink()
		{
			Bone = -1;
			IsLimit = false;
		}

		public IKLink(IKLink link)
		{
			FromIKLink(link);
		}

		public void FromIKLink(IKLink link)
		{
			Bone = link.Bone;
			IsLimit = link.IsLimit;
			Low = link.Low;
			High = link.High;
			Euler = link.Euler;
		}

		public void NormalizeAngle()
		{
			Vector3 low = default(Vector3);
			Low.x = Math.Min(Low.x, High.x);
			Vector3 high = default(Vector3);
			High.x = Math.Max(Low.x, High.x);
			Low.y = Math.Min(Low.y, High.y);
			High.y = Math.Max(Low.y, High.y);
			Low.z = Math.Min(Low.z, High.z);
			High.z = Math.Max(Low.z, High.z);
			Low = low;
			High = high;
		}

		public void NormalizeEulerAxis()
		{
			Euler = ((-1.57079637050629 >= (double)Low.x || (double)High.x >= 1.57079637050629) ? ((!(-1.57079637050629 >= (double)Low.y) && !((double)High.y >= 1.57079637050629)) ? EulerType.XYZ : EulerType.YZX) : EulerType.ZXY);
			FixAxis = FixAxisType.None;
			if ((double)Low.x == 0.0 && (double)High.x == 0.0 && (double)Low.y == 0.0 && (double)High.y == 0.0 && (double)Low.z == 0.0 && (double)High.z == 0.0)
			{
				FixAxis = FixAxisType.Fix;
			}
			else if ((double)Low.y == 0.0 && (double)High.y == 0.0 && (double)Low.z == 0.0 && (double)High.z == 0.0)
			{
				FixAxis = FixAxisType.X;
			}
			else if ((double)Low.x == 0.0 && (double)High.x == 0.0 && (double)Low.z == 0.0 && (double)High.z == 0.0)
			{
				FixAxis = FixAxisType.Y;
			}
			else if ((double)Low.x == 0.0 && (double)High.x == 0.0 && (double)Low.y == 0.0 && (double)High.y == 0.0)
			{
				FixAxis = FixAxisType.Z;
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f)
		{
			Bone = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
			IsLimit = s.ReadByte() != 0;
			if (IsLimit)
			{
				Low = V3_BytesConvert.FromStream(s);
				High = V3_BytesConvert.FromStream(s);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f)
		{
			PmxStreamHelper.WriteElement_Int32(s, Bone, f.BoneSize, signed: true);
			s.WriteByte((byte)(IsLimit ? 1 : 0));
			if (IsLimit)
			{
				V3_BytesConvert.ToStream(s, Low);
				V3_BytesConvert.ToStream(s, High);
			}
		}

		object ICloneable.Clone()
		{
			return new IKLink(this);
		}

		public IKLink Clone()
		{
			return new IKLink(this);
		}
	}

	public int Target;

	public int LoopCount;

	public float Angle;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.IK;

	public PmxBone RefTarget { get; set; }

	public List<IKLink> LinkList { get; private set; }

	public PmxIK()
	{
		Target = -1;
		LoopCount = 0;
		Angle = 1f;
		LinkList = new List<IKLink>();
	}

	public PmxIK(PmxIK ik)
	{
		FromPmxIK(ik);
	}

	public void FromPmxIK(PmxIK ik)
	{
		Target = ik.Target;
		LoopCount = ik.LoopCount;
		Angle = ik.Angle;
		LinkList = new List<IKLink>();
		for (int i = 0; i < ik.LinkList.Count; i++)
		{
			LinkList.Add(ik.LinkList[i].Clone());
		}
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Target = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
		LoopCount = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		Angle = PmxStreamHelper.ReadElement_Float(s);
		int num = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		LinkList.Clear();
		LinkList.Capacity = num;
		for (int i = 0; i < num; i++)
		{
			IKLink iKLink = new IKLink();
			iKLink.FromStreamEx(s, f);
			LinkList.Add(iKLink);
		}
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteElement_Int32(s, Target, f.BoneSize, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, LoopCount, 4, signed: true);
		PmxStreamHelper.WriteElement_Float(s, Angle);
		PmxStreamHelper.WriteElement_Int32(s, LinkList.Count, 4, signed: true);
		for (int i = 0; i < LinkList.Count; i++)
		{
			LinkList[i].ToStreamEx(s, f);
		}
	}

	object ICloneable.Clone()
	{
		return new PmxIK(this);
	}

	public PmxIK Clone()
	{
		return new PmxIK(this);
	}
}
