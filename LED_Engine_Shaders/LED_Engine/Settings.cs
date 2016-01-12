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
                    EngineMaps,
                    EngineMeshes,
                    EngineTextures,
                    EngineCubemapTextures,
                    EngineMaterials,
                    EngineShaders,

                    Maps,
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
            public static int MSAASamples = 0;
            public static bool FXAAEnabled = true;

            public static bool UsePostEffects = true;
            public static bool FogEnabled = true;
        }
    }
}
