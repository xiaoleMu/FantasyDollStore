using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public static class AnimatorOverrideControllerExtensions
	{
		public static string GetDump(this AnimatorOverrideController _this)
		{
			string dump = "";

			foreach(AnimationClipPair pair in _this.clips)
			{
				string originalClipName = pair.originalClip != null ? pair.originalClip.name : "null";
				string overrideClipName = pair.overrideClip != null ? pair.overrideClip.name : "null";
				dump += string.Format("{0} => {1}\n", originalClipName, overrideClipName);
			}

			return dump;
		}

		public static IEnumerable<AnimationClipPair> GetActivePairs(this AnimatorOverrideController _this)
		{
			foreach(AnimationClipPair pair in _this.clips)
			{
				if(pair.originalClip != pair.overrideClip && pair.overrideClip != null)
					yield return pair;
			}
		}
	}
}
