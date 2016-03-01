using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using QuickFont;
using QuickFont.Configuration;

namespace Example
{
    class Game
    {
        static GlfwWindowPtr Window = GlfwWindowPtr.Null;
        static int Width, Height;

        static QFont mainText;
        static QFontDrawing drawing;

        static Matrix4 QFontProjectionMatrix;
        static QFontRenderOptions mainTextOptions;

        [STAThread]
        private static void Main(string[] args)
        {
            Glfw.SetErrorCallback(OnError);

            if (!Glfw.Init())
            {
                Console.WriteLine("ERROR: Could not initialize GLFW, shutting down.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Glfw.WindowHint(WindowHint.Focused, 1); // Set Focus On Init
            Window = Glfw.CreateWindow(640, 480, "QuickFont GLFW (Pencil.Gaming) Example", GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
            Glfw.MakeContextCurrent(Window);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RescaleToWindowSize();
            OnLoad();

            // On...DoSomething functions
            Glfw.SetFramebufferSizeCallback(Window, OnFramebufferResize);
            Glfw.SetKeyCallback(Window, OnKeyPress);
            Glfw.SetWindowSizeCallback(Window, OnWindowResize);

            // Main Loop
            while (!Glfw.WindowShouldClose(Window))
            {
                OnRenderFrame();

                Glfw.SwapBuffers(Window); // Swap the front and back buffer, displaying the scene
                Glfw.PollEvents(); // Poll GLFW window events
            }

            Glfw.DestroyWindow(Window);
            Glfw.Terminate();
        }

        static void OnError(GlfwError ErrCode, string Description)
        {
            Console.WriteLine("GLFW Error ({0}): {1}", ErrCode, Description);
        }

        static void RescaleToWindowSize()
        {
            Glfw.GetFramebufferSize(Window, out Width, out Height);

            if (Width > 0 && Height > 0)
            {
                GL.Viewport(0, 0, Width, Height);
                QFontProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Width, 0, Height, -1.0f, 1.0f);
            }
        }

        static void OnFramebufferResize(GlfwWindowPtr window, int Width, int Height)
        {
            if (Width > 0 && Height > 0)
                RescaleToWindowSize();
        }

        static void OnWindowResize(GlfwWindowPtr window, int Width, int Height)
        {
            if (Width > 0 && Height > 0)
                RescaleToWindowSize();
        }

        static void OnKeyPress(GlfwWindowPtr window, Key KeyCode, int Scancode, KeyAction Action, KeyModifiers Mods)
        {
            if (Action == KeyAction.Press && KeyCode == Key.Escape)
                Glfw.SetWindowShouldClose(window, true);
        }

        static void OnLoad()
        {
            drawing = new QFontDrawing();

            var builderConfig = new QFontBuilderConfiguration(addDropShadow: true);
            builderConfig.ShadowConfig.blurRadius = 2; //reduce blur radius because font is very small
            builderConfig.ShadowConfig.blurPasses = 1;
            builderConfig.ShadowConfig.Type = ShadowType.Blurred;
            builderConfig.TextGenerationRenderHint = TextGenerationRenderHint.ClearTypeGridFit; //best render hint for this font
            mainText = new QFont("Fonts/times.ttf", 56, builderConfig);
            mainTextOptions = new QFontRenderOptions() { DropShadowActive = true, Colour = Color.White, WordSpacing = 0.5f };
        }

        static void OnRenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            drawing.ProjectionMatrix = QFontProjectionMatrix;
            drawing.DrawingPimitiveses.Clear();
            drawing.Print(mainText, "ModernQuickFont", new Vector3(Width / 2, (Height + 56) / 2, 0), QFontAlignment.Centre, mainTextOptions);
            drawing.RefreshBuffers();
            drawing.Draw();
			//GL.Disable(EnableCap.Blend);
        }
    }
}