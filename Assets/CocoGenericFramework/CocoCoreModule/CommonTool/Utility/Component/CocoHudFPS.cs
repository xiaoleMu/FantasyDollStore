using UnityEngine;
using System.Collections;


namespace CocoPlay
{
	[AddComponentMenu ("CocoPlay/Common Tool/Hint - HUD FPS")]
	public class CocoHudFPS : MonoBehaviour
	{
		// Attach this to any object to make a frames/second indicator.
		//
		// It calculates frames/second over each updateInterval,
		// so the display does not keep changing wildly.
		//
		// It is also fairly accurate at very low FPS counts (<10).
		// We do this not by simply counting frames per interval, but
		// by accumulating FPS for each frame. This way we end up with
		// corstartRect overall FPS even if the interval renders something like
		// 5.5 frames.

		public Vector2 startPos = new Vector2 (0.01f, 0.01f);
 
		// The rect the window is initially displayed at.
		public Rect displayRect = new Rect (10, 10, 75, 50);
		// Do you want the color to change if the FPS gets low
		public bool updateColor = true;
		// Do you want to allow the dragging of the FPS window
		public bool allowDrag = true;
		// The update frequency of the fps
		public float frequency = 0.5F;
		// How many decimal do you want to display
		public int nbDecimal = 1;
     
		// FPS accumulated over the interval
		private float accum = 0f;
		// Frames drawn over the interval
		private int frames = 0;
		// The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
		private Color color = Color.white;
		// The fps formatted into a string.
		private string sFPS = "";
		// The style the text will be displayed at, based en defaultSkin.label.
		private GUIStyle style;

		void Start ()
		{
			if (!CocoDebugSettingsData.Instance.IsFPSHudEnabled) {
				Destroy (this);
				return;
			}

			Vector2 pos = new Vector2 (startPos.x * Screen.width, startPos.y * Screen.height);
			pos.x = Mathf.Clamp (pos.x, 0, Screen.width - displayRect.width);
			pos.y = Mathf.Clamp (pos.y, 0, Screen.height - displayRect.height);

			displayRect.position = pos;
			startPos.x = displayRect.x / Screen.width;
			startPos.y = displayRect.y / Screen.height;

			StartCoroutine (FPS ());
		}

		void Update ()
		{
			accum += Time.timeScale / Time.deltaTime;
			++frames;
		}

		IEnumerator FPS ()
		{
			// Infinite loop executed every "frenquency" secondes.
			while (true) {
				// Update the FPS
				float fps = accum / frames;
				sFPS = fps.ToString ("f" + Mathf.Clamp (nbDecimal, 0, 10));
        
				//Update the color
				color = (fps >= 30) ? Color.green : ((fps > 15) ? Color.yellow : Color.red);
            
				accum = 0.0F;
				frames = 0;
            
				yield return new WaitForSeconds (frequency);
			}
		}

		void OnGUI ()
		{
			// Copy the default label skin, change the color and the alignement
			if (style == null) {
				style = new GUIStyle (GUI.skin.label);
				style.normal.textColor = Color.white;
				style.alignment = TextAnchor.MiddleCenter;
			}
        
			GUI.color = updateColor ? color : Color.white;
			displayRect = GUI.Window (0, displayRect, DoMyWindow, "");
			startPos.x = displayRect.x / Screen.width;
			startPos.y = displayRect.y / Screen.height;
		}

		void DoMyWindow (int windowID)
		{
			GUI.Label (new Rect (0, 0, displayRect.width, displayRect.height), sFPS + " FPS", style);
			if (allowDrag)
				GUI.DragWindow (new Rect (0, 0, Screen.width, Screen.height));
		}
	}
}
