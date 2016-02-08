using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Settings
    {
        public static float UnitsScale = 0.01f; // Система измерений (это значение по умолчанию, 1 unit = 100 см)

        public static class Debug
        {
            public static bool
                Enabled = false,
                ShowFPS = true,
                DrawDebugObjects = true;

            public static DrawMode DrawMode = DrawMode.Default;
        }

        public static class Paths
        {
            public static string
                StartupPath,
                EngineContentPath,
                GameDataPath,

                EngineMaps,
                EngineMeshes,
                EngineTextures,
                EngineCubemapTextures,
                EngineShaders,

                Maps,
                Meshes,
                Textures,
                CubemapTextures,
                Shaders;

            public static class ContentFiles
            {
                public static string
                    EngineMeshes,
                    EngineTextures,
                    EngineCubemapTextures,
                    EngineMaterials,
                    EngineShaders,

                    Meshes,
                    Textures,
                    CubemapTextures,
                    Materials,
                    Shaders;
            }
        }

        public static class Window
        {
            public static int
                ActiveMonitor = 0,

                OriginalX = 0, // "Original..." - values from config file
                OriginalY = 0,
                OriginalWidth = 800,
                OriginalHeight = 600,

                X = 0,
                Y = 0,
                Width = 800,
                Height = 600,

                RefreshRate = 0,
                RefreshRateLimit = 200,
                RedBits = 8,
                GreenBits = 8,
                BlueBits = 8;

            public static string Title = "";
            public static bool
                NeedReinitWindow = true,
                Fullscreen = false,
                FullscreenWindowed = false,
                CenterWindow = false,
                Borders = true,
                IsFocused = true;
        }

        public static class Graphics
        {
            public static int VSyncSwapInterval = 0;

            public static bool UsePostEffects = true;

            public static class FXAA
            {
                public static bool Enabled = true;

                /// <summary>
                /// fxaaQualitySubpix
                /// Default: 0.75
                /// </summary>
                public static float Subpix = 1.0f;

                /// <summary>
                /// fxaaQualityEdgeThreshold
                /// Default: 0.166
                /// </summary>
                public static float EdgeThreshold = 0.125f;

                /// <summary>
                /// fxaaQualityEdgeThresholdMin
                /// Default: 0.0625
                /// </summary>
                public static float EdgeThresholdMin = 0.0625f;
            }

            public static class Fog
            {
                public static bool Enabled = true;
                public static bool UseFogOnMap = false;
                public static float MinDistance = 20f;
                public static float MaxDistance = 80f;
                public static Vector3 Color = new Vector3(0.5f, 0.5f, 0.5f);
            }

            public static class Vignette
            {
                public static bool Enabled = true;

                /// <summary>
                /// Radius of vignette circle, default 0.5 (fit circle) - 0.75
                /// </summary>
                public static float Radius = 0.75f;

                /// <summary>
                /// Softness [0..1], default 0.5
                /// </summary>
                public static float Softness = 0.5f;

                /// <summary>
                /// Opacity [0..1], default 0.5
                /// </summary>
                public static float Opacity = 0.5f;
            }

            public static class Sepia
            {
                public static bool Enabled = false;

                /// <summary>
                /// Opacity [0..1], default 0.5
                /// </summary>
                public static float Opacity = 0.5f;
            }

            public static class GrayScale
            {
                public static bool Enabled = false;

                /// <summary>
                /// Opacity [0..1], default 0.5
                /// </summary>
                public static float Opacity = 0.5f;
            }
        }

        public static class GL
        {
            /// <summary>
            /// MaxTextureImageUnits - This is the number of fragment shader texture image units.
            /// </summary>
            public static int MaxTextureImageUnits = 0; //FS
            public static int MaxVertexTextureImageUnits = 0; //VS
            public static int MaxGeometryTextureImageUnits = 0; //GS

            /// <summary>
            /// TextureImageUnits = Min(MaxTextureImageUnits, MaxVertexTextureImageUnits, MaxGeometryTextureImageUnits)
            /// </summary>
            public static int TextureImageUnits = 0;
        }
    }
}
