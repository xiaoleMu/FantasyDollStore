using UnityEngine;
using System.Collections;
using CocoPlay;
using System.Collections.Generic;
using TabTale;
using UnityEngine.UI;
using CocoPlay.Native;

namespace Game
{
	public class GameDollSceneManage : GameGenericSceneBase
	{

		[Inject]
		public GameDollData dressupData { get; set; }

		[Inject]
		public DragGestureSignal dragGestureSignal { get; set; }

//		[Inject]
//		public PinchGestureSignal pinchGestureSignal { get; set; }

		[Inject]
		public GameDollCategoryBtnClickSignal categoryBtnClickSignal { get; set; }

		[Inject]
		public GameDollItemBtnClickSignal itemBtnClickSignal { get; set; }

		[Inject]
		public GameGlobalData m_GameData { get; set; }

		Camera m_MainCamera;
		Vector3 m_CameraFarPos = new Vector3 (-0.55f, 1.86f, 4.33f);
		Vector3 m_CameraNearPos = new Vector3 (-0.28f, 1.86f, 1.8f);
		Vector3 m_Camera_Pos_Shoes = new Vector3 (-0.28f, 0.75f, 1.8f);
		Vector3 m_Camera_Pos_Acessory = new Vector3 (-0.402f, 1.86f, 2.56f);
		Vector3 m_Camera_Pos_Skirt = new Vector3(-0.432f, 1.85f, 2.916f);


		Vector3 m_CameraMoveDir;

		Vector3 m_CameraOriginPos;
		Vector3 m_CameraOriginAngles;
		Vector3 m_CharOriAngle = new Vector3 (0f, 0f, 0f);

		float Drag_Max_Y = 1.86f;
		float Drag_Min_Y = 0.75f;

		string m_CurCategory;

		GameObject dressGuide;

		#region Init Clean

		Vector3 recordScale;

		protected override void Start ()
		{
			base.Start ();

			dressGuide = CocoLoad.InstantiateOrCreate ("character_rotate_arrow", transform, CocoLoad.TransStayOption.Local);
			Input.multiTouchEnabled = true;
			CocoNative.Log("===ShowDialog==Start_SceneName: "+m_SceneID.ToString());

			if (GlobalData.curSelectRole != -1){
				dressupData.m_DetailIndex = recordStateModel.RecordDolls[dressupData.m_DetailIndex].detailIndex;
			}
			else {
				m_CurRole.Dress.AddDressItem (new List<string> (){"eye_001","body_001","ear_001","tail_001","eye_001"});
			}
//			dressupData.m_DetailIndex = GlobalData.m_DetailIndex;
		}

		protected override void initCharacter ()
		{
			base.initCharacter ();

			m_CurRole.gameObject.SetActive (true);
			m_MainCamera = Camera.main;
			m_MainCamera.transform.localPosition = m_CameraFarPos;
			m_CameraOriginPos = m_MainCamera.transform.position;
			m_CameraOriginAngles = m_MainCamera.transform.eulerAngles;
			m_CameraMoveDir = m_CameraFarPos - m_CameraNearPos;
			GetComponent<GameDollUIControl> ().Init (m_CurSceneStep);


			m_CurRole.Animation.SetAnimationData (new GameDollAnimationData ());
			m_CurRole.Animation.SetAnimatorController ("dressup_animator");
			m_CurRole.Animation.SetAutoSwithEnable (true);
			m_CurRole.Animation.Play (dressupData.CA_Dressup_Standby);

			m_CurRole.transform.localPosition = new Vector3 (-0.04f, 0.26f, -0.17f);
			m_CurRole.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			recordScale = m_CurRole.transform.localScale;
			m_CurRole.transform.localScale = Vector3.one * 300f;

			m_CharOriAngle = m_CurRole.transform.eulerAngles;

//			StartCoroutine (initOtherRoles ());
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();

			dragGestureSignal.AddListener (OnSceneDrag);
//			pinchGestureSignal.AddListener (OnScenePinch);
			categoryBtnClickSignal.AddListener (OnCategoryBtnClick);
			itemBtnClickSignal.AddListener (OnItemBtnClick);
		}

		protected override void RemoveListeners ()
		{
			dragGestureSignal.RemoveListener (OnSceneDrag);
//			pinchGestureSignal.RemoveListener (OnScenePinch);
			categoryBtnClickSignal.RemoveListener (OnCategoryBtnClick);
			itemBtnClickSignal.RemoveListener (OnItemBtnClick);
			base.RemoveListeners ();
		}

