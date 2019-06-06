using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace CocoPlay
{
	public class CocoPrivacyPolicyButton : CocoUINormalButton
	{
		protected override void OnClick ()
		{
			CocoMainController.PluginManager.showPrivacyPolicy ();
		}
	}
}

