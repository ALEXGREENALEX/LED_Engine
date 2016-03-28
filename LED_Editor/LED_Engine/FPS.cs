using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using QuickFont;
using QuickFont.Configuration;

namespace LED_Engine
{
    public static class FPS
    {
        const int ElapsedMax = 20; //Calc FPS every 'ElapsedMax' frame

        public static bool ShowFPS = false;
        public static float
            OldTime = 0.0f,
            Period = 0.017f, //Start value, 1/60
            RealFPS = 60.0f; //Start value

        static int ElapsedIndex = 0;
        static float[] Elapsed = new float[ElapsedMax];

        static QFont FPS_Font;
        public static QFontDrawing FPS_FontDrawing;
        public static Matrix4 FPS_Font_ProjectionMatrix = Matrix4.Identity;
        static QFontRenderOptions FPS_Font_Options;

        public static void Font_Init()
        {
            Font_Free();

            FPS_FontDrawing = new QFontDrawing();

            var FontBuilderConfig = new QFontBuilderConfiguration(true);
            FontBuilderConfig.ShadowConfig.blurRadius = 2;
            FontBuilderConfig.ShadowConfig.blurPasses = 1;
            FontBuilderConfig.ShadowConfig.Type = ShadowType.Blurred;
            //FontBuilderConfig.TextGenerationRenderHint = TextGenerationRenderHint.ClearTypeGridFit;

            string FPS_FontPath = Engine.CombinePaths(Settings.Paths.EngineFonts, @"LCD\LCD.ttf");
            FPS_Font = new QFont(FPS_FontPath, 24f, FontBuilderConfig);
            //FPS_Font = new QFont(new Font("Lucida Console", 24f, FontStyle.Regular), FontBuilderConfig);

            FPS_Font_Options = new QFontRenderOptions();
            FPS_Font_Options.DropShadowActive = true;
            FPS_Font_Options.Colour = Color.Yellow;
        }

        public static void Font_Free()
        {
            if (FPS_Font != null)
                FPS_Font.Dispose();
            if (FPS_FontDrawing != null)
                FPS_FontDrawing.Dispose();
        }

        public static void Font_Render()
        {
            string FPS_RenderStr = RealFPS.ToString("0.0");

            FPS_FontDrawing.ProjectionMatrix = FPS_Font_ProjectionMatrix;
            FPS_FontDrawing.DrawingPimitiveses.Clear();
            FPS_FontDrawing.Print(FPS_Font, FPS_RenderStr, new Vector3(7f, Settings.Window.Height - 7f, 0f), QFontAlignment.Left, FPS_Font_Options);
            FPS_FontDrawing.RefreshBuffers();
            FPS_FontDrawing.Draw();
            GL.Disable(EnableCap.Blend);
        }

        public static void CalcFPS()
        {
            float NewTime = (float)Glfw.GetTime();
            float elapsed = NewTime - OldTime;

            Elapsed[ElapsedIndex] = elapsed;
            ElapsedIndex++;
            OldTime = NewTime;

            if (ElapsedIndex >= ElapsedMax)
            {
                ElapsedIndex = 0;

                for (int i = 0; i < ElapsedMax - 1; i++) // (Last + previus)
                    elapsed += Elapsed[i];

                Period = elapsed / (float)ElapsedMax;
                RealFPS = 1.0f / Period;

                //Glfw.SetWindowTitle(Game.Window, "FPS: " + RealFPS.ToString("0.0"));
                //Log.WriteLineGreen("FPS: " + RealFPS.ToString("0.0"));
            }
            Font_Render();
        }
    }
}
