using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace TabTale
{
	[RequireComponent(typeof(AppModalView))]
	public class BaseModalView : GameView, IBackButtonListener
	{
		[Inject]
		public IBackButtonService backButtonService { get; set; }

		[Inject]
		public ModalClosedSignal modalClosedSignal { get; set; }

		[Inject]
		public ModalOpenedSignal modalOpenedSignal { get; set; }

		protected AppModalView _appModalView;

		protected override void Awake ()
		{
			_appModalView = GetComponent<AppModalView>();
			base.Awake ();
		}

        protected override void AddListeners ()
        {
			base.AddListeners ();
			_appModalView.OpenCompleteEvent += SubscribeToBackButtonEvent;
			_appModalView.OpenCompleteEvent += OnModalOpenComplete;
			_appModalView.CloseCompleteEvent += UnSubscribeFromBackButtonEvent;
			_appModalView.CloseCompleteEvent += OnModalCloseComplete;

        }

        protected override void RemoveListeners ()
        {
			base.RemoveListeners ();
			_appModalView.OpenCompleteEvent -= SubscribeToBackButtonEvent;
			_appModalView.CloseCompleteEvent -= UnSubscribeFromBackButtonEvent;
			_appModalView.OpenCompleteEvent -= OnModalOpenComplete;
			_appModalView.CloseCompleteEvent -= OnModalCloseComplete;
        }

		#region IBackButtonListener implementation
		
		public void SubscribeToBackButtonEvent ()
		{
			backButtonService.AddListener(this);
		}
		
		public void UnSubscribeFromBackButtonEvent ()
		{
			backButtonService.RemoveListener(this);
		}

		protected virtual void OnModalOpenComplete()
		{
			modalOpenedSignal.Dispatch(GetId());
		}

		protected virtual void OnModalCloseComplete()
		{
			modalClosedSignal.Dispatch(GetId());
		}

		protected virtual string GetId()
		{
			return gameObject.name;
		}
		
		public virtual bool HandleBackButtonPress ()
		{
			CloseModalView();
			return true;
		}
		
		#endregion

		protected void CloseModalView()
		{
			GetComponent<AppModalView>().Close();
		}
	}




}
