using System.Collections.Generic;
using CocoStoreID = Game.CocoStoreID;

namespace CocoPlay
{
	public interface ICocoStoreConfigData
	{
		#if ABTEST
		string GetStoreKey (CocoStoreID ID);
		string GetStoreKey_TypeA (CocoStoreID ID);
		string GetStoreKey_TypeB (CocoStoreID ID);
		List<string> GetStoreKeyList_TypeA ();
		List<string> GetStoreKeyList_TypeB ();
		List<string> GetStoreKeyList ();
		Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot_TypeA ();
		Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot_TypeB ();
		Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot ();

		Dictionary<CocoStoreID, List<CocoStoreID>> GetABVersionSlot ();
		#else
		string GetStoreKey (CocoStoreID id);
		List<string> GetStoreKeyList ();
		Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot ();
		#endif

		List<CocoStoreItemData> ItemDatas { get; }
		CocoStoreItemData GetItemData (CocoStoreID itemId);
		CocoStoreItemData GetItemData (string itemKey);

		IEnumerable<CocoStoreID> ItemIds { get; }
	}
}
