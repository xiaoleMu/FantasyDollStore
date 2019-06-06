using UnityEngine;

namespace CocoPlay
{
	public class CocoShot
	{
		public static Texture2D CaptureScreen (Rect captureRect, Camera camera = null, float scaleFactor = 1.0f, TextureFormat format = TextureFormat.RGB24,
			int antiAliasing = 1, bool fixAlpha = false)
		{
			// camera
			if (camera == null) {
				camera = Camera.main;
			}
			if (camera == null) {
				Debug.LogError ("CocoShot->CaptureScreenshot: NO available camera!");
				return null;
			}

			// retain rt
			var renderTexture = RetainRenderTexture (ref captureRect, camera, scaleFactor, format, antiAliasing);
			if (renderTexture == null) {
				return null;
			}

			// set target rt
			var originalActiveRT = RenderTexture.active;
			var originalTargetRT = camera.targetTexture;
			RenderTexture.active = renderTexture;
			camera.targetTexture = renderTexture;

			// capture
			var texture = fixAlpha ? CaptureRectFixAlphaTexture (captureRect, camera, format) : CaptureRectTexture (captureRect, camera, format);
			texture.Apply (false);

			// resume target rt
			camera.targetTexture = originalTargetRT;
			RenderTexture.active = originalActiveRT;

			// release rt
			ReleaseRenderTexture (renderTexture);

			return texture;
		}

		private static RenderTexture RetainRenderTexture (ref Rect captureRect, Camera camera, float scaleFactor, TextureFormat textureFormat, int antiAliasing)
		{
			// rt format
			RenderTextureFormat rtFormat;
			switch (textureFormat) {
			case TextureFormat.ARGB32:
			case TextureFormat.RGBA32:
			case TextureFormat.RGB24:
				rtFormat = RenderTextureFormat.ARGB32;
				break;
			case TextureFormat.RGBAFloat:
				rtFormat = RenderTextureFormat.ARGBFloat;
				break;
			case TextureFormat.RGBAHalf:
				rtFormat = RenderTextureFormat.ARGBHalf;
				break;
			default:
				Debug.LogErrorFormat ("CocoShot->CaptureScreenshot: texture format [{0}] NOT supported!", textureFormat);
				return null;
			}

			var rtWidth = camera.pixelWidth;
			var rtHeight = camera.pixelHeight;
			if (!Mathf.Approximately (scaleFactor, 1.0f)) {
				rtWidth = Mathf.RoundToInt (rtWidth * scaleFactor);
				rtHeight = Mathf.RoundToInt (rtHeight * scaleFactor);

				captureRect = CocoMath.Scale (captureRect, scaleFactor);
			}
			captureRect = CocoMath.Round (captureRect);
			//Debug.LogError (captureRect + ", (" + rtWidth + ", " + rtHeight + ")");

			return RenderTexture.GetTemporary (rtWidth, rtHeight, 24, rtFormat, RenderTextureReadWrite.Default, antiAliasing);
		}

		private static void ReleaseRenderTexture (RenderTexture renderTexture)
		{
			RenderTexture.ReleaseTemporary (renderTexture);
			DestroyObject (renderTexture);
		}

		private static Texture2D CaptureRectTexture (Rect captureRect, Camera camera, TextureFormat format)
		{
			camera.Render ();
			var texture = new Texture2D ((int)captureRect.width, (int)captureRect.height, format, false);
			texture.ReadPixels (captureRect, 0, 0, false);
			return texture;
		}

		private static Texture2D CaptureRectFixAlphaTexture (Rect captureRect, Camera camera, TextureFormat format)
		{
			// resume camera original settings
			var originalClearFlags = camera.clearFlags;
			var oriinalBgColor = camera.backgroundColor;

			camera.clearFlags = CameraClearFlags.SolidColor;

			// capture with balck bg
			camera.backgroundColor = Color.black;
			var blackBgTexture = CaptureRectTexture (captureRect, camera, format);

			// capture with white bg
			camera.backgroundColor = Color.white;
			var whiteBgTexture = CaptureRectTexture (captureRect, camera, format);

			// correct real color
			CorrectWhiteBgTextureColors (whiteBgTexture, blackBgTexture);

			// resume camera original settings
			camera.backgroundColor = oriinalBgColor;
			camera.clearFlags = originalClearFlags;

			DestroyObject (blackBgTexture);
			return whiteBgTexture;
		}

		private static void CorrectWhiteBgTextureColors (Texture2D whiteBgTexture, Texture2D blackBgTexture)
		{
			for (var x = 0; x < whiteBgTexture.width; ++x) {
				for (var y = 0; y < whiteBgTexture.height; ++y) {
					var blackBgColor = blackBgTexture.GetPixel (x, y);
					if (blackBgColor == Color.clear) {
						continue;
					}

					var whiteBgColor = whiteBgTexture.GetPixel (x, y);
					CorrectWhiteBgColor (ref whiteBgColor, blackBgColor);
					whiteBgTexture.SetPixel (x, y, whiteBgColor);
				}
			}
		}

		private static void CorrectWhiteBgColor (ref Color whiteBgColor, Color blackBgColor)
		{
			var alpha = 1 - whiteBgColor.r + blackBgColor.r;
			whiteBgColor.r /= alpha;
			whiteBgColor.g /= alpha;
			whiteBgColor.b /= alpha;
			whiteBgColor.a = alpha;
		}

		private static void DestroyObject (Object obj)
		{
			if (Application.isPlaying) {
				Object.Destroy (obj);
			} else {
				Object.DestroyImmediate (obj);
			}
		}
	}
}