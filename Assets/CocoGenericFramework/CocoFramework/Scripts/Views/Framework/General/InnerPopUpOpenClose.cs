using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class InnerPopUpOpenClose : View
	{
		Animator _animator;

		public GameObject maskGameObject;
		public bool hideHudElements=false;

		[Inject]
		public InnerPopUpHideHudElementsSignal hideHudElementsSignal{get;set;}

		protected override  void Awake ()
		{
			base.Awake ();
			_animator = GetComponent<Animator> ();
		}


		public void OpenPanel ()
		{
    		_animator.SetTrigger ("openPanel");
			
            if(maskGameObject!=null)
				maskGameObject.SetActive(true);

			if(hideHudElements)
				hideHudElementsSignal.Dispatch(true);

		}

		public void ClosePanel ()
		{
			_animator.SetTrigger ("closePanel");
			
            if(maskGameObject!=null)
				maskGameObject.SetActive(false);

			if(hideHudElements)
				hideHudElementsSignal.Dispatch(false);
		}
	}

	public class InnerPopUpHideHudElementsSignal:Signal<bool>{}

}
