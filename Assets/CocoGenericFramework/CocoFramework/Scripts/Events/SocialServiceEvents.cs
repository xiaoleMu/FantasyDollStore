using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class SocialServiceEvents
	{
		[Inject]
		public SocialServiceAuthenticateSignal authenticateSignal {get;set;}

		[Inject]
		public ISocialService socialService{get;set;}

		[PostConstruct]
		public void Init()
		{
			if(socialService.IsAuthenticated)
				OnAuthenticate(true);
			else
				socialService.onAuthenticateEvent += OnAuthenticate;
		}

		void OnAuthenticate (bool res)
		{
			CoreLogger.LogDebug("SocialServiceEvents","OnAuthenticate "+res);
			socialService.onAuthenticateEvent -= OnAuthenticate;
			authenticateSignal.Dispatch (res,socialService.SocialId);
		}
	}

	public class SocialServiceAuthenticateSignal:Signal<bool,string>
	{
	}

}


