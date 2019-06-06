using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;
using System;

namespace TabTale
{
	public interface ITouchable  {
		
		void OnTouchBegin(Vector2 pos);
		void OnTouchMove(Vector2 pos);
		void OnTouchEnd(Vector2 pos);
	}
	
	public class GestureController : View
	{
		public event Action OnSwipeEvent = () => {};

		public float dragThreshold = 0.05f;

		Vector3 _firstTouchPos;
		Vector3 _lastTouchPosition = Vector3.zero;

		bool _isDrag = false;
		bool _isMouseDown = false;

		private List<Vector3> touchPositions = new List<Vector3>(); //store all the touch positions in list

		protected override void Start ()
		{
			base.Start ();

			Reset ();
		}

		void Reset ()
		{
			_isDrag = false;
			_isMouseDown = false;
		}

		void Update ()
		{
			UpdateInput ();
			//UpdateSwipeMovement ();
		}

		void UpdateInput ()
		{
			if (TouchUtils.IsTouchingUGUI ()) 
			{
				/*
				if(!_isDrag) // if already in drag mode, complete the action, otherwise, ignore the touch/mouse input
				{
					_isMouseDown = false;
					_isDrag = false;
					return;
				}
				*/
			}

			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
				UpdateTouches ();
			else
				UpdateMouse ();
				
		}

		/*
		void UpdateSwipeMovement ()
		{
			if (Mathf.Abs (_swipeSpeedX) > MIN_SWIPE_SPEED || Mathf.Abs (_swipeSpeedY) > MIN_SWIPE_SPEED) {
				if (Time.deltaTime == 0) {
					_swipeSpeedX = 0;
					_swipeSpeedY = 0;
					return;
				}
				
				TranslateCamera (_swipeSpeedX * Time.deltaTime, _swipeSpeedY * Time.deltaTime);
				_swipeSpeedX *= SWIPE_DRAG_FACTOR;
				_swipeSpeedY *= SWIPE_DRAG_FACTOR;
				
			} else {
				if (_swipeSpeedX > 0 || _swipeSpeedY > 0){
					cameraStopSignal.Dispatch(movementData.contentCamera);
				}
				
				_swipeSpeedX = 0;
				_swipeSpeedY = 0;
			}
		}
		*/

		void UpdateTouches ()
		{
			foreach (Touch touch in Input.touches)
			{
				if(touch.phase == TouchPhase.Ended)
				{
					_firstTouchPos = touchPositions[0];
					_lastTouchPosition = touchPositions[touchPositions.Count - 1];

					Debug.Log ("Last touch pos: " + _lastTouchPosition + " , first touch pos: " + _firstTouchPos);
					if (Mathf.Abs(_lastTouchPosition.x - _firstTouchPos.x) > dragThreshold || 
					    Mathf.Abs(_lastTouchPosition.y - _firstTouchPos.y) > dragThreshold)
					{
						Debug.Log ("Gesture Controller : Swipe Detected");
						OnSwipeDetected();
					}
				}
			}

		}

		void UpdateMouse ()
		{
			if (_isMouseDown) 
			{
				if (Input.GetMouseButtonUp (0)) 
				{
					_isMouseDown = false;
					OnTouchEnd (Input.mousePosition);
					return;
				} 
				else 
				{
					OnTouchMove (Input.mousePosition);
				}

			} 
			else 
			{
				if (Input.GetMouseButtonDown (0)) {
					_isMouseDown = true;
					OnTouchBegin (Input.mousePosition);
				}
			}


		}

		#region drag

		void OnBeginDrag ()
		{
			Debug.Log ("GestureController.OnBeginDrag");
			OnSwipeDetected();
		}

		void OnDrag ()
		{

		}

		void OnEndDrag ()
		{

		}

		#endregion

		#region swipe

		void OnSwipeDetected()
		{
			Debug.Log ("Swipe detected");
			OnSwipeEvent();

        }
        
        #endregion

		#region ITouchable implementation

		void OnTouchBegin (Vector3 pos)
		{
			_firstTouchPos = pos;
		}

		void OnTouchMove (Vector3 pos)
		{
			if (!_isDrag) {
				if (Vector3.Distance (_firstTouchPos, pos) > dragThreshold) {
					_isDrag = true;
					OnBeginDrag ();
				}
				
			} else {
				OnDrag ();
			}
		}

		void OnTouchEnd (Vector3 pos)
		{
			if (_isDrag) {
				_isDrag = false;
				OnEndDrag ();
			}
		}

		#endregion

	}



}