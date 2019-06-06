using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace CocoPlay {
	public class GameBetterList<T>{

		public int size = 0;
		public T[] buffer;

		[DebuggerHidden]
		public T this[int i]
		{
			get { return buffer[i]; }
			set { buffer[i] = value; }
		}

		public void Clear () { size = 0; }

		public void Add (T item)
		{
			if (buffer == null || size == buffer.Length) AllocateMore();
			buffer[size++] = item;
		}

		void AllocateMore ()
		{
			T[] newList = (buffer != null) ? new T[Mathf.Max(buffer.Length << 1, 32)] : new T[32];
			if (buffer != null && size > 0) buffer.CopyTo(newList, 0);
			buffer = newList;
		}
	}
}
