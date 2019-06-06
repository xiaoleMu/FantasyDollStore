using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace TabTale 
{
	public class ResetPlayerGUI
	{	
		private const string keyPlayerId = "playerId";

		[MenuItem("TabTale/GSDK/Reset Local Player &r")]
		public static void ResetLocalPlayerPrefsAndDatabase () 
		{
			ResetPlayerId();
			RemoveLocalDatabase();
		}

		private static void ResetPlayerId()
		{
			Debug.Log ("ResetPlayer: Setting playerId to 0");
			TTPlayerPrefs.SetValue(keyPlayerId,"0");

		}

		private static void RemoveLocalDatabase()
		{
			string dbFolder = Application.persistentDataPath + "/DB/";
			string dbName = "game.db";
			string dbPath = dbFolder + dbName;

			Debug.Log ("ResetPlayer: Deleting local db file from " + dbPath);
			File.Delete(dbPath);
		}

				
		[MenuItem("TabTale/GSDK/Reset PlayerPrefs and InAppPurchases &f")]
		public static void ResetAllPlayerPrefs() 
		{
			Debug.Log ("Resetting all player preferences & In app purchases");
			TTPlayerPrefs.DeleteAll();
		}
	}
}