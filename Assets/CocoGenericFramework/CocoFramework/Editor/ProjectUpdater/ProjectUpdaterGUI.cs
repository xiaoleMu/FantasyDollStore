using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace TabTale.ProjectUpdater {

	public class ProjectUpdaterGUI : EditorWindow
	{
		private static PackagesContainer _packages;
		private static PackageInstallationDataContainer _installedPackages;
		private static float _currentTime = 0.0f;

		private const float REFRESH_RATE = 6.0f;
		private const int PACKAGE_NAME_WIDTH = 150;
		private const int IS_INSTALLED_WIDTH = 60;
		
		[MenuItem("TabTale/GSDK/Setup Packages &p")]
		static void SetupPackages () 
		{
			LoadPackages();
			ShowWindow();
		}

		void OnGUI()
		{
			ShowPackagesWindowUI();
			FocusWindowIfItsOpen<ProjectUpdaterGUI>();
		}

		void Update()
		{
			_currentTime += 0.01f;

			if(_currentTime > REFRESH_RATE)
			{
				_currentTime = 0.0f;
				LoadPackages();
			}
		}

		private static void LoadPackages()
		{
			var serializer = new XmlSerializer(typeof(PackagesContainer));
			var stream = new FileStream("Assets/TabTale/UnityPackages/packages.xml", FileMode.Open);
			_packages = serializer.Deserialize(stream) as PackagesContainer;
			stream.Close();

			serializer = new XmlSerializer(typeof(PackageInstallationDataContainer));
			stream = new FileStream("packages.xml", FileMode.Open);
			_installedPackages = serializer.Deserialize(stream) as PackageInstallationDataContainer;
            stream.Close();
            
			Debug.Log ("Found " + _installedPackages.packagesData.Count + " installed packages. Total: " + _packages.packagesData.Length);
		}

		private static void ShowWindow()
		{
			int width = Screen.currentResolution.width;
			int height = Screen.currentResolution.height;
			EditorWindow.GetWindowWithRect<ProjectUpdaterGUI>(new Rect(width/3,height/2,width/4,height/3),false, "TabtalePackages");

		}

		void ShowPackagesWindowUI()
		{
			if(_packages != null)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label("Package Name", GUILayout.Width(PACKAGE_NAME_WIDTH));
					GUILayout.Label("Installed", GUILayout.Width(IS_INSTALLED_WIDTH));
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("------------------------------------------------");
				EditorGUILayout.EndHorizontal();
				
				foreach(PackageData packageData in _packages.packagesData)
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label(packageData.packageName, GUILayout.Width(PACKAGE_NAME_WIDTH));

						var installedPackage =_installedPackages.packagesData.FirstOrDefault(p => p.packageName == packageData.packageName);
						bool isInstalled = _installedPackages.packagesData.Any(p => p.packageName == packageData.packageName);

						GUILayout.Toggle(isInstalled,"");

						if(GUILayout.Button (isInstalled ? "Update" : "Install", GUILayout.Width(IS_INSTALLED_WIDTH)))
						{
							Debug.Log ("Installing " + packageData.packageName);

							if(installedPackage != null)
							{

							}
							else
							{
								PackageInstallationData pkg = new PackageInstallationData();
								pkg.packageName = packageData.packageName;
								_installedPackages.packagesData.Add(pkg);
							}


							SaveInstalledPackages();
							ProjectUpdater.StartUpdateProcess();

						}

						if(GUILayout.Button ("Uninstall", GUILayout.Width(IS_INSTALLED_WIDTH)))
						{
							Debug.Log ("Uninstalling " + packageData.packageName);
							_installedPackages.packagesData.RemoveAll(p => p.packageName == packageData.packageName);
							SaveInstalledPackages();
							ProjectUpdater.StartUpdateProcess();
							//ProjectUpdaterUninstall.UninstallPackage(packageData.packageName);

						}

						GUILayout.FlexibleSpace();
							
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		void SaveInstalledPackages()
		{
			var serializer = new XmlSerializer(typeof(PackageInstallationDataContainer));
			using(var stream = new FileStream("packages.xml", FileMode.Truncate))
			{
				serializer.Serialize(stream, _installedPackages);
            }
        }
    }
    
}
