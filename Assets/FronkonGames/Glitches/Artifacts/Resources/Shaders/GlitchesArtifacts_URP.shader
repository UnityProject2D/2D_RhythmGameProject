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
Shader "Hidden/Fronkon Games/Glitches/Artifacts URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }
    LOD 100
    ZTest Always ZWrite Off Cull Off

    Pass
    {
      Name "Fronkon Games Glitches Artifacts"

      HLSLPROGRAM
      #pragma target 3.5
      #pragma vertex GlitchesVert
      #pragma fragment GlitchesFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL
      //#pragma multi_compile GLITCHES_DEMO

      #include "Glitches.hlsl"
      #include "ColorBlend.hlsl"

      TEXTURE2D(_NoiseTex);
      float2 _LuminanceRange;
      int _SizeX;
      int _SizeY;
      float _Aberration;
      float _Interleave;
      float _Blocks;
      int _BlockBlend;
      float4 _BlockTint;
      float _Lines;
      int _LineBlend;
      float4 _LineTint;

      half4 GlitchesFrag(const VertexOutput input) : SV_Target0
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const float2 coord = uv * _ScreenParams.xy;

        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;
        
        float lum = Luminance(pixel.rgb);
        UNITY_BRANCH
        if (lum < _LuminanceRange.x || lum > _LuminanceRange.y)
          return pixel;
        
        const float2 _UVNoiseInv = float2(64.0, 64.0);
        
        float2 size = floor(coord / float2(_SizeX, _SizeY));
        float2 uv_noise = size / _UVNoiseInv;
        uv_noise += floor((float2)_Time.y * float2(1234.0, 3543.0)) / _UVNoiseInv;

        float blocks = SafePositivePow_float(frac(_Time.y * 1236.0453), 2.0) * _Blocks;
        float lines = SafePositivePow_float(frac(_Time.y * 2236.0453), 3.0) * _Lines;

        float2 uv_r = uv,
               uv_g = uv,
               uv_b = uv;

        UNITY_BRANCH
        if (SAMPLE_TEXTURE2D(_NoiseTex, sampler_LinearRepeat, uv_noise).r < blocks ||
            SAMPLE_TEXTURE2D(_NoiseTex, sampler_LinearRepeat, float2(uv_noise.y, 0.0)).g < lines)
        {
          float2 dist = (frac(uv_noise) - 0.5) * _Aberration;
          uv_r += dist * 0.1;
          uv_g += dist * 0.2;
          uv_b += dist * 0.15;
        }

        pixel.r = SAMPLE_MAIN(uv_r).r;
        pixel.g = SAMPLE_MAIN(uv_g).g;
        pixel.b = SAMPLE_MAIN(uv_b).b;

        UNITY_BRANCH
        if (SAMPLE_TEXTURE2D(_NoiseTex, sampler_LinearRepeat, uv_noise).g < blocks)
          pixel.rgb = lerp(pixel.rgb, ColorBlend(_BlockBlend, pixel.rgb, _BlockTint.rgb) , _BlockTint.a);
        
        UNITY_BRANCH
        if (SAMPLE_TEXTURE2D(_NoiseTex, sampler_LinearRepeat, float2(uv_noise.y, 0.0)).b * 3.5 < lines)
          pixel.rgb = lerp(pixel.rgb, ColorBlend(_LineBlend, pixel.rgb, _LineTint.rgb) , _LineTint.a);

        UNITY_BRANCH
        if (SAMPLE_TEXTURE2D(_NoiseTex, sampler_LinearRepeat, uv_noise).g * 1.5 < blocks ||
            SAMPLE_TEXTURE2D(_NoiseTex, sampler_LinearRepeat, float2(uv_noise.y, 0.0)).g * 2.5 < lines)
        {
          float interleave = frac(coord.y / 3.0) * _Interleave;
          float3 mask = float3(3.0, 0.0, 0.0);

          UNITY_BRANCH
          if (interleave > 0.333)
            mask = float3(0.0, 3.0, 0.0);

          UNITY_BRANCH
          if (interleave > 0.666)
            mask = float3(0.0, 0.0, 3.0);

          pixel.rgb *= mask;
        }

        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);
#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif
        return lerp(color, pixel, _Intensity);
      }

      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
