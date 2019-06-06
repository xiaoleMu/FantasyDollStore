using UnityEngine;
using System.Collections;
using UnityEditor;


namespace CocoPlay
{
	public abstract class CocoOptionalPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty (position, label, property);
			Rect contentRect = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

			SerializedProperty enabled = EnabledProperty (property);

			Rect rect = new Rect (contentRect.x, contentRect.y, 15, contentRect.height);
			EditorGUI.PropertyField (rect, enabled, GUIContent.none);

			rect.x += rect.width + 10;
			rect.width = contentRect.xMax - rect.x;
			if (enabled.boolValue) {
				DrawValue (position, rect, property);
			} else {
				EditorGUI.LabelField (rect, "Unused");
			}

			EditorGUI.EndProperty ();
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = base.GetPropertyHeight (property, label);
			SerializedProperty enabled = EnabledProperty (property);
			if (enabled.boolValue) {
				height += ValueExpandedHeight;
			}
			return height;
		}

		protected SerializedProperty EnabledProperty (SerializedProperty property)
		{
			return property.FindPropertyRelative ("m_Used");
		}

		protected SerializedProperty ValueProperty (SerializedProperty property)
		{
			return property.FindPropertyRelative ("m_Value");
		}

		protected virtual void DrawValue(Rect position, Rect valueRect, SerializedProperty property)
		{
			EditorGUI.PropertyField (valueRect, ValueProperty (property), GUIContent.none, true);
		}

		protected virtual float ValueExpandedHeight {
			get {
				return 0;
			}
		}
	}

	[CustomPropertyDrawer (typeof(CocoOptionalIntProperty))]
	public class CocoOptionalIntDrawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalFloatProperty))]
	public class CocoOptionalFloatDrawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalStringProperty))]
	public class CocoOptionalStringDrawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalVector3Property))]
	public class CocoOptionalVector3Drawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalVector2Property))]
	public class CocoOptionalVector2Drawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalSpriteProperty))]
	public class CocoOptionalSpriteDrawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalLayerMaskProperty))]
	public class CocoOptionalLayerMaskDrawer : CocoOptionalPropertyDrawer
	{
	}

	[CustomPropertyDrawer (typeof(CocoOptionalRectProperty))]
	public class CocoOptionalRectDrawer : CocoOptionalPropertyDrawer
	{
	}
}