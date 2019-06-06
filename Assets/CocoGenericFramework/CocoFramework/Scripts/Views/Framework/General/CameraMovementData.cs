using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;
using System;

namespace TabTale
{
	[Serializable]
	public class CameraMovementData : MonoBehaviour
	{
		public Camera contentCamera;
		public Transform cameraTransform;
		public bool allowHorizontalMove;
		public bool allowVerticalMove;
	}
}