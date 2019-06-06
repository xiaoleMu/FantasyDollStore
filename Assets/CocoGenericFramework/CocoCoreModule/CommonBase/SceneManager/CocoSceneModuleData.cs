using UnityEngine;
using System.Collections;

#if !COCO_FAKE
using CocoSceneID = Game.CocoSceneID;
#else
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#endif

namespace CocoPlay
{
	public class CocoSceneModuleData
	{
		public CocoSceneID sceneId;
		public string sceneName;
		public System.Type moduleType;
		public string moduleAssetPath;

		//Loading加载完成后是否等待场景初始化，true-等待，需要抛信号CocoSceneInitFinishSignal才能跳转，false-不等待，正常流程
		public bool waitInit = false;
		public CocoSceneModuleData (CocoSceneID sceneId, string sceneName, System.Type moduleType = null, string modulePath = null)
		{
			this.sceneId = sceneId;
			this.sceneName = sceneName;
			this.moduleType = moduleType;
			this.moduleAssetPath = modulePath;
		}
	}
}
