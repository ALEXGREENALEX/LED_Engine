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

        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; //For fix parsing values like "0.5" and "0,5"

            Glfw.SetErrorCallback(OnError);

            if (!Glfw.Init())
            {
                Log.WriteLineRed("ERROR: Could not initialize GLFW, shutting down.");
                Environment.Exit(1);
            }

            Engine.LoadConfig();
            Engine.ApplyConfig();
            //Engine.LoadContentLists();

            while (Settings.Window.NeedReinitWindow)
            {
                ApplySettingsAndCreateWindow();
                Settings.Window.NeedReinitWindow = false;
                
                Engine.GetGLSettings();

                if (!GLContextIsLoaded)
                {
                    Engine.LoadContentLists();
                    Engine.LoadEngineContent();
                    LoadResourcesFromMap_FAKE();
                    InitProgram();

                    GLContextIsLoaded = true;
                }

                #region PostProcessInit
                if (PostProcess.UsePostEffects)
                {
                    int FBWidth, FBHeight;
                    Glfw.GetFramebufferSize(Window, out FBWidth, out FBHeight);
                    PostProcess.Init(FBWidth, FBHeight);
                }
                #endregion
                RescaleToWindowSize();

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
                    if (Settings.Window.IsFocused)
                    {
                        OnUpdateFrame();
                        OnRenderFrame();
                    }
                    else
                        Thread.Sleep(100);

                    Glfw.SwapBuffers(Window); // Swap the front and back buffer, displaying the scene
                    Glfw.PollEvents(); // Poll GLFW window events

                    if (FPS.ShowFPS)
                        FPS.CalcFPS(); //FPS Counter, must be at the END of the Render LOOP!
                }
                #endregion

                PostProcess.Free();
            }
            Maps.Free(true); // Free all: Map -> Meshes, Shaders, Textures...

            Glfw.DestroyWindow(Window);
            Glfw.Terminate();

            //if (Settings.Debug.Enabled)
            //    Console.ReadKey();
        }
    }
}