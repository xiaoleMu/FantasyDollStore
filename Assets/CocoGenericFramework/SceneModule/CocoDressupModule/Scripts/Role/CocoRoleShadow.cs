using UnityEngine;
using System.Collections;


namespace CocoPlay
{
	[ExecuteInEditMode]
	public class CocoRoleShadow : MonoBehaviour
	{
		#region Init / Update

		void OnEnable ()
		{
			OnShadowSizeChanged (m_ShadowSize.Value);
			m_ShadowSize.onValueChanged = OnShadowSizeChanged;

			OnShadowStrengthChanged (m_ShadowStrength.Value);
			m_ShadowStrength.onValueChanged = OnShadowStrengthChanged;

			OnShadowRenderQueueChanged (m_ShadowRenderQueue.Value);
			m_ShadowRenderQueue.onValueChanged = OnShadowRenderQueueChanged;
		}

		void LateUpdate ()
		{
			if (m_FollowTarget == null) {
				m_FollowTarget = transform.parent;
			}
			if (m_FollowTarget == null) {
				return;
			}

			if (m_FollowInLocalSpace && transform.parent != null) {
				Vector3 pos = m_FollowOffset;
				if (m_FollowTarget != transform.parent) {
					pos += transform.parent.InverseTransformPoint (m_FollowTarget.position);
				}
				UpdateFreezePos (ref pos);
				transform.localPosition = pos;
			} else {
				Vector3 pos = m_FollowTarget.position + m_FollowOffset;
				UpdateFreezePos (ref pos);
				transform.position = pos;
			}

			if (m_ShadowAnglesInLocalSpace) {
				transform.localEulerAngles = m_ShadowAngles;
			} else {
				transform.eulerAngles = m_ShadowAngles;
			}
		}

		#endregion


		#region Follow

		[Header ("Follow")]
		[SerializeField]
		Transform m_FollowTarget = null;

		public Transform FollowTarget {
			get {
				return m_FollowTarget;
			}
			set {
				m_FollowTarget = value;
			}
		}

		[SerializeField]
		Vector3 m_FollowOffset = Vector3.zero;

		public Vector3 FollowOffset {
			get {
				return m_FollowOffset;
			}
			set {
				m_FollowOffset = value;
			}
		}

		[SerializeField]
		CocoOptionalFloatProperty m_FollowFreezePosY = new CocoOptionalFloatProperty (0.01f);

		public void EnableFollowFreezeY (float freezePos)
		{
			m_FollowFreezePosY.Used = true;
			m_FollowFreezePosY.Value = freezePos;
		}

		public void DisableFollowFreezeY ()
		{
			m_FollowFreezePosY.Used = false;
		}

		public float FollowFreezeY {
			get {
				return m_FollowFreezePosY.Value;
			}
		}

		void UpdateFreezePos (ref Vector3 pos)
		{
			if (m_FollowFreezePosY.Used) {
				pos.y = m_FollowFreezePosY.Value;
			}
		}

		[SerializeField]
		bool m_FollowInLocalSpace = true;

		public bool FollowInLocalSpace {
			get {
				return m_FollowInLocalSpace;
			}
			set {
				m_FollowInLocalSpace = value;
			}
		}

		#endregion


		#region Shadow Settings

		[Header ("Shadow Settings")]
		[SerializeField]
		CocoDetectableVector3Value m_ShadowSize = new CocoDetectableVector3Value (new Vector3 (11, 1, 11));

		public Vector3 ShadowSize {
			get {
				return m_ShadowSize.Value;
			}
			set {
				m_ShadowSize.Value = value;
			}
		}

		void OnShadowSizeChanged (Vector3 size)
		{
			transform.localScale = size;
		}

		[SerializeField]
		CocoDetectableFloatValue m_ShadowStrength = new CocoDetectableFloatValue (0.3f);

		public float ShadowStrength {
			get {
				return m_ShadowStrength.Value;
			}
			set {
				m_ShadowStrength.Value = value;
			}
		}

		[SerializeField]
		string m_ShadowStrengthProperty = "_Color";

		Material m_ShadowMaterial = null;

		Material ShadowMaterial {
			get {
				if (m_ShadowMaterial == null) {
					Renderer renderer = GetComponent<Renderer> ();
					if (renderer != null) {
						m_ShadowMaterial = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
					}
				}

				return m_ShadowMaterial;
			}
		}

		void OnShadowStrengthChanged (float strength)
		{
			if (ShadowMaterial == null || !ShadowMaterial.HasProperty (m_ShadowStrengthProperty)) {
				return;
			}

			Color color = ShadowMaterial.GetColor (m_ShadowStrengthProperty);
			color.a = Mathf.Clamp01 (strength);
			ShadowMaterial.SetColor (m_ShadowStrengthProperty, color);
		}

		[SerializeField]
		CocoDetectableIntValue m_ShadowRenderQueue = new CocoDetectableIntValue (3001);

		public int ShadowRenderQueue {
			get {
				return m_ShadowRenderQueue.Value;
			}
			set {
				m_ShadowRenderQueue.Value = value;
			}
		}

		void OnShadowRenderQueueChanged (int renderQueue)
		{
			if (ShadowMaterial == null) {
				return;
			}

			ShadowMaterial.renderQueue = renderQueue;
		}

		[SerializeField]
		Vector3 m_ShadowAngles = Vector3.zero;

		public Vector3 ShadowAngles {
			get {
				return m_ShadowAngles;
			}
			set {
				m_ShadowAngles = value;
			}
		}

		[SerializeField]
		bool m_ShadowAnglesInLocalSpace = true;

		public bool ShadowAnglesInLocalSpace {
			get {
				return m_ShadowAnglesInLocalSpace;
			}
			set {
				m_ShadowAnglesInLocalSpace = value;
			}
		}

		#endregion
	}
}