using UnityEngine;
using System;
using System.IO;
using System.Collections;

namespace TabTale
{
	public class RemoteImageService
	{
		private readonly string _configBucketId = "storageBucketId";

		private string _bucketId;
			
		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private const string Tag = "RemoteImageService";

		private string StorageUrl 
		{
			get 
			{
				if(String.IsNullOrEmpty(_bucketId))
				{
					logger.LogError(Tag,"Cannot fetch image from storage, bucket Id is not defined");
					return GsdkConstants.StorageServer;
				}
				else
				{
					return 	String.Format("{0}/{1}",GsdkConstants.StorageServer,_bucketId);
				}
			}
		}
		[PostConstruct]
		public void Init()
		{
			Data.DataElement connConfig = GameApplication.GetConfiguration("connection");

			_bucketId = (connConfig.IsNull || !connConfig.ContainsKey(_configBucketId)) ? "" : (string)connConfig[_configBucketId];

		}

		public WWW LoadRemoteImage(AssetData assetData)
		{
			string imageUrl = "";

			if(assetData.assetType == AssetType.ServerImage)
			{
				imageUrl = String.Format("{0}/{1}", StorageUrl, assetData.value);
			}
			else if(assetData.assetType == AssetType.Image)
			{
				imageUrl = assetData.value;
			}
			else
			{
				logger.LogError(Tag, "Attempting to loading remote image which is not of ServerImage type");
			}

			return getCachedWWW(imageUrl);
		}

		public WWW getCachedWWW(string url)
		{
			string filePath = Application.persistentDataPath;

			filePath += "/" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url)).Substring(0,Math.Min(100, url.Length));
			string loadFilepath = filePath;
			bool web = false;
			WWW www;
			bool useCached = false;
			useCached = System.IO.File.Exists(filePath);
			if (useCached)
			{
				//check how old
				System.DateTime written = File.GetLastWriteTimeUtc(filePath);
				System.DateTime now = System.DateTime.UtcNow;
				double totalHours = now.Subtract(written).TotalHours;
				if (totalHours > 300)
					useCached = false;
			}
			if (System.IO.File.Exists(filePath))
			{
				string pathforwww = "file://" + loadFilepath;
				Debug.Log("RemoteImageService - TRYING FROM CACHE " + url + "  file " + pathforwww);
				www = new WWW(pathforwww);
			}
			else
			{
				web = true;
				www = new WWW(url);
			}
			routineRunner.StartCoroutine(doLoad(www, filePath, web));
			return www;
		}
		
		static IEnumerator doLoad(WWW www, string filePath, bool web)
		{
			yield return www;
			
			if (www.error == null)
			{
				if (web)
				{
					//System.IO.Directory.GetFiles
					Debug.Log("RemoteImageService - SAVING DOWNLOAD  " + www.url + " to " + filePath);
					// string fullPath = filePath;
					File.WriteAllBytes(filePath, www.bytes);
					Debug.Log("RemoteImageService - SAVING DONE  " + www.url + " to " + filePath);
					//Debug.Log("FILE ATTRIBUTES  " + File.GetAttributes(filePath));
					//if (File.Exists(fullPath))
					// {
					//    Debug.Log("File.Exists " + fullPath);
					// }
				}
				else
				{
					Debug.Log("RemoteImageService - SUCCESS CACHE LOAD OF " + www.url);
				}
			}
			else
			{
				if (!web)
				{
					File.Delete(filePath);
				}
				Debug.Log("RemoteImageService - WWW ERROR " + www.error);
			}
		}
	}
}
