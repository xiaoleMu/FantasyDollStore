using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
    public static class OrderedEnumerable
    {
        public static IOrderedEnumerable<TElement> ThenBy<TElement, TKey>
            (this IOrderedEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            return source.CreateOrderedEnumerable(keySelector, Comparer<TKey>.Default, false);
        }

        public static IOrderedEnumerable<TElement> ThenByDescending<TElement, TKey>
            (this IOrderedEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            return source.CreateOrderedEnumerable(keySelector, Comparer<TKey>.Default, true);
        }
    }

    internal class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
    {
        private readonly IEnumerable<TElement> _source;
        private readonly Comparison<TElement> _comparison;

        internal OrderedEnumerable(IEnumerable<TElement> source, Comparison<TElement> comparison)
        {
            _source = source;
			_comparison = comparison;
        }

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> keyComparer, bool descending)
        {
            //IComparer<TElement> extraComparer = new ProjectionComparer<TElement, TKey>(keySelector, keyComparer);
			Comparison<TElement> extraComparison;
			if(descending)
			{
				extraComparison = (e1, e2) => keyComparer.Compare(keySelector(e2), keySelector(e1)) ;
			} else
			{
				extraComparison = (e1, e2) => keyComparer.Compare(keySelector(e1), keySelector(e2));
			}				

			Comparison<TElement> linkedComparison = (e1, e2) => {
				int c = _comparison(e1, e2);
				if(c == 0)
					return extraComparison(e1, e2);
				return c;
			};

			return new OrderedEnumerable<TElement>(_source, linkedComparison);
        }

        public IEnumerator<TElement> GetEnumerator()
        {            
            List<TElement> sortedList = _source.ToList();
			sortedList.Sort(_comparison);            
            
			foreach (TElement element in sortedList)
            {
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
