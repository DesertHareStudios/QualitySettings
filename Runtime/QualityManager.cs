using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace DesertHareStudios.QualitySettings {

    public static class QualityManager {
        public static event Action<Shadows> OnShadowsChanged;
        public static event Action<bool> OnHDRChanged;
        public static event Action<MSAA> OnAntiAliasingChanged;
        public static event Action<ShadowTier> OnShadowTierChanged;
#if UNITY_URP
        public static event Action<ShadowResolution> OnAdditionalShadowAtlasResolutionChanged;
        public static event Action<PostAntiAliasing> OnPostAntialiasingChanged;
        public static event Action<SoftShadowQuality> OnSoftShadowsQualityChanged;
#endif

        private static bool hasBeenInitialized = false;
        private static bool savePrefs = true;

        public static void Initialize() {
            if(hasBeenInitialized) {
                return;
            }
            savePrefs = false;
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
            SoftShadowsQuality = SoftShadowsQuality;
            CurrentUniversalAsset = UniversalAsset;
#endif
            savePrefs = true;
            hasBeenInitialized = true;
        }

        public static bool RealtimeReflectionProbes {
            get {
                return PlayerPrefs.GetInt(
                    "dhs.qualitysettings.RealtimeReflectionProbes",
                    UnityEngine.QualitySettings.realtimeReflectionProbes ? 1 : 0) > 0;
            }
            set {
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.RealtimeReflectionProbes", value ? 1 : 0);
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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.PixelLightCount", value);
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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.VSync", (int)value);
                UnityEngine.QualitySettings.vSyncCount = (int)value;
            }
        }

        public static TextureSize TextureSize {
            get {
                return (TextureSize)PlayerPrefs.GetInt("dhs.qualitysettings.TextureSize", UnityEngine.QualitySettings.globalTextureMipmapLimit);
            }
            set {
                if(savePrefs) PlayerPrefs.GetInt("dhs.qualitysettings.TextureSize", (int)value);
                UnityEngine.QualitySettings.globalTextureMipmapLimit = (int)value;
            }
        }

        public static AnisotropicFiltering AnisotropicFiltering {
            get {
                return (AnisotropicFiltering)PlayerPrefs.GetInt("dhs.qualitysettings.AnisotropicFiltering", (int)UnityEngine.QualitySettings.anisotropicFiltering);
            }
            set {
                if(savePrefs) PlayerPrefs.GetInt("dhs.qualitysettings.AnisotropicFiltering", (int)value);
                UnityEngine.QualitySettings.globalTextureMipmapLimit = (int)value;
            }
        }

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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.Shadows", (int)value);
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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.MainShadowResolution", (int)value);
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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.ShadowTier", (int)value);
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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.ShadowCascades", value);
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
                if(savePrefs) PlayerPrefs.SetFloat("dhs.qualitysettings.LODBias", value);
                UnityEngine.QualitySettings.lodBias = value;
            }
        }

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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.HDR", value ? 1 : 0);
                OnHDRChanged?.Invoke(value);
#if UNITY_URP
                CurrentUniversalAsset = UniversalAsset;
#endif
            }
        }

        public static MSAA AntiAliasing {
            get {
#if UNITY_URP
                return (MSAA)PlayerPrefs.GetInt("dhs.qualitysettings.AntiAliasing", CurrentUniversalAsset.msaaSampleCount);
#else
                return (MSAA)PlayerPrefs.GetInt("dhs.qualitysettings.AntiAliasing", UnityEngine.QualitySettings.antiAliasing);
#endif
            }
            set {
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.AntiAliasing", (int)value);
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
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.MaterialQuality", (int)value);
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
                return PlayerPrefs.GetFloat("dhs.qualitysettings.RenderScale", 1.0f);
            }
            set {
                if(savePrefs) PlayerPrefs.SetFloat("dhs.qualitysettings.RenderScale", value);
                CurrentUniversalAsset = UniversalAsset;
            }
        }

        public static ShadowResolution AdditionalShadowAtlasResolution {
            get {
                return (ShadowResolution)PlayerPrefs.GetInt(
                    "dhs.qualitysettings.AdditionalShadowAtlasResolution",
                    CurrentUniversalAsset.additionalLightsShadowmapResolution);
            }
            set {
                if(savePrefs) PlayerPrefs.SetInt(
                    "dhs.qualitysettings.AdditionalShadowAtlasResolution",
                    (int)value);
                OnAdditionalShadowAtlasResolutionChanged?.Invoke(value);
                //TODO apply additionalLightsShadowmapResolution somehow
            }
        }

        public static PostAntiAliasing PostAntiAliasing {
            get {
                return (PostAntiAliasing)PlayerPrefs.GetInt("dhs.qualitysettings.PostAntiAliasing", (int)PostAntiAliasing.Off);
            }
            set {
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.PostAntiAliasing", (int)value);
                OnPostAntialiasingChanged?.Invoke(value);
            }
        }

        private static UniversalRenderPipelineAsset internalAsset;
        public static UniversalRenderPipelineAsset CurrentUniversalAsset {
            get {
                if(!internalAsset) {
                    internalAsset = (UniversalRenderPipelineAsset)UnityEngine.QualitySettings.renderPipeline;
                    if(internalAsset) {
                        internalAsset = UnityEngine.Object.Instantiate(internalAsset);
                    }
                }
                if(!internalAsset) {
                    internalAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
                }
#if UNITY_EDITOR
                if(!internalAsset) {
                    internalAsset = UniversalRenderPipelineAsset.Create();
                }
#endif
                return internalAsset;
            }
            private set {
                internalAsset = value;
                UnityEngine.QualitySettings.renderPipeline = internalAsset;
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
            }
        }

        public static int LUTSize {
            get {
                return PlayerPrefs.GetInt("dhs.qualitysettings.lutsize", CurrentUniversalAsset.colorGradingLutSize);
            }
            set {
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.lutsize", Mathf.Clamp(value, 16, 65));
                CurrentUniversalAsset = UniversalAsset;
            }
        }

        public static SoftShadowQuality SoftShadowsQuality {

            get {
                return (SoftShadowQuality)PlayerPrefs.GetInt("dhs.qualitysettings.softshadowsquality", (int)SoftShadowQuality.Medium);
            }
            set {
                if(savePrefs) PlayerPrefs.SetInt("dhs.qualitysettings.softshadowsquality", (int)value);
                OnSoftShadowsQualityChanged?.Invoke(value);
            }
        }
#endif

    }

}