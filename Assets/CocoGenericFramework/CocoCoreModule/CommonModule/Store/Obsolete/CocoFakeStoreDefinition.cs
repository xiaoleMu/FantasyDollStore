using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace CocoPlay.Fake
{
	public class CocoStoreKey{
		public static string Store_Name_Ads_Removed = "txt_store_no_ads";
		public static string Store_Name_NoAds = "txt_store_remove_ads";
		public static string Store_txt_Unlocked = "txt_store_product_unlocked";

		public static string MainStorePopupPath = "Store_Main";
		public static string MiniStorePopupPath = "Store_Mini";
		public static string SceneLockPopupPath = "Store_SceneLock";
		public static string NoInternetPopupPath = "Popup_NoInternet";
	}

	public enum CocoStoreID
	{
		None = -1,
		FullVersion = 0,
		AllItems = 1,
		NoAds = 2,
		HairSalon = 3,
		Eclaire = 4,
		Spa = 5,
		RMoney = 6,

		CoverShoot = 7,
		Contest = 8,
	}
}
