using System;
using System.IO;
using System.Text;

namespace PmxLib;

public class PmxHeader : IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public const float LastVer = 2.1f;

	public static string PmxKey_v1 = "Pmx ";

	public static string PmxKey = "PMX ";

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Header;

	public float Ver
	{
		get
		{
			return ElementFormat.Ver;
		}
		set
		{
			ElementFormat.Ver = value;
		}
	}

	public PmxElementFormat ElementFormat { get; private set; }

	public PmxHeader(float ver)
	{
		if ((double)ver == 0.0)
		{
			ver = 2.1f;
		}
		ElementFormat = new PmxElementFormat(ver);
	}

	public PmxHeader(PmxHeader h)
	{
		FromHeader(h);
	}

	public void FromHeader(PmxHeader h)
	{
		ElementFormat = h.ElementFormat.Clone();
	}

	public void FromElementFormat(PmxElementFormat f)
	{
		ElementFormat = f;
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		byte[] array = new byte[4];
		s.Read(array, 0, array.Length);
		string @string = Encoding.ASCII.GetString(array);
		if (@string.Equals(PmxKey_v1))
		{
			Ver = 1f;
			byte[] array2 = new byte[4];
			s.Read(array2, 0, array2.Length);
		}
		else
		{
			if (!@string.Equals(PmxKey))
			{
				throw new Exception("ファイル形式が異なります.");
			}
			byte[] array3 = new byte[4];
			s.Read(array3, 0, array3.Length);
			Ver = BitConverter.ToSingle(array3, 0);
		}
		if (!((double)Ver <= 2.09999990463257))
		{
			throw new Exception("未対応のverです.");
		}
		ElementFormat = new PmxElementFormat(Ver);
		ElementFormat.FromStreamEx(s, null);
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		if (f == null)
		{
			f = ElementFormat;
		}
		_ = new byte[4];
		byte[] array = (((double)f.Ver > 1.0) ? Encoding.ASCII.GetBytes(PmxKey) : Encoding.ASCII.GetBytes(PmxKey_v1));
		s.Write(array, 0, array.Length);
		byte[] bytes = BitConverter.GetBytes(Ver);
		s.Write(bytes, 0, bytes.Length);
		f.ToStreamEx(s, null);
	}

	object ICloneable.Clone()
	{
		return new PmxHeader(this);
	}

	public PmxHeader Clone()
	{
		return new PmxHeader(this);
	}
}
