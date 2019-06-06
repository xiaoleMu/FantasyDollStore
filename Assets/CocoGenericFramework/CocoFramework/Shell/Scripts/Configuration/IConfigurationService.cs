using UnityEngine;
using System.Collections;
using TabTale.Data;

namespace TabTale 
{
	public interface IConfigurationService : IService
	{
		DataElement GetConfiguration(string id = "");
	}
}
