using UnityEngine;
using UnityEngine.UI;
using System;
using TabTale;

namespace CocoPlay.Native
{
	public class SimpleAlertView : MonoBehaviour, IBackButtonOwner
	{
		public static void Show (string title, string message, string button, string otherButton, Action<string> buttonAction)
		{
			var alertView = CocoMainController.ShowPopup<SimpleAlertView> ("native/SimpleAlertView");
			if (alertView == null) {
				return;
			}

			alertView.InitContent (title, message, button, otherButton);

			alertView._onButtonClickEvent = buttonAction;
		}


		#region Show / Hide

		[SerializeField]
		private CocoTweenShowOrHide _entity;

		private void Close ()
		{
			_entity.Hide (() => {
				if (_onButtonClickEvent != null) {
					_onButtonClickEvent (_clickedButtonText);
				}

				var modalView = GetComponent<AppModalView> ();
				if (modalView != null) {
					modalView.Close ();
				} else {
					Destroy (gameObject);
				}
			});
		}

		public bool IsShowed {
			get { return _entity.IsShowed; }
		}

		#endregion


		#region Content

		[SerializeField]
		private Text _titleText;

		[SerializeField]
		private Text _messageText;

		[SerializeField]
		private Text _buttonText;

		[SerializeField]
		private Text _otherButtonText;

		private void InitContent (string title, string message, string button, string otherButton)
		{
			_titleText.text = title;
			_messageText.text = message;
			_buttonText.text = button;
			_otherButtonText.text = otherButton;

			if (!OnlyOneButton) {
				return;
			}

			var buttonEntity = _otherButtonText.GetComponentInParent<Button> ();
			if (buttonEntity != null) {
				buttonEntity.gameObject.SetActive (false);
			} else {
				_otherButtonText.gameObject.SetActive (false);
			}
		}

		private bool OnlyOneButton {
			get { return string.IsNullOrEmpty (_otherButtonText.text); }
		}

		#endregion


		#region Button Click

		private Action<string> _onButtonClickEvent;

		private string _clickedButtonText;

		public void OnButtonClick ()
		{
			ClickButton (_buttonText.text);
		}

		public void OnOtherButtonClick ()
		{
			ClickButton (_otherButtonText.text);
		}

		private void ClickButton (string buttonText)
		{
			if (!IsShowed) {
				return;
			}

			_clickedButtonText = buttonText;

			Close ();
		}

		#endregion


		#region Back Button

		public bool IsBackButtonHeld {
			get { return true; }
		}

		public void TriggerCloseButton ()
		{
			if (OnlyOneButton) {
				OnButtonClick ();
			} else {
				OnOtherButtonClick ();
			}
		}

		#endregion
	}
}