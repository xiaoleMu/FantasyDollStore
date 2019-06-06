using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorTool : MonoBehaviour
{

    /// <summary>
    /// 菜单方法，遍历文件夹创建Animation Controller
    /// </summary>
//    [MenuItem("Tools/CreateAnimator")]
//    static void CreateAnimationAssets()
//    {
//        string rootFolder = "Assets/_Game/Models/Character/Animator";
//        string common_path = "Assets/_Game/Models/Character/Animator/common";
//        string resources_path = string.Format("{0}/Resources", rootFolder);
//        if (!Directory.Exists(resources_path))
//        {
//            Directory.CreateDirectory(resources_path);
//        }
//
//        // 遍历目录，查找生成controller文件
//        var folders = Directory.GetDirectories(rootFolder);
//        foreach (var folder in folders)
//        {
//            DirectoryInfo info = new DirectoryInfo(folder);
//            string scene_name = info.Name;
//
//            if (scene_name == "Resources")
//                continue;
//
//            if (scene_name.Contains("withoutcommon"))
//                continue;
//
//            string animator_name = string.Format("{0}/Resources/{1}_animator.controller", rootFolder, scene_name);
//            string search_path = string.Format("{0}/{1}", rootFolder, scene_name);
//
//            // 创建animationController文件
//            AnimatorController animator = AnimatorController.CreateAnimatorControllerAtPath(animator_name);
//
//            AnimatorStateMachine animatorLayer = animator.layers[0].stateMachine;
//            // 先添加一个默认的空状态
//            var emptyState = animatorLayer.AddState("empty");
//            animatorLayer.AddAnyStateTransition(emptyState);
//            //添加共同的动画
//            AddStateTranstion(common_path, animatorLayer);
//            //添加不同的动画
//            if (scene_name != "common")
//                AddStateTranstion(search_path, animatorLayer);
//
//            // 创建预设
//            //            GameObject go = LoadFbx(folderName);
//            //            PrefabUtility.CreatePrefab(string.Format("{0}/{1}.prefab", folder, folderName), go);
//            //            DestroyImmediate(go);
//        }
//    }
//
//    [MenuItem("Tools/CreateAnimator_WithoutCommon")]
//    static void CreateAnimationAssetsWithoutCommon()
//    {
//        string rootFolder = "Assets/_Game/Models/Character/Animator";
//        string resources_path = string.Format("{0}/Resources", rootFolder);
//        if (!Directory.Exists(resources_path))
//        {
//            Directory.CreateDirectory(resources_path);
//        }
//
//        // 遍历目录，查找生成controller文件
//        var folders = Directory.GetDirectories(rootFolder);
//        foreach (var folder in folders)
//        {
//            var info = new DirectoryInfo(folder);
//            string scene_name = info.Name;
//
//            if (scene_name == "Resources")
//                continue;
//
//            if (!scene_name.Contains("withoutcommon_"))
//                continue;
//
//            string search_path = string.Format("{0}/{1}", rootFolder, scene_name);
//            scene_name = scene_name.Replace("withoutcommon_", "");
//            string animator_name = string.Format("{0}/Resources/{1}_animator.controller", rootFolder, scene_name);
//            AnimatorController animator = AnimatorController.CreateAnimatorControllerAtPath(animator_name);
//
//            AnimatorStateMachine animatorLayer = animator.layers[0].stateMachine;
//            // 先添加一个默认的空状态
//            var emptyState = animatorLayer.AddState("empty");
//            animatorLayer.AddAnyStateTransition(emptyState);
//            AddStateTranstion(search_path, animatorLayer);
//        }
//    }
//
    /// <summary>
    /// 添加动画状态机状态
    /// </summary>
    /// <param name="path"></param>
    /// <param name="layer"></param>
	private static void AddStateTranstion(string path, AnimatorStateMachine animatorLayer)
    {
        var fileinfos = Directory.GetFiles(path);
        foreach (string fileName in fileinfos)
        {
			if(fileName.Contains(".DS_Store"))
				continue;
            var datas = AssetDatabase.LoadAllAssetsAtPath(fileName);
            if (datas.Length == 0)
            {
                continue;
            }

            // 遍历模型中包含的动画片段，将其加入状态机中
            foreach (var data in datas)
            {
                if (!(data is AnimationClip))
                    continue;
                var newClip = data as AnimationClip;
                if (newClip.name.StartsWith("__"))
                    continue;
                // 取出动画名字，添加到state里面
                var state = animatorLayer.AddState(newClip.name);
                state.motion = newClip;
                // 把State添加在Layer里面
                animatorLayer.AddAnyStateTransition(state);
            }
        }
    }

    /// <summary>
    /// 生成带动画控制器的对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject LoadFbx(string name)
    {
        var obj = Instantiate(Resources.Load(string.Format("fbx/{0}/{0}_model", name))) as GameObject;
        obj.GetComponent<Animator>().runtimeAnimatorController =
            Resources.Load<RuntimeAnimatorController>(string.Format("fbx/{0}/animation", name));
        return obj;
    }


	[MenuItem ("CocoPlay/Animator/Path Test", false, 55)]
	static void PathTest()
	{
		UnityEngine.Object[] arr=Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
		Debug.LogError(Application.dataPath);
		Debug.LogError(Application.dataPath.Substring(0,Application.dataPath.LastIndexOf('/')));

		string dataPath = Application.dataPath;
		int start = dataPath.LastIndexOf('/')+1;
		int end = dataPath.Length -start;
		Debug.LogError(Application.dataPath.Substring(start, end));

		string allPath = Application.dataPath.Substring(0,Application.dataPath.LastIndexOf('/'))+"/"+ AssetDatabase.GetAssetPath(arr[0]);
		Debug.LogError(allPath);

		string curPath = AssetDatabase.GetAssetPath(arr[0]);
		Debug.LogError(curPath);
	}

//	[MenuItem ("CocoPlay/Animator/Create with common", false, 21)]
//	static void CreateAnimatorWithCommon()
//	{
////		UnityEngine.Object[] GameobjectAsset = Selection.GetFiltered(typeof(UnityEngine.GameObject), SelectionMode.DeepAssets);
////		UnityEngine.Object[] objectAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
////		if(GameobjectAsset.Length == 0)
////		{
////			Debug.LogError("directory is empty : " +  AssetDatabase.GetAssetPath(objectAsset[0]));
////			return;
////		}
//
//		UnityEngine.Object[] arr=Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
//		string curPath = AssetDatabase.GetAssetPath(arr[0]);
//
//		int start = curPath.LastIndexOf('/')+1;
//		string up_path = curPath.Substring(0, start-1);
//		string directory = curPath.Substring(start, curPath.Length -start);
//
//		string resources_path = up_path + "/Resources";
//		Debug.LogError(resources_path);
//
//		string common_path = up_path + "/common";
//		if(!Directory.Exists(common_path))
//		{
//			Debug.LogError("common path error: " + common_path);
//			return;
//		}
//
//		if (!Directory.Exists(resources_path))
//		{
//			Directory.CreateDirectory(resources_path);
//		}
//
//
//		string animator_name = string.Format("{0}/{1}_animator.controller", resources_path, directory);
//
//		// 创建animationController文件
//		AnimatorController animator = AnimatorController.CreateAnimatorControllerAtPath(animator_name);
//
//		AnimatorStateMachine animatorLayer = animator.layers[0].stateMachine;
//		// 先添加一个默认的空状态
//		var emptyState = animatorLayer.AddState("empty");
//		animatorLayer.AddAnyStateTransition(emptyState);
//		//添加共同的动画
//		AddStateTranstion(common_path, animatorLayer);
//		//添加不同的动画
//		AddStateTranstion(curPath, animatorLayer);
//		AssetDatabase.SaveAssets();
//	}

    [MenuItem("Assets/Create Animator", false, 3001)]
    static void CreateAnimator()
    {
        CreateAnimatorWithOutCommon();
    }

    [MenuItem ("CocoPlay/Animator/Create", false, 56)]
	static void CreateAnimatorWithOutCommon()
	{
		UnityEngine.Object[] arr=Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
		string curPath = AssetDatabase.GetAssetPath(arr[0]);

		int start = curPath.LastIndexOf('/')+1;
		string up_path = curPath.Substring(0, start-1);
		string directory = curPath.Substring(start, curPath.Length -start);

		string resources_path = up_path + "/Resources";

		if (!Directory.Exists(resources_path))
		{
			Directory.CreateDirectory(resources_path);
		}


		string animator_name = string.Format("{0}/{1}_animator.controller", resources_path, directory);

		// 创建animationController文件
		AnimatorController animator = AnimatorController.CreateAnimatorControllerAtPath(animator_name);

		AnimatorStateMachine animatorLayer = animator.layers[0].stateMachine;
		// 先添加一个默认的空状态
		var emptyState = animatorLayer.AddState("empty");
		animatorLayer.AddAnyStateTransition(emptyState);
		//添加不同的动画

	    foreach (var path in arr)
	    {
	        AddStateTranstion(AssetDatabase.GetAssetPath(path), animatorLayer);
	    }

		AssetDatabase.SaveAssets();
	}
}
