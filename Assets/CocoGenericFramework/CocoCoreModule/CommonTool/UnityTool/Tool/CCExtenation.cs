using CocoPlay.ResourceManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CocoPlay{
	public static class CocoSprite
	{

		public static void SetSprite(this Image image, string name, bool pNativeSize = false)
		{
			string path = name;
			Sprite sprite = ResourceManager.Load<Sprite>(path);
			if (sprite != null)
			{
				image.sprite = sprite;
				if (pNativeSize)
					image.SetNativeSize();
			}
			else
			{
				Debug.LogError(name + " : not sprite");
			}
		}

		public static void SetSpriteWithRealPath(this Image image, string path, bool pNativeSize = false)
		{
			Sprite sprite = ResourceManager.Load<Sprite>(path);
			if (sprite != null)
			{
				image.sprite = sprite;
				if (pNativeSize)
					image.SetNativeSize();
			}
			else
			{
				Debug.LogError(path + " : not sprite");
			}
		}

//		public static void SetMaterialWithRealPath(this Image image, string path, bool pNativeSize = false)
//		{
//			Material material = Resources.Load<Material>(path);
//			if (material != null)
//			{
//				image.material = material;
//				if (pNativeSize)
//					image.SetNativeSize();
//			}
//			else
//			{
//				Debug.LogError(path + " : not material");
//			}
//		}
//
//		static public bool ContainsKey(this JsonData data, string key)
//		{
//			bool result = false;
//			if (data == null)
//				return result;
//			if (!data.IsObject)
//			{
//				return result;
//			}
//			IDictionary tdictionary = data as IDictionary;
//			if (tdictionary == null)
//				return result;
//			if (tdictionary.Contains(key))
//			{
//				result = true;
//			}
//			return result;
//		}
	}
}
