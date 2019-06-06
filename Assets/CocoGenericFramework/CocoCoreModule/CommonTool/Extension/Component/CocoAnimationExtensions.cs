using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	static public class CocoAnimationExtensions
	{

		#region Animation

		public static AnimationState GetAnimState (this UnityEngine.Animation anim, string clipName = null)
		{
			AnimationClip clip = string.IsNullOrEmpty (clipName) ? anim.clip : anim.GetClip (clipName);
			return clip != null ? anim [clip.name] : null;
		}

		public static IEnumerator WaitForPlay (this UnityEngine.Animation anim, string clipName = null, bool forward = true, System.Action endAction = null)
		{
			float length = anim.Play (clipName, forward);
			if (length > 0) {
				yield return new WaitForSeconds (length);
			}

			if (endAction != null) {
				endAction ();
			}
		}

		public static void AddClips (this UnityEngine.Animation anim, UnityEngine.Animation originAnim, params string[] animNames)
		{
			AnimationClip clip;
			foreach (string clipName in animNames) {
				clip = originAnim.GetClip (clipName);
				if (clip != null) {
					anim.AddClip (clip, clipName);
				}
			}
		}

		public static float Play (this UnityEngine.Animation anim, string clipName = null, bool forward = true)
		{
			AnimationState animState = anim.GetAnimState (clipName);
			if (animState != null) {
				if (forward) {
					animState.time = 0;
					animState.speed = Mathf.Abs (animState.speed);
				} else {
					animState.time = animState.length;
					animState.speed = -Mathf.Abs (animState.speed);
				}
				anim.Play (animState.name);
				return animState.length;
			}

			return 0;
		}

		/// <summary>
		/// 断点续播，clip正在播放时候，调用当前播放方向的倒播，会从当前点倒着播回去
		/// </summary>
		/// <returns>The with breakpoint.</returns>
		/// <param name="anim">Animation.</param>
		/// <param name="clipName">Clip name.</param>
		/// <param name="forward">If set to <c>true</c> forward.</param>
		public static float PlayWithBreakpoint (this UnityEngine.Animation anim, string clipName = null, bool forward = true)
		{
			AnimationState animState = anim.GetAnimState (clipName);
			if (animState != null) {
				animState.time = animState.length * (Mathf.Min (1f, animState.normalizedTime));
				if (forward) {
					animState.speed = Mathf.Abs (animState.speed);
				} else {
					animState.speed = -Mathf.Abs (animState.speed);
				}
				anim.Play (animState.name);
				return animState.length;
			}

			return 0;
		}

		public static void Sample (this UnityEngine.Animation anim, float time, string clipName = null, bool normalized = false)
		{
			AnimationState animState = anim.GetAnimState (clipName);
			if (animState != null) {
				anim.Play (animState.name);
				if (normalized) {
					animState.normalizedTime = time;
				} else {
					animState.time = time;
				}
				animState.enabled = true;
				anim.Sample ();
				animState.enabled = false;
			}
		}

		#endregion


		#region Animator

		public static void Sample (this Animator animator, float time, string stateName, bool normalized = false)
		{
			if (normalized) {
				animator.Play (stateName, 0, time);
			} else {
				animator.PlayInFixedTime (stateName, 0, time);
			}
			animator.Update (0);
		}

		#endregion
	}
}
