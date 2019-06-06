using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using UnityEditor;
using System.Xml;
using TabTale;

namespace TabTale.Packages {

	public static class PackageManager
	{
		public static void ExportPackages()
		{
			CoreLogger.LogDebug("PackageManager", "parsing command line");

			CommandLineParser cmdParser = new CommandLineParser();

			string filename = cmdParser["filename"];
			if(filename != null)
			{
				CoreLogger.LogDebug("PackageManager", string.Format("export manifest specified to be {0}", filename));
			} else
			{
				CoreLogger.LogDebug("PackageManager", "export manifest specified to be the default exports.xml");
				filename = "exports.xml";
			}

			string outputPath = cmdParser["outputpath"];
			if(outputPath != null)
			{
				CoreLogger.LogDebug("PackageManager",string.Format("output path set to be {0}", outputPath));
			} else{
				CoreLogger.LogDebug("PackageManager", "output path set to be the default");
			}

			ExportPackages(filename, outputPath);
		}	

		private static void ExportPackages(string moduleFilename, string outputPath)
		{
			if(!File.Exists(moduleFilename))
			{
				CoreLogger.LogError("PackageManager", string.Format("unable to find file {0}", moduleFilename));
				return;
			}

			CoreLogger.LogDebug("PackageManager", string.Format("beginning export of package in file {0}", moduleFilename));

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(moduleFilename);

			if(xmlDoc.DocumentElement.Name != "module")
			{
				CoreLogger.LogError("PackageManager", string.Format("error in file {0}: root element of module file should be module!", moduleFilename));
				return;
			}

			if(!xmlDoc.DocumentElement.HasAttribute("packageName"))
			{
				CoreLogger.LogError("PackageManager", string.Format("error in file {0}: module element missing packageName attribute!", moduleFilename));
				return;
			}

			string packageName = xmlDoc.DocumentElement.GetAttribute("packageName");
			string packageFilename = string.Format("{0}{1}{2}.unitypackage", outputPath, System.IO.Path.DirectorySeparatorChar, packageName);

			string[] folders = xmlDoc.DocumentElement.GetElementsByTagName("folder").Cast<XmlElement>().Where(e => e.HasAttribute("name")).Select(
				e => string.Format("Assets{0}{1}", System.IO.Path.DirectorySeparatorChar, e.GetAttribute("name"))).ToArray();

			foreach(string folder in folders)
			{
				CoreLogger.LogDebug("PackageManager", string.Format("including folder {0} and all its subfolders", folder));
			}

			CoreLogger.LogDebug("PackageManager", string.Format("exporting {0} folders to file {1}", folders.Length, packageFilename));

			AssetDatabase.ExportPackage(folders, packageFilename, ExportPackageOptions.Recurse);

			CoreLogger.LogDebug("PackageManager", "done!");
		}	
	}
}
