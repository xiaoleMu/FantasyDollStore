using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale
{
	public class AppInfoService : IAppInfo
	{
		private IPsdkServiceManager psdkMgr;

		#region IAppInfo implementation

		public string ApplicationId 
		{
			get 
			{
#if UNITY_IPHONE
				return PSDKMgr.Instance.GetAppID();
#else
				return PSDKMgr.Instance.BundleIdentifier;
#endif
			}
		}

		public string BundleIdentifier
		{
			get 
			{
				return PSDKMgr.Instance.BundleIdentifier;
			}
		}

		public string BundleVersion 
		{
			get 
			{
				return PsdkSerializedData.Instance.bundleVersion;
			}
		}

		#endregion


	}
}
