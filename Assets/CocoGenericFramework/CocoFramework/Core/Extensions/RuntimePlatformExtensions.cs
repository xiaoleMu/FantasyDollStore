using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class RuntimePlatformExtensions
	{
		public static bool IsEditor(this RuntimePlatform _this)
		{
			switch(_this)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return true;

			default:
				return false;
			}
		}
	}
}
