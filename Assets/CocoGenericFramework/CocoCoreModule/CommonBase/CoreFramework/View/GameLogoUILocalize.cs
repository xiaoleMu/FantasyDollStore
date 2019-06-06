using UnityEngine;
using UnityEngine.UI;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;
using CocoAudioID = CocoPlay.Fake.CocoAudioID;
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#else
using CocoUIButtonID = Game.CocoUIButtonID;
using CocoAudioID = Game.CocoAudioID;
using CocoLanguage = Game.CocoLanguage;
using CocoSceneID = Game.CocoSceneID;
#endif

#if UNITY_2017_2_OR_NEWER
using Prime31;
#endif

namespace CocoPlay
{
    public class GameLogoUILocalize : MonoBehaviour
    {
        [SerializeField]
        Image m_Image;
        [SerializeField]
        Sprite m_English;
        [SerializeField]
        Sprite m_Chinese;

        CocoLanguage mLanguage;
        void Start()
        {
            InitLanguage();

#if UNITY_EDITOR
            if (CocoDebugSettingsData.Instance.IsEditorLanguageEnabled) {
                mLanguage = CocoDebugSettingsData.Instance.EditorLanguage;
            }
#endif

            if (mLanguage == CocoLanguage.ChineseSimplified || mLanguage == CocoLanguage.ChineseTraditional)
                m_Image.sprite = m_Chinese;
            else
                m_Image.sprite = m_English;
        }

        private void InitLanguage()
        {
            mLanguage = CocoLanguage.English;
            switch (Application.platform)
            {
#if UNITY_IPHONE
                case RuntimePlatform.IPhonePlayer:
                    string strLang = EtceteraBinding.getCurrentLanguage();
                    if (strLang.Contains("zh-Hans"))
                        mLanguage = CocoLanguage.ChineseSimplified;
                    else if (strLang.Contains("ja"))
                        mLanguage = CocoLanguage.Japanese;
                    else if (strLang.Contains("ko"))
                        mLanguage = CocoLanguage.Korean;
                    else if (strLang.Contains("zh-Hant"))
                        mLanguage = CocoLanguage.ChineseTraditional;
                    else if (strLang.Contains("ru"))
                        mLanguage = CocoLanguage.Russia;
                    break;
#elif UNITY_ANDROID
            case RuntimePlatform.Android:
                string strLang = CocoCommonAndroid.GetCurrentLanguage();
                if (strLang.StartsWith("zh_CN"))
                    mLanguage = CocoLanguage.ChineseSimplified;
                else if (strLang.StartsWith("ja"))
                    mLanguage = CocoLanguage.Japanese;
                else if (strLang.StartsWith("ko"))
                    mLanguage = CocoLanguage.Korean;
                else if (strLang.StartsWith("zh_TW") || strLang.StartsWith("zh_HK"))
                    mLanguage = CocoLanguage.ChineseTraditional;
                else if (strLang.StartsWith("ru"))
                    mLanguage = CocoLanguage.Russia;
                break;
#endif
                default:
                    switch (Application.systemLanguage)
                    {
                        case SystemLanguage.Chinese:
                            mLanguage = CocoLanguage.ChineseSimplified;
                            break;
                        case SystemLanguage.Japanese:
                            mLanguage = CocoLanguage.Japanese;
                            break;
                        case SystemLanguage.Korean:
                            mLanguage = CocoLanguage.Korean;
                            break;
                        case SystemLanguage.Russian:
                            mLanguage = CocoLanguage.Russia;
                            break;
                    }
                    break;
            }
        }
    }
}
