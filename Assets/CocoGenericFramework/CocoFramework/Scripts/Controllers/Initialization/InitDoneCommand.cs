using System;
using UnityEngine;
using strange.extensions.command.impl;

using strange.framework.impl;

namespace TabTale
{
	public class InitDoneCommand : Command 
	{
		public override void Execute()
		{
			GameApplication.Instance.StrangeServicesInitDone = true;
		}
	}
}