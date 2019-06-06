using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace TabTale
{
	public class TouchUtils : MonoBehaviour
	{
		public static bool IsTouchingUGUI ()
		{
			try {
				if (EventSystem.current.IsPointerOverGameObject ()) {
					return true;
				}
				
				if (Input.touchCount > 0) {
					foreach (Touch item in Input.touches) {
						if (EventSystem.current.IsPointerOverGameObject (item.fingerId)) {
							return true;
						}
					}
				}
			} catch (System.Exception ex) {
				CoreLogger.LogError(ex.ToString());
			}

			return false;
		}

		public static Vector3 TouchPointToWorldPoint (Camera contentCamera)
		{
			float posX = 0, posY = 0;
			
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
				posX = Input.touches [0].position.x;
				posY = Input.touches [0].position.y;
			} else {
				posX = Input.mousePosition.x;
				posY = Input.mousePosition.y;
			}
			
			Vector3 vetor3 = new Vector3 (posX, posY, Mathf.Abs (contentCamera.transform.position.z));
			
			return contentCamera.ScreenToWorldPoint (vetor3);
		}
	}

}