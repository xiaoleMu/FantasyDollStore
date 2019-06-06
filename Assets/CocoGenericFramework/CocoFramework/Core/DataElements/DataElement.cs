using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using DataPair = System.Collections.Generic.KeyValuePair<string, TabTale.Data.DataElement>;


namespace TabTale.Data
{
	/// <summary>
	/// The basic unit for data storage, processing and sharing in the system. A DataElement is
	/// a CLR representation of a json document, with its descendant classes (DataPrimitive, 
	/// DataArray and DataObject) being actual instances.
	/// </summary>
	public abstract class DataElement : IEnumerable
	{
		static IJsonParser s_parser = new LitJsonParser();
		public static DataElement Parse(string json)
		{
			DataElement instace = s_parser.Parse(json);
			instace._json = json;
			instace._isSynced = true;
			return instace;
		}

		public bool IsNull
		{
			get { return object.ReferenceEquals(this, Null); }
		}
		
		static IEnumerable<KeyValuePair<string,DataElement>> ToDataPairEnumerable (IEnumerable<KeyValuePair<string,object>> kvpc)
		{
			foreach (var kvp in kvpc)
				yield return new KeyValuePair<string,DataElement> (kvp.Key, ToDataElement (kvp.Value));
		}
		
		static IEnumerable<DataElement> ToDataElementEnumerable (IEnumerable<object> arr)
		{
			foreach (var obj in arr)
				yield return ToDataElement (obj);
		}

		public static DataElement ToDataPrimitive(object ret)
		{
			if (ret is bool)
				return new DataPrimitve ((bool) ret);
			if (ret is byte)
				return new DataPrimitve ((byte) ret);
			if (ret is char)
				return new DataPrimitve ((char) ret);
			if (ret is decimal)
				return new DataPrimitve ((decimal) ret);
			if (ret is double)
				return new DataPrimitve ((double) ret);
			if (ret is float)
				return new DataPrimitve ((float) ret);
			if (ret is int)
				return new DataPrimitve ((int) ret);
			if (ret is long)
				return new DataPrimitve ((long) ret);
			if (ret is sbyte)
				return new DataPrimitve ((sbyte) ret);
			if (ret is short)
				return new DataPrimitve ((short) ret);
			if (ret is string)
				return new DataPrimitve ((string) ret);
			if (ret is uint)
				return new DataPrimitve ((uint) ret);
			if (ret is ulong)
				return new DataPrimitve ((ulong) ret);
			if (ret is ushort)
				return new DataPrimitve ((ushort) ret);
			if (ret is DateTime)
				return new DataPrimitve ((DateTime) ret);
			if (ret is DateTimeOffset)
				return new DataPrimitve ((DateTimeOffset) ret);
			if (ret is Guid)
				return new DataPrimitve ((Guid) ret);
			if (ret is TimeSpan)
				return new DataPrimitve ((TimeSpan) ret);
			if (ret is Uri)
				return new DataPrimitve ((Uri) ret);
			//throw new NotSupportedException (String.Format ("Unexpected parser return type: {0}", ret.GetType ()));
			return Null;
		}
		
		public static DataElement ToDataElement (object ret)
		{
			if (ret == null)
				return null;
			var kvpc = ret as IEnumerable<KeyValuePair<string,object>>;
			if (kvpc != null)
				return new DataObject (ToDataPairEnumerable (kvpc));
			var arr = ret as IEnumerable<object>;
			if (arr != null)
				return new DataArray (ToDataElementEnumerable (arr));

			return ToDataPrimitive(ret);
		}

		public static DataElement Null
		{
			get { return NullElement.Instance; }
		}
		
		public virtual int Count 
		{
			get { /*throw new InvalidOperationException ();*/ return 0; }
		}
		
		public abstract DataType DataType { get; }
	

		public virtual IEnumerable<KeyValuePair<string, DataElement>> GetDataPairs()
		{
			yield break;
		}
		
		public virtual DataElement this [int index] 
		{
			get { return Null; }
			set { /*throw new InvalidOperationException ();*/ }
		}
		
		public virtual DataElement this [string key] 
		{
			get { return Null; }
			set { /*throw new InvalidOperationException ();*/ }
		}
		
		public virtual bool ContainsKey (string key)
		{
			//throw new InvalidOperationException ();
			return false;
		}
		
		public virtual void Save (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");
			Save (new StreamWriter (stream));
		}
		
		public virtual void Save (TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException ("textWriter");
			SaveInternal (textWriter);
		}
		
