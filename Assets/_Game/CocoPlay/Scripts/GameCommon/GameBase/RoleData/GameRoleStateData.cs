using UnityEngine;
using System.Collections;
using TabTale;
using Game;
using System.Collections.Generic;

public class GameRoleStateData : IStateData
{
	public GameRoleID curGameRoleId = GameRoleID.Coco;
	public string curRoleId = "Coco";
	public int curGameRoleIndex = 0;
	public List<GameRoleDB> roleDB = new List<GameRoleDB> ();
	public List<GameRoleDB> roleBaiscDB = new List<GameRoleDB> ();
	public GameRoleDB selectRoleDB = new GameRoleDB ();
	public Dictionary<string, string> roleDataDic = new Dictionary<string, string> ();

    public bool roleNameSeted = false;
	#region base

	public string GetStateName ()
	{
		return "GameRoleState";
	}

	public override string ToString ()
	{
		return string.Format ("GameRoleState");
	}

	public string ToLogString ()
	{
		return string.Format ("GameRoleStateData");
	}

	public IStateData Clone ()
	{
		GameRoleStateData data = new GameRoleStateData ();
		return data;
	}

	#endregion
}
