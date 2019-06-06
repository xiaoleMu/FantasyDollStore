using System.IO;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public abstract class LocationHolder : LoadHolder<LocationLoadRequest>
	{
		#region Asset Bundle

		public abstract bool IsAssetBundleSyncLoadSupported { get; }

		public abstract AssetBundle LoadAssetBundle (AssetBundleHolder assetBundleHolder);

		public abstract AssetBundleLoadRequest LoadAssetBundleAsync (AssetBundleHolder assetBundleHolder);

		#endregion
	}


	public class StreamingLocationHolder : LocationHolder
	{
		public StreamingLocationHolder (string rootPath)
		{
			_rootPath = rootPath;
		}

		private readonly string _rootPath;


		#region Load

		public override bool IsSyncLoadSupported {
			get { return true; }
		}

		protected override bool LoadProcess ()
		{
			return true;
		}

		protected override bool UnloadProcess ()
		{
			return true;
		}

		#endregion


		#region Request

		protected override LocationLoadRequest CreateLoadRequest ()
		{
			return new LocationDefaultLoadRequest (this);
		}

		protected override void OnLoadRequestReceived (LocationLoadRequest loadRequest)
		{
		}

		#endregion


		#region Asset Bundle

		public override bool IsAssetBundleSyncLoadSupported {
			get { return true; }
		}

		public override AssetBundle LoadAssetBundle (AssetBundleHolder assetBundleHolder)
		{
			var path = GetAssetBundlePath (assetBundleHolder.Name);
			return AssetBundle.LoadFromFile (path);
		}

		public override AssetBundleLoadRequest LoadAssetBundleAsync (AssetBundleHolder assetBundleHolder)
		{
			var path = GetAssetBundlePath (assetBundleHolder.Name);
			return new AssetBundlePathLoadRequest (assetBundleHolder, path);
		}

		private string GetAssetBundlePath (string assetBundleName)
		{
			return Path.Combine (_rootPath, assetBundleName);
		}

		#endregion
	}


	public class ServerLocationHolder : LocationHolder
	{
		public ServerLocationHolder (string rootUri)
		{
			_rootUri = rootUri;
		}

		private readonly string _rootUri;


		#region Load

		public override bool IsSyncLoadSupported {
			get { return true; }
		}

		protected override bool LoadProcess ()
		{
			return true;
		}

		protected override bool UnloadProcess ()
		{
			return true;
		}

		#endregion


		#region Request

		protected override LocationLoadRequest CreateLoadRequest ()
		{
			return new LocationDefaultLoadRequest (this);
		}

		protected override void OnLoadRequestReceived (LocationLoadRequest loadRequest)
		{
		}

		#endregion


		#region Asset Bundle

		public override bool IsAssetBundleSyncLoadSupported {
			get { return false; }
		}

		public override AssetBundle LoadAssetBundle (AssetBundleHolder assetBundleHolder)
		{
			return null;
		}

		public override AssetBundleLoadRequest LoadAssetBundleAsync (AssetBundleHolder assetBundleHolder)
		{
			var uri = GetAssetBundleUri (assetBundleHolder.Name);
			return new AssetBundleUriLoadRequest (assetBundleHolder, uri);
		}

		private string GetAssetBundleUri (string assetBundleName)
		{
			return Path.Combine (_rootUri, assetBundleName);
		}

		#endregion
	}


	public class ODRLocationHolder : LocationHolder
	{
		public ODRLocationHolder (string odrTag)
		{
			_odrTag = odrTag;
		}

		private readonly string _odrTag;


		#region Load

		public override bool IsSyncLoadSupported {
			get { return false; }
		}

		protected override bool LoadProcess ()
		{
			return false;
		}

		protected override bool UnloadProcess ()
		{
			return true;
		}

		#endregion


		#region Request

		protected override LocationLoadRequest CreateLoadRequest ()
		{
			return new LocationODRLoadRequest (this, _odrTag);
		}

		protected override void OnLoadRequestReceived (LocationLoadRequest loadRequest)
		{
			var odrLoadRequest = loadRequest as LocationODRLoadRequest;
			if (odrLoadRequest != null) {
				odrLoadRequest.Dispose ();
			}
		}

		#endregion


		#region Asset Bundle

		public override bool IsAssetBundleSyncLoadSupported {
			get { return false; }
		}

		public override AssetBundle LoadAssetBundle (AssetBundleHolder assetBundleHolder)
		{
			var path = GetAssetBundleResourcePath (assetBundleHolder.Name);
			return AssetBundle.LoadFromFile (path);
		}

		public override AssetBundleLoadRequest LoadAssetBundleAsync (AssetBundleHolder assetBundleHolder)
		{
			var path = GetAssetBundleResourcePath (assetBundleHolder.Name);
			var request = new AssetBundleODRLoadRequest (assetBundleHolder, path, _odrTag);

			request.OnCompleted += loadRequest => {
				ResourceDebug.Log ("{0}->LoadAssetBundleAsync: request finish: name {1}, same {2}, done {3}, path {4}",
						GetType ().Name, assetBundleHolder.Name, loadRequest == request, request.IsDone, path);
				if (loadRequest == request) {
					assetBundleHolder.OnUnload += () => request.Dispose ();
				}
			};

			return request;
		}

		private string GetAssetBundleResourcePath (string assetBundleName)
		{
			return ResourceODRLocation.GetResourcePath (_odrTag, assetBundleName);
		}

		#endregion
	}
}