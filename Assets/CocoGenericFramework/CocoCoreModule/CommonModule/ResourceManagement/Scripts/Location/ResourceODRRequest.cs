using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;

#endif

namespace CocoPlay.ResourceManagement
{
	public class ResourceODRRequest : LoadRequest, IDisposable
	{
		public ResourceODRRequest (string[] tags, bool isVirtual)
		{
#if UNITY_IOS
			if (!isVirtual) {
				_odrRequest = OnDemandResources.PreloadAsync (tags);
				return;
			}

			_virtualStartTime = Time.time;
			_virtualDownloadTime = CocoDebugSettingsData.Instance.VirtualODRDownloadTime * tags.Length;
#else
			var tag = tags.Length > 0 ? tags [0] : string.Empty;
			Error = "this platform NOT be supported.";
			Debug.LogErrorFormat ("{0}->New: tags [{1}], isVirtual [{2}], error [{3}]", GetType ().Name, tag, isVirtual, Error);
#endif
		}


#if UNITY_IOS
		private readonly float _virtualStartTime;
		private readonly float _virtualDownloadTime;

		private readonly OnDemandResourcesRequest _odrRequest;

		protected override float RequestProgress {
			get {
				if (_odrRequest != null) {
					return _odrRequest.progress;
				}

				return _virtualDownloadTime > 0 ? Mathf.Clamp01 ((Time.time - _virtualStartTime) / _virtualDownloadTime) : 1f;
			}
		}

		protected override bool LoadTick ()
		{
			if (_odrRequest != null) {
				return _odrRequest.isDone;
			}

			return Time.time >= _virtualStartTime + _virtualDownloadTime;
		}

		protected override bool LoadFinish ()
		{
			if (_odrRequest != null) {
				Error = _odrRequest.error;
			}

			return true;
		}

		public override void Abort ()
		{
		}

		public void Dispose ()
		{
			if (_odrRequest != null) {
				_odrRequest.Dispose ();
			}
		}
#else
		protected override float RequestProgress {
			get { return 1f; }
		}

		protected override bool LoadTick ()
		{
			return true;
		}

		protected override bool LoadFinish ()
		{
			return false;
		}

		public override void Abort ()
		{
		}

		public void Dispose ()
		{
		}

#endif
	}
}