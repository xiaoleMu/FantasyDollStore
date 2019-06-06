using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabTale {

	public static class CameraUtils
	{
		public static Camera CloneCamera(Camera original)
		{
			Camera cameraClone = GameObject.Instantiate(original) as Camera;
			cameraClone.CopyFrom(original);
			cameraClone.name = original.name + "_clone";
			AudioListener listener = cameraClone.GetComponent<AudioListener>();
			if(listener != null)
				listener.enabled = false;
			
			//cameraClone.tag = "";
			
			return cameraClone;
		}

		public static IList<Camera> CloneCameras(IEnumerable<Camera> originals)
		{
			return originals.Select(c => CloneCamera(c)).ToList();
		}

		public static Camera CloneMainCamera()
		{
			Camera cameraClone = CloneCamera(Camera.main);

			//cameraClone.tag = "";
            
            return cameraClone;
        }

		public static void FixCameraToBoundries (Camera camera, Transform cameraTransform, Edges bounds, bool allowHorizontalMove, bool allowVerticalMove)
		{ 
			Vector2 newPos = GetAdjustedWorldCenterPoint (camera, bounds, cameraTransform.position.x, cameraTransform.position.y);
			
			if (!allowHorizontalMove)
				newPos.x = cameraTransform.position.x;
			
			if (!allowVerticalMove)
				newPos.y = cameraTransform.position.y;
			
			cameraTransform.position = new Vector3 (newPos.x, newPos.y, cameraTransform.position.z);
		}

		public static float GetLeftMostCameraPosInScene(Camera Camera, Vector3 leftEdge, float cameraPosition = Mathf.Infinity){
			return leftEdge.x + GetViewportCenterInWorldPoint(Camera, leftEdge.z, cameraPosition).x;
		}

		public static float GetRightMostCameraPosInScene(Camera Camera, Vector3 rightEdge, float cameraPosition = Mathf.Infinity){
			return rightEdge.x - GetViewportCenterInWorldPoint(Camera, rightEdge.z, cameraPosition).x;
		}

		public static float GetTopMostCameraPosInScene(Camera Camera, Vector3 topEdge, float cameraPosition = Mathf.Infinity){
			return topEdge.y - GetViewportCenterInWorldPoint(Camera, topEdge.z, cameraPosition).y;
		}
		
		public static float GetBottomMostCameraPosInScene(Camera Camera, Vector3 bottomEdge, float cameraPosition = Mathf.Infinity){
			return bottomEdge.y + GetViewportCenterInWorldPoint(Camera, bottomEdge.z, cameraPosition).y;
		}

		public static Vector3 GetScreenCenterInWorldPoint (Camera camera)
		{
			Vector3 startPoint = camera.ScreenToWorldPoint (new Vector3 (0, 0, -camera.transform.position.z));
			Vector3 endPoint = camera.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, -camera.transform.position.z));

			Vector3 span = endPoint - startPoint;
			Vector3 centerPoint = span / 2;
			
			return centerPoint;
		}

		public static Vector3 GetViewportCenterInWorldPoint (Camera camera, float targetZ, float cameraZ = Mathf.Infinity)
		{
			if (cameraZ == Mathf.Infinity){
				cameraZ = camera.transform.position.z;
			}

			Vector3 startPoint = camera.ViewportToWorldPoint (new Vector3 (0, 0, -cameraZ + targetZ));
			Vector3 endPoint = camera.ViewportToWorldPoint (new Vector3 (1, 1, -cameraZ + targetZ));

			Vector3 span = endPoint - startPoint;
			Vector3 centerPoint = span / 2;

			return centerPoint;
		}

		public static Vector2 GetAdjustedWorldCenterPoint (Camera camera, Edges rect, float x, float y)
		{
			Vector3 centerCameraPointLeft, centerCameraPointTop, centerCameraPointRight, centerCameraPointBottom;

			centerCameraPointLeft = GetViewportCenterInWorldPoint(camera, rect.left.z);
			centerCameraPointRight = GetViewportCenterInWorldPoint(camera, rect.right.z);
			centerCameraPointTop = GetViewportCenterInWorldPoint(camera, rect.top.z);
			centerCameraPointBottom = GetViewportCenterInWorldPoint(camera, rect.bottom.z);

			if (y < rect.bottom.y + centerCameraPointBottom.y) {
				y = rect.bottom.y + centerCameraPointBottom.y;
			} else if (y > rect.top.y - centerCameraPointTop.y) {
				y = rect.top.y - centerCameraPointTop.y;
			}
			if (x < rect.left.x + centerCameraPointLeft.x) {
				x = rect.left.x + centerCameraPointLeft.x;
			} else if (x > rect.right.x - centerCameraPointRight.x) {
				x = rect.right.x - centerCameraPointRight.x;
			}
			
			return new Vector2 (x, y);


		}
	}

	[Serializable]
	public struct Edges{
		public Vector3 left, right, top, bottom;
	}
}
