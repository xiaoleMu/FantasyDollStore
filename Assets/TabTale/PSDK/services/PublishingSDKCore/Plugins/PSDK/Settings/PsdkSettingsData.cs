using UnityEngine;
using System.Collections;
using System.IO;

namespace TabTale.Plugins.PSDK {

	[System.Serializable]
	public class PsdkSettingsData : ScriptableObject {
		
		public static string ASSET_PATH {
			get {
				if (Application.platform == RuntimePlatform.WindowsEditor) {
					return Path.Combine("Assets",TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath + "\\services\\PublishingSDKCore\\Data\\Resources\\PsdkSettingsData.asset");
				}
				return Path.Combine("Assets",TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath + "/services/PublishingSDKCore/Data/Resources/PsdkSettingsData.asset");
			}
		}

		protected PsdkSettingsData() {
		}

		private static PsdkSettingsData _instance=null;
		public static PsdkSettingsData Instance {
			get {
				if (_instance == null)
					_instance = Resources.Load ("PsdkSettingsData") as PsdkSettingsData;

				if (_instance == null) { // Didn't found asset, create one
					_instance = PsdkSettingsData.CreateInstance<PsdkSettingsData>();
				}

				return _instance;
			}
		}

		public bool universalBuild = true;
		public bool buildAPK = true;
		public bool UpdateManifest = true;

		public override string ToString() {
			return 
				"universalBuild:" + universalBuild.ToString()
					+ ", buildAPK:" + buildAPK.ToString()
					+ ", updateManifest:" + UpdateManifest.ToString()
				;

		}
	}
}
