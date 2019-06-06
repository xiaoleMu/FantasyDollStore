using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_5_6_OR_NEWER
using UnityEditor.Build;
#endif

namespace TabTale.Plugins.PSDK  {

	#if !UNITY_5_6_OR_NEWER
	[InitializeOnLoad]
	#endif
	public class DownloadLocalConsentForm 
	#if UNITY_5_6_OR_NEWER 
	: IPreprocessBuild
	#endif 
	{
		private const string STREAMING_ASSETS_PATH_JSON = "/StreamingAssets/psdk_";
		private const string STREAMING_ASSETS_PATH_CONSENT_ANDROID = "/Plugins/Android/assets/consentForm";
		private const string STREAMING_ASSETS_PATH_PRIVACY_ANDROID = "/Plugins/Android/assets/privacyForm";
		private const string STREAMING_ASSETS_PATH_CONSENT = "/StreamingAssets/consentForm";
		private const string STREAMING_ASSETS_PATH_PRIVACY = "/StreamingAssets/privacyForm";
		private const string STREAMING_ASSETS_PATH_CONSENT_ZIP = "/StreamingAssets/consentForm.zip";
		private const string STREAMING_ASSETS_PATH_PRIVACY_ZIP = "/StreamingAssets/privacyForm.zip";
		private const string STREAMING_ASSETS_PATH_HTML = "/StreamingAssets/index.html";

		private static DownloadLocalConsentForm _staticInsntance;

		#if !UNITY_5_6_OR_NEWER
		static DownloadLocalConsentForm() {
			if(!Directory.Exists(Application.dataPath + STREAMING_ASSETS_PATH_CONSENT)){
				_staticInsntance = new DownloadLocalConsentForm();
				Debug.Log("DownloadLocalConsentForm:: did not find consent form folder, downloading.");
				_staticInsntance.OnPreprocessBuild(EditorUserBuildSettings.activeBuildTarget,"");
			}
		}
		#endif

		public int callbackOrder 
		{ 
			get 
			{ 
				return 0; 
			} 
		}

		private void StartCoroutine (IEnumerator e)
		{
			while (e.MoveNext())
				;
		}

		public void OnPreprocessBuild (BuildTarget target, string path)
		{
			string consentFormUrl = null;
			string privacyFormUrl = null;
			string store = null;
			if(target == BuildTarget.Android){
				store = "google.json";
			}
			else if (target == BuildTarget.iOS){
				store = "ios.json";
			}
			string jsonFp = Application.dataPath + STREAMING_ASSETS_PATH_JSON + store;
			if(File.Exists(jsonFp)){
				try {
					string jsonStr = File.ReadAllText(jsonFp);
					if(jsonStr != null){
						Dictionary<string, object> jsonDict = Json.Deserialize(jsonStr) as Dictionary<string, object>;
						if(jsonDict != null){
							Dictionary<string, object> globalDict = jsonDict["consent"] as Dictionary<string, object>;
							if(globalDict != null){
								consentFormUrl = globalDict["consentFormURL"] as string;
								privacyFormUrl = globalDict["privacySettingsURL"] as string;
							}
						}
					}
				}
				catch (System.Exception e) {

				}

			}
			if(string.IsNullOrEmpty(consentFormUrl) || string.IsNullOrEmpty(privacyFormUrl)){
				Debug.Log("DownloadLocalConsentForm:: consentFormUrl or privacySettingsURL do not exist - aborting.");
			}
			else {
				string consentDst = "";
				string privacyDst = "";
				if(target == BuildTarget.Android){
					consentDst = Application.dataPath + STREAMING_ASSETS_PATH_CONSENT_ANDROID;
					privacyDst = Application.dataPath + STREAMING_ASSETS_PATH_PRIVACY_ANDROID;
				}
				else if(target == BuildTarget.iOS){
					consentDst = Application.dataPath + STREAMING_ASSETS_PATH_CONSENT;
					privacyDst = Application.dataPath + STREAMING_ASSETS_PATH_PRIVACY;
				}
				StartCoroutine(DownloadConsentForms(consentFormUrl,Application.dataPath + STREAMING_ASSETS_PATH_CONSENT_ZIP,consentDst));
				StartCoroutine(DownloadConsentForms(privacyFormUrl,Application.dataPath + STREAMING_ASSETS_PATH_PRIVACY_ZIP,privacyDst));
			}
		}

		private IEnumerator DownloadConsentForms(string url, string pathToZip, string unzipFolder)
		{
			string exceptionMessage = null;
			using(UnityEngine.WWW www = new UnityEngine.WWW(url)){
				while(!www.isDone);
				yield return www;
				if(string.IsNullOrEmpty(www.error)){
					try {
						if(!Directory.Exists(unzipFolder)){
							Directory.CreateDirectory(unzipFolder);
							File.WriteAllBytes(pathToZip ,www.bytes);
							ZipUtil.Unzip(pathToZip, unzipFolder);
							if(!File.Exists(unzipFolder + "/index.html")){
								exceptionMessage = "Html for consent form not found. Something must have went wrong with the download process.";
							}
							File.Delete(pathToZip);
						}

					}
					catch (System.Exception e) {
						exceptionMessage = "Failed to write consent form zip to file system. Exception - " + e.Message;
					}

				}
				else {
					exceptionMessage = "Failed to retrieve consent form from server. Error - " + www.error;
				}
				if(exceptionMessage != null){
					throw new System.Exception(exceptionMessage);
				}
				else {
					Debug.Log("Successfully downloaded consent and privacy forms");
				}
			}
		}
	}
}
