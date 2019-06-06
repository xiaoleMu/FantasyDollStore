using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabTale 
{
	public class ModuleContainer : IModuleContainer
	{
		IDictionary<Type, IModule> _modules = new Dictionary<Type, IModule>();
		IDictionary<Type, IModule> _availableModules;
		IDictionary<Type, IModule> _failedModules = new Dictionary<Type, IModule>();

		private bool _started = false;
		public void Start()
		{
			if(_started)
			{
				CoreLogger.LogWarning(LoggerModules.GameApplication, "Start called twice!");
				return;
			}

			foreach(IModule module in _modules.Values)
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("starting module {0}", module.GetType().Name));
				try
				{
					module.StartModule();
				} catch(Exception ex)
				{
					CoreLogger.LogError(LoggerModules.GameApplication, string.Format("failed to initialize module {0}:{1}", module.GetType().Name, ex));
				}
			}

			_started = true;
		}

		private bool _stopped = false;
		public void Stop()
		{
			if(!_started)
			{
				CoreLogger.LogWarning(LoggerModules.GameApplication, "Stop called before Start!");
				return;
			}

			if(_stopped)
			{
				CoreLogger.LogWarning(LoggerModules.GameApplication, "Stop called twice!");
				return;
			}

			foreach(IModule module in _modules.Values)
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("stopping and terminating module {0}", module.GetType().Name));
				module.StopModule();
				module.Terminate();
			}

			_stopped = true;
		}

		public ICollection<IModule> Modules
		{
			get { return _modules.Values; }
		}

		ITaskFactory _taskFactory;
		IServiceResolver _serviceResolver;
		public ModuleContainer(TaskFactory taskFactory, IServiceResolver serviceResolver)
		{
			_serviceResolver = serviceResolver;

			_taskFactory = taskFactory;

			_availableModules = taskFactory.GetComponentsInChildren<Component>().OfType<IModule>()
				.ToDictionary<IModule, Type>(m => m.GetType());
		}

		public IServiceResolver ServiceResolver
		{
			get { return _serviceResolver; }
		}

		public ITaskFactory TaskFactory
		{
			get { return _taskFactory; }
		}

		private IDictionary<IModule, ITask> _modulesInInitState = new Dictionary<IModule, ITask>();

		public void Get<TModule>(System.Action<TaskEnding, TModule> handler) 
			where TModule : IModule
		{
			IModule module;
			if(_modules.TryGetValue(typeof(TModule), out module))
			{
				handler(TaskEnding.Done, (TModule)module);
				return;
			}

			if(!_availableModules.TryGetValue(typeof(TModule), out module))
			{
				handler(TaskEnding.Cancelled, default(TModule));
				return;
			}

			IModule key = _modulesInInitState.Keys.FirstOrDefault(m => m.GetType() == typeof(TModule));
			if(key != null)
			{
				ITask value = _modulesInInitState[key];
				value.Done += result => {
					handler(result, (TModule)key);
				};
				return;
			}

			ITask init = module.GetInitializer(this);
			if(init == null)
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("unable to retrieve initializer for module {0} failed", module.GetType().Name));
				_failedModules[typeof(TModule)] = module;
				_availableModules.Remove(typeof(TModule));
				return;
			}

			_modulesInInitState[module] = init;
			init.Start(result => {
				if(result.IsOk())
				{
					CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("successfully initialized module {0}", module.GetType().Name));
					_modules[typeof(TModule)] = module;
					_availableModules.Remove(typeof(TModule));
					handler(result, (TModule)module);

					if(_started)
					{
						module.StartModule();
						CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("starting module {0}", module.GetType().Name));
					}
					return;

				} else
				{
					CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("failed to initialize module {0}", module.GetType().Name));
					_failedModules[typeof(TModule)] = module;
					_availableModules.Remove(typeof(TModule));
					handler(result, default(TModule));
					return;
				};
			});
		}

		public TModule Get<TModule>()
			where TModule : IModule
		{
			IModule module;
			if(_modules.TryGetValue(typeof(TModule), out module))
				return (TModule)module;
			
			if(!_availableModules.TryGetValue(typeof(TModule), out module))
				return default(TModule);

			if(_modulesInInitState.ContainsKey(module))
			{
				CoreLogger.LogError(LoggerModules.GameApplication, string.Format("module {0} is initializing and cannot be retrieved at the moment", module.GetType().Name));
				return default(TModule);
			}

			ITask init = module.GetInitializer(this);
			if(init == null)
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("unable to retrieve initializer for module {0} failed", module.GetType().Name));
				_failedModules[typeof(TModule)] = module;
				_availableModules.Remove(typeof(TModule));
				return default(TModule);
			}

			_modulesInInitState[module] = init;
			TaskEnding result = init.Handle();
			_modulesInInitState.Remove(module);

			if(result.IsOk())
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("successfully initialized module {0}", module.GetType().Name));
				_modules[typeof(TModule)] = module;
				_availableModules.Remove(typeof(TModule));
			} else
			{
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("failed to initialize module {0}", module.GetType().Name));
				_failedModules[typeof(TModule)] = module;
				_availableModules.Remove(typeof(TModule));
				return default(TModule);
			}

			if(_started)
			{
				module.StartModule();
				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("starting module {0}", module.GetType().Name));
			}

			return (TModule)module;
		}

		/// <summary>
		/// Go over all existing modules, and initialize them, if possible immediately.
		/// </summary>
		public void Init(int fromStage, int toStage)
		{
			CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("requested blocking initialization of modules stages {0} to {1}", fromStage, toStage));

			IList<KeyValuePair<Type, IModule>> modulesToRemove = new List<KeyValuePair<Type, IModule>>();

			foreach(KeyValuePair<Type, IModule> kvp in _availableModules.Where(kvp => (kvp.Value.Stage >= fromStage) && (kvp.Value.Stage <= toStage)))
			{
				IModule module = kvp.Value;

				CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("located module {0}", module.GetType().Name));
				modulesToRemove.Add(kvp);

				if(!_modules.ContainsKey(module.GetType()))
				{
					ITask init = module.GetInitializer(this);
					if(init != null)
					{
						TaskEnding result = init.Handle();
						if(result.IsOk())
						{
							CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("successfully initialized module {0}", module.GetType().Name));
							_modules[module.GetType()] = module;
						} else
						{
							CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("failed to initalize module {0}: {1}", module.GetType().Name, result));
							_failedModules[module.GetType()] = module;
						}
					} else
					{
						CoreLogger.LogError(LoggerModules.GameApplication, string.Format("failed to retrieve initializer for module {0}", module.GetType().Name));
						_failedModules[module.GetType()] = module;
					}
				}
			}

			foreach(KeyValuePair<Type, IModule> toRemove in modulesToRemove)
			{
				_availableModules.Remove(toRemove);
			}
		}

		public void Init(int fromStage, int toStage, System.Action handler)
		{
			CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("requesgted non blocking initialization of modules stages {0} to {1}; total available: {2}", fromStage, toStage, _availableModules.Count));
			
			int pending = 0;

			ICollection<ITask> tasks = new List<ITask>();
			
			IList<KeyValuePair<Type, IModule>> modulesToRemove = new List<KeyValuePair<Type, IModule>>();
			
			foreach(KeyValuePair<Type, IModule> kvp in _availableModules.Where(kvp => (kvp.Value.Stage >= fromStage) && (kvp.Value.Stage <= toStage)))
			{
				//always remember to create a local-scoped variable within foreach loops that use lambdas!!!
				IModule localModule = kvp.Value;
				
				if(!_modules.ContainsKey(localModule.GetType()))
				{
					ITask init = localModule.GetInitializer(this);
					if(init != null)
					{
						tasks.Add(init);

						CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("located module {0} - starting init", localModule.GetType().Name));

						init.Done += result => {

							if(result.IsOk())
							{
								CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("successfully done with module {0} ({1}); {2} modules remaining", localModule.GetType().Name, result, pending));
								_modules[localModule.GetType()] = localModule;
							} else
							{
								CoreLogger.LogDebug(LoggerModules.GameApplication, string.Format("failed with module {0} ({1}); {2} modules remaining", localModule.GetType().Name, result, pending));
								_failedModules[localModule.GetType()] = localModule;
							}
						};
						
					} else
					{
						CoreLogger.LogError(LoggerModules.GameApplication, string.Format("failed to retrieve initializer for module {0}", localModule.GetType().Name));
						_failedModules[localModule.GetType()] = localModule;
					}
				}
			}

			_taskFactory.Parallelize(tasks).Start(result => {
				handler();
			});	
			
			foreach(KeyValuePair<Type, IModule> toRemove in modulesToRemove)
			{
				_availableModules.Remove(toRemove);
			}
		}

