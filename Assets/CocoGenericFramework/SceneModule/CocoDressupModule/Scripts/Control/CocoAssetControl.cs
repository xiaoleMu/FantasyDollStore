using UnityEngine;
using System.Collections.Generic;
using CocoPlay.ResourceManagement;
using TabTale;

namespace CocoPlay
{
	public class CocoAssetControl : GameView
	{
		#region Init/Clean

		protected override void AddListeners ()
		{
			base.AddListeners ();
			dressLoadConfigHolderSignal.AddListener (OnLoadConfig);
			dressRequestItemHolderSignal.AddListener (OnRequestItemHolder);
		}

		protected override void RemoveListeners ()
		{
			dressLoadConfigHolderSignal.RemoveListener (OnLoadConfig);
			dressRequestItemHolderSignal.RemoveListener (OnRequestItemHolder);
			base.RemoveListeners ();
		}

		#endregion


		#region Load Config

		private const string DEFAULT_CONFIG_FILE_PATH = "asset_config/config_asset_global";

		[Inject]
		public CocoAssetLoadConfigHolderSignal dressLoadConfigHolderSignal { get; set; }

		[Inject]
		public CocoAssetLoadConfigHolderFinishSignal dressLoadConfigHolderFinishSignal { get; set; }

		public CocoAssetConfigHolder ConfigHolder { get; private set; }

		private bool m_ConfigIsReady;

		public bool ConfigIsReady {
			get { return m_ConfigIsReady; }
		}

		private void OnLoadConfig (string filePath)
		{
			LoadConfigData (filePath);
		}

		private void LoadConfigData (string filePath)
		{
			bool success = true;

			if (!m_ConfigIsReady) {
				success = LoadConfigHolder (filePath);
			}

			OnConfigHolderLoadFinish (success);
		}

		private void OnConfigHolderLoadFinish (bool success)
		{
			if (!success) {
				Debug.LogErrorFormat ("[{0}<{1}>]->OnConfigHolderLoadFinish: FAILED !", name, GetType ().Name);
			}

			ProcessConfigData ();

			//Debug.LogError (ConfigHolder.Print ());
			m_ConfigIsReady = true;
			OnLoadConfigHolderFinish (success);
		}

		private void OnLoadConfigHolderFinish (bool success)
		{
			dressLoadConfigHolderFinishSignal.Dispatch (success);
		}

		private bool LoadConfigHolder (string assetPath)
		{
			if (string.IsNullOrEmpty (assetPath)) {
				assetPath = DEFAULT_CONFIG_FILE_PATH;
			}

			TextAsset data = ResourceManager.Load<TextAsset> (assetPath);
			if (data == null) {
				Debug.LogErrorFormat ("GLOBAL CONFIG LOAD FAILED: config data NOT exists in path: [{0}].", assetPath);
				return false;
			}

			ConfigHolder = LitJson.JsonMapper.ToObject<CocoAssetConfigHolder> (data.text);
			if (ConfigHolder == null) {
				Debug.LogErrorFormat ("GLOBAL CONFIG LOAD FAILED: config data parse failed!");
				return false;
			}

			return true;
		}

		private void ProcessConfigData ()
		{
			ConfigHolder.LoadAsset ();
			ConfigHolder.LinkParent (null);
		}

		#endregion


		#region Request Dress Items

		[Inject]
		public CocoAssetRequestDressItemHolderSignal dressRequestItemHolderSignal { get; set; }

		[Inject]
		public CocoAssetRequestDressItemHolderFinishSignal dressRequestItemHolderFinishSignal { get; set; }

		private void OnRequestItemHolder (List<CocoDressItemHolder> itemHolders, string requestId)
		{
			LoadItemHolderAssets (itemHolders, requestId);
		}

		private void LoadItemHolderAssets (List<CocoDressItemHolder> itemHolders, string requestId)
		{
			itemHolders.ForEach (itemHolder => { itemHolder.LoadAsset (); });
			dressRequestItemHolderFinishSignal.Dispatch (itemHolders, requestId);
		}

		#endregion
	}
}