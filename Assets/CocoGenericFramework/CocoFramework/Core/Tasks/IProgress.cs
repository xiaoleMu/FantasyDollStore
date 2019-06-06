using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IProgress
	{
		/// <summary>
		/// A normalized value [0..1] denoting the state of the task - not
		/// all tasks can implement this accurately, but all are expected 
		/// to provide an approximation.
		/// </summary>
		/// <value>The progress.</value>
		float Progress { get; }
	}
}
