using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PmxLib;

internal static class V4_BytesConvert
{
	public static readonly int UnitBytes = 16;

	public static int ByteCount => UnitBytes;

	public static byte[] ToBytes(Vector4 v4)
	{
		List<byte> list = new List<byte>();
		list.AddRange(BitConverter.GetBytes(v4.x));
		list.AddRange(BitConverter.GetBytes(v4.y));
		list.AddRange(BitConverter.GetBytes(v4.z));
		list.AddRange(BitConverter.GetBytes(v4.w));
		return list.ToArray();
	}

	public static Vector4 FromBytes(byte[] bytes, int startIndex)
	{
		int num = 4;
		return new Vector4(BitConverter.ToSingle(bytes, startIndex), BitConverter.ToSingle(bytes, startIndex + num), BitConverter.ToSingle(bytes, startIndex + num * 2), BitConverter.ToSingle(bytes, startIndex + num * 3));
	}

	public static Vector4 FromStream(Stream s)
	{
		Vector4 zero = Vector4.zero;
		byte[] array = new byte[16];
		s.Read(array, 0, 16);
		int num = 0;
		zero.x = BitConverter.ToSingle(array, num);
		int num2 = num + 4;
		zero.y = BitConverter.ToSingle(array, num2);
		int num3 = num2 + 4;
		zero.z = BitConverter.ToSingle(array, num3);
		int startIndex = num3 + 4;
		zero.w = BitConverter.ToSingle(array, startIndex);
		return zero;
	}

	public static void ToStream(Stream s, Vector4 v)
	{
		s.Write(BitConverter.GetBytes(v.x), 0, 4);
		s.Write(BitConverter.GetBytes(v.y), 0, 4);
		s.Write(BitConverter.GetBytes(v.z), 0, 4);
		s.Write(BitConverter.GetBytes(v.w), 0, 4);
	}

	public static Color Vector4ToColor(Vector4 v)
	{
		Color result = default(Color);
		result.a = v.w;
		result.r = v.x;
		result.g = v.y;
		result.b = v.z;
		return result;
	}

	public static Vector4 ColorToVector4(Color color)
	{
		Vector4 result = default(Vector4);
		result.w = color.a;
		result.x = color.r;
		result.y = color.g;
		result.z = color.b;
		return result;
	}
}
