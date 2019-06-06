using System.IO;
using UnityEngine;


namespace TabTale.Plugins.PSDK {
	
	public static class PsdkVersionsMgr {

		static PsdkVersionsMgr() {
		}


		private static bool IsPackageExist(string pkgName) {
			try {
				// removing .unitypackage.............
				int index = pkgName.IndexOf(".unitypackage");
				if (index > 0) {
					pkgName.Remove(index);
				}
				string fileRelativePath = Path.Combine("psdk",Path.Combine("versions",pkgName + ".unitypackage.version.txt"));
				string version = PsdkUtils.ReadStreamingAssetsFile(fileRelativePath);
					if (version == null)
						return false;

					return true;
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
			return false;
		}

		public static bool IsGLDInstalled() {
			return IsPackageExist("PSDKGameLevelData");
		}
		public static bool IsRewardedAdsInstalled() {
			return IsPackageExist("PSDKRewardedAds");
		}
		public static bool IsMonetizationInstalled() {
			return IsPackageExist("PSDKMonetization");
		}
		public static bool IsBannersInstalled() {
			return IsPackageExist("PSDKBanners");
		}
		public static bool IsSplashInstalled() {
			return IsPackageExist("PSDKSplash");
		}
		public static bool IsGoogleAnalyticsInstalled() {
			return IsPackageExist("PSDKGoogleAnalytics");
		}
		public static bool IsCrashMonitoringToolInstalled() {
			return IsPackageExist("PSDKCrashTool");
		}
		public static bool IsAppsFlyerInstalled() {
			return IsPackageExist("PSDKAppsFlyer");
		}
	}
}
