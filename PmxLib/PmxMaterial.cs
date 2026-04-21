using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PmxLib;

public class PmxMaterial : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
{
	[Flags]
	public enum MaterialFlags
	{
		None = 0,
		DrawBoth = 1,
		Shadow = 2,
		SelfShadowMap = 4,
		SelfShadow = 8,
		Edge = 0x10,
		VertexColor = 0x20,
		PointDraw = 0x40,
		LineDraw = 0x80
	}

	public enum ExDrawMode
	{
		F1,
		F2,
		F3
	}

	public enum SphereModeType
	{
		None,
		Mul,
		Add,
		SubTex
	}

	public Color Diffuse;

	public Color Specular;

	public float Power;

	public Color Ambient;

	public ExDrawMode ExDraw;

	public MaterialFlags Flags;

	public Color EdgeColor;

	public float EdgeSize;

	public int FaceCount;

	public SphereModeType SphereMode;

	public PmxMaterialMorph.MorphData OffsetMul;

	public PmxMaterialMorph.MorphData OffsetAdd;

	public List<PmxFace> FaceList;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Material;

	public string Name { get; set; }

	public string NameE { get; set; }

	public string Tex { get; set; }

	public string Sphere { get; set; }

	public string Toon { get; set; }

	public string Memo { get; set; }

	public PmxMaterialAttribute Attribute { get; private set; }

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

	public void ClearFlags()
	{
		Flags = MaterialFlags.None;
	}

	public bool GetFlag(MaterialFlags f)
	{
		return (f & Flags) == f;
	}

	public void SetFlag(MaterialFlags f, bool val)
	{
		if (val)
		{
			Flags |= f;
		}
		else
		{
			Flags &= ~f;
		}
	}

	public void ClearOffset()
	{
		OffsetMul.Clear(PmxMaterialMorph.OpType.Mul);
		OffsetAdd.Clear(PmxMaterialMorph.OpType.Add);
	}

	public void UpdateAttributeFromMemo()
	{
		Attribute.SetFromText(Memo);
	}

	public PmxMaterial()
	{
		Name = "";
		NameE = "";
		Diffuse = new Color(1f, 1f, 1f);
		Specular = new Color(0f, 0f, 0f);
		Power = 0f;
		Ambient = new Color(1f, 1f, 1f);
		ClearFlags();
		EdgeColor = new Color(0f, 0f, 0f);
		EdgeSize = 1f;
		Tex = "";
		Sphere = "";
		SphereMode = SphereModeType.Mul;
		Toon = "";
		Memo = "";
		OffsetMul = default(PmxMaterialMorph.MorphData);
		OffsetAdd = default(PmxMaterialMorph.MorphData);
		ClearOffset();
		ExDraw = ExDrawMode.F3;
		Attribute = new PmxMaterialAttribute();
	}

	public PmxMaterial(PmxMaterial m, bool nonStr)
		: this()
	{
		FromPmxMaterial(m, nonStr);
	}

	public void FromPmxMaterial(PmxMaterial m, bool nonStr)
	{
		Diffuse = m.Diffuse;
		Specular = m.Specular;
		Power = m.Power;
		Ambient = m.Ambient;
		Flags = m.Flags;
		EdgeColor = m.EdgeColor;
		EdgeSize = m.EdgeSize;
		SphereMode = m.SphereMode;
		FaceCount = m.FaceCount;
		ExDraw = m.ExDraw;
		if (!nonStr)
		{
			Name = m.Name;
			NameE = m.NameE;
			Tex = m.Tex;
			Sphere = m.Sphere;
			Toon = m.Toon;
			Memo = m.Memo;
		}
		Attribute = m.Attribute;
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		Diffuse = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
		Specular = V3_BytesConvert.Vector3ToColor(V3_BytesConvert.FromStream(s));
		Power = PmxStreamHelper.ReadElement_Float(s);
		Ambient = V3_BytesConvert.Vector3ToColor(V3_BytesConvert.FromStream(s));
		Flags = (MaterialFlags)s.ReadByte();
		EdgeColor = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
		EdgeSize = PmxStreamHelper.ReadElement_Float(s);
		Tex = PmxStreamHelper.ReadString(s, f);
		Sphere = PmxStreamHelper.ReadString(s, f);
		SphereMode = (SphereModeType)s.ReadByte();
		Toon = PmxStreamHelper.ReadString(s, f);
		Memo = PmxStreamHelper.ReadString(s, f);
		FaceCount = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(Diffuse));
		V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(Specular));
		PmxStreamHelper.WriteElement_Float(s, Power);
		V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(Ambient));
		s.WriteByte((byte)Flags);
		V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(EdgeColor));
		PmxStreamHelper.WriteElement_Float(s, EdgeSize);
		PmxStreamHelper.WriteString(s, Tex, f);
		PmxStreamHelper.WriteString(s, Sphere, f);
		s.WriteByte((byte)SphereMode);
		PmxStreamHelper.WriteString(s, Toon, f);
		PmxStreamHelper.WriteString(s, Memo, f);
		PmxStreamHelper.WriteElement_Int32(s, FaceCount, 4, signed: true);
	}

	public void FromStreamEx_TexTable(Stream s, PmxTextureTable tx, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		Diffuse = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
		Specular = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
		Power = PmxStreamHelper.ReadElement_Float(s);
		Ambient = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
		Flags = (MaterialFlags)s.ReadByte();
		EdgeColor = V4_BytesConvert.Vector4ToColor(V4_BytesConvert.FromStream(s));
		EdgeSize = PmxStreamHelper.ReadElement_Float(s);
		Tex = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize, signed: true));
		Sphere = tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize, signed: true));
		SphereMode = (SphereModeType)s.ReadByte();
		Toon = ((s.ReadByte() != 0) ? SystemToon.GetToonName(s.ReadByte()) : tx.GetName(PmxStreamHelper.ReadElement_Int32(s, f.TexSize, signed: true)));
		Memo = PmxStreamHelper.ReadString(s, f);
		UpdateAttributeFromMemo();
		FaceCount = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
	}

	public void ToStreamEx_TexTable(Stream s, PmxTextureTable tx, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(Diffuse));
		V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(Specular));
		PmxStreamHelper.WriteElement_Float(s, Power);
		V3_BytesConvert.ToStream(s, V3_BytesConvert.ColorToVector3(Ambient));
		s.WriteByte((byte)Flags);
		V4_BytesConvert.ToStream(s, V4_BytesConvert.ColorToVector4(EdgeColor));
		PmxStreamHelper.WriteElement_Float(s, EdgeSize);
		PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(Tex), f.TexSize, signed: true);
		PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(Sphere), f.TexSize, signed: true);
		s.WriteByte((byte)SphereMode);
		int toonIndex = SystemToon.GetToonIndex(Toon);
		if (toonIndex < 0)
		{
			s.WriteByte(0);
			PmxStreamHelper.WriteElement_Int32(s, tx.GetIndex(Toon), f.TexSize, signed: true);
		}
		else
		{
			s.WriteByte(1);
			s.WriteByte((byte)toonIndex);
		}
		PmxStreamHelper.WriteString(s, Memo, f);
		PmxStreamHelper.WriteElement_Int32(s, FaceCount, 4, signed: true);
	}

	object ICloneable.Clone()
	{
		return new PmxMaterial(this, nonStr: false);
	}

	public PmxMaterial Clone()
	{
		return new PmxMaterial(this, nonStr: false);
	}
}
