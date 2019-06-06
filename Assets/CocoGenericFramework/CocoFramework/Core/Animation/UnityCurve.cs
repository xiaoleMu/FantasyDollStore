using UnityEngine;
using System.Collections;

namespace TabTale.Animation
{
	public class UnityCurve : ICurve
	{
		readonly AnimationCurve _animationCurve;

		public UnityCurve(AnimationCurve curve)
		{
			_animationCurve = curve;
		}

		#region ICurve implementation

		public float Get (float ratio)
		{
			return _animationCurve.Evaluate(ratio);
		}

		#endregion
	}
}