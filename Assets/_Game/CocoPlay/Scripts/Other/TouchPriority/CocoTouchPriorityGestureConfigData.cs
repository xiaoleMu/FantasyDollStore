
using UnityEngine;

namespace CocoPlay
{
	public abstract class CocoTouchPriorityConfigData
	{
		protected CocoTouchPriorityConfigData (CocoGestureType gestureType)
		{
			GestureType = gestureType;
		}

		public CocoGestureType GestureType { get; private set; }

		public CocoGestureType[] LowPriorityGestureTypes { get; set; }

		public CocoGestureType HighPriorityGestureTypes { get; set; }

		public abstract void ProcessGesture (object gesture);

		public abstract void CancelGestureProcess ();
	}

	public class CocoTouchPriorityGestureConfigData<T> : CocoTouchPriorityConfigData where T : Gesture, new()
	{
		public CocoTouchPriorityGestureConfigData (CocoGestureType gestureType) : base (gestureType)
		{
		}

		public System.Action<T> OnGesture { get; set; }

		#region implemented abstract members of CocoTouchPriorityGestureConfigData

		public override void ProcessGesture (object gesture)
		{
			T g = gesture as T;
			if (g != null) {
				OnGesture (g);
			}
		}

		public override void CancelGestureProcess ()
		{
			if (!typeof(T).IsSubclassOf (typeof(ContinuousGesture))) {
				return;
			}

			T gesture = new T ();
			ContinuousGesture cg = gesture as ContinuousGesture;
			if (cg == null) {
				return;
			}
			
			cg.State = GestureRecognitionState.Ended;
			ProcessGesture (gesture);
		}

		#endregion
	}
	
	public class CocoTouchPriorityFingerEventConfigData<T> : CocoTouchPriorityConfigData where T : FingerEvent, new()
	{
		public CocoTouchPriorityFingerEventConfigData (CocoGestureType gestureType) : base (gestureType)
		{
		}

		public System.Action<T> OnGesture { get; set; }

		#region implemented abstract members of CocoTouchPriorityGestureConfigData

		public override void ProcessGesture (object gesture)
		{
			T g = gesture as T;
			if (g != null) {
				OnGesture (g);
			}
		}

		public override void CancelGestureProcess ()
		{
		}

		#endregion
	}
}
