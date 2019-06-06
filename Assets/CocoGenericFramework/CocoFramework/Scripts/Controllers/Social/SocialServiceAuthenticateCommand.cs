using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class SocialServiceAuthenticateCommand : Command
	{
		[Inject]
		public ISocialService socialService {get;set;}

		[Inject]
		public SocialSync socialSync {get;set;}

		public override void Execute ()
		{
			CoreLogger.LogDebug("SocialServiceAuthenticateCommand","Execute IsAuthenticated:"+socialService.IsAuthenticated+" SocialId:"+socialService.SocialId);

			if(!socialService.IsAuthenticated)
				return;

			SocialConnectData socialConnectData = new SocialConnectData();
			socialConnectData.userId = socialService.SocialId;
			if(Application.platform == RuntimePlatform.Android)
				socialConnectData.network = SocialNetwork.PlayServices;
			else
				socialConnectData.network = SocialNetwork.GameCenter;

			socialSync.ConnectToNetwork(socialConnectData);
		}
	}

}


