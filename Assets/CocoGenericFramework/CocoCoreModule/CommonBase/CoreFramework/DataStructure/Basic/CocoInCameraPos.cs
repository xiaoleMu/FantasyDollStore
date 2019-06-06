using UnityEngine;
using CocoPlay;

namespace Game
{
	[System.Serializable]
	public class CocoInCameraPos
	{
		[SerializeField]
		Vector3 m_Pos;
		[SerializeField]
		Transform m_Trans;
		[SerializeField]
		Camera m_InCamera;

		public CocoInCameraPos (Vector3 pos, Camera camera = null, Transform trans = null)
		{
			m_Pos = pos;
			m_InCamera = camera;
			m_Trans = trans;
		}

		public Vector3 Pos {
			get {
				return m_Pos;
			}
		}

		public Transform Trans {
			get {
				return m_Trans;
			}
		}

		public Camera InCamera {
			get {
				return m_InCamera != null ? m_InCamera : Camera.main;
			}
		}

		public Vector3 WorldPos {
			get {
				return Trans == null ? Pos : Trans.TransformPoint (Pos);
			}
		}

		public Vector3 GetPosByCamera (Camera camera)
		{
			return CocoRay.ConvertCameraPos (WorldPos, InCamera, camera);
		}

		public Vector3 ScreenPos {
			get {
				return InCamera.WorldToScreenPoint (WorldPos);
			}
		}
	}

}