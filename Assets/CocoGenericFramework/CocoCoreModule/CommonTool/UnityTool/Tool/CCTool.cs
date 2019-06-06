using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TabTale;
using CocoPlay;
using Random = UnityEngine.Random;

public static class CCTool
{
    public static T GetEnmu<T>(string str)
    {
        T tag = (T)System.Enum.Parse(typeof(T), str);
        return tag;
    }

    public static bool GetKeyDown(KeyCode code)
    {
#if !UNITY_EDITOR
		return false;
#endif
        return Input.GetKeyDown(code);
    }

    #region Random

	public static int RandomByWeight(params int[] weightList)
    {
        int length = weightList.Length;
        int[] dataArr = new int[length];
        int total = 0;
        int returnIndex = 0;
        for (int i = 0; i < length; i++)
        {
            total += weightList[i];
            dataArr[i] = total;
        }
        int randomData = Random.Range(0, total);
        for (int i = 0; i < length; i++)
        {
            if (randomData < dataArr[i])
            {
                returnIndex = i;
                break;
            }

        }
        return returnIndex;
    }

    public static bool RandomBool
    {
        get { return Random.Range(0, 2f) >= 1; }
    }

    static public bool JSContainsKey(JsonData data, string key)
    {
        bool result = false;
        if (data == null)
            return result;
        if (!data.IsObject)
        {
            return result;
        }
        IDictionary tdictionary = data as IDictionary;
        if (tdictionary == null)
            return result;
        if (tdictionary.Contains(key))
        {
            result = true;
        }
        return result;
    }

    public static void SetOpacity(GameObject obj, string colorName, float opacity)
    {
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if (mesh == null)
            return;
        Material[] mats = mesh.materials;
        if (mats == null)
            return;
        foreach (Material mat in mats)
        {
            Color color = mat.GetColor(colorName);
            color.a = opacity;
            mat.SetColor(colorName, color);
        }
    }

    public static void SetColor(GameObject obj, Color color)
    {
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if (mesh == null)
            return;
        Material[] mats = mesh.materials;
        if (mats == null)
            return;
        foreach (Material mat in mats)
        {
            mesh.material.color = color;
        }
    }

    public static void SetImageOpacity(GameObject obj, float opacity)
    {
        Image image = obj.GetComponent<Image>();
        if (image == null)
            return;
        SetImageOpacity(image, opacity);
    }

