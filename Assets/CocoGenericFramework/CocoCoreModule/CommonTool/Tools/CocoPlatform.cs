using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoPlatform
	{
		public static string LocalPathProtocalHeader {
			get {
				switch (Application.platform) {
				case RuntimePlatform.Android:
					return string.Empty;
				}

				return "file://";
			}
		}

		public static string StreamingAssetRootPath {
			get {
				return LocalPathProtocalHeader + Application.streamingAssetsPath + "/";
			}
		}
	}
}
