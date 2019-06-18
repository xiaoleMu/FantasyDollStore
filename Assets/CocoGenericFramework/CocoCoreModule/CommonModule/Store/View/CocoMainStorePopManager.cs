using UnityEngine;
using Game;

namespace CocoPlay{
	public class CocoMainStorePopManager : CocoGenericPopupBase
	{
        [Inject]
        public StoreClickSignal storeClickSignal{ get; set;}
		public Vector3 m_ClickButtonPos = Vector3.zero;
		private Vector3 m_StartPos;
		[SerializeField]
		private GameObject m_RestoreBtn;

		public static CocoStoreID MiniStoreID;

		protected override void Start ()
		{
			#if UNITY_ANDROID
			m_RestoreBtn.SetActive (false);
			#endif

			base.Start ();
		}

		protected override void ShowPopup()
		{
			base.ShowPopup();

			var temp = CocoMainController.UICamera.WorldToScreenPoint(m_ClickButtonPos);
			m_StartPos = GameObject.Find("DialogCanvas Camera").GetComponent<Camera>().ScreenToWorldPoint(temp);
			if (m_ClickButtonPos == Vector3.zero){
				m_StartPos = m_ScaleParent.transform.position;
			}

			Vector3 endPos = m_ScaleParent.transform.position;
			m_ScaleParent.transform.position = m_StartPos;
			LeanTween.move(m_ScaleParent.gameObject, endPos, mNormalAniTime).setEase(LeanTweenType.easeInOutSine);
		}

		protected override void CloseBtnClick()
		{
            storeClickSignal.Dispatch("Close");
			base.CloseBtnClick();
			LeanTween.move(m_ScaleParent.gameObject, m_StartPos, mNormalAniTime).setEase(LeanTweenType.easeInOutSine);
		}


        protected override void OnButtonClickWithButtonId(CocoUIButtonID buttonID)
        {
            switch (buttonID){
                case CocoUIButtonID.Popup_Close:
                    CloseBtnClick();
                    break;
            }
        }

	}
}
