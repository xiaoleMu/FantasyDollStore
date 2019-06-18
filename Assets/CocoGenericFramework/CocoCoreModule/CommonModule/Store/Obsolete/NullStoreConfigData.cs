using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if COCO_FAKE
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
#else
using CocoStoreID = Game.CocoStoreID;
#endif

namespace CocoPlay.Fake
{
	public class NullStoreConfigData
	{
		public NullStoreConfigData ()
		{
		}

		public string GetStoreKey (CocoStoreID ID)
		{
			return string.Empty;
		}

		public List<string> GetStoreKeyList ()
		{
			return new List<string> ();
		}

		public Dictionary<CocoStoreID, List<CocoStoreID>> GetSlot ()
		{
			return new Dictionary<CocoStoreID, List<CocoStoreID>> ();
		}
	}
}

