using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace Game
{
	public class CocoStoreKey
	{
		public static string Store_Name_Ads_Removed = "txt_store_no_ads";
		public static string Store_Name_NoAds = "txt_store_remove_ads";
		public static string Store_txt_Unlocked = "txt_store_product_unlocked";

		public static string MainStorePopupPath = "Store_Main";
		public static string MiniStorePopupPath = "Store_Mini";
		public static string MiniStoreFullVersionPopupPath = "Store_Mini_FullVersion";
		public static string SceneLockPopupPath = "Store_SceneLock";
		public static string NoInternetPopupPath = "Popup_NoInternet";

		#region GP A/B Test

		public static string MainStorePopupPath_TypeA = "Store_Main_A";
		public static string MiniStorePopupPath_TypeA = "Store_Mini_A";
		public static string MainStorePopupPath_TypeB = "Store_Main_B";
		public static string MiniStorePopupPath_TypeB = "Store_Mini_B";

		#endregion
	}

	public enum CocoStoreID
	{
		None = -1,
		FullVersion = 0,
		AllItems = 1,
		NoAds = 2,
		HairSalon = 3,
		Spa = 4,
		DressDIY = 5,
		Contest = 6,

		FullGame = 7,

		FullVersion_A = 9,
		AllItems_A = 10,
		NoAds_A = 11,
		HairSalon_A = 12,
		Spa_A = 13,
		DressDIY_A = 14,
		Contest_A = 15,

		FullVersion_B = 18,
		NoAds_B = 19,
	}


}

