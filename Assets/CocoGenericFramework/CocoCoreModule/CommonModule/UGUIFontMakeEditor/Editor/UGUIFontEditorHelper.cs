using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class UGUIFontEditorHelper {

	[MenuItem("Coco Common/FontMaker/BatchCreateArtistFont", false, 120)]
	static void BatchCreateArtistFont()
	{
		ArtistFont.BatchCreateArtistFont();
	}
}
