using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib;

public class Pmx : IPmxObjectKey, IPmxStreamIO, ICloneable
{
	public class BackupBoneData
	{
		public string BoneName = "";

		public int BoneId;

		public PmxBone PmxBone;

		public BackupBoneData(string name, int id, PmxBone pmxBone)
		{
			BoneName = name;
			BoneId = id;
			PmxBone = pmxBone;
		}
	}

	public const string RootNodeName = "Root";

	public const string ExpNodeName = "表情";

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Pmx;

	public PmxHeader Header { get; set; }

	public PmxModelInfo ModelInfo { get; set; }

	public List<PmxVertex> VertexList { get; set; }

	public List<int> FaceList { get; set; }

	public List<PmxMaterial> MaterialList { get; set; }

	public List<PmxBone> BoneList { get; set; }

	public Dictionary<string, BackupBoneData> BoneBackupData { get; set; }

	public List<PmxMorph> MorphList { get; set; }

	public List<PmxNode> NodeList { get; set; }

	public List<PmxBody> BodyList { get; set; }

	public List<PmxJoint> JointList { get; set; }

	public List<PmxSoftBody> SoftBodyList { get; set; }

	public PmxNode RootNode { get; set; }

	public PmxNode ExpNode { get; set; }

	public string FilePath { get; set; }

	public static PmxSaveVersion SaveVersion { get; set; }

	public static bool AutoSelect_UVACount { get; set; }

	public Pmx()
	{
		if (!PmxLibClass.IsLocked())
		{
			Header = new PmxHeader(2.1f);
			ModelInfo = new PmxModelInfo();
			VertexList = new List<PmxVertex>();
			FaceList = new List<int>();
			MaterialList = new List<PmxMaterial>();
			BoneList = new List<PmxBone>();
			MorphList = new List<PmxMorph>();
			NodeList = new List<PmxNode>();
			BodyList = new List<PmxBody>();
			JointList = new List<PmxJoint>();
			SoftBodyList = new List<PmxSoftBody>();
			RootNode = new PmxNode();
			ExpNode = new PmxNode();
			InitializeSystemNode();
			FilePath = "";
		}
	}

	static Pmx()
	{
		SaveVersion = PmxSaveVersion.AutoSelect;
		AutoSelect_UVACount = true;
	}

	public Pmx(Pmx pmx)
		: this()
	{
		FromPmx(pmx);
	}

	public Pmx(string path)
		: this()
	{
		FromFile(path);
	}

	public virtual void Clear()
	{
		Header.ElementFormat.Ver = 2.1f;
		Header.ElementFormat.UVACount = 0;
		ModelInfo.Clear();
		VertexList.Clear();
		FaceList.Clear();
		MaterialList.Clear();
		BoneList.Clear();
		MorphList.Clear();
		BodyList.Clear();
		JointList.Clear();
		SoftBodyList.Clear();
		InitializeSystemNode();
		FilePath = "";
	}

	public void Initialize()
	{
		Clear();
		InitializeBone();
	}

	public void InitializeBone()
	{
		BoneList.Clear();
		PmxBone pmxBone = new PmxBone();
		pmxBone.Name = "センター";
		pmxBone.NameE = "center";
		pmxBone.Parent = -1;
		pmxBone.SetFlag(PmxBone.BoneFlags.Translation, val: true);
		BoneList.Add(pmxBone);
	}

	public void InitializeSystemNode()
	{
		RootNode.Name = "Root";
		RootNode.NameE = "Root";
		RootNode.SystemNode = true;
		RootNode.ElementList.Clear();
		RootNode.ElementList.Add(new PmxNode.NodeElement
		{
			ElementType = PmxNode.ElementType.Bone,
			Index = 0
		});
		ExpNode.Name = "表情";
		ExpNode.NameE = "Exp";
		ExpNode.SystemNode = true;
		ExpNode.ElementList.Clear();
		NodeList.Clear();
		NodeList.Add(RootNode);
		NodeList.Add(ExpNode);
	}

