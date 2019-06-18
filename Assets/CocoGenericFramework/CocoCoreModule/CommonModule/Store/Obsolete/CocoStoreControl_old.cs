using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;
using strange.extensions.signal.impl;

#if COCO_FAKE
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#else
using CocoStoreID = Game.CocoStoreID;
using CocoSceneID = Game.CocoSceneID;
#endif

namespace CocoPlay
{
	public class CocoStoreControl_old : GameView
	{
		static CocoStoreControl_old _this;
		static public CocoStoreControl_old Instance { get { return _this; } }

		[Inject]
		public IBillingService m_BillingService { get; set; }

		[Inject]
		public IAPPurchaseDoneSignal m_PurchaseDoneSignal { get; set; }

		[Inject]
		public ItemConfigModel m_ItemConfigModel { get; set; }

		[Inject]
		public IAPConfigModel m_IAPConfigModel { get; set; }

		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }

		[Inject]
		public InventoryStateModel m_Inventory { get; set; }

		[Inject]
		public StoreManager m_StoreManager { get; set; }

		[Inject]
		public NetworkCheck m_Network { get; set; }

		[Inject]
		public RequestRestoreSignal requestRestoreSignal {get; set;}

		[Inject]
		public PurchasesRestoredSignal m_RestoredSignal { get; set; }

		[Inject]
		public CocoStoreUpdateStateSignal m_UpdateStoreStateSingal { get; set; }

		[Inject]
		public ICocoStoreConfigData m_ConfigData {get; set;}


		protected override void Start()
		{
			base.Start();
			_this = this;
//			InitInfo();

            #if ABTEST

            if (globalRecordModel.CurGPType == GPType.Test_B){
                if (IsPurchased(CocoStoreID.NoAds_B))
                    settingsStateModel.SetNoAds (true);
            }

            #endif
            StartCoroutine(StartRestoreProgress());
		}

		protected override void AddListeners()
		{
			base.AddListeners();
			m_PurchaseDoneSignal.AddListener(OnPurchasedDone);
            m_RestoredSignal.AddListener(OnRestoreEvent);
		}

		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			m_PurchaseDoneSignal.RemoveListener(OnPurchasedDone);
            m_RestoredSignal.RemoveListener(OnRestoreEvent);
		}



		[Inject]
		public CocoGlobalRecordModel globalRecordModel {get; set;}
        string GetStoreKey(CocoStoreID ID,bool DefaultKey = false)
		{
            if (!DefaultKey)
            {
                #if ABTEST
                switch (globalRecordModel.CurGPType)
                {
                    case GPType.Test_A:
                        return m_ConfigData.GetStoreKey_TypeA(ID);

                    case GPType.Test_B:
                        return m_ConfigData.GetStoreKey_TypeB(ID);
                    default:
                        return m_ConfigData.GetStoreKey(ID);
                }
                #else
			    return m_ConfigData.GetStoreKey(ID);
                #endif

            } else
            {
                return m_ConfigData.GetStoreKey(ID);
            }
		}

		#region store
        bool IsPurchasedProduct(CocoStoreID ID,bool defaultKey = false)
		{
            string ItemID = GetStoreKey(ID,defaultKey);
            if (ItemID.IsNullOrEmpty())
                return false;
			return IsPurchasedProduct(ItemID);
		}



		bool IsPurchasedProduct(string ItemID)
		{
			if (m_Inventory.HasItem(ItemID))
			{
				return true;
			}

			string IapID = m_IAPConfigModel.GetIAPId(ItemID);
			if (m_BillingService.IsPurchased(IapID))
			{
				if (!m_Inventory.HasItem(ItemID))
					m_Inventory.SetQuantity(ItemID, 1);
				return true;
			}

			return false;
		}
		#endregion

		#region PurchasedDone
		void OnPurchasedDone(PurchaseIAPResult result)
		{
			if (result.result != PurchaseIAPResultCode.Success)
				return;
			CheckAll();
			CheckSlot();
			CheckFullversion();


			#if ABTEST
			if (globalRecordModel.CurGPType == GPType.None) CheckABVersionSlot ();
			#endif
			m_UpdateStoreStateSingal.Dispatch();
//			SendOnPurchaseFlurry ();
		}
