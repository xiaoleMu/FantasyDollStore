
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public struct GridInfo
{
	public RectOffset padding;
	public Vector2 cellSize;
	public Vector2 spacing;
}

static public class CCGridExtensions
{
	public static void SetInfo(this UnityEngine.UI.GridLayoutGroup grid, GridInfo gridInfo)
	{
		grid.padding = gridInfo.padding;
		grid.cellSize = gridInfo.cellSize;
		grid.spacing = gridInfo.spacing;
	}
}
