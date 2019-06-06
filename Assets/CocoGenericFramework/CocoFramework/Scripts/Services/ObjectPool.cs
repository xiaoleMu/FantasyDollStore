using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.AssetManagement;
using System.Linq;

namespace TabTale
{
	public class ObjectPool : MonoBehaviour
	{
		Dictionary<GameObject, LinkedList<GameObject>> _poolDictionary = new Dictionary<GameObject, LinkedList<GameObject>> ();
		Dictionary<GameObject, GameObject>  _allocatedDictionary = new Dictionary<GameObject, GameObject> ();
		Dictionary<string, GameObject>  _prefabPathDictionary = new Dictionary<string, GameObject> ();

		public void AddToPool (GameObject prefab, int count)
		{
			for (int i=0; i<count; i++) {
				GameObject instance = InstantiateGo (prefab);
				_allocatedDictionary [instance] = prefab;
				Release (instance);
			}
		}

		public void AddToPool (string prefabPath, int count){
			GameObject prefab = GameApplication.Instance.AssetManager.GetResource<GameObject> (prefabPath);
			_prefabPathDictionary [prefabPath] = prefab;
			AddToPool(prefab,count);
		}

		public bool IsInPool(string prefabPath){
			if(!_prefabPathDictionary.ContainsKey(prefabPath))
				return false;

			return IsInPool(_prefabPathDictionary[prefabPath]);
		}

		public bool IsInPool(GameObject prefab){
			return _poolDictionary.ContainsKey (prefab);
		}

		public GameObject Allocate (string prefabPath)
		{
			GameObject prefab;
			if (!_prefabPathDictionary.ContainsKey (prefabPath)) {
				prefab =  GameApplication.Instance.AssetManager.GetResource<GameObject> (prefabPath);
				_prefabPathDictionary [prefabPath] = prefab;
			} else
				prefab = _prefabPathDictionary [prefabPath];

			return Allocate (prefab);
		}

		public GameObject Allocate (GameObject prefab)
		{
			if (!_poolDictionary.ContainsKey (prefab)) {
				_poolDictionary [prefab] = new LinkedList<GameObject> ();
			}
			
			GameObject ret;
			if (_poolDictionary [prefab].Count == 0) {
				ret = InstantiateGo (prefab);
			} else {
				ret = _poolDictionary [prefab].First.Value;
				_poolDictionary [prefab].RemoveFirst ();
			}
			
			_allocatedDictionary [ret] = prefab;
			ret.SetActive (true);
			return ret;
		}

		public GameObject Allocate (GameObject prefab, Vector3 postion, Quaternion rotation)
		{
			//Debug.Log("ObjectPool.Allocate "+prefab);

			if (!_poolDictionary.ContainsKey (prefab)) {
				_poolDictionary [prefab] = new LinkedList<GameObject> ();
			}

			GameObject ret;
			if (_poolDictionary [prefab].Count == 0) {
				ret = InstantiateGo (prefab, postion, rotation);
			} else {
				ret = _poolDictionary [prefab].First.Value;
				ret.transform.position = postion;
				ret.transform.rotation = rotation;
				_poolDictionary [prefab].RemoveFirst ();
			}

			_allocatedDictionary [ret] = prefab;
			ret.SetActive (true);
			return ret;
		}

		GameObject InstantiateGo (GameObject obj)
		{
			GameObject ret;
			ret = Instantiate (obj) as GameObject;
			ret.transform.SetParent(gameObject.transform,false);
			return ret;
		}

		GameObject InstantiateGo (GameObject obj, Vector3 postion, Quaternion rotation)
		{
			GameObject ret;
			ret = Instantiate (obj, postion, rotation) as GameObject;
			ret.transform.SetParent(gameObject.transform,false);
			return ret;
		}

		public bool IsInstanceAllocated(GameObject instance){
			return _allocatedDictionary.ContainsKey(instance);
		}

		public void Release (GameObject instance)
		{
			if(!IsInstanceAllocated(instance)){
				Destroy(instance);
				return;
			}

			GameObject prefab = _allocatedDictionary [instance];
			//Debug.Log("ObjectPool.Release "+prefab);

			if (!_poolDictionary.ContainsKey (prefab)) {
				_poolDictionary [prefab] = new LinkedList<GameObject> ();
			}

			_allocatedDictionary.Remove (instance);
			instance.SetActive (false);
			instance.transform.SetParent(gameObject.transform,false);
			_poolDictionary [prefab].AddLast (instance);
		}

		public void RemoveFromPool(string prefabPath){
			if(!_prefabPathDictionary.ContainsKey(prefabPath))
				return;

			RemoveFromPool(_prefabPathDictionary[prefabPath]);
		}

		public void RemoveFromPool(GameObject prefab){

			foreach(var item in _allocatedDictionary.Where(kvp => kvp.Value == prefab).ToList())
			{
				_allocatedDictionary.Remove(item.Key);
			}

			foreach(var item in _prefabPathDictionary.Where(kvp => kvp.Value == prefab).ToList())
			{
				_prefabPathDictionary.Remove(item.Key);
			}

			if(!_poolDictionary.ContainsKey(prefab))
				return;

			foreach (var item in _poolDictionary[prefab]) {
				Destroy(item);
			}

			_poolDictionary.Remove(prefab);
		}

		public void RemoveFromPoolAll ()
		{
			foreach (KeyValuePair<GameObject, LinkedList<GameObject>> entry in _poolDictionary) {

				foreach (var item in entry.Value) {
					Destroy (item);
				}

//			for(int i=0;i<entry.Value.Count;i++){
//				Destroy(entry.Value[i]);
//			}
			}

			List<GameObject> objToDestroy = new List<GameObject> ();

			foreach (KeyValuePair<GameObject, GameObject> entry in _allocatedDictionary)
				objToDestroy.Add (entry.Key);

			for (int i=objToDestroy.Count-1; i>=0; i--)
				Destroy (objToDestroy [i]);

			_poolDictionary.Clear ();
			_allocatedDictionary.Clear ();
			_prefabPathDictionary.Clear ();

		}

		void DebugPrint ()
		{
			Debug.Log ("============DebugPrint");
			foreach (KeyValuePair<GameObject, LinkedList<GameObject>> entry in _poolDictionary) {
				Debug.Log ("=========" + entry.Key + " " + entry.Value.Count);
			}
		}

	}
}
