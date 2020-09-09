using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace OverdrawForURP.Editor
{
	[InitializeOnLoad]
	public class OverdrawMode
	{
		private const string RendererIndexKey = "OverdrawMode.RendererIndex";

		private static SceneView.CameraMode GetOverdrawMode() => SceneView.GetBuiltinCameraMode(DrawCameraMode.Overdraw);

		static OverdrawMode()
		{
			EditorApplication.delayCall += SetUp;
		}

		private static bool IsDefinedOverdrawMode()
		{
			var overdrawMode = GetOverdrawMode();
			var field = typeof(SceneView).GetProperty("userDefinedModes", BindingFlags.Static | BindingFlags.NonPublic);
			var userDefinedModes = field.GetValue(null) as List<SceneView.CameraMode>;
			foreach (var cameraMode in userDefinedModes)
			{
				if (cameraMode.name.Equals(overdrawMode.name) &&
					cameraMode.section.Equals(overdrawMode.section))
				{
					return true;
				}
			}
			return false;
		}

		private static void SetUp()
		{
			if (!Utility.IsUseUniversalRenderPipeline())
			{
				return;
			}

			var overdrawMode = GetOverdrawMode();
			if (!IsDefinedOverdrawMode())
			{
				SceneView.AddCameraMode(overdrawMode.name, overdrawMode.section);
			}

			foreach (SceneView sceneView in SceneView.sceneViews)
			{
				sceneView.onCameraModeChanged += cameraMode =>
				{
					OnCameraModeChanged(sceneView);
				};

				if (sceneView.cameraMode.name.Equals(overdrawMode.name) &&
					sceneView.cameraMode.section.Equals(overdrawMode.section))
				{
					OnCameraModeChanged(sceneView);
				}
			}
		}

		private static void OnCameraModeChanged(SceneView sceneView)
		{
			var overdrawMode = GetOverdrawMode();
			SceneView.CameraMode cameraMode = sceneView.cameraMode;

			if (!Utility.IsExistOverdrawRenderer())
			{
				if (cameraMode.name.Equals(overdrawMode.name))
				{
					Debug.LogError("[OverdrawForURP] Not set OverdrawRenderer asset. Please add to UniversalRenderpipelineAsset.");
				}
				return;
			}

			Object asset = Utility.LoadUniversalRenderPipelineAsset();
			var so = new SerializedObject(asset);
			SerializedProperty prop = so.FindProperty("m_DefaultRendererIndex");
			int overdrawRendererIndex = Utility.GetOverdrawRendererIndex(asset);

			if (cameraMode.name.Equals(overdrawMode.name))
			{
				if (prop.intValue != overdrawRendererIndex)
				{
					EditorPrefs.SetInt(RendererIndexKey, prop.intValue);
				}

				prop.intValue = overdrawRendererIndex;
				so.ApplyModifiedProperties();
			}
			else
			{
				if (EditorPrefs.HasKey(RendererIndexKey) && prop.intValue == overdrawRendererIndex)
				{
					prop.intValue = EditorPrefs.GetInt(RendererIndexKey);
					so.ApplyModifiedProperties();
				}
			}
		}
	}
}
