using UnityEngine;
using System.Collections;
using TabTale;
using CocoPlay;

namespace Game
{
	public class GameCameraControl : GameView
	{
		#region Follow

		[SerializeField]
		CocoSceneObjectFollow m_CameraFollow = null;
		[SerializeField]
		Transform m_CameraFollowTargetTrans = null;
		//[SerializeField]
		float m_CameraRotateFactor = 0.2f;
		//[SerializeField]
		float m_CameraZoomFactor = 0.01f;

		public CocoSceneObjectFollow CameraFollow {
			get {
				if (m_CameraFollow == null) {
					m_CameraFollow = Camera.main.GetComponent<CocoSceneObjectFollow> ();
				}
				return m_CameraFollow;
			}
		}

		public void ResetFollowTarget ()
		{
			if (m_CameraFollowTargetTrans == null) {
				return;
			}

			Transform cameraTrans = CameraFollow.transform;
			Vector3 forward = cameraTrans.forward;

			Ray ray = new Ray (cameraTrans.position, forward);
			Vector3 followOffset = CocoRay.GetWorldPosByZ (ray, transform.position.z);
			followOffset = m_CameraFollowTargetTrans.InverseTransformPoint (followOffset);
			CameraFollow.InitFollowTarget (m_CameraFollowTargetTrans, followOffset);
			//Debug.LogError (CameraFollow.FollowTargetOffset.x + ", " + CameraFollow.FollowTargetOffset.y + ", " + CameraFollow.FollowTargetOffset.z);
		}

		public void RotateCamera (Vector2 delta)
		{
			float deltaX = delta.x;
			float deltaY = -delta.y;
			if (Mathf.Abs (deltaX) >= Mathf.Abs (deltaY)) {
				CameraFollow.Rotate (new Vector2 (deltaX * m_CameraRotateFactor, 0));
			} else {
				CameraFollow.Rotate (new Vector2 (0, deltaY * m_CameraRotateFactor));
			}
		}

		public void ZoomCamera (float delta)
		{
			CameraFollow.Zoom (delta * m_CameraZoomFactor);
		}

		public float CameraFollowDumpping {
			get {
				return CameraFollow.followDumpping.Value;
			}
			set {
				CameraFollow.followDumpping.Value = value;
			}
		}

		#endregion


		#region Move

		[SerializeField]
		Vector3 m_CameraNormalPos = new Vector3 (0, 1.1f, 3.7f);
		[SerializeField]
		Vector3 m_CameraNormalAngles = new Vector3 (3, 180, 0);

		public void MoveCameraToNormalPos (float time, System.Action endAction = null)
		{
			MoveCamera (m_CameraNormalPos, m_CameraNormalAngles, time, endAction);
		}

		public void ResumeCameraToNormalPos ()
		{
			CameraFollow.ReturnInitPosition ();
		}

		public void MoveCamera (Vector3 pos, Vector3 eulerAngles, float time, System.Action endAction = null)
		{
			GameObject cameraGo = CameraFollow.gameObject;
			LeanTween.cancel (cameraGo);

			bool followEnabled = CameraFollow.enabled;
			if (followEnabled) {
				CameraFollow.enabled = false;
			}

			LeanTween.rotateLocal (cameraGo, eulerAngles, time).setEase (LeanTweenType.easeOutCubic);
			LeanTween.moveLocal (cameraGo, pos, time).setEase (LeanTweenType.easeOutCubic).onComplete = () => {
				if (followEnabled) {
					CameraFollow.enabled = true;
					ResetFollowTarget ();
				}
				if (endAction != null)
					endAction();
			};
		}

		#endregion
	}
}