		private SceneStep m_CurSceneStep = SceneStep.Step_Common;

		private RoleSelectedPopup m_RoleSelectPopup;

		protected override void OnButtonClickWithButtonName (CocoUINormalButton button, string pButtonName)
		{
			base.OnButtonClickWithButtonName (button, pButtonName);

			if (pButtonName == "next"){
				switch (m_CurSceneStep){
				case SceneStep.Step_Common:
					m_CurSceneStep = SceneStep.Step_Detail;
					StartCoroutine (ShowDetail ());
					break;

				case SceneStep.Step_Detail:
					m_CurSceneStep = SceneStep.Step_Finish;
					button.gameObject.SetActive (false);
					StartCoroutine (PlayAni ());
					break;

					default:
					m_CurSceneStep = SceneStep.Step_Common;
					break;
				}
			}
			else if (pButtonName == "font"){
				switch (m_CurSceneStep){
				case SceneStep.Step_Common:
					CocoMainController.EnterScene (CocoSceneID.Map);
					break;

				case SceneStep.Step_Detail:
					m_CurSceneStep = SceneStep.Step_Common;
					break;

				case SceneStep.Step_Finish:
					m_CurSceneStep = SceneStep.Step_Detail;
					StartCoroutine (ShowDetail ());
					break;

				default:
					m_CurSceneStep = SceneStep.Step_Common;
					break;
				}
			}
			else if (pButtonName == "doll"){
				CocoMainController.ShowPopup ("RoleSelectedPopup");
				m_RoleSelectPopup = FindObjectOfType<RoleSelectedPopup>();
				if (m_RoleSelectPopup != null){
					m_RoleSelectPopup.OnCloseDollSelect += OnChangeDoll;
				}
			}
		}

		private void OnChangeDoll (bool change){
			if (change){
				m_CurRole.Dress.AddDressItem (recordStateModel.RecordDolls[GlobalData.curSelectRole].dress);
//				m_CurRole.Dress.getdre
			}
		}

		private IEnumerator ShowDetail (){
			CocoMainController.Instance.TouchEnable = false;

			CCAction.RotateTo (m_CurRole.gameObject, new Vector3 (0f, 0f, 0f), 0.8f, LeanTweenType.easeInOutQuad);

			yield return StartCoroutine (GetComponent<GameDollUIControl> ().HideAni ());
			GetComponent<GameDollUIControl> ().DetailCategoryClick ();
			yield return StartCoroutine (GetComponent<GameDollUIControl> ().ShowAni ());

			CocoMainController.Instance.TouchEnable = true;
		}

		#endregion

		protected override void PlayEnd ()
		{
			Input.multiTouchEnabled = false;
			base.PlayEnd ();
			m_CurRole.transform.localPosition = Vector3.zero;
			m_CurRole.transform.localEulerAngles = Vector3.zero;
			m_MainController.popManager.CloseAllModals ();
//			DressupGlobalData.Instance.usedDressList.Clear();
			m_CurRole.transform.localScale = recordScale;
		}

		#region Category Change Listener

		private void OnCategoryBtnClick (GameDollCategoryButton button)
		{

			m_CurCategory = button.DressupCategoryData.CategoryID;
			ResetCameraAndCharaceter (m_CurCategory);
		}

		private void OnItemBtnClick (GameDollItemButton button)
		{
			string tCategoryID = button.DressupItemData.LinkedDressItemHolder.ParentHolder.id;
			ResetCameraAndCharaceter (tCategoryID);
			PlayCharacterAnimation (dressupData.GetCategoryData (tCategoryID).m_Animation);
		}

		#endregion


		#region Gesture

		[SerializeField]
		Collider m_UICollider;

		bool isUGUITouched = false;
		bool m_IsDragRotating;

