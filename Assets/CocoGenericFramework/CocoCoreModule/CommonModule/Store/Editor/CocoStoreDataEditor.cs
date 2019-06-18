#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using LitJson;
using TabTale;
using System.IO;
using System.Text;
using UnityEngine.Purchasing;
using CocoStoreID = Game.CocoStoreID;

namespace CocoPlay.Store
{
	[CustomEditor (typeof(CocoStoreData))]
	public class CocoStoreDataEditor : Editor
	{
		#region 自动创建ScriptableObject资源

		private const string CREATE_ASSET_PATH = "Assets/_Game/Resources/CocoStoreDatas.asset";

		//介于自动编译会重置文件，暂时先注释掉
		//	[DidReloadScripts]
		//	static void CreateStoreDataAsset () {
		//		if (CocoStoreData.Instance != null)
		//			return;
		//
		//		Debug.LogError ("文件夹下不存在 CocoStoreDatas.asset， 所以自动创建了一个，放在： " + createAssetPath);
		//
		//		var asset = CocoStoreData.CreateInstance <CocoStoreData> ();
		//		AssetDatabase.CreateAsset(asset, createAssetPath);
		//
		//		AssetDatabase.SaveAssets();
		//		AssetDatabase.Refresh();
		//	}

		#endregion


		[MenuItem ("CocoPlay/Store/Platform Settings", false, 110)]
		private static void FocusCocoStoreDataAssetObject ()
		{
			if (CocoStoreData.Instance != null) {
				Selection.activeObject = CocoStoreData.Instance;

				if (CheckHasSameData) {
					Debug.LogError ("Store Data isssue ! Check the same product ! (商品数据异常！检查重复iap id 和 product id )");
					return;
				}

				return;
			}

			Debug.LogError ("文件夹下不存在 CocoStoreDatas.asset， 所以创建了一个，放在： " + CREATE_ASSET_PATH);

			var asset = CreateInstance<CocoStoreData> ();
			AssetDatabase.CreateAsset (asset, CREATE_ASSET_PATH);
			Selection.activeObject = CocoStoreData.Instance;

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		}

		private bool sureClearAll;

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			var box = EditorGUILayout.BeginVertical ();
			List<CocoStoreItem> pAllStoreItems = CocoStoreData.Instance.allProductItems;
			GUILayout.Label ("Current items number (当前商品数量)： " + pAllStoreItems.Count.ToString ());

			string pStatus = CocoStoreData.Instance.isDataReady ? "OK" : "Empty";
			GUILayout.Label ("DB status (DB状态)：" + pStatus);

			int pStartPosY = 40;

			Rect pRect = new Rect (box.xMin, box.yMin + pStartPosY, 200, 20);
			if (GUI.Button (pRect, "Add item (添加一个商品配置)")) {
				pAllStoreItems.Add (new CocoStoreItem ());
			}

			GUILayout.Space (30);
			sureClearAll = EditorGUILayout.Toggle ("Sure to Clear All (确认清除)：", sureClearAll);

			pStartPosY += 50;
			pRect = new Rect (box.xMin, box.yMin + pStartPosY, 120, 20);

			if (sureClearAll) {
				if (GUI.Button (pRect, "Clear All (清除所有)")) {
					pAllStoreItems.Clear ();
					sureClearAll = false;
				}
			}

			pStartPosY += 30;
			pRect = new Rect (box.xMin, box.yMin + pStartPosY, 120, 20);
			if (GUI.Button (pRect, "Write in DB (写入DB)")) {
				WriteAllItemsDataToDB ();
			}

			GUILayout.Space (80);

			for (int i = 0; i < pAllStoreItems.Count; i++) {
				if (EditCocoStoreItem (i)) {
					pAllStoreItems.RemoveAt (i);
					i--;
				}
				GUILayout.Space (10);
			}

			EditorUtility.SetDirty (CocoStoreData.Instance);
			EditorGUILayout.EndVertical ();
			serializedObject.ApplyModifiedProperties ();
		}

