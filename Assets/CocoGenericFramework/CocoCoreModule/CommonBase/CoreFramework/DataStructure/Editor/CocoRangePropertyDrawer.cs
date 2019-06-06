using UnityEngine;
using System.Collections;
using UnityEditor;


namespace CocoPlay
{
	public class CocoRangeDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty (position, label, property);
			Rect valueRect = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			// from / to
			float valueWidth = valueRect.width * 2 / 5;
			Rect rect = new Rect (valueRect.x, valueRect.y, valueWidth, EditorGUIUtility.singleLineHeight);
			DrawSubValue (rect, property.FindPropertyRelative ("m_From"));
			rect.x = valueRect.xMax - valueWidth;
			DrawSubValue (rect, property.FindPropertyRelative ("m_To"));

			// <->
			rect.width = 26;
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.x = valueRect.center.x - rect.width / 2;
			rect.y = valueRect.center.y - EditorGUIUtility.singleLineHeight / 2;
			EditorGUI.DrawRect (rect, Color.grey);
			EditorGUI.LabelField (rect, "<->");
			
			EditorGUI.indentLevel = originalIndentLevel;

			EditorGUI.EndProperty ();
		}

		protected virtual void DrawSubValue (Rect rect, SerializedProperty value)
		{
			EditorGUI.PropertyField (rect, value, GUIContent.none);
		}
	}

	[CustomPropertyDrawer (typeof(CocoIntRange))]
	public class CocoIntRangeDrawer : CocoRangeDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoFloatRange))]
	public class CocoFloatRangeDrawer : CocoRangeDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoVector3Range))]
	public class CocoVector3RangeDrawer : CocoRangeDrawer
	{
		protected override void DrawSubValue (Rect rect, SerializedProperty value)
		{
			float originLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 15;

			EditorGUI.PropertyField (rect, value.FindPropertyRelative ("x"));
			rect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField (rect, value.FindPropertyRelative ("y"));
			rect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField (rect, value.FindPropertyRelative ("z"));

			EditorGUIUtility.labelWidth = originLabelWidth;
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 3;
		}
	}

	[CustomPropertyDrawer (typeof(CocoColorRange))]
	public class CocoColorRangeDrawer : CocoRangeDrawer
	{
	}

}
