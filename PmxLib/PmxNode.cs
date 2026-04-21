using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib;

public class PmxNode : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
{
	public enum ElementType
	{
		Bone,
		Morph
	}

	public class NodeElement : IPmxObjectKey, IPmxStreamIO, ICloneable
	{
		public ElementType ElementType;

		public int Index;

		PmxObject IPmxObjectKey.ObjectKey => PmxObject.NodeElement;

		public PmxBone RefBone { get; set; }

		public PmxMorph RefMorph { get; set; }

		public NodeElement()
		{
		}

		public NodeElement(NodeElement e)
		{
			FromNodeElement(e);
		}

		public void FromNodeElement(NodeElement e)
		{
			ElementType = e.ElementType;
			Index = e.Index;
		}

		public void FromStreamEx(Stream s, PmxElementFormat f)
		{
			ElementType = (ElementType)s.ReadByte();
			switch (ElementType)
			{
			case ElementType.Bone:
				Index = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, signed: true);
				break;
			case ElementType.Morph:
				Index = PmxStreamHelper.ReadElement_Int32(s, f.MorphSize, signed: true);
				break;
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f)
		{
			s.WriteByte((byte)ElementType);
			switch (ElementType)
			{
			case ElementType.Bone:
				PmxStreamHelper.WriteElement_Int32(s, Index, f.BoneSize, signed: true);
				break;
			case ElementType.Morph:
				PmxStreamHelper.WriteElement_Int32(s, Index, f.MorphSize, signed: true);
				break;
			}
		}

		public static NodeElement BoneElement(int index)
		{
			return new NodeElement
			{
				ElementType = ElementType.Bone,
				Index = index
			};
		}

		public static NodeElement MorphElement(int index)
		{
			return new NodeElement
			{
				ElementType = ElementType.Morph,
				Index = index
			};
		}

		object ICloneable.Clone()
		{
			return new NodeElement(this);
		}

		public NodeElement Clone()
		{
			return new NodeElement(this);
		}
	}

	public bool SystemNode;

	public List<NodeElement> ElementList;

	PmxObject IPmxObjectKey.ObjectKey => PmxObject.Node;

	public string Name { get; set; }

	public string NameE { get; set; }

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

	public PmxNode()
	{
		Name = "";
		NameE = "";
		SystemNode = false;
		ElementList = new List<NodeElement>();
	}

	public PmxNode(PmxNode node, bool nonStr)
		: this()
	{
		FromPmxNode(node, nonStr);
	}

	public void FromPmxNode(PmxNode node, bool nonStr)
	{
		if (!nonStr)
		{
			Name = node.Name;
			NameE = node.NameE;
		}
		SystemNode = node.SystemNode;
		int count = node.ElementList.Count;
		ElementList.Clear();
		ElementList.Capacity = count;
		for (int i = 0; i < count; i++)
		{
			ElementList.Add(node.ElementList[i].Clone());
		}
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		Name = PmxStreamHelper.ReadString(s, f);
		NameE = PmxStreamHelper.ReadString(s, f);
		SystemNode = s.ReadByte() != 0;
		int num = PmxStreamHelper.ReadElement_Int32(s, 4, signed: true);
		ElementList.Clear();
		ElementList.Capacity = num;
		for (int i = 0; i < num; i++)
		{
			NodeElement nodeElement = new NodeElement();
			nodeElement.FromStreamEx(s, f);
			ElementList.Add(nodeElement);
		}
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, Name, f);
		PmxStreamHelper.WriteString(s, NameE, f);
		s.WriteByte((byte)(SystemNode ? 1 : 0));
		PmxStreamHelper.WriteElement_Int32(s, ElementList.Count, 4, signed: true);
		for (int i = 0; i < ElementList.Count; i++)
		{
			ElementList[i].ToStreamEx(s, f);
		}
	}

	object ICloneable.Clone()
	{
		return new PmxNode(this, nonStr: false);
	}

	public PmxNode Clone()
	{
		return new PmxNode(this, nonStr: false);
	}
}
