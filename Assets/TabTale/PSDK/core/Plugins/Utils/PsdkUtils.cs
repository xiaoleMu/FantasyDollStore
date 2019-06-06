using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace TabTale.Plugins.PSDK {

	public static class PsdkUtils {

		[DllImport ("__Internal")]
		private static extern bool psdkUnityUnzip(string zippedFilePath, string destinationPath);

		[DllImport ("__Internal")]
		private static extern string psdkUtilsGetCacheDir();

		[DllImport ("__Internal")]
		private static extern string psdkUtilsGetBundleVersion ();

		[DllImport ("__Internal")]
		private static extern string psdkUtilsGetBundleIdentifier ();

		public static ConsentType StringToConsentType(string consentTypeStr)
		{
			ConsentType result = ConsentType.UNKNOWN;
			if(consentTypeStr != null){
				if(consentTypeStr.Equals("pa", System.StringComparison.InvariantCultureIgnoreCase)){
					result = ConsentType.PA;
				}
				else if(consentTypeStr.Equals("npa", System.StringComparison.InvariantCultureIgnoreCase)){
					result = ConsentType.NPA;
				}
				else if(consentTypeStr.Equals("ua", System.StringComparison.InvariantCultureIgnoreCase)){
					result = ConsentType.UA;
				}
				else if(consentTypeStr.Equals("ne", System.StringComparison.InvariantCultureIgnoreCase)){
					result = ConsentType.NE;
				}
			}
			return result;
		}


		#if UNITY_IOS && !UNITY_EDITOR
	        [DllImport("__Internal")]
        	static extern void psdkUtilsNativeLog(string line);
		#endif




#if UNITY_ANDROID
		public static AndroidJavaObject CreateJavaHashMapFromDictionary(IDictionary<string,string> keysDict)
		{
			if (keysDict == null)
				return null;

			AndroidJavaObject obj_HashMap = new AndroidJavaObject("java.util.HashMap");
			// Call 'put' via the JNI instead of using helper classes to avoid:
			//  "JNI: Init'd AndroidJavaObject with null ptr!"
			System.IntPtr method_Put = AndroidJNIHelper.GetMethodID(obj_HashMap.GetRawClass(), "put",
			                                                        "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			
			object[] args = new object[2];
			if (keysDict != null) {
				foreach (KeyValuePair<string,string> kvp in keysDict)
				{
					using(AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key + ""))
					{
						using(AndroidJavaObject v = new AndroidJavaObject("java.lang.String", kvp.Value + ""))
						{
							args[0] = k;
							args[1] = v;
							AndroidJNI.CallObjectMethod(obj_HashMap.GetRawObject(),
							                            method_Put, AndroidJNIHelper.CreateJNIArgArray(args));
						}
					}
				}
			}
			return obj_HashMap;
		}


		public static AndroidJavaObject CreateJavaJSONObjectFromDictionary(IDictionary<string,object> keysDict)
		{
			if (keysDict == null)
				return null;

			return new AndroidJavaObject("org.json.JSONObject",Json.Serialize (keysDict));
		}


#endif

		public static void MakeDir(string path)
		{
            #if UNITY_ANDROID
			using (AndroidJavaClass jc = new AndroidJavaClass ("com.tabtale.publishingsdk.core.utils.PublishingSDKFileUtils")) {
			    jc.Call ("makeDir", path);
            }
            #else 
            try {
                System.IO.Directory.CreateDirectory(path);
            }
            catch (System.Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            #endif
		}





//		try {
//			InputStream is = assetManager.open("psdk.json");
//			PublishingSDKFileUtils utils = new PublishingSDKFileUtils();
//			configJson = utils.convertStreamToString(is);
//		} catch (IOException e) {
//			Log.e(TAG, "start: failed to get InputStrem for psdk.json (is the file missing from assets?), exception: " + e.getMessage());
//			return null;
//		}
//
		public static string ReadAndroidAssetsFile(string fileRelativePath) {
			#if UNITY_ANDROID && ! UNITY_EDITOR
			try {
				using (AndroidJavaObject androidAssetManager = CurrentActivity.Call<AndroidJavaObject>("getAssets")) {
					if (androidAssetManager != null) {
					using (AndroidJavaObject androidInputStream = androidAssetManager.Call<AndroidJavaObject>("open",fileRelativePath)) {
						if (androidInputStream != null) {
								using (AndroidJavaObject psfu = new AndroidJavaObject ("com.tabtale.publishingsdk.core.utils.PublishingSDKFileUtils")) {
									return psfu.Call<string> ("convertStreamToString", androidInputStream);
								}									
							}
						}
					}
				}
				}
				catch (System.Exception e) {
					Debug.LogException(e);
				}

				#endif
			return null;
		}


		public static string ReadStreamingAssetsFile(string fileRelativePath) {
			
			string assetsFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileRelativePath);


			try {
				if (assetsFilePath.Contains("jar:file") || assetsFilePath.Contains("://")) 
				{
					// Android
					int timeout = 7; // seconds
					System.DateTime startTime = System.DateTime.Now;
						
					WWW www = new WWW(assetsFilePath);
					Debug.Log ("trying to read file: " + assetsFilePath);
					while (!www.isDone) {
						System.TimeSpan interval = System.DateTime.Now - startTime;
						if (interval.Seconds > timeout) 
							return null;
					}
					if (! System.String.IsNullOrEmpty(www.error)) {
						return null;
					}

					return System.Text.Encoding.UTF8.GetString(www.bytes);
				}
				else {
					if (! System.IO.File.Exists(assetsFilePath))
						return null;
						
					string config = System.IO.File.ReadAllText(assetsFilePath);
					return config;
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
			
			return null;
		}

		public static bool CopyStreamingAssetsFile(string streamingAssetsfileRelativePath, string destinationFilePath) {
			
			string assetsFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, streamingAssetsfileRelativePath);
			
			
			try {
				if (assetsFilePath.Contains("jar:file") || assetsFilePath.Contains("://")) 
				{
					// Android
					int timeout = 7; // seconds
					System.DateTime startTime = System.DateTime.Now;
					
					WWW www = new WWW(assetsFilePath);
					Debug.Log ("trying to read file: " + assetsFilePath);
					while (!www.isDone) {
						System.TimeSpan interval = System.DateTime.Now - startTime;
						if (interval.Seconds > timeout) 
							return false;
					}
					if (! System.String.IsNullOrEmpty(www.error)) {
						return false;
					}

					System.IO.File.WriteAllBytes(destinationFilePath, www.bytes);
					return true;
				}
				else {
					if (! System.IO.File.Exists(assetsFilePath))
						return false;

					System.IO.File.WriteAllBytes(destinationFilePath, System.IO.File.ReadAllBytes(assetsFilePath));
					return true;
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
			
			return false;
		}
		

		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();
			
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}
			
			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			
			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}
			
			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}

		public static void Unzip(string streamingAssetsZipFilePath, string destinationFolder) {

			string destZip = System.IO.Path.Combine( destinationFolder , "tmpttpsdk.zip");
			if (! CopyStreamingAssetsFile(streamingAssetsZipFilePath, destZip)) {
				Debug.LogError("Unzip:: Didn't read streaming assets zip file " + streamingAssetsZipFilePath);
				return ;
			}

			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				psdkUnityUnzip(destZip, destinationFolder);
			}

			if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
				AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
				AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject zipDecompressJavaObject  = new AndroidJavaObject("com.tabtale.publishingsdk.core.utils.ZipDecompress",destZip,destinationFolder);
				zipDecompressJavaObject.Call("setActivity",jo);
				if (!zipDecompressJavaObject.Call<bool>("unzip")) {
					Debug.LogError("PsdkUtils::Unzip file " + destZip + " was not unzipped to " + destinationFolder);
				}
				#endif
			}

			if (System.IO.File.Exists(destZip))
				System.IO.File.Delete(destZip);

		}

		public static T GetEnumFromString<T>(string enumValueAsString) {
			T[] values = System.Enum.GetValues(typeof(T)) as T[] ;
			foreach ( T item in values) {
				if (item.ToString().ToLower().Contains(enumValueAsString.ToString().ToLower()) || enumValueAsString.ToString().ToLower().Contains(item.ToString().ToLower())) 
					return item;
			}
			Debug.LogError("Didn't match enum " + typeof(T).ToString()  + " value of " + enumValueAsString.ToString());
			return default(T);
		}


		public static string BuildJsonStringFromDict(IDictionary<string,string> eventParams) {

			if (eventParams == null)
				return null;

			string jsonParams = "{";
			bool first = true;
			if ( eventParams != null) {
				foreach(KeyValuePair<string, string> param in eventParams){
					if (first) 
						first = false;
					else
						jsonParams += ",";
					jsonParams += "\"" + param.Key + "\" : \"" +param.Value + "\""; 
				}
			}
			jsonParams += "}";
			return jsonParams;
		}

		public static string BuildJsonStringFromDict(IDictionary<string,object> eventParams) {
			
			if (eventParams == null)
				return null;
			
			return TabTale.Plugins.PSDK.Json.Serialize (eventParams);
		}

		public static string temporaryCachePath {
			get {
				
				if (! System.String.IsNullOrEmpty(Application.temporaryCachePath))
					return Application.temporaryCachePath;
				
				#if UNITY_IOS
				return psdkUtilsGetCacheDir();
				#elif UNITY_ANDROID
				using (AndroidJavaObject FileObject = CurrentActivity.Call<AndroidJavaObject>("getCacheDir")) {
					if (FileObject != null)
						return FileObject.Call<string>("getAbsolutePath");
				}
				#endif
				
				
				return "";
			}
		}

		static string _bundleVersion = null;
		public static string bundleVersion {
			get {

				#if UNITY_EDITOR
				return UnityEditor.PlayerSettings.bundleVersion;
				#endif

				if (_bundleVersion != null) 
					return _bundleVersion;

				#if UNITY_IOS
				_bundleVersion = psdkUtilsGetBundleVersion();
				#elif UNITY_ANDROID
//				Context context = getApplicationContext(); // or activity.getApplicationContext()
//				PackageManager packageManager = context.getPackageManager();
//				String packageName = context.getPackageName();
//				
//				String myVersionName = "not available"; // initialize String
//				
//				try {
//					myVersionName = packageManager.getPackageInfo(packageName, 0).versionName;
//				} catch (PackageManager.NameNotFoundException e) {
//					e.printStackTrace();
//				}

				using (AndroidJavaObject Context = CurrentActivity.Call<AndroidJavaObject>("getApplicationContext")) {
					if (Context != null) {
						System.String packageName = Context.Call<string>("getPackageName");
						using (AndroidJavaObject packageManager = Context.Call<AndroidJavaObject>("getPackageManager")) {
							if (packageManager != null) {
								try {
									using (AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo",packageName, 0)) {
										if (packageManager != null) {
											_bundleVersion = packageInfo.Get<string>("versionName");
										}
									}
								}
								catch (System.Exception e) {
									Debug.LogException(e);
								}
							}
						}
					}
				}
				#endif
				
				if (_bundleVersion != null) {
					Debug.Log ("Unity native bundle version: " + _bundleVersion);
				}
				else {
					Debug.Log ("Unity native bundle version NULL");
				}
				return _bundleVersion;
			}
		}

		#if UNITY_ANDROID
		static AndroidJavaClass _unityPlayerJavaClass = null;
		public static AndroidJavaClass UnityPlayerJavaClass {
			get {
				#if ! UNITY_EDITOR
				if (_unityPlayerJavaClass == null)
					_unityPlayerJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				#endif
				return _unityPlayerJavaClass;
			}
		}
		
		public static AndroidJavaObject CurrentActivity {
			get {
				
				if (UnityPlayerJavaClass == null) return null;

				return UnityPlayerJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}
		#endif
		
       public static void NativeLog(string line) {
                line = Time.realtimeSinceStartup.ToString("N1") + ":" + line;
                Debug.Log (line);
                #if UNITY_IOS && !UNITY_EDITOR
               	psdkUtilsNativeLog (line);
                #endif
        }

	static string _bundleIdentifier = null;

	public static string BundleIdentifier {
		get {
			if (_bundleIdentifier != null) 
				return _bundleIdentifier;
			

				#if UNITY_EDITOR
					#if UNITY_5_6_OR_NEWER
					    #if UNITY_ANDROID
						     return UnityEditor.PlayerSettings.GetApplicationIdentifier( UnityEditor.BuildTargetGroup.Android);
						#else
						     return UnityEditor.PlayerSettings.GetApplicationIdentifier( UnityEditor.BuildTargetGroup.iOS);
						#endif
					#else
						return UnityEditor.PlayerSettings.bundleIdentifier;
					#endif
				#endif
				

				#if UNITY_IOS
				_bundleIdentifier = psdkUtilsGetBundleIdentifier();
				#elif UNITY_ANDROID

				using (AndroidJavaObject Context = CurrentActivity.Call<AndroidJavaObject>("getApplicationContext")) {
					if (Context != null) {
						System.String packageName = Context.Call<string>("getPackageName");
						using (AndroidJavaObject packageManager = Context.Call<AndroidJavaObject>("getPackageManager")) {
							if (packageManager != null) {
								try {
									using (AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo",packageName, 0)) {
										if (packageManager != null) {
											_bundleIdentifier = packageInfo.Get<string>("packageName");
										}
									}
								}
								catch (System.Exception e) {
									Debug.LogException(e);
								}
							}
						}
					}
				}
				#endif
				
				if (_bundleIdentifier != null) {
					Debug.Log ("Unity native bundle identifier: " + _bundleIdentifier);
				}
				else {
					Debug.Log ("Unity native bundle identifier NULL");
				}
				return _bundleIdentifier;

			
		}
	}


		public static string ReadPsdkConfigFromFile() {
			string fileRelativePath;
			bool android = false;
                        
                        string json = null;

			if (RuntimePlatform.IPhonePlayer == Application.platform)
				fileRelativePath = "psdk_ios.json";
			else  {
				fileRelativePath = "psdk_google.json";
				#if AMAZON
				fileRelativePath = "psdk_amazon.json";
                                #else
				json = PsdkUtils.ReadAndroidAssetsFile(fileRelativePath);
				if (null != json) {
					UnityEngine.Debug.Log("Read psdk_google.json directly from android assets folder !");
				}
				#endif
				android = true;
			}

                        if (json == null) {
			     json = PsdkUtils.ReadStreamingAssetsFile(fileRelativePath);
                        }

			if (json == null && android) 
				json = PsdkUtils.ReadStreamingAssetsFile("psdk_amazon.json");

			if (json == null) 
				json = PsdkUtils.ReadStreamingAssetsFile("psdk.json");
		
			if (null != json)
				return json;

			if (RuntimePlatform.IPhonePlayer == Application.platform)
				json = PsdkUtils.ReadStreamingAssetsFile("psdk_ios_.json");
			else  {
				#if AMAZON
				json = PsdkUtils.ReadStreamingAssetsFile("psdk_amazon_.json");
				#else
				json = PsdkUtils.ReadStreamingAssetsFile("psdk_google_.json");
				#endif
				android = true;
			}

			if (null != json) {
				Debug.LogWarning ("PSDK - You are using test json configuration file  !!");
				return json;
			}


			Debug.LogError ("PSDKMgr::ReadPsdkConfigFromFile failed reading json file Assets/StreamingAssets/psdk.json !!");
			return "{}";
		}


		static string _psdkRootPath = null;
		public static string PsdkRootPath {
			get {
				if (_psdkRootPath == null) {
					string[] paths = { "TabTale", "PSDK"};
					_psdkRootPath = "";
					foreach (string path in paths) {
						if (_psdkRootPath.Length == 0) 
							_psdkRootPath = path;
						else
							_psdkRootPath = Path.Combine(_psdkRootPath,path);
					}
				}
				return _psdkRootPath;
			}
		}

	}


}
