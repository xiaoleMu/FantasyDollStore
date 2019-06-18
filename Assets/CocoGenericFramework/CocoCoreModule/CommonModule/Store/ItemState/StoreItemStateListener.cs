using System.Collections;
using TabTale;
using UnityEngine;

namespace CocoPlay.Store
{
	public class StoreItemStateListener : GameView
	{
		protected override void Start ()
		{
			base.Start ();

			StartCoroutine (WaitForStartListening ());
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			StoreUpdateStateSignal.AddListener (OnStoreUpdateState);
		}

		protected override void RemoveListeners ()
		{
			StoreUpdateStateSignal.RemoveListener (OnStoreUpdateState);

			base.RemoveListeners ();
		}


		#region Listen

		private IStoreItemStateReceiver _receiver;

		public IStoreItemStateReceiver Receiver {
			get { return _receiver; }
			set { _receiver = value; }
		}

		private bool _isListening;

		private bool CanStartListening ()
		{
			return _receiver != null && StoreControl.IsInitialised;
		}

		private IEnumerator WaitForStartListening ()
		{
			yield return new WaitUntil (CanStartListening);

			_isListening = true;
			RefreshState ();
		}

		#endregion


		#region State

		[Inject]
		public CocoStoreUpdateStateSignal StoreUpdateStateSignal { get; set; }

		[Inject]
		public CocoStoreControl StoreControl { get; set; }

		[Inject]
		public CocoGlobalRecordModel GlobalRecordModel { get; set; }

		private const string STATE_KEY_PREFIX = "store_item_state_";

		private string STATE_KEY {
			get { return STATE_KEY_PREFIX + _receiver.ListeningId; }
		}

		private bool _isStateInited;
		private StoreItemState _itemState = StoreItemState.Unauthorized;

		private void OnStoreUpdateState ()
		{
			RefreshState ();
		}

		private void InitState ()
		{
			if (_isStateInited) {
				return;
			}

			_itemState = (StoreItemState)GlobalRecordModel.GetInt (STATE_KEY);
			_isStateInited = true;
		}

		private void UpdateState (StoreItemState state)
		{
			if (_itemState == state) {
				return;
			}

			_receiver.ReceiveStateChange (_itemState, state);

			_itemState = state;
			GlobalRecordModel.SetData (STATE_KEY, (int)_itemState);
		}

		private void RefreshState ()
		{
			if (!_isListening) {
				return;
			}

			InitState ();

			var purchased = StoreControl.IsPurchased (_receiver.ListeningId);
			switch (_itemState) {
			case StoreItemState.Authorized:
				if (!purchased) {
					UpdateState (StoreItemState.Revoked);
				}
				break;

			default:
				if (purchased) {
					UpdateState (StoreItemState.Authorized);
				}
				break;
			}
		}

		#endregion
	}
}