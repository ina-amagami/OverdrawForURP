using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using System.Collections.Generic;

namespace OverdrawForURP
{
	public class OverdrawPass : ScriptableRenderPass
	{
		private FilteringSettings m_FilteringSettings;
		private List<ShaderTagId> m_TagIdList = new List<ShaderTagId>();
		private bool m_IsOpaque;
		private Material m_Material;
		
		private class PassData
		{
			internal bool isOpaque;
			internal RendererListHandle rendererListHdl;
			internal UniversalCameraData cameraData;
		}

		public OverdrawPass(string profilerTag, RenderQueueRange renderQueueRange, Shader shader, bool isOpaque)
		{
			m_IsOpaque = isOpaque;

			profilingSampler = new ProfilingSampler(profilerTag);
			m_TagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
			m_TagIdList.Add(new ShaderTagId("UniversalForward"));
			m_TagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
			m_FilteringSettings = new FilteringSettings(renderQueueRange, LayerMask.NameToLayer("Everything"));

			m_Material = CoreUtils.CreateEngineMaterial(shader);
		}

		[Obsolete]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			var cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, profilingSampler))
			{
				var camera = renderingData.cameraData.camera;
				if (m_IsOpaque)
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

				var sortFlags = m_IsOpaque ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
				var drawSettings = CreateDrawingSettings(m_TagIdList, ref renderingData, sortFlags);
				drawSettings.overrideMaterial = m_Material;
				context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
		
		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			var cameraData = frameData.Get<UniversalCameraData>();
			var renderingData = frameData.Get<UniversalRenderingData>();
			var lightData = frameData.Get<UniversalLightData>();

			using (var builder =
			       renderGraph.AddRasterRenderPass<PassData>(passName, out var passData, profilingSampler))
			{
				var resourceData = frameData.Get<UniversalResourceData>();
				
				passData.isOpaque = m_IsOpaque;
				passData.cameraData = cameraData;

				var sortFlags = m_IsOpaque ? passData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
				var drawingSettings = RenderingUtils.CreateDrawingSettings(m_TagIdList, renderingData, passData.cameraData, lightData, sortFlags);
				drawingSettings.overrideMaterial = m_Material;

				var param = new RendererListParams(renderingData.cullResults, drawingSettings, m_FilteringSettings);
				passData.rendererListHdl = renderGraph.CreateRendererList(param);
				builder.UseRendererList(passData.rendererListHdl);
				
				builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
				builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);
				
				builder.SetRenderFunc((PassData data, RasterGraphContext rgContext) =>
					ExecutePass(data, rgContext));
			}
		}
        
        private static void ExecutePass(PassData data, RasterGraphContext context)
        {
	        var camera = data.cameraData.camera;
	        if (data.isOpaque)
	        {
		        if (data.cameraData.isSceneViewCamera || 
		            (camera.TryGetComponent(out UniversalAdditionalCameraData urpCameraData) &&
		             urpCameraData.renderType == CameraRenderType.Base))
		        {
			        context.cmd.ClearRenderTarget(RTClearFlags.ColorDepth, Color.black, 1f, 0);
		        }
	        }
	        context.cmd.DrawRendererList(data.rendererListHdl);
        }
	}
}
