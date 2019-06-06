using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay
{
	public class CocoTexture
	{

		#region Texture

		/**
		 * Create Texture (Color32), faster but only support ARGB32
		 */
		public static Texture2D CreateColor32Texture (int width, int height, bool applied, TextureFormat format, Color32 color32)
		{
			switch (format) {
			case TextureFormat.ARGB32:
				Texture2D texture = new Texture2D (width, height, TextureFormat.ARGB32, false);
				texture.SetPixels32 (CocoData.CreateArray<Color32> (width * height, color32));
				if (applied)
					texture.Apply ();
				return texture;
			}

			Debug.LogError ("CocoTexture->CreateColor32Texture: texture format [" + format + "] NOT supported.");
			return null;
		}

		/**
		 * Create Texture (Color), slower and support ARGB32, RGB24, Alpha8
		 */
		public static Texture2D CreateColorTexture (int width, int height, bool applied, TextureFormat format, Color color)
		{
			switch (format) {
			case TextureFormat.ARGB32:
			case TextureFormat.RGB24:
			case TextureFormat.Alpha8:
				Texture2D texture = new Texture2D (width, height, format, false);
				texture.SetPixels (CocoData.CreateArray<Color> (width * height, color));
				if (applied)
					texture.Apply ();
				return texture;
			}

			Debug.LogError ("CocoTexture->CreateColorTexture: texture format [" + format + "] NOT supported.");
			return null;
		}

		#endregion


		#region Texture Blend

		/**
		 * Blend color in texture
	 	*/
		public static void BlendTextureColor (Texture2D texture, Color color, CocoColorBlend.BlendAlgorithm blendFunc, float mixingFactor = 1)
		{
			Color[] texColors = texture.GetPixels ();
			Color tempColor;
			for (int i = 0; i < texColors.Length; i++) {
				tempColor = blendFunc (texColors [i], color);
				tempColor.r *= mixingFactor;
				tempColor.g *= mixingFactor;
				tempColor.b *= mixingFactor;
				texColors [i] = tempColor;
			}
			texture.SetPixels (texColors);
		}

		/**
		 * Blend color(hsl) in texture
	 	*/
		public static void BlendTextureColorHSL (Texture2D texture, Vector3 hsl, CocoColorBlend.BlendAlgorithm blendFunc, float mixingFactor = 1)
		{
			BlendTextureColor (texture, CocoColorConvert.Hsl2Rgb (hsl), blendFunc, mixingFactor);
		}

		#endregion

	}
}