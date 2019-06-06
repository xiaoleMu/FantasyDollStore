using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoSceneObjectFollow : MonoBehaviour
	{
		public System.Action<CocoSceneObjectFollow> OnMoveEvent;
		#region Init/Update

		bool m_Inited;
		Vector3 m_FollowInitOffset;

		void Start ()
		{
			if (followOnStart) {
				Init ();
			}
		}

		void LateUpdate ()
		{
			if (!m_Inited || m_FollowTargetTrans == null)
				return;

			m_FollowTargetPos = m_FollowTargetTrans.TransformPoint (m_FollowTargetOffset);

			Vector3 offset = m_ExpectedOffset;
			bool avoid = AvoidObstruction (ref offset);

			if (followDumpping.Used) {
				Vector3 currOffset = transform.position - m_FollowTargetPos;
				if (CocoMath.Approximately (currOffset, offset, 0.0001f)) {
					return;
				}

				float t = followDumpping.Value * Time.deltaTime;
				offset = !avoid ? Vector3.Slerp (currOffset, offset, t) : Vector3.Lerp (currOffset, offset, t);
			}

			transform.position = m_FollowTargetPos + offset;
			transform.LookAt (m_FollowTargetPos);

			if(OnMoveEvent != null)
				OnMoveEvent(this);
		}

		void Init ()
		{
			if (m_Inited) {
				return;
			}

			UpdateExpectedPosition ();

			m_FollowInitOffset = m_ExpectedOffset;
			m_CurrRotateAngle = Vector2.zero;

			m_Inited = true;
		}

		#endregion


		#region Follow

		[Header ("Follow")]
		public bool followOnStart = true;

		[SerializeField]
		Transform m_FollowTargetTrans = null;

		public Transform FollowTargetTrans {
			get {
				return m_FollowTargetTrans;
			}
		}

		[SerializeField]
		Vector3 m_FollowTargetOffset = Vector3.zero;

		public Vector3 FollowTargetOffset {
			get {
				return m_FollowTargetOffset;
			}
			set {
				m_FollowTargetOffset = value;
			}
		}

		Vector3 m_FollowTargetPos;

		public CocoFloatRange followDistanceRange = new CocoFloatRange (10, 20);
		public CocoOptionalFloatProperty followDumpping = new CocoOptionalFloatProperty (4);

		// expected
		Vector3 m_ExpectedOffset;
		float m_ExpectedDistance;

		public void InitFollowTarget (Transform pTargetTrans, Vector3 pFocalPointOffset)
		{
			m_FollowTargetTrans = pTargetTrans;
			m_FollowTargetOffset = pFocalPointOffset;

			m_Inited = false;
			Init ();
		}

		void UpdateExpectedPosition ()
		{
			m_FollowTargetPos = m_FollowTargetTrans.TransformPoint (m_FollowTargetOffset);
			m_ExpectedOffset = transform.position - m_FollowTargetPos;
			float distance = m_ExpectedOffset.magnitude;
			if (distance <= 0) {
				return;
			}

			m_ExpectedDistance = Mathf.Clamp (distance, followDistanceRange.From, followDistanceRange.To);
			m_ExpectedOffset *= m_ExpectedDistance / distance;
			transform.position = m_FollowTargetPos + m_ExpectedOffset;
		}

		public void Zoom (float pDelta)
		{
			if (pDelta == 0)
				return;

			float distance = Mathf.Clamp (m_ExpectedDistance - pDelta, followDistanceRange.From, followDistanceRange.To);
			//Debug.LogErrorFormat ("{0}->Zoom: delta {1}, origin: {2}, new: {3}", GetType ().Name, pDelta, m_ExpectedDistance, distance);
			if (distance == m_ExpectedDistance)
				return;

			m_ExpectedOffset = m_ExpectedOffset * distance / m_ExpectedDistance;
			m_ExpectedDistance = distance;
		}

		#endregion


		#region Rotation

		[Header ("Rotation")]
		public CocoOptionalFloatRangeProperty horizontalAngleRange = new CocoOptionalFloatRangeProperty ();
		public CocoFloatRange verticalAngleRange = new CocoFloatRange (-20, 75);
		Vector2 m_CurrRotateAngle = Vector2.zero;

		public void Rotate (Vector2 pAngle)
		{
			Vector2 totalAngle = m_CurrRotateAngle + pAngle;
			if (horizontalAngleRange.Used) {
				totalAngle.x = Mathf.Clamp (totalAngle.x, horizontalAngleRange.Value.From, horizontalAngleRange.Value.To);
			}
			totalAngle.y = Mathf.Clamp (totalAngle.y, verticalAngleRange.From, verticalAngleRange.To);
			m_CurrRotateAngle = totalAngle;

			Vector3 offset = Quaternion.AngleAxis (m_CurrRotateAngle.x, Vector3.up) * m_FollowInitOffset;
			offset = Quaternion.AngleAxis (m_CurrRotateAngle.y, transform.right) * offset;
			m_ExpectedOffset = offset.normalized * m_ExpectedDistance;
		}

		#endregion


		#region Avoid Obstruction

		[Header ("Obstruction")]
		public CocoOptionalLayerMaskProperty obstructionLayerMask = new CocoOptionalLayerMaskProperty (1);

		bool AvoidObstruction (ref Vector3 offset)
		{
			if (!obstructionLayerMask.Used) {
				return false;
			}
				
			RaycastHit hit;
			if (!Physics.Raycast (m_FollowTargetPos, offset, out hit, m_ExpectedDistance, obstructionLayerMask.Value.value)) {
				return false;
			}

			if (hit.transform == m_FollowTargetTrans) {
				return false;
			}

			offset = hit.point - m_FollowTargetPos;
			return true;
		}

		#endregion


		#region Return

		public void ReturnInitPosition ()
		{
			if (m_FollowTargetTrans == null) {
				return;
			}

			m_FollowTargetPos = m_FollowTargetTrans.TransformPoint (m_FollowTargetOffset);
			m_ExpectedDistance = m_FollowInitOffset.magnitude;
			m_ExpectedOffset = m_FollowInitOffset;
			m_CurrRotateAngle = Vector2.zero;
		}

		#endregion
	}
}