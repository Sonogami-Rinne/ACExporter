using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib;

public class PmxMorph : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
{
	public enum OffsetKind
	{
		Group,
		Vertex,
		Bone,
		UV,
		UVA1,
		UVA2,
		UVA3,
		UVA4,
		Material,
		Flip,
		Impulse
	}

	public int Panel;

	public OffsetKind Kind;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Morph;

	public string Name { get; set; }

	public string NameE { get; set; }

	public List<PmxBaseMorph> OffsetList { get; private set; }

	public bool IsUV
	{
		get
		{
			if (Kind != OffsetKind.UV && Kind != OffsetKind.UVA1 && Kind != OffsetKind.UVA2 && Kind != OffsetKind.UVA3)
			{
				return Kind == OffsetKind.UVA4;
			}
			return true;
		}
	}

	public bool IsVertex => Kind == OffsetKind.Vertex;

	public bool IsBone => Kind == OffsetKind.Bone;

	public bool IsMaterial => Kind == OffsetKind.Material;

	public bool IsFlip => Kind == OffsetKind.Flip;

	public bool IsImpulse => Kind == OffsetKind.Impulse;

	public bool IsGroup => Kind == OffsetKind.Group;

	public string NXName
	{
		get
		{
			return Name;
		}
		set
		{
			Name = value;
		}
	}

	public static string KindText(OffsetKind kind)
	{
		string result = "-";
		switch (kind)
		{
		case OffsetKind.Group:
			result = "グループ";
			break;
		case OffsetKind.Vertex:
			result = "頂点";
			break;
		case OffsetKind.Bone:
			result = "ボーン";
			break;
		case OffsetKind.UV:
			result = "UV";
			break;
		case OffsetKind.UVA1:
			result = "追加UV1";
			break;
		case OffsetKind.UVA2:
			result = "追加UV2";
			break;
		case OffsetKind.UVA3:
			result = "追加UV3";
			break;
		case OffsetKind.UVA4:
			result = "追加UV4";
			break;
		case OffsetKind.Material:
			result = "材質";
			break;
		case OffsetKind.Flip:
			result = "フリップ";
			break;
		case OffsetKind.Impulse:
			result = "インパルス";
			break;
		}
		return result;
	}

	public PmxMorph()
	{
		Name = "";
		NameE = "";
		Panel = 4;
		Kind = OffsetKind.Vertex;
		OffsetList = new List<PmxBaseMorph>();
	}

	public PmxMorph(PmxMorph m, bool nonStr)
	{
		FromPmxMorph(m, nonStr);
	}

	public void FromPmxMorph(PmxMorph m, bool nonStr)
	{
		if (!nonStr)
		{
			Name = m.Name;
			NameE = m.NameE;
		}
		Panel = m.Panel;
		Kind = m.Kind;
		int count = m.OffsetList.Count;
		OffsetList = new List<PmxBaseMorph>(count);
		for (int i = 0; i < count; i++)
		{
			OffsetList.Add(m.OffsetList[i].Clone());
		}
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		Panel = PmxStreamHelper.ReadElement_Int32(s, 1, signed: true);
		Kind = (OffsetKind)PmxStreamHelper.ReadElement_Int32(s, 1, signed: true);
		int num = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		OffsetList.Clear();
		OffsetList.Capacity = num;
		for (int i = 0; i < num; i++)
		{
			switch (Kind)
			{
			case OffsetKind.Group:
			case OffsetKind.Flip:
			{
				PmxGroupMorph pmxGroupMorph = new PmxGroupMorph();
				pmxGroupMorph.FromStreamEx(s, f);
				OffsetList.Add(pmxGroupMorph);
				break;
			}
			case OffsetKind.Vertex:
			{
				PmxVertexMorph pmxVertexMorph = new PmxVertexMorph();
				pmxVertexMorph.FromStreamEx(s, f);
				OffsetList.Add(pmxVertexMorph);
				break;
			}
			case OffsetKind.Bone:
			{
				PmxBoneMorph pmxBoneMorph = new PmxBoneMorph();
				pmxBoneMorph.FromStreamEx(s, f);
				OffsetList.Add(pmxBoneMorph);
				break;
			}
			case OffsetKind.UV:
			case OffsetKind.UVA1:
			case OffsetKind.UVA2:
			case OffsetKind.UVA3:
			case OffsetKind.UVA4:
			{
				PmxUVMorph pmxUVMorph = new PmxUVMorph();
				pmxUVMorph.FromStreamEx(s, f);
				OffsetList.Add(pmxUVMorph);
				break;
			}
			case OffsetKind.Material:
			{
				PmxMaterialMorph pmxMaterialMorph = new PmxMaterialMorph();
				pmxMaterialMorph.FromStreamEx(s, f);
				OffsetList.Add(pmxMaterialMorph);
				break;
			}
			case OffsetKind.Impulse:
			{
				PmxImpulseMorph pmxImpulseMorph = new PmxImpulseMorph();
				pmxImpulseMorph.FromStreamEx(s, f);
				OffsetList.Add(pmxImpulseMorph);
				break;
			}
			}
		}
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		if (IsImpulse && (double)f.Ver < 2.09999990463257)
		{
			return;
		}
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		PmxStreamHelper.WriteElement_Int32(s, Panel, 1, signed: true);
		if (IsFlip && (double)f.Ver < 2.09999990463257)
		{
			PmxStreamHelper.WriteElement_Int32(s, 0, 1, signed: true);
		}
		else
		{
			PmxStreamHelper.WriteElement_Int32(s, (int)Kind, 1, signed: true);
		}
		PmxStreamHelper.WriteElement_Int32(s, OffsetList.Count, 4, signed: true);
		for (int i = 0; i < OffsetList.Count; i++)
		{
			switch (Kind)
			{
			case OffsetKind.Group:
			case OffsetKind.Flip:
				(OffsetList[i] as PmxGroupMorph).ToStreamEx(s, f);
				break;
			case OffsetKind.Vertex:
				(OffsetList[i] as PmxVertexMorph).ToStreamEx(s, f);
				break;
			case OffsetKind.Bone:
				(OffsetList[i] as PmxBoneMorph).ToStreamEx(s, f);
				break;
			case OffsetKind.UV:
			case OffsetKind.UVA1:
			case OffsetKind.UVA2:
			case OffsetKind.UVA3:
			case OffsetKind.UVA4:
				(OffsetList[i] as PmxUVMorph).ToStreamEx(s, f);
				break;
			case OffsetKind.Material:
				(OffsetList[i] as PmxMaterialMorph).ToStreamEx(s, f);
				break;
			case OffsetKind.Impulse:
				(OffsetList[i] as PmxImpulseMorph).ToStreamEx(s, f);
				break;
			}
		}
	}

	object ICloneable.Clone()
	{
		return new PmxMorph(this, nonStr: false);
	}

	public PmxMorph Clone()
	{
		return new PmxMorph(this, nonStr: false);
	}
}
