using System.Collections.Generic;
using System.IO;
using System.Xml;
using LitJson;
using TabTale;
using UnityEditor;
using UnityEngine;

//using GooglePlayGames;

/// <summary>
/// Game center editor tool.
/// -by duhaitao 2017.5.27
/// </summary>
public class GameCenterEditorTool : EditorWindow
{
    private string m_Result = "";
    private string m_Path = "";
    private string m_Progam = "";

    [MenuItem("Coco Common/GameCenter Editor", false, 144)]
    private static void showWindow()
    {
        if (EditorApplication.isPlaying)
            EditorUtility.DisplayDialog("Play Mode", "游戏正在运行中....", "OK");
        else if (EditorInstances.liveInstance != null)
        {
            EditorInstances.liveInstance.Focus();
        }
        else
        {
            EditorWindow window = GetWindow(typeof(GameCenterEditorTool), true, "GameCenter Editor");
            window.minSize = new Vector2(500, 500);
        }
    }

    public void OnGUI()
    {
        GUI.skin.label.wordWrap = true;

        EditorInstances.liveInstance = this;
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("功能说明：设置iOS GameCenter，设置Android GPGS", GUILayout.Width(300));
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("选择XML文件:（XML文件来源-找俊雅到GP后台下载游戏成就）", GUILayout.Width(400));
        EditorGUILayout.BeginHorizontal();
        m_Path = EditorGUILayout.TextField(m_Path, GUILayout.Width(Screen.width - 175));
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            m_Path = EditorUtility.OpenFilePanel("选择", Application.dataPath, "xml");
        }
        if (GUILayout.Button("设置数据", GUILayout.Width(100)))
        {
            Debug.LogError("path : " + m_Path);
            ReadXml(m_Path);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("处理结果：");
        GUILayout.Label(m_Result, GUILayout.Width(Screen.width), GUILayout.Height(65));
        EditorGUILayout.LabelField("【注意】");
        GUILayout.Label(
            "Android GPGS还需要到菜单Window/Google Play Games/Steup/AndroidSetup,把xml文件的全部内容粘贴到中间的文本框，点击下面的Setup进行设置。\n" +
            "原本是可以自动设置的，但是由于项目维护团队的原因，目前只能改回手动再设置一次", GUILayout.Width(Screen.width), GUILayout.Height(35));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("GameCenterData 常量：-> 粘贴到 GameCenterData.cs ", GUILayout.Width(Screen.width * 0.5f));
        GUILayout.Label("Xml内容 -> 粘贴到Android Setup ：", GUILayout.Width(Screen.width * 0.5f));
        EditorGUILayout.EndHorizontal();

//		GUILayout.Label ("GameCenterData 变量：");
        EditorGUILayout.BeginHorizontal();
        GUILayout.TextArea(m_Progam, GUILayout.Width(Screen.width * 0.5f - 10), GUILayout.Height(Screen.height - 250));
        var xmlStr = m_Xml != null ? m_Xml.OuterXml : "";
        GUILayout.TextArea(xmlStr, GUILayout.Width(Screen.width * 0.5f - 10), GUILayout.Height(Screen.height - 250));
        EditorGUILayout.EndHorizontal();
    }

