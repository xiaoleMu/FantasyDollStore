using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale
{
	public class PSdkCrashTools : ICrashTools
	{
		private readonly bool IsStandAlone = (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer);

		#region ICrashTools implementation
		public void AddBreadCrumb (string crumb)
		{
			if(IsStandAlone) return;

			PSDKMgr.Instance.GetCrashMonitoringToolService().AddBreadCrumb(crumb);
		}
		public void ClearAllBreadCrumbs ()
		{
			if(IsStandAlone) return;

			PSDKMgr.Instance.GetCrashMonitoringToolService().ClearAllBreradCrumbs();
		}
		#endregion
	}
}
