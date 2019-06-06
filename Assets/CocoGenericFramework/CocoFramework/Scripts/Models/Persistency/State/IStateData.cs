using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IStateData 
	{
		string GetStateName();
		string ToLogString();
		IStateData Clone();
	}
}
