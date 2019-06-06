using UnityEngine;
using System;

namespace TabTale
{
	public interface IModalityManager : IService
	{
		IModalHandle Add (IModalHandle handle, bool isOverriding=false, bool isStacking = false);
        bool IsModalPresent { get; }
		string ModalName {get;}
		void CloseAllModals();
	}

    public enum IModalMaskType { Masked, CloseOnOutSideTap, NonMasked}

	public interface IModalHandle
	{
		event Action<IModalHandle> CloseCompleteEvent;
		event Action OpenHandlerEvent;
		event Action CloseHandlerEvent;
		
        IModalMaskType MaskType { get; set; }
		
		GameObject Parent { get; set; }
		
		string Name {get;}

		void Open ();
		
		void Close ();

		void MoveToBackground ();

		void RetunToForeground ();

		void Destroy ();


	}
	
	public interface IModalsCanvas
	{
		event Action MaskClickEvent;
		
		void ShowMask ();
		
		void HideMask ();
	}

}
