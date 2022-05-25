using UnityEngine;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace DesertHareStudios.QualitySettings {

    [RequireComponent(typeof(Camera))]
#if UNITY_URP
    [RequireComponent(typeof(UniversalAdditionalCameraData))]
#endif
    public class QualityCameraSettings : MonoBehaviour {

        private Camera cam;

#if UNITY_URP
        private UniversalAdditionalCameraData additionalCameraData;
#endif

        private void Awake() {
            QualityManager.Initialize();
            cam = GetComponent<Camera>();

            QualityManager.OnAntiAliasingChanged += OnAntiAliasingChanged;
            QualityManager.OnHDRChanged += OnHDRChanged;

            OnAntiAliasingChanged(QualityManager.AntiAliasing);
            OnHDRChanged(QualityManager.HDR);

#if UNITY_URP
            additionalCameraData = GetComponent<UniversalAdditionalCameraData>();

            QualityManager.OnShadowsChanged += OnShadowsChanged;
            QualityManager.OnPostAntialiasingChanged += OnPostAntialiasingChanged;

            OnShadowsChanged(QualityManager.Shadows);
            OnPostAntialiasingChanged(QualityManager.PostAntiAliasing);
#endif
        }

        private void OnDestroy() {
            QualityManager.OnAntiAliasingChanged -= OnAntiAliasingChanged;
            QualityManager.OnHDRChanged -= OnHDRChanged;
#if UNITY_URP
            QualityManager.OnShadowsChanged -= OnShadowsChanged;
            QualityManager.OnPostAntialiasingChanged -= OnPostAntialiasingChanged;
#endif
        }

        private void OnAntiAliasingChanged(MSAA obj) {
            switch(obj) {
                case MSAA.Off:
                    cam.allowMSAA = false;
                    break;
                case MSAA.MSAA2:
                case MSAA.MSAA4:
                case MSAA.MSAA8:
                    cam.allowMSAA = true;
                    break;
            }
        }

        private void OnHDRChanged(bool obj) {
            cam.allowHDR = obj;
        }

#if UNITY_URP
        private void OnShadowsChanged(Shadows obj) {
            switch(obj) {
                case Shadows.Off:
                    additionalCameraData.renderShadows = false;
                    break;
                case Shadows.Hard:
                case Shadows.Soft:
                    additionalCameraData.renderShadows = true;
                    break;
            }
        }

        private void OnPostAntialiasingChanged(PostAntiAliasing obj) {
            switch(obj) {
                case PostAntiAliasing.Off:
                    additionalCameraData.antialiasing = AntialiasingMode.None;
                    additionalCameraData.antialiasingQuality = AntialiasingQuality.Low;
                    break;
                case PostAntiAliasing.FXAA:
                    additionalCameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    additionalCameraData.antialiasingQuality = AntialiasingQuality.Medium;
                    break;
                case PostAntiAliasing.SMAALow:
                    additionalCameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    additionalCameraData.antialiasingQuality = AntialiasingQuality.Low;
                    break;
                case PostAntiAliasing.SMAAMedium:
                    additionalCameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    additionalCameraData.antialiasingQuality = AntialiasingQuality.Medium;
                    break;
                case PostAntiAliasing.SMAAHigh:
                    additionalCameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    additionalCameraData.antialiasingQuality = AntialiasingQuality.High;
                    break;
            }
        }
#endif
    }
}