using System;
using System.Collections.Generic;
using UnityEngine;

namespace PmxLib;

public class VmdLight : VmdFrameBase, IBytesConvert, ICloneable
{
	public Color Color;

	public Vector3 Direction;

	public int ByteCount => 28;

	public VmdLight()
	{
	}

	public VmdLight(VmdLight light)
		: this()
	{
		FrameIndex = light.FrameIndex;
		Color = light.Color;
		Direction = light.Direction;
	}

	public byte[] ToBytes()
	{
		List<byte> list = new List<byte>();
		list.AddRange(BitConverter.GetBytes(FrameIndex));
		list.AddRange(BitConverter.GetBytes(Color.r));
		list.AddRange(BitConverter.GetBytes(Color.g));
		list.AddRange(BitConverter.GetBytes(Color.b));
		list.AddRange(BitConverter.GetBytes(Direction.x));
		list.AddRange(BitConverter.GetBytes(Direction.y));
		list.AddRange(BitConverter.GetBytes(Direction.z));
		return list.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		FrameIndex = BitConverter.ToInt32(bytes, startIndex);
		int num = startIndex + 4;
		Color.r = BitConverter.ToSingle(bytes, num);
		int num2 = num + 4;
		Color.g = BitConverter.ToSingle(bytes, num2);
		int num3 = num2 + 4;
		Color.b = BitConverter.ToSingle(bytes, num3);
		int num4 = num3 + 4;
		Direction.x = BitConverter.ToSingle(bytes, num4);
		int num5 = num4 + 4;
		Direction.y = BitConverter.ToSingle(bytes, num5);
		int startIndex2 = num5 + 4;
		Direction.z = BitConverter.ToSingle(bytes, startIndex2);
	}

	public object Clone()
	{
		return new VmdLight(this);
	}
}
