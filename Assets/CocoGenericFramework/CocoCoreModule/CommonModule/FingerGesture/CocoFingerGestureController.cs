﻿using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using TabTale;

namespace CocoPlay
{
	public class CocoFingerGestureController : GameView
	{

		private TapRecognizer mTapRecognizer;
		private DragRecognizer mDragRecognizer;
		private DragRecognizer mTwoFingerDragRecognizer;
		private PinchRecognizer mPinchRecognizer;
		private SwipeRecognizer mSwipeRecognizer;
		private LongPressRecognizer mPressRecognizer;
		private TwistRecognizer mTwistRecognizer;
		private FingerDownDetector mFingeDown;
		private FingerUpDetector mFingeUp;

		[Inject]
		public TapGestureSignal tapGestureSignal { get; set; }
		[Inject]
		public DragGestureSignal dragGestureSignal { get; set; }
		[Inject]
		public PinchGestureSignal pinchGestureSignal { get; set; }
		[Inject]
		public SwipeGestureSignal swipeGestureSignal { get; set; }
		[Inject]
		public TwoFingerDragGestureSignal twoFingerDragGestureSignal { get; set; }
		[Inject]
		public TwistGestureSignal twistGestureSignal { get; set; }
		[Inject]
		public DownEventSignal m_DownSingal { get; set; }
		[Inject]
		public UpEventSignal m_UpSingal { get; set; }
		[Inject]
		public EnableFingerGestureSignal enableGestureController { get; set; }
		[Inject]
		public IModalityManager modalityManager { get; set; }

		private bool m_EnableGestures = true;
        [HideInInspector]
        public bool ForceEnable = false;

		private bool EnableGesture
		{
			get
			{
                if (ForceEnable)
                    return true;
                
				if (!m_EnableGestures)
				{
					return false;
				}
				
				if (modalityManager.IsModalPresent)
				{
					return false;
				}

				if (!CocoMainController.Instance.TouchEnable)
				{
					return false;
				}

				return true;
			}
		}

		protected override void OnRegister ()
		{
			base.OnRegister ();

			enableGestureController.AddListener (EnableGestureSignalEvent);

			mTapRecognizer = gameObject.AddComponent<TapRecognizer> ();
			mTapRecognizer.UseSendMessage = false;
			mTapRecognizer.IsExclusive = true;

			mSwipeRecognizer = gameObject.AddComponent<SwipeRecognizer> ();
			mSwipeRecognizer.UseSendMessage = false;
			mSwipeRecognizer.IsExclusive = true;

			mDragRecognizer = gameObject.AddComponent<DragRecognizer> ();
			mDragRecognizer.UseSendMessage = false;
			mDragRecognizer.IsExclusive = true;

			mTwoFingerDragRecognizer = gameObject.AddComponent<DragRecognizer> ();
			mTwoFingerDragRecognizer.RequiredFingerCount = 2;
			mTwoFingerDragRecognizer.UseSendMessage = false;
			mTwoFingerDragRecognizer.IsExclusive = true;

			mPinchRecognizer = gameObject.AddComponent<PinchRecognizer> ();
			mPinchRecognizer.UseSendMessage = false;
			mPinchRecognizer.IsExclusive = true;
			
			mTwistRecognizer = gameObject.AddComponent<TwistRecognizer> ();
			mTwistRecognizer.UseSendMessage = false;
			mTwistRecognizer.IsExclusive = true;

//			mPressRecognizer = gameObject.AddComponent<LongPressRecognizer> ();
//			mPressRecognizer.UseSendMessage = false;
//			mPressRecognizer.IsExclusive = true;

			mFingeDown = gameObject.AddComponent<FingerDownDetector> ();
			mFingeUp = gameObject.AddComponent<FingerUpDetector> ();

			mTapRecognizer.OnGesture += OnSceneTap;
			mDragRecognizer.OnGesture += OnSceneDrag;
			mPinchRecognizer.OnGesture += OnScenePinch;
			mSwipeRecognizer.OnGesture += OnSceneSwipe;
			mTwoFingerDragRecognizer.OnGesture += OnSceneTwoFingerDrag;
			mTwistRecognizer.OnGesture += OnSceneTwist;
			mFingeDown.OnFingerDown += OnSceneDown;
			mFingeUp.OnFingerUp += OnSceneUp;
		}

