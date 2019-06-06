using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CocoPlay
{
	[CustomPropertyDrawer (typeof(EnumFlagAttribute))]
	public class EnumFlagDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var flagSettings = (EnumFlagAttribute)attribute;

			var targetEnum = GetBaseProperty<Enum> (property);

			var propName = flagSettings.DisplayName;
			if (string.IsNullOrEmpty (propName))
				propName = property.displayName;

			EditorGUI.BeginProperty (position, label, property);
			var enumNew = EditorGUI.EnumMaskPopup (position, propName, targetEnum);
			property.intValue = (int)Convert.ChangeType (enumNew, targetEnum.GetType ());
			EditorGUI.EndProperty ();
		}

		private static T GetBaseProperty<T> (SerializedProperty prop)
		{
			// Separate the steps it takes to get to this property
			var separatedPaths = prop.propertyPath.Split ('.');

			// Go down to the root of this serialized property
			var reflectionTarget = prop.serializedObject.targetObject as object;
			var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

			// Walk down the path to get the target object
			foreach (var path in separatedPaths) {
				var type = reflectionTarget.GetType ();
				var fieldInfo = type.GetField (path, bindingFlags);
				if (fieldInfo == null) {
					return default(T);
				}

				reflectionTarget = fieldInfo.GetValue (reflectionTarget);
			}
			return (T)reflectionTarget;
		}
	}
}