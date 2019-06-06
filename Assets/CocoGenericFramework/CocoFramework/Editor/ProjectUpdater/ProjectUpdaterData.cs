using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace TabTale.ProjectUpdater
{
	[XmlRoot("packages")]
	public class PackagesContainer
	{
		[XmlElement("package")]
		public PackageData[] packagesData { get; set; }
	}
	
	public class PackageData
	{
		[XmlElement("name")]
		public string packageName { get; set; }
		
		[XmlElement("path")]
		public string packagePath { get; set; }
		
		[XmlElement("destination")]
		public string packageDestination { get; set; }
		
		[XmlElement("description")]
		public string packageDescription { get; set; }
		
		
	}
	
	[XmlRoot("packages")]
	public class PackageInstallationDataContainer
	{
		[XmlElement("package")]
		public List<PackageInstallationData> packagesData { get; set; }
	}
	
	
	
	public class PackageInstallationData
	{
		[XmlAttribute("name")]
		public string packageName { get; set; }
	}
}
