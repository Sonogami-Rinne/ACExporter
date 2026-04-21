using System;
using System.Collections.Generic;

namespace PmxLib;

public class VmdMotionIPL : IBytesConvert, ICloneable
{
	public VmdIplData MoveX = new VmdIplData();

	public VmdIplData MoveY = new VmdIplData();

	public VmdIplData MoveZ = new VmdIplData();

	public VmdIplData Rotate = new VmdIplData();

	public int ByteCount => 16;

	public VmdMotionIPL()
	{
	}

	public VmdMotionIPL(VmdMotionIPL ipl)
	{
		MoveX = (VmdIplData)ipl.MoveX.Clone();
		MoveY = (VmdIplData)ipl.MoveY.Clone();
		MoveZ = (VmdIplData)ipl.MoveZ.Clone();
		Rotate = (VmdIplData)ipl.Rotate.Clone();
	}

	public byte[] ToBytes()
	{
		return new List<byte>
		{
			(byte)MoveX.P1.X,
			(byte)MoveX.P1.Y,
			(byte)MoveX.P2.X,
			(byte)MoveX.P2.Y,
			(byte)MoveY.P1.X,
			(byte)MoveY.P1.Y,
			(byte)MoveY.P2.X,
			(byte)MoveY.P2.Y,
			(byte)MoveZ.P1.X,
			(byte)MoveZ.P1.Y,
			(byte)MoveZ.P2.X,
			(byte)MoveZ.P2.Y,
			(byte)Rotate.P1.X,
			(byte)Rotate.P1.Y,
			(byte)Rotate.P2.X,
			(byte)Rotate.P2.Y
		}.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		MoveX.P1.X = bytes[startIndex];
		int num = startIndex + 1;
		MoveX.P1.Y = bytes[num];
		int num2 = num + 1;
		MoveX.P2.X = bytes[num2];
		int num3 = num2 + 1;
		MoveX.P2.Y = bytes[num3];
		int num4 = num3 + 1;
		MoveY.P1.X = bytes[num4];
		int num5 = num4 + 1;
		MoveY.P1.Y = bytes[num5];
		int num6 = num5 + 1;
		MoveY.P2.X = bytes[num6];
		int num7 = num6 + 1;
		MoveY.P2.Y = bytes[num7];
		int num8 = num7 + 1;
		MoveZ.P1.X = bytes[num8];
		int num9 = num8 + 1;
		MoveZ.P1.Y = bytes[num9];
		int num10 = num9 + 1;
		MoveZ.P2.X = bytes[num10];
		int num11 = num10 + 1;
		MoveZ.P2.Y = bytes[num11];
		int num12 = num11 + 1;
		Rotate.P1.X = bytes[num12];
		int num13 = num12 + 1;
		Rotate.P1.Y = bytes[num13];
		int num14 = num13 + 1;
		Rotate.P2.X = bytes[num14];
		int num15 = num14 + 1;
		Rotate.P2.Y = bytes[num15];
	}

	public object Clone()
	{
		return new VmdMotionIPL(this);
	}
}
