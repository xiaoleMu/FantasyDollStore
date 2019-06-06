using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class GameCommand : Command 
	{
		[Inject]
		public ILogger logger { get; set; }

		protected string Tag { get; set; }

		public GameCommand() : base()
		{
			SetTag()	;
		}

		protected void SetTag()
		{
			Tag = GetType().Name;
		}
	}
}