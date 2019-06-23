using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CocoPlay;
using TabTale;
using UnityEngine.UI;

namespace Game
{
    public class GameGenericSceneBase : CocoGenericSceneBase
    {
        [Inject]
        public GameGlobalData gameGlobalData { get; set; }

        [Inject]
        public CocoRoleControl roleControl { get; set; }

        [Inject]
        public CocoPopupClosedSignal popupCloseSignal { get; set; }

        [Inject]
        public CocoRemovePopupSignal m_CloseSignal { get; set; }

        [Inject]
        public GameRecordStateModel recordStateModel { get; set; }

        public CocoSceneID nextSceneId = CocoSceneID.Map;
        public GameObject nextButton;

        protected CocoRoleEntity m_CurRole;

        [HideInInspector] public bool isCreateNewCharacter = false;

        protected bool sceneisLockBase;

        protected override void initCharacter()
        {
            base.initCharacter();

            m_CurRole = CreateOrGetMainRole();
            m_CurRole.gameObject.SetActive(false);
            roleControl.CurRole = m_CurRole;

            SetNextButtonStatus(false);
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            popupCloseSignal.AddListener(OnPopupClose);
            locationClosedSignal.AddListener(OnLocationClosed);
        }

        protected override void RemoveListeners()
        {
            popupCloseSignal.RemoveListener(OnPopupClose);
            locationClosedSignal.RemoveListener(OnLocationClosed);
            base.RemoveListeners();
        }

        protected CocoRoleEntity CreateOrGetMainRole()
        {
            Pair<GameRoleID, string> roleName =
                gameGlobalData.MainRoleNames.Find(pRole => pRole.First == gameGlobalData.GetRoleId());
            GameRoleID roleID = roleName.First;
            var tRoleEntity = roleControl.GetOrCreateMainRole(gameGlobalData.GetRoleConfigID(roleID), roleName.Second);
            tRoleEntity.transform.localPosition = Vector3.zero;
            tRoleEntity.transform.localEulerAngles = Vector3.zero;

//            tRoleEntity.Shadow.EnableFollowFreezeY(0.002f);

            if (isCreateNewCharacter)
            {
                if (!gameGlobalData.LoadedRole.Contains(roleID))
                    gameGlobalData.LoadedRole.Add(roleID);
                isCreateNewCharacter = false;
            }
            else if (!gameGlobalData.LoadedRole.Contains(roleID))
            {
                gameGlobalData.LoadedRole.Add(roleID);
            }

            return tRoleEntity;
        }


        protected override void cleanCharacter()
        {
            m_CurRole.gameObject.SetActive(false);

            if (m_CurRole.Animation != null)
            {
                m_CurRole.Animation.animator.runtimeAnimatorController = null;
            }

            base.cleanCharacter();
        }

        void ResetRoleTransform(CocoRoleEntity pRole)
        {
            CocoLoad.SetParent(pRole, pRole.transform.parent);
        }


        protected override void OnLoadingFinished()
        {
            base.OnLoadingFinished();
            CocoAudio.StopBgMusic();
            CocoAudio.PlayBgMusic(gameGlobalData.GetSceneBg(gameGlobalData.CurSceneID));
        }

        
        protected virtual void OnPopupClose(string key)
        {
        }

        protected virtual void OnPurchased()
        {
        }

        protected override void PlayStart()
        {
            base.PlayStart();
        }

        protected override void PlayEnd()
        {
            m_CurRole.gameObject.SetActive(false);
            m_CurRole.transform.localPosition = Vector3.zero;
            m_CurRole.transform.localEulerAngles = Vector3.zero;

            m_CurRole.Animation.SetAutoSwithEnable(false);
//            m_CurRole.Animation.animator.Stop();

            m_CurRole.Animation.animator.runtimeAnimatorController = null;


            CocoAudio.StopBgMusic();

            m_CloseSignal.Dispatch();

            base.PlayEnd();
        }

        #region UI Event

        protected override void OnButtonClickWithButtonId(CocoUINormalButton button, CocoUIButtonID buttonID)
        {
            switch (buttonID)
            {
                case CocoUIButtonID.Common_Complete:
                    CocoMainController.EnterScene(nextSceneId);
                    break;
                default:
                    base.OnButtonClickWithButtonId(button, buttonID);
                    break;
            }
        }

        protected override void OnButtonClickWithButtonName(CocoUINormalButton button, string pButtonName)
        {
            switch (pButtonName)
            {
                case "BubbleButton":
                    CocoMainController.EnterScene(nextSceneId);
                    break;
                default:
                    base.OnButtonClickWithButtonName(button, pButtonName);
                    break;
            }
        }

        #endregion


        #region Commom Tool

        protected IEnumerator MakeTransFaceTarget(Transform pTrans, Transform pTarget, float pTime = 0.5f)
        {
            LeanTween.cancel(pTrans.gameObject);

            Vector3 pDir = pTarget.position - pTrans.position;
            pDir.y = 0;
            Quaternion pRotation = Quaternion.LookRotation(pDir);

            LeanTween.rotate(pTrans.gameObject, pRotation.eulerAngles, pTime);
            yield return new WaitForSeconds(pTime);
        }

        protected void MakeTransFaceTargetTrans(Transform pTrans, Transform pTarget)
        {
            LeanTween.cancel(pTrans.gameObject);

            Vector3 pDir = pTarget.position - pTrans.position;
            pDir.y = 0;
            Quaternion pRotation = Quaternion.LookRotation(pDir);

            pTrans.rotation = pRotation;
        }

        #endregion

        #region 对勾按钮动画 与 切换场景

        public Image bubbleImage;
        public Sprite showSprite;
        public GameObject bubble;

        protected void SetNextButtonStatus(bool pEnable)
        {
            if (nextButton == null || nextButton.activeSelf == pEnable)
                return;

            nextButton.SetActive(pEnable);
            if (pEnable)
            {
//                CocoAudio.PlaySound(CocoAudioID.Button_Click_16);
                nextButton.transform.localScale = Vector3.zero;
                LeanTween.scale(nextButton, Vector3.one * 1f, 1f).setEase(LeanTweenType.easeOutElastic)
                    .setOnComplete(() => { LeanTween.scale(nextButton, Vector3.one * 1.1f, 1f).setLoopPingPong(); });
            }
        }

        protected virtual void SceneComplete()
        {
          
        }

    
        #endregion


        #region chartBox close

        [Inject]
        public LocationClosedSignal locationClosedSignal { get; set; }

        private void OnLocationClosed(TabTale.Publishing.LocationResult result)
        {
            Debug.LogError("CocoAudio.IsOn is : " + CocoAudio.IsOn + "result is : " + result);
			// sessionStart事件会导致广告正在弹出时，播放背景音乐
			if (result.location == ApplicationLocation.AppLoaded && !result.sourceAssigned)
				return;
            ResumeBGMusic();
        }

        protected void ResumeBGMusic()
        {
            CocoAudio.StopBgMusic();
            if (!CocoAudio.IsOn)
            {
                CocoAudio.StopAll();
            }
            else
            {
                CocoAudio.MuteAll(false);
            }
        }

        #endregion

        protected virtual IEnumerator PlayBgMusic()
        {
            yield return new WaitForEndOfFrame();
//            CocoAudio.PlayBgMusic(gameGlobalData.GetSceneBg(gameGlobalData.CurSceneID));
        }


      
    }

}