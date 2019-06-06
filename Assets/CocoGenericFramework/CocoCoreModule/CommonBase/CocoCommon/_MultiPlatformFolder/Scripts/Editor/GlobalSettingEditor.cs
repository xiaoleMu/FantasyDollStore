using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System.Xml;
using System;

namespace CocoPlay
{
	[CustomEditor (typeof(GlobalSettingData))]
	public class GlobalSettingEditor : Editor
	{
		#region 处理设置面板

		private const string ASSET_PATH = "Assets/_Game/Resources/GlobalSettingData.asset";

		[MenuItem ("CocoPlay/Platform/Platform Settings", false, 105)]
		private static void FocusCocoStoreDataAssetObject ()
		{
			if (GlobalSettingData.Instance != null) {
				Selection.activeObject = GlobalSettingData.Instance;

				return;
			}

			Debug.LogError ("文件夹下不存在 GlobalSettingData.asset， 所以创建了一个，放在： " + ASSET_PATH);

			var asset = CreateInstance<GlobalSettingData> ();
			AssetDatabase.CreateAsset (asset, ASSET_PATH);
			Selection.activeObject = GlobalSettingData.Instance;

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}

		private bool sureClearAll;
		private static bool _switchPlatform;

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			var box = EditorGUILayout.BeginVertical ();

			var pSettingData = GlobalSettingData.Instance;
			if (pSettingData == null) return;

			var pAllItems = pSettingData.allPlatformItems;
			GUILayout.Label ("Current items number (当前平台数量)： " + pAllItems.Count.ToString ());

			var pStartPosY = 20;

			var pRect = new Rect (box.xMin, box.yMin + pStartPosY, 200, 20);
			if (GUI.Button (pRect, "Add item (添加一个平台配置)")) {
				pAllItems.Add (new MultiPlatformItem ());
			}

			GUILayout.Space (30);
			sureClearAll = EditorGUILayout.Toggle ("Sure to Clear All (确认清除)：", sureClearAll);

			pStartPosY += 50;
			pRect = new Rect (box.xMin, box.yMin + pStartPosY, 120, 20);

			if (sureClearAll) {
				if (GUI.Button (pRect, "Clear All (清除所有)")) {
					pAllItems.Clear ();
					sureClearAll = false;
				}
			}

			GUILayout.Space (40);

			pSettingData.isDebugMode = EditorGUILayout.Toggle ("开启测试模式：", pSettingData.isDebugMode);
			_switchPlatform = EditorGUILayout.Toggle ("切换引擎平台：", _switchPlatform);

			for (var i = 0; i < pAllItems.Count; i++) {
				if (EditCocoStoreItem (i)) {
					pAllItems.RemoveAt (i);
					i--;
				}
				GUILayout.Space (10);
			}

			EditorUtility.SetDirty (GlobalSettingData.Instance);
			EditorGUILayout.EndVertical ();
			serializedObject.ApplyModifiedProperties ();
		}

