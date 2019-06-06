using UnityEngine;
using System.Collections;
using TabTale.Data;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.AssetManagement 
{
	public class UnityAssetManager : IAssetManager
	{
		ICoroutineFactory _owner;
		public UnityAssetManager(ICoroutineFactory owner)
		{
			_owner = owner;
		}

		IEnumerator Resulter<T>(ResourceLoader<T> loader)
			where T : Object
		{
			yield return null;
			loader.Start(result => {});
		}

		public class ResourceLoader<T> : ITask<T>
			where T : Object
		{
			public ResourceLoader(T resource)
			{
				_startTime = Time.time;
				_resource = resource;
				_name = string.Format("ResourceLoader:{0}", resource.name);
			}

			string _name;
			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			string _asset;
			public ResourceLoader(string asset)
			{
				_asset = asset;
			}

			public ITask Start(float timeout = -1f)
			{
				return Start (result => {}, timeout);
			}

			public ITask Start(System.Action<TaskEnding> resultHandler, float timeout = -1f)
			{
				_taskState = TaskState.Started;

				Done += resultHandler;

				_resource = Resources.Load(_asset) as T;
				if(_resource == null)
				{
					CoreLogger.LogWarning("UnityResourceManager", string.Format("resource {0} not found in project!", _asset));
				}
				
				_startTime = Time.time;
				Done(TaskEnding.Done);
				if(_resource != null)
					ResultSet(_resource);

				_taskState = TaskState.Done;

				return this;
			}

			#region ITask implementation

			public event System.Action<TaskEnding> Done = (result) => {};

			public void Cancel(System.Action<TaskEnding> resultHandler)
			{
				Done += resultHandler;
				Cancel ();
			}

			public void Cancel()
			{
				CoreLogger.LogWarning(LoggerModules.SceneManager, string.Format("resource loader {0} received Cancel, which is not supported!", _name));
			}

			public TaskEnding Handle ()
			{
				return TaskEnding.Done;
			}

			TaskState _taskState = TaskState.Ready;
			public TaskState State 
			{
				get { return _taskState; }
			}

			private float _startTime;
			public float StartTime 
			{
				get { return _startTime; }
			}

			public float Progress 
			{
				get { return 1.0f; }
			}

			#endregion

			#region IResourceLoader implementation

			private T _resource;
			public T Result 
			{
				get { return _resource; }
			}

			public event System.Action<T> ResultSet = r => {};

			#endregion
		}

		#region IAssetManager implementation

		public ITask<T> LoadResource<T> (string name)
			where T : Object
		{
			ResourceLoader<T> loader = new ResourceLoader<T>(GetResource<T>(name));
			_owner.StartCoroutine(() => Resulter(loader));
			return loader;
		}

		public System.Func<IEnumerator> GetBundleCacher(IEnumerable<string> bundleNames)
		{
			return EnumerableAction.NullAction;
		}

		public System.Func<IEnumerator> GetResourceCacher(IEnumerable<string> resourceNames)
		{
			return EnumerableAction.NullAction;
		}

		public void LoadResources(IEnumerable<string> resourceNames, System.Action<DefaultingDictionary<string, Object>> handler)
		{
			DefaultingDictionary<string, Object> resources = new DefaultingDictionary<string, Object>();
			
			int pending = resourceNames.Count();
			if(pending == 0)
			{
				handler(resources);
				return;
			}
			
			foreach(string name in resourceNames)
			{
				ITask<Object> loader = LoadResource<Object>(name);
				loader.Start(result => {
					if(result.IsOk())
					{
						resources[name] = loader.Result;
					}
					pending--;
					if(pending == 0)
					{
						handler(resources);
					}
				});
			}
		}

		public T GetResource<T>(string name) 
			where T : Object
		{
			return GetResource<T>(name, false);
		}

		public T GetResource<T>(string name, bool isCascading) where T : Object
		{
			Object obj = null;
			if(isCascading)
			{
				obj = GameApplication.LoadFrameworkResource(name) as T;
			}
			if(obj == null)
			{
				obj = Resources.Load(name);
			}
			if(obj == null)
			{
				CoreLogger.LogWarning("UnityResourceManager", string.Format("resource {0} not found in framework paths!", name));
			}
			
			return obj as T;
		}

		public IEnumerator LoadValue(string url, System.Action<DataElement> handler)
		{
			if(handler != null)
				handler(null);

			yield break;
		}

		#endregion

		#region IService Implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion
	}
}
