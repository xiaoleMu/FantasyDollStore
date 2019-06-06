using UnityEngine;
using System.Collections;
using System.IO;

namespace TabTale.Plugins.PSDK {

	[System.Serializable]
	public class PsdkInternalSettingsData : ScriptableObject {
		
		public static string ASSET_PATH {
			get {
				if (Application.platform == RuntimePlatform.WindowsEditor) {
					return Path.Combine("Assets",TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath + "\\services\\PublishingSDKCore\\Data\\Resources\\PsdkInternalSettingsData.asset");
				}
				return Path.Combine("Assets",TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath + "/services/PublishingSDKCore/Data/Resources/PsdkInternalSettingsData.asset");
			}
		}

		protected PsdkInternalSettingsData() {
		}

		private static PsdkInternalSettingsData _instance=null;
		public static PsdkInternalSettingsData Instance {
			get {
				if (_instance == null)
					_instance = Resources.Load ("PsdkInternalSettingsData") as PsdkInternalSettingsData;

				if (_instance == null) { // Didn't found asset, create one
					_instance = PsdkInternalSettingsData.CreateInstance<PsdkInternalSettingsData>();
				}

				return _instance;
			}
		}

		public bool socialGamePackage = false;

		public override string ToString() {
			return 
				"socialGamePackage:" + socialGamePackage.ToString()
//					+ ", buildAPK:" + buildAPK.ToString()
//					+ ", updateManifest:" + UpdateManifest.ToString()
				;

		}
	}
}
