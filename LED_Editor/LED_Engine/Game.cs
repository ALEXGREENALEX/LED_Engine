using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    partial class Game
    {
        static void OnError(GlfwError ErrCode, string Description)
        {
            Log.WriteLineRed("GLFW Error ({0}): {1}", ErrCode, Description);
        }

        static void OnFramebufferResize(GlfwWindowPtr window, int Width, int Height)
        {
            if (Width > 0 && Height > 0)
                RescaleToWindowSize();
        }

        static void OnKeyPress(GlfwWindowPtr window, Key KeyCode, int Scancode, KeyAction Action, KeyModifiers Mods)
        {
            if (KeyCode == Key.Escape)
            {
                focused = false;
                editor_topform.Focus();
            }

            if ((Mods == KeyModifiers.Alt || Mods == KeyModifiers.AltGr) && KeyCode == Key.Enter && Action == KeyAction.Press)
            {
                if (Settings.Window.Fullscreen)
                    Settings.Window.Fullscreen = false;
                else
                    Settings.Window.Fullscreen = true;

                Settings.Window.NeedReinitWindow = true;
            }

            #region DrawMode
            if (KeyCode == Key.F1 && Action == KeyAction.Press)
            {
                String[] S = Enum.GetNames(typeof(DrawMode));
                if (Settings.Debug.DrawMode.ToString().GetHashCode() == S[0].GetHashCode())
                    Settings.Debug.DrawMode = (DrawMode)Enum.Parse(typeof(DrawMode), S[S.Length - 1]);
                else
                    Settings.Debug.DrawMode = (DrawMode)((int)(Settings.Debug.DrawMode) - 1);
            }

            if (KeyCode == Key.F2 && Action == KeyAction.Press)
            {
                String[] S = Enum.GetNames(typeof(DrawMode));
                if (Settings.Debug.DrawMode.ToString().GetHashCode() == S[S.Length - 1].GetHashCode())
                    Settings.Debug.DrawMode = (DrawMode)0;
                else
                    Settings.Debug.DrawMode = (DrawMode)((int)(Settings.Debug.DrawMode) + 1);
            }

            switch (Settings.Debug.DrawMode)
            {
                case DrawMode.Default:
                    FBO.ShaderIndex_P1 = FBO.ShaderIndex_P1_Orig;
                    break;
                case DrawMode.Depth:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_Depth;
                    break;
                case DrawMode.Diffuse:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_Diffuse;
                    break;
                case DrawMode.Light:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_Light;
                    break;
                case DrawMode.Normals:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_Normals;
                    break;
                case DrawMode.AmbientOclusion:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_AmbientOclusion;
                    break;
                case DrawMode.Emissive:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_Emissive;
                    break;
                case DrawMode.Specular:
                    FBO.ShaderIndex_P1 = FBO.DebugShaderIndex_Specular;
                    break;
            }
            #endregion

            if (KeyCode == Key.F12 && Action == KeyAction.Press)
            {
                LightSettings lightSettings = new LightSettings();
                lightSettings.Show();
            }
        }

        static void OnMouseClick(GlfwWindowPtr window, MouseButton MouseButton, KeyAction KeyAction)
        {
            // Mouse jump fix
            //Glfw.SetInputMode(Window, InputMode.CursorMode, CursorMode.CursorCaptured);
            //Glfw.GetWindowPos(Window, out Settings.Window.X, out Settings.Window.Y);
            //Glfw.SetCursorPos(Window,
            //    (double)(Settings.Window.X + Settings.Window.Width / 2),
            //    (double)(Settings.Window.Y + Settings.Window.Height / 2));
            focused = true;
        }

        static void OnMouseMove(GlfwWindowPtr window, double posx, double posy)
        {
            if (Settings.Window.IsFocused)
            {
                if (focused)
                {
                    double CenterX = Settings.Window.X + Settings.Window.Width / 2;
                    double CenterY = Settings.Window.Y + Settings.Window.Height / 2;

                    MainCamera.AddRotation((float)(CenterX - posx), (float)(CenterY - posy));
                    Glfw.SetCursorPos(window, CenterX, CenterY);
                }
            }
        }

        static void OnWindowPositionChange(GlfwWindowPtr window, int X, int Y)
        {
            Settings.Window.X = X;
            Settings.Window.Y = Y;
        }

        static void OnWindowResize(GlfwWindowPtr window, int Width, int Height)
        {
            if (Width > 0 || Height > 0)
            {
                Settings.Window.Width = Width;
                Settings.Window.Height = Height;
            }
        }

        static void OnFocusChanged(GlfwWindowPtr window, bool Focused)
        {
            Settings.Window.IsFocused = Focused;

            if (Settings.Window.IsFocused)
                Glfw.SetInputMode(window, InputMode.CursorMode, CursorMode.CursorCaptured);
            else
                Glfw.SetInputMode(window, InputMode.CursorMode, CursorMode.CursorNormal);
        }

        static void OnRenderFrame()
        {
            // G-Buffer
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO.FBO_P1);
            GL.Enable(EnableCap.DepthTest); // Включаем тест глубины
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(Color4.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Copy All Meshes to DrawableObjects
            List<Mesh> DrawableMeshes = new List<Mesh>();

            for (int i = 0; i < Models.MODELS.Count; i++)
                if (Models.MODELS[i].Visible)
                    DrawableMeshes.AddRange(Models.MODELS[i].Meshes);

            foreach (Mesh v in DrawableMeshes) //Draw All
                Draw(v);

            // Draw All Geometry to PostProcess FBO
            FBO.Draw_P1();

            // Draw SkyBox to PostProcess FBO
            if (SkyBox != null)
            {
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO.FBO_PP);
                GL.ClearColor(Color4.Gray);
                GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.DepthTest);
                Draw(SkyBox);
            }

            // Draw PostProcessed Result to Screen (FBO = 0)
            FBO.Draw_PostPocess();
        }

        static void OnUpdateFrame()
        {
            // Проверяем нажатия клавиш каждый кадр, а не по прерыванию!
            KeybrdState = KeyboardState.GetState(Window);

            if (focused == true)
            {
                float camMoveSens = 5.0f * FPS.Period;
                float camRotateSens = 200.0f * FPS.Period;
                float MoveX = 0.0f, MoveY = 0.0f, MoveZ = 0.0f,
                    RotateX = 0.0f, RotateY = 0.0f;

                if (KeybrdState[Key.W])
                    MoveY += camMoveSens;
                if (KeybrdState[Key.A])
                    MoveX -= camMoveSens;
                if (KeybrdState[Key.S])
                    MoveY -= camMoveSens;
                if (KeybrdState[Key.D])
                    MoveX += camMoveSens;
                if (KeybrdState[Key.E])
                    MoveZ += camMoveSens;
                if (KeybrdState[Key.Q])
                    MoveZ -= camMoveSens;

                if (MoveX != 0 || MoveY != 0 || MoveZ != 0)
                    MainCamera.Move(MoveX, MoveY, MoveZ);

                if (KeybrdState[Key.Left])
                    RotateX += camRotateSens;
                if (KeybrdState[Key.Right])
                    RotateX -= camRotateSens;
                if (KeybrdState[Key.Up])
                    RotateY += camRotateSens;
                if (KeybrdState[Key.Down])
                    RotateY -= camRotateSens;

                if (RotateX != 0 || RotateY != 0)
                    MainCamera.AddRotation(RotateX, RotateY);

                SkyBox.Position = MainCamera.Position;
            }
        }
    }
}