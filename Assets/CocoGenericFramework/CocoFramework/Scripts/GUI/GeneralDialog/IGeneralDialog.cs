using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public interface IGeneralDialog
	{

		event Action OnDestroyEvent;

		void Show (GeneralDialogData data);

		void Hide ();
	}
}
