using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale 
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SplashScreen : TabTale.TaskQueue
	{
		public bool autoFit = true;

		private Camera _camera;

		[System.Serializable]
		public class SplashScreenSegment
		{
			public Sprite sprite;
			public float delay;
		}

		public SplashScreenSegment[] segments;
		public bool cyclic = false;

		private SpriteRenderer _renderer;
		private CaptureCamera _captureCamera;
		new void Start()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_camera = transform.parent.GetComponentInChildren<Camera>();

			if(_camera == null)
			{
				GameObject child = new GameObject("Splash_Camera", typeof(Camera));
				_camera = child.AddMissingComponent<Camera>();
			}

			_camera.orthographic = true;
			_camera.transform.position = new Vector3(0, 0, -1);

			_captureCamera = new CaptureCamera(_camera);

			if(transition.effect != null)
			{
				_processor = _camera.gameObject.AddMissingComponent<ShaderProcessor>();
				_processor.material = Instantiate(transition.effect) as Material;
				_processor.material.SetTexture("_tex1", _captureCamera.Texture);
				_processor.material.SetFloat("_startTime", 0f);
				_processor.material.SetFloat("_endTime", transition.duration);
				_processor.material.SetFloat("_now", transition.duration);
			}

			base.Start();
		}

		void Update()
		{
			if(_processor != null)
			{
				if(_switchTime > 0f)
				{
					float elapsed = Time.time - _switchTime;
					_processor.material.SetFloat("_now", elapsed);
				}
			}
		}

		public override void ManualStart()
		{
			StartCoroutine(ShowSplashScreen());
			base.ManualStart();
		}

		private int _splashIndex = 0;

		private void AutoFit()
		{
			Bounds bounds = _renderer.sprite.bounds;

			_camera.orthographicSize = bounds.size.y / 2.0f;
		}

		[System.Serializable]
		public class Transition
		{
			public Material effect;
			public float duration = 2;
		}
		public Transition transition;

		private ShaderProcessor _processor;
		private float _switchTime = 0f;

		/// <summary>
		/// Sets the next sprite on the splash, handling
		/// transition effects if defined.
		/// </summary>
		/// <returns>Amount of seconds which are the minimum wait time for this sprite.</returns>
		/// <param name="sprite">Sprite.</param>
		private float SetSprite(SplashScreenSegment segment)
		{
			if(_renderer.sprite == segment.sprite)
				return 0f;

			CoreLogger.LogDebug(_loggerModule, "setting new sprite");

			_renderer.sprite = segment.sprite;

			if(autoFit)
				AutoFit();

			float waitTime = (_processor != null ? Mathf.Max(segment.delay, transition.duration) : segment.delay);
			return waitTime;
		}

		private bool TasksPending
		{
			get { return Count > 0; }
		}

		private int _loggerModule = 0;
		void Awake()
		{
			_loggerModule = CoreLogger.RegisterModule("SplashScreen");
		}

		private IEnumerator ShowSplashScreen()
		{
			_renderer.sprite = null;

			int numTasks = Count;
			CoreLogger.LogDebug(_loggerModule, string.Format("starting with {0} sprites and {1} tasks...", segments.Length, numTasks));

			if(segments.Length < 1)
			{
				while(TasksPending)
				{
					yield return new WaitForEndOfFrame();
				}

				CoreLogger.LogDebug(_loggerModule, string.Format("finished with 0 sprites and {0} tasks", numTasks));

				Done();
				yield break;
			}

			int i = 0;
			while(TasksPending)
			{
				_captureCamera.Capture();
				if(_renderer.sprite != null)
					_switchTime = Time.time;
				yield return null;

				SplashScreenSegment segment = segments[_splashIndex];
				float waitTime = SetSprite(segment);

				yield return new WaitForSeconds(waitTime);

				i++;
				if(cyclic)
				{
					_splashIndex = (_splashIndex + 1) % segments.Length;
				} else
				{
					_splashIndex = Mathf.Min(_splashIndex + 1, segments.Length - 1);
				}
			}

			CoreLogger.LogDebug(_loggerModule, string.Format("done showing {0} sprites out of {1}, with {2} tasks", i, segments.Length, numTasks));

			Done();
		}

		public event System.Action Done = () => {};
	}

}