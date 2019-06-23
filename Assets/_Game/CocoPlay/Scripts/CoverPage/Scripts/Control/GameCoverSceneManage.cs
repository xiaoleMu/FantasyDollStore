using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CocoPlay;
using TabTale;
using UnityEngine.UI;

namespace Game
{
    public class GameCoverSceneManage : GameGenericSceneBase
    {
       
		protected override void OnButtonClickWithButtonName (CocoUINormalButton button, string pButtonName)
		{
			base.OnButtonClickWithButtonName (button, pButtonName);
			if (pButtonName == "btnCoverPageStart"){
				CocoMainController.EnterScene (CocoSceneID.Doll);
			}
		}
    }
}