		private bool EditCocoStoreItem (int pIndex)
		{
			if (GlobalSettingData.Instance == null) {
				return false;
			}

			var pIsRemoveData = false;

			var pPlatformItem = GlobalSettingData.Instance.allPlatformItems [pIndex];

			var box = EditorGUILayout.BeginVertical (GUI.skin.box);

			var pRect = new Rect (box.xMax - 50, box.yMin + 2, 50, 15);
			if (GUI.Button (pRect, "X")) {
				pIsRemoveData = true;
			}

			pRect = new Rect (box.xMax - 180, box.yMin + 2, 100, 15);
			if (GUI.Button (pRect, "选择平台")) {
				SwitchPlatformByItem (pPlatformItem);
			}

			pPlatformItem.foldout = EditorGUILayout.Foldout (pPlatformItem.foldout, pPlatformItem.curPlatform.ToString ());
			GUILayout.Space (8);

			if (pPlatformItem.foldout) {
				if (pPlatformItem.isSelected) {
					EditorGUILayout.Toggle ("已选择", pPlatformItem.isSelected);
				}

				pPlatformItem.curPlatform = (MultiPlatformType)EditorGUILayout.EnumPopup ("curPlatform (平台类型): ", pPlatformItem.curPlatform);
				pPlatformItem.bundleId = EditorGUILayout.TextField ("bundleId: ", pPlatformItem.bundleId);
				pPlatformItem.bundleVersion = EditorGUILayout.TextField ("bundleVersion(版本号): ", pPlatformItem.bundleVersion);

				pPlatformItem.iconFolder = EditorGUILayout.TextField ("IconFolder(图标文件夹): ", pPlatformItem.iconFolder);

				pPlatformItem.allowOrientations =
					(MultiPlatformItem.Orientation)EditorGUILayout.EnumMaskPopup ("orientations(允许屏幕方向)", pPlatformItem.allowOrientations);

				switch (pPlatformItem.curPlatform) {
				case MultiPlatformType.iOS:

					break;
				case MultiPlatformType.GooglePlay:
				case MultiPlatformType.Amazon:
					pPlatformItem.bundleVersionCode = EditorGUILayout.IntField ("bundleVersionCode: ", pPlatformItem.bundleVersionCode);

					pPlatformItem.keystoreName = EditorGUILayout.TextField ("keystoreName: ", pPlatformItem.keystoreName);
					pPlatformItem.keystorePass = EditorGUILayout.TextField ("keystorePass: ", pPlatformItem.keystorePass);
					pPlatformItem.keyaliasName = EditorGUILayout.TextField ("keyaliasName: ", pPlatformItem.keyaliasName);
					pPlatformItem.keyaliasPass = EditorGUILayout.TextField ("keyaliasPass: ", pPlatformItem.keyaliasPass);

					break;
				}

				pPlatformItem.scriptingDefs = EditorGUILayout.TextField ("scriptingDefs(宏定义): ", pPlatformItem.scriptingDefs);

				pPlatformItem.splitBinary = EditorGUILayout.Toggle ("splitBinary(分包): ", pPlatformItem.splitBinary);
			}

			GlobalSettingData.Instance.allPlatformItems [pIndex] = pPlatformItem;

			EditorGUILayout.EndVertical ();

			return pIsRemoveData;
		}

		#endregion


		#region 处理平台切换

		private static void UnSelectAllPlatformItems ()
		{
			if (GlobalSettingData.Instance == null) return;

			foreach (var platformItem in GlobalSettingData.Instance.allPlatformItems) {
				platformItem.isSelected = false;
			}
		}

