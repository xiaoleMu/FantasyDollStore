using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TabTale.Collections;

namespace TabTale.Data
{
	public class DataArray : DataElement, IList<DataElement>
	{
		private List<DataElement> _list;

		#region Constructors
		
		public DataArray (params DataElement [] items)
		{
			_list = new List<DataElement> ();
			AddRange (items);
		}
		
		public DataArray (IEnumerable<DataElement> items)
		{
			if (items == null)
				throw new ArgumentNullException ("items");
			
			_list = new List<DataElement> (items);
		}

		#endregion
		
		public override int Count 
		{
			get { return _list.Count; }
		}
		
		public bool IsReadOnly 
		{
			get { return false; }
		}
		
		public override sealed DataElement this[int index] 
		{
			get { return _list[index]; }
			set 
			{ 
				_list[index] = value; 
				_isSynced = false;
			}
		}
		
		public override DataType DataType 
		{
			get { return DataType.Array; }
		}
		
		public void Add (DataElement item)
		{
			if (item == null)
				//throw new ArgumentNullException ("item");
				return;
			
			_list.Add (item);

			_isSynced = false;
		}
		
		public void AddRange (IEnumerable<DataElement> items)
		{
			if (items == null)
				//throw new ArgumentNullException ("items");
				return;
			
			_list.AddRange (items);

			_isSynced = false;
		}
		
		public void AddRange (params DataElement [] items)
		{
			if (items == null)
				return;
			
			_list.AddRange (items);

			_isSynced = false;
		}
		
		public void Clear ()
		{
			_list.Clear ();

			_isSynced = false;
		}
		
		public bool Contains (DataElement item)
		{
			return _list.Contains (item);
		}
		
		public void CopyTo (DataElement [] array, int arrayIndex)
		{
			_list.CopyTo (array, arrayIndex);
		}
		
		public int IndexOf (DataElement item)
		{
			return _list.IndexOf (item);
		}
		
		public void Insert (int index, DataElement item)
		{
			_list.Insert (index, item);
			_isSynced = false;
		}
		
		public bool Remove (DataElement item)
		{
			_isSynced = false;
			return  _list.Remove (item);
		}
		
		public void RemoveAt (int index)
		{
			_list.RemoveAt (index);
			_isSynced = false;
		}
		
		public override void Save (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");
			stream.WriteByte ((byte) '[');
			for (int i = 0; i < _list.Count; i++) {
				DataElement v = _list [i];
				if (v != null)
					v.Save (stream);
				else {
					stream.WriteByte ((byte) 'n');
					stream.WriteByte ((byte) 'u');
                    stream.WriteByte ((byte) 'l');
                    stream.WriteByte ((byte) 'l');
                }
                
                if (i < Count - 1) {
                    stream.WriteByte ((byte) ',');
                    stream.WriteByte ((byte) ' ');
                }
            }
            stream.WriteByte ((byte) ']');
        }
        
        IEnumerator<DataElement> IEnumerable<DataElement>.GetEnumerator ()
        {
            return _list.GetEnumerator ();
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _list.GetEnumerator ();
        }
    }
}

