using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoRay
	{
		#region Ray

		/// <summary>
		/// Ray by screen posiiton from camera
		/// </summary>

		public static Ray RayByScreenPos (Camera camera, Vector3 screenPos)
		{
			return camera.ScreenPointToRay (screenPos);
		}

		public static Ray RayByScreenPos (Vector3 screenPos)
		{
			return RayByScreenPos (Camera.main, screenPos);
		}

		/// <summary>
		/// Ray by view port from camera
		/// </summary>

		public static Ray RayByViewPort (Camera camera, Vector3 virePort)
		{
			return camera.ViewportPointToRay (virePort);
		}

		public static Ray RayByViewPort (Vector3 virePort)
		{
			return RayByViewPort (Camera.main, virePort);
		}

		/// <summary>
		/// Ray by world position from camera
		/// </summary>

		public static Ray RayByWorldPos (Camera camera, Vector3 worldPos)
		{
			return new Ray (camera.transform.position, worldPos - camera.transform.position);
		}

		public static Ray RayByWorldPos (Vector3 worldPos)
		{
			return RayByWorldPos (Camera.main, worldPos);
		}

		#endregion


		#region Raycast (out Hit Info)

		/// <summary>
		/// Raycast by screen position from camera, out hit info
		/// </summary>

		public static bool RaycastByScreenPos (Camera camera, Vector3 screenPos, out RaycastHit hit, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			Ray ray = RayByScreenPos (camera, screenPos);
			return Physics.Raycast (ray, out hit, maxDistance, layerMask);
		}

		public static bool RaycastByScreenPos (Camera camera, Vector3 screenPos, out RaycastHit hit, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByScreenPos (camera, screenPos, out hit, camera.farClipPlane, layerMask);
		}

		public static bool RaycastByScreenPos (Vector3 screenPos, out RaycastHit hit, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByScreenPos (Camera.main, screenPos, out hit, maxDistance, layerMask);
		}

		public static bool RaycastByScreenPos (Vector3 screenPos, out RaycastHit hit, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByScreenPos (Camera.main, screenPos, out hit, layerMask);
		}

		/// <summary>
		/// Raycast by view port from camera, out hit info
		/// </summary>

		public static bool RaycastByViewPort (Camera camera, Vector3 viewPort, out RaycastHit hit, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			Ray ray = RayByViewPort (camera, viewPort);
			return Physics.Raycast (ray, out hit, maxDistance, layerMask);
		}

		public static bool RaycastByViewPort (Camera camera, Vector3 viewPort, out RaycastHit hit, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByViewPort (camera, viewPort, out hit, camera.farClipPlane, layerMask);
		}

		public static bool RaycastByViewPort (Vector3 viewPort, out RaycastHit hit, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByViewPort (Camera.main, viewPort, out hit, maxDistance, layerMask);
		}

		public static bool RaycastByViewPort (Vector3 viewPort, out RaycastHit hit, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByViewPort (Camera.main, viewPort, out hit, layerMask);
		}

		#endregion


		#region Raycast Collider

		/// <summary>
		/// Raycast collider by screen position from camera, return collider
		/// </summary>

		public static Collider RaycastByScreenPos (Camera camera, Vector3 screenPos, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			RaycastHit hit;
			if (RaycastByScreenPos (camera, screenPos, out hit, maxDistance, layerMask)) {
				return hit.collider;
			}
			return null;
		}

		public static Collider RaycastByScreenPos (Camera camera, Vector3 screenPos, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByScreenPos (camera, screenPos, camera.farClipPlane, layerMask);
		}

		public static Collider RaycastByScreenPos (Vector3 screenPos, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByScreenPos (Camera.main, screenPos, maxDistance, layerMask);
		}

		public static Collider RaycastByScreenPos (Vector3 screenPos, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByScreenPos (Camera.main, screenPos, layerMask);
		}


		/// <summary>
		/// Raycast collider by view port from camera, return collider
		/// </summary>

		public static Collider RaycastByViewPort (Camera camera, Vector3 viewPort, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			RaycastHit hit;
			if (RaycastByViewPort (camera, viewPort, out hit, maxDistance, layerMask)) {
				return hit.collider;
			}
			return null;
		}

		public static Collider RaycastByViewPort (Camera camera, Vector3 viewPort, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByViewPort (camera, viewPort, camera.farClipPlane, layerMask);
		}

		public static Collider RaycastByViewPort (Vector3 viewPort, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByViewPort (Camera.main, viewPort, maxDistance, layerMask);
		}

		public static Collider RaycastByViewPort (Vector3 viewPort, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByViewPort (Camera.main, viewPort, layerMask);
		}

		/// <summary>
		/// Raycast collider by world position from camera, return collider
		/// </summary>

		public static Collider RaycastByWorldPos (Camera camera, Vector3 worldPos, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			Ray ray = RayByWorldPos (camera, worldPos);
			Debug.DrawRay (ray.origin, ray.direction * 10f, Color.red, 0.5f);
			return Raycast (ray, maxDistance, layerMask);
		}

		public static Collider RaycastByWorldPos (Camera camera, Vector3 worldPos, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByWorldPos (camera, worldPos, camera.farClipPlane, layerMask);
		}

		public static Collider RaycastByWorldPos (Vector3 worldPos, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByWorldPos (Camera.main, worldPos, maxDistance, layerMask);
		}

		public static Collider RaycastByWorldPos (Vector3 worldPos, int layerMask = Physics.DefaultRaycastLayers)
		{
			return RaycastByWorldPos (Camera.main, worldPos, layerMask);
		}

		#endregion


		#region Raycast Collider (Ray)

		/// <summary>
		/// Raycast collider by ray, return collider
		/// </summary>

		public static Collider Raycast (Ray ray, float maxDistance, int layerMask = Physics.DefaultRaycastLayers)
		{
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, maxDistance, layerMask)) {
				return hit.collider;
			}
			return null;
		}

		public static Collider Raycast (Ray ray, int layerMask = Physics.DefaultRaycastLayers)
		{
			return Raycast (ray, Mathf.Infinity, layerMask);
		}

		#endregion


		#region Ray to Position

		public static Vector3 GetWorldPosByX (Ray cameraRay, float worldPosX)
		{
			float t = (worldPosX - cameraRay.origin.x) / cameraRay.direction.x;
			return cameraRay.GetPoint (t);
		}

		public static Vector3 GetWorldPosByX (Camera camera, Vector3 screenPos, float worldPosX)
		{
			Ray ray = CocoRay.RayByScreenPos (camera, screenPos);
			return GetWorldPosByX (ray, worldPosX);
		}

		public static Vector3 GetWorldPosByX (Vector3 screenPos, float worldPosX)
		{
			return GetWorldPosByX (Camera.main, screenPos, worldPosX);
		}

		public static Vector3 GetWorldPosByY (Ray cameraRay, float worldPosY)
		{
			float t = (worldPosY - cameraRay.origin.y) / cameraRay.direction.y;
			return cameraRay.GetPoint (t);
		}

		public static Vector3 GetWorldPosByY (Camera camera, Vector3 screenPos, float worldPosY)
		{
			Ray ray = CocoRay.RayByScreenPos (camera, screenPos);
			return GetWorldPosByY (ray, worldPosY);
		}

		public static Vector3 GetWorldPosByY (Vector3 screenPos, float worldPosY)
		{
			return GetWorldPosByY (Camera.main, screenPos, worldPosY);
		}

		public static Vector3 GetWorldPosByZ (Ray cameraRay, float worldPosZ)
		{
			float t = (worldPosZ - cameraRay.origin.z) / cameraRay.direction.z;
			return cameraRay.GetPoint (t);
		}

		public static Vector3 GetWorldPosByZ (Camera camera, Vector3 screenPos, float worldPosZ)
		{
			Ray ray = CocoRay.RayByScreenPos (camera, screenPos);
			return GetWorldPosByZ (ray, worldPosZ);
		}

		public static Vector3 GetWorldPosByZ (Vector3 screenPos, float worldPosZ)
		{
			return GetWorldPosByZ (Camera.main, screenPos, worldPosZ);
		}

		#endregion


		#region Bounds

		public static bool IsRayInTargetBounds (Ray ray, Transform target, Bounds bounds)
		{
			Vector3 center = target.TransformPoint (bounds.center);
			Bounds worldBounds = new Bounds (center, bounds.size);
			return worldBounds.IntersectRay (ray);
		}

		#endregion

		public static Vector3 ConvertCameraPos (Vector3 fromPos, Camera fromCamera, Camera toCamera)
		{
			Vector3 pos = fromCamera.WorldToScreenPoint (fromPos);
//			if (Mathf.Abs (pos.z) < toCamera.nearClipPlane) {
//				pos.z = toCamera.nearClipPlane * Mathf.Sign (pos.z);
//			}
//			Debug.LogError (pos);
			pos = toCamera.ScreenToWorldPoint (pos);
//			Debug.LogError (fromPos + ", " + pos.x + ", " + pos.y + ", " + pos.z + ", " + fromCamera.name + ", " + toCamera.name);
			return pos;
		}
	}
}
