using UnityEngine;
using System.Collections;
using TabTale.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.Data {

	public class ObservableDataObject : DataObject, INotifyCollectionChanged<KeyValuePair<string, DataElement>>
	{
		#region IPublisher implementation
		
		private IList<CollectionChangedEventHandler<KeyValuePair<string, DataElement>>> _handlers = 
			new List<CollectionChangedEventHandler<KeyValuePair<string, DataElement>>>();
		private IList<WeakReference<CollectionChangedEventHandler<KeyValuePair<string, DataElement>>>> _autoReleaseHandlers = 
			new List<WeakReference<CollectionChangedEventHandler<KeyValuePair<string, DataElement>>>>();
		
		public void Subscribe (CollectionChangedEventHandler<KeyValuePair<string, DataElement>> handler)
		{
			if(_handlers.Contains(handler))
				return;
			
			_handlers.Add(handler);
		}
		
		public void Subscribe (IRetainer<CollectionChangedEventHandler<KeyValuePair<string, DataElement>>> target, CollectionChangedEventHandler<KeyValuePair<string, DataElement>> handler)
		{
			_autoReleaseHandlers.Add(new WeakReference<CollectionChangedEventHandler<KeyValuePair<string, DataElement>>>(handler));
			target.Retain(handler);
		}
		
		public void UnSubscribe (CollectionChangedEventHandler<KeyValuePair<string, DataElement>> handler)
		{
			_handlers.Remove(handler);
		}
		
		public void UnSubscribe (object target)
		{
			_handlers.RemoveAll(h => h.Target == target);
		}
		
		private void InvokeHandlers(CollectionAction action, ICollection<KeyValuePair<string, DataElement>> newItems, ICollection<KeyValuePair<string, DataElement>> oldItems)
		{
			foreach(CollectionChangedEventHandler<KeyValuePair<string, DataElement>> handler in _handlers)
			{
				handler(this, action, newItems, oldItems);
			}
			
			_autoReleaseHandlers.RemoveAll(wr => !wr.IsAlive);
			
			foreach(CollectionChangedEventHandler<KeyValuePair<string, DataElement>> handler in _autoReleaseHandlers.Select(wr => wr.Target))
			{
				handler(this, action, newItems, oldItems);
			}
		}
		
		#endregion
	}
}
