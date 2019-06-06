using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Linq;
using TabTale;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

namespace CocoPlay
{
	public class CocoGenericSceneBase : GameView, IBackButtonListener
	{
		#if COCO_FAKE
		[Inject]
		public CocoGlobalData GlobalData {get; set;}
		#else
		[Inject]
		public Game.GameGlobalData GlobalData {get; set;}
		#endif

		[Inject]
		public CocoSceneLoadingFinishSignal loadFinishedSignal { get; set; }
	    
	    [Inject]
	    public CocoSceneLoadingStartSignal loadStartSignal { get; set; }

		protected CocoMainController m_MainController;
		protected CocoSceneID m_SceneID;
		[SerializeField]
		protected CocoSceneID BackSceneID = CocoSceneID.None;

		#region init

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		override protected void AddListeners ()
		{
			base.AddListeners ();

			SubscribeToBackButtonEvent ();

			uiButtonClickSignal.AddListener (OnUIButtonClick);
			loadFinishedSignal.AddListener (OnLoadingFinished);
		    loadStartSignal.AddListener(OnLoadingStart);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		override protected void RemoveListeners ()
		{
			UnSubscribeFromBackButtonEvent ();

			uiButtonClickSignal.RemoveListener (OnUIButtonClick);
			loadFinishedSignal.RemoveListener (OnLoadingFinished);
		    loadStartSignal.RemoveListener(OnLoadingStart);

			base.RemoveListeners ();
		}

		protected override void OnRegister ()
		{
			base.OnRegister ();

			RegisterInModule ();
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start ()
		{
			base.Start ();

			initScene ();
			initCharacter ();
			initUI ();

			//由于CoverPage界面不通过我们自己的场景切换加载，特殊处理。
			if (GlobalData.CurSceneID == CocoSceneID.None) 
			{
				GlobalData.CurSceneID = m_SceneID;
				StartCoroutine(EditorLoadingFinished());
			}
		}

		IEnumerator EditorLoadingFinished()
		{
			yield return new WaitForEndOfFrame();
			OnLoadingFinished ();
		}

		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
		}

		public virtual void Clear (){
			PlayEnd ();
			cleanUI ();
			cleanCharacter ();
			cleanScene ();
		}

		/// <summary>
		/// Inits the character.
		/// </summary>
		virtual protected void initCharacter ()
		{
			//初始化人物，动作等。
		}

		/// <summary>
		/// Cleans the character.
		/// </summary>
		virtual protected void cleanCharacter ()
		{
			//清除人物，动作等。
		}

		/// <summary>
		/// Inits the scene.
		/// </summary>
		virtual protected void initScene ()
		{
			//初始化场景信息。
			m_MainController = CocoMainController.Instance;
		}

		/// <summary>
		/// Cleans the scene.
		/// </summary>
		virtual protected void cleanScene ()
		{
			//清除场景信息。
		}

		/// <summary>
		/// Inits the U.
		/// </summary>
		virtual protected void initUI ()
		{
			//初始化UI信息。

		}

		/// <summary>
		/// Cleans the U.
		/// </summary>
		virtual protected void cleanUI ()
		{
			//清除UI信息。
		}

		/// <summary>
		/// Plaies the start.
		/// </summary>
		protected virtual void PlayStart ()
		{
			//初始化结束，开始场景
		}

		/// <summary>
		/// Plaies the end.
		/// </summary>
		protected virtual void PlayEnd ()
		{
			//场景结束
		}
	    
	    virtual protected void OnLoadingStart (CocoSceneID nextSceneID)
	    {
	       
	    }

		/// <summary>
		/// Raises the loading finished event.
		/// </summary>
		virtual protected void OnLoadingFinished ()
		{
			setingScene ();
//			CocoAudio.PlayBgMusic (m_SceneID);

			PlayStart ();
		}

		/// <summary>
		/// Setings the scene.
		/// </summary>
		private void setingScene ()
		{
			//设置场景UI触摸时的判断为拖拽的参数，解决某些机型按钮不容易点击的问题。
			if (EventSystem.current != null) {
				EventSystem.current.pixelDragThreshold = (int)(0.5f * Screen.dpi / 2.54f);
			}
				
//			StartCoroutine (ClearLoadAssets ());
		}

		IEnumerator ClearLoadAssets ()
		{
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();

			Resources.UnloadUnusedAssets ();
			System.GC.Collect ();
		}

		#endregion

		#region back button

		[Inject]
		public BackButtonService backButtonService { get; set; }

		/// <summary>
		/// Subscribes to back button event.
		/// </summary>
		public void SubscribeToBackButtonEvent ()
		{
			backButtonService.AddListener (this);
		}

		/// <summary>
		/// Uns the subscribe from back button event.
		/// </summary>
		public void UnSubscribeFromBackButtonEvent ()
		{
			backButtonService.RemoveListener (this);
		}

		/// <summary>
		/// Handles the back button press.
		/// </summary>
		/// <returns><c>true</c>, if back button press was handled, <c>false</c> otherwise.</returns>
		public bool HandleBackButtonPress ()
		{
			return OnBackButtonPress ();
		}

		/// <summary>
		/// Raises the back button press event.
		/// </summary>
		protected virtual bool OnBackButtonPress ()
		{
			Debug.LogError ("Scene OnBackButtonPress");
			if (m_MainController.TouchEnable == false)
				return true;
			Debug.LogError ("Scene OnBackButtonPress11111");
			if (CocoMainController.isPopupShowing)
				return true;
			Debug.LogError ("Scene OnBackButtonPress22222");
			if (BackSceneID == CocoSceneID.None) {
				backButtonService.DefaultBackButtonAction ();
			} else {
				OnBackButtonClick ();
			}
			return true;
		}

		#endregion

		#region button click

		protected virtual void OnBackButtonClick (){
			CocoMainController.EnterScene (BackSceneID);
		}

		[Inject]
		public CocoUIButtonClickSignal uiButtonClickSignal { get; set; }
		/// <summary>
		/// Raises the user interface button click event.
		/// </summary>
		/// <param name="buttonID">Button I.</param>
		private void OnUIButtonClick (CocoUINormalButton button)
		{
			if (button.UseButtonID) {
				OnButtonClickWithButtonId (button, button.ButtonID);
			} else {
				OnButtonClickWithButtonName (button, button.ButtonKey);
			}
		}

		/// <summary>
		/// Raises the button click with button identifier event.
		/// </summary>
		/// <param name="pButtonId">P button identifier.</param>
		protected virtual void OnButtonClickWithButtonId (CocoUINormalButton button, CocoUIButtonID buttonID)
		{
			//当使用的按钮以按钮id为key时继承
			switch (buttonID)
			{
			case CocoUIButtonID.Common_Back:
				OnBackButtonClick ();
				break;
//			case GameButtonID.GoStore:
//	//			m_MainController.ShowPopup(PlayerKey.Store_MainStore, IModalMaskType.NonMasked);
//				break;
//
//			case GameButtonID.ItemStore:
//	//			m_MainController.ShowPopup(PlayerKey.Store_MainStore, IModalMaskType.NonMasked);
//	//			FindObjectOfType<MainStoreManager>().curButtonID = GameButtonID.ItemStore;
//				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Raises the button click with button name event.
		/// </summary>
		/// <param name="pButtonName">P button name.</param>
		protected virtual void OnButtonClickWithButtonName (CocoUINormalButton button, string pButtonName)
		{
			//当使用的按钮以按钮名的名字为key时继承
		}

		#endregion


		#region Scene Module

		[Inject]
		public CocoSceneModuleBase sceneModule { get; set; }

		void RegisterInModule ()
		{
			sceneModule.SceneManager = this;
			m_SceneID = sceneModule.Data.sceneId;
		}

		#endregion
	}
}


