using UnityEngine;
using CocoStoreID = Game.CocoStoreID;

namespace CocoPlay.Store
{
	public class StoreItemStateReceiverSample : IStoreItemStateReceiver
	{
		public StoreItemStateReceiverSample (CocoStoreID storeId)
		{
			ListeningId = storeId;
		}

		public CocoStoreID ListeningId { get; private set; }

		public void ReceiveStateChange (StoreItemState fromState, StoreItemState toState)
		{
			Debug.LogErrorFormat ("change state [{0}]: [{1}] -> [{2}]", ListeningId, fromState, toState);
		}
	}
}