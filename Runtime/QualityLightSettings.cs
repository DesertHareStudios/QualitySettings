using UnityEngine;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace DesertHareStudios.QualitySettings {

    [RequireComponent(typeof(Light))]
#if UNITY_URP
    [RequireComponent(typeof(UniversalAdditionalLightData))]
#endif
    public class QualityLightSettings : MonoBehaviour {

        private Light l;
#if UNITY_URP
        private UniversalAdditionalLightData uald;
#endif

        private void Awake() {
            QualityManager.Initialize();
            l = GetComponent<Light>();
            QualityManager.OnShadowTierChanged += OnShadowTierChanged;
            QualityManager.OnShadowsChanged += OnShadowsChanged;
#if UNITY_URP
            uald = GetComponent<UniversalAdditionalLightData>();
            QualityManager.OnAdditionalShadowAtlasResolutionChanged += OnAdditionalShadowAtlasResolutionChanged;
            OnAdditionalShadowAtlasResolutionChanged(QualityManager.AdditionalShadowAtlasResolution);
#endif
            OnShadowTierChanged(QualityManager.ShadowTier);
            OnShadowsChanged(QualityManager.Shadows);
        }

        private void OnDestroy() {
            QualityManager.OnShadowTierChanged -= OnShadowTierChanged;
            QualityManager.OnShadowsChanged -= OnShadowsChanged;
#if UNITY_URP
            QualityManager.OnAdditionalShadowAtlasResolutionChanged -= OnAdditionalShadowAtlasResolutionChanged;
#endif
        }

        private void OnShadowsChanged(Shadows obj) {
            switch(obj) {
                case Shadows.Off:
                    l.shadows = LightShadows.None;
                    break;
                case Shadows.Hard:
                    l.shadows = LightShadows.Hard;
                    break;
                case Shadows.Soft:
                    l.shadows = LightShadows.Soft;
                    break;
            }
        }

        private void OnShadowTierChanged(ShadowTier obj) {
            switch(obj) {
                case ShadowTier.Off:
                    l.shadows = LightShadows.None;
                    break;
                case ShadowTier.Low:
                case ShadowTier.Medium:
                case ShadowTier.High:
                    switch(QualityManager.Shadows) {
                        case Shadows.Off:
                            l.shadows = LightShadows.None;
                            break;
                        case Shadows.Hard:
                            l.shadows = LightShadows.Hard;
                            break;
                        case Shadows.Soft:
                            l.shadows = LightShadows.Soft;
                            break;
                    }
                    break;
            }
#if UNITY_URP
            switch(obj) {
                case ShadowTier.Off:
                case ShadowTier.Low:
                    l.shadowCustomResolution = QualityManager.CurrentUniversalAsset.additionalLightsShadowResolutionTierLow;
                    break;
                case ShadowTier.Medium:
                    l.shadowCustomResolution = QualityManager.CurrentUniversalAsset.additionalLightsShadowResolutionTierMedium;
                    break;
                case ShadowTier.High:
                    l.shadowCustomResolution = QualityManager.CurrentUniversalAsset.additionalLightsShadowResolutionTierHigh;
                    break;
            }
#endif
        }

#if UNITY_URP
        private void OnAdditionalShadowAtlasResolutionChanged(ShadowResolution obj) {
            OnShadowTierChanged(QualityManager.ShadowTier);
        }
#endif

    }
}