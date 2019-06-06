using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK 
{
	public interface IPsdkGameLevelData : IPsdkService
    {
		bool SetupGameLevelData();
        IDictionary<string, object> GetData(string name);
		string GetDataFileContent(string filename);
		string GetDataFileContent(string filename,string ext);
		IPsdkGameLevelData GetImplementation();
    }
}
