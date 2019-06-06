using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

namespace TabTale.Data {

	public class LitJsonParser : IJsonParser
	{
		public DataElement Parse(string json)
		{
			JsonReader reader = new JsonReader(json);
			return ReadValue(reader);
		}

		static DataElement ReadValue(JsonReader reader)
		{
			reader.Read ();
			
			if (reader.Token == JsonToken.ArrayEnd ||
			    reader.Token == JsonToken.Null)
				return null;

			DataElement dataElement = null;

			switch(reader.Token)
			{
			case JsonToken.String:
//			case JsonToken.Double:
//			case JsonToken.Int:
//			case JsonToken.Long:
			case JsonToken.Natural:
			case JsonToken.Real:

			case JsonToken.Boolean:
				dataElement = DataElement.ToDataPrimitive(reader.Value);
				break;

			case JsonToken.ArrayStart:
				dataElement = new DataArray();
				while (true) 
				{
					DataElement item = ReadValue(reader);
					if (item == null && reader.Token == JsonToken.ArrayEnd)
						break;

					(dataElement as DataArray).Add(item);
				}
				break;

			case JsonToken.ObjectStart:
				dataElement = new DataObject();
				
				while (true) 
				{
					reader.Read ();
					
					if (reader.Token == JsonToken.ObjectEnd)
						break;
					
					string property = (string) reader.Value;
					(dataElement as DataObject)[property] = ReadValue(reader);
				}
				break;

			}

			return dataElement;
		}
	}
}
