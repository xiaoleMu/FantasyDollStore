using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TabTale 
{
	/// <summary>
	/// A container for services and their providers. This class can hold services (in the form
	/// of Type instances representing interfaces), and providers (in the form of instances 
	/// of any class implementing the basic interface specified by TBase). 
	/// When a service is requested, we check if an active provider is already present - if so, 
	/// we return it. Otherwise, we search among our providers, and try to initialize one that
	/// implements the service. If we succeed, we mark it as active, and return it.
	/// </summary>
	public class ServiceResolver : IServiceResolver
	{
		//all providers that this container knows of, that have passed the init. call
		private ICollection<IService> _activeProviders = new List<IService>();

		private class ProviderWithPriority
		{
			public IService Provider;
			public int Priority;
		}

		//all providers that this container knows of, that have not yet passed the init. call
		private ICollection<ProviderWithPriority> _dormantProviders = new List<ProviderWithPriority>();

		private IDictionary<System.Type, System.Func<IService>> _providerFactories = new Dictionary<System.Type, System.Func<IService>>();

		//providers that have failed their init call
		private ICollection<IService> _failedProviders = new List<IService>();

		//services we have not yet found a provider for
		private ICollection<Type> _unprovidedServices = new List<Type>();

		//all services that we know how to provide - that is, for each service type, 
		//we map an active provider
		private IDictionary<Type, IService> _serviceProviders = new Dictionary<Type, IService>();

		private int _loggerModule = 0;

		public bool Init()
		{
			return true;
		}

		public void StartProviders()
		{
			CoreLogger.LogDebug(_loggerModule, string.Format("resolver starting all providers..."));

			foreach(Type serviceType in _unprovidedServices)
			{
				IService provider = Get (serviceType);
				if(provider != null)
				{
					CoreLogger.LogDebug(_loggerModule, string.Format("resolver will now provide for service {0} with {1}", serviceType.Name, provider.GetType().Name));
				}
			}
		}

		/// <summary>
		/// Adds a managed service to this resolver. If a provider is present, the 
		/// resolver will now know how to return it in response to requests for this
		/// service. The service must derive from TBase.
		/// </summary>
		/// <returns><c>true</c>, if service was added, <c>false</c> otherwise. A service will
		/// not be added if it does not derive from TBase</returns>
		/// <param name="serviceType">Service type.</param>
		public bool AddService(Type serviceType)
		{
			//check if service is relevant
			if(!typeof(IService).IsAssignableFrom(serviceType))			
			{
				CoreLogger.LogWarning(_loggerModule, string.Format("cannot add incompatible type {0} to loader of {1}", serviceType.Name, typeof(IService).Name));
				return false;
			}

			//if so, check if it's already here
			if(IsServiceKnown(serviceType))
			{
				CoreLogger.LogDebug(_loggerModule, string.Format("service {0} already handled by this container", serviceType.Name));
				return true;
			}

			//now add it
			_unprovidedServices.Add(serviceType);

			return true;
		}

		/// <summary>
		/// Same as AddService, only works on a group. 
		/// </summary>
		/// <returns>The number of services added.</returns>
		/// <param name="serviceTypes">Service types.</param>
		public int AddServices(IEnumerable<Type> services)
		{
			//only work on relevant services, that are not already here
			IEnumerable<Type> newServices = services.Where(st => typeof(IService).IsAssignableFrom(st))
				.Where(st => !IsServiceKnown(st));

			int count = 0;
			foreach(Type serviceType in newServices)
			{
				_unprovidedServices.Add(serviceType);
				count++;
			}

			return count;
		}

		public void AddProvider<TProvider>(System.Func<TProvider> factory)
			where TProvider : IService
		{
			_providerFactories[typeof(TProvider)] = () => factory();
		}

		public void AddProvider(IService provider, int priority = 0)
		{
			//check if it is already here - either as an active provider, or as a dormant one
			if(IsProviderKnown(provider))
			{
				CoreLogger.LogDebug(_loggerModule, string.Format("provider {0} already handled by this container", provider.GetType().Name));
				return;
			}

			//if not, it is first a dormant one
			_dormantProviders.Add(new ProviderWithPriority { Provider = provider, Priority = priority });
		}

		private bool IsProviderKnown(IService provider)
		{
			return (_dormantProviders.Where ( p => p == provider).FirstOrDefault() != null) || 
					_failedProviders.Contains(provider) || _activeProviders.Contains(provider);
		}

		private bool IsServiceKnown(Type serviceType)
		{
			return _serviceProviders.ContainsKey(serviceType) || _unprovidedServices.Contains(serviceType);
		}

		public int AddProviders(IEnumerable<IService> providers)
		{
			//only work on providers not yet here
			IEnumerable<IService> newProviders = providers.Where(p => !IsProviderKnown(p));

			int count = 0;

			foreach(IService provider in newProviders)
			{
				_dormantProviders.Add(new ProviderWithPriority(){Provider = provider, Priority = 0});
				count++;
			}

			return count;
		}

		ITaskFactory _taskFactory;
		public ServiceResolver(ITaskFactory taskFactory, int loggerModule = 0)
		{
			_taskFactory = taskFactory;
			_loggerModule = loggerModule;
		}

		public ITaskFactory TaskFactory
		{
			get { return _taskFactory; }
		}

		bool InitNow(IService provider)
		{
			ITask task = provider.GetInitializer(this);
			return task.Handle().IsOk();
		}

		IEnumerator ActivateServiceProvider(Type serviceType, System.Action<IService> handler)
		{
			List<ProviderWithPriority> dormantProviders = _dormantProviders.Where(p => p.Provider.GetType().GetInterfaces().Contains(serviceType)).ToList();
			dormantProviders.Sort((p1, p2) => p1.Priority.CompareTo(p2.Priority));
			
			foreach(IService dormantProvider in dormantProviders.Select(p => p.Provider))			
			{	
				IService candidate = dormantProvider;
				ITask task = candidate.GetInitializer(this);
				TaskEnding initResult = default(TaskEnding);
				bool initDone = false;

				task.Start(result => {
					initResult = result;
					initDone = true;
				});

				while(!initDone)
				{
					yield return null;
				}

				if(initResult.IsOk())
				{
					CoreLogger.LogDebug(_loggerModule, string.Format("successfully iniialized provider {0} for service {1}", dormantProvider.GetType().Name, serviceType.Name));
					_serviceProviders[serviceType] = dormantProvider;
					_dormantProviders.Remove(_dormantProviders.First(p => p.Provider == dormantProvider));
					_activeProviders.Add(dormantProvider);
					_unprovidedServices.Remove(serviceType);
					handler(candidate);
					yield break;

				} else
				{
					CoreLogger.LogDebug(_loggerModule, string.Format("failed to initalize provider {0} for service {1}", dormantProvider.GetType().Name, serviceType.Name));
					_dormantProviders.Remove(_dormantProviders.First(p => p.Provider == dormantProvider));
					_failedProviders.Add(dormantProvider);
				}
			}
			
			foreach(KeyValuePair<Type, System.Func<IService>> kvp in _providerFactories)
			{
				if(kvp.Key.GetInterfaces().Contains(serviceType))
				{
					IService provider = kvp.Value();
					if(provider != null)
					{
						ITask task = provider.GetInitializer(this);
						TaskEnding initResult = new TaskEnding();
						bool initDone = false;
						
						task.Start(result => {
							initResult = result;
							initDone = true;
						});
						
						while(!initDone)
						{
							yield return null;
						}

						if(initResult.IsOk())
						{
							CoreLogger.LogDebug(_loggerModule, string.Format("successfully iniialized provider {0} for service {1}", provider.GetType().Name, serviceType.Name));
							_serviceProviders[serviceType] = provider;
							_activeProviders.Add(provider);
							_unprovidedServices.Remove(serviceType);
							handler(provider);
							yield break;

						} else
						{
							CoreLogger.LogDebug(_loggerModule, string.Format("failed to initalize provider for service {0}", serviceType.Name));
						}
					}
				}
			}
			
			handler(null);
		}

		IService ActivateServiceProvider(Type serviceType)
		{
			//non-active providers that implement the requested interface, sorted by priority
			List<ProviderWithPriority> dormantProviders = _dormantProviders.Where(p => p.Provider.GetType().GetInterfaces().Contains(serviceType)).ToList();
			dormantProviders.Sort((p1, p2) => p1.Priority.CompareTo(p2.Priority));
//			IEnumerable<TBase> dormantProviders = _dormantProviders.Where(p => p.Provider.GetType().GetInterfaces().Contains(serviceType))
//				.OrderBy(p => p.Priority).Select(p => p.Provider);

			foreach(IService dormantProvider in dormantProviders.Select(p => p.Provider))			
			{

				if(InitNow(dormantProvider))
				{
					CoreLogger.LogDebug(_loggerModule, string.Format("successfully iniialized provider {0} for service {1}", dormantProvider.GetType().Name, serviceType.Name));
					_serviceProviders[serviceType] = dormantProvider;
					_dormantProviders.Remove(_dormantProviders.First(p => p.Provider == dormantProvider));
					_activeProviders.Add(dormantProvider);
					_unprovidedServices.Remove(serviceType);
					return dormantProvider;
				} else
				{
					CoreLogger.LogDebug(_loggerModule, string.Format("failed to initalize provider {0} for service {1}", dormantProvider.GetType().Name, serviceType.Name));
					_dormantProviders.Remove(_dormantProviders.First(p => p.Provider == dormantProvider));
					_failedProviders.Add(dormantProvider);
				}
			}

			foreach(KeyValuePair<Type, System.Func<IService>> kvp in _providerFactories)
			{
				if(kvp.Key.GetInterfaces().Contains(serviceType))
				{
					IService provider = kvp.Value();
					if(provider != null)
					{
						if(InitNow(provider))
						{
							CoreLogger.LogDebug(_loggerModule, string.Format("successfully iniialized provider {0} for service {1}", provider.GetType().Name, serviceType.Name));
							_serviceProviders[serviceType] = provider;
							_activeProviders.Add(provider);
							_unprovidedServices.Remove(serviceType);
							return provider;
						} else
						{
							CoreLogger.LogDebug(_loggerModule, string.Format("failed to initalize provider for service {0}", serviceType.Name));
						}
					}
				}
			}
			
			return null;
		}

		IService FetchService(Type serviceType)
		{
			//an active provider already assigned to this service?
			IService serviceProvider;
			if(_serviceProviders.TryGetValue(serviceType, out serviceProvider))
				return serviceProvider;
			
			//do we even know this service?
			if(!_unprovidedServices.Contains(serviceType))
			{
				CoreLogger.LogNotice(_loggerModule, string.Format("requested service {0} is not known to this container!", serviceType.Name));
				return null;
			}
			
			//do we have an active provider for this service? an active provider is probably already providing other services, 
			//but can provide this as well
			IService activeProvider = _activeProviders.Where(p => p.GetType().GetInterfaces().Contains(serviceType)).FirstOrDefault();
			if(activeProvider != null)
			{
				CoreLogger.LogDebug(_loggerModule, string.Format("adding service {0}, provided by {1}", serviceType.Name, activeProvider.GetType().Name));
				_serviceProviders[serviceType] = activeProvider;
				return activeProvider;
			}

			return null;
		}

		public IService Get(Type serviceType)
		{
			IService service = FetchService(serviceType);
			if(service != null)
				return service;

			//no active provider - see if we can activate a dormant provider
			
			CoreLogger.LogDebug(_loggerModule, string.Format("adding service {0}, now looking for provider...", serviceType.Name));

			return ActivateServiceProvider(serviceType);	        
        }

		public TService Get<TService>()
			where TService : IService
		{
			return (TService)Get(typeof(TService));
		}

		public void Get<TService>(System.Action<TaskEnding, TService> handler)
			where TService : IService
		{
			IService service = FetchService(typeof(TService));
			if(service != null)
			{
				handler(TaskEnding.Done, (TService)service);
				return;
			}
			
			//no active provider - see if we can activate a dormant provider
			
			CoreLogger.LogDebug(_loggerModule, string.Format("adding service {0}, now looking for provider...", typeof(TService).Name));
			
			_taskFactory.FromEnumerableAction(() => 
				ActivateServiceProvider(typeof(TService), s => {
				if(s == null)
				{
					handler(TaskEnding.Cancelled, default(TService));
				} else
				{
					handler(TaskEnding.Done, (TService)s);
				}
			})).Start(-1);
		}

		public IEnumerable<IService> Providers
		{
			get { return _dormantProviders.Select(p => p.Provider).Concat(_activeProviders); }
		}

		public IEnumerable<IService> ActiveProviders
		{
			get { return _activeProviders; }
		}
	}
}
