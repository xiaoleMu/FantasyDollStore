using UnityEngine;
using System.Collections;

namespace TabTale 
{
	public interface IApplicationPreInit : IService
	{
		event System.Action<bool> Done;

		void Init();
	}

}
