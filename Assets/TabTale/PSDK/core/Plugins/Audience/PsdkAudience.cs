using UnityEngine;
using System.Collections;


namespace TabTale.Plugins.PSDK {
	public class PsdkAudience : IPsdkAudience {


		IPsdkAudience _impl;

		public PsdkAudience(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkAudience (); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkAudience (sm.GetImplementation ()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:		_impl = new UnityPsdkAudience (); break;
			default:
				throw new System.Exception ("Platform not supported for External Configuration.");
			}
		}

		public bool Setup() {
			bool rc = _impl.Setup();
			return rc;
		}

		public IPsdkAudience GetImplementation()
		{
			return _impl;
		}

		public void SetBirthYear (int birthYear)
		{
			_impl.SetBirthYear (birthYear);
		}
		public int GetAge ()
		{
			return _impl.GetAge ();
		}

		public PSDKAudienceMode GetAudienceMode ()
		{
			return _impl.GetAudienceMode ();
		}

		public void psdkStartedEvent() {
			_impl.psdkStartedEvent ();
		}
	}
}