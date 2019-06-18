using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CocoPlay.Localization;

#if !COCO_FAKE
using CocoSceneID = Game.CocoSceneID;
using CocoStoreKey = Game.CocoStoreKey;
#else
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
using CocoStoreKey = CocoPlay.Fake.CocoStoreKey;
#endif

namespace CocoPlay {
	public class CocoNoadsButton : CocoStoreItemButton 
	{
		[SerializeField]
		Text m_ItemName;

		protected override void UpdatePrice()
		{
			if (IsPurchased())
			{
				m_ItemName.text = CocoLocalization.Get(CocoStoreKey.Store_Name_Ads_Removed);
				price.gameObject.SetActive(false);
				m_ItemName.transform.SetLocal_Y(0);
			}
			else
			{
				price.gameObject.SetActive(true);
				string itemPrice = m_StoreControl.GetPriceString(m_StoreID, useISOCurrencySymbol);
//				itemPrice = itemPrice.Replace(" ", "");
				price.text = itemPrice;

				m_ItemName.text = CocoLocalization.Get(CocoStoreKey.Store_Name_NoAds);
				m_ItemName.transform.SetLocal_Y(12);
			}
		}
	}
}