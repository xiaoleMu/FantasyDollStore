using UnityEngine;
using System.Collections;
using Game;

public class GameRoleDB
{
	/// <summary>
	/// 主键，确定唯一角色
	/// 不可修改
	/// </summary>
	public GameRoleID roleID = GameRoleID.Coco;
	/// <summary>
	/// 名字
	/// 修改数据一定要调用gameGlobalData.SaveRoleInfos ();
	/// </summary>
	public string name = "Coco";

	//Home相关数据
	public int eyeball = 1;
	public int eyebrow = 0;
	public int skincolor = 1;
	public int facesize = 2;
	public int eyesize = 0;
	public int nosesie = 2;
	public int mouthsize = 4;
	public int headsize = 0;
	//头发颜色序号
	public int hairColorIndex = -1;

    public static GameRoleDB RandomRoleDB(){

        GameRoleDB data = new GameRoleDB();
        data.eyeball = CCRandom.Range(0, 12);
        data.eyebrow = CCRandom.Range(0, 12);
        data.skincolor = CCRandom.Range(0, 8);
        data.facesize = CCRandom.Range(0, 6);
        data.eyesize = CCRandom.Range(0, 6);
        data.nosesie = CCRandom.Range(0, 6);
		data.mouthsize = CCRandom.Range(0, 6);
        data.hairColorIndex = CCRandom.Range(1, 22);
        return data;
    }

	public static GameRoleDB RandomBattlerOrCrewDB()
	{
		GameRoleDB data = new GameRoleDB();
		data.eyeball = CCRandom.Range(0, 12);
		data.skincolor = CCRandom.Range(0, 8);
		data.hairColorIndex = CCRandom.Range(1, 22);
		return data;
	}
}
