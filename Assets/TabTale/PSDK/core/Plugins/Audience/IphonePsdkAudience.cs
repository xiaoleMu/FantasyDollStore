using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;
//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;


namespace TabTale.Plugins.PSDK {
	
	public class IphonePsdkAudience : IPsdkAudience {

		[DllImport ("__Internal")]
		private static extern void psdkAudienceSetBirthYear(int birthYear);

		[DllImport ("__Internal")]
		private static extern int psdkAudienceGetAge();

		[DllImport ("__Internal")]
		private static extern int psdkAudiencegetAudienceMode();

		public bool  Setup() {
			return true;
		}

		public IPsdkAudience GetImplementation()
		{
			return this;
		}

		public void SetBirthYear (int birthYear)
		{
			psdkAudienceSetBirthYear (birthYear);
		}
		public int GetAge ()
		{
			return psdkAudienceGetAge();
		}

		public PSDKAudienceMode GetAudienceMode ()
		{
			int modeInt = psdkAudiencegetAudienceMode ();
			PSDKAudienceMode retMode = PSDKAudienceMode.NON_CHILDREN;
			switch(modeInt){
			case 0:
				retMode = PSDKAudienceMode.CHILDREN;
				break;
			case 1:
				retMode = PSDKAudienceMode.MIXED_UNKNOWN;
				break;
			case 2:
				retMode = PSDKAudienceMode.MIXED_NON_CHILDREN;
				break;
			case 3:
				retMode = PSDKAudienceMode.NON_CHILDREN;
				break;
			case 4:
				retMode = PSDKAudienceMode.MIXED_CHILDREN;
				break;

			default:
				break;
			}

			return retMode;
		}

		public void psdkStartedEvent() {

		}
	}

}
