using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class AnimatorModalView : AppModalView
	{

		Animator _animator;

		string _openTrigger = "open";
		string _closeTrigger = "close";

		void Awake ()
		{
			_animator = GetComponent<Animator> ();
		}

		void OnEnable(){
			if(_state == States.Opening){
				OnOpen ();
			}
		}

		public void Open(string trigger){
			_openTrigger = trigger;
			base.Open();
		}

		public void Close(string trigger){
			_closeTrigger = trigger;
			base.Close();
		}

		protected override void OnOpen ()
		{
			_animator.SetTrigger (_openTrigger);
		}
	
		protected override void OnClose ()
		{
			_animator.SetTrigger (_closeTrigger);
		}

		public void OnOpenAnimationComplete(){
			base.OnOpenComplete();
		}

		public void OnCloseAnimationComplete(){
			base.OnCloseComplete();
		}
	
	}
}
