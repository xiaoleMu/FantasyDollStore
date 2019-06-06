using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Collections {

	public enum CollectionAction
	{
		Add, 
		Move, 
		Remove, 
		Replace, 
		Reset
	};

	public delegate void CollectionChangedEventHandler<TItem>(object sender, 
	                                                          CollectionAction action, 
	                                                          ICollection<TItem> oldItems, 
	                                                          ICollection<TItem> newItems) ;

	public interface INotifyCollectionChanged<TItem> : IPublisher<CollectionChangedEventHandler<TItem>>
	{
	}
}
