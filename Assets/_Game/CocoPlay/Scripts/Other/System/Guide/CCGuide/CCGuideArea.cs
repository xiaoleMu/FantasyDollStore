using UnityEngine;
using System.Collections;
using TabTale;
public class CCGuideArea : MonoBehaviour 
{
	public RectTransform Collider_Up;
	public RectTransform Collider_Down;
	public RectTransform Collider_Left;
	public RectTransform Collider_Right;

	public void SetArea(Vector3 pos, Vector2 size)
	{
		Vector2 ScreenSize = CCTool.GetWindowSize();
		float ScreenWidth = ScreenSize.x;
		float ScreenHeight = ScreenSize.y;

		transform.position = pos;
		Vector2 halfSize = new Vector2(size.x/2f, size.y/2f);

		Vector3 left_up = new Vector3(pos.x - halfSize.x, pos.y + halfSize.y, 0) + new Vector3(ScreenWidth/2, + ScreenHeight/2, 0);
		Vector3 left_down = new Vector3(pos.x - halfSize.x, pos.y - halfSize.y, 0)+ new Vector3(ScreenWidth/2, + ScreenHeight/2, 0);
		Vector3 right_up = new Vector3(pos.x + halfSize.x, pos.y + halfSize.y, 0)+ new Vector3(ScreenWidth/2, + ScreenHeight/2, 0);
		Vector3 right_down = new Vector3(pos.x + halfSize.x, pos.y - halfSize.y, 0)+ new Vector3(ScreenWidth/2, + ScreenHeight/2, 0);

		//up
		{
			RectTransform pCollider = Collider_Up;
			float cHeight = ScreenHeight-left_up.y;
			pCollider.SetSize(new Vector2(ScreenWidth, cHeight));
			pCollider.transform.SetLocal_Y((size.y + cHeight)/2);
		}
//
//		//down
//		{
//			BoxCollider pCollider = Collider_Down;
//			float cHeight = left_down.y;
//			pCollider.size = new Vector3(ScreenWidth, cHeight, 1);
//			pCollider.center = new Vector3(0, cHeight/2f-ScreenHeight/2, 0);
//		}
//
//		//left
//		{
//			BoxCollider pCollider = Collider_Left;
//			float cWidth = left_up.x;
//			pCollider.size = new Vector3(cWidth, ScreenHeight, 1);
//			pCollider.center = new Vector3(left_up.x-ScreenWidth/2 -cWidth/2, 0, 0);
//		}
//
//		//right
//		{
//			BoxCollider pCollider = Collider_Right;
//			float cWidth = ScreenWidth - right_up.x;
//			pCollider.size = new Vector3(cWidth, ScreenHeight, 1);
//			pCollider.center = new Vector3(right_up.x-ScreenWidth/2 +cWidth/2, 0, 0);
//		}
	}
}
