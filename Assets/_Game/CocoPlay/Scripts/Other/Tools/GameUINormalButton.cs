using UnityEngine;
using System.Collections;
using CocoPlay;
using UnityEngine.Events;

namespace Game
{
	public class GameUINormalButton : CocoUINormalButton
	{
		public bool handleEvents = false;
		public UnityEvent targetEvent;
        public System.Action OnClickEvent;

		protected override void OnClick ()
		{
			base.OnClick ();
			if (handleEvents) {
				targetEvent.Invoke ();
			}
            if (OnClickEvent != null)
            {
                OnClickEvent();
            }
		}
	}
}

