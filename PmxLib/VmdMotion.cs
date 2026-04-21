using System;
using System.Collections.Generic;

namespace PmxLib;

public class VmdMotion : VmdFrameBase, IBytesConvert, ICloneable
{
	private const int NameBytes = 15;

	private const ushort PhysicsOffNum = 3939;

	public string Name = "";

	public Vector3 Position;

	public Quaternion Rotate = Quaternion.identity;

	public VmdMotionIPL IPL = new VmdMotionIPL();

	protected int NoDataCount = 48;

	public bool PhysicsOff { get; set; }

	public int ByteCount => 47 + IPL.ByteCount + NoDataCount;

	public VmdMotion()
	{
		PhysicsOff = false;
	}

	public VmdMotion(VmdMotion motion)
		: this()
	{
		Name = motion.Name;
		FrameIndex = motion.FrameIndex;
		Position = motion.Position;
		Rotate = motion.Rotate;
		IPL = (VmdMotionIPL)motion.IPL.Clone();
		PhysicsOff = motion.PhysicsOff;
	}

	public byte[] ToBytes()
	{
		List<byte> list = new List<byte>();
		byte[] array = new byte[15];
		BytesStringProc.SetString(array, Name, 0, 253);
		list.AddRange(array);
		list.AddRange(BitConverter.GetBytes(FrameIndex));
		list.AddRange(BitConverter.GetBytes(Position.x));
		list.AddRange(BitConverter.GetBytes(Position.y));
		list.AddRange(BitConverter.GetBytes(Position.z));
		list.AddRange(BitConverter.GetBytes(Rotate.x));
		list.AddRange(BitConverter.GetBytes(Rotate.y));
		list.AddRange(BitConverter.GetBytes(Rotate.z));
		list.AddRange(BitConverter.GetBytes(Rotate.w));
		byte[] array2 = new byte[IPL.ByteCount * 4];
		byte[] array3 = IPL.ToBytes();
		int num = array3.Length;
		for (int i = 0; i < num; i++)
		{
			array2[i * 4] = array3[i];
		}
		if (PhysicsOff)
		{
			byte[] bytes = BitConverter.GetBytes(3939);
			array2[2] = bytes[0];
			array2[3] = bytes[1];
		}
		list.AddRange(array2);
		return list.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		byte[] array = new byte[15];
		Array.Copy(bytes, startIndex, array, 0, 15);
		Name = BytesStringProc.GetString(array, 0);
		int num = startIndex + 15;
		FrameIndex = BitConverter.ToInt32(bytes, num);
		int num2 = num + 4;
		Position.x = BitConverter.ToSingle(bytes, num2);
		int num3 = num2 + 4;
		Position.y = BitConverter.ToSingle(bytes, num3);
		int num4 = num3 + 4;
		Position.z = BitConverter.ToSingle(bytes, num4);
		int num5 = num4 + 4;
		Rotate.x = BitConverter.ToSingle(bytes, num5);
		int num6 = num5 + 4;
		Rotate.y = BitConverter.ToSingle(bytes, num6);
		int num7 = num6 + 4;
		Rotate.z = BitConverter.ToSingle(bytes, num7);
		int num8 = num7 + 4;
		Rotate.w = BitConverter.ToSingle(bytes, num8);
		int sourceIndex = num8 + 4;
		int byteCount = IPL.ByteCount;
		byte[] array2 = new byte[byteCount * 4];
		byte[] array3 = new byte[IPL.ByteCount];
		Array.Copy(bytes, sourceIndex, array2, 0, array2.Length);
		PhysicsOff = BitConverter.ToUInt16(array2, 2) == 3939;
		for (int i = 0; i < byteCount; i++)
		{
			array3[i] = array2[i * 4];
		}
		IPL.FromBytes(array3, 0);
	}

	public object Clone()
	{
		return new VmdMotion(this);
	}
}
