using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace TabTale
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(AppModalView))]
	public class GameModalView : MainView
	{	
		private static readonly string CLOSE_TRIGGER = "closePanel";
		private static readonly string OPEN_TRIGGER = "openPanel";

		AppModalView _appModalView;
		Animator _animator;

		protected override void Awake ()
		{
			base.Awake ();
			
			_appModalView = GetComponent<AppModalView>();
			_animator = GetComponent<Animator>();

			AddListeners();
		}

		protected override void OnRegister ()
		{
			base.OnRegister ();

		}
		
		protected override void AddListeners ()
		{
			_appModalView.CloseCompleteEvent += OnCloseComplete;
			_appModalView.OpenCompleteEvent += OnOpenComplete;
			//closeSignal.AddListener(ClosePhone);
		}

		protected override void RemoveListeners ()
		{
			_appModalView.CloseCompleteEvent -= OnCloseComplete;
			_appModalView.OpenCompleteEvent -= OnOpenComplete;
			//closeSignal.RemoveListener(ClosePhone);
		}

		public void Close()
		{
			_animator.SetTrigger(CLOSE_TRIGGER);
		}

		void OnCloseAnimationComplete ()
		{
			//closeCompleteSignal.Dispatch();
			_appModalView.Close();
		}

		void OnOpenComplete()
		{
			_animator.SetTrigger(OPEN_TRIGGER);

		}

		void OnCloseComplete()
		{
            
        }

		protected override void OnUnRegister ()
		{
			RemoveListeners();
			base.OnUnRegister ();
		}
	}

}