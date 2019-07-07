using UnityEngine;
using System.Collections;
using CocoPlay;
using System.Collections.Generic;
using TabTale;
using UnityEngine.UI;
using CocoPlay.Native;
using System.Linq;

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
				if (GlobalData.curSelectRole != -1){
					roleControl.CurRole.Dress.AddDressItem (recordStateModel.RecordDolls[GlobalData.curSelectRole].dress);

					List<string> body = roleControl.CurRole.Dress.GetDressIdsByCategory ("body");
					List<string> ear = roleControl.CurRole.Dress.GetDressIdsByCategory ("ear");
					List<string> nose = roleControl.CurRole.Dress.GetDressIdsByCategory ("nose");
					List<string> tail = roleControl.CurRole.Dress.GetDressIdsByCategory ("tail");
					List<string> ids = body.Union(ear).Union(nose).Union(tail).ToList<string>(); 
					foreach (string id in ids){
						var dress = roleControl.CurRole.Dress.GetDressItem (id);
						SkinnedMeshRenderer render = dress.ItemRenderers[0];
						for(int i=0; i<render.materials.Length; i++){
							Texture2D normal = Resources.Load <Texture2D> (string.Format("role/basic/basic/textures/common/material_{0:D3}_nomal", recordStateModel.RecordDolls[GlobalData.curSelectRole].detailIndex));
							Texture2D rgb = Resources.Load <Texture2D> (string.Format("role/basic/basic/textures/common/material_{0:D3}_rgb", recordStateModel.RecordDolls[GlobalData.curSelectRole].detailIndex));
							render.materials[i].SetTexture ("_BumpMap", normal);
							render.materials[i].SetTexture ("_metallicRsmoothGdiffuseB", rgb);
						}
					}
				}
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
				tempDoll.Dress.AddDressItem (recordStateModel.RecordDolls[i].dress);

				List<string> body = roleControl.CurRole.Dress.GetDressIdsByCategory ("body");
				List<string> ear = roleControl.CurRole.Dress.GetDressIdsByCategory ("ear");
				List<string> nose = roleControl.CurRole.Dress.GetDressIdsByCategory ("nose");
				List<string> tail = roleControl.CurRole.Dress.GetDressIdsByCategory ("tail");
				List<string> ids = body.Union(ear).Union(nose).Union(tail).ToList<string>(); 
				foreach (string id in ids){
					var dress = roleControl.CurRole.Dress.GetDressItem (id);
					SkinnedMeshRenderer render = dress.ItemRenderers[0];
					for(int j=0; j<render.materials.Length; j++){
						Texture2D normal = Resources.Load <Texture2D> (string.Format("role/basic/basic/textures/common/material_{0:D3}_nomal", recordStateModel.RecordDolls[i].detailIndex));
						Texture2D rgb = Resources.Load <Texture2D> (string.Format("role/basic/basic/textures/common/material_{0:D3}_rgb", recordStateModel.RecordDolls[i].detailIndex));
						render.materials[j].SetTexture ("_BumpMap", normal);
						render.materials[j].SetTexture ("_metallicRsmoothGdiffuseB", rgb);
					}
				}

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
