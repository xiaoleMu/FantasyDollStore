using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class SocialNetworkAuthenticateCommand : Command
	{
		[Inject]
		public ISocialNetworkService socialNetworkService {get;set;}
		
		[Inject]
		public SocialSync socialSync {get;set;}

		[Inject]
		public bool isLoggedIn { get; set; }

		//[Inject]
		//public ToggleLoadingDialogSignal toggleLoadingDialogSignal { get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		public override void Execute ()
		{
			CoreLogger.LogDebug(LoggerModules.SocialServices, "Execute IsLoggedIn " + isLoggedIn);

			socialStateModel.WaitingForSocialLogin = false;


			if(!isLoggedIn)
			{
				//toggleLoadingDialogSignal.Dispatch(false);
				return;
			}
			
			SocialConnectData socialConnectData = new SocialConnectData();
			socialConnectData.network = SocialNetwork.Facebook;
			socialConnectData.email = socialNetworkService.Email;
			socialConnectData.photoUrl = socialNetworkService.PhotoUrl;
			socialConnectData.userName = socialNetworkService.UserName;
			socialConnectData.userId = socialNetworkService.UserId;
			socialConnectData.userToken = socialNetworkService.UserToken;
			socialConnectData.initiatedByUser = true;

			socialSync.ConnectToNetwork(socialConnectData);

		}

	}

}
