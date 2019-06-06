using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public abstract class ResourceLoadRequest : LoadRequest
	{
		public Object Asset { get; private set; }

		protected override bool LoadFinish ()
		{
			Asset = AssetLoadFinish ();
			return Asset != null;
		}

		public override void Abort ()
		{
		}

		protected abstract Object AssetLoadFinish ();
	}


	public class ResourceDefaultLoadRequest : ResourceLoadRequest
	{
		public ResourceDefaultLoadRequest (string path, System.Type type)
		{
			_request = Resources.LoadAsync (path, type);
		}

		private readonly ResourceRequest _request;

		protected override float RequestProgress {
			get { return _request.progress; }
		}

		protected override bool LoadTick ()
		{
			return _request.isDone;
		}

		protected override Object AssetLoadFinish ()
		{
			return _request.asset;
		}
	}


	public class ResourceInAssetBundleLoadRequest : ResourceLoadRequest
	{
		public ResourceInAssetBundleLoadRequest (string path, System.Type type, AssetBundle assetBundle)
		{
			_request = assetBundle.LoadAssetAsync (path, type);
		}

		private readonly AssetBundleRequest _request;

		protected override float RequestProgress {
			get { return _request.progress; }
		}

		protected override bool LoadTick ()
		{
			return _request.isDone;
		}

		protected override Object AssetLoadFinish ()
		{
			return _request.asset;
		}
	}
}