using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Pencil.Gaming;

namespace LED_Engine
{
    public static class FPS
    {
        public static bool ShowFPS = false;
        public static double
            FPS_Frame = 0.0,
            RealFPS = 60.0;

        const double FPS_UpdateTime = 0.1;

        public static void CalcFPS()
        {
            FPS_Frame++;
            if (Glfw.GetTime() >= FPS_UpdateTime)
            {
                RealFPS = FPS_Frame / Glfw.GetTime();
                FPS_Frame = 0.0;
                Glfw.SetTime(0.0);
                Glfw.SetWindowTitle(Game.Window, "FPS: " + RealFPS.ToString("0.0"));
            }
        }
    }

    //public static class UPS
    //{
    //    //static int upsFrameCount = 0;
    //    //static long upsStartTime = 0;

    //    //public static void LimitUPS(int ups)
    //    //{
    //    //    long freq;
    //    //    long frame;
    //    //    freq = Stopwatch.Frequency;
    //    //    frame = Stopwatch.GetTimestamp();
    //    //    while ((frame - upsStartTime) * ups < freq * upsFrameCount)
    //    //    {
    //    //        int sleepTime = (int)((upsStartTime * ups + freq * upsFrameCount - frame * ups) * 1000 / (freq * ups));
    //    //        if (sleepTime > 0) Thread.Sleep(sleepTime);
    //    //        frame = Stopwatch.GetTimestamp();
    //    //    }
    //    //    if (++upsFrameCount > ups)
    //    //    {
    //    //        upsFrameCount = 0;
    //    //        upsStartTime = frame;
    //    //    }
    //    //}
    //}
}
