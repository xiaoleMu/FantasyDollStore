using UnityEngine;
using System.Collections;
using System.IO;

public class CCShot
{
	public static Texture2D GetScreenShot(float ratio)
	{
		Camera pCamera = Camera.main;
		int pWidth = (int)(Screen.width * ratio);
		int pHeight = (int)(Screen.height * ratio);

		Texture2D screenshotTexture = new Texture2D(pWidth, pHeight, TextureFormat.RGB24, false);
		RenderTexture screenRT = new RenderTexture(pWidth, pHeight, 24);
		pCamera.targetTexture = screenRT;
		pCamera.Render();
		RenderTexture.active = screenRT;
		screenshotTexture.ReadPixels(new Rect(0, 0, pWidth, pHeight), 0, 0);
		pCamera.targetTexture = null;
		RenderTexture.active = null;
		screenRT.Release();
		RenderTexture.Destroy(screenRT);


		screenshotTexture.Apply();
		screenRT = null;
		return screenshotTexture;
	}

	public static Texture2D GetScreenShotWithWidth(int _ShotWidth, Camera pCamera = null, TextureFormat pTextureFormat = TextureFormat.RGB24)
	{
		if (pCamera == null)
			pCamera = Camera.main;

		int pWidth = (int)(Screen.width * 1);
		int pHeight = (int)(Screen.height * 1);
		int texture_Width = (int)(_ShotWidth * (pHeight / 768f));

		RenderTexture screenRT = new RenderTexture(pWidth, pHeight, 24);
		pCamera.targetTexture = screenRT;
		pCamera.Render();
		RenderTexture.active = screenRT;

		Texture2D screenshotTexture = new Texture2D(texture_Width, pHeight, pTextureFormat, false);
		screenshotTexture.ReadPixels(new Rect((pWidth - _ShotWidth * (pHeight / 768f)) / 2f, 0, _ShotWidth * (pHeight / 768f), pHeight), 0, 0);
		pCamera.targetTexture = null;
		RenderTexture.active = null;
		screenRT.Release();
		Object.Destroy(screenRT);

		screenshotTexture.Apply();
		return screenshotTexture;
	}

	public static Texture2D GetScreenShotWithRect(Rect pRect, Camera pCamera = null,TextureFormat pTextureFormat = TextureFormat.RGB24)
	{
		if (pCamera == null)
			pCamera = Camera.main;

		var screenRT = new RenderTexture((int)pRect.width, (int)pRect.height, 24);
		//		screenRT.antiAliasing = 4;
		pCamera.targetTexture = screenRT;
		pCamera.Render();
		RenderTexture.active = screenRT;

		var screenshotTexture = new Texture2D((int)pRect.width, (int)pRect.height, pTextureFormat, false);
		screenshotTexture.ReadPixels(pRect, 0, 0);
		pCamera.targetTexture = null;
		RenderTexture.active = null;
		screenRT.Release();
		Object.Destroy(screenRT);

		screenshotTexture.Apply();
		return screenshotTexture;
	}

	public static Texture2D GetScreenShotWithRectaa(Rect pRect, Camera pCamera = null)
	{
		if (pCamera == null)
			pCamera = Camera.main;

		var screenshotTexture = new Texture2D((int)pRect.width, (int)pRect.height, TextureFormat.RGB24, false);
		var screenRT = new RenderTexture(Screen.width, Screen.height, 24);
		screenRT.antiAliasing = 4;
		pCamera.targetTexture = screenRT;
		pCamera.Render();
		RenderTexture.active = screenRT;
		screenshotTexture.ReadPixels(pRect, 0, 0);
		pCamera.targetTexture = null;
		RenderTexture.active = null;
		Object.Destroy(screenRT);
		screenRT.Release();
		screenRT = null;

		screenshotTexture.Apply();
		return screenshotTexture;
	}
	/// <summary>
	/// 如果只取局部区域的话，建议用这个截屏方法 - dht
	/// </summary>
	/// <returns>The camera.</returns>
	/// <param name="rect">Rect.</param>
	/// <param name="cameras">Cameras.</param>
	public static Texture2D CaptureCameras (Rect rect, params Camera[] cameras)
	{
		RenderTexture rt = new RenderTexture ((int)Screen.width, (int)Screen.height, 24);
		//临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
		for (int i = 0; i < cameras.Length; i++) {
			if (cameras [i] != null) {
				cameras [i].targetTexture = rt;
				cameras [i].Render ();
			}
		}

		//激活这个rt, 并从中中读取像素。
		RenderTexture.active = rt;
		Texture2D screenShot = new Texture2D ((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
		screenShot.ReadPixels (rect, 0, 0);   //注：这个时候，它是从RenderTexture.active中读取像素
		screenShot.Apply ();

		//重置相关参数，以使用camera继续在屏幕上显示
		for (int i = 0; i < cameras.Length; i++) {
			if (cameras [i] != null)
				cameras [i].targetTexture = null;
		}
		RenderTexture.active = null;
		rt.Release ();
		GameObject.Destroy (rt);
		rt = null;

		return screenShot;
	}

	public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

		float incX = (1.0f / (float)targetWidth);
		float incY = (1.0f / (float)targetHeight);

		for (int i = 0; i < result.height; ++i)
		{
			for (int j = 0; j < result.width; ++j)
			{
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}

		result.Apply();
		return result;
	}

	public static string SaveTexture(string fileName, Texture2D texture)
	{
		if(texture == null)
			return string.Empty;

		string path = Application.temporaryCachePath + "/" + fileName;
		if (File.Exists(path))
			File.Delete(path);
		System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
		return path;
	}

	public static void SaveTextureToPersistentDataPath (Texture2D pTargetTexture, string pTextureName)
	{
		byte[] pTextureBytes = pTargetTexture.EncodeToPNG ();
		System.IO.File.WriteAllBytes (GetTexturePersistentDataPath (pTextureName), pTextureBytes);
	}

	public static string GetTexturePersistentDataPath (string pTextureName)
	{
		string pPath = Application.persistentDataPath + "/" + pTextureName + ".png";
		return pPath;
	}

	public static void DeleteTextureInPersistentDataPath(string pTextureName)
	{
		string path = GetTexturePersistentDataPath (pTextureName);
		if (File.Exists(path))
			File.Delete(path);
	}
}
