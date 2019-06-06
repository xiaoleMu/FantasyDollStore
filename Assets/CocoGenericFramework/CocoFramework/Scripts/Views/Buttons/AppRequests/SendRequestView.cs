using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace TabTale {

	public class SendRequestView : MainView 
	{
		[Inject]
		public RequestsManager requestManager { get; set; }

		[Inject]
		public ModelSyncService modelSyncService { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public ISocialService socialService { get; set; }

		public Button sendRequestButton;
		public Sprite requestSent;
		public Sprite requestAvailable;

		public RequestType requestType;

		[HideInInspector]
		public List<string> friendID;

		protected override void OnRegister ()
		{
			base.OnRegister ();
			CheckConection();
		}

		private void CheckConection()
		{
			if(networkCheck.HasInternetConnection())
			{
				if(socialService.IsAuthenticated)
				{
					CheckIfPlayerOwnsTheID ();
				}
				else
				{
					this.gameObject.SetActive (false);
				}
			}
			else
			{
				this.gameObject.SetActive (false);
			}
		}

		private void CheckIfPlayerOwnsTheID()
		{
			if (requestManager.IsPlayerID (friendID[0])) 
			{
				this.gameObject.SetActive (false);
			} 
			else 
			{
				SetRequestUI ();
			}
		}

		private void SetRequestUI()
		{
			if (!requestManager.CheckStatus (friendID[0], RequestStatus.Sent)) 
			{
				sendRequestButton.interactable = true;
				sendRequestButton.image.sprite = requestAvailable;

			} else {

				sendRequestButton.interactable = false;
				sendRequestButton.image.sprite = requestSent;
			}
		}
			
		protected override  void OnDestroy()
		{
			base.OnDestroy();
		}
			
		public void OnClick()
		{
			if (!requestManager.CheckStatus (friendID[0], RequestStatus.Sent)) 
			{
				requestManager.SendRequest (friendID, requestType);
				sendRequestButton.interactable = false;
				sendRequestButton.image.sprite = requestSent;
				Debug.Log ("<b>RequestManagerView:</b> " + "SendRequest fired!");
			}
		}
			
	}

}