using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public static class EnumerableAction
	{
		static IEnumerableAction s_nullAction;
		static EnumerableAction()
		{
			s_nullAction = new Action(NullAction, 1, "NullAction");
		}

		public static IEnumerableAction Null
		{
			get { return s_nullAction; }
		}

		class Action : Action<object>, IEnumerableAction
		{
			public Action(System.Func<IEnumerator> actionFactory, int estimatedIterations = int.MaxValue, string name = "")
				: base(actionFactory, () => null, estimatedIterations, name)
			{
			}
		}

		class Action<T> : IEnumerableAction<T>
		{
			protected System.Func<IEnumerator> _actionFactory;
			protected System.Func<T> _resultGetter;
			int _estimatedIterations;
			string _name;
			public Action(System.Func<IEnumerator> actionFactory, System.Func<T> resultGetter, int estimateIterations = int.MaxValue, string name = "")
			{
				_actionFactory = actionFactory;
				_resultGetter = resultGetter;
				_estimatedIterations = estimateIterations;
				_name = name;
			}

			#region IEnumerableAction implementation

			public string Name
			{
				get { return _name; }
			}

			public float EstimateProgress(int iteration)
			{
				return (float)iteration / (float)_estimatedIterations;
			}

			public event System.Action<T> ResultSet = result => {};

			class Enumerator : IEnumerator
			{
				IEnumerator _enumerator = null;
				T _result;
				System.Func<T> _resultGetter;
				System.Action<T> _resultHandler;
				public Enumerator(System.Func<IEnumerator> factory, System.Func<T> resultGetter, System.Action<T> resultHandler)
				{
					_resultGetter = resultGetter;
					_enumerator = factory();
					_resultHandler = resultHandler;
				}

				public bool MoveNext()
				{
					bool b = _enumerator.MoveNext();
					if(!b)
					{
						_result = _resultGetter();
						_resultHandler(_result);
					}

					return b;
				}

				public object Current
				{
					get { return _enumerator.Current; }
				}

				public void Reset()
				{
					_enumerator.Reset();
				}
			}

			public IEnumerator Run (System.Action<T> handler)
			{
				ResultSet += handler;
				return Run ();
			}

			public IEnumerator Run ()
			{
				IEnumerator enumerator = new Enumerator(_actionFactory, _resultGetter, result => {
					ResultSet(result);
				});
				return enumerator;
			}

			#endregion
		}

		public static IEnumerator WrapAction(System.Action action)
		{
			action();
			yield break;
		}

		#region Creation of IEnumerableAction objects

		public static IEnumerableAction Create(System.Action action)
		{
			return new Action(() => WrapAction(action));
		}

		public static IEnumerableAction Create(System.Func<IEnumerator> actionFactory)
		{
			return new Action(actionFactory);
		}

		public static IEnumerableAction<T> Create<T>(System.Func<IEnumerator> actionFactory, System.Func<T> resultGetter)
		{
			return new Action<T>(actionFactory, resultGetter);
        }

		public static IEnumerableAction<T> Create<T>(System.Func<IEnumerator> actionFactory, System.Func<T> resultGetter, int estimatedIterations)
		{
			return new Action<T>(actionFactory, resultGetter, estimatedIterations, "<unkonwn>");
        }

		public static IEnumerableAction<T> Create<T>(System.Func<IEnumerator> actionFactory, System.Func<T> resultGetter, int estimatedIterations, string name)
		{
			return new Action<T>(actionFactory, resultGetter, estimatedIterations, name);
		}

		public static IEnumerableAction Create(System.Func<IEnumerator> actionFactory, int estimatedIterations, string name)
		{
			return new Action(actionFactory, estimatedIterations, name);
        }

		public static IEnumerableAction Create(System.Func<bool> predicate)
		{
			return new Action(() => WaitForPredicate(predicate));
		}

		#endregion

		public static IEnumerator NullAction()
		{
			yield break;
        }

		static IEnumerator WaitForPredicate(System.Func<bool> predicate, float timeout = -1f)
		{
			float start = Time.time;
			
			while(Time.time - start <= timeout || timeout < 0)
			{
				if(predicate())
				{
					yield break;
                } else
                {
                    yield return null;
                }			
            }
        }

		#region Compositing Actions        

		public static IEnumerator FromActions(IEnumerable<System.Action> actions)
		{
			foreach(var action in actions)
			{
				action();
				yield return null;
			}
		}

		static IEnumerator _Serialize(IEnumerable<System.Func<IEnumerator>> actions)
		{
			foreach(System.Func<IEnumerator> action in actions)
			{
				IEnumerator enumerator = action();
				while(enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
		}

		static IEnumerator _Parallelize(IEnumerable<System.Func<IEnumerator>> actionFactories)
		{
			IList<IEnumerator>[] actions = new IList<IEnumerator>[2];
			actions[0] = actionFactories.Select(af => af()).ToList();
			actions[1] = new List<IEnumerator>();

			int current = 0;
			int next;
			do
			{
				next = (current + 1) % 2;
				actions[next].Clear();
				foreach(IEnumerator action in actions[current])
				{
					if(action.MoveNext())
					{
						actions[next].Add(action);
					};
				}

				yield return null;

				current = next;

			} while(actions[next].Count > 0);
		}

		static IEnumerator _ParallelizeExpanded(IEnumerable<System.Func<IEnumerator>> actionFactories)
		{
			IList<IEnumerator>[] actions = new IList<IEnumerator>[2];
			actions[0] = actionFactories.Select(af => af()).ToList();
			actions[1] = new List<IEnumerator>();
			
			int current = 0;
			int next;
			do
			{
				next = (current + 1) % 2;
				actions[next].Clear();
				foreach(IEnumerator action in actions[current])
				{
					if(action.MoveNext())
                    {
                        actions[next].Add(action);
                    };
                }

				yield return null;                
                
                current = next;
                
            } while(actions[next].Count > 0);
		}

		public static System.Func<IEnumerator> Parallelize(IEnumerable<System.Func<IEnumerator>> actions)
		{
			return () => _Parallelize(actions);
		}

		public static System.Func<IEnumerator> ParallelizeExpanded(IEnumerable<System.Func<IEnumerator>> actions)
		{
			return () => _ParallelizeExpanded(actions);
		}

		static IEnumerator _Serialize(IEnumerable<IEnumerableAction> actions)
		{
			foreach(IEnumerableAction action in actions)
			{
				IEnumerator enumerator = action.Run();
				while(enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
		}

		public static System.Func<IEnumerator> Serialize(IEnumerable<System.Func<IEnumerator>> actions)
		{
			return () => _Serialize(actions);
		}

		public static IEnumerableAction Serialize(IEnumerable<IEnumerableAction> actions)
		{
			return Create (() => _Serialize(actions));
		}

		static IEnumerator _Concat(System.Func<IEnumerator> action1, System.Func<IEnumerator> action2)
		{
			IEnumerator enumerator = action1();
			while(enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
			
			enumerator = action2();
			while(enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		static IEnumerator _Concat(IEnumerableAction action1, IEnumerableAction action2)
		{
			IEnumerator enumerator = action1.Run();
			while(enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
			
			enumerator = action2.Run();
			while(enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		public static System.Func<IEnumerator> Concat(System.Func<IEnumerator> action1, System.Func<IEnumerator> action2)
		{
			return () => _Concat(action1, action2);
		}

		public static IEnumerableAction Concat(IEnumerableAction action1, IEnumerableAction action2)
		{
			return Create(() => _Concat(action1, action2));
		}

		#endregion
	}
}
