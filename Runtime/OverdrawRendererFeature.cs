using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OverdrawForURP
{
	public class OverdrawRendererFeature : ScriptableRendererFeature
	{
#if UNITY_EDITOR || USE_RUNTIME_OVERDRAW
		private OverdrawPass opaquePass;
		private OverdrawPass transparentPass;

		[SerializeField] private Shader opaqueShader = null;
		[SerializeField] private Shader transparentShader = null;
#endif

		public override void Create()
		{
#if UNITY_EDITOR || USE_RUNTIME_OVERDRAW
			if (!opaqueShader || !transparentShader)
			{
				return;
			}
			opaquePass = new OverdrawPass("Overdraw Render Opaque", RenderQueueRange.opaque, opaqueShader, true);
			opaquePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
			transparentPass = new OverdrawPass("Overdraw Render Transparent", RenderQueueRange.transparent, transparentShader, false);
			transparentPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
#endif
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
#if UNITY_EDITOR || USE_RUNTIME_OVERDRAW
			renderer.EnqueuePass(opaquePass);
			renderer.EnqueuePass(transparentPass);
#endif
		}
	}
}