		void SaveInternal (TextWriter w)
		{
			switch (DataType) 
			{
			case DataType.Object:
				w.Write ('{');
				bool following = false;
				foreach (DataPair pair in ((DataObject) this)) 
				{
					if (following)
						w.Write (", ");
					w.Write ('\"');
					w.Write (EscapeString (pair.Key));
					w.Write ("\": ");
					if (pair.Value == null)
					{
						w.Write ("null");
					} else
					{
						pair.Value.SaveInternal (w);
					}
					following = true;
				}
				w.Write ('}');
				break;

			case DataType.Array:
				w.Write ('[');
				following = false;
				foreach (DataElement v in ((DataArray) this)) 
				{
					if (following)
						w.Write (", ");
					if (v != null) 
					{
						v.SaveInternal (w);
					} else
					{
						w.Write ("null");
					}
					following = true;
				}
				w.Write (']');
				break;

			case DataType.Boolean:
				w.Write ((bool) this ? "true" : "false");
				break;

			case DataType.String:
				w.Write ('"');
				w.Write (EscapeString (((DataPrimitve) this).GetFormattedString ()));
				w.Write ('"');
				break;

			default:
				w.Write (((DataPrimitve) this).GetFormattedString ());
				break;
			}
		}

		protected string _json = "";
		protected bool _isSynced = false;
		public override string ToString ()
		{
			if(!_isSynced)
			{
				StringWriter sw = new StringWriter ();
				Save (sw);
				_json = sw.ToString ();
				_isSynced = true;
			}

			return _json;
		}

		private IEnumerator Enumerator()
		{
			yield break;
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return Enumerator();
		}
		
		internal string EscapeString (string src)
		{
			if (src == null)
				return null;
			
			for (int i = 0; i < src.Length; i++)
			if (src [i] == '"' || src [i] == '\\') {
				var sb = new StringBuilder ();
				if (i > 0)
					sb.Append (src, 0, i);
				return DoEscapeString (sb, src, i);
			}
			return src;
		}
		
		string DoEscapeString (StringBuilder sb, string src, int cur)
		{
			int start = cur;
			for (int i = cur; i < src.Length; i++)
			if (src [i] == '"' || src [i] == '\\') {
				sb.Append (src, start, i - start);
				sb.Append ('\\');
				sb.Append (src [i]);
				start = i + 1;
			}
			sb.Append (src, start, src.Length - start);
			return sb.ToString ();
		}
		
		// CLI -> JsonValue
		
		public static implicit operator DataElement (bool value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (byte value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (char value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (decimal value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (double value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (float value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (int value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (long value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (sbyte value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (short value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (string value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (uint value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (ulong value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (ushort value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (DateTime value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (DateTimeOffset value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (Guid value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (TimeSpan value)
		{
			return new DataPrimitve (value);
		}
		
		public static implicit operator DataElement (Uri value)
		{
			return new DataPrimitve (value);
		}
		
		// JsonValue -> CLI

		public static implicit operator bool (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return false;
			return Convert.ToBoolean (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator byte (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToByte (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator char (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return '\0';
			return Convert.ToChar (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator decimal (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToDecimal (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator double (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToDouble (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator float (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0.0f;
			return Convert.ToSingle (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator int (DataElement value)
		{
			if (value == null)
				return 0;

			DataPrimitve primitive = value as DataPrimitve;
			if(primitive == null)
				return 0;

			return primitive.ToInt();
		}
		
		public static implicit operator long (DataElement value)
		{
			if (value == null)
				return 0;

			return Convert.ToInt64 (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator sbyte (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToSByte (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator short (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToInt16 (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator string (DataElement value)
		{
			if (value == null)
				return "";
			return value.ToString();
		}
		
		public static implicit operator uint (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToUInt16 (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator ulong (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToUInt64(((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator ushort (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return 0;
			return Convert.ToUInt16 (((DataPrimitve) value).Value, NumberFormatInfo.InvariantInfo);
		}
		
		public static implicit operator DateTime (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return DateTime.MinValue;
			return (DateTime) ((DataPrimitve) value).Value;
		}
		
		public static implicit operator DateTimeOffset (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return DateTimeOffset.MinValue;
			return (DateTimeOffset) ((DataPrimitve) value).Value;
		}
		
		public static implicit operator TimeSpan (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return TimeSpan.MinValue;
			return (TimeSpan) ((DataPrimitve) value).Value;
		}
		
		public static implicit operator Guid (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return Guid.Empty;
			return (Guid) ((DataPrimitve) value).Value;
		}

		private static Uri s_uri = new Uri("http://0.0.0.0/");

		public static Uri ErrorUri
		{
			get { return s_uri; }
		}
		
		public static implicit operator Uri (DataElement value)
		{
			if (value == null)
				//throw new ArgumentNullException ("value");
				return s_uri;
			return (Uri) ((DataPrimitve) value).Value;
		}

		public virtual void Merge(DataElement other)
		{
		}	
	}
}
