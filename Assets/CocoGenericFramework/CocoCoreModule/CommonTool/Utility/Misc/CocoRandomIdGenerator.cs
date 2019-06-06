using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoRandomIdGenerator
	{
		int[] m_Ids;
		int m_Count;

		int m_FirstIndex;
		int m_LastIndex;
		bool m_Backward;

		public CocoRandomIdGenerator (int pIdCount)
		{
			m_Count = pIdCount;

			if (m_Count > 0) {
				m_Ids = new int[m_Count];
				for (int i = 0; i < m_Count; i++) {
					m_Ids [i] = i;
				}

				Reset ();
			}
		}

		public int Count {
			get {
				return m_Count;
			}
		}

		public int RandomId {
			get {
				if (m_Count <= 0) {
					return -1;
				}
				if (m_Count == 1) {
					return m_Ids [0];
				}

				int index = Random.Range (m_FirstIndex, m_LastIndex);
				int id = m_Ids [index];

				//Debug.LogWarning (GetHashCode () + ": (" + m_FirstIndex + ", " + m_LastIndex + ") -> [" + index + "]" + id + ", " + m_Backward);

				if (m_Backward) {
					if (m_FirstIndex < m_Count - 1) {
						m_Ids [index] = m_Ids [m_FirstIndex];
						m_Ids [m_FirstIndex] = id;
						m_FirstIndex++;
					} else {
						m_Backward = false;
						m_FirstIndex = 0;
						m_LastIndex = m_Count - 1;
					}
				} else {
					m_LastIndex--;
					if (m_LastIndex > 0) {
						m_Ids [index] = m_Ids [m_LastIndex];
						m_Ids [m_LastIndex] = id;
					} else {
						m_Backward = true;
						m_FirstIndex = 1;
						m_LastIndex = m_Count;
					}
				}

				//string str = GetHashCode () + ": [";
				//for (int i = 0; i < m_Count; i++) {
				//	str += m_Ids [i] + ", ";
				//}
				//Debug.LogWarning (str + "]");

				return id;
			}
		}

		public void Reset ()
		{
			m_FirstIndex = 0;
			m_LastIndex = m_Count;
			m_Backward = false;
		}
	}
}
