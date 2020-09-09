using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

namespace OverdrawForURP.Editor
{
	public class OverdrawRendererModificationProcessor : UnityEditor.AssetModificationProcessor
	{
		private static string[] OnWillSaveAssets(string[] paths)
		{
			var list = new List<string>(paths.Length);
			foreach (string path in paths)
			{
				if (IsUsingOverdrawRenderer(path))
				{
					continue;
				}
				list.Add(path);
			}
			return list.ToArray();
		}

		private static bool IsUsingOverdrawRenderer(string path)
		{
			if (!path.EndsWith(".asset")) { return false; }
			var asset = AssetDatabase.LoadAssetAtPath<Object>(path) as UniversalRenderPipelineAsset;
			if (!asset) { return false; }
			return Utility.GetOverdrawRendererIndex(asset) == Utility.GetDefaultRendererIndex(asset);
		}
	}
}
