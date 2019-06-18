using Game;

namespace CocoPlay.Store
{
	public interface IStoreItemStateReceiver
	{
		CocoStoreID ListeningId { get; }

		void ReceiveStateChange (StoreItemState fromState, StoreItemState toState);
	}
}