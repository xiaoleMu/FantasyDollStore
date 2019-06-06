using UnityEngine;
using System;
using System.Collections;
//using System.Data;
using System.Reflection;
using LitJson;
using strange.extensions.command.impl;

namespace TabTale
{
	public class OnRequestReceivedCommand : GameCommand
	{
		[Inject]
		public RequestStateModel requestStateModel { get; set; }

		public override void Execute ()
		{
			logger.Log(Tag,"Execute");

		}
	}
	
}
