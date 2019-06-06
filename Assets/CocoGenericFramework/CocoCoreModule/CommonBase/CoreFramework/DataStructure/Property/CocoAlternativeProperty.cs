using UnityEngine;
using System.Collections;


namespace CocoPlay
{
	public class CocoAlternativeProperty<T, U>
	{
		public CocoAlternativeProperty ()
		{
			m_FirstUsed = true;
			m_First = default(T);
			m_Second = default (U);
		}

		public CocoAlternativeProperty (T value)
		{
			m_FirstUsed = true;
			m_First = value;
			m_Second = default (U);
		}

		public CocoAlternativeProperty (U value)
		{
			m_FirstUsed = false;
			m_First = default(T);
			m_Second = value;
		}

		[SerializeField]
		bool m_FirstUsed = true;

		public bool FirstUsed {
			get {
				return m_FirstUsed;
			}
			set {
				m_FirstUsed = value;
			}
		}

		[SerializeField]
		T m_First;

		public T First {
			get {
				if (m_FirstUsed) {
					return m_First;
				}

				//ThrowExceptionIfUnused (true);
				return default (T);
			}
			set {
				if (m_FirstUsed) {
					m_First = value;
				}

				//ThrowExceptionIfUnused (true);
			}
		}

		[SerializeField]
		U m_Second;

		public U Second {
			get {
				if (!m_FirstUsed) {
					return m_Second;
				}

				//ThrowExceptionIfUnused (false);
				return default (U);
			}
			set {
				if (!m_FirstUsed) {
					m_Second = value;;
				}

				//ThrowExceptionIfUnused (false);
			}
		}

		void ThrowExceptionIfUnused (bool first)
		{
			if (m_FirstUsed != first) {
				throw new System.FieldAccessException (string.Format ("{0}: {1} is NOT used!", GetType (), (first ? "first" : "second")));
			}
		}

		public bool ValueIsEquals (CocoAlternativeProperty<T, U> other)
		{
			if (other == null) {
				return false;
			}

			if (FirstUsed == other.FirstUsed) {
				if (FirstUsed) {
					if (First == null && other.First == null) {
						return true;
					}
					if (First == null || other.First == null) {
						return false;
					}
					return FirstNonNullIsEquals (other.First);
				} else {
					if (Second == null && other.Second == null) {
						return true;
					}
					if (Second == null || other.Second == null) {
						return false;
					}
					return SecondNonNullIsEquals (other.Second);
				}
			}

			return false;
		}

		public static bool ValueIsEquals (CocoAlternativeProperty<T, U> p1, CocoAlternativeProperty<T, U> p2)
		{
			if (p1 == null) {
				return p2 == null;
			}

			return p1.ValueIsEquals (p2);
		}

		protected virtual bool FirstNonNullIsEquals (T first)
		{
			return m_First.Equals (first);
		}

		protected virtual bool SecondNonNullIsEquals (U second)
		{
			return m_Second.Equals (second);
		}

		public override string ToString ()
		{
			string strValue = FirstUsed ? string.Format ("First={0}", First) : string.Format ("Second={0}", Second);
			return string.Format ("[CocoAlternativeProperty: FirstUsed={0}, {1}]", FirstUsed, strValue);
		}
	}

}
