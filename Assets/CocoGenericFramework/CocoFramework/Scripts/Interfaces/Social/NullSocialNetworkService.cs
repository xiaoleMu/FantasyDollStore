using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class NullSocialNetworkProvider : ISocialNetworkService
	{
		public bool IsInitialized { get; private set; }
		
		public bool IsLoggedIn { get; private set;}
		
		public string UserId { get; private set;}
		
		public string AppId { get; private set;}
		
		public string UserToken { get; private set;}
		
		public string UserName { get; private set;}
		
		public string Email { get; private set;}
		
		public string PhotoUrl { get; private set;}

		public void Init () { CoreLogger.LogDebug("NullSocialNetworkProvider", "Init"); }
		
		public void Login () { CoreLogger.LogDebug("NullSocialNetworkProvider", "Login"); }
		
		public void Logout () { CoreLogger.LogDebug("NullSocialNetworkProvider", "Logout"); }
		
		public void GetFriendList () { CoreLogger.LogDebug("NullSocialNetworkProvider", "GetFriendList"); }
		
		public void Publish () { CoreLogger.LogDebug("NullSocialNetworkProvider", "Publish"); }
		
		public void RequestPhoto (string id, int width, int height) { CoreLogger.LogDebug("NullSocialNetworkProvider", "RequestPhoto"); }
		
		public void FacebookNotificationRequest (string message,string title, string data, List<string> recepient) { CoreLogger.LogDebug("NullSocialNetworkProvider", "RecommendToFriends"); }
		
		public void GetPermissions () { CoreLogger.LogDebug("NullSocialNetworkProvider", "GetPermissions"); }

		public void InviteFriends () { CoreLogger.LogDebug("NullSocialNetworkProvider", "InviteFriends"); }

	}
}
