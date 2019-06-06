using UnityEngine;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using TabTale;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#else
using CocoUIButtonID = Game.CocoUIButtonID;
using CocoAudioID = Game.CocoAudioID;
using CocoLanguage = Game.CocoLanguage;
using CocoSceneID = Game.CocoSceneID;
#endif

namespace CocoPlay{
	[RequireComponent(typeof(AppModalView))]
	public class CocoGenericPopupBase : GameView, IBackButtonListener
	{
		protected CocoMainController m_MainController;

		[SerializeField]
		protected GameObject m_ScaleParent;
		[SerializeField]
		AppModalView m_ModaleView;
		[SerializeField]
		protected float mNormalAniTime = 0.5f;
		[SerializeField]
		protected string m_PopupKey;

		[Inject]
		public CocoPopupShowSignal popupShowSignal {get; set;}
		[Inject]
		public CocoPopupClosedSignal popupClosedSignal {get; set;}
		[Inject]
		public CocoRemovePopupSignal m_CloseSignal{ get; set; }
		[Inject]
		public SoundManager soundManagerPop { get; set; }

		[SerializeField]
		protected bool m_NeedAudio = true;

		[SerializeField]
		private bool m_IgnoreTimeScale = true;

		public bool IgnoreTimeScale {
			get { return m_IgnoreTimeScale; }
			set { m_IgnoreTimeScale = value; }
		}

		public System.Action onClose;

		#region init

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected override void AddListeners()
		{
			base.AddListeners();

			uiButtonClickSignal.AddListener (OnUIButtonClick);
			m_CloseSignal.AddListener(CloseBtnClick);
			SubscribeToBackButtonEvent ();
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners()
		{
			uiButtonClickSignal.RemoveListener (OnUIButtonClick);
			m_CloseSignal.RemoveListener(CloseBtnClick);
			UnSubscribeFromBackButtonEvent();
			base.RemoveListeners();
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			Init ();
			ShowPopup ();
			popupShowSignal.Dispatch(m_PopupKey);
		}

//		private IEnumerator StartPopup(){
//			yield return new WaitForSeconds (ShowPopup ());
//			OnShowPopFinished ();
//		}

		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		protected override void OnDestroy (){
			Clean ();
			base.OnDestroy ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init (){
			if (m_ModaleView == null)
				m_ModaleView = GetComponent<AppModalView>();

			m_MainController = CocoMainController.Instance;

//			ShowPopup ();
		}

		/// <summary>
		/// Clean this instance.
		/// </summary>
		protected virtual void Clean (){
			//清除UI信息。

			m_ModaleView = null;
		}

		/// <summary>
		/// Shows the popup.
		/// </summary>
		virtual protected void ShowPopup (){
			// 显示弹框，添加弹框动画。
			if (m_NeedAudio)
				soundManagerPop.PlaySound (new List<string> { "showPopupAudio" }, SoundLayer.Main);
			m_ScaleParent.transform.localScale = Vector3.zero;
			gameObject.SetActive(true);
			LeanTween.scale(m_ScaleParent, Vector3.one, mNormalAniTime).setEase(LeanTweenType.easeInOutSine).setIgnoreTimeScale(IgnoreTimeScale).setOnComplete(()=>{
				OnShowFinished ();
			});
		}

		virtual protected void OnShowFinished (){

		}

		/// <summary>
		/// Closes the popup.
		/// </summary>
		virtual protected void CloseBtnClick (){
			// 关闭弹框，添加弹框关闭动画。
			LeanTween.scale(m_ScaleParent, Vector3.zero, mNormalAniTime).setEase(LeanTweenType.easeInOutSine).setIgnoreTimeScale(IgnoreTimeScale).setOnComplete(()=>{
//				Debug.LogError ("ClosePopup Finished !!!");
				ClosePopup ();
			});
		}

		/// <summary>
		/// Closes the popup.
		/// </summary>
		virtual  protected void ClosePopup (){
			m_ModaleView.Close ();
			popupClosedSignal.Dispatch (m_PopupKey);
			if(onClose != null)
				onClose();
		}

		#endregion

		#region button click
		[Inject]
		public CocoUIButtonClickSignal uiButtonClickSignal {get; set;}
		/// <summary>
		/// Raises the user interface button click event.
		/// </summary>
		/// <param name="buttonID">Button I.</param>
		protected virtual void OnUIButtonClick (CocoUINormalButton button)
		{
			if (! gameObject.activeInHierarchy) return;

			if (button.UseButtonID) {
				OnButtonClickWithButtonId (button.ButtonID);
			} else {
				OnButtonClickWithButtonName (button.ButtonKey);
			}
		}

		/// <summary>
		/// Raises the button click with button identifier event.
		/// </summary>
		/// <param name="pButtonId">P button identifier.</param>
		protected virtual void OnButtonClickWithButtonId (CocoUIButtonID buttonID){
			//当使用的按钮以按钮id为key时继承
			switch (buttonID){
			case CocoUIButtonID.Popup_Close:
				CloseBtnClick ();
				break;
			}
		}

		/// <summary>
		/// Raises the button click with button name event.
		/// </summary>
		/// <param name="pButtonName">P button name.</param>
		protected virtual void OnButtonClickWithButtonName (string buttonName){
			//当使用的按钮以按钮名的名字为key时继承
		}

		#endregion

		#region back button

		[Inject]
		public BackButtonService backButtonService { get; set; }

		/// <summary>
		/// Subscribes to back button event.
		/// </summary>
		public void SubscribeToBackButtonEvent()
		{
			backButtonService.AddListener(this);
		}

		/// <summary>
		/// Uns the subscribe from back button event.
		/// </summary>
		public void UnSubscribeFromBackButtonEvent()
		{
			backButtonService.RemoveListener(this);
		}

		/// <summary>
		/// Handles the back button press.
		/// </summary>
		/// <returns><c>true</c>, if back button press was handled, <c>false</c> otherwise.</returns>
		public bool HandleBackButtonPress()
		{
			return OnBackButtonPress();
		}

		/// <summary>
		/// Raises the back button press event.
		/// </summary>
		protected virtual bool OnBackButtonPress()
		{
            if (m_MainController != null && m_MainController.TouchEnable == false)
				return true;

			CloseBtnClick();
			return true;
		}

		#endregion

	}

	public class CocoPopupShowSignal : Signal<string> {};
	public class CocoPopupClosedSignal : Signal<string> {};
	public class CocoRemovePopupSignal : Signal {};
}