//		[Inject]
//		public CocoGlobalRecordModel GlobalRecordModel {get; set;}
//		#if COCO_FAKE
//		[Inject]
//		public CocoGlobalData GlobalData {get; set;}
//		#else
//		[Inject]
//		public Game.GameGlobalData GlobalData {get; set;}
//		#endif
//		//flurry
//		private string storeName = "mainstore";
//		private void SendOnPurchaseFlurry (){
//			Dictionary<string, object> eventParams = new Dictionary<string, object>();
//			eventParams.Add ("Game_Duration", GlobalRecordModel.GameDuration);
//			eventParams.Add ("Session_Number", GlobalRecordModel.SessionNumber);
//			eventParams.Add ("Scene_Name", CCFlurry.GetFlurrySceneName(GlobalData.CurSceneID) + "_" + storeName);
//			CCFlurry.LogEvent ("On_Purchase", eventParams);
//		}

		void CheckAll()
		{
			#if ABTEST

			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeA())
				{
					IsPurchasedProduct(itemID);
				}
				break;

			case GPType.Test_B:
				foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeB())
				{
					IsPurchasedProduct(itemID);
				}
				break;

				default:
				foreach (var itemID in m_ConfigData.GetStoreKeyList())
				{
					IsPurchasedProduct(itemID);
				}
				break;
			}

			#else

			foreach (var itemID in m_ConfigData.GetStoreKeyList())
			{
				IsPurchasedProduct(itemID);
			}

			#endif
		}

		#if ABTEST

		void CheckABVersionSlot (){
			Dictionary <CocoStoreID, List<CocoStoreID>> dict;
			dict = m_ConfigData.GetABVersionSlot();
			foreach(var keyValue in dict)
			{
                if (IsPurchasedProduct(keyValue.Key,true))
				{
					foreach (var child_ID in keyValue.Value)
					{
						string storeKey = m_ConfigData.GetStoreKey (child_ID);
						if (storeKey == string.Empty){
							storeKey = m_ConfigData.GetStoreKey_TypeA (child_ID);
						}
						if (storeKey == string.Empty) storeKey = m_ConfigData.GetStoreKey_TypeB (child_ID);
						if (storeKey != string.Empty){
							m_Inventory.SetQuantity(storeKey, 1);
						}else{
							Debug.LogError ("Not fot store key with sotre id " + child_ID.ToString());
						}
					}
				}

			}
		}

		#endif

		void CheckSlot()
		{
			Dictionary <CocoStoreID, List<CocoStoreID>> dict;
			#if ABTEST
			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				dict = m_ConfigData.GetSlot_TypeA();
				foreach(var keyValue in dict)
				{
					CheckSlotData(keyValue.Key, keyValue.Value);
				}
				break;

			case GPType.Test_B:
				dict = m_ConfigData.GetSlot_TypeB();
				foreach(var keyValue in dict)
				{
					CheckSlotData(keyValue.Key, keyValue.Value);
				}
				break;

				default:
				dict = m_ConfigData.GetSlot();
				foreach(var keyValue in dict)
				{
					CheckSlotData(keyValue.Key, keyValue.Value);
				}
				break;
			}

			#else

			dict = m_ConfigData.GetSlot();
			foreach(var keyValue in dict)
			{
				CheckSlotData(keyValue.Key, keyValue.Value);
			}

			#endif
		}

		void CheckFullversion()
		{
			//之前逻辑fullversion会解锁所有小包  不符合现在逻辑
			return;
			#if ABTEST
			bool allBuy;
			string FullVersionID;
			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				if (IsPurchasedProduct(CocoStoreID.FullVersion_A))
				{
					foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeA())
					{
						m_Inventory.SetQuantity(itemID, 1);
					}
					return;
				}

				allBuy = true;
				FullVersionID = GetStoreKey(CocoStoreID.FullVersion_A);
				foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeA())
				{
					if(itemID != FullVersionID && IsPurchasedProduct(itemID) == false)
					{
						allBuy = false;
						break;
					}
				}

				if(allBuy)
					m_Inventory.SetQuantity(FullVersionID, 1);
				break;

			case GPType.Test_B:
				if (IsPurchasedProduct(CocoStoreID.FullVersion_B))
				{
					foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeB())
					{
						m_Inventory.SetQuantity(itemID, 1);
					}
					return;
				}

