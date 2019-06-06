using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CocoPlay
{
	[CustomPropertyDrawer (typeof(CocoUIButtonIDProperty))]
	public class CocoUIButtonIDDrawer : CocoAlternativePropertyDrawer
	{
		protected override string FirstLabel {
			get {
				return "Id";
			}
		}

		protected override string SecondLabel {
			get {
				return "Name";
			}
		}
	}

	[CustomPropertyDrawer (typeof(CocoUIButtonAudioProperty))]
	public class CocoUIButtonAudioDrawer : CocoAlternativePropertyDrawer
	{
		protected override string FirstLabel {
			get {
				return "Id";
			}
		}

		protected override string SecondLabel {
			get {
				return "Name";
			}
		}
	}
}
