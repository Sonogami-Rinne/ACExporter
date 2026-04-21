using System;

[Serializable]
internal class ChaFileData
{
	public string Name = "";

	public string Value = "";

	public ChaFileData(string name, string value)
	{
		name = name.Replace(">k__BackingField", "");
		name = name.Replace("<", "");
		Name = name;
		Value = value;
	}
}