		private bool EditCocoStoreItem (int pIndex)
		{
			bool pIsRemoveData = false;

			CocoStoreItem pStoreItem = CocoStoreData.Instance.allProductItems [pIndex];

			var box = EditorGUILayout.BeginVertical (GUI.skin.box);

			Rect pRect = new Rect (box.xMax - 110, box.yMin + 2, 50, 15);
			//按照总部的商品id的套路自动填写
			bool isAuto = GUI.Button (pRect, "Auto");
			pRect = new Rect (box.xMax - 50, box.yMin + 2, 50, 15);
			if (GUI.Button (pRect, "X")) {
				pIsRemoveData = true;
			}

			pStoreItem.foldout = EditorGUILayout.Foldout (pStoreItem.foldout, pStoreItem.productId.ToString ());
			GUILayout.Space (5);

			if (pStoreItem.foldout) {
#if UNITY_5_6_OR_NEWER
				var bundleId = PlayerSettings.applicationIdentifier;
#else
			var bundleId = PlayerSettings.bundleIdentifier;
#endif

				var storeId = pStoreItem.productId.ToString ().ToLower ();
				pStoreItem.productId = (CocoStoreID)EditorGUILayout.EnumPopup ("ProductId (商品Id): ", pStoreItem.productId);
				pStoreItem.itemIdString = EditorGUILayout.TextField ("item Id: ",
					string.IsNullOrEmpty (pStoreItem.itemIdString) ? pStoreItem.iosIapString : pStoreItem.itemIdString);
				pStoreItem.iosIapString = EditorGUILayout.TextField ("iOS iap Id: ",
					isAuto ? string.Format ("{0}.{1}", bundleId, storeId) : pStoreItem.iosIapString);
				pStoreItem.gpIapString = EditorGUILayout.TextField ("GP iap Id: ",
					isAuto ? string.Format ("{0}_{1}", bundleId, storeId) : pStoreItem.gpIapString);
				pStoreItem.amIapString = EditorGUILayout.TextField ("AM iap Id: ",
					isAuto ? string.Format ("{0}amazon_{1}", bundleId, storeId) : pStoreItem.amIapString);
				pStoreItem.productType = (ProductType)EditorGUILayout.EnumPopup ("ProductType (商品类型): ", pStoreItem.productType);
				pStoreItem.isNoAdsLap = EditorGUILayout.Toggle ("NoAdsLap (能否去广告)", pStoreItem.isNoAdsLap);

				EditorGUILayout.BeginHorizontal ();

				GUIStyle pStyle = new GUIStyle (EditorStyles.foldout);
				pStoreItem.relatedItemsFoldout = EditorGUILayout.Foldout (pStoreItem.relatedItemsFoldout, "relatedStoreItems", pStyle);
				int pItemsNum = EditorGUILayout.IntField ("", pStoreItem.relatedStoreItems.Count);
				while (pStoreItem.relatedStoreItems.Count != pItemsNum) {
					pStoreItem.relatedItemsFoldout = true;

					if (pStoreItem.relatedStoreItems.Count < pItemsNum) {
						pStoreItem.relatedStoreItems.Add (CocoStoreID.NoAds);
					}

					if (pStoreItem.relatedStoreItems.Count > pItemsNum) {
						pStoreItem.relatedStoreItems.RemoveAt (pStoreItem.relatedStoreItems.Count - 1);
					}
				}

				EditorGUILayout.EndHorizontal ();
				if (pStoreItem.relatedItemsFoldout) {
					for (int i = 0; i < pStoreItem.relatedStoreItems.Count; i++) {
						pStoreItem.relatedStoreItems [i] = (CocoStoreID)EditorGUILayout.EnumPopup ("CocoStoreID ", pStoreItem.relatedStoreItems [i]);
					}
				}
			}

			CocoStoreData.Instance.allProductItems [pIndex] = pStoreItem;

			EditorGUILayout.EndVertical ();

			return pIsRemoveData;
		}

