using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System.Collections.Generic;

namespace CocoPlay
{
	public interface ICocoMakeupData
	{
		List<CocoMakeupCategoryData> GetCategoryList ();

		CocoMakeupCategoryData GetCategoryData(CocoMakeupCategory pCategory);

		List<CocoMakeupItemData> GetItemList(CocoMakeupCategory pCategory);

		string GetMaterialProperty(CocoMakeupPaintLayer layer);
	}


	public class CocoMakeupCategoryData
	{
		public CocoMakeupCategory Category;
		// icon
		public string m_IconNormalPath;  //通用icon路径
		public string m_IconSelectedPath;//选择icon路径

		// item
		public string m_ItemPrefabsPath; //预设路径

		//Makeup Data
		public CocoMakeupCategoryPaintData PaintData; //
		public CocoMakeupPaintLayer PaintLayer; //绘制在什么位置
	}

	public class CocoMakeupItemData
	{
		public int m_Index;
		public CocoMakeupCategory Category;
		public string[] m_IconPaths;

		public string m_TexturePath;
		public string m_id;

		public string ID
		{
			get { return m_id; }
		}
	}

	public class CocoMakeupCategoryPaintData
	{
		public CocoMakeupPaintType paintType; //绘画类型

		public CocoMakeupCategoryPaintData (CocoMakeupPaintType type)
		{
			paintType = type;
		}
	}

	public class CocoMakeupCategoryPaintData_ChangeTexture : CocoMakeupCategoryPaintData
	{
		public CocoMakeupCategoryPaintData_ChangeTexture () : base (CocoMakeupPaintType.ChangeTexture)
		{
		}
	}


	public class CocoMakeupCategoryPaintData_PaintTexture : CocoMakeupCategoryPaintData
	{
		// paint kit
		public string paintMaskPath; //mask 路径
		public string paintBrushPath; //笔刷路径
		public Vector2 BrushSize;   //笔刷大小
		public int paintLayerId;  // 绘制层数
		public bool paintBlendColor; //是否混合颜色
		// tool
		public bool faceToCameraWhenPaint; //涂抹时脸是否对着相机

		public CocoMakeupCategoryPaintData_PaintTexture () : base (CocoMakeupPaintType.PaintTexture)
		{
		}
	}
}
