using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FpsCounterTxt : MonoBehaviour
{

	public  float fpsUpdateInterval = 0.5F;
	private float _accum = 0; // FPS accumulated over the interval
	private int   _frames = 0; // Frames drawn over the interval
	private float _timeleft; // Left time for current interval
	private Text _fpsTxt;

	void Awake ()
	{
		_fpsTxt  = GetComponent<Text>();
	}

	// Use this for initialization
	void Start ()
	{
		_timeleft = fpsUpdateInterval;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_timeleft -= Time.deltaTime;
		_accum += Time.timeScale / Time.deltaTime;
		++_frames;
		
		// Interval ended - update GUI text and start new interval
		if (_timeleft <= 0.0) {
			// display two fractional digits (f2 format)
			float fps = _accum / _frames;
			string format = System.String.Format ("{0:F2} FPS", fps);
			_fpsTxt.text = format;

			_timeleft = fpsUpdateInterval;
			_accum = 0.0F;
			_frames = 0;
		}
	}
}
