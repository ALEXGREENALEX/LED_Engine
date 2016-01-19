using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    partial class Game
    {
        public static bool GLContextIsLoaded = false;
        public static GlfwWindowPtr Window = GlfwWindowPtr.Null;

        static KeyboardState KeybrdState;

        [STAThread]
        private static void Main(string[] args)
        {
            Lights.MakeSomeLightsForTEST();

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
                RescaleToWindowSize();
                Settings.Window.NeedReinitWindow = false;

                if (!GLContextIsLoaded)
                {
                    Engine.GetGLSettings();
                    Engine.LoadContentLists();
                    
                    // FBO Init
                    int FB_Width, FB_Height;
                    Glfw.GetFramebufferSize(Window, out FB_Width, out FB_Height);
                    FBO.Init(FB_Width, FB_Height);

                    Engine.LoadEngineContent();
                    LoadResourcesFromMap_FAKE();
                    InitProgram();

                    GLContextIsLoaded = true;
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
                    Glfw.PollEvents(); // Poll GLFW window events
                    if (Settings.Window.IsFocused)
                    {
                        OnUpdateFrame();
                        OnRenderFrame();
                    }
                    else
                        Thread.Sleep(100);

                    Glfw.SwapBuffers(Window); // Swap the front and back buffer, displaying the scene

                    if (FPS.ShowFPS)
                        FPS.CalcFPS(); //FPS Counter, must be at the END of the Render LOOP!
                }
                #endregion
            }
            Maps.Free(true); // Free all: Map -> Meshes, Shaders, Textures...
            FBO.Free(true);
            Glfw.DestroyWindow(Window);
            Glfw.Terminate();

            //if (Settings.Debug.Enabled)
            //    Console.ReadKey();
        }
    }
}