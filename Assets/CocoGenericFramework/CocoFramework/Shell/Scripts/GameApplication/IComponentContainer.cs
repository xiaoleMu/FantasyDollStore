using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IComponentContainer
	{
		TComponent GetComponent<TComponent>() where TComponent : Component;
	}
}
