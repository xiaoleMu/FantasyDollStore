using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale
{
	public class PSdkCoreServices : IPsdkCoreServices
	{
		#region IPsdkCoreServices implementation

		public bool OnBackPressed ()
		{
			return PSDKMgr.Instance.OnBackPressed();
		}

		#endregion
	}
}
