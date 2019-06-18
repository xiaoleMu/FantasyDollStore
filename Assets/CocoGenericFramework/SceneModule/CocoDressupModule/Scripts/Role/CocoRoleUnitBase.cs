using UnityEngine;
using TabTale;

namespace CocoPlay
{
	public class CocoRoleUnitBase : GameView
	{
		public CocoRoleEntity Owner { get; private set; }

		public virtual void Init (CocoRoleEntity owner)
		{
			Owner = owner;
		}
	}
}
