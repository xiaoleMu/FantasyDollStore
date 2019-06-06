using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	[Serializable]
	public struct NewSocialPlayerData 
	{
		public string id;
		public string name;
		public int level;
		public SocialNetwork socialType;
	}
}