		private void WriteAllItemsDataToDB ()
		{
			if (CheckHasSameData) {
				Debug.LogError ("Write DB Filed ! Check the same product ! (写入失败！检查重复iap id 和 product id )");
				return;
			}

			SerializedProperty pProperty = serializedObject.FindProperty ("isDBReady");
			pProperty.boolValue = true;

			WriteAllItemsDataToIapConfig ();
			WriteAllItemsDataToItemConfig ();
			WriteIapItemsDataToJson ("psdk_ios.json", "ios");
			WriteIapItemsDataToJson ("psdk_google.json", "google");
			WriteIapItemsDataToJson ("psdk_amazon.json", "amazon");
		}

		private void WriteAllItemsDataToIapConfig ()
		{
			string pPath = "data source=" + Application.streamingAssetsPath + "/DB/game.db";

			CocoSqliteDBHelper db = new CocoSqliteDBHelper (pPath);

			string pTableName = "iap_config";
			int pCount = db.FindTable (pTableName, pPath);
			int pTotalItem = 0;

			if (pCount != 0) {
				//首先清除之前所有的数据
				db.DeleteContents (pTableName);

				List<CocoStoreItem> pAllStoreItems = CocoStoreData.Instance.allProductItems;

				foreach (var storeItem in pAllStoreItems) {
					string pKey = "'" + storeItem.itemIdString + "'";

					IAPConfigData pData = new IAPConfigData ();
					pData.id = storeItem.itemIdString;
					pData.name = storeItem.productId.ToString ();
					pData.noAdsIap = storeItem.isNoAdsLap;

					switch (storeItem.productType) {
					case ProductType.Consumable:
						pData.consumable = true;
						break;
					case ProductType.Subscription:
						pData.isSubscription = true;
						break;
					}

					pData.iapData = new List<IAPData> ();

					if (!string.IsNullOrEmpty (storeItem.iosIapString)) {
						pData.iapData.Add (new IAPData { store = StoreType.iOS, iapId = storeItem.iosIapString });
					}

					if (!string.IsNullOrEmpty (storeItem.gpIapString)) {
						pData.iapData.Add (new IAPData { store = StoreType.Google, iapId = storeItem.gpIapString });
					}

					if (!string.IsNullOrEmpty (storeItem.amIapString)) {
						pData.iapData.Add (new IAPData { store = StoreType.Amazon, iapId = storeItem.amIapString });
					}

					string pValue = "'" + JsonMapper.ToJson (pData) + "'";

					db.InsertInto (pTableName, new[] { pKey, pValue });
					Debug.LogWarning ("Write Data For IAP Config Success ! : ( " + pKey + "," + pValue + ")");
					pTotalItem += 1;
				}
			}
			db.CloseSqlConnection (); //avoid to database is locked
			Debug.LogWarning ("Write IAP Config Data Success ! Total (写入成功！总共)：" + pTotalItem.ToString ());
		}

		private void WriteAllItemsDataToItemConfig ()
		{
			string pPath = "data source=" + Application.streamingAssetsPath + "/DB/game.db";

			CocoSqliteDBHelper db = new CocoSqliteDBHelper (pPath);

			string pTableName = "item_config";
			int pCount = db.FindTable (pTableName, pPath);
			int pTotalItem = 0;

			if (pCount != 0) {
				//首先清除之前所有的数据
				db.DeleteContents (pTableName);

				List<CocoStoreItem> pAllStoreItems = CocoStoreData.Instance.allProductItems;

				foreach (var storeItem in pAllStoreItems) {
					string pKey = "'" + storeItem.itemIdString + "'";

					ItemConfigData pData = new ItemConfigData ();
					pData.id = storeItem.itemIdString;
					pData.name = storeItem.productId.ToString ();
					pData.iapConfigId = storeItem.itemIdString;

					string pValue = "'" + JsonMapper.ToJson (pData) + "'";

					db.InsertInto (pTableName, new[] { pKey, pValue });
					Debug.LogWarning ("Write Data For Item Config  Success ! : ( " + pKey + "," + pValue + ")");
					pTotalItem += 1;
				}
			}
			db.CloseSqlConnection (); //avoid to database is locked
			Debug.LogWarning ("Write Item Config Data Success ! Total (写入成功！总共)：" + pTotalItem.ToString ());
		}

