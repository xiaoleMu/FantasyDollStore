using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceODRLocation
	{
		#region Settings

		public static string GetResourcePath (string odrTag, string resName)
		{
			if (!Application.isEditor) {
				return "res://" + resName;
			}

			var virtualTagPath = GetVirtualFullTagPath (odrTag);
			return Path.Combine (virtualTagPath, resName);
		}

		#endregion


		#region Virtual

		private const string VIRTUAL_DIRECTORY = "odr";

		public static string VirtualRootPath {
			get { return ResourceSettings.CombinePath (ResourceSettings.VirtualRootPath, VIRTUAL_DIRECTORY); }
		}

		public static string VirtualFullRootPath {
			get { return Path.Combine (ResourceSettings.VirtualFullRootPath, VIRTUAL_DIRECTORY); }
		}

		public static string GetVirtualTagPath (string odrTag)
		{
			return Path.Combine (VirtualRootPath, odrTag);
		}

		public static string GetVirtualFullTagPath (string odrTag)
		{
			return Path.Combine (VirtualFullRootPath, odrTag);
		}

		#endregion


		#region Load

		private static readonly HashSet<string> _downloadedVirtualTags = new HashSet<string> ();

		public static ResourceODRRequest PreloadAsync (string[] tags)
		{
			var isVirtual = Application.isEditor;
			var downloadTags = isVirtual ? CollectUnDownloadedVirtualTags (tags) : tags;

			var request = new ResourceODRRequest (downloadTags, isVirtual);
			request.OnCompleted += loadRequest => {
				if (loadRequest != request || request.HasError) {
					return;
				}

				if (isVirtual) {
					OnVirtualTagsDownloadFinish (downloadTags);
				}
			};

			return request;
		}

		private static void OnVirtualTagsDownloadFinish (string[] tags)
		{
			foreach (var tag in tags) {
				ResourceDebug.Log ("ResourceODRLocation->OnVirtualTagsDownloadFinish: [{0}]", tag);
				_downloadedVirtualTags.Add (tag);
			}
		}

		private static string[] CollectUnDownloadedVirtualTags (string[] tags)
		{
			var virtualTags = new List<string> (tags.Length);

			foreach (var tag in tags) {
				if (_downloadedVirtualTags.Contains (tag)) {
					continue;
				}

				virtualTags.Add (tag);
				ResourceDebug.Log ("ResourceODRLocation->CollectUnDownloadedVirtualTags: [{0}]", tag);
			}

			return virtualTags.ToArray ();
		}

		#endregion
	}
}