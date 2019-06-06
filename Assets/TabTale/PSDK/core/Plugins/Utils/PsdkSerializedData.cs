using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class PsdkSerializedData : ScriptableObject {
	
	public static string ASSET_PATH {
		get {
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				return Path.Combine("Assets",TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath + "\\services\\PublishingSDKCore\\Data\\Resources\\PsdkSerializedData.asset");
			}
			return Path.Combine("Assets",TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath + "/services/PublishingSDKCore/Data/Resources/PsdkSerializedData.asset");
		}
	}

	protected PsdkSerializedData() {
		
	}

	private static PsdkSerializedData _instance=null;
	public static PsdkSerializedData Instance {
		get {
			if (_instance == null) {
				_instance = Resources.Load ("PsdkSerializedData") as PsdkSerializedData;
                if(_instance != null)
				    _instance.hideFlags = HideFlags.NotEditable;
			}
			return _instance;
		}
	}

	public string unityVersion;
	public string productName;
	public string bundleIdentifier;
	public string bundleVersion;
	public string androidBundleVersionCode;
	public string psdkVersion;
	public string buildTime;
	public string unityScriptingBackend;

	public string psdkCoreVersion {
		get {
			string coreVersion = psdkVersion;
			int psdkCoreIndex = coreVersion.IndexOf ("PSDKCore:") + 9;
			coreVersion = coreVersion.Substring (psdkCoreIndex, Mathf.Min(coreVersion.IndexOf (",", psdkCoreIndex),coreVersion.Length) - psdkCoreIndex);
			return coreVersion;
		}
	}

	public override string ToString() {
		return 
			unityVersion 
			+ " " + productName
			+ " " + bundleIdentifier
			+ " " + bundleVersion
			+ " " + androidBundleVersionCode
			+ " " + buildTime
			+ " " + unityScriptingBackend
			+ " " + psdkVersion
			;

	}
}