		private void WriteIapItemsDataToJson (string jsonName, string platformName)
		{
			string readPath = Application.dataPath + "/StreamingAssets/" + jsonName;
			string psdkJson = File.ReadAllText (readPath);

			List<CocoStoreItem> pAllStoreItems = CocoStoreData.Instance.allProductItems;
			IDictionary<string, object> dict = TabTale.Plugins.PSDK.Json.Deserialize (psdkJson) as IDictionary<string, object>;

			if (dict != null) {
				List<object> iaps = new List<object> ();
				foreach (var storeItem in pAllStoreItems) {
					var iapId = string.Empty;
					switch (platformName) {
					case "ios":
						iapId = storeItem.iosIapString;
						break;
					case "google":
						iapId = storeItem.gpIapString;
						break;
					case "amazon":
						iapId = storeItem.amIapString;
						break;
					}

					if (string.IsNullOrEmpty (iapId)) {
						// skip empty id data
						continue;
					}

					IDictionary<string, object> iapsItemData = new Dictionary<string, object> ();

					iapsItemData.Add ("id", storeItem.itemIdString);
					iapsItemData.Add ("iapId", iapId);

					switch (storeItem.productType) {
					case ProductType.Consumable:
						iapsItemData.Add ("type", "consumable");
						break;
					case ProductType.Subscription:
						iapsItemData.Add ("type", "subscription");
						break;
					default:
						iapsItemData.Add ("type", "non-consumable");
						break;
					}

					iapsItemData.Add ("noAds", storeItem.isNoAdsLap);

					iaps.Add (iapsItemData);
				}

				IDictionary<string, object> allIapsDic = new Dictionary<string, object> ();
				allIapsDic.Add ("included", true);
				allIapsDic.Add ("iaps", iaps);

				if (dict.ContainsKey ("billing")) {
					dict ["billing"] = allIapsDic;
				} else {
					dict.Add ("billing", allIapsDic);
				}

				IDictionary<string, object> rateusDic = new Dictionary<string, object> ();
				rateusDic.Add ("included", true);
				if (dict.ContainsKey ("rateUs")) {
					dict ["rateUs"] = rateusDic;
				} else {
					dict.Add ("rateUs", rateusDic);
				}

				string json = TabTale.Plugins.PSDK.Json.Serialize (dict);
				json = TabTale.Plugins.PSDK.JsonFormatter.PrettyPrint (json);
				File.WriteAllText (readPath, json, Encoding.UTF8);
			}
		}

		private static bool CheckHasSameData {
			get {
				bool pHaveSameItem = false;

				List<CocoStoreItem> pAllStoreItems = CocoStoreData.Instance.allProductItems;
				for (int i = 0; i < pAllStoreItems.Count; i++) {
					for (int j = i + 1; j < pAllStoreItems.Count; j++) {
						if (HasSameIapContent (pAllStoreItems [i], pAllStoreItems [j])) {
							pHaveSameItem = true;
							break;
						}
					}

					if (pHaveSameItem)
						break;
				}

				return pHaveSameItem;
			}
		}

		private static bool HasSameIapContent (CocoStoreItem item1, CocoStoreItem item2)
		{
			if (item1.productId == item2.productId) {
				return true;
			}

			if (item1.itemIdString == item2.itemIdString) {
				return true;
			}

			if (!string.IsNullOrEmpty (item1.iosIapString) && item1.iosIapString == item2.iosIapString) {
				return true;
			}

			if (!string.IsNullOrEmpty (item1.gpIapString) && item1.gpIapString == item2.gpIapString) {
				return true;
			}

			if (!string.IsNullOrEmpty (item1.amIapString) && item1.amIapString == item2.amIapString) {
				return true;
			}

			return false;
		}
	}
}
#endif