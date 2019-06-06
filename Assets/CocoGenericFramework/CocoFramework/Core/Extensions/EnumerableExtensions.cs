using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale 
{
	public static class EnumerableExtensions
	{
		public static int CalculateHash<TElement>(this IEnumerable<TElement> _this)
			where TElement : class
		{
			int code = 0;
			foreach(TElement element in _this)
			{
				code = 31 * code + (element == null ? 0 : element.GetHashCode());
			}

			return code;
		}

		public static int FirstIndex<TElement>(this IEnumerable<TElement> _this, System.Predicate<TElement> predicate)
		{
			IEnumerator<TElement> enumerator = _this.GetEnumerator();
			int index = -1;
			while(enumerator.MoveNext())
			{
				index++;
				if(predicate(enumerator.Current))
					return index;
			}
			return -1;
		}

		public static IEnumerable<TElement> SelectDistinct<TElement, TKey>(this IEnumerable<TElement> _this, System.Func<TElement, TKey> groupSelector)
		{
			return _this.GroupBy(groupSelector).Select(g => g.First());
		}

		public static DefaultingDictionary<TKey, TValue> ToDefaultingDictionary<TValue, TKey>(this IEnumerable<TValue> _this, System.Func<TValue, TKey> keySelector)
		{
			DefaultingDictionary<TKey, TValue> dictionary = new DefaultingDictionary<TKey, TValue>();

			foreach(TValue value in _this)
			{
				dictionary[keySelector(value)] = value;
			}

			return dictionary;
		}

		public static DefaultingDictionary<TKey, TValue> ToDefaultingDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> _this, 
		                                                                                               System.Func<TSource, TKey> keySelector, 
		                                                                                               System.Func<TSource, TValue> valueSelector)
		{
			DefaultingDictionary<TKey, TValue> dictionary = new DefaultingDictionary<TKey, TValue>();

			foreach(TSource source in _this)
			{
				dictionary[keySelector(source)] = valueSelector(source);
			}

			return dictionary;
		}

		public static DefaultingDictionary<TKey, TValue> ToDefaultingDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> _this, 
		                                                                                               System.Func<TSource, TKey> keySelector, 
		                                                                                               System.Func<TSource, TValue> valueSelector, TValue defaultValue)
		{
			DefaultingDictionary<TKey, TValue> dictionary = new DefaultingDictionary<TKey, TValue>(defaultValue);
			
			foreach(TSource source in _this)
			{
				dictionary[keySelector(source)] = valueSelector(source);
			}
			
			return dictionary;
		}

		public static int Max<TSource>(this IEnumerable<TSource> _this, System.Func<TSource, int> selector)
		{
			int max = int.MinValue;
			foreach(TSource element in _this)
			{
				int current = selector(element);
				if(current > max)
				{
					max = current;
				}
			}

			return max;
		}

		public static int Sum<TSource>(this IEnumerable<TSource> _this, System.Func<TSource, int> selector)
		{
			int sum = 0;
			foreach(TSource element in _this)
			{
				sum += selector(element);
			}

			return sum;
		}

		public static int Sum(this IEnumerable<int> _this)
		{
			int sum = 0;
			foreach(int element in _this)
			{
				sum += element;
			}
			
			return sum;
		}

		public static int Max(this IEnumerable<int> _this)
		{
			int max = int.MinValue;
			foreach(int element in _this)
			{
				if(element > max)
				{
					max = element;
				}
			}
			
			return max;
		}

		public static double Max(this IEnumerable<double> _this)
		{
			double max = double.MinValue;
			foreach(double element in _this)
			{
				if(element > max)
				{
					max = element;
				}
			}

			return max;
		}
		
		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> _this, TAccumulate seed,
		                                                          System.Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate accumulate = seed;
			foreach(TSource value in _this)
			{
				accumulate = func(accumulate, value);
			}

			return accumulate;
		}

		public static void ForEach<T>(this IEnumerable<T> value, System.Action<T> action)
		{
			foreach (T item in value)
			{
				action(item);
			}
		}
	}
}