using System;

namespace CocoPlay.ResourceManagement
{
	public abstract class LoadHolder<TRequest> : IDisposable where TRequest : LoadRequest
	{
		public void Dispose ()
		{
			Unload ();
		}


		#region Load

		public bool IsLoaded { get; private set; }

		public bool IsLoading {
			get { return _loadRequest != null; }
		}

		public abstract bool IsSyncLoadSupported { get; }

		public bool Load ()
		{
			if (IsLoaded) {
				return true;
			}

			if (IsLoading) {
				ResourceDebug.Log ("{0}->Load: async request is loading. ", GetType ().Name);
				return false;
			}

			if (!IsSyncLoadSupported) {
				ResourceDebug.Log ("{0}->Load: synchronous load is NOT supported. ", GetType ().Name);
				return false;
			}

			IsLoaded = LoadProcess ();
			return IsLoaded;
		}

		public TRequest LoadAsync ()
		{
			if (IsLoaded) {
				return null;
			}

			if (IsLoading) {
				return _loadRequest;
			}

			_loadRequest = CreateLoadRequest ();
			_loadRequest.OnCompleted += OnLoadRequestCompleted;

			return _loadRequest;
		}

		public void Unload ()
		{
			// in loading
			if (IsLoading) {
				_loadRequest.Abort ();
				_loadRequest = null;
				return;
			}

			if (!IsLoaded) {
				return;
			}

			if (UnloadProcess ()) {
				IsLoaded = false;
			}
		}

		protected abstract bool LoadProcess ();

		protected abstract bool UnloadProcess ();

		#endregion


		#region Request

		private TRequest _loadRequest;

		private void OnLoadRequestCompleted (LoadRequest loadRequest)
		{
			if (_loadRequest == null || _loadRequest != loadRequest) {
				return;
			}

			IsLoaded = _loadRequest.IsDone && string.IsNullOrEmpty (_loadRequest.Error);
			OnLoadRequestReceived (_loadRequest);
			_loadRequest = null;
		}

		protected abstract TRequest CreateLoadRequest ();

		protected abstract void OnLoadRequestReceived (TRequest loadRequest);

		#endregion
	}
}