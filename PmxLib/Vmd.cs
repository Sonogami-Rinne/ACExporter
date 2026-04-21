using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib;

public class Vmd : IBytesConvert, ICloneable
{
	public enum VmdVersion
	{
		v2,
		v1
	}

	public enum NormalizeDataType
	{
		All,
		Motion,
		Skin,
		Camera,
		Light,
		SelfShadow,
		VisibleIK
	}

	private const int HeaderBytes = 30;

	private const string HeaderString_V1 = "Vocaloid Motion Data file";

	private const string HeaderString_V2 = "Vocaloid Motion Data 0002";

	public const string CameraHeaderName = "カメラ・照明";

	private const int ModelNameBytes_V1 = 10;

	private const int ModelNameBytes_V2 = 20;

	public string VMDHeader = "Vocaloid Motion Data 0002";

	public string ModelName = "";

	public int ModelNameBytes = 20;

	private VmdVersion m_ver;

	public List<VmdMotion> MotionList = new List<VmdMotion>();

	public List<VmdMorph> MorphList = new List<VmdMorph>();

	public List<VmdCamera> CameraList = new List<VmdCamera>();

	public List<VmdLight> LightList = new List<VmdLight>();

	public List<VmdSelfShadow> SelfShadowList = new List<VmdSelfShadow>();

	public List<VmdVisibleIK> VisibleIKList = new List<VmdVisibleIK>();

	public VmdVersion Version => m_ver;

	public int ByteCount => 30 + ModelNameBytes + GetListBytes(MotionList) + GetListBytes(MorphList) + GetListBytes(VisibleIKList) + GetListBytes(CameraList) + GetListBytes(LightList) + GetListBytes(SelfShadowList);

	public Vmd()
	{
		PmxLibClass.IsLocked();
	}

	public Vmd(Vmd vmd)
		: this()
	{
		VMDHeader = vmd.VMDHeader;
		ModelName = vmd.ModelName;
		MotionList = CP.CloneList(vmd.MotionList);
		MorphList = CP.CloneList(vmd.MorphList);
		CameraList = CP.CloneList(vmd.CameraList);
		LightList = CP.CloneList(vmd.LightList);
		SelfShadowList = CP.CloneList(vmd.SelfShadowList);
		VisibleIKList = CP.CloneList(vmd.VisibleIKList);
	}

	public Vmd(string path)
		: this()
	{
		FromFile(path);
	}

