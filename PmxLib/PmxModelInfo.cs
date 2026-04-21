using System;
using System.IO;

namespace PmxLib;

public class PmxModelInfo : IPmxObjectKey, IPmxStreamIO, ICloneable
{
	PmxObject IPmxObjectKey.ObjectKey => PmxObject.ModelInfo;

	public string ModelName { get; set; }

	public string ModelNameE { get; set; }

	public string Comment { get; set; }

	public string CommentE { get; set; }

	public PmxModelInfo()
	{
		Clear();
	}

	public PmxModelInfo(PmxModelInfo info)
	{
		FromModelInfo(info);
	}

	public void FromModelInfo(PmxModelInfo info)
	{
		ModelName = info.ModelName;
		Comment = info.Comment;
		ModelNameE = info.ModelNameE;
		CommentE = info.CommentE;
	}

	public void Clear()
	{
		ModelName = "";
		Comment = "";
		ModelNameE = "";
		CommentE = "";
	}

	public void FromStreamEx(Stream s, PmxElementFormat f)
	{
		ModelName = PmxStreamHelper.ReadString(s, f);
		ModelNameE = PmxStreamHelper.ReadString(s, f);
		Comment = PmxStreamHelper.ReadString(s, f);
		CommentE = PmxStreamHelper.ReadString(s, f);
	}

	public void ToStreamEx(Stream s, PmxElementFormat f)
	{
		PmxStreamHelper.WriteString(s, ModelName, f);
		PmxStreamHelper.WriteString(s, ModelNameE, f);
		PmxStreamHelper.WriteString(s, Comment, f);
		PmxStreamHelper.WriteString(s, CommentE, f);
	}

	object ICloneable.Clone()
	{
		return new PmxModelInfo(this);
	}

	public PmxModelInfo Clone()
	{
		return new PmxModelInfo(this);
	}
}
