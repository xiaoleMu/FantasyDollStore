using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public class AppModalView : MonoBehaviour
	{
		public event Action CloseCompleteEvent = () => {};
		public event Action OpenCompleteEvent = () => {};
		public event Action OpenEvent = () => {};

				
		protected enum States
		{
			Closed,
			Opening,
			Open,
			Closing
		}
		protected States _state = States.Closed;
		bool _isInBackground = false;
	
		public void Open ()
		{
			if(_state == States.Opening || _state == States.Open)
				return;

			gameObject.SetActive (true);
			State = States.Opening;
			OpenEvent();
			OnOpen ();
		}

		States State {
			get {
				return _state;
			}
			set {
				_state = value;
			}
		}
	
		protected virtual void OnOpen ()
		{
			OnOpenComplete ();
		}
	
		public virtual void OnOpenComplete ()
		{
			State = States.Open;
			OpenCompleteEvent ();
		}

		public void Close ()
		{
			if (_state == States.Closing){
				return;
			}

			if(_state == States.Opening){
				OnCloseComplete ();
				return;
			}

			State = States.Closing;
			if (_isInBackground)
				OnCloseComplete ();
			else
				OnClose ();
		}
	
		protected virtual void OnClose ()
		{
			OnCloseComplete ();
		}
	
		public virtual void OnCloseComplete ()
		{
			State = States.Closed;
			gameObject.SetActive (false);
			CloseCompleteEvent ();
		}
	
		public virtual void RetunToForeground ()
		{
			if (_state == States.Closing){
				return;
			}

			_isInBackground = false;
			gameObject.SetActive (true);
		}

		public virtual void MoveToBackground ()
		{
			if (_state == States.Closing){
				OnCloseComplete();
				return;
			}
			_isInBackground = true;
			gameObject.SetActive (false);
		}

		public bool IsOpen {
			get {
				return _state != States.Closed;
			}
		}
	}
}
