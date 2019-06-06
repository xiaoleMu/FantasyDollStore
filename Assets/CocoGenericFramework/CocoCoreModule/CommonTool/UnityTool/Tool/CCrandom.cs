using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CCRandom
{
	static public bool Bool
	{
		get { return UnityEngine.Random.Range(0, 2f) >= 1; }
	}

	static public int Range(int min, int max)
	{
		UnityEngine.Random.seed = GetRandomSeed();
		return UnityEngine.Random.Range(min, max);
	}

	static public float Range(float min, float max)
	{
		UnityEngine.Random.seed = GetRandomSeed();
		return UnityEngine.Random.Range(min, max);
	}

	static public int Range()
	{
		System.Random rd = new System.Random(GetRandomSeed());
		return rd.Next();
	}

	static public int GetRandomSeed()
	{
		byte[] bytes = new byte[4];
		System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
		rng.GetBytes(bytes);
		return BitConverter.ToInt32(bytes, 0);
	}

	public static int RandomByWeight(params int[] weightList)
	{
		int length = weightList.Length;
		int[] dataArr = new int[length];
		int total = 0;
		int returnIndex = 0;
		for (int i = 0; i < length; i++)
		{
			total += weightList[i];
			dataArr[i] = total;
		}
		int randomData = UnityEngine.Random.Range(0, total);
		for (int i = 0; i < length; i++)
		{
			if (randomData < dataArr[i])
			{
				returnIndex = i;
				break;
			}

		}
		return returnIndex;
	}
}
