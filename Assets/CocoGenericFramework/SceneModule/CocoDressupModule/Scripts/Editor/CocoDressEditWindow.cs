using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using CocoPlay.ResourceManagement;


namespace CocoPlay
{
	public class CocoDressEditWindow : EditorWindow, ICocoDressAssetConfiguratorOwner
	{
		[MenuItem ("CocoPlay/Configurator/Dress Configurator...", false, 0)]
		private static void Init ()
		{
			var window = GetWindow<CocoDressEditWindow> (TITLE, true);
			window.titleContent = new GUIContent (CocoDressEditorHelper.GetLanguageText (TITLE));
			window.minSize = new Vector2 (720, 360);
		}

		private void OnEnable ()
		{
			InitLanguageTexts ();
			LoadEditorConfig ();
		}

		private void OnDisable ()
		{
			SaveEditorConfig ();
			SaveAssetConfig ();

			m_EditConfigHolder = null;
			m_AssetConfigHolder = null;
		}

		private void OnGUI ()
		{
			if (m_EditConfigHolder == null) {
				LoadEditorConfig ();
			}

			DrawDirectorySettings ();
			DrawAssetGenerationSettings ();
			DrawAssetBundleSettings ();
		}


		#region Text

		private const string TITLE = "Dress Configurator";

		// directory settings
		private const string LABEL_DIRECTORY_SETTINGS = "---- Directory Settings ----";
		private const string LABEL_ROOT_DIRECTORY = "Root Directory:";
		private const string LABEL_ORIGIN_DIRECTORY = "Asset Origin Directory:";
		private const string LABEL_TARGET_DIRECTORY = "Asset Target Directory:";
		private const string LABEL_CONFIG_DIRECTORY = "Config Directory:";
		private const string LABEL_PRETTY_PRINT = "use Pretty Print";
		private const string LABEL_RANDOM_SORTING = "random Sorting";

		// asset generate settings
		private const string LABEL_ASSET_GENERATE_SETTINGS = "---- Asset Generation Settings ----";
		private const string LABEL_GLOBAL_CONFIG_PATH = "Global Config File:";
		private const string BUTTON_LOAD = "Load";
		private const string BUTTON_SAVE = "Save";
		private const string BUTTON_AUTO_GENERATE = "Auto Generate";
		private const string BUTTON_CANCEL = "Cancel";
		private const string BUTTON_STILL_CONTINUE = "Still Continue";

		// asset bundle settings
		private const string LABEL_ASSET_BUNDLE_SETTINGS = "---- AssetBundle Settings ----";
		private const string LABEL_USE_ASSET_BUNDLE = "Use AssetBundle";
		private const string LABEL_COMPRESS_OPTION = "Compress Option";
		private const string BUTTON_OK = "OK";
		private const string BUTTON_AUTO_SET_AB_TAGS = "Auto Set AssetBundle Tags";
		private const string BUTTON_CORRECT_AB_TAGS = "Correct AssetBundle Tags";
		private const string BUTTON_AUTO_GENERATE_AB = "Auto Generate AssetBundle";
		private const string LABEL_SELECT_EXTERNAL = "Select External Tags:";
		private const string LABEL_GENERATE = "Genetate";
		private const string LABEL_SELECT_TARGET = "Select Target Platfrom:";
		private const string LABEL_COPY_TO_TARGET = "Copy To Target";

		// message
		private const string MESSAGE_TITLE_WARNING = "Warning !!";
		private const string MESSAGE_TITLE_INFO = "Info";
		private const string MESSAGE_CONTENT_AUTO_GENERATE = "Config data ALREADY exists! still generate automatically? (origin data will lost...)";
		private const string MESSAGE_CONTENT_NOT_USE_ASSET_BUNDLE = "AssetBundle be disabled in editor config, please enable it before continue.";

		private void InitLanguageTexts ()
		{
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, TITLE, "Coco资源编辑器");

