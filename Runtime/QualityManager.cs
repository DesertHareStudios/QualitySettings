using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace DesertHareStudios.QualitySettings {

    public static class QualityManager {

        public static void Initialize() {
            if(hasBeenInitialized) {
                return;
            }
            RealtimeReflectionProbes = RealtimeReflectionProbes;
            PixelLightCount = PixelLightCount;
            VSync = VSync;
            TextureSize = TextureSize;
            AnisotropicFiltering = AnisotropicFiltering;
            Shadows = Shadows;
            MainShadowResolution = MainShadowResolution;
            ShadowTier = ShadowTier;
            ShadowCascades = ShadowCascades;
            ShadowDistance = ShadowDistance;
            LODBias = LODBias;
            HDR = HDR;
            AntiAliasing = AntiAliasing;
            MaterialQuality = MaterialQuality;
#if UNITY_URP
            RenderScale = RenderScale;
            AdditionalShadowAtlasResolution = AdditionalShadowAtlasResolution;
            PostAntiAliasing = PostAntiAliasing;
            CurrentUniversalAsset = UniversalAsset;
#endif
            hasBeenInitialized = true;
        }

        private static bool hasBeenInitialized = false;


        public static bool RealtimeReflectionProbes {
            get {
                return PlayerPrefs.GetInt(
                    "dhs.qualitysettings.RealtimeReflectionProbes",
                    UnityEngine.QualitySettings.realtimeReflectionProbes ? 1 : 0) > 0;
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.RealtimeReflectionProbes", value ? 1 : 0);
                UnityEngine.QualitySettings.realtimeReflectionProbes = value;
            }
        }

        public static int PixelLightCount {
            get {
                return PlayerPrefs.GetInt(
                    "dhs.qualitysettings.PixelLightCount",
#if UNITY_URP
                    CurrentUniversalAsset.maxAdditionalLightsCount);
#else
                    UnityEngine.QualitySettings.pixelLightCount);
#endif
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.PixelLightCount", value);
                UnityEngine.QualitySettings.pixelLightCount = value;
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static VSync VSync {
            get {
                return (VSync)PlayerPrefs.GetInt("dhs.qualitysettings.VSync", UnityEngine.QualitySettings.vSyncCount);
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.VSync", (int)value);
                UnityEngine.QualitySettings.vSyncCount = (int)value;
            }
        }

        public static TextureSize TextureSize {
            get {
                return (TextureSize)PlayerPrefs.GetInt("dhs.qualitysettings.TextureSize", UnityEngine.QualitySettings.masterTextureLimit);
            }
            set {
                PlayerPrefs.GetInt("dhs.qualitysettings.TextureSize", (int)value);
                UnityEngine.QualitySettings.masterTextureLimit = (int)value;
            }
        }

        public static AnisotropicFiltering AnisotropicFiltering {
            get {
                return (AnisotropicFiltering)PlayerPrefs.GetInt("dhs.qualitysettings.AnisotropicFiltering", (int)UnityEngine.QualitySettings.anisotropicFiltering);
            }
            set {
                PlayerPrefs.GetInt("dhs.qualitysettings.AnisotropicFiltering", (int)value);
                UnityEngine.QualitySettings.masterTextureLimit = (int)value;
            }
        }

        public static event Action<Shadows> OnShadowsChanged;
        public static Shadows Shadows {
            get {
#if UNITY_URP
                var data = CurrentUniversalAsset;
                return (Shadows)PlayerPrefs.GetInt("dhs.qualitysettings.Shadows", (int)( data.supportsMainLightShadows ? ( data.supportsSoftShadows ? Shadows.Soft : Shadows.Hard ) : Shadows.Off ));
#else
                Shadows defaultValue = Shadows.Soft;
                switch(UnityEngine.QualitySettings.shadows) {
                    case UnityEngine.ShadowQuality.Disable:
                        defaultValue = Shadows.Off;
                        break;
                    case UnityEngine.ShadowQuality.HardOnly:
                        defaultValue = Shadows.Hard;
                        break;
                    default:
                    case UnityEngine.ShadowQuality.All:
                        defaultValue = Shadows.Soft;
                        break;
                }
                return (Shadows)PlayerPrefs.GetInt("dhs.qualitysettings.Shadows", (int)defaultValue);
#endif
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.Shadows", (int)value);
                OnShadowsChanged?.Invoke(value);
                switch(value) {
                    case Shadows.Off:
                        UnityEngine.QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
                        break;
                    case Shadows.Hard:
                        UnityEngine.QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
                        break;
                    case Shadows.Soft:
                        UnityEngine.QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                        break;
                }
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static ShadowResolution MainShadowResolution {
            get {
#if UNITY_URP
                return (ShadowResolution)PlayerPrefs.GetInt("dhs.qualitysettings.MainShadowResolution", CurrentUniversalAsset.mainLightShadowmapResolution);
#else
                ShadowResolution defaultValue = ShadowResolution.Resolution512;
                switch(UnityEngine.QualitySettings.shadowResolution) {
                    case UnityEngine.ShadowResolution.Low:
                        defaultValue = ShadowResolution.Resolution256;
                        break;
                    default:
                    case UnityEngine.ShadowResolution.Medium:
                        defaultValue = ShadowResolution.Resolution512;
                        defaultValue = ShadowResolution.Resolution1024;
                        break;
                    case UnityEngine.ShadowResolution.High:
                        defaultValue = ShadowResolution.Resolution2048;
                        break;
                    case UnityEngine.ShadowResolution.VeryHigh:
                        defaultValue = ShadowResolution.Resolution4096;
                        break;
                }
                return (ShadowResolution)PlayerPrefs.GetInt("dhs.qualitysettings.MainShadowResolution", (int)defaultValue); 
#endif
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.MainShadowResolution", (int)value);
                switch(value) {
                    case ShadowResolution.Off:
                    case ShadowResolution.Resolution256:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
                        break;
                    case ShadowResolution.Resolution512:
                    case ShadowResolution.Resolution1024:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Medium;
                        break;
                    case ShadowResolution.Resolution2048:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
                        break;
                    case ShadowResolution.Resolution4096:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                        break;
                }
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static event Action<ShadowTier> OnShadowTierChanged;
        public static ShadowTier ShadowTier {
            get {
                ShadowTier defaultTier = ShadowTier.Medium;
                switch(UnityEngine.QualitySettings.shadowResolution) {
                    case UnityEngine.ShadowResolution.Low:
                        defaultTier = ShadowTier.Off;
                        break;
                    case UnityEngine.ShadowResolution.Medium:
                        defaultTier = ShadowTier.Low;
                        break;
                    case UnityEngine.ShadowResolution.High:
                        defaultTier = ShadowTier.Medium;
                        break;
                    case UnityEngine.ShadowResolution.VeryHigh:
                        defaultTier = ShadowTier.High;
                        break;
                }
                return (ShadowTier)PlayerPrefs.GetInt("dhs.qualitysettings.ShadowTier", (int)defaultTier);
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.ShadowTier", (int)value);
                switch(value) {
                    case ShadowTier.Off:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
                        break;
                    case ShadowTier.Low:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Medium;
                        break;
                    case ShadowTier.Medium:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
                        break;
                    case ShadowTier.High:
                        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                        break;
                }
                OnShadowTierChanged?.Invoke(value);
            }
        }

        public static int ShadowCascades {
            get {
#if UNITY_URP
                return PlayerPrefs.GetInt("dhs.qualitysettings.ShadowCascades", CurrentUniversalAsset.shadowCascadeCount);
#else
                return PlayerPrefs.GetInt("dhs.qualitysettings.ShadowCascades", UnityEngine.QualitySettings.shadowCascades);
#endif
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.ShadowCascades", value);
                UnityEngine.QualitySettings.shadowCascades = value;
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static float ShadowDistance {
            get {
                return PlayerPrefs.GetFloat("dhs.qualitysettings.ShadowDistance",
#if UNITY_URP
                    CurrentUniversalAsset.shadowDistance
#else
                    UnityEngine.QualitySettings.shadowDistance
#endif
                    );
            }
            set {
                PlayerPrefs.SetFloat("dhs.qualitysettings.ShadowDistance", value);
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static float LODBias {
            get {
                return PlayerPrefs.GetFloat("dhs.qualitysettings.LODBias", UnityEngine.QualitySettings.lodBias);
            }
            set {
                PlayerPrefs.SetFloat("dhs.qualitysettings.LODBias", value);
                UnityEngine.QualitySettings.lodBias = value;
            }
        }

        public static event Action<bool> OnHDRChanged;
        public static bool HDR {
            get {
#if UNITY_URP
                return PlayerPrefs.GetInt(
                    "dhs.qualitysettings.HDR",
                    CurrentUniversalAsset.supportsHDR ? 1 : 0) > 0;
#else
                return PlayerPrefs.GetInt("dhs.qualitysettings.HDR", 0) > 0;
#endif
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.HDR", value ? 1 : 0);
                OnHDRChanged?.Invoke(value);
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static event Action<MSAA> OnAntiAliasingChanged;
        public static MSAA AntiAliasing {
            get {
#if UNITY_URP
                return (MSAA)PlayerPrefs.GetInt("dhs.qualitysettings.AntiAliasing", CurrentUniversalAsset.msaaSampleCount);
#else
                return (MSAA)PlayerPrefs.GetInt("dhs.qualitysettings.AntiAliasing", UnityEngine.QualitySettings.antiAliasing);
#endif
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.AntiAliasing", (int)value);
                OnAntiAliasingChanged?.Invoke(value);
                UnityEngine.QualitySettings.antiAliasing = (int)value;
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static MaterialQuality MaterialQuality {
            get {
                return (MaterialQuality)PlayerPrefs.GetInt("dhs.qualitysettings.MaterialQuality", (int)MaterialQuality.High);
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.MaterialQuality", (int)value);
                switch(value) {
                    case MaterialQuality.High:
                        Shader.EnableKeyword("MATERIAL_QUALITY_HIGH");
                        Shader.DisableKeyword("MATERIAL_QUALITY_MEDIUM");
                        Shader.DisableKeyword("MATERIAL_QUALITY_LOW");
                        break;
                    case MaterialQuality.Medium:
                        Shader.DisableKeyword("MATERIAL_QUALITY_HIGH");
                        Shader.EnableKeyword("MATERIAL_QUALITY_MEDIUM");
                        Shader.DisableKeyword("MATERIAL_QUALITY_LOW");
                        break;
                    case MaterialQuality.Low:
                        Shader.DisableKeyword("MATERIAL_QUALITY_HIGH");
                        Shader.DisableKeyword("MATERIAL_QUALITY_MEDIUM");
                        Shader.EnableKeyword("MATERIAL_QUALITY_LOW");
                        break;
                }
            }
        }


#if UNITY_URP
        public static float RenderScale {
            get {
                return PlayerPrefs.GetFloat("dhs.qualitysettings.RenderScale", UnityEngine.QualitySettings.lodBias);
            }
            set {
                PlayerPrefs.SetFloat("dhs.qualitysettings.RenderScale", value);
                CurrentUniversalAsset = UniversalAsset;
            }
        }

        public static event Action<ShadowResolution> OnAdditionalShadowAtlasResolutionChanged;
        public static ShadowResolution AdditionalShadowAtlasResolution {
            get {
                return (ShadowResolution)PlayerPrefs.GetInt(
                    "dhs.qualitysettings.AdditionalShadowAtlasResolution",
                    CurrentUniversalAsset.additionalLightsShadowmapResolution);
            }
            set {
                PlayerPrefs.SetInt(
                    "dhs.qualitysettings.AdditionalShadowAtlasResolution",
                    (int)value);
                OnAdditionalShadowAtlasResolutionChanged?.Invoke(value);
                //TODO apply additionalLightsShadowmapResolution somehow
            }
        }

        public static event Action<PostAntiAliasing> OnPostAntialiasingChanged;

        public static PostAntiAliasing PostAntiAliasing {
            get {
                return (PostAntiAliasing)PlayerPrefs.GetInt("dhs.qualitysettings.PostAntiAliasing", (int)PostAntiAliasing.Off);
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.PostAntiAliasing", (int)value);
                OnPostAntialiasingChanged?.Invoke(value);
            }
        }

        public static UniversalRenderPipelineAsset CurrentUniversalAsset {
            get {
                UniversalRenderPipelineAsset output = (UniversalRenderPipelineAsset)UnityEngine.QualitySettings.renderPipeline;
                if(!output) {
                    output = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
                }
#if UNITY_EDITOR
                if(!output) {
                    output = UniversalRenderPipelineAsset.Create();
                }
#endif
                return output;
            }
            set {
                UnityEngine.QualitySettings.renderPipeline = value;
            }
        }

        public static UniversalRenderPipelineAsset UniversalAsset {
            get {
                UniversalRenderPipelineAsset output = CurrentUniversalAsset;
                output.maxAdditionalLightsCount = PixelLightCount;
                output.msaaSampleCount = (int)AntiAliasing;
                output.renderScale = RenderScale;
                output.shadowCascadeCount = ShadowCascades;
                output.supportsHDR = HDR;
                output.shadowDistance = ShadowDistance;
                output.colorGradingLutSize = LUTSize;
                return output;
            }
            set {
                CurrentUniversalAsset = value;
                PixelLightCount = value.maxAdditionalLightsCount;
                AntiAliasing = (MSAA)value.msaaSampleCount;
                RenderScale = value.renderScale;
                ShadowCascades = value.shadowCascadeCount;
                HDR = value.supportsHDR;
                Shadows = value.supportsMainLightShadows ? ( value.supportsSoftShadows ? Shadows.Soft : Shadows.Hard ) : Shadows.Off;
                ShadowDistance = value.shadowDistance;
                MainShadowResolution = (ShadowResolution)value.mainLightShadowmapResolution;
                AdditionalShadowAtlasResolution = (ShadowResolution)value.additionalLightsShadowmapResolution;
                CurrentUniversalAsset = UniversalAsset;
            }
        }

        public static int LUTSize {
            get {
                return PlayerPrefs.GetInt("dhs.qualitysettings.lutsize", CurrentUniversalAsset.colorGradingLutSize);
            }
            set {
                PlayerPrefs.SetInt("dhs.qualitysettings.lutsize", Mathf.Clamp(value, 16, 65));
                CurrentUniversalAsset = UniversalAsset;
            }
        }
#endif

    }

    public enum VSync {
        Off = 0,
        EveryVBlank = 1,
        EverySecondVBlank = 2
    }

    public enum TextureSize {
        Full = 0,
        Half = 1,
        Quarter = 2,
        Eighth = 3
    }

    public enum Shadows {
        Off = 0, Hard = 1, Soft = 2
    }

    public enum ShadowResolution {
        Off = 0,
        Resolution256 = 256,
        Resolution512 = 512,
        Resolution1024 = 1024,
        Resolution2048 = 2048,
        Resolution4096 = 4096
    }

    public enum ShadowTier {
        Off, Low, Medium, High
    }

    public enum MSAA {
#if UNITY_URP
        Off = 1,
#else
    Off = 0,
#endif
        MSAA2 = 2,
        MSAA4 = 4,
        MSAA8 = 8
    }

    public enum PostAntiAliasing {
        Off, FXAA, SMAALow, SMAAMedium, SMAAHigh
    }

    public enum MaterialQuality {
        High = 0,
        Medium = 1,
        Low = 2
    }

}