using UnityEngine;
using System.Collections;
using TabTale;
using System.Collections.Generic;
using Game;

public class GameRoleStateModel : StateModel<GameRoleStateData>
{
	public List<GameRoleDB> roleDB {
		get {
			return _data.roleDB;
		}
	}

	public List<GameRoleDB> roleBaiscDB {
		get {
			return _data.roleBaiscDB;
		}
	}

	/// <surmary>
	/// 当前设置，角色id始终都是"coco",此处数据存的是多角色ID
	/// </surmary>
	public GameRoleID curGameRoleId { 
		get {
			return _data.curGameRoleId;
		}
		set {
			_data.curGameRoleId = value;
			Save ();
		}
	}
	
	public int CurGameRoleIndex {
		get {
			return _data.curGameRoleIndex;
		}
		set {
			_data.curGameRoleIndex = value;
			Save ();
		}
	}

	public string curRoleId { 
		get {
			return _data.curRoleId;
		}
		set {
			_data.curRoleId = value;
			Save ();
		}
	}

	public void UpdateRoleData (string pRoleId, string pRoleData)
	{
		if (_data.roleDataDic.ContainsKey (pRoleId)) {
			_data.roleDataDic [pRoleId] = pRoleData;
		} else {
			_data.roleDataDic.Add (pRoleId, pRoleData);
		}
		Save ();
	}

	public Dictionary<string, string> roleDataDic {
		get {
			return _data.roleDataDic;
		}
	}

	public GameRoleDB selectRoleDB{
		get {
			return _data.selectRoleDB;
		}
	}

    public bool IsNameSet
    {
        get {
            return _data.roleNameSeted;
        }
        
        set {
            _data.roleNameSeted = value;
        }
    }

	public void PublicSave ()
	{
		Save ();
	}
}