		private static void SwitchPlatformByItem (MultiPlatformItem pPlatformItem)
		{
			var targetGroup = GetBuildTargetGroup (pPlatformItem);
			if (targetGroup == BuildTargetGroup.Unknown) {
				Debug.LogError ("SwitchPlatformByItem: Current platform [" + pPlatformItem.curPlatform + "] is not support ! (当前平台不支持)");
				return;
			}

			UnSelectAllPlatformItems ();

			pPlatformItem.isSelected = true;
			if (_switchPlatform) {
				switch (pPlatformItem.curPlatform) {
				case MultiPlatformType.iOS:
					EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.iOS, BuildTarget.iOS);
					break;
				case MultiPlatformType.GooglePlay:
				case MultiPlatformType.Amazon:
					EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTargetGroup.Android, BuildTarget.Android);
					break;
				}
			}

			SetAndroidAPILevel (pPlatformItem);
			SetSplitBinaryOption (pPlatformItem);
			SetOrientationAndSplash (pPlatformItem);
			SetPlatformIcons (pPlatformItem);
			SetScriptingDefineSymbols (pPlatformItem);
			SetBillingModeFile (pPlatformItem);
			SetAndroidManifestFile (pPlatformItem);
		}

		private static void SetAndroidAPILevel (MultiPlatformItem pPlatformItem)
		{
			PlayerSettings.companyName = "Cocoplay";

#if UNITY_5_6_OR_NEWER
			var targetGroup = GetBuildTargetGroup (pPlatformItem);
			PlayerSettings.SetApplicationIdentifier (targetGroup, pPlatformItem.bundleId);
			// PlayerSettings.applicationIdentifier = pPlatformItem.bundleId;
#else
			PlayerSettings.bundleIdentifier = pPlatformItem.bundleId;
#endif

			PlayerSettings.bundleVersion = pPlatformItem.bundleVersion;

			PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies;

			switch (pPlatformItem.curPlatform) {
			case MultiPlatformType.iOS:
				break;
			case MultiPlatformType.GooglePlay:
				break;
			case MultiPlatformType.Amazon:
				break;
			default:
				throw new ArgumentOutOfRangeException ();
			}


			PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
			// PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_7_0;
			PlayerSettings.iOS.allowHTTPDownload = false;
			PlayerSettings.SetUseDefaultGraphicsAPIs (BuildTarget.iOS, true);

			PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16; //
			PlayerSettings.Android.bundleVersionCode = pPlatformItem.bundleVersionCode;

			PlayerSettings.Android.keystoreName = pPlatformItem.keystoreName;
			PlayerSettings.Android.keystorePass = pPlatformItem.keystorePass;
			PlayerSettings.Android.keyaliasName = pPlatformItem.keyaliasName;
			PlayerSettings.Android.keyaliasPass = pPlatformItem.keyaliasPass;

			PlayerSettings.SetUseDefaultGraphicsAPIs (BuildTarget.Android, false);
			var pApisGroup = new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 };
			PlayerSettings.SetGraphicsAPIs (BuildTarget.Android, pApisGroup);
		}

		public static void SetOrientationAndSplash (MultiPlatformItem pPlatformItem)
		{
			var orientationCount = 0;
			var defaultOrientation = UIOrientation.LandscapeLeft;

			if (IsOrientationAllowed (pPlatformItem, MultiPlatformItem.Orientation.PortraitUpsideDown)) {
				PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
				orientationCount++;
				defaultOrientation = UIOrientation.PortraitUpsideDown;
			} else {
				PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
			}

			if (IsOrientationAllowed (pPlatformItem, MultiPlatformItem.Orientation.Portrait)) {
				PlayerSettings.allowedAutorotateToPortrait = true;
				orientationCount++;
				defaultOrientation = UIOrientation.Portrait;
			} else {
				PlayerSettings.allowedAutorotateToPortrait = false;
			}

			if (IsOrientationAllowed (pPlatformItem, MultiPlatformItem.Orientation.LandscapeRight)) {
				PlayerSettings.allowedAutorotateToLandscapeRight = true;
				orientationCount++;
				defaultOrientation = UIOrientation.LandscapeRight;
			} else {
				PlayerSettings.allowedAutorotateToLandscapeRight = false;
			}

			if (IsOrientationAllowed (pPlatformItem, MultiPlatformItem.Orientation.LandscapeLeft)) {
				PlayerSettings.allowedAutorotateToLandscapeLeft = true;
				orientationCount++;
				defaultOrientation = UIOrientation.LandscapeLeft;
			} else {
				PlayerSettings.allowedAutorotateToLandscapeLeft = false;
			}

			switch (orientationCount) {
			case 0:
				// no set, use default
				SetDefaultOrientation (pPlatformItem);
				break;
			case 1:
				PlayerSettings.defaultInterfaceOrientation = defaultOrientation;
				break;
			default:
				PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
				break;
			}

			Debug.LogFormat ("GlobalSettingEditor->SetOrientationAndSplash: orientation default[{0}], left [{1}], right [{2}], up [{3}], down [{4}]",
				PlayerSettings.defaultInterfaceOrientation,
				PlayerSettings.allowedAutorotateToLandscapeLeft, PlayerSettings.allowedAutorotateToLandscapeRight,
				PlayerSettings.allowedAutorotateToPortrait, PlayerSettings.allowedAutorotateToPortraitUpsideDown);

			PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFill;
		}

		private static bool IsOrientationAllowed (MultiPlatformItem pPlatformItem, MultiPlatformItem.Orientation orientation)
		{
			return (pPlatformItem.allowOrientations & orientation) == orientation;
		}

		private static void SetDefaultOrientation (MultiPlatformItem pPlatformItem)
		{
			switch (pPlatformItem.curPlatform) {
			case MultiPlatformType.iOS:
			case MultiPlatformType.GooglePlay:
				PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
				PlayerSettings.allowedAutorotateToLandscapeLeft = true;
				PlayerSettings.allowedAutorotateToLandscapeRight = true;
				break;
			case MultiPlatformType.Amazon:
				PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
				PlayerSettings.allowedAutorotateToLandscapeLeft = true;
				break;
			}
		}

		private static BuildTargetGroup GetBuildTargetGroup (MultiPlatformItem pPlatformItem)
		{
			switch (pPlatformItem.curPlatform) {
			case MultiPlatformType.iOS:
				return BuildTargetGroup.iOS;
			case MultiPlatformType.GooglePlay:
			case MultiPlatformType.Amazon:
				return BuildTargetGroup.Android;
			}

			return BuildTargetGroup.Unknown;
		}

		private static void SetPlatformIcons (MultiPlatformItem pPlatformItem)
		{
			var targetGroup = GetBuildTargetGroup (pPlatformItem);

			var pIconsFoldPath = Application.dataPath + "/" + pPlatformItem.iconFolder;
			if (!Directory.Exists (pIconsFoldPath)) {
				Debug.LogError ("Icon folder [ " + pPlatformItem.iconFolder + "] is not exist (图标文件夹不存在)!");
				return;
			}

			var iconFilePaths = Directory.GetFiles (pIconsFoldPath, "*.png", SearchOption.AllDirectories);
			if (iconFilePaths.Length <= 0) {
				Debug.LogError ("There is no png icon image in (不存在png图标) [" + pPlatformItem.iconFolder + "]");
				return;
			}

			var pIconTextureList = new List<Texture2D> ();
			foreach (var pIconPath in iconFilePaths) {
				var assetPath = pIconPath.Substring (Application.dataPath.Length);
				assetPath = "Assets" + assetPath;

				var pIconTex = AssetDatabase.LoadAssetAtPath<Texture2D> (assetPath);
				pIconTextureList.Add (pIconTex);
			}

			var pTextureSizes = PlayerSettings.GetIconSizesForTargetGroup (targetGroup);
			var pTargetIconArray = new Texture2D[pTextureSizes.Length];

			for (var i = 0; i < pTextureSizes.Length; i++) {
				pTargetIconArray [i] = null;

				foreach (var pTex in pIconTextureList) {
					if (pTex.width == pTextureSizes [i]) {
						pTargetIconArray [i] = pTex;
						break;
					}
				}
			}

			PlayerSettings.SetIconsForTargetGroup (targetGroup, pTargetIconArray);
		}

		public static void SetScriptingDefineSymbols (MultiPlatformItem pPlatformItem)
		{
			if (GlobalSettingData.Instance == null) return;

			var targetGroup = GetBuildTargetGroup (pPlatformItem);
			var changed = false;

			// current symbols
			var currGroupSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (targetGroup);
			var currSymbolSet = CollectScriptingDefineSymbols (currGroupSymbols);

			// will added symbols
			var willAddedSymbolSet = CollectScriptingDefineSymbols (pPlatformItem.scriptingDefs);
			// will removed symbols
			var willRemoveGroupSymbols = new List<string> ();
			foreach (var item in GlobalSettingData.Instance.allPlatformItems) {
				if (item.curPlatform != pPlatformItem.curPlatform) {
					willRemoveGroupSymbols.Add (item.scriptingDefs);
				}
			}
			var willRemovedSymbolSet = CollectScriptingDefineSymbols (willRemoveGroupSymbols.ToArray ());

			// global debug settings
			if (CocoDebugSettingsData.Instance.IsGlobalEnabled) {
				willAddedSymbolSet.Add (CocoDebugSettingsData.GLOBAL_SCRIPTING_SYMBOL);
			} else {
				willRemovedSymbolSet.Add (CocoDebugSettingsData.GLOBAL_SCRIPTING_SYMBOL);
			}

			// string strLog = string.Format (" start [{0}], remove [{1}], add [{2}]",
			// 	currGroupSymbols, JoinScriptingDefineSymbols (willRemovedSymbolSet),
			// 	pPlatformItem.scriptingDefs);

			// remove will added from will removed list
			RemoveScriptingDefineSymbols (willRemovedSymbolSet, willAddedSymbolSet);

			// remove other platform symbols
			changed |= RemoveScriptingDefineSymbols (currSymbolSet, willRemovedSymbolSet);

			// add current platform symbols
			changed |= AddScriptingDefineSymbols (currSymbolSet, willAddedSymbolSet);

			//strLog = string.Format (" {0} - end [{1}], changed {2}", strLog, JoinScriptingDefineSymbols (currSymbolSet), changed);
			//Debug.LogError (strLog);
			if (!changed) {
				return;
			}

			// write new symbols
			currGroupSymbols = JoinScriptingDefineSymbols (currSymbolSet);
			PlayerSettings.SetScriptingDefineSymbolsForGroup (targetGroup, currGroupSymbols);
		}

		private static string JoinScriptingDefineSymbols (HashSet<string> symbolSet)
		{
			var symbols = new string[symbolSet.Count];
			symbolSet.CopyTo (symbols);
			return string.Join (";", symbols);
		}

		private static bool RemoveScriptingDefineSymbols (HashSet<string> currSymbolSet, HashSet<string> willRemovedSymbolSet)
		{
			var changed = false;

			foreach (var symbol in willRemovedSymbolSet) {
				if (currSymbolSet.Contains (symbol)) {
					currSymbolSet.Remove (symbol);
					changed = true;
				}
			}

			return changed;
		}

		private static bool AddScriptingDefineSymbols (HashSet<string> currSymbolSet, HashSet<string> willAddedSymbolSet)
		{
			var changed = false;

			foreach (var symbol in willAddedSymbolSet) {
				if (!currSymbolSet.Contains (symbol)) {
					currSymbolSet.Add (symbol);
					changed = true;
				}
			}

			return changed;
		}

		private static HashSet<string> CollectScriptingDefineSymbols (params string[] groupSymbols)
		{
			var symbolSet = new HashSet<string> ();

			if (groupSymbols != null && groupSymbols.Length > 0) {
				foreach (var t in groupSymbols) {
					var symbols = t.Split (new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (var symbol in symbols) {
						symbolSet.Add (symbol);
					}
				}
			}

			return symbolSet;
		}

		private const string TARGET_BILLING_MODE_JSON_PATH = "/Plugins/UnityPurchasing/Resources/BillingMode.json";

		private static void SetBillingModeFile (MultiPlatformItem pPlatformItem)
		{
			var pJsonFilePath = Application.dataPath + TARGET_BILLING_MODE_JSON_PATH;
			if (!File.Exists (pJsonFilePath))
				return;

			var pReader = new StreamReader (pJsonFilePath);
			var pJsonContent = pReader.ReadToEnd ();
			var pJsonFileData = JsonMapper.ToObject (pJsonContent);
			pReader.Close ();

			switch (pPlatformItem.curPlatform) {
			case MultiPlatformType.iOS:
			case MultiPlatformType.GooglePlay:
				pJsonFileData ["androidStore"] = "GooglePlay";
				break;
			case MultiPlatformType.Amazon:
				pJsonFileData ["androidStore"] = "AmazonAppStore";
				break;
			}
			pJsonContent = pJsonFileData.ToJson ();
			//		Debug.LogWarning (pJsonContent);

			var fs = new FileStream (pJsonFilePath, FileMode.Create);
			var sw = new StreamWriter (fs);
			sw.Write (pJsonContent);
			sw.Close ();
			fs.Close ();
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}

		private static void SetAndroidManifestFile (MultiPlatformItem pPlatformItem)
		{
			var pFilePath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
			if (!File.Exists (pFilePath)) {
				Debug.LogError ("AndroidManifest file not exit! " + pFilePath);
				return;
			}

			var pDocument = new XmlDocument ();
			pDocument.Load (pFilePath);

			var rootNode = pDocument.SelectSingleNode ("manifest");
			if (rootNode == null) {
				Debug.LogError ("root node is null ");
				return;
			}

			var pNodeList = rootNode.SelectNodes ("uses-permission");
			if (pNodeList != null) {
				for (var i = 0; i < pNodeList.Count; i++) {
					var pElement = (XmlElement)pNodeList [i];

					if (pElement != null && pElement.GetAttribute ("android:name") == "com.android.vending.BILLING") {
						rootNode.RemoveChild (pNodeList [i]);
					}
				}
			}

			var pNewElement = pDocument.CreateElement ("Node", "uses-permission", "");
			pNewElement.SetAttribute (":android:name", "com.android.vending.BILLING");

			if (pPlatformItem.curPlatform == MultiPlatformType.Amazon) {
				pNewElement.SetAttribute (":tools:node", "remove");
				pNewElement.SetAttribute (":xmlns:tools", "http://schemas.android.com/tools");
			}

			rootNode.AppendChild (pNewElement);

			pDocument.Save (pFilePath);
		}

		public static void SetSplitBinaryOption (MultiPlatformItem pPlatformItem)
		{
			switch (pPlatformItem.curPlatform) {
#if UNITY_IOS
			case MultiPlatformType.iOS:
				Debug.LogFormat ("GlobalSettingEditor->SetSplitBinaryOption: useOnDemandResources [{0}]", pPlatformItem.splitBinary);
				PlayerSettings.iOS.useOnDemandResources = pPlatformItem.splitBinary;
				break;
#elif UNITY_ANDROID
			case MultiPlatformType.GooglePlay:
			case MultiPlatformType.Amazon:
				Debug.LogFormat ("GlobalSettingEditor->SetSplitBinaryOption: useAPKExpansionFiles [{0}]", pPlatformItem.splitBinary);
				PlayerSettings.Android.useAPKExpansionFiles = pPlatformItem.splitBinary;
				break;
#else
			default:
				Debug.Log ("SetSplitBinaryOption: Current platform [" + pPlatformItem.curPlatform + "] is not support ! (当前平台不支持)");
				break;
#endif
			}
		}

		#endregion


		#region Menu

		public static MultiPlatformItem GetPlatformItem (MultiPlatformType platformType)
		{
			if (GlobalSettingData.Instance == null) return null;

			var pAllItems = GlobalSettingData.Instance.allPlatformItems;
			foreach (var item in pAllItems) {
				if (item.curPlatform == platformType) {
					return item;
				}
			}

			return null;
		}

		private static void SwitchToTargetPlatform (MultiPlatformType platformType)
		{
			var platformItem = GetPlatformItem (platformType);
			if (platformItem == null) {
				Debug.LogErrorFormat ("data NOT found for platform [{0}]!", platformType);
				return;
			}

			var originSwitchPlatformEnabled = _switchPlatform;
			_switchPlatform = true;
			SwitchPlatformByItem (platformItem);
			_switchPlatform = originSwitchPlatformEnabled;
		}


		[MenuItem ("CocoPlay/Platform/Switch Platform to iOS", false, 106)]
		private static void SwitchToiOSPlatform ()
		{
			SwitchToTargetPlatform (MultiPlatformType.iOS);
		}

		[MenuItem ("CocoPlay/Platform/Switch Platform to GooglePlay", false, 107)]
		private static void SwitchToGooglePlayPlatform ()
		{
			SwitchToTargetPlatform (MultiPlatformType.GooglePlay);
		}

		[MenuItem ("CocoPlay/Platform/Switch Platform to Amazon", false, 108)]
		private static void SwitchToAmazonPlatform ()
		{
			SwitchToTargetPlatform (MultiPlatformType.Amazon);
		}

		#endregion
	}
}