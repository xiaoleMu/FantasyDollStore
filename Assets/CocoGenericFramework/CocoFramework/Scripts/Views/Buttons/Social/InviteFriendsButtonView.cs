using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


namespace TabTale 
{
	public class InviteFriendsButtonView : MainView 
	{
		[Inject]
		public ISocialNetworkService socialNetwork { get; set;}

		//new parametars in DB needed in GeneralParametars with appropriate values

		public void OnClick ()
		{
				if (socialNetwork.IsLoggedIn)
				{
					socialNetwork.InviteFriends();
				} 
				else	
				{
					Debug.Log ("InviteFriendsButtonView - User not authenticated to social network, attempting to log in");
					socialNetwork.Login ();
				}
			}
		}
}
