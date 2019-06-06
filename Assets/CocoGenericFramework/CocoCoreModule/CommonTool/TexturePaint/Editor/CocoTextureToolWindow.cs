using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CocoPlay
{
	public class CocoTextureToolWindow : EditorWindow
	{
		[MenuItem ("CocoPlay/Asset/Texture Creator...", false, 60)]
		static void Init ()
		{
			CocoTextureToolWindow window = EditorWindow.GetWindow<CocoTextureToolWindow> ("Texture Tool", true);
		}

		void OnGUI ()
		{
			EditorGUILayout.BeginVertical ();

			DrawTextureCreation ();

			EditorGUILayout.EndVertical ();
		}

		#region Texture Creation

		Color m_TextureColor = Color.white;

		const int TEXTURE_SIZE_MIN = 1;
		const int TEXTURE_SIZE_MAX = 2048;
		int m_TextureWidth = 64;
		int m_TextureHeight = 64;

		enum TextureFormat
		{
			PNG,
			JPG,
		}

		TextureFormat m_TextureFormat = TextureFormat.PNG;
		int m_TextureQuality = 75;

		void DrawTextureCreation ()
		{
			float originLabelWidth = EditorGUIUtility.labelWidth;
			float originFieldWidth = EditorGUIUtility.fieldWidth;
			EditorGUILayout.BeginHorizontal (GUI.skin.box);

			EditorGUILayout.BeginVertical ();
			// texture
			EditorGUIUtility.labelWidth = 100;
			m_TextureColor = EditorGUILayout.ColorField ("Texture Color", m_TextureColor);

			// size
			EditorGUILayout.BeginHorizontal ();
			EditorGUIUtility.labelWidth = 80;
			EditorGUIUtility.fieldWidth = 80;
			m_TextureWidth = Mathf.Clamp (EditorGUILayout.IntField ("Width", m_TextureWidth), TEXTURE_SIZE_MIN, TEXTURE_SIZE_MAX);
			m_TextureHeight = Mathf.Clamp (EditorGUILayout.IntField ("Height", m_TextureHeight), TEXTURE_SIZE_MIN, TEXTURE_SIZE_MAX);
			EditorGUILayout.EndHorizontal ();

			// format
			EditorGUILayout.BeginHorizontal ();
			m_TextureFormat = (TextureFormat)EditorGUILayout.EnumPopup (m_TextureFormat);
			if (m_TextureFormat == TextureFormat.JPG) {
				m_TextureQuality = EditorGUILayout.IntSlider (m_TextureQuality, 1, 100);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

			if (GUILayout.Button ("Create", GUILayout.Height (EditorGUIUtility.singleLineHeight * 3))) {
				string extension = m_TextureFormat == TextureFormat.PNG ? "png" : "jpg";
				string title = string.Format ("Save Texture (*.{0})", extension);
				string path = EditorUtility.SaveFilePanel (title, string.Empty, "Default", extension);

				if (!string.IsNullOrEmpty (path)) {
					CreateTextureInPath (path);
				}
			}

			EditorGUILayout.EndHorizontal ();
			EditorGUIUtility.labelWidth = originLabelWidth;
			EditorGUIUtility.fieldWidth = originFieldWidth;
		}

		void CreateTextureInPath (string path)
		{
			Texture2D texture = CocoTexture.CreateColorTexture (m_TextureWidth, m_TextureHeight, true, UnityEngine.TextureFormat.ARGB32, m_TextureColor);
			byte[] bytes = m_TextureFormat == TextureFormat.PNG ? texture.EncodeToPNG () : texture.EncodeToJPG (m_TextureQuality);
			System.IO.File.WriteAllBytes (path, bytes);
			AssetDatabase.Refresh ();
		}

		#endregion
	}
}
