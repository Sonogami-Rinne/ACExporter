using System;
using System.Collections.Generic;

namespace PmxLib;

public class VmdMorph : VmdFrameBase, IBytesConvert, ICloneable
{
	private const int NameBytes = 15;

	public string Name = "";

	public float Value;

	public int ByteCount => 23;

	public VmdMorph()
	{
	}

	public VmdMorph(VmdMorph skin)
	{
		Name = skin.Name;
		FrameIndex = skin.FrameIndex;
		Value = skin.Value;
	}

	public byte[] ToBytes()
	{
		List<byte> list = new List<byte>();
		byte[] array = new byte[15];
		BytesStringProc.SetString(array, Name, 0, 253);
		list.AddRange(array);
		list.AddRange(BitConverter.GetBytes(FrameIndex));
		list.AddRange(BitConverter.GetBytes(Value));
		return list.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		byte[] array = new byte[15];
		Array.Copy(bytes, startIndex, array, 0, 15);
		Name = BytesStringProc.GetString(array, 0);
		int num = startIndex + 15;
		FrameIndex = BitConverter.ToInt32(bytes, num);
		int startIndex2 = num + 4;
		Value = BitConverter.ToSingle(bytes, startIndex2);
	}

	public object Clone()
	{
		return new VmdMorph(this);
	}
}
