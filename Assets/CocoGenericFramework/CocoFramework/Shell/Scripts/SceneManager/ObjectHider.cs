using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager
{
	/// <summary>
	/// An ObjectHider holds a list of object names and tags - objects with those names
	/// and/or tags will be deactivated when this objects starts, and reactivated when 
	/// it is terminated. It is used by loading scenes to hide overlay ui canvases.
	/// </summary>
	public class ObjectHider : MonoBehaviour
	{		
		/// <summary>
		/// The names of objects to be deactivated.
		/// when it's done.
		/// </summary>
		public string[] deactivateByName;
		
		public string[] deactivateByTag;

		public bool deactivate = true;
		public bool reactivate = false;

		protected IEnumerable<GameObject> GetDeactivations()
		{
			if(deactivateByName != null)
			{
				foreach(string name in deactivateByName)
				{
					GameObject go = GameObject.Find(name);
					if(go != null)
						yield return go;
				}
			}
			
			IList<GameObject> gameObjects = new List<GameObject>();
			if(deactivateByTag != null)
			{
				foreach(string tag in deactivateByTag)
				{
					try
					{
						foreach(GameObject go in GameObject.FindGameObjectsWithTag(tag))
						{
							gameObjects.Add(go);
						}
					} catch
					{
					}
				}
			}
			
			foreach(GameObject go in gameObjects)
				yield return go;
		}
		
		IEnumerable<GameObject> _deactivations;
		protected void Deactivate()
		{
			if(!deactivate)
				return;

			_deactivations = GetDeactivations().ToList();
			
			foreach(GameObject go in _deactivations)
			{
				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("SceneLoader deactivating {0}", go.name));
				go.SetActive(false)	;				
			}
		}
		
		protected void Reactivate()
		{
			if(!reactivate)
				return;

			foreach(GameObject go in _deactivations)
			{
				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("SceneLoader reactivating {0}", go.name));
				go.SetActive(true);				
			}
		}

		public virtual void Start()
		{
			Deactivate();
		}

		public virtual void OnDestroy()
		{
			Reactivate();
		}
	}
}
