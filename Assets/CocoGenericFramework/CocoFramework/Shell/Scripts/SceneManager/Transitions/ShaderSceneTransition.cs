using UnityEngine;
using System.Collections;
using TabTale;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager 
{
	public class ShaderSceneTransition : SceneTransition
	{
		public Material material;

		RenderTexture _oldSceneTexture;
		IList<Camera> _clonedCameras;
		IDictionary<Camera, ShaderProcessor> _shaderProcessors;

		protected override void OnStartTransitionOut ()
		{
			if(material == null)
				return;
			
			_oldSceneTexture = new RenderTexture(Screen.width, Screen.height, 24);

			//make a copy of allCameras, since we'll be modifying it during iteraion
			IList<Camera> allCameras = Camera.allCameras.ToList();
			
			_clonedCameras = allCameras.Select(c => CameraUtils.CloneCamera(c).DontDestroyOnLoad()).ToList();
			
			foreach(Camera camera in _clonedCameras)
			{
				camera.targetTexture = _oldSceneTexture;
				camera.Render();
			}
			
			//we now have the old scene cached in _oldScene
		}

		ShaderProcessor CreateShaderProcessor(Camera camera)
		{
			ShaderProcessor processor = camera.gameObject.AddComponent<ShaderProcessor>();
			processor.material = Instantiate(material) as Material;
			processor.material.SetTexture("_tex1", _oldSceneTexture);
			processor.material.SetFloat("_startTime", 0f);
			processor.material.SetFloat("_endTime", transitionInMinimalDuration);
			processor.material.SetFloat("_now", _now - _transitionStartTime);
			return processor;
		}

		public string targetCameraName = "<unknown>";
	
		protected override void OnStartTransitionIn()
		{
			foreach(Camera camera in _clonedCameras)
			{
				try 
				{
					DestroyObject(camera.gameObject);					
				} catch (System.Exception) 
				{					
				}
			}

			if(material == null)
				return;

			Camera sceneCamera = Camera.allCameras.FirstOrDefault(camera => camera.name.Contains(targetCameraName));
			if(sceneCamera == null)
				sceneCamera = Camera.main;

			_shaderProcessors = new Dictionary<Camera, ShaderProcessor>();
			_shaderProcessors[sceneCamera] = CreateShaderProcessor(sceneCamera);

			StartCoroutine(UpdateShader());
		}

		protected override void OnEndTransitionIn ()
		{
			foreach(ShaderProcessor processor in _shaderProcessors.Values)
			{
				Destroy(processor);
			}

			Destroy(_oldSceneTexture);
		}

		IEnumerator UpdateShader()
		{
			float effectStart = Time.time;
			float elapsed = 0;
			while(transitionInMinimalDuration > elapsed)
			{
				foreach(ShaderProcessor processor in _shaderProcessors.Values)
				{
					processor.material.SetFloat("_now", elapsed);
				}

				yield return null;
				elapsed = Time.time - effectStart;
			}
		}
	}
}
