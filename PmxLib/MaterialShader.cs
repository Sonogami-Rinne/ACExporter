using System.Collections.Generic;

namespace PmxLib;

public class MaterialShader
{
	public string shaderName;

	public List<MaterialShaderItem> properties;

	public MaterialShader(string name, List<MaterialShaderItem> properties)
	{
		shaderName = name;
		this.properties = properties;
	}
}
