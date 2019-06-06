using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TabTale.AssetManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace TabTale
{
	public class UGuiGeneralDialog : BaseModalView, IBackButtonListener, IModalDataReceiver<GeneralDialogData>
	{
		public GameObject closeBtn;
		public GameObject titleContainer;
		public GameObject messageContainer;
		public GameObject imageContainer;
		public GameObject buttonsContainer;
		public GameObject buttonsLayeout;
		public GameObject buttonPrefab;


		AppModalView _appModal;
		GeneralDialogData _data;
		IAssetManager _assetManager;
		Text _titleTxt;
		Text _messageTxt;

		#region INTERFACE_BACK_BTN
		public void SubscribeToBackButtonEvent() { }

		public void UnSubscribeFromBackButtonEvent() { }

		public bool HandleBackButtonPress()
		{
			Close ();
			return true;
		}
		#endregion

		// Use this for initialization
		protected override void Awake ()
		{
			base.Awake ();
			_appModal = GetComponent<AppModalView>();

			_assetManager = GameApplication.Instance.AssetManager;

			_titleTxt = titleContainer.GetComponentInChildren<Text> ();
			_messageTxt = messageContainer.GetComponentInChildren<Text> ();
		}

		protected override void Start ()
		{
			base.Start ();
		}

		public void SetData (GeneralDialogData data)
		{
			_data = data;
			
			InitFromData ();
		}

		void InitFromData ()
		{

			closeBtn.SetActive (_data.hasCloseButton);
			if (string.IsNullOrEmpty(_data.title)) {
				titleContainer.SetActive (false);
			} else {
				titleContainer.SetActive (true);
				_titleTxt.text = _data.title;
			}

			if (string.IsNullOrEmpty(_data.message)) {
				messageContainer.SetActive (false);
			} else {
				messageContainer.SetActive (true);
				_messageTxt.text = _data.message;
			}

			if (string.IsNullOrEmpty(_data.imagePrefabPath)) {
				imageContainer.SetActive (false);
			} else {
				InsertImage (_data.imagePrefabPath);
			}

			ClearButtons ();
			if (_data.buttons.Count == 0) {
				buttonsContainer.SetActive (false);
			} else {
				_data.buttons.ForEach (item => {
					AddButton (item);
				});
			}
		}

		void InsertImage (string prefabPath)
		{
			GameObject imgPrefab = _assetManager.GetResource<GameObject> (prefabPath);
			if (imgPrefab != null) {
				for (int i=imageContainer.transform.childCount-1; i>=0; i--) {
					Destroy (imageContainer.transform.GetChild (i).gameObject);
				}
				GameObject imgGo = Instantiate (imgPrefab) as GameObject;
				imgGo.transform.SetParent (imageContainer.transform, false);
				imageContainer.SetActive (true);
			} else {
				CoreLogger.LogWarning ("UGuiGeneralDialogController", "InsertImage prefab" + prefabPath + " not found");
				imageContainer.SetActive (false);
			}
			imageContainer.SetActive (true);
		}

		void ClearButtons ()
		{
			for (int i=buttonsLayeout.transform.childCount-1; i>=0; i--) {
				Destroy (buttonsLayeout.transform.GetChild (i).gameObject);
			}
		}

		void AddButton (BasicDialogButtonData data)
		{
			GameObject buttonGo = GameObject.Instantiate (buttonPrefab) as GameObject;

			Text caption = buttonGo.GetComponentInChildren<Text> ();
			if (caption != null)
				caption.text = data.caption;

			Button button = buttonGo.GetComponent<Button> ();
			button.onClick.AddListener (() => {data.Dispatch ();Close();});

			buttonGo.transform.SetParent (buttonsLayeout.transform, false);
		}

		public void Close ()
		{
			_appModal.Close();
		}

		public void OnCloseClick ()
		{
			Close ();
		}
	}
}