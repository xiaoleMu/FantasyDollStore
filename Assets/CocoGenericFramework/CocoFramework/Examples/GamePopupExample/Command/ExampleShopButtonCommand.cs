using System;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TabTale;

namespace TabTale
{
	public class ExampleShopButtonCommand : Command
	{
		[Inject]
		public IModalityManager modalityManager { get; set; }
		
		public override void Execute()
		{
			CoreLogger.LogDebug("ExampleShopButtonCommand","Execute");
			
			modalityManager.Add(new AppModalHandle("GamePopups/ExampleShopModal"),true);
			
		}
		
		
	}
}
