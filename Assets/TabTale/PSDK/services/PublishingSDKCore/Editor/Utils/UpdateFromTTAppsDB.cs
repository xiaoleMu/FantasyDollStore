using UnityEngine;
using UnityEditor;
using System.IO;
using TabTale.Plugins.PSDK;
using System.Collections;
using System.Collections.Generic;

public class UpdateFromTTAppsDB : MonoBehaviour {

	const string API_VERSION = "v4";
	const string API_RELATIVE_PATH = "build-config";
	const string PRODUCTION_DOMAIN = "gateway.ttpsdk.info";
	const string TESTING_DOMAIN = "tt-apptesting-gatewayzuul.us-west-2.elasticbeanstalk.com";


	[MenuItem("TabTale/PSDK/Update psdk to production configuration")]
	static void MenuUpdateProcuctionFiles() {
		updateFiles (PRODUCTION_DOMAIN);
	}

	[MenuItem("TabTale/PSDK/Update psdk to testing configuration")]
	static void MenuUpdateTestingFiles() {
		updateFiles (TESTING_DOMAIN);
	}
	
	static void updateFiles(string domain) {
		
		string [] stores = { "apple","google","amazon"};
		foreach (string store in stores) {
			Debug.Log ("Updating configuration for " + store);
			string json = getConfigJsonFromUrl(store,PsdkUtils.BundleIdentifier,domain);
			if (json == null) {
				Debug.LogError("Failed update config for store:" + store);
				continue;
			}
			string file_store = "psdk_" + store + ".json";
			if (store == "apple") {
				file_store = "psdk_ios.json";
			}
			File.WriteAllText(Path.Combine(Application.streamingAssetsPath,file_store),json);
		}
	}

	static string getConfigJsonFromUrl(string store, string bundleIdentifier, string domain = PRODUCTION_DOMAIN, string api_version = API_VERSION) {

		if (api_version == API_VERSION) {
			// extract version from PSDK client version
			api_version = "v" + PsdkSerializedData.Instance.psdkCoreVersion.Split('.')[0];
		}

		string url = "http://" + domain + "/" + API_RELATIVE_PATH + "/" + api_version + "/" + store + "/" + bundleIdentifier;
		Debug.Log (url);
		WWW www = new WWW (url);
		while (! www.isDone)
			;
		if (System.String.IsNullOrEmpty (www.error)) {
			IDictionary<string, object> jsonDict = TabTale.Plugins.PSDK.Json.Deserialize( www.text) as IDictionary<string, object>;
			if (jsonDict == null) {
				Debug.LogError("Didn't get dictionary from " + url);
				return null;
			}

			return JsonFormatter.PrettyPrint(www.text);
		}
		
		return null;

	}
}
