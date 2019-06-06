using UnityEngine;
using System.Collections;

namespace TabTale {

	public interface ILocalGameNotifiation
	{
		string Title { get; }
		string Body { get; }
		string Image { get; }
		System.DateTime Time { get; }
	}
}
