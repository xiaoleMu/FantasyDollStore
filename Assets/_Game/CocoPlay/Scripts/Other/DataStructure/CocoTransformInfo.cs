using UnityEngine;

namespace CocoPlay
{
	[System.Serializable]
	public class CocoTransformInfo
	{
		public Vector3 Position = Vector3.zero;
		public Vector3 EulerAngles = Vector3.zero;
		public Vector3 Scale = Vector3.one;
		public bool IsLocal = true;

		public CocoTransformInfo ()
		{
		}
		
		public CocoTransformInfo (Vector3 pos, Vector3 angles, Vector3 scale, bool isLocal)
		{
			Position = pos;
			EulerAngles = angles;
			Scale = scale;
			IsLocal = isLocal;
		}

		public CocoTransformInfo (Transform trans, bool isLocal)
		{
			SetByTransform (trans, isLocal);
		}

		public void SetByTransform (Transform trans, bool isLocal)
		{
			IsLocal = isLocal;

			if (isLocal) {
				Position = trans.localPosition;
				EulerAngles = trans.localEulerAngles;
				Scale = trans.localScale;
			} else {
				Position = trans.position;
				EulerAngles = trans.eulerAngles;
				Scale = trans.lossyScale;
			}
		}

		public void ApplyToTransform (Transform trans)
		{
			if (IsLocal) {
				trans.localPosition = Position;
				trans.localEulerAngles = EulerAngles;
				trans.localScale = Scale;
			} else {
				trans.position = Position;
				trans.eulerAngles = EulerAngles;
				trans.localScale = Vector3.Scale (Scale, CocoMath.Divide (trans.localScale, trans.lossyScale));
			}
		}

		public static void ApplyTransform (Transform fromTrans, Transform toTrans, bool isLocal)
		{
			if (isLocal) {
				toTrans.localPosition = fromTrans.localPosition;
				toTrans.localRotation = fromTrans.localRotation;
				toTrans.localScale = fromTrans.localScale;
			} else {
				toTrans.position = fromTrans.position;
				toTrans.rotation = fromTrans.rotation;
				toTrans.localScale =  Vector3.Scale (fromTrans.lossyScale, CocoMath.Divide (toTrans.localScale, toTrans.lossyScale));
			}
		}
	}
}