	public void FromFile(string path)
	{
		try
		{
			FromBytes(File.ReadAllBytes(path), 0);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public void NormalizeList(NormalizeDataType type)
	{
		switch (type)
		{
		case NormalizeDataType.All:
			MotionList.Sort(VmdFrameBase.Compare);
			MorphList.Sort(VmdFrameBase.Compare);
			CameraList.Sort(VmdFrameBase.Compare);
			LightList.Sort(VmdFrameBase.Compare);
			SelfShadowList.Sort(VmdFrameBase.Compare);
			VisibleIKList.Sort(VmdFrameBase.Compare);
			break;
		case NormalizeDataType.Motion:
			MotionList.Sort(VmdFrameBase.Compare);
			break;
		case NormalizeDataType.Skin:
			MorphList.Sort(VmdFrameBase.Compare);
			break;
		case NormalizeDataType.Camera:
			CameraList.Sort(VmdFrameBase.Compare);
			break;
		case NormalizeDataType.Light:
			LightList.Sort(VmdFrameBase.Compare);
			break;
		case NormalizeDataType.SelfShadow:
			SelfShadowList.Sort(VmdFrameBase.Compare);
			break;
		case NormalizeDataType.VisibleIK:
			VisibleIKList.Sort(VmdFrameBase.Compare);
			break;
		}
	}

	public byte[] ToBytes()
	{
		NormalizeList(NormalizeDataType.All);
		List<byte> list = new List<byte>();
		byte[] array = new byte[30];
		BytesStringProc.SetString(array, VMDHeader, 0, 0);
		list.AddRange(array);
		byte[] array2 = new byte[ModelNameBytes];
		BytesStringProc.SetString(array2, ModelName, 0, 253);
		list.AddRange(array2);
		int count = MotionList.Count;
		list.AddRange(BitConverter.GetBytes(count));
		for (int i = 0; i < count; i++)
		{
			list.AddRange(MotionList[i].ToBytes());
		}
		int count2 = MorphList.Count;
		list.AddRange(BitConverter.GetBytes(count2));
		for (int j = 0; j < count2; j++)
		{
			list.AddRange(MorphList[j].ToBytes());
		}
		int count3 = VisibleIKList.Count;
		list.AddRange(BitConverter.GetBytes(count3));
		for (int k = 0; k < count3; k++)
		{
			list.AddRange(VisibleIKList[k].ToBytes());
		}
		int count4 = CameraList.Count;
		list.AddRange(BitConverter.GetBytes(count4));
		for (int l = 0; l < count4; l++)
		{
			list.AddRange(CameraList[l].ToBytes());
		}
		int count5 = LightList.Count;
		list.AddRange(BitConverter.GetBytes(count5));
		for (int m = 0; m < count5; m++)
		{
			list.AddRange(LightList[m].ToBytes());
		}
		int count6 = SelfShadowList.Count;
		list.AddRange(BitConverter.GetBytes(count6));
		for (int n = 0; n < count6; n++)
		{
			list.AddRange(SelfShadowList[n].ToBytes());
		}
		return list.ToArray();
	}

	public void FromBytes(byte[] bytes, int startIndex)
	{
		byte[] array = new byte[30];
		Array.Copy(bytes, startIndex, array, 0, 30);
		VMDHeader = BytesStringProc.GetString(array, 0);
		int num;
		if (string.Compare(VMDHeader, "Vocaloid Motion Data file", ignoreCase: true) == 0)
		{
			ModelNameBytes = 10;
			num = 1;
			m_ver = VmdVersion.v1;
		}
		else
		{
			if (string.Compare(VMDHeader, "Vocaloid Motion Data 0002", ignoreCase: true) != 0)
			{
				throw new Exception("対応したVMDファイルではありません");
			}
			ModelNameBytes = 20;
			num = 2;
			m_ver = VmdVersion.v2;
		}
		int num2 = startIndex + 30;
		Array.Copy(bytes, num2, array, 0, ModelNameBytes);
		ModelName = BytesStringProc.GetString(array, 0);
		int num3 = num2 + ModelNameBytes;
		int num4 = BitConverter.ToInt32(bytes, num3);
		int num5 = num3 + 4;
		MotionList.Clear();
		MotionList.Capacity = num4;
		for (int i = 0; i < num4; i++)
		{
			VmdMotion vmdMotion = new VmdMotion();
			vmdMotion.FromBytes(bytes, num5);
			num5 += vmdMotion.ByteCount;
			MotionList.Add(vmdMotion);
		}
		if (bytes.Length <= num5)
		{
			return;
		}
		int num6 = BitConverter.ToInt32(bytes, num5);
		int num7 = num5 + 4;
		MorphList.Clear();
		MorphList.Capacity = num6;
		for (int j = 0; j < num6; j++)
		{
			VmdMorph vmdMorph = new VmdMorph();
			vmdMorph.FromBytes(bytes, num7);
			num7 += vmdMorph.ByteCount;
			MorphList.Add(vmdMorph);
		}
		if (bytes.Length <= num7)
		{
			return;
		}
		int num8 = BitConverter.ToInt32(bytes, num7);
		int num9 = num7 + 4;
		CameraList.Clear();
		CameraList.Capacity = num8;
		switch (num)
		{
		case 1:
		{
			for (int l = 0; l < num8; l++)
			{
				VmdCamera_v1 vmdCamera_v = new VmdCamera_v1();
				vmdCamera_v.FromBytes(bytes, num9);
				num9 += vmdCamera_v.ByteCount;
				CameraList.Add(vmdCamera_v.ToVmdCamera());
			}
			break;
		}
		case 2:
		{
			for (int k = 0; k < num8; k++)
			{
				VmdCamera vmdCamera = new VmdCamera();
				vmdCamera.FromBytes(bytes, num9);
				num9 += vmdCamera.ByteCount;
				CameraList.Add(vmdCamera);
			}
			break;
		}
		}
		if (bytes.Length <= num9)
		{
			return;
		}
		int num10 = BitConverter.ToInt32(bytes, num9);
		int num11 = num9 + 4;
		LightList.Clear();
		LightList.Capacity = num10;
		for (int m = 0; m < num10; m++)
		{
			VmdLight vmdLight = new VmdLight();
			vmdLight.FromBytes(bytes, num11);
			num11 += vmdLight.ByteCount;
			LightList.Add(vmdLight);
		}
		if (bytes.Length <= num11)
		{
			return;
		}
		int num12 = BitConverter.ToInt32(bytes, num11);
		int num13 = num11 + 4;
		SelfShadowList.Clear();
		SelfShadowList.Capacity = num12;
		for (int n = 0; n < num12; n++)
		{
			VmdSelfShadow vmdSelfShadow = new VmdSelfShadow();
			vmdSelfShadow.FromBytes(bytes, num13);
			num13 += vmdSelfShadow.ByteCount;
			SelfShadowList.Add(vmdSelfShadow);
		}
		if (bytes.Length > num13)
		{
			int num14 = BitConverter.ToInt32(bytes, num13);
			int num15 = num13 + 4;
			VisibleIKList.Clear();
			VisibleIKList.Capacity = num14;
			for (int num16 = 0; num16 < num14; num16++)
			{
				VmdVisibleIK vmdVisibleIK = new VmdVisibleIK();
				vmdVisibleIK.FromBytes(bytes, num15);
				num15 += vmdVisibleIK.ByteCount;
				VisibleIKList.Add(vmdVisibleIK);
			}
		}
	}

	public object Clone()
	{
		return new Vmd(this);
	}

	public static int GetListBytes<T>(List<T> list) where T : IBytesConvert
	{
		int count = list.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			num += list[i].ByteCount;
		}
		return num;
	}
}
