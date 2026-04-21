using System;
using System.IO;

namespace PmxLib;

public class PmxElementFormat : IPmxStreamIO, ICloneable
{
	public enum StringEncType
	{
		UTF16,
		UTF8
	}

	private const int SizeBufLength = 8;

	public const int MaxUVACount = 4;

	public float Ver { get; set; }

	public StringEncType StringEnc { get; set; }

	public int UVACount { get; set; }

	public int VertexSize { get; set; }

	public int TexSize { get; set; }

	public int MaterialSize { get; set; }

	public int BoneSize { get; set; }

	public int MorphSize { get; set; }

	public int BodySize { get; set; }

	public PmxElementFormat(float ver)
	{
		if ((double)ver == 0.0)
		{
			ver = 2.1f;
		}
		Ver = ver;
		StringEnc = StringEncType.UTF16;
		UVACount = 0;
		VertexSize = 2;
		TexSize = 1;
		MaterialSize = 1;
		BoneSize = 2;
		MorphSize = 2;
		BodySize = 4;
	}

	public PmxElementFormat(PmxElementFormat f)
	{
		FromElementFormat(f);
	}

	public void FromElementFormat(PmxElementFormat f)
	{
		Ver = f.Ver;
		StringEnc = f.StringEnc;
		UVACount = f.UVACount;
		VertexSize = f.VertexSize;
		TexSize = f.TexSize;
		MaterialSize = f.MaterialSize;
		BoneSize = f.BoneSize;
		MorphSize = f.MorphSize;
		BodySize = f.BodySize;
	}

	public static int GetUnsignedBufSize(int count)
	{
		if (count < 256)
		{
			return 1;
		}
		if (count < 65536)
		{
			return 2;
		}
		return 4;
	}

	public static int GetSignedBufSize(int count)
	{
		if (count < 128)
		{
			return 1;
		}
		if (count < 32768)
		{
			return 2;
		}
		return 4;
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		byte[] array = new byte[PmxStreamHelper.ReadElement_Int32(s, 1, signed: true)];
		s.Read(array, 0, array.Length);
		int num = 0;
		if ((double)Ver <= 1.0)
		{
			byte[] array2 = array;
			int num2 = num;
			int num3 = num2 + 1;
			VertexSize = array2[num2];
			byte[] array3 = array;
			int num4 = num3;
			int num5 = num4 + 1;
			BoneSize = array3[num4];
			byte[] array4 = array;
			int num6 = num5;
			int num7 = num6 + 1;
			MorphSize = array4[num6];
			byte[] array5 = array;
			int num8 = num7;
			int num9 = num8 + 1;
			MaterialSize = array5[num8];
			byte[] array6 = array;
			int num10 = num9;
			BodySize = array6[num10];
		}
		else
		{
			byte[] array7 = array;
			int num11 = num;
			int num12 = num11 + 1;
			StringEnc = (StringEncType)array7[num11];
			byte[] array8 = array;
			int num13 = num12;
			int num14 = num13 + 1;
			UVACount = array8[num13];
			byte[] array9 = array;
			int num15 = num14;
			int num16 = num15 + 1;
			VertexSize = array9[num15];
			byte[] array10 = array;
			int num17 = num16;
			int num18 = num17 + 1;
			TexSize = array10[num17];
			byte[] array11 = array;
			int num19 = num18;
			int num20 = num19 + 1;
			MaterialSize = array11[num19];
			byte[] array12 = array;
			int num21 = num20;
			int num22 = num21 + 1;
			BoneSize = array12[num21];
			byte[] array13 = array;
			int num23 = num22;
			int num24 = num23 + 1;
			MorphSize = array13[num23];
			byte[] array14 = array;
			int num25 = num24;
			BodySize = array14[num25];
		}
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		byte[] array = new byte[8];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0;
		}
		int num = 0;
		if ((double)Ver <= 1.0)
		{
			int num2 = num;
			int num3 = num2 + 1;
			int num4 = (byte)VertexSize;
			array[num2] = (byte)num4;
			int num5 = num3;
			int num6 = num5 + 1;
			int num7 = (byte)BoneSize;
			array[num5] = (byte)num7;
			int num8 = num6;
			int num9 = num8 + 1;
			int num10 = (byte)MorphSize;
			array[num8] = (byte)num10;
			int num11 = num9;
			int num12 = num11 + 1;
			int num13 = (byte)MaterialSize;
			array[num11] = (byte)num13;
			int num14 = num12;
			int num15 = (byte)BodySize;
			array[num14] = (byte)num15;
		}
		else
		{
			int num16 = num;
			int num17 = num16 + 1;
			int num18 = (byte)StringEnc;
			array[num16] = (byte)num18;
			int num19 = num17;
			int num20 = num19 + 1;
			int num21 = (byte)UVACount;
			array[num19] = (byte)num21;
			int num22 = num20;
			int num23 = num22 + 1;
			int num24 = (byte)VertexSize;
			array[num22] = (byte)num24;
			int num25 = num23;
			int num26 = num25 + 1;
			int num27 = (byte)TexSize;
			array[num25] = (byte)num27;
			int num28 = num26;
			int num29 = num28 + 1;
			int num30 = (byte)MaterialSize;
			array[num28] = (byte)num30;
			int num31 = num29;
			int num32 = num31 + 1;
			int num33 = (byte)BoneSize;
			array[num31] = (byte)num33;
			int num34 = num32;
			int num35 = num34 + 1;
			int num36 = (byte)MorphSize;
			array[num34] = (byte)num36;
			int num37 = num35;
			int num38 = (byte)BodySize;
			array[num37] = (byte)num38;
		}
		PmxStreamHelper.WriteElement_Int32(s, array.Length, 1, signed: true);
		s.Write(array, 0, array.Length);
	}

	object ICloneable.Clone()
	{
		return new PmxElementFormat(this);
	}

	public PmxElementFormat Clone()
	{
		return new PmxElementFormat(this);
	}
}
