using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class GeomUtils
	{
		public static float ClampDegrees(float angle)
		{
			while(angle > 360)
				angle -= 360;

			while(angle < -360)
				angle += 360;

			return angle;
		}
	}
}