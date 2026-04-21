using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PmxLib;

public class Vpd
{
	public class PoseData
	{
		public Quaternion Rotation;

		public Vector3 Translation;

		public string BoneName { get; set; }

		public PoseData()
		{
		}

		public PoseData(string name, Quaternion r, Vector3 t)
		{
			BoneName = name;
			Rotation = r;
			Translation = t;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{" + BoneName);
			string text = "0.000000";
			stringBuilder.AppendLine("  " + Translation.X.ToString(text) + "," + Translation.Y.ToString(text) + "," + Translation.Z.ToString(text) + ";");
			stringBuilder.AppendLine("  " + Rotation.X.ToString(text) + "," + Rotation.Y.ToString(text) + "," + Rotation.Z.ToString(text) + "," + Rotation.W.ToString(text) + ";");
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}

	public class MorphData
	{
		public float Value;

		public string MorphName { get; set; }

		public MorphData()
		{
		}

		public MorphData(string name, float val)
		{
			MorphName = name;
			Value = val;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{" + MorphName);
			stringBuilder.AppendLine("  " + Value + ";");
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}

	public static string VpdHeader = "Vocaloid Pose Data file";

	private static string HeadGetReg = "^[^V]*Vocaloid Pose Data file";

	private static string InfoGetReg = "^[^V]*Vocaloid Pose Data file[^\\n]*\\n[\\n\\s]*(?<name>[^;]+);[\\s]*[^\\n]*\\n+(?<num>[^;]+);";

	private static string BoneGetReg = "\\n+\\s*Bone(?<no>\\d+)\\s*\\{\\s*(?<name>[^\\r\\n]+)[\\r\\n]+\\s*(?<trans_x>[^,]+),(?<trans_y>[^,]+),(?<trans_z>[^;]+);[^\\n]*\\n+(?<rot_x>[^,]+),(?<rot_y>[^,]+),(?<rot_z>[^,]+),(?<rot_w>[^;]+);[^\\n]*\\n+\\s*\\}";

	private static string MorphGetReg = "\\n+\\s*Morph(?<no>\\d+)\\s*\\{\\s*(?<name>[^\\r\\n]+)[\\r\\n]+\\s*(?<val>[^;]+);[^\\n]*\\n+\\s*\\}";

	private static string NameExt = ".osm";

	public string ModelName { get; set; }

	public List<PoseData> PoseList { get; private set; }

	public List<MorphData> MorphList { get; private set; }

	public bool Extend { get; set; }

	public Vpd()
	{
		PoseList = new List<PoseData>();
		MorphList = new List<MorphData>();
		Extend = true;
	}

	public static bool IsVpdText(string text)
	{
		return new Regex(HeadGetReg, RegexOptions.IgnoreCase).IsMatch(text);
	}

	public bool FromText(string text)
	{
		bool result = false;
		try
		{
			if (!IsVpdText(text))
			{
				return result;
			}
			Match match = new Regex(InfoGetReg, RegexOptions.IgnoreCase).Match(text);
			if (match.Success)
			{
				string text2 = match.Groups["name"].Value;
				if (text2.ToLower().Contains(NameExt))
				{
					text2 = text2.Replace(NameExt, "");
				}
				ModelName = text2;
			}
			PoseList.Clear();
			Match match2 = new Regex(BoneGetReg, RegexOptions.IgnoreCase).Match(text);
			while (match2.Success)
			{
				Vector3 t = new Vector3(0f, 0f, 0f);
				Quaternion identity = Quaternion.Identity;
				string value = match2.Groups["name"].Value;
				float.TryParse(match2.Groups["trans_x"].Value, out var result2);
				float.TryParse(match2.Groups["trans_y"].Value, out var result3);
				float.TryParse(match2.Groups["trans_z"].Value, out var result4);
				t.x = result2;
				t.y = result3;
				t.z = result4;
				float.TryParse(match2.Groups["rot_x"].Value, out result2);
				float.TryParse(match2.Groups["rot_y"].Value, out result3);
				float.TryParse(match2.Groups["rot_z"].Value, out result4);
				float.TryParse(match2.Groups["rot_w"].Value, out var result5);
				identity.x = result2;
				identity.y = result3;
				identity.z = result4;
				identity.w = result5;
				PoseList.Add(new PoseData(value, identity, t));
				match2 = match2.NextMatch();
			}
			MorphList.Clear();
			Match match3 = new Regex(MorphGetReg, RegexOptions.IgnoreCase).Match(text);
			while (match3.Success)
			{
				float result6 = 0f;
				string value2 = match3.Groups["name"].Value;
				float.TryParse(match3.Groups["val"].Value, out result6);
				MorphList.Add(new MorphData(value2, result6));
				match3 = match3.NextMatch();
			}
			result = true;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(VpdHeader);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(ModelName + NameExt + ";");
		stringBuilder.AppendLine(PoseList.Count + ";");
		stringBuilder.AppendLine();
		for (int i = 0; i < PoseList.Count; i++)
		{
			stringBuilder.AppendLine("Bone" + i + PoseList[i].ToString());
		}
		if (Extend)
		{
			for (int j = 0; j < MorphList.Count; j++)
			{
				stringBuilder.AppendLine("Morph" + j + MorphList[j].ToString());
			}
		}
		return stringBuilder.ToString();
	}
}