		void OnSceneDrag (DragGesture pDragGesture)
		{
			switch (pDragGesture.Phase) {
			case ContinuousGesturePhase.Started:
				isUGUITouched = CCCollider.IsTouchCollider (CocoMainController.UICamera, m_UICollider);
				m_IsDragRotating = (Mathf.Abs (pDragGesture.DeltaMove.x) > Mathf.Abs (pDragGesture.DeltaMove.y));
				if (m_IsDragRotating)
					CCAction.Stop (m_CurRole.gameObject);
				break;

			case ContinuousGesturePhase.Updated:
				if (!isUGUITouched && !LeanTween.isTweening (m_CurRole.gameObject)) {
					if (m_IsDragRotating) {
						m_CurRole.transform.Rotate (0, -pDragGesture.DeltaMove.x * 0.5f, 0);
						if (dressGuide != null) {
							Destroy (dressGuide);
						}
					} else
						DragCamera (pDragGesture.DeltaMove.y);
				}
				break;

			case ContinuousGesturePhase.Ended:
				isUGUITouched = false;
				break;
			}
		}

//		private void OnScenePinch (PinchGesture pPinchGesture)
//		{
//			switch (pPinchGesture.Phase) {
//			case ContinuousGesturePhase.Started:
//				isUGUITouched = CCCollider.IsTouchCollider (CocoMainController.UICamera, m_UICollider);
//				if (!isUGUITouched)
//					CCAction.Stop (Camera.main.gameObject);
//				break;
//			case ContinuousGesturePhase.Updated:
//				if (!isUGUITouched) {
//
//					float dis = Mathf.Abs (Camera.main.transform.eulerAngles.x - m_CameraOriginAngles.x);
//
//					if (dis > 0) {
//
//						CCAction.RotateToX (Camera.main.gameObject, m_CameraOriginAngles.x, 0.2f);
//
//					}
//
//					Vector3 NearPos = GetNearPos ();
//					Vector3 pCamPos = Camera.main.transform.localPosition;
//					pCamPos -= m_CameraMoveDir * pPinchGesture.Delta * Time.deltaTime * 0.15f;
//					pCamPos.x = CheckValue (pCamPos.x, NearPos.x, m_CameraFarPos.x);
//					pCamPos.y = CheckValue (pCamPos.y, NearPos.y, m_CameraFarPos.y);
//					pCamPos.z = CheckValue (pCamPos.z, NearPos.z, m_CameraFarPos.z);
//					Camera.main.transform.position = pCamPos;
//					CheckCameraPosY ();
//
//				}
//
//				break;
//			case ContinuousGesturePhase.Ended:
//				isUGUITouched = false;
//				break;
//			}
//		}

		void DragCamera (float DeltaY)
		{
			float pos_y = m_MainCamera.transform.position.y;
			pos_y -= DeltaY * Time.deltaTime * 0.08f;
			CCPosition.Set_Y (m_MainCamera.transform, pos_y);
			CheckCameraPosY ();
		}

		Vector3 GetNearPos ()
		{
			return m_CameraNearPos;
		}

		void CheckCameraPosY ()
		{
			float persent = (m_CameraFarPos.z - Camera.main.transform.position.z) / (m_CameraFarPos.z - m_CameraNearPos.z);

			float pos_y = Camera.main.transform.position.y;

			float offset_y = Drag_Max_Y - Drag_Min_Y;
			float Enable_Max_Y = Drag_Max_Y - (1 - persent) * (Drag_Max_Y - m_CameraFarPos.y);
			float Enable_Min_Y = Enable_Max_Y - offset_y * persent;

			pos_y = Mathf.Clamp (pos_y, Enable_Min_Y, Enable_Max_Y);
			CCPosition.Set_Y (Camera.main.transform, pos_y);
		}

		void ResetCameraAndCharaceter (string category)
		{
			ResetCameraDir ();

			Vector3 cameraPos = Camera.main.transform.position;
			cameraPos.y = m_CurRole.transform.position.y;
			Quaternion _Quaternion_A = Quaternion.LookRotation (cameraPos - (m_CurRole.transform.position));


			CCAction.RotateTo (m_CurRole.gameObject, m_CharOriAngle, 0.8f);
			if (category == "shoes") {
				CCAction.Move (Camera.main.gameObject, m_Camera_Pos_Shoes, 0.8f, LeanTweenType.easeInOutQuad);
			} else if (category == "accessories") {
				CCAction.Move (Camera.main.gameObject, m_Camera_Pos_Acessory, 0.8f, LeanTweenType.easeInOutQuad);
			}else
 			{
				CCAction.Move (Camera.main.gameObject, m_CameraOriginPos, 0.8f, LeanTweenType.easeInOutQuad);
				CCAction.RotateTo (Camera.main.gameObject, m_CameraOriginAngles, 0.8f, LeanTweenType.easeInOutQuad);
			}

			if (category == "tail"){
				CCAction.RotateTo (m_CurRole.gameObject, new Vector3 (0f, 160f, 0f), 0.8f, LeanTweenType.easeInOutQuad);
			}
			else{
				CCAction.RotateTo (m_CurRole.gameObject, new Vector3 (0f, 0f, 0f), 0.8f, LeanTweenType.easeInOutQuad);
			}
		}

