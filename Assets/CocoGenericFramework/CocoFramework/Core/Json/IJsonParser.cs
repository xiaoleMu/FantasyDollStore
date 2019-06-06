using UnityEngine;
using System.Collections;

namespace TabTale.Data {

	public interface IJsonParser
	{
		DataElement Parse(string json);
	}
}
