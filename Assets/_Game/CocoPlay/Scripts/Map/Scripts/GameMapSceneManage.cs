using UnityEngine;
using System.Collections;
using CocoPlay;
using System.Collections.Generic;
using TabTale;
using UnityEngine.UI;
using CocoPlay.Native;

namespace Game
{
	public class GameMapSceneManage : GameGenericSceneBase
	{
		#region Init Clean

		protected override void initCharacter ()
		{
			base.initCharacter ();

			StartCoroutine (initOtherRoles ());
		}


		private SceneStep m_CurSceneStep = SceneStep.Step_Common;

		private RoleSelectedPopup m_RoleSelectPopup;

		protected override void OnButtonClickWithButtonName (CocoUINormalButton button, string pButtonName)
		{
			base.OnButtonClickWithButtonName (button, pButtonName);

			switch (pButtonName){
			case "doll":
				CocoMainController.ShowPopup ("RoleSelectedPopup");
				m_RoleSelectPopup = FindObjectOfType<RoleSelectedPopup>();
				if (m_RoleSelectPopup != null){
					m_RoleSelectPopup.OnCloseDollSelect += OnChangeDoll;
				}
				break;

			case "Talk":
				StartCoroutine (HideTalk ());
				break;

			case "start":
				GlobalData.curSelectRole = -1;
				CocoMainController.EnterScene (CocoSceneID.Doll);
				break;
			}

		}

		private void OnChangeDoll (bool change){
			if (change){
				CocoMainController.EnterScene (CocoSceneID.Doll);
			}
		}

		#endregion


		#region OtherCharacter

		[SerializeField]
		Transform m_DollTransParent;
		[Inject]
		public GameDollData dollData {get; set;}

		private Vector3[] m_DollsPos = new Vector3[] {new Vector3(0.865f, 0.26f, 0f), new Vector3(0.31f, 0.26f, 0f), new Vector3(-0.235f, 0.26f, 0f),new Vector3(-0.84f, 0.26f, 0f),
			new Vector3(0.865f, -0.37f, 0f), new Vector3(0.31f, -0.37f, 0f), new Vector3(-0.235f, -0.37f, 0f),new Vector3(-0.84f, -0.37f, 0f)};

		protected IEnumerator initOtherRoles()
		{
			for (int i=0; i<recordStateModel.RecordDolls.Count; i++){
				string roleName = gameGlobalData.GetRoleConfigID(GameRoleID.Coco);
				CocoRoleEntity tempDoll = roleControl.CreateTempRole(roleName, roleName, m_DollTransParent);
				tempDoll.Dress.AddDressItem (recordStateModel.RecordDolls[i]);
				tempDoll.transform.localPosition = m_DollsPos[i];
				tempDoll.transform.localScale = Vector3.one * 100f;
				tempDoll.Animation.SetAnimationData (new GameDollAnimationData ());
				tempDoll.Animation.SetAnimatorController ("dressup_animator");
				tempDoll.Animation.SetAutoSwithEnable (true);
				tempDoll.Animation.Play (dollData.CA_Dressup_Standby);

				yield return new WaitForEndOfFrame ();
			}
		}

		#endregion


		#region Show Popup

		[SerializeField]
		GameObject m_TalkObj;
		[SerializeField]
		GameObject m_Girl;
		[SerializeField]
		GameObject m_TalkBG;
		[SerializeField]
		Text m_TalkTxt;

		private IEnumerator ShowTalk (){
			m_Girl.transform.SetLocal_X (900f);
			m_TalkBG.transform.localScale = Vector3.zero;
			m_TalkObj.SetActive (true);

			LeanTween.moveLocalX (m_Girl, 455f, 0.3f);
			yield return new WaitForSeconds (0.3f);

			LeanTween.scale (m_TalkBG, Vector3.one, 0.3f);
			yield return new WaitForSeconds (0.3f);
		}

		private IEnumerator HideTalk (){
			LeanTween.scale (m_TalkBG, Vector3.zero, 0.3f);
			yield return new WaitForSeconds (0.3f);

			LeanTween.moveLocalX (m_Girl, 900f, 0.3f);
			yield return new WaitForSeconds (0.3f);

			m_TalkObj.SetActive (false);
		}

		#endregion
	}
}
