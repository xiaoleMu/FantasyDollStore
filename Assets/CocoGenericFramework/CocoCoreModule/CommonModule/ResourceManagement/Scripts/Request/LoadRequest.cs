using System.Collections;

namespace CocoPlay.ResourceManagement
{
	public abstract class LoadRequest : IEnumerator
	{
		public object Current {
			get { return null; }
		}

		public void Reset ()
		{
		}

		public bool MoveNext ()
		{
			if (!LoadTick ()) {
				return true;
			}

			IsDone = true;

			var result = LoadFinish ();
			if (!result && string.IsNullOrEmpty (Error)) {
				Error = "load finish [failed].";
			}

			ResourceDebug.Log ("{0}->MoveNext: load finish: error [{1}]", GetType ().Name, Error);

			if (_onCompleted != null) {
				_onCompleted (this);
			}

			return false;
		}


		#region State

		public bool IsDone { get; private set; }

		protected abstract float RequestProgress { get; }

		public float Progress {
			get { return IsDone ? 1f : RequestProgress; }
		}

		public string Error { get; protected set; }

		public bool HasError {
			get { return !string.IsNullOrEmpty (Error); }
		}

		#endregion


		#region Load

		private System.Action<LoadRequest> _onCompleted;

		public event System.Action<LoadRequest> OnCompleted {
			add {
				if (IsDone) {
					value (this);
					return;
				}

				_onCompleted += value;
			}
			// ReSharper disable once DelegateSubtraction
			remove { _onCompleted -= value; }
		}

		protected abstract bool LoadTick ();

		protected abstract bool LoadFinish ();

		public abstract void Abort ();

		#endregion
	}
}