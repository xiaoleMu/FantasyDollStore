using UnityEngine;
using System.Collections;
using TabTale.AssetManagement;
using TabTale;
using System;


public class AppModalHandle<TData> : IModalHandle {

	public event Action<IModalHandle> CloseCompleteEvent = (t) => {};
	public event Action OpenHandlerEvent = () => {};
	public event Action CloseHandlerEvent = () => {};
	public string prefabPath;
	public GameObject prefab;

    public IModalMaskType MaskType{get; set;}

    public GameObject Parent {get;set;}

	private GameObject originParent {get;set;}

	public TData Data {get;set;}

	public string Name {get{return prefab.name;}}

#if UNITY_2017_1_OR_NEWER
	ILogger logger = Debug.unityLogger;
#else
	ILogger logger = Debug.logger;
#endif

	AppModalView _view;
	IAssetManager _assetManager;
	string _id;
	ObjectPool _objectPool;

	public AppModalView AppModalView {
		get {
			return _view;
		}
	}

	public string Id 
	{
		get { return _id; }
		set { _id = value; }
	}

	public AppModalHandle()
	{
		Init();
	}
	
	public AppModalHandle(GameObject prefab)
	{
		this.prefab = prefab;
		Init();
	}
	
	public AppModalHandle(string prefabPath)
	{
		this.prefabPath = prefabPath;
		Init();
	}

	public AppModalHandle(GameObject prefab,TData data)
	{
		this.prefab = prefab;
		this.Data = data;
		Init();
	}

	public AppModalHandle(string prefabPath,TData data)
	{
		this.prefabPath = prefabPath;
		this.Data = data;
		Init();
	}


    public AppModalHandle(string prefabPath, IModalMaskType maskType)
    {
        this.prefabPath = prefabPath;
        Init();
        MaskType = maskType;
    }

	public void SetId(string id)
	{
		this.Id = id;
	}
	void Init()
	{
		_assetManager = GameApplication.Instance.AssetManager;
		_objectPool = GameApplication.Instance.GetComponent<ObjectPool>();
        MaskType = IModalMaskType.Masked;
	}

	private bool IsUsingExistingGameObject 
	{
		get { return (prefabPath == null); }
	}

	public void Open ()
	{
		if(_view!=null)
			return;

		if(IsCached)
			_view = GetViewFromCache();
		else
			_view = CreateView();
		
		if(_view==null)
			return;
		
		IModalDataReceiver<TData> dataReceiver = _view.gameObject.GetComponentOrInterface<IModalDataReceiver<TData>>();
		if(dataReceiver!=null)
			dataReceiver.SetData(Data);

		_view.CloseCompleteEvent+=OnCloseComplete;
		OpenHandlerEvent ();
		_view.Open();
	}

	bool IsCached
	{
		get
		{
			if(_objectPool==null)
				return false;
			
			if(prefab==null)
				return _objectPool.IsInPool(prefabPath);
			
			return _objectPool.IsInPool(prefab);
		}
	}

	AppModalView CreateView()
	{
		
		AppModalView ret=null;
		GameObject modalGo=null;
		
		if(IsUsingExistingGameObject)
		{
			modalGo = prefab.gameObject;
			originParent = prefab.transform.parent.gameObject;
		}
		else
		{
			prefab = _assetManager.GetResource<GameObject> (prefabPath, true);
			if(prefab==null)
			{
				logger.LogError("AppModalHandle","Open prefab "+prefabPath+" not found");
				CloseCompleteEvent(this);
				return ret;
			}

			modalGo = GameObject.Instantiate (prefab) as GameObject;
		}

		modalGo.transform.SetParent(Parent.transform,false);
		ret = modalGo.GetComponent<AppModalView>();
		
		if(ret==null)
		{
			logger.LogError("AppModalHandle","Open "+modalGo.name+" does not contain AppModalBase component");
			GameObject.Destroy(modalGo);
			CloseCompleteEvent(this);
			return ret;
		}

		return ret;
	}

	AppModalView GetViewFromCache()
	{
		AppModalView ret=null;
		GameObject modalGo;
		if(prefab==null)
			modalGo = _objectPool.Allocate(prefabPath);
		else
			modalGo = _objectPool.Allocate(prefab);
		
		if(modalGo==null)
		{
			logger.LogError("AppModalHandle","GetViewFromCache "+modalGo.name+" does not contain AppModalBase component");
			GameObject.Destroy(modalGo);
			CloseCompleteEvent(this);
			return ret;
		}
		
		if(modalGo.transform.parent!=Parent.transform)
			modalGo.transform.SetParent(Parent.transform,false);
		
		ret = modalGo.GetComponent<AppModalView>();
		
		if(ret==null){
			logger.LogError("AppModalHandle","GetViewFromCache "+modalGo.name+" dous not contain AppModalBase component");
			GameObject.Destroy(modalGo);
			CloseCompleteEvent(this);
			return ret;
		}
		
		return ret;
	}

	public void Close()
	{
		if(_view != null)
		{
			_view.Close();
		}
	}
	
	public void RetunToForeground ()
	{
		_view.RetunToForeground();
	}
	public void MoveToBackground ()
	{
		_view.MoveToBackground();
	}
	
	void OnCloseComplete ()
	{
		CloseHandlerEvent ();
		CloseCompleteEvent(this);

	}
	
	public void Destroy()
	{
		if(_view==null)
			return;
		
		_view.CloseCompleteEvent -= OnCloseComplete;

		if(IsUsingExistingGameObject)
		{
			// If the modal's parent is gone then destroy it (we have no where to return it to) - this would occur mostly on scene transition
			if(originParent == null)
			{
				GameObject.Destroy(_view.gameObject);
			}
			else
			{
				_view.gameObject.transform.SetParent(originParent.transform);
			}

		}
		else
		{
			if(_objectPool!=null && _objectPool.IsInstanceAllocated(_view.gameObject))
				_objectPool.Release(_view.gameObject);
			else
				GameObject.Destroy(_view.gameObject);
		}

		_view=null;
	}
}


public class AppModalHandle:AppModalHandle<object>{

	public AppModalHandle(GameObject prefab):base(prefab){ }
	
	public AppModalHandle(string prefabPath):base(prefabPath) { }

    public AppModalHandle(string prefabPath, IModalMaskType maskType): base(prefabPath, maskType) {	}
     
}

public interface IModalDataReceiver<TData>
{
	void SetData (TData data);
}	

