using UnityEngine;
using System.Collections;

namespace TabTale {

	/// <summary>
	/// The most basic script, which applies a filter to a camera.
	/// The filter can be though of as a post-processor to the 
	/// image provided by the camera, and comes in the form of
	/// a Material, which is usually simply a container for a shader.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class ShaderProcessor : MonoBehaviour
	{
		public Material material;

		void OnRenderImage (RenderTexture src, RenderTexture dst)
		{
			if(material != null)
				Graphics.Blit(src, dst, material);
		}
	}
}
