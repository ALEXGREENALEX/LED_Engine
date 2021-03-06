﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    partial class Game
    {
        public static bool ContextIsLoaded = false;
        public static GlfwWindowPtr Window = GlfwWindowPtr.Null;

        static KeyboardState KeybrdState;

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; //For fix parsing values like "0.5" and "0,5"

            Glfw.SetErrorCallback(OnError);

            if (!Glfw.Init())
            {
                Log.WriteLineRed("ERROR: Could not initialize GLFW, shutting down.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Engine.LoadConfig();
            Engine.ApplyConfig();

            while (Settings.Window.NeedReinitWindow)
            {
                ApplySettingsAndCreateWindow();
                Engine.GetGLSettings();
                Engine.LoadContentLists();

                int FB_Width, FB_Height;
                Glfw.GetFramebufferSize(Window, out FB_Width, out FB_Height);
                FBO.Init(FB_Width, FB_Height);
                RescaleToWindowSize(); // This function call FBO.Init()!!! That's is needed! (mb GLFW bug...)
                
                Settings.Window.NeedReinitWindow = false;

                if (!ContextIsLoaded)
                {
                    Engine.LoadEngineContent();

                    //Load Map
                    if (args.Length > 0)
                        Maps.LoadMap(args[0]);
                    else
                        Maps.LoadMap("Sponza"); //Sponza //SampleMap //Materials_Test_Map

                    ContextIsLoaded = true;
                }

                // On...DoSomething functions
                Glfw.SetCursorPosCallback(Window, OnMouseMove);
                Glfw.SetMouseButtonCallback(Window, OnMouseClick);
                Glfw.SetFramebufferSizeCallback(Window, OnFramebufferResize);
                Glfw.SetKeyCallback(Window, OnKeyPress);
                Glfw.SetWindowFocusCallback(Window, OnFocusChanged);
                Glfw.SetWindowPosCallback(Window, OnWindowPositionChange);
                Glfw.SetWindowSizeCallback(Window, OnWindowResize);

                #region Main Loop
                while (!(Glfw.WindowShouldClose(Window) || Settings.Window.NeedReinitWindow))
                {
                    OnUpdateFrame();
                    OnRenderFrame();

                    if (FPS.ShowFPS)
                        FPS.CalcFPS(); //FPS Counter, must be at the END of the Render LOOP!

                    Glfw.SwapBuffers(Window); // Swap the front and back buffer, displaying the scene
                    Glfw.PollEvents(); // Poll GLFW window events
                }
                #endregion
            }
            Maps.Free(true); // Free all: Map -> Meshes, Shaders, Textures...
            FBO.Free();
            FPS.Font_Free();
            Engine.ClearLists();
            Glfw.DestroyWindow(Window);
            Glfw.Terminate();

            if (Settings.Debug.Enabled)
            {
                //Console.ReadKey();
                string log = Log.GetLog();
                if (log.Length > 256)
                    System.Windows.Forms.Clipboard.SetText(log);
            }
        }
    }
}