using UnityEngine;
using System.Collections;
using TabTale;
using UnityEngine.UI;
using System.Collections.Generic;

#if !COCO_FAKE
using CocoStoreID = Game.CocoStoreID;

#else
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
#endif

namespace CocoPlay
{
	public class CocoLockableStateControl : GameView, ICocoLockableStateControl
	{
		#if COCO_FAKE
		[Inject]
		public CocoGlobalData GlobalData {get; set;}
		#else
		[Inject]
		public Game.GameGlobalData GlobalData {get; set;}
		#endif
		
		[Inject] 
		public UpdateRvStatusSignal refreshRvSignal { get; set; }

		[Inject]
		public CocoStoreUpdateStateSignal updateStateSignal { get; set; }

		protected override void AddListeners ()
		{
			base.AddListeners ();
			refreshRvSignal.AddListener (UpdateState);
			updateStateSignal.AddListener (UpdateState);
		}

		protected override void RemoveListeners ()
		{
			refreshRvSignal.RemoveListener (UpdateState);
			updateStateSignal.RemoveListener (UpdateState);
			base.RemoveListeners ();
		}


		#region Data

		//[SerializeField]
		protected CocoLockableItemData m_StateData;
		protected ICocoLockStateModel m_StateModel;
		bool m_IsInited = false;

		public virtual void InitState (CocoLockableItemData data, ICocoLockStateModel model)
		{
			m_StateData = data;
			m_StateModel = model;
			m_IsInited = true;

			// need display price
		    InitPriceLabel ();
		    InitLevelLabel();
			UpdateState ();
		}

		protected virtual void InitPriceLabel ()
		{
			if (m_PriceLabel == null) {
				return;
			}
		    
		    if (m_StateData.price <= 0) {
		        return;
		    }

			Text text = m_PriceLabel.GetComponentInChildren<Text> (true);
			if (text != null) {
				text.text = m_StateData.price.ToString ();
			}
		}
	    
	    protected virtual void InitLevelLabel ()
	    {
	        if (m_StateData.lockType != CocoLockType.Level)
	            return;
	            
	        if (m_StateData.level <= 0) {
	            return;
	        }
	        
	        if (m_LevelLabel == null) {
	            return;
	        }

	        Text text = m_LevelLabel.GetComponentInChildren<Text> (true);
	        if (text != null) {
	            text.text = m_StateData.level.ToString ();
	        }
	    }

		#endregion


		#region State

		[Inject]
		public CocoStoreControl storeControl { get; set; }

		public enum LabelState
		{
			None,
			GetFree,
			Lock,
			Price,
		    Level
		}

		public LabelState m_LabelState = LabelState.None;

		[SerializeField]
		GameObject m_GetFreeLabel;

		public GameObject GetFreeLabel {
			get {
				return m_GetFreeLabel;
			}
		}

		[SerializeField]
		GameObject m_LockLabel;

		public GameObject LockLabel {
			get {
				return m_LockLabel;
			}
		}

		[SerializeField]
		GameObject m_PriceLabel;

		public GameObject PriceLabel {
			get {
				return m_PriceLabel;
			}
		}
	    
	    [SerializeField]
	    protected GameObject m_LevelLabel;

	    public GameObject LevelLabel {
	        get {
	            return m_LevelLabel;
	        }
	    }

		[Inject]
		public CocoGlobalRecordModel globalRecordModel { get; set; }

		protected virtual LabelState GetLabelState ()
		{
			// check purchase state
			if (m_StateModel != null) {
				bool purchased = m_StateModel.IsItemPurchased (m_StateData.itemId);
				if (purchased) {
					return LabelState.None;
				}
			}

			// check in unlock state
			if (m_StateData.lockType == CocoLockType.Non || // non-lock
			    storeControl.IsPurchased (m_StateData.storeProductId)) {	// product purchased in store 
				return m_StateData.price <= 0 ? LabelState.None : LabelState.Price;
			}
		    
		    if (m_StateModel != null && m_StateModel.IsTempUnlocked (m_StateData.itemId)) {	// temp unlock
				return (m_StateData.price <= 0 || m_StateData.tempUnlockAsPurchased) ? LabelState.None : LabelState.Price;
			}
		    
		    if (m_StateData.lockType == CocoLockType.Level)
		    {
		        return  GetGameLevel() >= m_StateData.level ? LabelState.None : LabelState.Level;
		    }

			// set in lock state
			if (m_StateData.lockType == CocoLockType.RV) {
				bool NoAds;
				#if ABTEST
				switch (globalRecordModel.CurGPType){
				case GPType.Test_A:
					NoAds = storeControl.IsPurchased (CocoStoreID.NoAds_A);
					break;

				case GPType.Test_B:
					NoAds = storeControl.IsPurchased (CocoStoreID.NoAds_B);
					break;

				default:
					NoAds = storeControl.IsPurchased (CocoStoreID.NoAds);
					break;
				}
				#else
				NoAds = storeControl.IsPurchased (CocoStoreID.NoAds);
				#endif
				if (NoAds) {
					// no ads purchased
					return LabelState.Lock;
				} else if (CocoMainController.AdsControl.RVReady) {
					// rv ready
#if !NO_FLURRY
					CocoFlurry.LogEvent ("Video_Available_Impression", "Impression", FlurryKey);
#endif
					return LabelState.GetFree;
				} else {
#if !NO_FLURRY
					CocoFlurry.LogEvent ("Video_NOT_Available_Impression", "Impression", FlurryKey);
#endif
				}
			}

			return LabelState.Lock;
		}

