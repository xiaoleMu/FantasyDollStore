﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Text;
using System.IO;

public class ZipUtil
{
	#if UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void unzip (string zipFilePath, string location);

	[DllImport("__Internal")]
	private static extern void zip (string zipFilePath);

	[DllImport("__Internal")]
	private static extern void addZipFile (string addFile);
	#endif

	public static void Unzip (string zipFilePath, string location)
	{
		#if UNITY_EDITOR 
		if (Application.platform == RuntimePlatform.OSXEditor) {
		Directory.CreateDirectory (location);
		
		using (ZipFile zip = ZipFile.Read (zipFilePath)) {
			
			zip.ExtractAll (location, ExtractExistingFileAction.OverwriteSilently);
		}
		}
		if (Application.platform == RuntimePlatform.Android) {
			#if UNITY_ANDROID
			using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
				zipper.CallStatic ("unzip", zipFilePath, location);
			}
			#endif
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
				unzip (zipFilePath, location);
		}
		#endif
	}

	public static void Zip (string zipFileName, params string[] files)
	{
		#if UNITY_EDITOR 
		if (Application.platform == RuntimePlatform.OSXEditor) {
			string path = Path.GetDirectoryName (zipFileName);
			Directory.CreateDirectory (path);
		
			using (ZipFile zip = new ZipFile()) {
				foreach (string file in files) {
					zip.AddFile (file, "");
				}
				zip.Save (zipFileName);
			}
		}
		if (Application.platform == RuntimePlatform.Android) {
			#if UNITY_ANDROID
			using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
				zipper.CallStatic ("zip", zipFileName, files);
			}
			#endif
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			foreach (string file in files) {
				addZipFile (file);
			}
			zip (zipFileName);
		}
		#endif
	}
}
