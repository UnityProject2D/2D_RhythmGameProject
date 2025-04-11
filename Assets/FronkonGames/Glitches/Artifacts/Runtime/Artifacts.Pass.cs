////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif

namespace FronkonGames.Glitches.Artifacts
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Artifacts
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

#if UNITY_6000_0_OR_NEWER
#else
      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

      private readonly int renderTextureHandle0 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH0");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif
      private readonly Texture2D noiseTexture;

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

        internal static readonly int LuminanceRange = Shader.PropertyToID("_LuminanceRange");
        internal static readonly int Blocks = Shader.PropertyToID("_Blocks");
        internal static readonly int SizeX = Shader.PropertyToID("_SizeX");
        internal static readonly int SizeY = Shader.PropertyToID("_SizeY");
        internal static readonly int BlockBlend = Shader.PropertyToID("_BlockBlend");
        internal static readonly int BlockTint = Shader.PropertyToID("_BlockTint");
        internal static readonly int Lines = Shader.PropertyToID("_Lines");
        internal static readonly int LineBlend = Shader.PropertyToID("_LineBlend");
        internal static readonly int LineTint = Shader.PropertyToID("_LineTint");
        internal static readonly int Aberration = Shader.PropertyToID("_Aberration");
        internal static readonly int Interleave = Shader.PropertyToID("_Interleave");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class TextureIDs
      {
        internal static readonly int Noise = Shader.PropertyToID("_NoiseTex");
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings) : base()
      {
        this.settings = settings;

        if (noiseTexture == null)
        {
          noiseTexture = Resources.Load<Texture2D>("Textures/Noise");
          if (noiseTexture == null)
            Log.Warning("Error loading texture 'Textures/Noise'");
        }

#if UNITY_6000_0_OR_NEWER
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
#endif
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass()
      {
        material = null;
        Resources.UnloadAsset(noiseTexture);
      }

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        material.SetTexture(TextureIDs.Noise, noiseTexture);
        material.SetVector(ShaderIDs.LuminanceRange, settings.luminanceRange);
        material.SetFloat(ShaderIDs.Blocks, settings.blocks);
        material.SetInt(ShaderIDs.SizeX, settings.size.x);
        material.SetInt(ShaderIDs.SizeY, settings.size.y);
        material.SetInt(ShaderIDs.BlockBlend, (int)settings.blockBlend);
        material.SetColor(ShaderIDs.BlockTint, settings.blockTint);
        material.SetFloat(ShaderIDs.Lines, settings.lines);
        material.SetInt(ShaderIDs.LineBlend, (int)settings.lineBlend);
        material.SetColor(ShaderIDs.LineTint, settings.lineTint);
        material.SetFloat(ShaderIDs.Aberration, settings.aberration);
        material.SetFloat(ShaderIDs.Interleave, settings.interleave);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);
      }

#if UNITY_6000_0_OR_NEWER
      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        if (material == null || settings.intensity == 0.0f)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && settings.affectSceneView == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
        renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

        resourceData.cameraColor = destination;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.intensity <= 0.0f ||
            noiseTexture == null ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        cmd.Blit(colorBuffer, renderTextureHandle0, material);
        cmd.Blit(renderTextureHandle0, colorBuffer, material);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd) => cmd.ReleaseTemporaryRT(renderTextureHandle0);
#else
    #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}