		public string FlurryKey {
			get { return string.IsNullOrEmpty(m_StateData.flurryKey) ? m_StateData.itemId : m_StateData.flurryKey; }
		}

		protected virtual void UpdateState ()
		{
			if (!m_IsInited) {
				return;
			}

			m_LabelState = GetLabelState ();

			switch (m_LabelState) {
			case LabelState.GetFree:
				SetLabelActive (m_GetFreeLabel, true);
				SetLabelActive (m_LockLabel, false);
				SetLabelActive (m_PriceLabel, false);
			    SetLabelActive (m_LevelLabel, false);
				break;
			case LabelState.Lock:
				SetLabelActive (m_GetFreeLabel, false);
				SetLabelActive (m_LockLabel, true);
				SetLabelActive (m_PriceLabel, false);
			    SetLabelActive (m_LevelLabel, false);
				break;
			case LabelState.Price:
				SetLabelActive (m_GetFreeLabel, false);
				SetLabelActive (m_LockLabel, false);
				SetLabelActive (m_PriceLabel, true);
			    SetLabelActive (m_LevelLabel, false);
				break;
            case LabelState.Level:
                SetLevelState();
                break;
			default:
				SetLabelActive (m_GetFreeLabel, false);
				SetLabelActive (m_LockLabel, false);
				SetLabelActive (m_PriceLabel, false);
			    SetLabelActive (m_LevelLabel, false);
				break;
			}

		    SetState(m_LabelState);
		}

	    protected virtual void SetState(LabelState state)
	    {
	        
	    }

		void SetLabelActive (GameObject labelGo, bool active)
		{
			if (labelGo != null) {
				labelGo.SetActive (active);
			}
		}

	    protected virtual int GetGameLevel()
	    {
	        return 1;
	    }
	    
	    protected virtual void SetLevelState()
	    {
	        SetLabelActive (m_GetFreeLabel, false);
	        SetLabelActive (m_LockLabel, false);
	        SetLabelActive (m_PriceLabel, false);
	        SetLabelActive (m_LevelLabel, true);
	    }
		#endregion


		#region Unlock

		/// <summary>
		/// callback for unlock.
		/// <param type="bool">persistent or temporary</param>
		/// </summary>
		public System.Action onTempUnlocked = null;

		public bool IsLocked {
			get {
				switch (m_LabelState) {
				case LabelState.GetFree:
				case LabelState.Lock:
				case LabelState.Level:
					return true;
				}

				return false;
			} 
		}

		public void TryUnlock ()
		{
			if (CocoMainController.AdsControl.IsRVRequesting) {
				return;
			}

			Debug.LogError ("TryUnlock m_LabelState is : " + m_LabelState);
			switch (m_LabelState) {
			case LabelState.GetFree:
#if !NO_FLURRY
				var param = string.IsNullOrEmpty (m_StateData.flurryKey) ? m_StateData.itemId : m_StateData.flurryKey;
				CocoFlurry.LogEvent ("Video_Available_Clicked", "Clicks", param);
#endif
//				CocoMainController.AdsControl.RequesetRV (this);
				break;
			case LabelState.Level:
			case LabelState.Lock:
                #if ABTEST
                switch (globalRecordModel.CurGPType){

                    case GPType.Test_B:
                        storeControl.Buy(CocoStoreID.FullVersion_B);
                        break;
                    default:
                        CocoMainController.Instance.ShowMiniStorePopup (m_StateData.storeProductId);
                        break;
                }
                #else
//				if (GlobalData.IAPButtonShowMainStore(m_StateData.storeProductId))
//				{
//					CocoMainController.Instance.ShowMainStorePopup(transform.position, CocoStoreID.None);
//				}
//				else
//				{
//					CocoMainController.Instance.ShowMiniStorePopup(m_StateData.storeProductId);
//				}
                #endif
				break;
			}
		}

		public string GetRVKey ()
		{
			return m_StateData.itemId;
		}

		public void OnRvReleased ()
		{
			if (m_StateModel == null) {
				return;
			}

			m_StateModel.TempUnlockItem (m_StateData.itemId);

			UpdateState ();
			if (onTempUnlocked != null) {
				onTempUnlocked ();
			}
		}

		#endregion


		#region Purchase

		public bool IsPurchased {
			get {
				return m_LabelState == LabelState.None;
			}
		}

		public virtual bool Purchase ()
		{
			if (m_StateModel == null) {
				return false;
			}

			if (m_LabelState == LabelState.Price) {
				m_StateModel.PurchaseItem (m_StateData.itemId);
				UpdateState ();
				return true;
			}

			return false;
		}

		#endregion
	}


	//[System.Serializable]
	public class CocoLockableItemData
	{
		public string itemId = string.Empty;
		public CocoStoreID storeProductId = CocoStoreID.AllItems;
		public int price = 0;
	    public int level = 0;
		public CocoLockType lockType = CocoLockType.Non;
		public bool tempUnlockAsPurchased = true;
		public string flurryKey = string.Empty;
	}
}