			// editor settings
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_DIRECTORY_SETTINGS, "---- 目录设置 ----");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_ROOT_DIRECTORY, "根目录:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_ORIGIN_DIRECTORY, "原始资源目录:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_TARGET_DIRECTORY, "目标输出目录:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_CONFIG_DIRECTORY, "配置文件目录:");

			// asset settings
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_ASSET_GENERATE_SETTINGS, "---- 资源生成设置 ----");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_PRETTY_PRINT, "格式化输出:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_RANDOM_SORTING, "随机排序:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_GLOBAL_CONFIG_PATH, "全局配置文件:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_LOAD, "载入");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_SAVE, "保存");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_AUTO_GENERATE, "自动生成");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_CANCEL, "取消");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_STILL_CONTINUE, "仍然继续");

			// asset bundle settings
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_ASSET_BUNDLE_SETTINGS, "---- AssetBundle 设置 ----");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_USE_ASSET_BUNDLE, "使用AssetBundle方式:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_COMPRESS_OPTION, "压缩选项:");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_OK, "好的");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_AUTO_SET_AB_TAGS, "自动设置AssetBundle标签");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_CORRECT_AB_TAGS, "纠正AssetBundle标签设置");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, BUTTON_AUTO_GENERATE_AB, "自动生成AssetBundle");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_SELECT_EXTERNAL, "选择外部标签");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_GENERATE, "生成");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_SELECT_TARGET, "选择目标平台");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, LABEL_COPY_TO_TARGET, "拷贝到目标位置");

			// message
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, MESSAGE_TITLE_WARNING, "警告 !!");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, MESSAGE_TITLE_INFO, "信息");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, MESSAGE_CONTENT_AUTO_GENERATE, "配置数据已经存在! 仍然自动生成? (原有数据将被覆盖...)");
			CocoDressEditorHelper.SetLanguageText (SystemLanguage.Chinese, MESSAGE_CONTENT_NOT_USE_ASSET_BUNDLE, "Asset Bundle在编辑器配置在被禁用, 请启用它再继续");
		}

		private string Text (string key)
		{
			return CocoDressEditorHelper.GetLanguageText (key);
		}

		#endregion


		#region Editor Config

		private static CocoDressEditorConfigHolder m_EditConfigHolder;

		public static CocoDressEditorConfigHolder ConfigHolder {
			get {
				if (m_EditConfigHolder == null) {
					LoadEditorConfig ();
				}
				return m_EditConfigHolder;
			}
		}

		private static void LoadEditorConfig ()
		{
			// load config
			var path = CocoDressEditorHelper.GetFullPath (CocoDressSettings.EditorConfigFilePath);
			m_EditConfigHolder = CocoData.LoadFromJsonFile<CocoDressEditorConfigHolder> (path);

			// fix default config
			FixEditorDefaultConfig ();

			UpdateAssetFullDirectory ();
		}

		private static void FixEditorDefaultConfig ()
		{
			if (m_EditConfigHolder == null) {
				m_EditConfigHolder = new CocoDressEditorConfigHolder ();
			}

			var needFix = string.IsNullOrEmpty (m_EditConfigHolder.originRootDirectory) || string.IsNullOrEmpty (m_EditConfigHolder.assetDirectory) ||
			                 string.IsNullOrEmpty (m_EditConfigHolder.configDirectory) || string.IsNullOrEmpty (m_EditConfigHolder.globalConfigFileName);
			if (!needFix) {
				return;
			}

			m_EditConfigHolder.originRootDirectory = CocoDressSettings.GetDefaultOriginRootDirectory (false);
			m_EditConfigHolder.assetDirectory = CocoDressSettings.DEFAULT_ASSET_DIRECTORY;
			m_EditConfigHolder.configDirectory = CocoDressSettings.DEFAULT_CONFIG_DIRECTORY;
			m_EditConfigHolder.globalConfigFileName = CocoDressSettings.DEFAULT_CONFIG_FILE_NAME;
			m_EditConfigHolder.useAssetBundle = false;
		}

		private void SaveEditorConfig ()
		{
			if (m_EditConfigHolder == null) {
				return;
			}

			var path = CocoDressEditorHelper.GetFullPath (CocoDressSettings.EditorConfigFilePath);
			CocoData.SaveToJsonFile (m_EditConfigHolder, path, true);

			AssetDatabase.Refresh ();
		}

		#endregion


		#region Directory Settings

		private static string _rootFullDirectory;
		private static string _assetConfigFullDirectory;

		private static void UpdateAssetFullDirectory ()
		{
			_rootFullDirectory = Path.Combine (Application.dataPath, m_EditConfigHolder.originRootDirectory);
			_assetConfigFullDirectory = Path.Combine (_rootFullDirectory, m_EditConfigHolder.configDirectory);
		}

		private void DrawDirectorySettings ()
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// title
			EditorGUILayout.LabelField (Text (LABEL_DIRECTORY_SETTINGS), EditorStyles.boldLabel);
			EditorGUILayout.Space ();

			// directory field
			if (CocoDressEditorHelper.PathField (Text (LABEL_ROOT_DIRECTORY), ref m_EditConfigHolder.originRootDirectory, Application.dataPath, true)) {
				UpdateAssetFullDirectory ();
			}

			CocoDressEditorHelper.PathField (Text (LABEL_ORIGIN_DIRECTORY), ref m_EditConfigHolder.assetDirectory, _rootFullDirectory, true);

			if (CocoDressEditorHelper.PathField (Text (LABEL_CONFIG_DIRECTORY), ref m_EditConfigHolder.configDirectory, _rootFullDirectory, true)) {
				UpdateAssetFullDirectory ();
			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}

		#endregion


		#region Asset Generation Settings

		private CocoAssetConfigHolder m_AssetConfigHolder;
		private CocoDressAssetConfigurator _assetConfigurator = new CocoDressAssetConfigurator ();

		private void LoadAssetConfig ()
		{
			var path = Path.Combine (_assetConfigFullDirectory, m_EditConfigHolder.globalConfigFileName);
			m_AssetConfigHolder = CocoData.LoadFromJsonFile<CocoAssetConfigHolder> (path);

			if (m_AssetConfigHolder == null) {
				return;
			}

			m_AssetConfigHolder.LoadSubConfigs (_rootFullDirectory);
			m_AssetConfigHolder.LinkParent (null);
		}

		private void SaveAssetConfig ()
		{
			if (m_AssetConfigHolder == null) {
				return;
			}

			m_AssetConfigHolder.SaveSubConfigs (_rootFullDirectory, m_EditConfigHolder.prettyPrint);

			var path = Path.Combine (_assetConfigFullDirectory, m_EditConfigHolder.globalConfigFileName);
			CocoData.SaveToJsonFile (m_AssetConfigHolder, path, true);

			AssetDatabase.Refresh ();
		}

		private void DrawAssetGenerationSettings ()
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// title
			EditorGUILayout.LabelField (Text (LABEL_ASSET_GENERATE_SETTINGS), EditorStyles.boldLabel);
			EditorGUILayout.Space ();

			// config
			CocoDressEditorHelper.PathField (Text (LABEL_GLOBAL_CONFIG_PATH), ref m_EditConfigHolder.globalConfigFileName, _assetConfigFullDirectory, true);

			// settings field
			EditorGUILayout.BeginVertical (GUI.skin.box);
			m_EditConfigHolder.prettyPrint = EditorGUILayout.Toggle (Text (LABEL_PRETTY_PRINT), m_EditConfigHolder.prettyPrint);
			m_EditConfigHolder.randomSorting = EditorGUILayout.Toggle (Text (LABEL_RANDOM_SORTING), m_EditConfigHolder.randomSorting);
			EditorGUILayout.EndVertical ();

			// buttons
			EditorGUILayout.BeginHorizontal ();
			if (m_AssetConfigHolder == null) {
				if (GUILayout.Button (Text (BUTTON_LOAD))) {
					LoadAssetConfig ();
				}
			} else {
				if (GUILayout.Button (Text (BUTTON_SAVE))) {
					SaveAssetConfig ();
				}
			}
			GUILayout.Space (20);
			if (CocoDressEditorHelper.Button (Text (BUTTON_AUTO_GENERATE), Color.red)) {
				AutoGenerateAssetConfig ();
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}

		private void AutoGenerateAssetConfig ()
		{
			var path = Path.Combine (_assetConfigFullDirectory, m_EditConfigHolder.globalConfigFileName);
			if (m_AssetConfigHolder != null || File.Exists (path)) {
				if (EditorUtility.DisplayDialog (MESSAGE_TITLE_WARNING, MESSAGE_CONTENT_AUTO_GENERATE, BUTTON_CANCEL, BUTTON_STILL_CONTINUE)) {
					return;
				}
			}

			m_AssetConfigHolder = _assetConfigurator.AutoGenerateConfigByOwner (this);

			SaveAssetConfig ();
		}

		#endregion


		#region Custom Config

		public static Action onConfigGenerationStarted = null;
		public static Action<CocoAssetConfigHolder> onConfigGenerationEnded = null;
		public static Action<CocoRoleDressItemHolder> onConfigRoleDressItemBeforeRandomSorting = null;
		public static Action<CocoRoleDressItemHolder> onConfigRoleDressItemAfterRandomSorting = null;

		public static Func<CocoDressItemHolder, string> OnAfterFindModel = null;

		// param: raw items, in category, in scene
		// return: reorganized items
		public static Func<Dictionary<string, CocoDressItemHolder>, string, string, Dictionary<string, CocoDressItemHolder>> onConfigRawItemsCollected = null;


		public CocoDressEditorConfigHolder EditorConfigHolder {
			get { return m_EditConfigHolder; }
		}

		public void OnConfigGenerationStarted ()
		{
			if (onConfigGenerationStarted != null) {
				onConfigGenerationStarted ();
			}
		}

		public void OnConfigGenerationEnded (CocoAssetConfigHolder assetConfigHolder)
		{
			if (onConfigGenerationEnded != null) {
				onConfigGenerationEnded (assetConfigHolder);
			}
		}

		public void OnConfigAllRoleDressItemsBeforeRandomSorting (CocoAssetConfigHolder assetConfigHolder)
		{
			if (onConfigRoleDressItemBeforeRandomSorting == null) {
				return;
			}

			assetConfigHolder.RoleDressHolderDic.ForEach (roleDressHolder => {
				roleDressHolder.sceneHolders.ForEach (sceneHolder => {
					sceneHolder.itemHolders.ForEach (itemHolder => onConfigRoleDressItemBeforeRandomSorting (itemHolder));
				});
			});
		}

		public void OnConfigAllRoleDressItemsAfterRandomSorting (CocoAssetConfigHolder assetConfigHolder)
		{
			if (onConfigRoleDressItemAfterRandomSorting == null) {
				return;
			}

			assetConfigHolder.RoleDressHolderDic.ForEach (roleDressHolder => {
				roleDressHolder.sceneHolders.ForEach (sceneHolder => {
					sceneHolder.itemHolders.ForEach (itemHolder => onConfigRoleDressItemAfterRandomSorting (itemHolder));
				});
			});
		}

		public string OnItemModelContentFilled (CocoDressItemHolder itemHolder)
		{
			return OnAfterFindModel != null ? OnAfterFindModel (itemHolder) : string.Empty;
		}

		public Dictionary<string, CocoDressItemHolder> OnConfigRawItemsCollected (Dictionary<string, CocoDressItemHolder> rawItems, string categoryId,
			string sceneId)
		{
			return onConfigRawItemsCollected != null ? onConfigRawItemsCollected (rawItems, categoryId, sceneId) : null;
		}

		#endregion


		#region AssetBundle Settings

		private readonly CocoDressAssetBundleConfigurator _assetBundleConfigurator = new CocoDressAssetBundleConfigurator ();

		private int _platformIndex;


		private void DrawAssetBundleSettings ()
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);

			// title
			EditorGUILayout.LabelField (Text (LABEL_ASSET_BUNDLE_SETTINGS), EditorStyles.boldLabel);
			EditorGUILayout.Space ();

			// settings field
			EditorGUILayout.BeginVertical (GUI.skin.box);
			var useAssetBundle = EditorGUILayout.Toggle (Text (LABEL_USE_ASSET_BUNDLE), m_EditConfigHolder.useAssetBundle);
			if (useAssetBundle != m_EditConfigHolder.useAssetBundle) {
				UpdateResourceDirectory (useAssetBundle);
				m_EditConfigHolder.useAssetBundle = useAssetBundle;
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();

			if (m_EditConfigHolder.useAssetBundle) {
				DrawAssetBundleContentSettings ();
			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}

		private void DrawAssetBundleContentSettings ()
		{
			// buttons
			if (GUILayout.Button (Text (BUTTON_AUTO_SET_AB_TAGS))) {
				ProcessAssetBundleTags (_assetBundleConfigurator.AutoSetAssetBundleTags);
			}
		}

		private void UpdateResourceDirectory (bool useAssetBundle)
		{
			var rootDirectory = m_EditConfigHolder.originRootDirectory;
			var originKey = Path.DirectorySeparatorChar + ResourceSettings.DEFAULT_RESOURCES_DIRECTORY;
			var targetKey = Path.DirectorySeparatorChar + ResourceSettings.RUNTIME_RESOURCES_DIRECTORY;

			if (!useAssetBundle) {
				var tempDirectory = originKey;
				originKey = targetKey;
				targetKey = tempDirectory;
			}

			// end with resource directory
			if (rootDirectory.EndsWith (originKey)) {
				var top = rootDirectory.Substring (0, rootDirectory.Length - originKey.Length);
				m_EditConfigHolder.originRootDirectory = top + targetKey;
				UpdateAssetFullDirectory ();
				return;
			}

			originKey += Path.DirectorySeparatorChar;
			targetKey += Path.DirectorySeparatorChar;
			var index = rootDirectory.LastIndexOf (originKey, StringComparison.Ordinal);
			if (index < 0) {
				return;
			}

			var front = rootDirectory.Substring (0, index);
			index += originKey.Length;
			var back = rootDirectory.Substring (index, rootDirectory.Length - index);
			m_EditConfigHolder.originRootDirectory = front + targetKey + back;
			UpdateAssetFullDirectory ();
		}

		/// <summary>
		/// process asset bundle tags with function
		/// </summary>
		/// <param name="processFunc">process function (assetConfigHolder, rootDirectory)</param>
		private void ProcessAssetBundleTags (Action<CocoAssetConfigHolder, string> processFunc)
		{
			if (m_AssetConfigHolder == null) {
				LoadAssetConfig ();
			}
			if (m_AssetConfigHolder == null) {
				return;
			}

			if (!m_EditConfigHolder.useAssetBundle) {
				EditorUtility.DisplayDialog (Text (MESSAGE_TITLE_WARNING), Text (MESSAGE_CONTENT_NOT_USE_ASSET_BUNDLE), Text (BUTTON_OK));
				return;
			}

			var assetRootDirectory = Path.Combine ("Assets", m_EditConfigHolder.originRootDirectory);
			processFunc (m_AssetConfigHolder, assetRootDirectory);
		}

		#endregion
	}
}