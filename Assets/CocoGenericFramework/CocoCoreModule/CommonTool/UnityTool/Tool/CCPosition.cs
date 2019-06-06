using UnityEngine;
using System.Collections;

public class CCPosition
{
	#region Position

	public static void Set_X(Transform trans, float x)
	{
		Vector3 pos = trans.position;
		pos.x = x;
		trans.position = pos;
	}

	public static void Set_Y(Transform trans, float y)
	{
		Vector3 pos = trans.position;
		pos.y = y;
		trans.position = pos;
	}

	public static void Set_Z(Transform trans, float z)
	{
		Vector3 pos = trans.position;
		pos.z = z;
		trans.position = pos;
	}

	public static void SetLocal_X(Transform trans, float x)
	{
		Vector3 pos = trans.localPosition;
		pos.x = x;
		trans.localPosition = pos;
	}

	public static void SetLocal_Y(Transform trans, float y)
	{
		Vector3 pos = trans.localPosition;
		pos.y = y;
		trans.localPosition = pos;
	}

	public static void SetLocal_Z(Transform trans, float z)
	{
		Vector3 pos = trans.localPosition;
		pos.z = z;
		trans.localPosition = pos;
	}

	public static void AddLocal(Transform trans, Vector3 offset)
	{
		trans.localPosition += offset;
	}

	public static void AddLocal_X(Transform trans, float x)
	{
		trans.localPosition += new Vector3(x, 0, 0);
	}

	public static void AddLocal_Y(Transform trans, float y)
	{
		trans.localPosition += new Vector3(0, y, 0);
	}

	public static void AddLocal_Z(Transform trans, float z)
	{
		trans.localPosition += new Vector3(0, 0, z);
	}

	public static void Add_X(Transform trans, float x)
	{
		trans.position += new Vector3(x, 0, 0);
	}

	public static void Add_Y(Transform trans, float y)
	{
		trans.position += new Vector3(0, y, 0);
	}

	public static void Add_Z(Transform trans, float z)
	{
		trans.position += new Vector3(0, 0, z);
	}

	public static void Add(Transform trans, Vector3 add)
	{
		trans.position += add;
	}

	#endregion
}

public class CCEulerAngles
{
	public static void Set_Z(Transform trans, float z)
	{
		Vector3 ang = trans.localEulerAngles;
		ang.z = z;
		trans.localEulerAngles = ang;
	}
}
