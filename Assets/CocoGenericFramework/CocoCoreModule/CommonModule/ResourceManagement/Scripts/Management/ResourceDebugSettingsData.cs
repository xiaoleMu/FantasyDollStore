using UnityEngine;

namespace CocoPlay
{
	public partial class CocoDebugSettingsData
	{
		[Header ("Resource Management")]
		[SerializeField]
		private bool _isResourceLogEnabled;

		public bool IsResourceLogEnabled {
			get { return IsGlobalEnabled && _isResourceLogEnabled; }
			set {
				_isResourceLogEnabled = value;
				Save ();
			}
		}

		[SerializeField]
		private float _virtualODRDownloadTime = 1f;

		public float VirtualODRDownloadTime {
			get { return _virtualODRDownloadTime; }
			set {
				_virtualODRDownloadTime = value;
				Save ();
			}
		}
	}
}