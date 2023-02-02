namespace DesertHareStudios.QualitySettings {
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
        Off, FXAA, SMAALow, SMAAMedium, SMAAHigh, TAALow, TAAMedium, TAAHigh
    }

    public enum MaterialQuality {
        High = 0,
        Medium = 1,
        Low = 2
    }

}