		protected override void OnUnRegister ()
		{
			enableGestureController.RemoveListener (EnableGestureSignalEvent);

			mTapRecognizer.OnGesture -= OnSceneTap;
			mDragRecognizer.OnGesture -= OnSceneDrag;
			mPinchRecognizer.OnGesture -= OnScenePinch;
			mSwipeRecognizer.OnGesture -= OnSceneSwipe;
			mTwoFingerDragRecognizer.OnGesture -= OnSceneTwoFingerDrag;
			mTwistRecognizer.OnGesture -= OnSceneTwist;
			mFingeDown.OnFingerDown -= OnSceneDown;
			mFingeUp.OnFingerUp -= OnSceneUp;
			base.OnUnRegister ();
		}
			
		private void EnableGestureSignalEvent (bool pEnable)
		{
			m_EnableGestures = pEnable;
		}

		private void OnSceneTap (TapGesture tapGesture)
		{
			if (!EnableGesture)
			{
				return;
			}

			//Debug.LogErrorFormat ("{0}->OnSceneTap: pos: {1}", GetType ().Name, tapGesture.Position);

			tapGestureSignal.Dispatch (tapGesture);
		}

		private void OnSceneDrag (DragGesture dragGesture)
		{
			if (!EnableGesture)
			{
				return;
			}

			//Debug.LogErrorFormat ("{0}->OnSceneDrag: pos: {1}, delta: {2}, phase: {3}", GetType ().Name, dragGesture.Position, dragGesture.DeltaMove, dragGesture.Phase);

			dragGestureSignal.Dispatch (dragGesture);
		}

		private void OnScenePinch (PinchGesture pinchGesture)
		{
			if (!EnableGesture)
			{
				return;
			}

			//Debug.LogErrorFormat ("{0}->OnScenePinch: pos: {1}, delta: {2}, phase: {3}", GetType ().Name, pinchGesture.Position, pinchGesture.Delta, pinchGesture.Phase);

			pinchGestureSignal.Dispatch (pinchGesture);
		}

		private void OnSceneSwipe (SwipeGesture swipeGesture)
		{
			if (!EnableGesture)
			{
				return;
			}

			//Debug.LogErrorFormat ("{0}->OnSceneSwipe: pos: {1}, direction: {2}, velocity: {3}", GetType ().Name, swipeGesture.Position, swipeGesture.Direction, swipeGesture.Velocity);

			swipeGestureSignal.Dispatch (swipeGesture);
		}

		private void OnSceneTwoFingerDrag (DragGesture dragGesture)
		{
			if (!EnableGesture)
			{
				return;
			}

			//Debug.LogErrorFormat ("{0}->OnSceneTwoFingerDrag: pos: {1}, delta: {2}, phase: {3}", GetType ().Name, dragGesture.Position, dragGesture.DeltaMove, dragGesture.Phase);

			twoFingerDragGestureSignal.Dispatch (dragGesture);
		}
		
		private void OnSceneTwist (TwistGesture twistGesture)
		{
			if (!EnableGesture)
			{
				return;
			}

			//Debug.LogErrorFormat ("{0}->OnSceneSwipe: pos: {1}, direction: {2}, velocity: {3}", GetType ().Name, swipeGesture.Position, swipeGesture.Direction, swipeGesture.Velocity);

			twistGestureSignal.Dispatch (twistGesture);
		}

		private void OnSceneDown (FingerDownEvent downEvent)
		{
			if (!EnableGesture)
				return;

			//Debug.LogErrorFormat ("{0}->OnSceneDown: pos: {1}, finger: {2}", GetType ().Name, downEvent.Position, downEvent.Finger.Index);

			m_DownSingal.Dispatch (downEvent);
		}

		private void OnSceneUp(FingerUpEvent upEvent)
		{
			if (!EnableGesture)
				return;

			//Debug.LogErrorFormat ("{0}->OnSceneUp: pos: {1}, finger: {2}", GetType ().Name, upEvent.Position, upEvent.Finger.Index);

			m_UpSingal.Dispatch (upEvent);
		}
	}

	public class TapGestureSignal : Signal <TapGesture>{}

	public class DragGestureSignal : Signal <DragGesture>{}

	public class PinchGestureSignal : Signal <PinchGesture>{}

	public class SwipeGestureSignal : Signal <SwipeGesture>{}

	public class TwoFingerDragGestureSignal : Signal <DragGesture>{}

	public class TwistGestureSignal : Signal <TwistGesture>{}

	public class DownEventSignal : Signal <FingerDownEvent>{}

	public class UpEventSignal : Signal <FingerUpEvent>{}

	public class EnableFingerGestureSignal : Signal <bool>{}
}