		void ResetCameraDir ()
		{
			m_CameraMoveDir = m_CameraFarPos - GetNearPos ();
		}

//		float CheckValue (float value, float x, float y)
//		{
//			float min;
//			float max;
//			if (x > y) {
//				min = y;
//				max = x;
//			} else {
//				min = x;
//				max = y;
//			}
//			value = Mathf.Clamp (value, min, max);
//			return value;
//		}

		#endregion

		#region Character

		void PlayCharacterAnimation (CCAnimationData animData)
		{
			if (animData != m_CurRole.Animation.CurrAnimData) {
				m_CurRole.Animation.Play (animData);
			}
		}

		public void MakeCharacterFaceToCamera ()
		{
			CCAction.Stop (m_CurRole.gameObject);
			Vector3 direction = Camera.main.transform.position - m_CurRole.transform.position;
			direction.y = 0;
			Quaternion rotation = Quaternion.LookRotation (direction);
			float angle = Quaternion.Angle (m_CurRole.transform.rotation, rotation);
			if (angle > 0.1f) {
				LeanTween.rotateY (m_CurRole.gameObject, rotation.eulerAngles.y, angle / 360).setEase (LeanTweenType.easeOutQuad);
			}
		}

		void ZoomCameraToCharacter (bool zoomIn)
		{
			Transform cameraTrans = Camera.main.transform;
			CCAction.Stop (cameraTrans.gameObject);
			Vector3 pos = zoomIn ? m_CameraNearPos : m_CameraFarPos;
			float distance = Vector3.Distance (pos, cameraTrans.localPosition);
			if (distance > 0.01f) {
				LeanTween.moveLocal (cameraTrans.gameObject, pos, distance / 0.25f).setEase (LeanTweenType.easeOutQuad);
			}
		}

		#endregion







		#region DollAni

		private IEnumerator PlayAni (){
			RecordDoll ();

			yield return StartCoroutine (GetComponent<GameDollUIControl> ().HideAni ());

			yield return StartCoroutine (m_CurRole.Animation.StartPlay (dressupData.CA_Dressup_win01));
			yield return StartCoroutine (m_CurRole.Animation.StartPlay (dressupData.CA_Dressup_win02));

			CocoMainController.EnterScene (CocoSceneID.RecordVolue);
		}

		#endregion


		#region OtherCharacter

		[SerializeField]
		Transform m_DollTransParent;

		private Vector3[] m_DollsPos = new Vector3[] {new Vector3(0.95f, 0.34f, 0f), new Vector3(0.31f, 0.34f, 0f), new Vector3(-0.31f, 0.34f, 0f),new Vector3(-0.95f, 0.34f, 0f),
			new Vector3(0.95f, -0.39f, 0f), new Vector3(0.31f, -0.39f, 0f), new Vector3(-0.31f, -0.39f, 0f),new Vector3(-0.95f, -0.39f, 0f)};

		protected IEnumerator initOtherRoles()
		{
			for (int i=0; i<recordStateModel.RecordDolls.Count; i++){
				string roleName = gameGlobalData.GetRoleConfigID(GameRoleID.Coco);
				CocoRoleEntity tempDoll = roleControl.CreateTempRole(roleName, roleName, m_DollTransParent);
				tempDoll.Dress.AddDressItem (recordStateModel.RecordDolls[i].dress);
				tempDoll.transform.localPosition = m_DollsPos[i];
				tempDoll.transform.localScale = Vector3.one * 120f;
				tempDoll.Animation.SetAnimationData (new GameDollAnimationData ());
				tempDoll.Animation.SetAnimatorController ("dressup_animator");
				tempDoll.Animation.SetAutoSwithEnable (true);
				tempDoll.Animation.Play (dressupData.CA_Dressup_Standby);

				yield return new WaitForEndOfFrame ();
			}
		}

		private void RecordDoll (){
			List<string> doll = m_CurRole.Dress.GetAllDressIds ();
			DollRecordData recordData = new DollRecordData ();
			recordData.detailIndex = dressupData.m_DetailIndex;
			recordData.dress = doll;
			recordStateModel.AddRecordDoll (recordData, GlobalData.curSelectRole);
		}

		#endregion


	}
}
