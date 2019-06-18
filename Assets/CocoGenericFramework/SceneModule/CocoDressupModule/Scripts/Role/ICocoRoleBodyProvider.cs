using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay
{
	public interface ICocoRoleBodyProvider
	{
		Transform GetBone (string boneName);

		SkinnedMeshRenderer GetRenderer (string rendererName);

		event System.Action<SkinnedMeshRenderer, bool> OnRendererChanged;
	}
}