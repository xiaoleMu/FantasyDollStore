using TabTale;
using UnityEngine;

namespace CocoPlay
{
	public interface IBackButtonOwner
	{
		bool IsShowed { get; }
		bool IsBackButtonHeld { get; }
		void TriggerCloseButton ();
	}


	public class BackButtonListenHelper : GameView, IBackButtonListener
	{
		protected override void AddListeners ()
		{
			base.AddListeners ();
			RegisterListener (ListenStartPoint.OnRegister);
		}

		protected override void RemoveListeners ()
		{
			UnRegisterListener ();
			base.RemoveListeners ();
		}

		protected override void Start ()
		{
			base.Start ();
			RegisterListener (ListenStartPoint.OnStart);

			StartCoroutine (CocoWait.WaitForFrame (1, () => RegisterListener (ListenStartPoint.AfterStart)));
		}


		#region Listen Mode

		private enum ListenStartPoint
		{
			OnRegister,
			OnStart,
			AfterStart
		}


		[SerializeField]
		private ListenStartPoint _listenStartPoint = ListenStartPoint.OnRegister;

		private bool _isListening;

		private void RegisterListener (ListenStartPoint startPoint)
		{
			if (_isListening || startPoint != _listenStartPoint) {
				return;
			}

			_isListening = true;
			SubscribeToBackButtonEvent ();
		}

		private void UnRegisterListener ()
		{
			if (!_isListening) {
				return;
			}

			UnSubscribeFromBackButtonEvent ();
			_isListening = false;
		}

		#endregion


		#region Owner

		private IBackButtonOwner _owner;

		private IBackButtonOwner Owner {
			get { return _owner ?? (_owner = GetComponent<IBackButtonOwner> ()); }
		}

		private bool IsBackButtonHeld {
			get { return Owner != null && Owner.IsBackButtonHeld; }
		}

		private void TriggerCloseButton ()
		{
			if (Owner == null || !Owner.IsShowed) {
				return;
			}

			Owner.TriggerCloseButton ();
		}

		#endregion


		#region IBackButtonListener Implemention

		[Inject]
		public IBackButtonService BackButtonService { get; set; }

		public void SubscribeToBackButtonEvent ()
		{
			BackButtonService.AddListener (this);
		}

		public void UnSubscribeFromBackButtonEvent ()
		{
			BackButtonService.RemoveListener (this);
		}

		public bool HandleBackButtonPress ()
		{
			if (!IsBackButtonHeld) {
				return false;
			}

			TriggerCloseButton ();
			return true;
		}

		#endregion
	}
}