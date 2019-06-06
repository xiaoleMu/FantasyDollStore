using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface ISharedData 
	{
		string GetTableName();
		string GetId();
		string ToLogString();
		ISharedData Clone();
	}
}
