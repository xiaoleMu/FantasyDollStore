using UnityEngine;
using System.Collections.Generic;
using System;
using TabTale;

namespace CocoPlay
{
	public class CocoModuleContainer : GameView
	{
		#region Module Container

		Dictionary<Type, Dictionary<object, CocoModuleBase>> m_TypeModuleDic = null;

		Dictionary<Type, Dictionary<object, CocoModuleBase>> TypeModuleDic {
			get {
				if (m_TypeModuleDic == null) {
					m_TypeModuleDic = new Dictionary<Type, Dictionary<object, CocoModuleBase>> ();
				}
				return m_TypeModuleDic;
			}
		}

		Dictionary<object, CocoModuleBase> GetModuleDic<T> ()
		{
			Type moduleType = typeof(T);

			if (TypeModuleDic.ContainsKey (moduleType)) {
				return TypeModuleDic [moduleType];
			}

			Dictionary<object, CocoModuleBase> moduleDic = new Dictionary<object, CocoModuleBase> ();
			TypeModuleDic.Add (moduleType, moduleDic);
			return moduleDic;
		}

		#endregion


		#region Add Module

		public TModule AddModule<TModule> (string assetPath = null, object moduleId = null) where TModule : CocoModuleBase
		{
			TModule module = CocoLoad.InstantiateOrCreate<TModule> (assetPath, transform);
			if (!AddModule<TModule> (module, moduleId)) {
				Destroy (module.gameObject);
				return null;
			}

			return module;
		}

		public TModuleEntity AddModule<TModule, TModuleEntity> (string assetPath = null, object moduleId = null) where TModule : CocoModuleBase where TModuleEntity: CocoModuleBase
		{
			TModuleEntity module = CocoLoad.InstantiateOrCreate<TModuleEntity> (assetPath, transform);
			if (!AddModule<TModule> (module, moduleId)) {
				Destroy (module.gameObject);
				return null;
			}

			return module;
		}

		public TModule AddModule<TModule> (Type moduleType, string assetPath = null, object moduleId = null) where TModule : CocoModuleBase
		{
			if (moduleType == null) {
				return AddModule<TModule> (assetPath, moduleId);
			}

			if (!moduleType.IsSubclassOf (typeof (CocoModuleBase))) {
				Debug.LogErrorFormat ("{0}->AddModule: can NOT add module [{1}<{2}>], because the type is NOT module!", GetType (), moduleId, moduleType.Name);
				return null;
			}

			TModule module = (TModule)CocoLoad.InstantiateOrCreate (moduleType, assetPath, transform);
			if (!AddModule<TModule> (module, moduleId)) {
				Destroy (module.gameObject);
				return null;
			}

			return module;
		}

		public TModule AddModule<TModule> (CocoModuleBase module, object moduleId = null) where TModule : CocoModuleBase
		{
			Dictionary<object, CocoModuleBase> moduleDic = GetModuleDic<TModule> ();
			string moduleKey = GetModuleKeyById (ref moduleId);
			if (moduleDic.ContainsKey (moduleKey)) {
				Debug.LogErrorFormat ("{0}->AddModule: can NOT add module [{1}<{2}>], because the one with same id already exists!", GetType (), moduleId, module.GetType ().Name);
				return null;
			}

			if (module.transform.parent != transform) {
				CocoLoad.SetParent (module, transform);
			}

			// init module
			CocoRoot.BindValue<TModule> ((TModule)module, moduleId);
			module.Init (moduleId);
			moduleDic.Add (moduleKey, module);

			return (TModule)module;
		}

		#endregion


		#region Get Module

		public TModule GetModule<TModule> (object moduleId = null) where TModule : CocoModuleBase
		{
			Dictionary<object, CocoModuleBase> moduleDic = GetModuleDic<TModule> ();
			string moduleKey = GetModuleKeyById (ref moduleId);
			if (!moduleDic.ContainsKey (moduleKey)) {
				Debug.LogWarningFormat ("{0}->GetModule: can NOT get module [{1}<{2}>], because id don't exist!", GetType (), moduleId, typeof(TModule).Name);
				return null;
			}

			return (TModule)moduleDic [moduleKey];
		}

		#endregion


		#region Remove Module

		public void RemoveModule<TModule> (object moduleId = null) where TModule : CocoModuleBase
		{
			Dictionary<object, CocoModuleBase> moduleDic = GetModuleDic<TModule> ();
			string moduleKey = GetModuleKeyById (ref moduleId);
			if (!moduleDic.ContainsKey (moduleKey)) {
				Debug.LogWarningFormat ("{0}->RemoveModule: NOT need remove module [{1}<{2}>], because id don't exist!", GetType (), moduleId, typeof(TModule).Name);
				return;
			}

			// clean module
			CocoModuleBase module = moduleDic [moduleKey];
			module.Clean ();
			CocoRoot.Unbind<TModule> (moduleId);
			moduleDic.Remove (moduleKey);

			// destroy module
			Destroy (module.gameObject);
		}

		#endregion


		#region Helper

		string GetModuleKeyById (ref object moduleId)
		{
			if (moduleId is string) {
				if (string.IsNullOrEmpty ((string)moduleId)) {
					moduleId = null;
				}
			}

			return moduleId != null ? moduleId.ToString () : string.Empty;
		}

		#endregion

	}
}
