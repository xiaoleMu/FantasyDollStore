using UnityEngine;

#if UNITY_IOS && !UNITY_2017_2_OR_NEWER
using System.Runtime.InteropServices;
#endif

namespace CocoPlay
{
	public class SafeAreaBinding
	{
#if UNITY_IOS && !UNITY_2017_2_OR_NEWER
		[DllImport ("__Internal")]
		private static extern void GetSafeAreaImpl (out float x, out float y, out float w, out float h);

		[DllImport ("__Internal")]
		private static extern bool IsSafeAreaInsets ();
#endif

		public static Rect SafeArea {
			get {
#if UNITY_EDITOR
				var safeArea = CocoDebugSettingsData.Instance.SafeAreaRatio;
				safeArea.x *= Screen.width;
				safeArea.y *= Screen.height;
				safeArea.width *= Screen.width;
				safeArea.height *= Screen.height;
				return safeArea;
#elif UNITY_2017_2_OR_NEWER
				return Screen.safeArea;
#elif UNITY_IOS
				float x, y, w, h;
				GetSafeAreaImpl (out x, out y, out w, out h);
				return new Rect (x, y, w, h);
#else
				return new Rect (0, 0, Screen.width, Screen.height);
#endif
			}
		}

		public static bool IsSafeAreaInserts {
			get {
#if UNITY_EDITOR
				var safeAreaRatio = CocoDebugSettingsData.Instance.SafeAreaRatio;
				return !Mathf.Approximately (safeAreaRatio.x, 0f) || !Mathf.Approximately (safeAreaRatio.y, 0f) ||
				       !Mathf.Approximately (safeAreaRatio.width, 1f) || Mathf.Approximately (safeAreaRatio.height, 1f);
#elif UNITY_2017_2_OR_NEWER
				var safeArea = Screen.safeArea;
				return !Mathf.Approximately (safeArea.x, 0f) || !Mathf.Approximately (safeArea.y, 0f) ||
				       !Mathf.Approximately (safeArea.width, Screen.width) || Mathf.Approximately (safeArea.height, Screen.height);
#elif UNITY_IOS
				return IsSafeAreaInsets ();
#else
				return false;
#endif
			}
		}
	}
}