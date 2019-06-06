using UnityEngine;
using System.Collections;
using TabTale.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.Data {

	public class ObservableDataArray : DataArray, INotifyCollectionChanged<DataElement>
	{
		#region IPublisher implementation
		
		private IList<CollectionChangedEventHandler<DataElement>> _handlers = 
			new List<CollectionChangedEventHandler<DataElement>>();
		private IList<WeakReference<CollectionChangedEventHandler<DataElement>>> _autoReleaseHandlers = 
			new List<WeakReference<CollectionChangedEventHandler<DataElement>>>();
		
		public void Subscribe (CollectionChangedEventHandler<DataElement> handler)
		{
			if(_handlers.Contains(handler))
				return;
			
			_handlers.Add(handler);
		}
		
		public void Subscribe (IRetainer<CollectionChangedEventHandler<DataElement>> target, CollectionChangedEventHandler<DataElement> handler)
		{
			_autoReleaseHandlers.Add(new WeakReference<CollectionChangedEventHandler<DataElement>>(handler));
			target.Retain(handler);
		}
		
		public void UnSubscribe (CollectionChangedEventHandler<DataElement> handler)
		{
			_handlers.Remove(handler);
		}
		
		public void UnSubscribe (object target)
		{
			_handlers.RemoveAll(h => h.Target == target);
		}
		
		private void InvokeHandlers(CollectionAction action, ICollection<DataElement> newItems, ICollection<DataElement> oldItems)
		{
			foreach(CollectionChangedEventHandler<DataElement> handler in _handlers)
			{
				handler(this, action, newItems, oldItems);
            }
            
            _autoReleaseHandlers.RemoveAll(wr => !wr.IsAlive);
            
            foreach(CollectionChangedEventHandler<DataElement> handler in _autoReleaseHandlers.Select(wr => wr.Target))
            {
                handler(this, action, newItems, oldItems);
            }
        }
        
        #endregion
	}
}
