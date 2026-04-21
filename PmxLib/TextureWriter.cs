using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace PmxLib;

public class TextureWriter
{
	public static readonly List<string> normalMapProperties = new List<string> { "_NormalMap", "_NormalMapDetail", "_BumpMap" };

	public static Texture2D ToTexture2D(RenderTexture renderTexture)
	{
		int width = renderTexture.width;
		int height = renderTexture.height;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		texture2D.Apply();
		return texture2D;
	}

	public static bool ConvertNormalMap(ref Texture tex, string propertyName)
	{
		if (!normalMapProperties.Contains(propertyName))
		{
			return false;
		}
		Texture2D tex2 = ((!(tex is RenderTexture)) ? (tex as Texture2D) : ToTexture2D(tex as RenderTexture));
		MakeTextureReadable(ref tex2);
		Color[] pixels = tex2.GetPixels(0);
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].r = 1f;
		}
		tex2.SetPixels(pixels, 0);
		tex2.Apply(updateMipmaps: true);
		RenderTexture renderTexture = new RenderTexture(tex2.width, tex2.height, 0);
		renderTexture.useMipMap = true;
		RenderTexture.active = renderTexture;
		Graphics.Blit(tex2, renderTexture);
		tex = renderTexture;
		return true;
	}

	private static void MakeTextureReadable(ref Texture2D tex, RenderTextureFormat rtf = RenderTextureFormat.Default, RenderTextureReadWrite cs = RenderTextureReadWrite.Default)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(tex.width, tex.height, 0, rtf, cs);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		GL.Clear(clearDepth: false, clearColor: true, new Color(0f, 0f, 0f, 0f));
		Graphics.Blit(tex, temporary);
		tex = GetT2D(temporary);
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		tex.Apply(updateMipmaps: true);
	}

	public static Texture2D GetT2D(RenderTexture renderTexture)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
		texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
		RenderTexture.active = active;
		return texture2D;
	}

	//public static void SaveTexR(RenderTexture renderTexture, string path)
	//{
	//	Texture2D t2D = GetT2D(renderTexture);
	//	byte[] bytes = t2D.EncodeToPNG();
	//	new Thread((ThreadStart)delegate
	//	{
	//		Thread.CurrentThread.IsBackground = false;
	//		File.WriteAllBytes(path, bytes);
	//	}).Start();
	//	Object.DestroyImmediate(t2D);
	//}

	//public static void SaveTex(Texture tex, string path, RenderTextureFormat rtf = RenderTextureFormat.Default, RenderTextureReadWrite cs = RenderTextureReadWrite.Default)
	//{
	//	RenderTexture temporary = RenderTexture.GetTemporary(tex.width, tex.height, 0, rtf, cs);
	//	RenderTexture active = RenderTexture.active;
	//	RenderTexture.active = temporary;
	//	GL.Clear(clearDepth: false, clearColor: true, new Color(0f, 0f, 0f, 0f));
	//	Graphics.Blit(tex, temporary);
	//	SaveTexR(temporary, path);
	//	RenderTexture.active = active;
	//	RenderTexture.ReleaseTemporary(temporary);
	//}
}