	public void UpdateSystemNode()
	{
		for (int i = 0; i < NodeList.Count; i++)
		{
			if (NodeList[i].SystemNode)
			{
				if (NodeList[i].Name == "Root")
				{
					RootNode = NodeList[i];
				}
				else if (NodeList[i].Name == "表情")
				{
					ExpNode = NodeList[i];
				}
			}
		}
	}

	public bool FromFile(string path)
	{
		bool result = false;
		using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			try
			{
				FromStreamEx(s, null);
				result = true;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}
		FilePath = path;
		return result;
	}

	public bool ToFile(string path)
	{
		bool result = false;
		using (FileStream s = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			try
			{
				NormalizeVersion();
				_ = AutoSelect_UVACount;
				ToStreamEx(s, null);
				result = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw new Exception("保存中にエラーが発生しました." + ex);
			}
		}
		FilePath = path;
		return result;
	}

	public void FromPmx(Pmx pmx)
	{
		Clear();
		FilePath = pmx.FilePath;
		Header = pmx.Header.Clone();
		ModelInfo = pmx.ModelInfo.Clone();
		int count = pmx.VertexList.Count;
		VertexList.Capacity = count;
		for (int i = 0; i < count; i++)
		{
			VertexList.Add(pmx.VertexList[i].Clone());
		}
		int count2 = pmx.FaceList.Count;
		FaceList.Capacity = count2;
		for (int j = 0; j < count2; j++)
		{
			FaceList.Add(pmx.FaceList[j]);
		}
		int count3 = pmx.MaterialList.Count;
		MaterialList.Capacity = count3;
		for (int k = 0; k < count3; k++)
		{
			MaterialList.Add(pmx.MaterialList[k].Clone());
		}
		int count4 = pmx.BoneList.Count;
		BoneList.Capacity = count4;
		for (int l = 0; l < count4; l++)
		{
			BoneList.Add(pmx.BoneList[l].Clone());
		}
		int count5 = pmx.MorphList.Count;
		MorphList.Capacity = count5;
		for (int m = 0; m < count5; m++)
		{
			MorphList.Add(pmx.MorphList[m].Clone());
		}
		int count6 = pmx.NodeList.Count;
		NodeList.Clear();
		NodeList.Capacity = count6;
		for (int n = 0; n < count6; n++)
		{
			NodeList.Add(pmx.NodeList[n].Clone());
			if (NodeList[n].SystemNode)
			{
				if (NodeList[n].Name == "Root")
				{
					RootNode = NodeList[n];
				}
				else if (NodeList[n].Name == "表情")
				{
					ExpNode = NodeList[n];
				}
			}
		}
		int count7 = pmx.BodyList.Count;
		BodyList.Capacity = count7;
		for (int num = 0; num < count7; num++)
		{
			BodyList.Add(pmx.BodyList[num].Clone());
		}
		int count8 = pmx.JointList.Count;
		JointList.Capacity = count8;
		for (int num2 = 0; num2 < count8; num2++)
		{
			JointList.Add(pmx.JointList[num2].Clone());
		}
		int count9 = pmx.SoftBodyList.Count;
		SoftBodyList.Capacity = count9;
		for (int num3 = 0; num3 < count9; num3++)
		{
			SoftBodyList.Add(pmx.SoftBodyList[num3].Clone());
		}
	}

