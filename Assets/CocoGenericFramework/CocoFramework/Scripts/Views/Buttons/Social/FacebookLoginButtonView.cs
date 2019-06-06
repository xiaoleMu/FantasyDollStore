using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Linq;
using TabTale.AssetManagement;
using System;
using UnityEngine.UI;

namespace TabTale
{
	public class FacebookLoginButtonView : MainView
	{
		[Inject]
		public ISocialNetworkService socialNetwork{ get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		[Inject]
		public SocialNetworkLoginCompletedSignal loginCompletedSignal { get; set; }
		
		public bool HideAfterLogin = true;
		public bool DisableAfterLogin = false;
		
		Button _button;

		bool _isLoggedIn = false;

		void RefreshUI()
		{
			if (_isLoggedIn) 
			{
				UpdateUIAfterLogin();
			}
			else
			{
				UpdateUIBeforeLogin();
				
			}
		}

		protected override void OnRegister()
		{
			base.OnRegister ();

			_button = GetComponent<Button>();

			RefreshUI();
		}

		protected virtual void UpdateUIBeforeLogin()
		{

		}

		protected virtual void UpdateUIAfterLogin()
		{
			if (HideAfterLogin)
			{
				Hide();
			}
			else if (DisableAfterLogin)
			{
				_button.interactable = false;
			}
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		void OnLogin (SocialNetwork network)
		{
			UpdateUIAfterLogin();
		}

		protected bool IsLoggedInAndVerified()
		{
			Debug.Log("socialNetwork.IsLoggedIn? " + socialNetwork.IsLoggedIn + ", user id: " + socialNetwork.UserId);
			Debug.Log("socialStateModel.GetState().facebookId? " + socialStateModel.GetState().facebookId);

			return socialNetwork.IsLoggedIn && socialStateModel.GetState().facebookId == socialNetwork.UserId;
		}

		public void Login ()
		{
			if (IsLoggedInAndVerified()) 
			{
				print ("already logged in");
			} 
			else 
			{
				socialNetwork.Login ();
			}
		}
	}

}
