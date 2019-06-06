using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class CCTransformExtensions
{
    #region Position

    public static void Set_X(this Transform trans, float x)
    {
        Vector3 pos = trans.position;
        pos.x = x;
        trans.position = pos;
    }

    public static void Set_Y(this Transform trans, float y)
    {
        Vector3 pos = trans.position;
        pos.y = y;
        trans.position = pos;
    }

    public static void Set_Z(this Transform trans, float z)
    {
        Vector3 pos = trans.position;
        pos.z = z;
        trans.position = pos;
    }

    public static void SetLocal_X(this Transform trans, float x)
    {
        Vector3 pos = trans.localPosition;
        pos.x = x;
        trans.localPosition = pos;
    }

    public static void SetLocal_Y(this Transform trans, float y)
    {
        Vector3 pos = trans.localPosition;
        pos.y = y;
        trans.localPosition = pos;
    }

    public static void SetLocal_Z(this Transform trans, float z)
    {
        Vector3 pos = trans.localPosition;
        pos.z = z;
        trans.localPosition = pos;
    }

    public static void AddLocal(this Transform trans, Vector3 offset)
    {
        trans.localPosition += offset;
    }

    public static void AddLocal_X(this Transform trans, float x)
    {
        trans.localPosition += new Vector3(x, 0, 0);
    }

    public static void AddLocal_Y(this Transform trans, float y)
    {
        trans.localPosition += new Vector3(0, y, 0);
    }

    public static void AddLocal_Z(this Transform trans, float z)
    {
        trans.localPosition += new Vector3(0, 0, z);
    }

    public static void Add_X(this Transform trans, float x)
    {
        trans.position += new Vector3(x, 0, 0);
    }

    public static void Add_Y(this Transform trans, float y)
    {
        trans.position += new Vector3(0, y, 0);
    }

    public static void Add_Z(this Transform trans, float z)
    {
        trans.position += new Vector3(0, 0, z);
    }

    public static void Add(this Transform trans, Vector3 add)
    {
        trans.position += add;
    }
    #endregion

    static public void RemoveAllChildren(this Transform target)
    {
		foreach (Transform child in target) {
			GameObject.Destroy (child.gameObject);
		}
//
//        var m_child = new List<Transform>();
//        Transform[] array = target.GetComponentsInChildren<Transform>();
//        foreach (var tran in array)
//        {
//            if (tran.parent == target)
//                m_child.Add(tran);
//        }
//
//        foreach (var child in m_child)
//        {
//            GameObject.Destroy(child.gameObject);
//        }
//        m_child.Clear();
    }

	static void SetZero(this Transform target)
	{
		target.localEulerAngles = Vector3.zero;
		target.localPosition = Vector3.zero;
		target.localScale = Vector3.one;
	}
}
