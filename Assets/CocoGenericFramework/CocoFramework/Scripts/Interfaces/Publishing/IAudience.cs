using UnityEngine;
using System.Collections;
using strange.extensions.promise.api;
using TabTale.Plugins.PSDK;

namespace TabTale 
{
	public interface IAudience
	{
		void SetBirthYear (int birthYear);
		int GetAge ();
		PSDKAudienceMode GetAudienceMode ();
	}
}