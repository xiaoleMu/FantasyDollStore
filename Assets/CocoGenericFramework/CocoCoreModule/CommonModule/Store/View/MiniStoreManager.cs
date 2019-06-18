using UnityEngine;
using Game;

namespace CocoPlay {
	public class MiniStoreManager : CocoGenericPopupBase
	{
		public static CocoMiniStoreOpenType MiniStoreOpenType = CocoMiniStoreOpenType.ItemClickMiniStore;
		public static CocoStoreID MiniStoreID = CocoStoreID.AllItems;

		[Inject]
		public CocoStoreControl StoreControl {get; set;}
		[Inject]
		public CocoStoreUpdateStateSignal storeUpdateStateSignal {get; set;}
        [Inject]
        public StoreClickSignal storeClickSignal{ get; set;}
		[Inject]
		public CocoGlobalRecordModel globalRecordModel {get; set;}

		[SerializeField]
		private GameObject m_ItemsParent;

		protected override void Start ()
		{
			#if ABTEST
			if (globalRecordModel.CurGPType != GPType.Test_B){
				InitButton ();
			}
			#else
			InitButton ();
			#endif

			base.Start ();
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			storeUpdateStateSignal.AddListener (OnStoreUpdateState);
		}

		protected override void RemoveListeners ()
		{
			storeUpdateStateSignal.RemoveListener (OnStoreUpdateState);

			base.RemoveListeners ();
		}

		private void OnStoreUpdateState (){
			ClosePopup ();
		}

		private void InitButton ()
		{
			#if ABTEST
			CocoStoreItemButton[] buttons = m_ItemsParent.GetComponentsInChildren <CocoStoreItemButton> ();
			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				for (int i=0; i<buttons.Length; i++){
					if (buttons[i].StoreID == MiniStoreID || buttons[i].StoreID == CocoStoreID.FullVersion_A){
						buttons[i].gameObject.SetActive (true);
					}
					else {
						Destroy(buttons[i].gameObject);
					}
				}
				break;

			case GPType.Test_B:
				break;

				default:
				for (int i=0; i<buttons.Length; i++){
					if (buttons[i].StoreID == MiniStoreID || buttons[i].StoreID == CocoStoreID.FullVersion){
						buttons[i].gameObject.SetActive (true);
					}
					else {
						Destroy(buttons[i].gameObject);
					}
				}
				break;
			}
			#else
			CocoStoreItemButton[] buttons = m_ItemsParent.GetComponentsInChildren <CocoStoreItemButton> ();
			for (int i=0; i<buttons.Length; i++){
				if (buttons[i].StoreID == MiniStoreID || buttons[i].StoreID == CocoStoreID.FullVersion){
					buttons[i].gameObject.SetActive (true);
				}
				else {
					Destroy(buttons[i].gameObject);
				}
			}
			#endif
		}

		protected override void OnButtonClickWithButtonName (string buttonName)
		{
			base.OnButtonClickWithButtonName (buttonName);
			if (buttonName == "MainStore"){
                storeClickSignal.Dispatch("Main_Store");
                ClosePopup ();
				CocoMainController.Instance.ShowMainStorePopup (Vector3.zero, MiniStoreID);
			}
		}

        protected override void OnButtonClickWithButtonId(CocoUIButtonID buttonID)
        {
            switch (buttonID){
                case CocoUIButtonID.Popup_Close:
                    CloseBtnClick ();
                    break;
            }
        }

        protected override void CloseBtnClick()
        {
            storeClickSignal.Dispatch("Close");
            base.CloseBtnClick();
        }
	}
}