	public virtual void FromStreamEx(Stream s, PmxElementFormat f)
	{
		PmxHeader pmxHeader = new PmxHeader(2.1f);
		pmxHeader.FromStreamEx(s, null);
		Header.FromHeader(pmxHeader);
		ModelInfo.FromStreamEx(s, pmxHeader.ElementFormat);
		int num = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		VertexList.Clear();
		VertexList.Capacity = num;
		for (int i = 0; i < num; i++)
		{
			PmxVertex pmxVertex = new PmxVertex();
			pmxVertex.FromStreamEx(s, pmxHeader.ElementFormat);
			VertexList.Add(pmxVertex);
		}
		int num2 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		FaceList.Clear();
		FaceList.Capacity = num2;
		for (int j = 0; j < num2; j++)
		{
			FaceList.Add(PmxStreamHelper.ReadElement_Int32(s, pmxHeader.ElementFormat.VertexSize, signed: false));
		}
		PmxTextureTable pmxTextureTable = new PmxTextureTable();
		pmxTextureTable.FromStreamEx(s, pmxHeader.ElementFormat);
		int num3 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		MaterialList.Clear();
		MaterialList.Capacity = num3;
		for (int k = 0; k < num3; k++)
		{
			PmxMaterial pmxMaterial = new PmxMaterial();
			pmxMaterial.FromStreamEx_TexTable(s, pmxTextureTable, pmxHeader.ElementFormat);
			MaterialList.Add(pmxMaterial);
		}
		int num4 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		BoneList.Clear();
		BoneList.Capacity = num4;
		for (int l = 0; l < num4; l++)
		{
			PmxBone pmxBone = new PmxBone();
			pmxBone.FromStreamEx(s, pmxHeader.ElementFormat);
			BoneList.Add(pmxBone);
		}
		int num5 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		MorphList.Clear();
		MorphList.Capacity = num5;
		for (int m = 0; m < num5; m++)
		{
			PmxMorph pmxMorph = new PmxMorph();
			pmxMorph.FromStreamEx(s, pmxHeader.ElementFormat);
			MorphList.Add(pmxMorph);
		}
		int num6 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		NodeList.Clear();
		NodeList.Capacity = num6;
		for (int n = 0; n < num6; n++)
		{
			PmxNode pmxNode = new PmxNode();
			pmxNode.FromStreamEx(s, pmxHeader.ElementFormat);
			NodeList.Add(pmxNode);
			if (NodeList[n].SystemNode)
			{
				if (NodeList[n].Name == "Root")
				{
					RootNode = NodeList[n];
				}
				else if (NodeList[n].Name == "表情")
				{
					ExpNode = NodeList[n];
				}
			}
		}
		int num7 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		BodyList.Clear();
		BodyList.Capacity = num7;
		for (int num8 = 0; num8 < num7; num8++)
		{
			PmxBody pmxBody = new PmxBody();
			pmxBody.FromStreamEx(s, pmxHeader.ElementFormat);
			BodyList.Add(pmxBody);
		}
		int num9 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		JointList.Clear();
		JointList.Capacity = num9;
		for (int num10 = 0; num10 < num9; num10++)
		{
			PmxJoint pmxJoint = new PmxJoint();
			pmxJoint.FromStreamEx(s, pmxHeader.ElementFormat);
			JointList.Add(pmxJoint);
		}
		if (!((double)pmxHeader.Ver < 2.09999990463257))
		{
			int num11 = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
			SoftBodyList.Clear();
			SoftBodyList.Capacity = num11;
			for (int num12 = 0; num12 < num11; num12++)
			{
				PmxSoftBody pmxSoftBody = new PmxSoftBody();
				pmxSoftBody.FromStreamEx(s, pmxHeader.ElementFormat);
				SoftBodyList.Add(pmxSoftBody);
			}
		}
	}

