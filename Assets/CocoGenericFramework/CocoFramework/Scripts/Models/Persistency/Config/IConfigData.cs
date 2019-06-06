using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IConfigData 
	{
		string GetTableName();
		string GetId();
		string ToLogString();
		bool IsBlob();
		IConfigData Clone();
	}
}
