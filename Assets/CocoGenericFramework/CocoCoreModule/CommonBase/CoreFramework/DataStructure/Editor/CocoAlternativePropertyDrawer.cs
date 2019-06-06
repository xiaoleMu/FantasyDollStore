using UnityEngine;
using System.Collections;
using UnityEditor;


namespace CocoPlay
{
	public abstract class CocoAlternativePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty (position, label, property);
			position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

			SerializedProperty firstUsed = property.FindPropertyRelative ("m_FirstUsed");

			Rect rect = new Rect (position.x, position.y, 15, position.height);
			EditorGUI.PropertyField (rect, firstUsed, GUIContent.none);

			string valueLabel = string.Empty;
			string valueName = string.Empty;
			if (firstUsed.boolValue) {
				valueLabel = FirstLabel;
				valueName = "m_First";
			} else {
				valueLabel = SecondLabel;
				valueName = "m_Second";
			}

			rect.x += rect.width + 5;
			rect.width = 45;
			EditorGUI.LabelField (rect, valueLabel);

			rect.x += rect.width + 5;
			rect.width = position.xMax - rect.x;
			EditorGUI.PropertyField (rect, property.FindPropertyRelative (valueName), GUIContent.none);

			EditorGUI.EndProperty ();
		}

		protected virtual string FirstLabel {
			get {
				return "First";
			}
		}

		protected virtual string SecondLabel {
			get {
				return "Second";
			}
		}
	}

}