using UnityEngine;
using System;

public interface IGooglePlayObbDownloader
{
    string PublicKey { get; set; }

    string GetExpansionFilePath();

    string GetMainOBBPath();

    string GetPatchOBBPath();

    void FetchOBB();

    DownloadProgressInfo GetDownloadProgressInfo();

    void OnAppResume();

    void OnAppStop();

    void OnResumeDownload(bool pIsNoWifi = false);

    void OnPauseDownload();

    void OnAbortDownload();

    OBBDownloadState GetDownloadState();

    bool GetIsFinish();

    void Destory();
}

public class GooglePlayObbDownloadManager
{
    private static readonly AndroidJavaClass m_AndroidOSBuildClass = new AndroidJavaClass("android.os.Build");
    private static IGooglePlayObbDownloader m_Instance;

    public static IGooglePlayObbDownloader GetGooglePlayObbDownloader()
    {
        if (m_Instance != null)
            return m_Instance;

        if (!IsDownloaderAvailable())
            return null;

        m_Instance = new GooglePlayObbDownloader();
        return m_Instance;
    }

    public static bool IsDownloaderAvailable()
    {
        return m_AndroidOSBuildClass.GetRawClass() != IntPtr.Zero;
    }
}

public class DownloadProgressInfo
{
    //    Log.e(TAG, "onDownloadProgress:" + (int) (progress.mOverallProgress >> 8));
    //    Log.e(TAG, "onDownloadProgress:" + Long.toString(progress.mOverallProgress* 100 /progress.mOverallTotal) + "%");

    public long OverallTotal = 0;

    public long OverallProgress = 0;

    // 默认字节 >>8  变成KB
    public long TimeRemaining = 0;

    // time remaining
    public float CurrentSpeed = 0f;

    // speed in KB/S
}

public enum OBBDownloadState
{
    STATE_NONE = 0,
    STATE_IDLE = 1,
    STATE_FETCHING_URL = 2,
    STATE_CONNECTING = 3,
    STATE_DOWNLOADING = 4,
    STATE_COMPLETED = 5,

    STATE_PAUSED_NETWORK_UNAVAILABLE = 6,
    STATE_PAUSED_BY_REQUEST = 7,

    /**
     * Both STATE_PAUSED_WIFI_DISABLED_NEED_CELLULAR_PERMISSION and
     * STATE_PAUSED_NEED_CELLULAR_PERMISSION imply that Wi-Fi is unavailable and
     * cellular permission will restart the service. Wi-Fi disabled means that
     * the Wi-Fi manager is returning that Wi-Fi is not enabled, while in the
     * other case Wi-Fi is enabled but not available.
     */
    STATE_PAUSED_WIFI_DISABLED_NEED_CELLULAR_PERMISSION = 8,

    STATE_PAUSED_NEED_CELLULAR_PERMISSION = 9,

    /**
     * Both STATE_PAUSED_WIFI_DISABLED and STATE_PAUSED_NEED_WIFI imply that
     * Wi-Fi is unavailable and cellular permission will NOT restart the
     * service. Wi-Fi disabled means that the Wi-Fi manager is returning that
     * Wi-Fi is not enabled, while in the other case Wi-Fi is enabled but not
     * available.
     * <p>
     * The service does not return these values. We recommend that app
     * developers with very large payloads do not allow these payloads to be
     * downloaded over cellular connections.
     */
    STATE_PAUSED_WIFI_DISABLED = 10,

    STATE_PAUSED_NEED_WIFI = 11,

    STATE_PAUSED_ROAMING = 12,

    /**
     * Scary case. We were on a network that redirected us to another website
     * that delivered us the wrong file.
     */
    STATE_PAUSED_NETWORK_SETUP_FAILURE = 13,

    STATE_PAUSED_SDCARD_UNAVAILABLE = 14,

    STATE_FAILED_UNLICENSED = 15,
    STATE_FAILED_FETCHING_URL = 16,
    STATE_FAILED_SDCARD_FULL = 17,
    STATE_FAILED_CANCELED = 18,

    STATE_FAILED = 19
}

public static class OBBDownloadStateExcention
{
    public static string GetOBBDownloadStateStr(this OBBDownloadState pState)
    {
        string tStr = "Starting...";
        switch (pState)
        {
            case OBBDownloadState.STATE_IDLE:
                tStr = "Waiting for download to start";
                break;
            case OBBDownloadState.STATE_FETCHING_URL:
                tStr = "Looking for resources to download";
                break;
            case OBBDownloadState.STATE_CONNECTING:
                tStr = "Connecting to the download server";
                break;
            case OBBDownloadState.STATE_DOWNLOADING:
                tStr = "Downloading resources";
                break;
            case OBBDownloadState.STATE_COMPLETED:
                tStr = "Download finished";
                break;
            case OBBDownloadState.STATE_PAUSED_NETWORK_UNAVAILABLE:
                tStr = "Download paused because no network is available";
                break;
            case OBBDownloadState.STATE_PAUSED_NETWORK_SETUP_FAILURE:
                tStr = "Download paused. Test a website in browser";
                break;
            case OBBDownloadState.STATE_PAUSED_BY_REQUEST:
                tStr = "Download paused";
                break;
            case OBBDownloadState.STATE_PAUSED_NEED_WIFI:
                tStr = "Download paused because wifi is unavailable";
                break;
            case OBBDownloadState.STATE_PAUSED_WIFI_DISABLED:
                tStr = "Download paused because wifi is disabled";
                break;
            case OBBDownloadState.STATE_PAUSED_ROAMING:
                tStr = "Download paused because you are roaming";
                break;
            case OBBDownloadState.STATE_PAUSED_SDCARD_UNAVAILABLE:
                tStr = "Download paused because the external storage is unavailable";
                break;
            case OBBDownloadState.STATE_FAILED_UNLICENSED:
                tStr = "Download failed because you may not have purchased this app";
                break;
            case OBBDownloadState.STATE_FAILED_FETCHING_URL:
                tStr = "Download failed because the resources could not be found";
                break;
            case OBBDownloadState.STATE_FAILED_SDCARD_FULL:
                tStr = "Download failed because the external storage is full";
                break;
            case OBBDownloadState.STATE_FAILED_CANCELED:
                tStr = "Download cancelled";
                break;
            case OBBDownloadState.STATE_FAILED:
                tStr = "Download failed";
                break;
            case OBBDownloadState.STATE_PAUSED_WIFI_DISABLED_NEED_CELLULAR_PERMISSION:
                tStr = "Download paused because wifi is disabled";
                break;
            case OBBDownloadState.STATE_PAUSED_NEED_CELLULAR_PERMISSION:
                tStr = "Download paused because wifi is unavailable";
                break;
        }
        return tStr;
    }
}