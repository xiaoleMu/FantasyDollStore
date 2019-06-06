using System;
using UnityEngine;
#if COCO_FAKE
using CocoLanguage = CocoPlay.Fake.CocoLanguage;
#else
using CocoLanguage = Game.CocoLanguage;
#endif
#if UNITY_2017_2_OR_NEWER
using Prime31;
#endif

namespace CocoPlay.Localization
{
    public static class CocoLanguageSetting
    {
        private const string SAVEKEY = "CocoLanguageSetting_SAVEKEY";
        private static CocoLanguage m_Language;
        private static bool m_LanguageLoaded;
        
        public static CocoLanguage Language
        {
            get { return m_Language; }
        }
    
        public static string GetLanguageName(CocoLanguage language)
        {
            string name;
            switch (language)
            {
                case CocoLanguage.ChineseSimplified:
                    name = "language_zh_hans";
                    break;
                case CocoLanguage.Japanese:
                    name = "language_ja";
                    break;
                case CocoLanguage.Korean:
                    name = "language_ko";
                    break;
                case CocoLanguage.ChineseTraditional:
                    name = "language_zh_hant";
                    break;
                case CocoLanguage.Russia:
                    name = "language_ru";
                    break;
    
                case CocoLanguage.French:
                    name = "language_fr";
                    break;
                case CocoLanguage.Italien:
                    name = "language_it";
                    break;
                case CocoLanguage.German:
                    name = "language_de";
                    break;
                case CocoLanguage.Spanish:
                    name = "language_es";
                    break;
                case CocoLanguage.Portuguese:
                    name = "language_pt";
                    break;
    
                default:
                    name = "language_en";
                    break;
            }
    
            return name;
        }
        
        public static CocoLanguage GetDeviceLanguage()
        {
            // Language
            CocoLanguage language = CocoLanguage.English;
            switch (Application.platform)
            {
            #if UNITY_IPHONE
                case RuntimePlatform.IPhonePlayer:
                string strLang = EtceteraBinding.getCurrentLanguage();
                if (strLang.Contains("zh-Hans"))
                    language = CocoLanguage.ChineseSimplified;
                else if (strLang.Contains("ja"))
                    language = CocoLanguage.Japanese;
                else if (strLang.Contains("ko"))
                    language = CocoLanguage.Korean;
                else if (strLang.Contains("zh-Hant"))
                    language = CocoLanguage.ChineseTraditional;
                else if (strLang.Contains("ru"))
                    language = CocoLanguage.Russia;
                
                else if (strLang.Contains ("fr"))
                    language = CocoLanguage.French;
                else if (strLang.Contains ("it"))
                    language = CocoLanguage.Italien;
                else if (strLang.Contains ("de"))
                    language = CocoLanguage.German;
                else if (strLang.Contains ("es"))
                    language = CocoLanguage.Spanish;
                else if (strLang.Contains ("pt"))
                    language = CocoLanguage.Portuguese;
    
                break;
            #elif UNITY_ANDROID
            case RuntimePlatform.Android:
                string strLang = CocoCommonAndroid.GetCurrentLanguage();
                if (strLang.StartsWith("zh_CN"))
                language = CocoLanguage.ChineseSimplified;
                else if (strLang.StartsWith("ja"))
                language = CocoLanguage.Japanese;
                else if (strLang.StartsWith("ko"))
                language = CocoLanguage.Korean;
                else if (strLang.StartsWith("zh_TW") || strLang.StartsWith("zh_HK"))
                language = CocoLanguage.ChineseTraditional;
                else if (strLang.StartsWith("ru"))
                language = CocoLanguage.Russia;
                else if (strLang.StartsWith("fr"))
                language = CocoLanguage.French;
                else if (strLang.StartsWith("it"))
                language = CocoLanguage.Italien;
                else if (strLang.StartsWith("de"))
                language = CocoLanguage.German;
                else if (strLang.StartsWith("es"))
                language = CocoLanguage.Spanish;
                else if (strLang.StartsWith("pt"))
                language = CocoLanguage.Portuguese;
                break;
            #endif
            default:
                switch (Application.systemLanguage)
                {
                case SystemLanguage.Chinese:
                    language = CocoLanguage.ChineseSimplified;
                    break;
                case SystemLanguage.Japanese:
                    language = CocoLanguage.Japanese;
                    break;
                case SystemLanguage.Korean:
                    language = CocoLanguage.Korean;
                    break;
                case SystemLanguage.Russian:
                    language = CocoLanguage.Russia;
                    break;
    
                case SystemLanguage.French:
                    language = CocoLanguage.French;
                    break;
                case SystemLanguage.Italian:
                    language = CocoLanguage.Italien;
                    break;
                case SystemLanguage.German:
                    language = CocoLanguage.German;
                    break;
                case SystemLanguage.Spanish:
                    language = CocoLanguage.Spanish;
                    break;
                case SystemLanguage.Portuguese:
                    language = CocoLanguage.Portuguese;
                    break;
                }
                break;
            }
            return language; 
        }
    
        public static void LoadLanguage()
        {
            if (m_LanguageLoaded)
                return;

            m_Language = CocoLanguage.English;
            if (PlayerPrefs.HasKey(SAVEKEY))
                m_Language = (CocoLanguage)Enum.Parse(typeof(CocoLanguage), PlayerPrefs.GetString(SAVEKEY));
            else
                m_Language = GetDeviceLanguage();
                        
    #if UNITY_EDITOR
            if (CocoDebugSettingsData.Instance.IsEditorLanguageEnabled) {
                m_Language = CocoDebugSettingsData.Instance.EditorLanguage;
            }
    #endif
            CocoLocalization.SetLanguage(GetLanguageName(m_Language));
            m_LanguageLoaded = CocoLocalization.IsDictionaryLoaded;
        }
    
        public static void SetLanguage(CocoLanguage language)
        {
            PlayerPrefs.SetString(SAVEKEY, language.ToString());
            m_Language = language;
            CocoLocalization.SetLanguage(GetLanguageName(m_Language));
        }
    }
}