	public void UpdateElementFormatSize(PmxElementFormat f, PmxTextureTable tx)
	{
		if (f == null)
		{
			f = Header.ElementFormat;
		}
		f.VertexSize = PmxElementFormat.GetUnsignedBufSize(VertexList.Count);
		f.MaterialSize = PmxElementFormat.GetSignedBufSize(MaterialList.Count);
		f.BoneSize = PmxElementFormat.GetSignedBufSize(BoneList.Count);
		f.MorphSize = PmxElementFormat.GetSignedBufSize(MorphList.Count);
		f.BodySize = PmxElementFormat.GetSignedBufSize(BodyList.Count);
		if (tx == null)
		{
			tx = new PmxTextureTable(MaterialList);
		}
		f.TexSize = PmxElementFormat.GetSignedBufSize(tx.Count);
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxHeader header = Header;
		PmxTextureTable pmxTextureTable = new PmxTextureTable(MaterialList);
		UpdateElementFormatSize(header.ElementFormat, pmxTextureTable);
		header.ToStreamEx(s, null);
		ModelInfo.ToStreamEx(s, header.ElementFormat);
		PmxStreamHelper.WriteElement_Int32(s, VertexList.Count, 4, signed: true);
		for (int i = 0; i < VertexList.Count; i++)
		{
			VertexList[i].ToStreamEx(s, header.ElementFormat);
		}
		PmxStreamHelper.WriteElement_Int32(s, FaceList.Count, 4, signed: true);
		for (int j = 0; j < FaceList.Count; j++)
		{
			PmxStreamHelper.WriteElement_Int32(s, FaceList[j], header.ElementFormat.VertexSize, signed: false);
		}
		pmxTextureTable.ToStreamEx(s, header.ElementFormat);
		PmxStreamHelper.WriteElement_Int32(s, MaterialList.Count, 4, signed: true);
		for (int k = 0; k < MaterialList.Count; k++)
		{
			MaterialList[k].ToStreamEx_TexTable(s, pmxTextureTable, header.ElementFormat);
		}
		PmxStreamHelper.WriteElement_Int32(s, BoneList.Count, 4, signed: true);
		for (int l = 0; l < BoneList.Count; l++)
		{
			BoneList[l].ToStreamEx(s, header.ElementFormat);
		}
		PmxStreamHelper.WriteElement_Int32(s, MorphList.Count, 4, signed: true);
		for (int m = 0; m < MorphList.Count; m++)
		{
			MorphList[m].ToStreamEx(s, header.ElementFormat);
		}
		PmxStreamHelper.WriteElement_Int32(s, NodeList.Count, 4, signed: true);
		for (int n = 0; n < NodeList.Count; n++)
		{
			NodeList[n].ToStreamEx(s, header.ElementFormat);
		}
		PmxStreamHelper.WriteElement_Int32(s, BodyList.Count, 4, signed: true);
		for (int num = 0; num < BodyList.Count; num++)
		{
			BodyList[num].ToStreamEx(s, header.ElementFormat);
		}
		PmxStreamHelper.WriteElement_Int32(s, JointList.Count, 4, signed: true);
		for (int num2 = 0; num2 < JointList.Count; num2++)
		{
			JointList[num2].ToStreamEx(s, header.ElementFormat);
		}
		if (!((double)header.Ver < 2.09999990463257))
		{
			PmxStreamHelper.WriteElement_Int32(s, SoftBodyList.Count, 4, signed: true);
			for (int num3 = 0; num3 < SoftBodyList.Count; num3++)
			{
				SoftBodyList[num3].ToStreamEx(s, header.ElementFormat);
			}
		}
	}

	public void ClearMaterialNames()
	{
		for (int i = 0; i < MaterialList.Count; i++)
		{
			MaterialList[i].Name = "材質" + (i + 1);
		}
	}

	public static void UpdateBoneIKKind(List<PmxBone> boneList)
	{
		for (int i = 0; i < boneList.Count; i++)
		{
			boneList[i].IKKind = PmxBone.IKKindType.None;
		}
		for (int j = 0; j < boneList.Count; j++)
		{
			PmxBone pmxBone = boneList[j];
			if (!pmxBone.GetFlag(PmxBone.BoneFlags.IK))
			{
				continue;
			}
			pmxBone.IKKind = PmxBone.IKKindType.IK;
			int target = pmxBone.IK.Target;
			if (CP.InRange(boneList, target))
			{
				boneList[target].IKKind = PmxBone.IKKindType.Target;
			}
			for (int k = 0; k < pmxBone.IK.LinkList.Count; k++)
			{
				int bone = pmxBone.IK.LinkList[k].Bone;
				if (CP.InRange(boneList, bone))
				{
					boneList[bone].IKKind = PmxBone.IKKindType.Link;
				}
			}
		}
	}

