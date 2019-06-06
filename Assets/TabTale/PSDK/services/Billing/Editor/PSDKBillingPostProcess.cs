using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class PSDKBillingPostProcess {

	private const string unityIapPath = "Plugins/UnityPurchasing/Resources/BillingMode.json";
	private const string unityIap2017ZipPath = "Assets/TabTale/PSDK/services/Billing/Editor/UnityIAP2017.zip";

	static PSDKBillingPostProcess()
	{
		
// 		#if UNITY_2017_2_OR_NEWER
// 		if(File.Exists(unityIap2017ZipPath)){
// 			Directory.Delete("Assets/Plugins/UnityPurchasing",true);
// 			Directory.Delete("Assets/Plugins/UnityChannel",true);
// 			Debug.Log("Unzipping file");
// 			ZipUtil.Unzip(unityIap2017ZipPath,"Assets/Plugins");
// 			Debug.Log("Finished unzipping file");
// 		}
// 		else {
// 			Debug.LogError("failed to find Unity IAP 2017 updated plugin. This might result in build issues.");
// 		}
// 		#endif

		if(File.Exists(unityIap2017ZipPath)){
			File.Delete(unityIap2017ZipPath);
		}

		if(!AssetDatabase.IsValidFolder("Assets/Plugins"))
			AssetDatabase.CreateFolder("Assets", "Plugins");
		if(!AssetDatabase.IsValidFolder("Assets/Plugins/UnityPurchasing"))
			AssetDatabase.CreateFolder("Assets/Plugins", "UnityPurchasing");
		if(!AssetDatabase.IsValidFolder("Assets/Plugins/UnityPurchasing/resources"))
			AssetDatabase.CreateFolder("Assets/Plugins/UnityPurchasing", "resources");




		if (PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Contains("AMAZON")) {
			Debug.Log("PSDKBillingPostProcess:: amazon selected");
//			UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.AmazonAppStore);

		}
		else {
			Debug.Log("PSDKBillingPostProcess:: google selected");
//			UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.GooglePlay);
		}


		#if AMAZON
		Debug.Log("PSDKBillingPostProcess:: amazon selected");
		UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.AmazonAppStore);
		#else
		Debug.Log("PSDKBillingPostProcess:: google selected");
//		UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.GooglePlay);
		#endif


		string str = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath,unityIapPath));
		Debug.Log("PSDKBillingPostProcess:: " + str);


	}



}
