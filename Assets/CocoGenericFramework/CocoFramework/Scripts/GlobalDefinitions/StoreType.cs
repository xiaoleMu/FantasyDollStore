using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale 
{
	public enum StoreType
	{
		All,
		Amazon,
		Google,
		iOS
	}
		
	static class StoreInfo
	{
		public static StoreType CurrentStoreType
		{			
			get 
			{ 
				#if AMAZON
				StoreType androidStoreType = StoreType.Amazon;
				#else
				StoreType androidStoreType = StoreType.Google;

				#endif

				return (Application.platform == RuntimePlatform.IPhonePlayer) ? StoreType.iOS : androidStoreType;
			}
		}
	}
}