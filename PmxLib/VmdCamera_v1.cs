using System;
using System.Collections.Generic;

namespace PmxLib;

internal class VmdCamera_v1 : VmdFrameBase, IBytesConvert, ICloneable
{
	public float Distance;

	public Vector3 Position;

	public Vector3 Rotate;

	public VmdIplData CameraIpl = new VmdIplData();

	public int ByteCount => 36;

	public VmdCamera_v1()
	{
	}

	public VmdCamera_v1(VmdCamera_v1 camera)
	{
		FrameIndex = camera.FrameIndex;
		Distance = camera.Distance;
		Position = camera.Position;
		Rotate = camera.Rotate;
		CameraIpl = camera.CameraIpl;
	}

	public VmdCamera ToVmdCamera()
	{
		return new VmdCamera
		{
			FrameIndex = FrameIndex,
			Distance = Distance,
			Position = Position,
			Rotate = Rotate,
			IPL = 
			{
				MoveX = new VmdIplData(CameraIpl),
				MoveY = new VmdIplData(CameraIpl),
				MoveZ = new VmdIplData(CameraIpl),
				Rotate = new VmdIplData(CameraIpl),
				Distance = new VmdIplData(CameraIpl),
				Angle = new VmdIplData(CameraIpl)
			},
			Angle = 45f,
			Pers = 0
		};
	}

	public byte[] ToBytes()
	{
		List<byte> list = new List<byte>();
		list.AddRange(BitConverter.GetBytes(FrameIndex));
		list.AddRange(BitConverter.GetBytes(Distance));
		list.AddRange(BitConverter.GetBytes(Position.x));
		list.AddRange(BitConverter.GetBytes(Position.y));
		list.AddRange(BitConverter.GetBytes(Position.z));
		list.AddRange(BitConverter.GetBytes(Rotate.x));
		list.AddRange(BitConverter.GetBytes(Rotate.y));
		list.AddRange(BitConverter.GetBytes(Rotate.z));
		list.Add((byte)CameraIpl.P1.X);
		list.Add((byte)CameraIpl.P2.X);
		list.Add((byte)CameraIpl.P1.Y);
		list.Add((byte)CameraIpl.P2.Y);
		return list.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		FrameIndex = BitConverter.ToInt32(bytes, startIndex);
		int num = startIndex + 4;
		Distance = BitConverter.ToSingle(bytes, num);
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
		CameraIpl.P1.X = bytes[num8];
		int num9 = num8 + 1;
		CameraIpl.P1.Y = bytes[num9];
		int num10 = num9 + 1;
		CameraIpl.P2.X = bytes[num10];
		int num11 = num10 + 1;
		CameraIpl.P2.Y = bytes[num11];
	}

	public object Clone()
	{
		return new VmdCamera_v1(this);
	}
}
