using System;

namespace CocoPlay.ResourceManagement
{
	public abstract class LocationLoadRequest : LoadRequest
	{
		protected LocationLoadRequest (LocationHolder holder)
		{
			_holder = holder;
		}

		private readonly LocationHolder _holder;

		public LocationHolder Holder {
			get { return _holder; }
		}
	}


	public class LocationDefaultLoadRequest : LocationLoadRequest
	{
		public LocationDefaultLoadRequest (LocationHolder holder) : base (holder)
		{
		}

		protected override float RequestProgress {
			get { return 1f; }
		}

		protected override bool LoadTick ()
		{
			return true;
		}

		protected override bool LoadFinish ()
		{
			return true;
		}

		public override void Abort ()
		{
		}
	}


	public class LocationODRLoadRequest : LocationLoadRequest, IDisposable
	{
		public LocationODRLoadRequest (LocationHolder holder, string odrTag) : base (holder)
		{
			_odrTag = odrTag;
			_odrRequest = ResourceODRLocation.PreloadAsync (new[] { odrTag });
		}


		private readonly string _odrTag;
		private readonly ResourceODRRequest _odrRequest;

		protected override float RequestProgress {
			get { return _odrRequest.Progress; }
		}

		protected override bool LoadTick ()
		{
			return !_odrRequest.MoveNext ();
		}

		protected override bool LoadFinish ()
		{
			ResourceDebug.Log ("{0}->LoadFinish: odr {1}", GetType ().Name, _odrTag);

			if (string.IsNullOrEmpty (_odrRequest.Error)) {
				return true;
			}

			Error = _odrRequest.Error;
			return false;
		}

		public override void Abort ()
		{
			_odrRequest.Dispose ();
		}

		public void Dispose ()
		{
			ResourceDebug.Log ("{0}->Dispose: odr {1}", GetType ().Name, _odrTag);
			_odrRequest.Dispose ();
		}
	}
}