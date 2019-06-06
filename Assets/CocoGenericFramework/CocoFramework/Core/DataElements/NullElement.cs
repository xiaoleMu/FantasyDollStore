using UnityEngine;
using System.Collections;

namespace TabTale.Data {

	public class NullElement : DataElement
	{
		#region implemented abstract members of DataElement

		public override DataType DataType 
		{
			get { return DataType.Null; }
		}

		#endregion

		private NullElement()
		{
		}

		private static NullElement s_instance = new NullElement();
		public static NullElement Instance
		{
			get { return s_instance; }
		}

		public override string ToString ()
		{
			return "";
		}
	}
}
