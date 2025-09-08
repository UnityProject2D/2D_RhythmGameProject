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
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Glitches.Artifacts
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Artifacts
  {
    /// <summary> Settings. </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Common settings.

      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Artifacts settings.

      /// <summary> Artifacts size. Default (16, 16). </summary>
      public Vector2Int size = DefaultSize;

      /// <summary>
      /// Luminance range in which the effect is applied.
      /// If the luminance is less than luminanceRange.x or greater than luminanceRange.y, the effect is not applied.
      /// </summary>
      public Vector2 luminanceRange = Vector2.up;

      /// <summary> Intensity of the blocks effect [0, 1]. Default 0.1. </summary>
      public float blocks = 0.1f;

      /// <summary> Operation used to mix the block with the original color. Default Multiply. </summary>
      public ColorBlends blockBlend = ColorBlends.Multiply;

      /// <summary> Block color. </summary>
      public Color blockTint = DefaultBlockTint;

      /// <summary> Intensity of the lines effect [0, 1]. Default 0.4. </summary>
      public float lines = 0.4f;

      /// <summary> Operation used to mix the line with the original color. Default Multiply. </summary>
      public ColorBlends lineBlend = ColorBlends.Multiply;

      /// <summary> Line color. </summary>
      public Color lineTint = DefaultLineTint;

      /// <summary> Color aberration [0, 1]. Default 0.3. </summary>
      public float aberration = 0.3f;

      /// <summary> Interleave intensity [0, 1]. Default 1. </summary>
      public float interleave = 1.0f;

      public static Vector2Int DefaultSize = Vector2Int.one * 16;
      public static Color DefaultBlockTint = new(0.0f, 1.0f, 0.0f, 0.25f);
      public static Color DefaultLineTint = new(0.0f, 1.0f, 0.0f, 0.5f);

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Color settings.

      /// <summary> Brightness [-1, 1]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0, 10]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0, 1]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
      public float saturation = 1.0f;

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Advanced settings.
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

#if !UNITY_6000_0_OR_NEWER
      /// <summary> Enable render pass profiling. </summary>
      public bool enableProfiling = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
#endif

      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        blocks = 0.1f;
        luminanceRange = Vector2.up;
        size = DefaultSize;
        blockBlend = ColorBlends.Multiply;
        blockTint = DefaultBlockTint;
        lines = 0.4f;
        lineBlend = ColorBlends.Multiply;
        lineTint = DefaultLineTint;
        aberration = 0.3f;
        interleave = 1.0f;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

        affectSceneView = false;
#if !UNITY_6000_0_OR_NEWER
        enableProfiling = false;
        filterMode = FilterMode.Bilinear;
#endif
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      }
    }
  }
}