    private XmlDocument m_Xml;
    private void ReadXml(string pAth)
    {
        if (string.IsNullOrEmpty(pAth))
        {
            m_Result = "文件路径不能为空";
            Debug.LogError(m_Result);
            return;
        }

        List<LeaderboardConfigData> allLeaderboards = new List<LeaderboardConfigData>();
        List<AchievementsConfigData> allAchievements = new List<AchievementsConfigData>();
        string leaderboardTableName = new LeaderboardConfigData().GetTableName();
        string achievementTableName = new AchievementsConfigData().GetTableName();

        const string leaderboard_perfix = "leaderboard";
        const string achievement_perfix = "achievement";
        List<string> nodeNames = new List<string>();

        //读取数据
       
        m_Xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings {IgnoreComments = true};
        //这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
        try
        {
            m_Xml.Load(XmlReader.Create(pAth, set));
        }
        catch (IOException e)
        {
            m_Result = "IOException : \n" + e.Message;
            Debug.LogError(e.Message);
            return;
        }
        catch (XmlException e)
        {
            m_Result = "XmlException : \n" + e.Message;
            Debug.LogError(e.Message);
            return;
        }

        //解析数据
        XmlNodeList nodes = m_Xml.SelectSingleNode("resources").ChildNodes;
        foreach (XmlElement node in nodes)
        {
            string nodeName = node.GetAttribute("name");
            string prefix = nodeName.Split('_')[0];
            if (prefix.Equals(leaderboard_perfix))
            {
                nodeNames.Add(nodeName);
                LeaderboardConfigData item_lb = new LeaderboardConfigData();
                item_lb.id = item_lb.leaderboardId = node.InnerText;
                item_lb.name = nodeName;
                item_lb.store = EditorHelpers.storeNames[0];
                allLeaderboards.Add(item_lb);
            }
            else if (prefix.Equals(achievement_perfix))
            {
                nodeNames.Add(nodeName);
                AchievementsConfigData item_ac = new AchievementsConfigData();
                item_ac.id = item_ac.achievementId = node.InnerText;
                item_ac.name = nodeName;
                item_ac.store = EditorHelpers.storeNames[0];
                item_ac.progress = 0;
                allAchievements.Add(item_ac);
            }
        }

        //存储数据
        if (allLeaderboards.Count > 0 || allAchievements.Count > 0)
        {
            string path = "data source=" + Application.streamingAssetsPath + "/DB/game.db";
            CocoSqliteDBHelper db = new CocoSqliteDBHelper(path);

            if (allLeaderboards.Count > 0)
            {
                db.DeleteContents(leaderboardTableName);

                foreach (LeaderboardConfigData itemlb in allLeaderboards)
                {
                    db.InsertInto(leaderboardTableName, new[]
                    {
                        "'" + itemlb.GetId() + "'",
                        "'" + JsonMapper.ToJson(itemlb) + "'"
                    });
                }
            }
            if (allAchievements.Count > 0)
            {
                db.DeleteContents(achievementTableName);
                foreach (AchievementsConfigData itemac in allAchievements)
                {
                    db.InsertInto(achievementTableName, new[]
                    {
                        "'" + itemac.GetId() + "'",
                        "'" + JsonMapper.ToJson(itemac) + "'"
                    });
                }
            }

            m_Result = string.Format("Complete ! \nleaderboard : {0}\nachievement : {1}", allLeaderboards.Count,
                allAchievements.Count);
            Debug.LogError(m_Result.Replace("\n", ""));
            db.CloseSqlConnection();

            CreateProgam(nodeNames);

//			SetGPGSAndroid (xml.OuterXml);
        }
        else
        {
            m_Result = "未能读取到有效的数据";
            Debug.LogError(m_Result);
        }
    }

//	private void SetGPGSAndroid (string mConfigData)
//	{
//		//只需要xml数据就可以了，无需常量类，如果传了常量类名，会自动在Assets下创建一个[类名.cs]文件
//
//		if (GPGSAndroidSetupUI.PerformSetup ("", "", mConfigData, null)) {
//			result += "\n" + GPGSStrings.AndroidSetup.SetupComplete;
//
//			//删除生成的[.cs]常量类文件，没啥用！
//			if (File.Exists ("Assets/.cs")) {
//				File.Delete ("Assets/.cs");
//			}
//		} else {
//			GPGSUtil.Alert (GPGSStrings.Error,
//				"Invalid or missing XML resource data.  Make sure the data is" +
//				" valid and contains the app_id element");
//		}
//	}

    private void CreateProgam(IEnumerable<string> pIds)
    {
        m_Progam = "";
        foreach (string id in pIds)
        {
            m_Progam += string.Format("public const string {0} = \"{1}\";\n", id.ToUpper(), id);
        }
    }

    private void OnDestroy()
    {
        EditorInstances.liveInstance = null;
    }
}