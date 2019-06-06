using UnityEngine;
using System.Collections;


namespace CocoPlay
{
	public class CocoOptionalProperty<T>
	{
		public CocoOptionalProperty ()
		{
			m_Used = false;
			m_Value = default (T);
		}

		public CocoOptionalProperty (T value)
		{
			m_Used = true;
			m_Value = value;
		}

		[SerializeField]
		bool m_Used = false;

		public bool Used {
			get {
				return m_Used;
			}
			set {
				m_Used = value;
			}
		}

		[SerializeField]
		T m_Value;

		public T Value {
			get {
				if (m_Used) {
					return m_Value;
				}

				// ThrowExceptionIfDisabled ();
				return default (T);
			}
			set {
				if (m_Used) {
					m_Value = value;
				}

				// ThrowExceptionIfDisabled ();
			}
		}

		void ThrowExceptionIfDisabled ()
		{
			if (!m_Used) {
				throw new System.FieldAccessException (string.Format ("{0}: is NOT enabled!", GetType ()));
			}
		}

		public bool ValueIsEquals (CocoOptionalProperty<T> other)
		{
			if (other == null) {
				return false;
			}

			if (Used == other.Used) {
				if (!Used) {
					return true;
				}

				if (Value == null && other.Value == null) {
					return true;
				}
				if (Value == null || other.Value == null) {
					return false;
				}
				return ValueNonNullIsEquals (other.Value);
			}

			return false;
		}

		public static bool ValueIsEquals (CocoOptionalProperty<T> p1, CocoOptionalProperty<T> p2)
		{
			if (p1 == null) {
				return p2 == null;
			}

			return p1.ValueIsEquals (p2);
		}

		protected virtual bool ValueNonNullIsEquals (T otherValue)
		{
			return m_Value.Equals (otherValue);
		}

		public override string ToString ()
		{
			return string.Format ("[CocoOptionalProperty: Used={0}, Value={1}]", Used, Value);
		}
	}

	[System.Serializable]
	public class CocoOptionalIntProperty : CocoOptionalProperty<int>
	{
		public CocoOptionalIntProperty () : base ()
		{
		}

		public CocoOptionalIntProperty (int value) : base (value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalFloatProperty : CocoOptionalProperty<float>
	{
		public CocoOptionalFloatProperty () : base ()
		{
		}

		public CocoOptionalFloatProperty (float value) : base (value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalStringProperty : CocoOptionalProperty<string>
	{
		public CocoOptionalStringProperty () : base ()
		{
		}

		public CocoOptionalStringProperty (string value) : base (value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalVector3Property : CocoOptionalProperty<Vector3>
	{
		public CocoOptionalVector3Property () : base ()
		{
		}

		public CocoOptionalVector3Property (Vector3 value) : base (value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalVector2Property : CocoOptionalProperty<Vector2>
	{
		public CocoOptionalVector2Property () : base ()
		{
		}

		public CocoOptionalVector2Property (Vector2 value) : base (value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalSpriteProperty : CocoOptionalProperty<Sprite>
	{
		public CocoOptionalSpriteProperty () : base ()
		{
		}

		public CocoOptionalSpriteProperty (Sprite value) : base (value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalLayerMaskProperty : CocoOptionalProperty<LayerMask>
	{
		public CocoOptionalLayerMaskProperty () : base ()
		{
		}

		public CocoOptionalLayerMaskProperty (LayerMask value) : base (value)
		{
		}

		public CocoOptionalLayerMaskProperty (int value) : base ((LayerMask)value)
		{
		}
	}

	[System.Serializable]
	public class CocoOptionalRectProperty : CocoOptionalProperty<Rect>
	{
		public CocoOptionalRectProperty () : base ()
		{
		}

		public CocoOptionalRectProperty (Rect value) : base (value)
		{
		}
	}

}
