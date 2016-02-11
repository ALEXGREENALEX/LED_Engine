using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Text;
using System.Threading;
using Pencil.Gaming;

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

                Glfw.SetWindowTitle(Game.Window, "FPS: " + RealFPS.ToString("0.0"));
            }
        }
    }
}
