using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using CocoStoreID = Game.CocoStoreID;

namespace CocoPlay.Store
{
	public class CocoStoreData : ScriptableObject
	{
		private static CocoStoreData _instance;

		public static CocoStoreData Instance {
			get {
				if (_instance == null)
					_instance = Resources.Load<ScriptableObject> ("CocoStoreDatas") as CocoStoreData;

				return _instance;
			}
		}

		[SerializeField]
		private bool isDBReady;

		public bool isDataReady {
			get { return isDBReady; }
		}

		public List<CocoStoreItem> allProductItems = new List<CocoStoreItem> ();
		public Dictionary<CocoStoreID, CocoStoreItem> allProductItemsDic = new Dictionary<CocoStoreID, CocoStoreItem> ();
	}


	[System.Serializable]
	public class CocoStoreItem
	{
		//在面板上是否折叠
		public bool foldout = true;

		public CocoStoreID productId = CocoStoreID.FullVersion;
		public string itemIdString = string.Empty;
		public string iosIapString = "";
		public string gpIapString = "";
		public string amIapString = "";

		public ProductType productType = ProductType.NonConsumable;
		public bool isNoAdsLap;

		public bool relatedItemsFoldout = true;
		public List<CocoStoreID> relatedStoreItems = new List<CocoStoreID> ();
	}
}