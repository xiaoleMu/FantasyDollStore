using UnityEngine;
using System.Collections;
using UnityEditor;


namespace CocoPlay
{
	public class CocoOptionalRangeDrawer : CocoOptionalPropertyDrawer
	{
//		protected override void DrawValue (Rect position, Rect valueRect, SerializedProperty property)
//		{
//			SerializedProperty value = ValueProperty (property);
//
//			float valueWidth = valueRect.width * 2 / 5;
//
//			// from
//			Rect rect = new Rect (valueRect.x, valueRect.y, valueWidth, EditorGUIUtility.singleLineHeight);
//			DrawSubValue (rect, value.FindPropertyRelative ("m_From"));
//			rect.x = valueRect.xMax - valueWidth;
//			DrawSubValue (rect, value.FindPropertyRelative ("m_To"));
//
//			// <->
//			rect.width = 26;
//			rect.height = EditorGUIUtility.singleLineHeight;
//			rect.x = valueRect.center.x - rect.width / 2;
//			rect.y = valueRect.center.y - EditorGUIUtility.singleLineHeight / 2;
//			EditorGUI.LabelField (rect, "<->");
//		}
//
//		protected virtual void DrawSubValue (Rect rect, SerializedProperty value)
//		{
//			EditorGUI.PropertyField (rect, value, GUIContent.none);
//		}
	}

	[CustomPropertyDrawer (typeof(CocoOptionalIntRangeProperty))]
	public class CocoOptionalIntRangeDrawer : CocoOptionalRangeDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalFloatRangeProperty))]
	public class CocoOptionalFloatRangeDrawer : CocoOptionalRangeDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalVector3RangeProperty))]
	public class CocoOptionalVector3RangeDrawer : CocoOptionalRangeDrawer
	{
//		protected override void DrawSubValue (Rect rect, SerializedProperty value)
//		{
//			float originLabelWidth = EditorGUIUtility.labelWidth;
//			EditorGUIUtility.labelWidth = 15;
//
//			EditorGUI.PropertyField (rect, value.FindPropertyRelative ("x"));
//			rect.y += EditorGUIUtility.singleLineHeight;
//			EditorGUI.PropertyField (rect, value.FindPropertyRelative ("y"));
//			rect.y += EditorGUIUtility.singleLineHeight;
//			EditorGUI.PropertyField (rect, value.FindPropertyRelative ("z"));
//
//			EditorGUIUtility.labelWidth = originLabelWidth;
//		}

		protected override float ValueExpandedHeight {
			get {
				return EditorGUIUtility.singleLineHeight * 2;
			}
		}
	}
		
	[CustomPropertyDrawer (typeof(CocoOptionalColorRangeProperty))]
	public class CocoOptionalColorRangeDrawer : CocoOptionalRangeDrawer
	{
	}

}
