using UnityEngine;
using System;

namespace TabTale
{
	public static class ColorUtils
	{
		// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
		public static string ColorToHex(Color32 color, bool alpha = false)
		{
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			if(alpha)
			{
				hex += color.a.ToString("X2");
			}

			return hex;
		}
		
		public static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);

			byte a = 255;
			if(hex.Length > 6)
			{
				a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			}

			return new Color32(r,g,b, a);
		}

		public static void SetTextureColor(Texture2D texture, Color color)
		{
			for(int i=0;i<texture.width;i++)
			{
				for(int j=0;j<texture.height;j++)
				{
					texture.SetPixel(i, j, color);
				}
			}
		}

		public static Color FromArgb (int alpha, int red, int green, int blue)
		{
			float fa = ((float)alpha) / 255.0f;
			float fr = ((float)red) / 255.0f;
			float fg = ((float)green) / 255.0f;
			float fb = ((float)blue) / 255.0f;
			return new Color (fr, fg, fb, fa);
		}


		public static void ColorToHSV (Color color, out double hue, out double saturation, out double value)
		{
			int max = Math.Max (color.R (), Math.Max (color.G (), color.B ()));
			int min = Math.Min (color.R (), Math.Min (color.G (), color.B ()));

			hue = color.GetHue ();
			saturation = (max == 0) ? 0 : 1d - (1d * min / max);
			value = max / 255d;
		}

		public static Color ColorFromHSV (double hue, double saturation, double value)
		{
			int hi = Convert.ToInt32 (Math.Floor (hue / 60)) % 6;
			double f = hue / 60 - Math.Floor (hue / 60);

			value = value * 255;
			int v = Convert.ToInt32 (value);
			int p = Convert.ToInt32 (value * (1 - saturation));
			int q = Convert.ToInt32 (value * (1 - f * saturation));
			int t = Convert.ToInt32 (value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return FromArgb (255, v, t, p);
			else if (hi == 1)
				return FromArgb (255, q, v, p);
			else if (hi == 2)
				return FromArgb (255, p, v, t);
			else if (hi == 3)
				return FromArgb (255, p, q, v);
			else if (hi == 4)
				return FromArgb (255, t, p, v);
			else
				return FromArgb (255, v, p, q);
		}

//		public static Color RGB2GRAY (Color c)
//		{
//			int r = c.R ();
//			int g = c.G ();
//			int b = c.B ();
//			int a = c.A ();
//
//			int gray = (int)(r * 0.3f + g * 0.59f + b * 0.11f);
//			gray += (255 - gray) / 4;
//			return FromArgb (a, gray, gray, gray);
//		}

		public static Color RGB2GRAY (Color c)
		{
			float gray = c.r * 0.3f + c.g * 0.59f + c.b * 0.11f;
			float tDis = 0.35f;
			gray *= 1f - (tDis * 2);
			gray += tDis*1.2f;
			int tGray = (int)(gray * 255);
			return FromArgb (c.A (), tGray, tGray, tGray);
		}

		public static Color RGB2GRAYBlack (Color c)
		{
			float gray = c.r * 0.3f + c.g * 0.59f + c.b * 0.11f;
			float tDis = 0.35f;
			gray *= 1f - (tDis * 2);
			gray += tDis;
			gray /= 2f;
			int tGray = (int)(gray * 255);
			return FromArgb (c.A (), tGray, tGray, tGray);
		}

		/// <summary>
		/// 反色
		/// </summary>
		/// <param name="c">C.</param>
		public static Color Opposite (Color c)
		{
			int r = c.R ();
			int g = c.G ();
			int b = c.B ();
			int a = c.A ();

			return FromArgb (a, 255 - r, 255 - g, 255 - b);
		}

		public static int G (this Color col)
		{
			return (int)(col.g * 255.0f);
		}

		public static int B (this Color col)
		{
			return (int)(col.b * 255.0f);
		}

		public static int R (this Color col)
		{
			return (int)(col.r * 255.0f);
		}

		public static int A (this Color col)
		{
			return (int)(col.a * 255.0f);
		}

		public static float GetHue (this Color c)
		{
			int r = c.R ();
			int g = c.G ();
			int b = c.B ();

			byte minval = (byte)Math.Min (r, Math.Min (g, b));
			byte maxval = (byte)Math.Max (r, Math.Max (g, b));

			if (maxval == minval)
				return 0.0f;

			float diff = (float)(maxval - minval);
			float rnorm = (maxval - r) / diff;
			float gnorm = (maxval - g) / diff;
			float bnorm = (maxval - b) / diff;

			float hue = 0.0f;
			if (r == maxval)
				hue = 60.0f * (6.0f + bnorm - gnorm);
			if (g == maxval)
				hue = 60.0f * (2.0f + rnorm - bnorm);
			if (b == maxval)
				hue = 60.0f * (4.0f + gnorm - rnorm);
			if (hue > 360.0f)
				hue = hue - 360.0f;

			return hue;
		}
	}
}