//		public void Init(int fromStage, int toStage, System.Action handler)
//		{
//			Logger.LogDebug(LoggerModules.GameApplication, string.Format("requesgted non blocking initialization of modules stages {0} to {1}; total available: {2}", fromStage, toStage, _availableModules.Count));
//
//			int pending = 0;
//
//			IList<KeyValuePair<Type, IModule>> modulesToRemove = new List<KeyValuePair<Type, IModule>>();
//			
//			foreach(KeyValuePair<Type, IModule> kvp in _availableModules.Where(kvp => (kvp.Value.Stage >= fromStage) && (kvp.Value.Stage <= toStage)))
//			{
//				//always remember to create a local-scoped variable within foreach loops that use lambdas!!!
//				IModule localModule = kvp.Value;
//
//				if(!_modules.ContainsKey(localModule.GetType()))
//				{
//					ITask init = localModule.GetInitializer(this);
//					if(init != null)
//					{
//						Logger.LogDebug(LoggerModules.GameApplication, string.Format("located module {0} - starting init", localModule.GetType().Name));
//						pending++;
//						init.Start(result => {
//							pending--;
//							if(result.IsOk())
//							{
//								Logger.LogDebug(LoggerModules.GameApplication, string.Format("successfully done with module {0} ({1}); {2} modules remaining", localModule.GetType().Name, result, pending));
//								_modules[localModule.GetType()] = localModule;
//							} else
//							{
//								Logger.LogDebug(LoggerModules.GameApplication, string.Format("failed with module {0} ({1}); {2} modules remaining", localModule.GetType().Name, result, pending));
//								_failedModules[localModule.GetType()] = localModule;
//							}
//
//							if(pending == 0)
//							{
//								handler();
//							}
//						});
//
//					} else
//					{
//						Logger.LogError(LoggerModules.GameApplication, string.Format("failed to retrieve initializer for module {0}", localModule.GetType().Name));
//						_failedModules[localModule.GetType()] = localModule;
//					}
//				}
//			}
//
//			if(pending == 0)
//				handler();
//			
//			foreach(KeyValuePair<Type, IModule> toRemove in modulesToRemove)
//			{
//				_availableModules.Remove(toRemove);
//			}
//		}
	}
}
