using System;
using System.Collections.Generic;
using RSG;
using System.Linq;

namespace TabTale
{
	static class GsdkAndroidPermissions
	{
		public const string CAMERA = "android.permission.CAMERA";
		public const string READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
		public const string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
	}

	public class PermissionRequestResult
	{
		public List<Tuple<string,bool>> permissions;

		public static PermissionRequestResult Create(string[] perms, bool[] isGranted)
		{
			PermissionRequestResult result = new PermissionRequestResult();

			result.permissions = new List<Tuple<string,bool>>();
			for(int i=0; i<perms.Length; i++)
			{
				result.permissions.Add(new Tuple<string,bool>(perms[i],isGranted[i]));
			}

			return result;
		}
		public bool IsGranted(string permission)
		{
			Tuple<string,bool> t = permissions.FirstOrDefault(p => p.Item1 == permission);

			if(t == null) return false;

			return t.Item2;
		}

		public override string ToString ()
		{
			return permissions.ToString();
		}
	}

	public interface IAndroidPermissionWrapper
	{
		bool IsPermissionGranted(string permission);

		bool ShouldShowRequestPermissionRationale(string permission);

		IPromise<PermissionRequestResult> RequestPermissions(List<string> permissions);
	}
}

