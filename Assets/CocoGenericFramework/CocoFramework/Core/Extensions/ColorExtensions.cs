using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class ColorExtensions
	{
		public static string ToHex(this Color _this, bool alpha = true)
		{
			return ColorUtils.ColorToHex(_this, alpha);
		}
	}
}
