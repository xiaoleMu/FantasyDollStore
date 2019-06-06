#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using LitJson;
using TabTale;
using Object=UnityEngine.Object;

public class DBTool
{
	public static string LocalPath
	{
		get
		{
			return "data source=" + Application.streamingAssetsPath + "/DB/game.db";
		}
	}

	static public void Write(string SheetName, string TableKey, string TableValue, string DataKey, string DataValue)
	{
		IAPConfigData pIapConfigData = new IAPConfigData ();
		CocoSqliteDBHelper db = new CocoSqliteDBHelper (DBTool.LocalPath);
		int pTableCount = db.FindTable (SheetName, DBTool.LocalPath);
		if (pTableCount == 0)
		{
			db.CreateTable (SheetName, new string[] {TableKey, TableValue}, new string[] {"text", "text"});
		}
		
		SqliteDataReader pDataReader = db.SelectWhere (
			SheetName,
			new string[] {TableValue},
			new string[] {TableKey},
			new string[] {"="},
			new string[] {DataKey});
		
		if (pDataReader.Read ())
		{
			db.Delete (
				SheetName, 
				new string[] {TableKey},
				new string[] {"'" + DataKey + "'"}
			);
		}
		pDataReader.Close ();

		db.InsertInto (SheetName, new string[] {"'" + DataKey + "'", "'" + DataValue + "'"});	
		db.CloseSqlConnection ();	
	}

	static public void DeleteRuntimeDB()
	{
		string pRuntimeDBPath = Application.persistentDataPath + "/DB/game.db";
		if (File.Exists (pRuntimeDBPath))
		{
			Debug.LogWarning ("delete " + pRuntimeDBPath);
			File.Delete (pRuntimeDBPath);
		}
	}
}
#endif