    public static void SetImageOpacity(Image img, float opacity)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, opacity);
    }

    public static Color Color255To1(Color pColor)
    {
        return new Color(pColor.r / 255f, pColor.g / 255f, pColor.b / 255f, pColor.a / 255f);
    }

    public static Color Color255To1(float r, float g, float b, float a = 255)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color Color1To255(Color pColor)
    {
        return new Color(Mathf.Round(pColor.r * 255f), Mathf.Round(pColor.g * 255f), Mathf.Round(pColor.b * 255f), Mathf.Round(pColor.a * 255f));
    }

    static bool ColorEqual(Color p1, Color p2)
    {
        if (Mathf.Abs(p1.r - p2.r) > 0.004f)
            return false;

        if (Mathf.Abs(p1.g - p2.g) > 0.004f)
            return false;

        if (Mathf.Abs(p1.b - p2.b) > 0.004f)
            return false;

        return true;
    }

    #endregion

    #region Screen Position

    public static Vector3 GetUIToScreenPos(Vector3 WorldPosition)
    {
        Vector3 pScreenPos = CocoMainController.UICamera.WorldToScreenPoint(WorldPosition);
        return pScreenPos;
    }

    public static Vector3 GetSceneToScreenPos(Vector3 WorldPosition)
    {
        Vector3 pScreenPos = Camera.main.WorldToScreenPoint(WorldPosition);
        return pScreenPos;
    }

    public static Vector3 GetScreenToUIWorldPos(Vector3 ScreenPos)
    {
        Vector3 worldPos = CocoMainController.UICamera.ScreenToWorldPoint(ScreenPos);
        return worldPos;
    }

    public static Vector3 GetSceneToUIWorldPos(Vector3 SceneWorldPos)
    {
        Vector3 screenPos = CCTool.GetSceneToScreenPos(SceneWorldPos);
        Vector3 UIPos = GetScreenToUIWorldPos(screenPos);
        UIPos.z = 0;
        return UIPos;
    }

    [Obsolete]
    public static Vector2 GetSceneSize(float z)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(0, 0, z));

        Vector3 ScreenPoint_left = new Vector3(0, 0, screenPosition.z);
        Vector3 WorldPoint_left = Camera.main.ScreenToWorldPoint(ScreenPoint_left);

        Vector3 ScreenPoint_right = new Vector3(Screen.width, Screen.height, screenPosition.z);
        Vector3 WorldPoint_right = Camera.main.ScreenToWorldPoint(ScreenPoint_right);

        float width = Mathf.Abs(WorldPoint_left.x - WorldPoint_right.x);
        float height = Mathf.Abs(ScreenPoint_left.y - WorldPoint_right.y);
        return new Vector2(width, height);
    }
    
    public static Vector2 GetSceneSize(Camera camera, float z)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(new Vector3(0, 0, z));

        Vector3 ScreenPoint_left = new Vector3(0, 0, screenPosition.z);
        Vector3 WorldPoint_left = camera.ScreenToWorldPoint(ScreenPoint_left);

        Vector3 ScreenPoint_right = new Vector3(Screen.width, Screen.height, screenPosition.z);
        Vector3 WorldPoint_right = camera.ScreenToWorldPoint(ScreenPoint_right);

        float width = Mathf.Abs(WorldPoint_left.x - WorldPoint_right.x);
        float height = Mathf.Abs(ScreenPoint_left.y - WorldPoint_right.y);
        return new Vector2(width, height);
    }

    #endregion

    public static T GetInstance<T>()
    {
        StrangeRoot strangeRoot = GameApplication.Instance.ModuleContainer.Get<StrangeRoot>();
        GameRoot gameRoot = strangeRoot.GameRoot;
        var binder = ((GameContext)gameRoot.context).injectionBinder;
        return binder.GetInstance<T>();
    }

    #region vector3 string

    public static Vector3 CreateVecotr3(string str)
    {
        if (str == "")
            return Vector3.zero;
        int length = str.Length;
        str = str.Substring(1, length - 2);
        string[] nums = str.Split(',');
        if (nums.Length != 3)
            return Vector3.zero;
        float x = float.Parse(nums[0]);
        float y = float.Parse(nums[1]);
        float z = float.Parse(nums[2]);
        return new Vector3(x, y, z);
    }

    public static string Vector3ToString(Vector3 pos)
    {
        if (pos == null)
            return "({0}, {1}, {2})";
        else
            return string.Format("({0}, {1}, {2})", pos.x, pos.y, pos.z);
    }

    public static Color StringToColor(string str)
    {
        if (str == "")
            return Color.white;
		Color color = Color.white;
		ColorUtility.TryParseHtmlString("#" + str, out color);
		return color;
    }
	public static string ColorToString(Color color)
	{
		string colorString = ColorUtility.ToHtmlStringRGBA(color);
		return colorString;
	}
    #endregion

    public static float GetAnimLength(GameObject obj, string anim = "")
    {
        Animation pAnimation = obj.GetComponent<Animation>();
        if (pAnimation == null)
            return 0;
        if (anim == "")
            return pAnimation.clip.length;
        AnimationClip manim = pAnimation.GetClip(anim);
        if (manim == null)
            return 0;
        return manim.length;
    }

    public static Vector2 GetWindowSize()
    {
        Canvas root = GameObject.FindObjectOfType<Canvas>();
        RectTransform rectTrans = root.GetComponent<RectTransform>();
        return rectTrans.GetSize();
    }

    public static void SetMaterialAlpha(Material pMat, float pAlpha)
    {
        Color tColor = pMat.color;
        pMat.color = new Color(tColor.r, tColor.g, tColor.b, Mathf.Clamp(pAlpha, 0f, 1f));
    }

    static public T TrunEnum<T>(string str)
    {
        return (T)System.Enum.Parse(typeof(T), str);
    }

    static public Sprite TextureToSprite(Texture2D texture)
    {
        if (texture == null)
            return null;
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    static public int GetTime()
    {
        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(System.DateTime.Now - startTime).TotalSeconds;
    }

    static public void RemoveAllChildren(GameObject obj)
    {
        List<Transform> m_child = new List<Transform>();
        Transform[] array = obj.GetComponentsInChildren<Transform>();
        foreach (Transform tran in array)
        {
            if (tran.parent == obj.transform)
                m_child.Add(tran);
        }

        foreach (Transform tran in m_child)
        {
            GameObject.Destroy(tran.gameObject);
        }
        m_child.Clear();
    }

    static public GameObject GetGameobjectInChildren(GameObject obj, string name)
    {
        List<Transform> m_child = new List<Transform>();
        Transform[] array = obj.GetComponentsInChildren<Transform>();
        foreach (Transform tran in array)
        {
            if (tran.name == name)
                return tran.gameObject;
        }
        return null;
    }

    static public void PlayParticle(GameObject obj)
    {
        ParticleSystem[] array = obj.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem _Particle in array)
        {
            _Particle.Play();
        }
    }

    static public void StopParticle(GameObject obj)
    {
        ParticleSystem[] array = obj.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem _Particle in array)
        {
            _Particle.Stop();
        }
    }

    public static void SetImageAlpha(Image pImg, float pAlpha)
    {
        if (pImg == null)
        {
            Debug.LogError("找不到图片");
            return;
        }

        Color tColor = pImg.color;
        if (pAlpha <= 0)
            pAlpha = 0.004f;
        tColor.a = pAlpha;
        pImg.color = tColor;
    }

    /// <summary>
    /// 传入图片路径，加载图片到Image
    /// </summary>
    ///<param name="pImg">需要设置sprite的UIImage</param>
    /// <param name="pPath">需要加下载的图片路径</param>
    /// <param name="pSize">UIImage的大小</param>
	public static bool SetImageSprite(Image pImg, string pPath, Vector2 pSize)
    {
        if (pImg == null)
        {
#if UNITY_EDITOR
            Debug.LogError("找不到图片");
#endif
			return false;
        }

        if (!System.IO.File.Exists(pPath))
        {
#if UNITY_EDITOR
            Debug.LogError("找不到路径 : " + pPath);
#endif
			return false;
        }

        var bytes = System.IO.File.ReadAllBytes(pPath);
        var tex = new Texture2D(1, 1);
        tex.LoadImage(bytes, true);
        //tex.Apply();
        pImg.sprite = CCTool.TextureToSprite(tex);

        float tWidth = pSize.x;
        float tHeight = pSize.y;

        if (tex.width / tex.height > tWidth / tHeight)
            tHeight = tex.height * tWidth / tex.width;
        else
            tWidth = tex.width * tHeight / tex.height;
        pImg.GetComponent<RectTransform>().sizeDelta = new Vector2(tWidth, tHeight);
		return true;
    }

	public static Sprite GetSprite(string pPath, bool nonReandable = true)
	{
		if (!System.IO.File.Exists(pPath))
		{
			#if UNITY_EDITOR
			Debug.LogError("找不到路径 : " + pPath);
			#endif
			return null;
		}

		var bytes = System.IO.File.ReadAllBytes(pPath);
		var tex = new Texture2D(1, 1);
		tex.LoadImage(bytes, nonReandable);
		Sprite spr = CCTool.TextureToSprite(tex);
		return spr;
	}

    public static string DateToStirng(System.DateTime pTime)
    {
        return string.Format("{0}年{1}月{2}日{3}时{4}分{5}秒", pTime.Year, pTime.Month, pTime.Day, pTime.Hour, pTime.Minute, pTime.Second);
    }

    public static Rect AutoAdaptImage(RectTransform pRect, Texture2D pTargetImage){

   
        Rect pRealRect = new Rect(0,0,1,1);
        bool pBaseWeightOrHeight = (((float)pTargetImage.height / (float)pTargetImage.width) > (pRect.GetHeight () / pRect.GetWidth ()));

        if (pBaseWeightOrHeight) {
            float pRealHeight = pRect.GetWidth () / (float)pTargetImage.width * (float)pTargetImage.height;
            pRealRect.height = pRect.GetHeight () / pRealHeight;
            pRealRect.y = Mathf.Abs(1f - pRealRect.height) / 2.0f;
        }
        else{               
            float pRealWight = pRect.GetHeight () / (float)pTargetImage.height * (float)pTargetImage.width;
            pRealRect.width = pRect.GetWidth () / pRealWight;
            pRealRect.x = Mathf.Abs(1f - pRealRect.width) / 2.0f;
        }

        return pRealRect;
    }

    /// <summary>
    /// 记录当前时间戳
    /// </summary>
    /// <returns>The current time.</returns>
    static public string RecordCurTime(){
        System.DateTime curtime = System.DateTime.Now;

        return ConvertDateTime(curtime).ToString();
    }

    /// <summary>
    /// datatime 转化为时间戳
    /// </summary>
    /// <returns>The date time int.</returns>
    /// <param name="time">Time.</param>
    static public long ConvertDateTime(System.DateTime time)
    {
        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (long)(time - startTime).TotalSeconds;
    }

    /// <summary>
    /// unix 时间戳转化为datetime
    /// </summary>
    /// <returns>The conver to date time.</returns>
    /// <param name="unixTime">Unix time.</param>
    static public System.DateTime StringConverToDateTime(string unixTime){
        if (string.IsNullOrEmpty(unixTime))
            return new System.DateTime(1970, 1, 1);

        System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        long lTime = long.Parse(unixTime + "0000000");
        System.TimeSpan toNow = new System.TimeSpan(lTime); 
        return dtStart.Add(toNow);
    }

	public static string RankSuffix(int pIndex)
	{
		if (pIndex > 10 && pIndex <= 20) {
			return string.Format ("{0}{1}", pIndex, "th");
		} else if (pIndex % 10 == 1) {
			return string.Format ("{0}{1}", pIndex, "st");
		} else if (pIndex % 10 == 2) {
			return string.Format ("{0}{1}", pIndex, "nd");
		} else if (pIndex % 10 == 3) {
			return string.Format ("{0}{1}", pIndex, "rd");
		} else {
			return string.Format ("{0}{1}", pIndex, "th");
		}
	}

	[System.Obsolete ("Use CCTool.Destroy")]
	static public void DestroyObject(UnityEngine.Object obj)
	{
		if(obj != null)
		{
			UnityEngine.Object.Destroy(obj);
			obj = null;
		}
	}

	/// <summary>
	/// 删除Object,内部会判断是否为空
	/// </summary>
	public static void Destroy<T> (ref T obj) where T : UnityEngine.Object
	{
		if (obj != null) {
			UnityEngine.Object.Destroy (obj);
			obj = null;
		}
	}

	public static void DestroyGameObject ( ref GameObject pObj) 
	{
		Destroy (ref pObj);
	}

	/// <summary>
	/// 删除GameObject,内部会判断是否为空
	/// </summary>
	public static void DestroyGameObject<T> (ref T com) where T : Component
	{
		if (com != null) {
			GameObject.Destroy (com.gameObject);
			com = null;
		}
	}
		
    public static bool IsLowDevice()
    {
        return SystemInfo.systemMemorySize < 600;
    }
}
