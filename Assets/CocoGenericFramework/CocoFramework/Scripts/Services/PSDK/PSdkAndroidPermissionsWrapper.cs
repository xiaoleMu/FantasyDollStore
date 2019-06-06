using System;
using RSG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TabTale
{
	public class PSdkAndroidPermissionsWrapper : IAndroidPermissionWrapper
	{
		[Inject]
		public ILogger logger { get; set; }

		private static string Tag = "PSdkAndroidPermissionsWrapper";

		private static AndroidPermissionsWrapper PermsWrapper
		{
			get { return AndroidPermissionsWrapper.Instance; }
		}
			
		#region IAndroidPermissionWrapper implementation
		public IPromise<PermissionRequestResult> RequestPermissions(List<string> permissions)
		{
			logger.Log(Tag,"RequestPermissions: " + permissions.ToString());

			var result = new Promise<PermissionRequestResult>();

			Action<string[],bool[]> ResultHandler = null;
			ResultHandler = (string[] perms ,bool[] isGranted) => {
				
				PermsWrapper.OnRequestPermissionsResultEvent -= ResultHandler;

				PermissionRequestResult permissionRequestResult = PermissionRequestResult.Create(perms, isGranted);
				result.Resolve(permissionRequestResult);
			};

			PermsWrapper.OnRequestPermissionsResultEvent += ResultHandler;

			return result;
		}

		public bool IsPermissionGranted(string permission)
		{
			logger.Log(Tag, "IsPermissionGranted :" + permission);

			return PermsWrapper.CheckSelfPermission(permission);
		}

		public bool ShouldShowRequestPermissionRationale (string permission)
		{
			logger.Log(Tag, "ShouldShowRequestPermissionRationale :" + permission);

			return PermsWrapper.ShouldShowRequestPermissionRationale(permission);
		}
		#endregion
		
	}
}

