using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale 
{
	public sealed class InAppPurchase
	{
		public static readonly InAppPurchase NoAds;
		
		private readonly string _name;
		
		private static IDictionary<string, InAppPurchase> s_nameToEvent = new Dictionary<string, InAppPurchase>();
		
		static InAppPurchase()
		{
			NoAds = NameToEvent("NoAds");	
		}
		
		private InAppPurchase(string name)
		{
			this._name = name;
		}
		
		public override string ToString()
		{
			return _name;
		}		
		
		public string Name
		{
			get { return _name; }
		}
		
		public static ICollection<InAppPurchase> Events
		{
			get { return s_nameToEvent.Values; }
		}
		
		public static InAppPurchase NameToEvent(string name)
		{
			InAppPurchase inAppPurchase;
			if(s_nameToEvent.TryGetValue(name, out inAppPurchase))
				return inAppPurchase;
			
			inAppPurchase = new InAppPurchase(name);
			
			s_nameToEvent[name] = inAppPurchase;
			
			return inAppPurchase;
		}
	}
}