using UnityEngine;
using System.Collections;


namespace CocoPlay
{
	public class CocoSceneModuleBase : CocoModuleBase
	{
		CocoGenericSceneBase m_SceneManager = null;

		public CocoGenericSceneBase SceneManager {
			get {
				return m_SceneManager;
			}
			set {
				m_SceneManager = value;
			}
		}

		CocoSceneModuleData m_Data = null;

		public CocoSceneModuleData Data {
			get {
				return m_Data;
			}
			set {
				m_Data = value;
			}
		}
	}
}
