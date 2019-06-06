using UnityEngine;

namespace CocoPlay
{
	public class EnumFlagAttribute : PropertyAttribute
	{
		public readonly string DisplayName;

		public EnumFlagAttribute ()
		{
		}

		public EnumFlagAttribute (string name)
		{
			DisplayName = name;
		}
	}
}