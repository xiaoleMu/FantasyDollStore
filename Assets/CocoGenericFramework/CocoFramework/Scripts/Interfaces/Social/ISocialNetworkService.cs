using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace TabTale
{
	public interface ISocialNetworkService
	{
		bool IsInitialized { get; }
		
		bool IsLoggedIn { get; }
		
		string UserId { get; }
		
		string AppId { get; }
		
		string UserToken { get; }
		
		string UserName { get; }
		
		string Email { get; }
		
		string PhotoUrl { get; }

		void Init ();

		void Login ();

		void Logout ();

		void GetFriendList ();

		void Publish ();

		void RequestPhoto (string id, int width, int height);

		void FacebookNotificationRequest (string message,string title, string data, List<string> recepient);

		void GetPermissions ();

		void InviteFriends();

	

	}

	public class SocialNetworkInitSignal : Signal
	{
	}
	
	public class SocialNetworkLoginCompletedSignal : Signal<bool, string>
	{
	}
	
	public class SocialNetworkPhotoSignal: Signal<string, Texture>{
		
	}
	
	public class SocialNetworkAppRequestSignal : Signal<bool, string>{
		
	}
	
	public class SocialNetworkUserNameReadySignal : Signal<string>{
		
	}

	public class SocialNetworkUserEmailReadySignal : Signal<string>{
		
	}
	
	public class SocialNetworkServerVerificationCompleteSignal : Signal<SocialNetwork>{
		
	}

}
