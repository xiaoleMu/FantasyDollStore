using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public class SocialNetworkData : ICloneable
	{
		public SocialNetwork type;
		public string id;

		#region ICloneable implementation

		public object Clone ()
		{
			SocialNetworkData clone = new SocialNetworkData();
			clone.type = this.type;
			clone.id = this.id;
			return clone;
		}

		#endregion
	}
}
