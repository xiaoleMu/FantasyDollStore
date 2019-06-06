using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CocoPlay
{
	public abstract class CocoDetectableValueDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty (position, label, property);
			Rect contentRect = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

			EditorGUI.BeginChangeCheck ();

			EditorGUIUtility.labelWidth = 40;
			SerializedProperty valueProperty = property.FindPropertyRelative ("m_Value");
			EditorGUI.PropertyField (contentRect, valueProperty, new GUIContent ("Value"));

			if (EditorGUI.EndChangeCheck ()) {
				object target = fieldInfo.GetValue (property.serializedObject.targetObject);
				UpdateTargetValue (target, valueProperty);
				EditorUtility.SetDirty (property.serializedObject.targetObject);
			}

			EditorGUI.EndProperty ();
		}

		protected abstract void UpdateTargetValue (object target, SerializedProperty valueProperty);
	}

	[CustomPropertyDrawer (typeof(CocoDetectableIntValue))]
	public class CocoDetectableIntValueDrawer : CocoDetectableValueDrawer
	{
		protected override void UpdateTargetValue (object target, SerializedProperty valueProperty)
		{
			((CocoDetectableIntValue)target).Value = valueProperty.intValue;
		}
	}

	[CustomPropertyDrawer (typeof(CocoDetectableFloatValue))]
	public class CocoDetectableFloatValueDrawer : CocoDetectableValueDrawer
	{
		protected override void UpdateTargetValue (object target, SerializedProperty valueProperty)
		{
			((CocoDetectableFloatValue)target).Value = valueProperty.floatValue;
		}
	}

	[CustomPropertyDrawer (typeof(CocoDetectableBoolValue))]
	public class CocoDetectableBoolValueDrawer : CocoDetectableValueDrawer
	{
		protected override void UpdateTargetValue (object target, SerializedProperty valueProperty)
		{
			((CocoDetectableBoolValue)target).Value = valueProperty.boolValue;
		}
	}

	[CustomPropertyDrawer (typeof(CocoDetectableVector3Value))]
	public class CocoDetectableVector3ValueDrawer : CocoDetectableValueDrawer
	{
		protected override void UpdateTargetValue (object target, SerializedProperty valueProperty)
		{
			((CocoDetectableVector3Value)target).Value = valueProperty.vector3Value;
		}
	}
}
