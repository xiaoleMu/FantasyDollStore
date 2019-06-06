using UnityEngine;
using System.Collections;

namespace TabTale.Animation
{
	public interface ICurve
	{
		float Get(float ratio);
	}
}