using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class ReportMaxProgressCommand : Command 
	{
		[Inject]
		public int levelReached { get; set; }

		[Inject]
		public ISyncService syncService { get; set; }

		public override void Execute ()
		{
			Debug.Log ("ReportMaxProgressCommand.Execute : reporting max progress for new level reached - " + levelReached);

			syncService.ReportMaxProgress(levelReached);
		}
	}
}