using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace TabTale.Data
{
	/// <summary>
	/// A Json Data primitive - can be a number, string, boolean or null (json null, not CLR null).
	/// </summary>
	public class DataPrimitve : DataElement
	{
		private object _value;

		public override string ToString ()
		{
			return string.Format ("{0}", _value);
		}
		
		public DataPrimitve (bool value)
		{
			this._value = value;
		}
		
		public DataPrimitve (byte value)
		{
			this._value = value;
		}
		
		public DataPrimitve (char value)
		{
			this._value = value;
		}
		
		public DataPrimitve (decimal value)
		{
			this._value = value;
		}
		
		public DataPrimitve (double value)
		{
			this._value = value;
		}
		
		public DataPrimitve (float value)
		{
			this._value = value;
		}
		
		public DataPrimitve (int value)
		{
			this._value = value;
		}
		
		public DataPrimitve (long value)
		{
			this._value = value;
		}
		
		public DataPrimitve (sbyte value)
		{
			this._value = value;
		}
		
		public DataPrimitve (short value)
		{
			this._value = value;
		}
		
		public DataPrimitve (string value)
		{
			this._value = value;
		}
		
		public DataPrimitve (DateTime value)
		{
			this._value = value;
		}
		
		public DataPrimitve (uint value)
		{
			this._value = value;
		}
		
		public DataPrimitve (ulong value)
		{
			this._value = value;
		}
		
		public DataPrimitve (ushort value)
		{
			this._value = value;
		}
		
		public DataPrimitve (DateTimeOffset value)
		{
			this._value = value;
		}
		
		public DataPrimitve (Guid value)
		{
			this._value = value;
		}
		
		public DataPrimitve (TimeSpan value)
		{
			this._value = value;
		}
		
		public DataPrimitve (Uri value)
		{
			this._value = value;
		}
		
		internal object Value 
		{
			get { return _value; }
		}
		
		public override DataType DataType 
		{
			get 
			{
				// FIXME: what should we do for null? Handle it as null so far.
				if (_value == null)
					return DataType.Null;
				
				switch (Type.GetTypeCode (_value.GetType ())) 
				{
				case TypeCode.Boolean:
					return DataType.Boolean;
				case TypeCode.Char:
				case TypeCode.String:
				case TypeCode.DateTime:
				case TypeCode.Object: // DateTimeOffset || Guid || TimeSpan || Uri
					return DataType.String;
				default:
					return DataType.Number;
				}
			}
		}
		
		static readonly byte [] true_bytes = Encoding.UTF8.GetBytes ("true");
		static readonly byte [] false_bytes = Encoding.UTF8.GetBytes ("false");
		
		public override void Save (Stream stream)
		{
			switch (DataType) 
			{
			case DataType.Boolean:
				if ((bool) _value)
				{
					stream.Write (true_bytes, 0, 4);
				} else
				{
					stream.Write (false_bytes, 0, 5);
				}
				break;

			case DataType.String:
				stream.WriteByte ((byte) '\"');
				byte [] bytes = Encoding.UTF8.GetBytes (EscapeString (_value.ToString ()));
				stream.Write (bytes, 0, bytes.Length);
				stream.WriteByte ((byte) '\"');
				break;

			default:
				bytes = Encoding.UTF8.GetBytes (GetFormattedString ());
				stream.Write (bytes, 0, bytes.Length);
				break;
			}
		}

		public int ToInt()
		{
			if(_value.IsNumber())
				return (int)_value;

			return Convert.ToInt32 (_value, NumberFormatInfo.InvariantInfo);
		}
		
		internal string GetFormattedString ()
		{
			switch (DataType) 
			{
			case DataType.String:
				if (_value is string || _value == null)
					return (string) _value;
				throw new NotImplementedException ("GetFormattedString from value type " + _value.GetType ());

			case DataType.Number:
				string s;
				if (_value is float || _value is double)
				{
					// Use "round-trip" format
					s = ((IFormattable) _value).ToString ("R", NumberFormatInfo.InvariantInfo);
				} else
				{
					s = ((IFormattable) _value).ToString ("G", NumberFormatInfo.InvariantInfo);
				}

				if (s == "NaN" || s == "Infinity" || s == "-Infinity")
				{
					return "\"" + s + "\"";
				} else
				{
					return s;
				}

			default:
				throw new InvalidOperationException ();
			}
		}
	}
}