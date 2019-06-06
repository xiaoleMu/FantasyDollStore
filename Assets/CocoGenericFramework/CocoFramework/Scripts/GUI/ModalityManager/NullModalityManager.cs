using UnityEngine;
using System.Collections;
using TabTale.Publishing;

namespace TabTale
{
	public class NullModalityManager : NullService, IModalityManager
	{
	    public bool IsModalPresent { get { return true; } }

		public string ModalName { get { return "NullModal"; } }

		public void CloseAllModals () {}

		public IModalHandle Add (IModalHandle handle, bool isOverriding = false, bool isStacking = false)
		{
			return  handle;
		}
	}
}
