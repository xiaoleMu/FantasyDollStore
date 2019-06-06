using UnityEngine;
using System.Collections;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

public static class ICrossContextInjectionBinderExtensions 
{
	public static IInjectionBinding ReBind<T>(this ICrossContextInjectionBinder _this)
	{
		IInjectionBinding binding = _this.GetBinding<T>();
		if(binding!=null)
			_this.CrossContextBinder.Unbind<T>();
		
		return _this.Bind<T>();
	}

	public static void Wire<T,S>(this ICrossContextInjectionBinder _this)
	{
		_this.Bind<S>().ToSingleton();
		_this.ReBind<T>().To<S>().CrossContext();
	}
}
