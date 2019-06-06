using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TabTale.Cocoplay
{
	public class GameUIAlertViewContentAdjuster : MonoBehaviour
	{
		public Text title;
		public Text message;
		public Text buttonOne;
		public Text buttonTwo;
		public float buttonOffsetX = 112;

		public void UpdateContent (string pTitle, string pMessage, string pButtonOne, string pButtonTwo)
		{
			title.text = pTitle;
			message.text = pMessage;
			buttonOne.text = pButtonOne;

			Transform btnOneTrans = buttonOne.transform.parent;
			Transform btnTwoTrans = buttonTwo.transform.parent;
			if (string.IsNullOrEmpty (pButtonTwo)) {
				btnTwoTrans.gameObject.SetActive (false);
				Vector3 pos = btnOneTrans.localPosition;
				pos.x = 0;
				btnOneTrans.localPosition = pos;
			} else {
				buttonTwo.text = pButtonTwo;
				Vector3 pos = btnOneTrans.localPosition;
				pos.x = - buttonOffsetX;
				btnOneTrans.localPosition = pos;
				pos.x = buttonOffsetX;
				btnTwoTrans.localPosition = pos;
			}
		}
	}
}