using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CocoPlay.ResourceManagement
{
	public abstract class AssetBundleLoadRequest : LoadRequest
	{
		protected AssetBundleLoadRequest (AssetBundleHolder holder)
		{
			_holder = holder;
			InitDependenciesRequest (_holder.Dependencies);
			_isDependenciesLoading = true;
		}

		private readonly AssetBundleHolder _holder;

		public AssetBundleHolder Holder {
			get { return _holder; }
		}

		public AssetBundle Asset { get; private set; }

		private bool _isDependenciesLoading;

		protected override float RequestProgress {
			get {
				if (_isDependenciesLoading) {
					return DependenciesProgress / 2f;
				}

				return (1f + MainRequestProgress) / 2f;
			}
		}

		protected override bool LoadTick ()
		{
			if (_isDependenciesLoading) {
				if (!DependenciesLoadTick ()) {
					return false;
				}

				InitMainRequest ();
				_isDependenciesLoading = false;
			}

			return MainLoadTick ();
		}

		protected override bool LoadFinish ()
		{
			Asset = MainLoadFinish ();
			ReferenceDependencies ();
			ResourceDebug.Log ("{0}->LoadFinish: name [{1}], finished [{2}]", GetType ().Name, _holder.Name, Asset != null);
			return Asset != null;
		}

		public override void Abort ()
		{
			if (IsDone) {
				return;
			}

			if (_isDependenciesLoading) {
				DependenciesAbort ();
			} else {
				MainAbort ();
			}
		}


		#region Main

		protected abstract void InitMainRequest ();

		protected abstract float MainRequestProgress { get; }

		protected abstract bool MainLoadTick ();

		protected abstract AssetBundle MainLoadFinish ();

		protected abstract void MainAbort ();

		#endregion


		#region Dependency

		private GroupLoadRequest<AssetBundleHolder> _dependenciesRequest;

		private void InitDependenciesRequest (List<AssetBundleHolder> holders)
		{
			if (holders == null || holders.Count <= 0) {
				return;
			}

			_dependenciesRequest = new GroupLoadRequest<AssetBundleHolder> (holders, holder => holder.LoadAsync ());
		}

		private float DependenciesProgress {
			get { return _dependenciesRequest != null ? _dependenciesRequest.Progress : 1f; }
		}

		private bool DependenciesLoadTick ()
		{
			return _dependenciesRequest == null || !_dependenciesRequest.MoveNext ();
		}

		private void DependenciesAbort ()
		{
			if (_dependenciesRequest != null) {
				_dependenciesRequest.Abort ();
			}
		}

		private void ReferenceDependencies ()
		{
			if (_holder.Dependencies == null) {
				return;
			}

			foreach (var dependency in _holder.Dependencies) {
				if (!dependency.IsLoaded) {
					continue;
				}

				dependency.Reference ();
			}
		}

		#endregion
	}


	public class AssetBundlePathLoadRequest : AssetBundleLoadRequest
	{
		public AssetBundlePathLoadRequest (AssetBundleHolder holder, string path) : base (holder)
		{
			_path = path;
		}

		private readonly string _path;


		#region Main

		private AssetBundleCreateRequest _mainRequest;

		protected override void InitMainRequest ()
		{
			_mainRequest = AssetBundle.LoadFromFileAsync (_path);
		}

		protected override float MainRequestProgress {
			get { return _mainRequest.progress; }
		}

		protected override bool MainLoadTick ()
		{
			return _mainRequest.isDone;
		}

		protected override AssetBundle MainLoadFinish ()
		{
			return _mainRequest.assetBundle;
		}

		protected override void MainAbort ()
		{
		}

		#endregion
	}


	public class AssetBundleUriLoadRequest : AssetBundleLoadRequest
	{
		public AssetBundleUriLoadRequest (AssetBundleHolder holder, string uri) : base (holder)
		{
			_uri = uri;
		}

		private readonly string _uri;


		#region Main

		private UnityWebRequestAsyncOperation _mainRequest;

		protected override void InitMainRequest ()
		{
#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle (_uri);
#else
            var webRequest = UnityWebRequest.GetAssetBundle (_uri);
#endif
			_mainRequest = webRequest.SendWebRequest ();
		}

		protected override float MainRequestProgress {
			get { return _mainRequest.progress; }
		}

		protected override bool MainLoadTick ()
		{
			return _mainRequest.isDone;
		}

		protected override AssetBundle MainLoadFinish ()
		{
			return DownloadHandlerAssetBundle.GetContent (_mainRequest.webRequest);
		}

		protected override void MainAbort ()
		{
			_mainRequest.webRequest.Abort ();
		}

		#endregion
	}


	public class AssetBundleODRLoadRequest : AssetBundleLoadRequest, IDisposable
	{
		public AssetBundleODRLoadRequest (AssetBundleHolder holder, string resPath, string odrTag) : base (holder)
		{
			_odrTag = odrTag;
			_resPath = resPath;
		}


		private readonly string _odrTag;
		private readonly string _resPath;

		public void Dispose ()
		{
			MainDispose ();
		}


		#region Main

		private AssetBundleCreateRequest _mainResRequest;
		private ResourceODRRequest _mainOdrRequest;

		protected override void InitMainRequest ()
		{
			_mainOdrRequest = ResourceODRLocation.PreloadAsync (new[] { _odrTag });
			_mainOdrRequest.OnCompleted += OnODRRequestCompleted;

			ResourceDebug.Log ("{0}->InitMainRequest: odrTag [{1}]", GetType ().Name, _odrTag);
		}

		protected override float MainRequestProgress {
			get {
				var resProgress = _mainResRequest != null ? _mainResRequest.progress : 0f;
				return (_mainOdrRequest.Progress + resProgress) / 2f;
			}
		}

		protected override bool MainLoadTick ()
		{
			if (_mainOdrRequest.MoveNext ()) {
				return false;
			}

			return _mainResRequest != null && _mainResRequest.isDone;
		}

		protected override AssetBundle MainLoadFinish ()
		{
			if (_mainResRequest == null) {
				return null;
			}

			ResourceDebug.Log ("{0}->MainLoadFinish: odrTag [{1}] isDone [{2}], name [{3}]", GetType ().Name, _odrTag,
				_mainResRequest.isDone, _mainResRequest.assetBundle != null ? _mainResRequest.assetBundle.name : "null");
			return _mainResRequest.assetBundle;
		}

		protected override void MainAbort ()
		{
			if (_mainOdrRequest == null) {
				return;
			}

			_mainOdrRequest.OnCompleted -= OnODRRequestCompleted;
			_mainOdrRequest.Dispose ();
			ResourceDebug.Log ("{0}->MainAbort: odrTag [{1}] dispose", GetType ().Name, _odrTag);
		}

		private void MainDispose ()
		{
			if (_mainOdrRequest == null) {
				return;
			}

			_mainOdrRequest.Dispose ();
			ResourceDebug.Log ("{0}->MainDispose: odrTag [{1}] dispose", GetType ().Name, _odrTag);
		}

		private void OnODRRequestCompleted (LoadRequest loadRequest)
		{
			if (loadRequest != _mainOdrRequest) {
				return;
			}

			ResourceDebug.Log ("{0}->OnODRRequestCompleted: isDone [{1}], error [{2}]", GetType ().Name, _mainOdrRequest.IsDone,
				_mainOdrRequest.Error);

			_mainOdrRequest.OnCompleted -= OnODRRequestCompleted;

			if (!string.IsNullOrEmpty (_mainOdrRequest.Error)) {
				Error = _mainOdrRequest.Error;
				return;
			}

			ResourceDebug.Log ("{0}->OnODRRequestCompleted: LoadFromFileAsync [{1}]", GetType ().Name, _resPath);
			_mainResRequest = AssetBundle.LoadFromFileAsync (_resPath);
		}

		#endregion
	}
}