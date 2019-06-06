
namespace CocoPlay
{
	[System.Flags]
	public enum CocoGestureType
	{
		None = 0,

		// single
		Tap = 0x01,
		Drag = 0x02,
		Swipe = 0x04,

		// multi
		Pinch = 0x0100,
		TwoFingerDrag = 0x0200,
		Twist = 0x0400,

		// finger
		FingerDown = 0x010000,
		FingerUp = 0x020000,

		// all
		All_SingleTouch = Tap | Drag | Swipe,
		All_MultiTouch = Pinch | TwoFingerDrag | Twist,
		All_FingerEvent = FingerDown | FingerUp,
		All_Touch = All_SingleTouch | All_MultiTouch,
		All = All_Touch | All_FingerEvent
	}
}
