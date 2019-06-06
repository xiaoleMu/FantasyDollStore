using UnityEngine;

namespace CocoPlay
{
	public class CocoColorBlend
	{
		#region  Blend Algorithm

		public delegate Color BlendAlgorithm (Color colorA, Color colorB);

		public static Color Multiply (Color colorA, Color colorB)
		{
			return colorA * colorB;
		}

		public static Color Multiply_NoAlpha (Color colorA, Color colorB)
		{
			return new Color (colorA.r * colorB.r, colorA.g * colorB.g, colorA.b * colorB.b, colorA.a);
		}

		#endregion


		#region Replace Hue

		public static Color ReplaceHue (Color color, float hue)
		{
			float h, s, v;
			Color.RGBToHSV (color, out h, out s, out v);
			return Color.HSVToRGB (hue, s, v);
		}

		public static Color ReplaceHue (Color colorA, Color colorB)
		{
			float h, s, v;
			Color.RGBToHSV (colorB, out h, out s, out v);
			return ReplaceHue (colorA, h);
		}

		public static Color ReplaceHue (Color color, float hue, Vector3 saturationFix, Vector3 lightFix)
		{
			float h, s, v;
			Color.RGBToHSV (color, out h, out s, out v);

			if (s < saturationFix.x) {
				s = s * saturationFix.y + saturationFix.z;
			}
			if (v > lightFix.x) {
				v = v * lightFix.y + lightFix.z;
			}

			return Color.HSVToRGB (hue, s, v);
		}

		public static Color ReplaceHue (Color colorA, Color colorB, Vector3 saturationFix, Vector3 lightFix)
		{
			float h, s, v;
			Color.RGBToHSV (colorB, out h, out s, out v);
			return ReplaceHue (colorA, h, saturationFix, lightFix);
		}

		#endregion
	}
}