using System;
using System.Collections.Generic;

namespace CocoPlay.ResourceManagement
{
	public class GroupLoadRequest<T> : LoadRequest
	{
		public GroupLoadRequest (List<T> providers, Func<T, LoadRequest> provideFunc)
		{
			_providers = providers;
			_provideFunc = provideFunc;

			_currProviderIndex = -1;
			_currRequest = ProvideNextRequest ();
		}

		private readonly List<T> _providers;
		private readonly Func<T, LoadRequest> _provideFunc;
		private int _currProviderIndex;
		private LoadRequest _currRequest;


		private LoadRequest ProvideNextRequest ()
		{
			_currProviderIndex++;
			for (; _currProviderIndex < _providers.Count; _currProviderIndex++) {
				var request = _provideFunc (_providers [_currProviderIndex]);
				if (request != null) {
					return request;
				}

				ResourceDebug.Log ("{0}-ProvideNextRequest: get null request from provider [{1}], maybe already loaded, skipped ...",
					GetType ().Name, _currProviderIndex);
			}

			ResourceDebug.Log ("{0}-ProvideNextRequest: all requests be provided.", GetType ().Name);
			return null;
		}

		protected override float RequestProgress {
			get {
				if (_currRequest == null) {
					return 1f;
				}

				return (_currProviderIndex + _currRequest.Progress) / _providers.Count;
			}
		}

		protected override bool LoadTick ()
		{
			if (_currRequest == null) {
				return true;
			}

			if (_currRequest.MoveNext ()) {
				ResourceDebug.Log ("{0}->LoadTick: index [{1}] - progress [{2}]", GetType ().Name, _currProviderIndex, _currRequest.Progress);
				return false;
			}

			ResourceDebug.Log ("{0}->LoadTick: finish index [{1}]: - progress [{2}], error [{3}]", GetType ().Name, _currProviderIndex,
				_currRequest.Progress, _currRequest.Error);

			// end current
			if (!string.IsNullOrEmpty (_currRequest.Error)) {
				Error = string.Format ("{0}{1}: error [{2}].\n", Error, _currProviderIndex, _currRequest.Error);
			}

			// move next
			_currRequest = ProvideNextRequest ();

			return false;
		}

		protected override bool LoadFinish ()
		{
			ResourceDebug.Log ("{0}-LoadFinish: error [{1}]", GetType ().Name, Error);
			return true;
		}

		public override void Abort ()
		{
			if (_currRequest == null) {
				return;
			}

			_currRequest.Abort ();
			_currRequest = null;
		}
	}
}