	public void UpdateBoneIKKind()
	{
		UpdateBoneIKKind(BoneList);
	}

	public void NormalizeVertex_SDEF_C0()
	{
		for (int i = 0; i < VertexList.Count; i++)
		{
			VertexList[i].NormalizeSDEF_C0(BoneList);
		}
	}

	public float RequireVersion(out bool isQDEF, out bool isExMorph, out bool isExJoint, out bool isSoftBody)
	{
		Func<bool> func = delegate
		{
			bool result4 = false;
			for (int k = 0; k < VertexList.Count; k++)
			{
				if (VertexList[k].Deform == PmxVertex.DeformType.QDEF)
				{
					result4 = true;
					break;
				}
			}
			return result4;
		};
		Func<bool> func2 = delegate
		{
			bool result3 = false;
			for (int j = 0; j < MorphList.Count; j++)
			{
				PmxMorph pmxMorph = MorphList[j];
				if (pmxMorph.IsFlip || pmxMorph.IsImpulse)
				{
					result3 = true;
					break;
				}
			}
			return result3;
		};
		Func<bool> func3 = delegate
		{
			bool result2 = false;
			for (int i = 0; i < JointList.Count; i++)
			{
				if (JointList[i].Kind != 0)
				{
					result2 = true;
					break;
				}
			}
			return result2;
		};
		Func<bool> func4 = () => SoftBodyList.Count > 0;
		isQDEF = func();
		isExMorph = func2();
		isExJoint = func3();
		isSoftBody = func4();
		float result = 2f;
		if (isQDEF | isExMorph | isExJoint | isSoftBody)
		{
			result = 2.1f;
		}
		return result;
	}

	private void NormalizeVersion()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		float ver = 2f;
		switch (SaveVersion)
		{
		case PmxSaveVersion.AutoSelect:
			Header.Ver = ver;
			break;
		case PmxSaveVersion.PMX2_0:
		{
			string text = "";
			if (flag)
			{
				text = text + "頂点ウェイト : QDEF -> BDEF4" + Environment.NewLine;
			}
			if (flag2)
			{
				text = text + "モーフ : インパルス->削除／フリップ->グループ" + Environment.NewLine;
			}
			if (flag3)
			{
				text = text + "Joint : 拡張Joint -> 基本Joint(ﾊﾞﾈ付6DOF)" + Environment.NewLine;
			}
			if (flag4)
			{
				text = text + "SoftBody : 削除" + Environment.NewLine;
			}
			Header.Ver = 2f;
			if (text.Length > 0)
			{
				_ = "PMX2.0での保存では 以下の項目が書き換えられますが よろしいですか?" + Environment.NewLine + Environment.NewLine + text;
			}
			break;
		}
		case PmxSaveVersion.PMX2_1:
			Header.Ver = 2.1f;
			break;
		}
	}

	public void NormalizeUVACount()
	{
		if (VertexList.Count <= 0)
		{
			Header.ElementFormat.UVACount = 0;
			return;
		}
		Func<Vector4, bool> func = (Vector4 v) => (double)Math.Abs(v.x) > 9.99999996004197E-13 || (double)Math.Abs(v.y) > 9.99999996004197E-13 || (double)Math.Abs(v.z) > 9.99999996004197E-13 || (double)Math.Abs(v.w) > 9.99999996004197E-13;
		int num = 0;
		foreach (PmxVertex vertex in VertexList)
		{
			for (int i = 0; i < vertex.UVA.Length; i++)
			{
				if (func(vertex.UVA[i]))
				{
					int num2 = i + 1;
					if (num < num2)
					{
						num = num2;
					}
				}
			}
		}
		Header.ElementFormat.UVACount = num;
	}

	object ICloneable.Clone()
	{
		return new Pmx(this);
	}

	public virtual Pmx Clone()
	{
		return new Pmx(this);
	}
}
