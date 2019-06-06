using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using DataPair = System.Collections.Generic.KeyValuePair<string, TabTale.Data.DataElement>;
using DataPairEnumerable = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, TabTale.Data.DataElement>>;
using TabTale.Collections;

namespace TabTale.Data
{
	public class DataObject : DataElement, IDictionary<string, DataElement>, ICollection<DataPair>
	{
		DefaultingDictionary<string, DataElement> _values;

		#region Constructors

		public DataObject()
		{
			_values = new DefaultingDictionary<string, DataElement> (Null);
		}
		
		public DataObject (params DataPair [] items)
		{
			_values = new DefaultingDictionary<string, DataElement> (Null);
			
			if (items != null)
				AddRange (items);
		}
		
		public DataObject (DataPairEnumerable items)
		{
			if (items == null)
				throw new ArgumentNullException ("items");
			
			_values = new DefaultingDictionary<string, DataElement> (Null);
			AddRange (items);
		}

		#endregion
		
		public override int Count 
		{
			get { return _values.Count; }
		}
		
		public IEnumerator<DataPair> GetEnumerator ()
		{
			return _values.GetEnumerator ();
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return _values.GetEnumerator ();
		}

		public override DataPairEnumerable GetDataPairs ()
		{
			foreach(KeyValuePair<string, DataElement> pair in _values)
			{
				yield return pair;
			}
		}
		
		public override sealed DataElement this [string key] 
		{
			get { return _values [key]; }

			set 
			{ 
				_values [key] = value; 
				_isSynced = false;
			}
		}
		
		public override DataType DataType 
		{
			get { return DataType.Object; }
		}
		
		public ICollection<string> Keys 
		{
			get { return _values.Keys; }
		}
		
		public ICollection<DataElement> Values 
		{
			get { return _values.Values; }
		}
		
		public void Add (string key, DataElement value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			_values.Add (key, value);
			_isSynced = false;
		}
		
		public void Add (DataPair pair)
		{
			Add (pair.Key, pair.Value);
			_isSynced = false;
		}
		
		public void AddRange (DataPairEnumerable items)
		{
			if (items == null)
				throw new ArgumentNullException ("items");
			
			foreach (var pair in items)
				_values.Add (pair.Key, pair.Value);
			_isSynced = false;
		}
		
		public void AddRange (params DataPair [] items)
		{
			AddRange ((DataPairEnumerable) items);
			_isSynced = false;
		}
		
		public void Clear ()
		{
			_values.Clear ();
			_isSynced = false;
		}
		
		bool ICollection<DataPair>.Contains (DataPair item)
		{
			return (_values as ICollection<DataPair>).Contains (item);
		}
		
		bool ICollection<DataPair>.Remove (DataPair item)
		{
			_isSynced = false;
			return (_values as ICollection<DataPair>).Remove (item);
		}
		
		public override bool ContainsKey (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			return _values.ContainsKey (key);
		}
		
		public void CopyTo (DataPair [] array, int arrayIndex)
		{
			(_values as ICollection<DataPair>).CopyTo (array, arrayIndex);
		}
		
		public bool Remove (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			_isSynced = false;
			return _values.Remove (key);
		}
		
		bool ICollection<DataPair>.IsReadOnly 
		{
			get { return false; }
		}
		
		public override void Save (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");
			stream.WriteByte ((byte) '{');
			foreach (DataPair pair in _values) {
				stream.WriteByte ((byte) '"');
				byte [] bytes = Encoding.UTF8.GetBytes (EscapeString (pair.Key));
				stream.Write (bytes, 0, bytes.Length);
				stream.WriteByte ((byte) '"');
				stream.WriteByte ((byte) ',');
				stream.WriteByte ((byte) ' ');
				if (pair.Value == null) {
					stream.WriteByte ((byte) 'n');
					stream.WriteByte ((byte) 'u');
					stream.WriteByte ((byte) 'l');
					stream.WriteByte ((byte) 'l');
				} else
					pair.Value.Save (stream);
			}
			stream.WriteByte ((byte) '}');
		}
		
		public bool TryGetValue (string key, out DataElement value)
		{
			return _values.TryGetValue (key, out value);
		}

		public override void Merge (DataElement other)
		{
			DataObject otherObject = other as DataObject;
			if(otherObject == null)
				return;

			foreach(KeyValuePair<string, DataElement> pair in otherObject._values)
			{
				_values.AddIfMissing(pair);
			}
		}
	}
}
