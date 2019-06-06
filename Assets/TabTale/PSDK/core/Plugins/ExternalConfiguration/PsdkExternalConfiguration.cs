using UnityEngine;
using System.Collections;


namespace TabTale.Plugins.PSDK {
	public class PsdkExternalConfiguration : IPsdkExternalConfiguration {


		IPsdkExternalConfiguration _impl;

		public PsdkExternalConfiguration(IPsdkServiceManager sm) {
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkExternalConfiguration (); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkExternalConfiguration (sm.GetImplementation ()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:		_impl = new UnityPsdkExternalConfiguration (); break;
			default:
				throw new System.Exception ("Platform not supported for External Configuration.");
			}
		}

		public bool Setup() {
			bool rc = _impl.Setup();
			return rc;
		}

		public string GetGooglePlayLicenceKey()
		{
			return _impl.GetGooglePlayLicenceKey();
		}

		public IPsdkExternalConfiguration GetImplementation()
		{
			return _impl;
		}

		public void psdkStartedEvent() {
			_impl.psdkStartedEvent();
		}

		public string GetExperimentGroup () {
			return _impl.GetExperimentGroup();
		}
	}
}