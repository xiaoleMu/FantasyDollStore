using UnityEngine;
using System.IO;
using System;

internal class GooglePlayObbDownloader : IGooglePlayObbDownloader
{
    private static readonly AndroidJavaClass ENVIRONMENT_CLASS = new AndroidJavaClass("android.os.Environment");
    private const string ENVIRONMENT_MEDIA_MOUNTED = "mounted";

    public string PublicKey { get; set; }

    private bool m_IsSetupedKey = false;
    private AndroidJavaObject m_DownloadClient;
    private DownloadProgressInfo m_DownloadProgressInfo;

    private void ApplyPublicKey()
    {
        if (m_IsSetupedKey) return;

        if (string.IsNullOrEmpty(PublicKey))
        {
            Debug.LogError(
                "GooglePlayObbDownloader: The public key is not set - did you forget to set it in the script?\n");
        }

        m_IsSetupedKey = true;

        using (var downloaderServiceClass =
            new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
        {
            downloaderServiceClass.SetStatic("BASE64_PUBLIC_KEY", PublicKey);
            // Used by the preference obfuscator
            downloaderServiceClass.SetStatic("SALT",
                new byte[]
                {
                    1, 43, 256 - 12, 256 - 1, 54, 98, 256 - 100, 256 - 12, 43, 2, 256 - 8, 256 - 4, 9, 5, 256 - 106,
                    256 - 108, 256 - 33, 45, 256 - 1, 84
                });
        }
    }

    public void FetchOBB()
    {
        ApplyPublicKey();

        if (m_DownloadClient != null) return;

        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            m_DownloadClient = new AndroidJavaObject("com.unity3d.plugin.downloader.UnityDownloaderClient");

            try
            {
                m_DownloadClient.Call("Check", currentActivity);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public DownloadProgressInfo GetDownloadProgressInfo()
    {
        if (m_DownloadProgressInfo == null)
            m_DownloadProgressInfo = new DownloadProgressInfo();

        if (m_DownloadClient != null)
        {
            var info = m_DownloadClient.Call<AndroidJavaObject>("GetProgressInfo");
            m_DownloadProgressInfo.CurrentSpeed = info.Get<float>("mCurrentSpeed");
            m_DownloadProgressInfo.OverallProgress = info.Get<long>("mOverallProgress");
            m_DownloadProgressInfo.OverallTotal = info.Get<long>("mOverallTotal");
            m_DownloadProgressInfo.TimeRemaining = info.Get<long>("mTimeRemaining");
        }
        return m_DownloadProgressInfo;
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public void OnAppResume()
    {
        if (m_DownloadClient != null)
        {
            try
            {
                m_DownloadClient.Call("onResume");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void OnAppStop()
    {
        if (m_DownloadClient != null)
        {
            try
            {
                m_DownloadClient.Call("onStop");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    /// <summary>
    /// 恢复下载
    /// </summary>
    /// <param name="pIsNoWifi"> 非WiFi下是否下载</param>
    public void OnResumeDownload(bool pIsNoWifi = false)
    {
        if (m_DownloadClient != null)
        {
            try
            {
                m_DownloadClient.Call("resumeDownload", pIsNoWifi);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    /// <summary>
    /// 暂停下载
    /// </summary>
    public void OnPauseDownload()
    {
        if (m_DownloadClient != null)
        {
            try
            {
                m_DownloadClient.Call("pauseDownload");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    /// <summary>
    /// 停止下载
    /// </summary>
    public void OnAbortDownload()
    {
        if (m_DownloadClient != null)
        {
            try
            {
                m_DownloadClient.Call("abortDownload");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    /// <summary>
    /// 获取下载状态
    /// </summary>
    /// <returns></returns>
    public OBBDownloadState GetDownloadState()
    {
        int state = 0;
        if (m_DownloadClient != null)
        {
            try
            {
				state = m_DownloadClient.Call<int>("getDownloadState");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        return (OBBDownloadState) state;
    }

    /// <summary>
    /// 全部完成，可以跳转场景
    /// </summary>
    /// <returns></returns>
    public bool GetIsFinish()
    {
        if (m_DownloadClient != null)
        {
            try
            {
                return m_DownloadClient.Call<bool>("getIsFinish");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        return false;
    }

    public void Destory()
    {
        OnAbortDownload();

        if (m_DownloadClient != null)
        {
            m_DownloadClient.Dispose();
            m_DownloadClient = null;
        }
    }


    private string m_ExpansionFilePath;

    public string GetExpansionFilePath()
    {
        if (ENVIRONMENT_CLASS.CallStatic<string>("getExternalStorageState") != ENVIRONMENT_MEDIA_MOUNTED)
        {
            m_ExpansionFilePath = null;
            return m_ExpansionFilePath;
        }
        if (string.IsNullOrEmpty(m_ExpansionFilePath))
        {
            const string obb_path = "Android/obb";
            using (var externalStorageDirectory =
                ENVIRONMENT_CLASS.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
            {
                var externalRoot = externalStorageDirectory.Call<string>("getPath");
                m_ExpansionFilePath = string.Format("{0}/{1}/{2}", externalRoot, obb_path, ObbPackage);
            }
        }
        return m_ExpansionFilePath;
    }

    public string GetMainOBBPath()
    {
        return GetOBBPackagePath(GetExpansionFilePath(), "main");
    }

    public string GetPatchOBBPath()
    {
        return GetOBBPackagePath(GetExpansionFilePath(), "patch");
    }

    private static string GetOBBPackagePath(string pExpansionFilePath, string pRefix)
    {
        if (string.IsNullOrEmpty(pExpansionFilePath))
            return null;
        var filePath = string.Format("{0}/{1}.{2}.{3}.obb", pExpansionFilePath, pRefix, ObbVersion, ObbPackage);
        return File.Exists(filePath) ? filePath : null;
    }

    private static string m_ObbPackage;

    private static string ObbPackage
    {
        get
        {
            if (m_ObbPackage == null)
            {
                PopulateOBBProperties();
            }
            return m_ObbPackage;
        }
    }

    private static int m_ObbVersion;

    private static int ObbVersion
    {
        get
        {
            if (m_ObbVersion == 0)
            {
                PopulateOBBProperties();
            }
            return m_ObbVersion;
        }
    }

// This code will reuse the package version from the .apk when looking for the .obb
// Modify as appropriate
    private static void PopulateOBBProperties()
    {
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            m_ObbPackage = currentActivity.Call<string>("getPackageName");
            var packageInfo = currentActivity.Call<AndroidJavaObject>("getPackageManager")
                .Call<AndroidJavaObject>("getPackageInfo", m_ObbPackage, 0);
            m_ObbVersion = packageInfo.Get<int>("versionCode");
        }
    }
}