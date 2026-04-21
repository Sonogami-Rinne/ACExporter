using System;
using UnityEngine;

[Serializable]
internal class TextureData
{
	public string textureName;

	public Vector2 offset;

	public Vector2 scale;

	public TextureData(Material mat, string prop, string texName)
	{
		if (!mat.HasProperty(prop))
		{
			textureName = null;
			return;
		}
		textureName = texName;
		offset = mat.GetTextureOffset(prop);
		scale = mat.GetTextureScale(prop);
	}
}
