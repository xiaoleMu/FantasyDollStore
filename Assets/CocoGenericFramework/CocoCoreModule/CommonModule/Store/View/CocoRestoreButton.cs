using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CocoPlay {
	public class CocoRestoreButton : CocoUINormalButton 
	{
		[Inject]
		public CocoStoreControl StoreControl {get; set;}
        [Inject]
        public StoreClickSignal storeClickSingal{ get; set;}

		protected override void OnClick ()
		{
			base.OnClick ();
            storeClickSingal.Dispatch("restore");
			StoreControl.RequestRestore ();
		}
	}
}