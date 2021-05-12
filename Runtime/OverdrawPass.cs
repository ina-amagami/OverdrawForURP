using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace OverdrawForURP
{
	public class OverdrawPass : ScriptableRenderPass
	{
		private string profilerTag;
		private FilteringSettings filteringSettings;
		private List<ShaderTagId> tagIdList = new List<ShaderTagId>();
#if UNITY_2020_2_OR_NEWER
		private ProfilingSampler sampler;
#else
		private ProfilingSampler profilingSampler;
#endif
		private bool isOpaque;
		private Material material;

		public OverdrawPass(string profilerTag, RenderQueueRange renderQueueRange, Shader shader, bool isOpaque)
		{
			this.profilerTag = profilerTag;
			this.isOpaque = isOpaque;

#if UNITY_2020_2_OR_NEWER
			profilingSampler = new ProfilingSampler(nameof(OverdrawPass));
			sampler = new ProfilingSampler(profilerTag);
#else
			profilingSampler = new ProfilingSampler(profilerTag);
#endif
			tagIdList.Add(new ShaderTagId("UniversalForward"));
			tagIdList.Add(new ShaderTagId("LightweightForward"));
			tagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			filteringSettings = new FilteringSettings(renderQueueRange, LayerMask.NameToLayer("Everything"));

			material = CoreUtils.CreateEngineMaterial(shader);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
#if UNITY_2020_2_OR_NEWER
			using (new ProfilingScope(cmd, sampler))
#else
			using (new ProfilingScope(cmd, profilingSampler))
#endif
			{
				var camera = renderingData.cameraData.camera;
				if (isOpaque)
				{
					if (renderingData.cameraData.isSceneViewCamera || 
						(camera.TryGetComponent(out UniversalAdditionalCameraData urpCameraData) &&
						urpCameraData.renderType == CameraRenderType.Base))
					{
						cmd.ClearRenderTarget(true, true, Color.black);
					}
				}
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();

				var sortFlags = isOpaque ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
				var drawSettings = CreateDrawingSettings(tagIdList, ref renderingData, sortFlags);
				drawSettings.overrideMaterial = material;
				context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}
}
