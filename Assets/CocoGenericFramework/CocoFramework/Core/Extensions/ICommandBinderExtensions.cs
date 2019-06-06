using UnityEngine;
using System.Collections;
using strange.extensions.command.api;

public static class ICommandBinderExtensions {

	public static ICommandBinding SafeBind<T>(this ICommandBinder _this) 
	{
		ICommandBinding binding = _this.GetBinding<T>();
		if(binding!=null)
			return binding;

		return _this.Bind<T>();
	}
	
}