//				allBuy = true;
//				FullVersionID = GetStoreKey(CocoStoreID.FullVersion_B);
//				foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeB())
//				{
//					if(itemID != FullVersionID && IsPurchasedProduct(itemID) == false)
//					{
//						allBuy = false;
//						break;
//					}
//				}
//
//				if(allBuy)
//					m_Inventory.SetQuantity(FullVersionID, 1);
				break;

				default:
				if (IsPurchasedProduct(CocoStoreID.FullVersion))
				{
					foreach (var itemID in m_ConfigData.GetStoreKeyList())
					{
						m_Inventory.SetQuantity(itemID, 1);
					}
					return;
				}

				allBuy = true;
				FullVersionID = GetStoreKey(CocoStoreID.FullVersion);
				foreach (var itemID in m_ConfigData.GetStoreKeyList())
				{
					if(itemID != FullVersionID && IsPurchasedProduct(itemID) == false)
					{
						allBuy = false;
						break;
					}
				}

				if(allBuy)
					m_Inventory.SetQuantity(FullVersionID, 1);
				break;
			}
			#else
			if (IsPurchasedProduct(CocoStoreID.FullVersion))
			{
			foreach (var itemID in m_ConfigData.GetStoreKeyList())
			{
			m_Inventory.SetQuantity(itemID, 1);
			}
			return;
			}

			bool allBuy = true;
			string FullVersionID = GetStoreKey(CocoStoreID.FullVersion);
			foreach (var itemID in m_ConfigData.GetStoreKeyList())
			{
			if(itemID != FullVersionID && IsPurchasedProduct(itemID) == false)
			{
			allBuy = false;
			break;
			}
			}

			if(allBuy)
			m_Inventory.SetQuantity(FullVersionID, 1);
			#endif
		}

		void CheckSlotData(CocoStoreID ID, List<CocoStoreID> slot)
		{
			if (IsPurchasedProduct(ID))
			{
				foreach (var child_ID in slot)
				{
					m_Inventory.SetQuantity(GetStoreKey(child_ID), 1);
				}
				return;
			}

			bool allBuy = true;
			foreach(var slot_ID in slot)
			{
				if(IsPurchasedProduct(slot_ID) == false)
				{
					allBuy = false;
					break;
				}
			}
			if(allBuy)
				m_Inventory.SetQuantity(GetStoreKey(ID), 1);
		}
		#endregion

		public void GetGod (){
			#if ABTEST

			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeA())
				{
					m_Inventory.SetQuantity(itemID, 1);
				}
				break;

			case GPType.Test_B:
				foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeB())
				{
					m_Inventory.SetQuantity(itemID, 1);
				}
				break;

				default:
				foreach (var itemID in m_ConfigData.GetStoreKeyList())
				{
					m_Inventory.SetQuantity(itemID, 1);
				}
				break;
			}

			#else

			foreach (var itemID in m_ConfigData.GetStoreKeyList())
			{
				m_Inventory.SetQuantity(itemID, 1);
			}

			#endif

			settingsStateModel.SetNoAds (true);

			m_UpdateStoreStateSingal.Dispatch();
		}

		#region Restore

		public void RequestRestore()
		{
//			if(m_Network.HasInternetConnection())
//			{
//				if (Application.platform == RuntimePlatform.IPhonePlayer ||
//					Application.platform == RuntimePlatform.OSXPlayer) {
//					requestRestoreSignal.Dispatch();
//				}
//			}
//			else
//			{
//				CocoMainController.ShowNoInternetPopup ();
//			}

			if (!IsManualRestoreAvailable) {
				return;
			}

			if (IsBillingAvailable) {
				requestRestoreSignal.Dispatch ();
			} else {
//				CocoMainController.ShowNoInternetPopup ();
			}
		}

        private IEnumerator StartRestoreProgress () {
            while (!m_BillingService.IsInitialised)
                yield return new WaitForSeconds (0.5f);
            OnRestoreEvent (true);
        }

		void OnRestoreEvent(bool success)
		{

			if (!success)
				return;
            CheckoutAllProduction();
            CheckSlot();
            #if ABTEST
            CheckABVersionSlot();
            if (globalRecordModel.CurGPType == GPType.Test_B){
                if (IsPurchased(CocoStoreID.NoAds_B))
                    settingsStateModel.SetNoAds (true);
            }
            #endif
            CheckFullversion();
			m_UpdateStoreStateSingal.Dispatch();
		}

        public void DebugAllProductionSate(string start){

            #if ABTEST
            foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeA())
            {
                Debug.LogError(itemID + "-----billing-------" + m_BillingService.IsPurchased(m_IAPConfigModel.GetIAPId(itemID)) + "------IAPID---" + m_IAPConfigModel.GetIAPId(itemID));
            }
            foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeB())
            {
                Debug.LogError(itemID + "-----billing-------" + m_BillingService.IsPurchased(m_IAPConfigModel.GetIAPId(itemID)) + "------IAPID---" + m_IAPConfigModel.GetIAPId(itemID));
            }
            foreach (var itemID in m_ConfigData.GetStoreKeyList())
            {
                Debug.LogError(itemID + "-----billing-------" + m_BillingService.IsPurchased(m_IAPConfigModel.GetIAPId(itemID)) + "------IAPID---" + m_IAPConfigModel.GetIAPId(itemID));
            }
            #else
            foreach (var itemID in m_ConfigData.GetStoreKeyList())
            {
            Debug.LogError(itemID + "-----billing-------" + m_BillingService.IsPurchased(m_IAPConfigModel.GetIAPId(itemID)) + "------IAPID---" + m_IAPConfigModel.GetIAPId(itemID));
            }
            #endif
        }

        void CheckoutAllProduction()
        {

            #if ABTEST
            foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeA())
            {
                IsPurchasedProduct(itemID);
            }
            foreach (var itemID in m_ConfigData.GetStoreKeyList_TypeB())
            {
                IsPurchasedProduct(itemID);
            }
            foreach (var itemID in m_ConfigData.GetStoreKeyList())
            {
                IsPurchasedProduct(itemID);
            }
            #else
            foreach (var itemID in m_ConfigData.GetStoreKeyList())
                {
                IsPurchasedProduct(itemID);
                }
            #endif
        }
		#endregion

		#region 查询接口

		public string GetPriceString(CocoStoreID ID, bool useISOCurrencySymbol = false)
		{
			string itemID = GetStoreKey(ID);
			Debug.LogError(itemID);
			if (string.IsNullOrEmpty(itemID))
				return "";

			string m_IapID = m_IAPConfigModel.GetIAPId(itemID);
			Debug.LogError(m_IapID);
			if (useISOCurrencySymbol)
				return string.Format ("{0} {1}", m_BillingService.GetISOCurrencySymbol (m_IapID), m_BillingService.GetPriceInLocalCurrency (m_IapID));
			else
				return m_BillingService.GetLocalizedPriceString (m_IapID);
		}

		public bool IsPurchased(CocoStoreID ID)
		{
			#if ABTEST

			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				if (IsPurchasedProduct(CocoStoreID.FullVersion_A))
				{
					return true;
				}
				else
				{
					return IsPurchasedProduct(ID);
				}
				break;

			case GPType.Test_B:
				if (IsPurchasedProduct(CocoStoreID.FullVersion_B))
				{
					return true;
				}
				else
				{
					return IsPurchasedProduct(ID);
				}
				break;

				default:
				if (IsPurchasedProduct(CocoStoreID.FullVersion))
				{
					return true;
				}
				else
				{
					return IsPurchasedProduct(ID);
				}
				break;
			}

			#else

			//之前逻辑会优先判断fullversion包  应该先判断能包含其的包
			var dict = m_ConfigData.GetSlot();

			foreach (var value in dict.Keys)
			{
				if (dict[value].Contains(ID))
				{
					if (IsPurchased(value))
					{
						return true;
					}
				}
			}

			return IsPurchasedProduct(ID);
			#endif
		}

		public void Buy(CocoStoreID ID)
		{
			string itemID = GetStoreKey(ID);

//			#if UNITY_EDITOR
//			m_StoreManager.BuyItem(itemID);
//			#else
//			if (m_Network.HasInternetConnection() && m_BillingService.IsInitialised)
// 				m_StoreManager.BuyItem(itemID);
//			else
//				CocoMainController.ShowNoInternetPopup ();
//			#endif

			if (IsBillingAvailable) {
				m_StoreManager.BuyItem(itemID);
			} else {
//				CocoMainController.ShowNoInternetPopup ();
			}
		}

		public void Buy(CocoStoreID ID, Action<BuyItemResult> pAcitonDone)
		{
			string itemID = GetStoreKey(ID);

//			#if UNITY_EDITOR
//			m_StoreManager.BuyItem(itemID);
//			#else
//			if (m_Network.HasInternetConnection() && m_BillingService.IsInitialised)
// 				m_StoreManager.BuyItem(itemID);
//			else
//				CocoMainController.ShowNoInternetPopup ();
//			#endif

			if (IsBillingAvailable) {
				m_StoreManager.BuyItem(itemID).Done(pAcitonDone);
			} else {
				pAcitonDone(new BuyItemResult(BuyItemResultCode.Error));
//				CocoMainController.ShowNoInternetPopup ();
			}
		}

		private bool IsBillingAvailable {
			get {
				// disable network check on iOS
#if !UNITY_IOS
				if (!m_Network.HasInternetConnection ()) {
					return false;
				}
#endif
				return m_BillingService.IsInitialised;
			}
		}

		private bool IsManualRestoreAvailable {
			get {
#if UNITY_IOS
				return true;
#else
				return false;
#endif
			}
		}

		#endregion


//		#region Flurry
//		[Inject]
//		public CocoGlobalRecordModel GlobalRecordModel {get; set;}
//
//		private void OnPurchaseFlurry (){
//			Dictionary<string, object> eventParams = new Dictionary<string, object>();
//			eventParams.Add ("Game_Duration", GlobalRecordModel.GameDuration);
//			eventParams.Add ("Session_Number", GlobalRecordModel.SessionNumber);
//			eventParams.Add ("Scene_Name", GlobalRecordModel.SessionNumber);
//
//			CCFlurry.LogEvent ("On_Purchase", eventParams);
//		}
//
//		#endregion
	}
}

