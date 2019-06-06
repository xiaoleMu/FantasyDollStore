using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	/// <summary>
	/// A retainer is an object retains (dah) references to other objects dynamically.
	/// This allows the lifecycle of other objects to be dependent upon its own lifecycle - 
	/// when it is removed, the references it holds are also removed, and as result, if no
	/// other object is retaining those "retainees", they are also released. This is useful
	/// for creating weak references to objects which we want to be dependent on the lifecycles
	/// of other objects. Specifically, it is used for weak delegates - the target of a weak
	/// delegate should implement the IRetainer interface, and then the delegate can be 
	/// auto-released when the target is removed. 
	/// </summary>
	public interface IRetainer
	{
		void Retain(object obj);
	}
	
	public class Retainer : IRetainer
	{
		private ICollection<object> _objects = new List<object>();
		public void Retain(object obj)
		{
			if(_objects.Contains(obj))
				return;
			
			_objects.Add(obj);
		}
	}
	
	public interface IRetainer<TObject>
		where TObject : class
	{
		void Retain(TObject obj);
	}
	
	public class Retainer<TObject> : Retainer, IRetainer<TObject>
		where TObject : class
	{
		public void Retain(TObject obj)
		{
			base.Retain(obj);
		}
	}
}
