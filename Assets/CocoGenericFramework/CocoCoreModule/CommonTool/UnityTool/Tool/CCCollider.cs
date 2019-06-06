using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCCollider
{
	public static RaycastHit GetTouchHit()
	{
		RaycastHit pHit;
		Ray pRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(pRay, out pHit);
		return pHit;
	}

	public static RaycastHit GetTouchHit(Vector3 ScreenPos)
	{
		RaycastHit pHit;
		Ray pRay = Camera.main.ViewportPointToRay(ScreenPos);
		Physics.Raycast(pRay, out pHit);
		return pHit;
	}

	public static Vector3 GetColliderPos(Collider pCollider, Vector3 ScreenPos)
	{
		RaycastHit hit = GetColliderHit(pCollider, ScreenPos);
		if(hit.collider != null)
		{
			return hit.point;
		}
		return Vector3.zero;
	}

	public static Vector3 GetColliderPos(Camera camera, Collider pCollider, Vector3 ScreenPos)
	{
		RaycastHit hit = GetColliderHit(camera, pCollider, ScreenPos);
		if(hit.collider != null)
		{
			return hit.point;
		}
		return Vector3.zero;
	}

	public static RaycastHit GetColliderHit(Collider pCollider, Vector3 ScreenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(ScreenPos);
		RaycastHit hit;
		pCollider.Raycast(ray, out hit, 100.0f);
		return hit;
	}

	public static RaycastHit GetColliderHit(Camera camera, Collider pCollider, Vector3 ScreenPos)
	{
		Ray ray = camera.ScreenPointToRay(ScreenPos);
		RaycastHit hit;
		pCollider.Raycast(ray, out hit, 1000.0f);
		return hit;
	}

	public static bool GetHitEnable(Collider pCollider, Vector3 ScreenPos, Camera camera)
	{
		RaycastHit hit = GetColliderHit(camera, pCollider, ScreenPos);
		return hit.collider != null;
	}

	public static bool GetHitEnable(Collider pCollider, Vector3 ScreenPos)
	{
		return GetColliderHit(Camera.main, pCollider, ScreenPos).collider != null;
	}


	public static List<RaycastHit> GetHitList(Vector3 ScreenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(ScreenPos);
		RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
		List<RaycastHit> pHits = new List<RaycastHit>(hits);
		return pHits;
	}

	public static List<RaycastHit> GetHitList<T>(Vector3 ScreenPos) where T : MonoBehaviour
	{
		Ray ray = Camera.main.ScreenPointToRay(ScreenPos);
		RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
		List<RaycastHit> pHits = new List<RaycastHit>(hits);
		List<RaycastHit> pReturnHits = new List<RaycastHit>(hits);
		foreach(var pHit in pHits)
		{
			T pComponent = pHit.transform.GetComponent<T>();
			if(pComponent == null)
				pReturnHits.Remove(pHit);
		}
		return pReturnHits;
	}

	public static T GetComponent<T>(Camera camera, Vector3 ScreenPos) where T : MonoBehaviour
	{
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(ScreenPos);
		Physics.Raycast(ray, out hit, 100);
		if(hit.collider == null)
			return null;

		return hit.collider.gameObject.GetComponent<T>();
	}

	public static List<T> GetComponents<T>(Camera camera, Vector3 ScreenPos) where T : MonoBehaviour
	{
		Ray ray = camera.ScreenPointToRay(ScreenPos);
		RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
		List<RaycastHit> pHits = new List<RaycastHit>(hits);
		List<T> pReturn = new List<T>();
		foreach(var pHit in pHits)
		{
//			T pComponent = pHit.transform.GetComponentInChildren<T>();
			T pComponent = pHit.transform.GetComponent<T>();
			if(pComponent != null)
				pReturn.Add(pComponent);
		}
		return pReturn;
	}

	static public bool IsTouchCollider(Camera _camera, Collider pCollider)
	{
		RaycastHit hit = GetColliderHit(_camera, pCollider, Input.mousePosition);
		if(hit.collider != null)
			return true;
		else
			return false;
	}

	static public bool IsTouchCollider(Camera _camera, Collider pCollider, Vector3 screenPosition)
	{
		RaycastHit hit = GetColliderHit(_camera, pCollider, screenPosition);
		if(hit.collider != null)
			return true;
		else
			return false;
	}

	static public bool IsTouchUICollider(Collider collider, Vector3 UIWordPosition)
	{
		Vector3 origin = UIWordPosition + Vector3.back * 100;
		Vector3 direction = Vector3.forward;
		Ray ray = new Ray(origin, direction);
		RaycastHit hit;
		collider.Raycast(ray, out hit, 200.0f);
		return hit.collider != null;
	}
}
