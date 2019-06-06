using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class SocialNetworkUserEmailReadyCommand : Command
	{
		[Inject]
		public SocialStateModel socialStateModel {get;set;}

		[Inject]//Command param
		public string email {get;set;}

		public override void Execute ()
		{
			socialStateModel.SetEmail(email);
		}
	}

}
