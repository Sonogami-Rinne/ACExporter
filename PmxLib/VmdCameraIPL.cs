using System;
using System.Collections.Generic;

namespace PmxLib;

public class VmdCameraIPL : IBytesConvert, ICloneable
{
	public VmdIplData MoveX = new VmdIplData();

	public VmdIplData MoveY = new VmdIplData();

	public VmdIplData MoveZ = new VmdIplData();

	public VmdIplData Rotate = new VmdIplData();

	public VmdIplData Distance = new VmdIplData();

	public VmdIplData Angle = new VmdIplData();

	public int ByteCount => 24;

	public VmdCameraIPL()
	{
	}

	public VmdCameraIPL(VmdCameraIPL ipl)
	{
		MoveX = (VmdIplData)ipl.MoveX.Clone();
		MoveY = (VmdIplData)ipl.MoveY.Clone();
		MoveZ = (VmdIplData)ipl.MoveZ.Clone();
		Rotate = (VmdIplData)ipl.Rotate.Clone();
		Distance = (VmdIplData)ipl.Distance.Clone();
		Angle = (VmdIplData)ipl.Angle.Clone();
	}

	public byte[] ToBytes()
	{
		return new List<byte>
		{
			(byte)MoveX.P1.X,
			(byte)MoveX.P2.X,
			(byte)MoveX.P1.Y,
			(byte)MoveX.P2.Y,
			(byte)MoveY.P1.X,
			(byte)MoveY.P2.X,
			(byte)MoveY.P1.Y,
			(byte)MoveY.P2.Y,
			(byte)MoveZ.P1.X,
			(byte)MoveZ.P2.X,
			(byte)MoveZ.P1.Y,
			(byte)MoveZ.P2.Y,
			(byte)Rotate.P1.X,
			(byte)Rotate.P2.X,
			(byte)Rotate.P1.Y,
			(byte)Rotate.P2.Y,
			(byte)Distance.P1.X,
			(byte)Distance.P2.X,
			(byte)Distance.P1.Y,
			(byte)Distance.P2.Y,
			(byte)Angle.P1.X,
			(byte)Angle.P2.X,
			(byte)Angle.P1.Y,
			(byte)Angle.P2.Y
		}.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		MoveX.P1.X = bytes[startIndex];
		int num = startIndex + 1;
		MoveX.P2.X = bytes[num];
		int num2 = num + 1;
		MoveX.P1.Y = bytes[num2];
		int num3 = num2 + 1;
		MoveX.P2.Y = bytes[num3];
		int num4 = num3 + 1;
		MoveY.P1.X = bytes[num4];
		int num5 = num4 + 1;
		MoveY.P2.X = bytes[num5];
		int num6 = num5 + 1;
		MoveY.P1.Y = bytes[num6];
		int num7 = num6 + 1;
		MoveY.P2.Y = bytes[num7];
		int num8 = num7 + 1;
		MoveZ.P1.X = bytes[num8];
		int num9 = num8 + 1;
		MoveZ.P2.X = bytes[num9];
		int num10 = num9 + 1;
		MoveZ.P1.Y = bytes[num10];
		int num11 = num10 + 1;
		MoveZ.P2.Y = bytes[num11];
		int num12 = num11 + 1;
		Rotate.P1.X = bytes[num12];
		int num13 = num12 + 1;
		Rotate.P2.X = bytes[num13];
		int num14 = num13 + 1;
		Rotate.P1.Y = bytes[num14];
		int num15 = num14 + 1;
		Rotate.P2.Y = bytes[num15];
		int num16 = num15 + 1;
		Distance.P1.X = bytes[num16];
		int num17 = num16 + 1;
		Distance.P2.X = bytes[num17];
		int num18 = num17 + 1;
		Distance.P1.Y = bytes[num18];
		int num19 = num18 + 1;
		Distance.P2.Y = bytes[num19];
		int num20 = num19 + 1;
		Angle.P1.X = bytes[num20];
		int num21 = num20 + 1;
		Angle.P2.X = bytes[num21];
		int num22 = num21 + 1;
		Angle.P1.Y = bytes[num22];
		int num23 = num22 + 1;
		Angle.P2.Y = bytes[num23];
	}

	public object Clone()
	{
		return new VmdCameraIPL(this);
	}
}
