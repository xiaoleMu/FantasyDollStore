using System;
using System.Collections.Generic;
using LitJson;

namespace TabTale
{	
	public class BindingData : ICloneable
	{
		public string Bind;
		public string To;
		public List<string> Options;
		
		public object Clone()
		{
			BindingData c = new BindingData();
			c.Bind = Bind;
			c.To = To;
			c.Options = new List<string>(Options);
			return c;
		}
		public override string ToString ()
		{
			return string.Format ("{{ Bind:{0}, To:{1}, Options:{2} }}", Bind, To, Options.ToArray());
		}
	}
}