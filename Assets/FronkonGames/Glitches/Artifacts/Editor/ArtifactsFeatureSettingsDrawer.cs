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
using UnityEditor;
using static FronkonGames.Glitches.Artifacts.Inspector;

namespace FronkonGames.Glitches.Artifacts.Editor
{
  /// <summary> Artifacts inspector. </summary>
  [CustomPropertyDrawer(typeof(Artifacts.Settings))]
  public class ArtifactsFeatureSettingsDrawer : Drawer
  {
    private Artifacts.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<Artifacts.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 0.", settings.intensity, 1.0f);

      /////////////////////////////////////////////////
      // Artifacts.
      /////////////////////////////////////////////////
      Separator();

      settings.size = Vector2Field("Size", "Artifacts size. Default (16, 16).", settings.size, Artifacts.Settings.DefaultSize);
      settings.aberration = Slider("Aberration", "Color aberration [0, 1]. Default 0.3.", settings.aberration, 0.3f);
      settings.interleave = Slider("Interleave", "Interleave intensity [0, 1]. Default 1.", settings.interleave, 1.0f);
      MinMaxSlider("Luminance range", "Luminance range in which the effect is applied.", ref settings.luminanceRange.x, ref settings.luminanceRange.y, 0.0f, 1.0f, 0.0f, 1.0f);

      settings.blocks = Slider("Blocks", "Intensity of the blocks effect [0, 1]. Default 0.1.", settings.blocks, 0.1f);
      IndentLevel++;
      settings.blockBlend = (ColorBlends)EnumPopup("Blend", "Operation used to mix the block with the original color. Default Multiply.", settings.blockBlend, ColorBlends.Multiply);
      settings.blockTint = ColorField("Tint", "Block color.", settings.blockTint, Artifacts.Settings.DefaultBlockTint);
      IndentLevel--;

      settings.lines = Slider("Lines", "Intensity of the lines effect [0, 1]. Default 0.4.", settings.lines, 0.4f);
      IndentLevel++;
      settings.lineBlend = (ColorBlends)EnumPopup("Blend", "Operation used to mix the line with the original color. Default Multiply.", settings.lineBlend, ColorBlends.Multiply);
      settings.lineTint = ColorField("Tint", "Line color.", settings.lineTint, Artifacts.Settings.DefaultLineTint);
      IndentLevel--;

      /////////////////////////////////////////////////
      // Color.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Color") == true)
      {
        IndentLevel++;

        settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
        settings.contrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.contrast, 0.0f, 10.0f, 1.0f);
        settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
        settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
        settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.saturation, 0.0f, 2.0f, 1.0f);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Advanced.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Advanced") == true)
      {
        IndentLevel++;

#if !UNITY_6000_0_OR_NEWER
        settings.filterMode = (UnityEngine.FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, UnityEngine.FilterMode.Bilinear);
#endif
        settings.affectSceneView = Toggle("Affect the Scene View?", "Does it affect the Scene View?", settings.affectSceneView);
        settings.whenToInsert = (UnityEngine.Rendering.Universal.RenderPassEvent)EnumPopup("RenderPass event",
          "Render pass injection. Default BeforeRenderingTransparents.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingTransparents);
#if !UNITY_6000_0_OR_NEWER
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);
#endif

        IndentLevel--;
      }
    }
  }
}
