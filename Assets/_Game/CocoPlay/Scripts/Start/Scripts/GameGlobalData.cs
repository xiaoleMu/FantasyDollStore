using System;
using System.Collections.Generic;
using CocoPlay;
using LitJson;
using TabTale;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices;
using CocoPlay.Native;

namespace Game
{
    public partial class GameGlobalData : CocoGlobalData
    {
		public string appVersion = "0.3.0";

        private GameRoleStateModel m_RoleStateModel;

	
        public GameGlobalData()
        {
            InitData();
        }
        protected void InitData()
        {
            m_RoleStateModel = CocoRoot.GetInstance<GameRoleStateModel>();

            InitPlayer();
        }
			

		public int m_SceneScore = 0;

        #region Player Data

        private Dictionary<GameRoleID, string> m_RoleIDDic = null;

		public static bool IsIphoneX (){
			return Screen.width/Screen.height > 16f/9f;
		}

		#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern bool IsSafeAreaInsets();
		#endif

		public static bool IsSafeArea (){
			#if UNITY_EDITOR
			return false;
			#elif UNITY_IOS
			return IsSafeAreaInsets ();
			#else
			return false;
			#endif
		}

        public string GetRoleConfigID(GameRoleID pRoleId)
        {
            return m_RoleIDDic.GetValue(pRoleId);
        }

        private List<Pair<GameRoleID, string>> m_MainRoleNames = null;

        public List<Pair<GameRoleID, string>> MainRoleNames
        {
            get { return m_MainRoleNames; }
        }

        private void InitPlayer()
        {
            m_RoleIDDic = new Dictionary<GameRoleID, string>
            {
			{GameRoleID.Coco, "coco"},
            };


            m_MainRoleNames = new List<Pair<GameRoleID, string>>
            {
			new Pair<GameRoleID, string>(GameRoleID.Coco, "coco")
            };
        }

        #endregion

 
        #region GetBgmID

        public CocoAudioID GetSceneBg(CocoSceneID pSceneID)
        {
            switch (pSceneID)
            {
                case CocoSceneID.Map:
                    return CocoAudioID.Bgm_Map;

                default:
                    return CocoAudioID.None;
            }
        }

        #endregion


        #region role

        public List<GameRoleID> LoadedRole = new List<GameRoleID>();

        public GameRoleDB curRoleInfos
        {
            get { return roleInfos(GetRoleId()); }
        }

        public GameRoleDB roleInfos(GameRoleID pGameRoleID)
        {
            GameRoleDB tGameRoleDb = m_RoleStateModel.roleDB.Find(pInfo => pInfo.roleID == pGameRoleID);
//
            if (tGameRoleDb == null)
            {
                tGameRoleDb = new GameRoleDB();
                GameRoleDB tGameRoleBsicDb = m_RoleStateModel.roleBaiscDB.Find(pInfo => pInfo.roleID == pGameRoleID);

                m_RoleStateModel.roleDB.Add(SetRoleDb(tGameRoleDb, tGameRoleBsicDb));

                SaveRoleInfos();
            }
//			Debug.LogError ("name:"+tGameRoleDB.roleID);
//			Debug.LogError (tGameRoleDB.name);
            return tGameRoleDb;
        }

        public GameRoleDB roleBasicInfos(GameRoleID pGameRoleID, bool pIsCreate = false)
        {
            GameRoleDB tGameRoleDb = m_RoleStateModel.roleBaiscDB.Find(pInfo => pInfo.roleID == pGameRoleID);

            if (pIsCreate)
            {
                GameRoleDB tempRoleDb = m_RoleStateModel.roleDB.Find(pInfo => pInfo.roleID == pGameRoleID);
                if (tempRoleDb != null)
                {
                    m_RoleStateModel.roleDB.Remove(tempRoleDb);
                }
                else
                    tempRoleDb = new GameRoleDB();
                m_RoleStateModel.roleDB.Add(SetRoleDb(tempRoleDb, tGameRoleDb));
            }
            return tGameRoleDb;
        }

        private GameRoleDB SetRoleDb(GameRoleDB pOldRoleDb, GameRoleDB pNewRoleDb)
        {
            pOldRoleDb.roleID = pNewRoleDb.roleID;
            pOldRoleDb.name = pNewRoleDb.name;
            pOldRoleDb.eyeball = pNewRoleDb.eyeball;
            pOldRoleDb.eyebrow = pNewRoleDb.eyebrow;
            pOldRoleDb.skincolor = pNewRoleDb.skincolor;
            pOldRoleDb.facesize = pNewRoleDb.facesize;
            pOldRoleDb.eyesize = pNewRoleDb.eyesize;
            pOldRoleDb.nosesie = pNewRoleDb.nosesie;
            pOldRoleDb.mouthsize = pNewRoleDb.mouthsize;
            pOldRoleDb.hairColorIndex = pNewRoleDb.hairColorIndex;
            return pOldRoleDb;
        }

        public void SaveRoleInfos()
        {
            m_RoleStateModel.PublicSave();
        }

        public GameRoleID GetRoleId()
        {
            if (Enum.IsDefined(typeof(GameRoleID), m_RoleStateModel.curRoleId))
            {
                return (GameRoleID) Enum.Parse(typeof(GameRoleID), m_RoleStateModel.curRoleId);
            }

            return GameRoleID.Ava;
        }

	#endregion

    }
    
}