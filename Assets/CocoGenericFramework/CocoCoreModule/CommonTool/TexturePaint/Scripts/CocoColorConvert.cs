using UnityEngine;

namespace CocoPlay
{
	public class CocoColorConvert
	{

		#region HSL <-> RGB

		/// <summary>
		/// convert color mode from hsl (float) to rgb
		/// </summary>

		public static Color Hsl2Rgb (float hue, float saturation, float lightness)
		{
			Color rgb = new Color (lightness, lightness, lightness);

			float temp1, temp2;
			if (saturation != 0) {
				if (lightness < 0.5f) {
					temp2 = lightness * (1 + saturation);
				} else {
					temp2 = (lightness + saturation) - (lightness * saturation);
				}

				temp1 = 2.0f * lightness - temp2;

				rgb.r = H2Rgb (temp1, temp2, hue + 1.0f / 3.0f);
				rgb.g = H2Rgb (temp1, temp2, hue);
				rgb.b = H2Rgb (temp1, temp2, hue - 1.0f / 3.0f);
			}

			return rgb;
		}

		/// <summary>
		/// convert color mode from hsl (Vector3) to rgb
		/// </summary>

		public static Color Hsl2Rgb (Vector3 hsl)
		{
			return Hsl2Rgb (hsl.x, hsl.y, hsl.z);
		}


		static float H2Rgb (float t1, float t2, float h)
		{
			if (h < 0)
				h += 1;
			if (h > 1)
				h -= 1;
			if (6.0f * h < 1)
				return t1 + (t2 - t1) * 6.0f * h;
			if (2.0f * h < 1)
				return t2;
			if (3.0f * h < 2)
				return t1 + (t2 - t1) * ((2.0f / 3.0f) - h) * 6.0f;
			return t1;
		}

		/// <summary>
		/// convert color mode from rgb to hsl
		/// </summary>

		public static Vector3 Rgb2Hsl (Color rgb)
		{
			Vector3 hsl = Vector3.zero;

			float minChannel, maxChannel;
			if (rgb.r > rgb.g) {
				maxChannel = rgb.r;
				minChannel = rgb.g;
			} else {
				maxChannel = rgb.g;
				minChannel = rgb.r;
			}

			if (rgb.b > maxChannel)
				maxChannel = rgb.b;
			if (rgb.b < minChannel)
				minChannel = rgb.b;

			// L
			hsl.z = (maxChannel + minChannel) / 2;

			// S
			float delta = maxChannel - minChannel;
			if (delta != 0) { 
				if (hsl.z < 0.5f) {
					hsl.y = delta / (maxChannel + minChannel);
				} else {
					hsl.y = delta / (2.0f - maxChannel - minChannel);
				}

				// H
				Vector3 deltaRgb;
				deltaRgb.x = ((maxChannel - rgb.r) / 6.0f + delta / 2.0f) / delta;
				deltaRgb.y = ((maxChannel - rgb.g) / 6.0f + delta / 2.0f) / delta;
				deltaRgb.z = ((maxChannel - rgb.b) / 6.0f + delta / 2.0f) / delta;
				if (rgb.r == maxChannel)
					hsl.x = deltaRgb.z - deltaRgb.y;
				else if (rgb.g == maxChannel)
					hsl.x = 1.0f / 3.0f + deltaRgb.x - deltaRgb.z;
				else
					hsl.x = 2.0f / 3.0f + deltaRgb.y - deltaRgb.x;

				if (hsl.x < 0)
					hsl.x += 1;
				else if (hsl.x > 1)
					hsl.x -= 1;
			}

			return hsl;
		}

		#endregion


	}
}