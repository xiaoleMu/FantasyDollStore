using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	[RequireComponent(typeof(ObjectPool))]
	public class ModalityManager : MonoService,IModalityManager
	{

		IList<IModalHandle> _queueModals = new List<IModalHandle> ();
		IList<IModalHandle> _sustainedModals = new List<IModalHandle> ();
		IModalHandle _current;
		IModalsCanvas _modalsCanvas;
		GameObject _modalsParent;
	    public bool IsModalPresent {get { return _current != null; } }

		public string ModalName {get { return _current != null ? _current.Name : ""; } }

		public ModalityManager ()
		{
			
		}

		protected override IEnumerator InitService (IServiceResolver resolver)
		{
			GameObject dialogCanvas = (GameApplication.Instance as GameApplication).gameObject.GetComponentInChildren<Canvas>().gameObject;

			_modalsParent = dialogCanvas;
			if (_modalsParent != null) {
				_modalsCanvas = _modalsParent.GetComponentOrInterface<IModalsCanvas> ();
				if (_modalsCanvas != null) {
					_modalsCanvas.MaskClickEvent += OnMaskClickEvent;
				}
			} else {
				CoreLogger.LogError ("ModalsService", "cannot find parent canvas in scene");
			}

			yield break;
		}

		public IModalHandle Add (IModalHandle handle, bool isOverriding=false, bool isStacking = false)
		{
			if (_current == null) {
				if (_modalsCanvas != null && handle.MaskType != IModalMaskType.NonMasked)
					_modalsCanvas.ShowMask ();
				Open (handle);
				return handle;
			}
			if(isStacking)
			{
				if (isOverriding) 
				{
					_sustainedModals.Add (_current);
					Open (handle);
				} 
				else 
				{
					_current.MoveToBackground ();
					_sustainedModals.Add (_current);
					Open (handle);
				}
			}

			else
			{
				if (isOverriding) 
				{
					_current.MoveToBackground ();
					_sustainedModals.Add (_current);
					Open (handle);
				} 
				else 
				{
					_queueModals.Add (handle);
				}

			}
				
			return handle;
		}

		void Open (IModalHandle modal)
		{
			_current = modal;
			_current.Parent = _modalsParent;
			_current.CloseCompleteEvent += OnCloseComplete;
			_current.Open ();

		}

		void OnCloseComplete (IModalHandle closedHandle)
		{
			closedHandle.CloseCompleteEvent -= OnCloseComplete;
			closedHandle.Destroy ();

			if (closedHandle == _current)
			{
				_current = null;
			} 
			else 
			{
				_sustainedModals.Remove (closedHandle);
				_queueModals.Remove (closedHandle);
			}

			if (_sustainedModals.Count > 0) {
				_current = _sustainedModals [_sustainedModals.Count - 1];
				_sustainedModals.RemoveAt (_sustainedModals.Count - 1);
				_current.RetunToForeground ();
			} else if (_queueModals.Count > 0) {
				Open (_queueModals [0]);
				_queueModals.RemoveAt (0);
			} else {
				if (_modalsCanvas != null)
					_modalsCanvas.HideMask ();
			}
		}

		void OnMaskClickEvent ()
		{
			if (_current != null && _current.MaskType == IModalMaskType.CloseOnOutSideTap)
				_current.Close ();
		}

		public void CloseAllModals()
		{
			if (_current != null) 
			{
				_current.Close ();
				_sustainedModals.Clear ();
				_queueModals.Clear ();
			}
		}
	}
}


