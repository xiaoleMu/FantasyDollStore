using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TabTale
{
    public enum SoundLayer { Music, Main }
    public class SoundMapping : ScriptableObject 
    {
        /// Implement this in your derived class to create the instance for custom mapping 
        /// Don't forget to put the file generated (CustomSoundMapping.asset) in the Resources Folder
        /// 
		/* 
		#if UNITY_EDITOR
		[MenuItem("Assets/Create/SoundMapping")]
		public static void CreateAsset()
		{
			ScriptableObjectUtility.CreateAsset<YourDerivedClass>("CustomSoundMapping");
		}
		#endif
		*/

        private static SoundMapping _map;

        public static SoundMapping Map
        {
            get
            {
                if (_map == null)
                    _map = Resources.Load<ScriptableObject>("CustomSoundMapping") as SoundMapping;

                if (_map == null)
                    _map = Resources.Load<ScriptableObject>("SoundMapping") as SoundMapping;

                return _map;
            }
        }

        #region Generic Sounds

        public List<string> GeneralButtonClick  = new List<string>() {"general_button"};
		public List<string> MenuSlideIn         = new List<string>() {"menu_slide_in"};
		public List<string> MenuSlideOut        = new List<string>() {"menu_slide_out"};
		public List<string> MatchStart          = new List<string>() {"match_start"};
		public List<string> MatchEndList        = new List<string>() {"match_start"};
		#endregion
	

        public static string GetSoundClipPathForEventType(List<string> soundList)
        {
            return "Sounds/"+soundList.GetRandomItem();
        }

		public static string GetSoundClipPathForEventType(string soundName)
		{
			return "Sounds/"+soundName;
		}
    }
}