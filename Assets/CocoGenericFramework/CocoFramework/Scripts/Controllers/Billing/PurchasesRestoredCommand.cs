using UnityEngine;
using strange.extensions.command.impl;

namespace TabTale
{
	public class PurchasesRestoredCommand : Command
	{
		public override void Execute()
		{
			PlayerPrefs.SetInt("restoredPurchases", 1);
		}
	}
}