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
            RescaleToWindowSize();
        }

        static void OnKeyPress(GlfwWindowPtr window, Key KeyCode, int Scancode, KeyAction Action, KeyModifiers Mods)
        {
            if (KeyCode == Key.Escape)
                Glfw.SetWindowShouldClose(window, true);

            if ((Mods == KeyModifiers.Alt || Mods == KeyModifiers.AltGr) && KeyCode == Key.Enter && Action == KeyAction.Press)
            {
                if (Settings.Window.Fullscreen)
                    Settings.Window.Fullscreen = false;
                else
                    Settings.Window.Fullscreen = true;

                Settings.Window.NeedReinitWindow = true;
            }

            if (KeyCode == Key.F1 && Action == KeyAction.Press)
                Settings.Debug.DrawMode = (Settings.Debug.DrawMode == DrawMode.DrawNormals ? DrawMode.Default : DrawMode.DrawNormals);

            if (KeyCode == Key.F2 && Action == KeyAction.Press)
                Settings.Debug.DrawMode = (Settings.Debug.DrawMode == DrawMode.WordNormal ? DrawMode.Default : DrawMode.WordNormal);

            if (KeyCode == Key.F12 && Action == KeyAction.Press)
            {
                MessageBox.Show(Shaders.SHADERS.Count.ToString(), "SHADERS");
                MessageBox.Show(Textures.TEXTURES.Count.ToString(), "TEXTURES");
                MessageBox.Show(Materials.MATERIALS.Count.ToString(), "MATERIALS");
                MessageBox.Show(Models.MODELS.Count.ToString(), "MODELS");
                MessageBox.Show(Meshes.MESHES.Count.ToString(), "MESHES");
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
        }

        static void OnMouseMove(GlfwWindowPtr window, double posx, double posy)
        {
            double deltaX = Settings.Window.X + Settings.Window.Width / 2 - posx;
            double deltaY = Settings.Window.Y + Settings.Window.Height / 2 - posy;
            MainCamera.AddRotation((float)deltaX, (float)deltaY);
            Glfw.SetCursorPos(window,
                (double)(Settings.Window.X + Settings.Window.Width / 2),
                (double)(Settings.Window.Y + Settings.Window.Height / 2));
        }

        static void OnWindowPositionChange(GlfwWindowPtr window, int X, int Y)
        {
            Settings.Window.X = X;
            Settings.Window.Y = Y;
        }

        static void OnWindowResize(GlfwWindowPtr window, int Width, int Height)
        {
            Settings.Window.Width = Width;
            Settings.Window.Height = Height;
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
            if (Settings.Window.IsFocused)
            {
                if (PostProcess.UsePostEffects) // Bind FBO for PostProcess
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, PostProcess.fbo);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Enable(EnableCap.DepthTest); // Включаем тест глубины
                //GL.DepthFunc(DepthFunction.Lequal);

                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha); // Функция смешивания цветов для прозрачных материалов
                //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.One);

                //if (Settings.Debug.Enabled)
                //{
                //    GL.Disable(EnableCap.Dither);
                //    GL.FrontFace(FrontFaceDirection.Ccw);
                //    GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                //    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);
                //}

                // Copy All Objects to DrawableObjects
                DrawableObjects.Clear();
                DrawableObjects.AddRange(Objects);
                foreach (var Model in Models.MODELS)
                    DrawableObjects.AddRange(Model.Meshes);
                if (Settings.Debug.Enabled && Settings.Debug.DrawDebugObjects)
                    DrawableObjects.AddRange(DebugObjects);

                #region Сортировка прозрачных объектов
                for (int i = 0; i < DrawableObjects.Count; i++)
                    if (DrawableObjects[i].Materials[0].Transparent)
                    {
                        TransparentObjects.Add(DrawableObjects[i]);
                        DrawableObjects.RemoveAt(i);
                        i--;
                    }

                TransparentObjects.Sort(delegate(Mesh A, Mesh B)
                {
                    return (B.Position - MainCamera.Position).Length.CompareTo((A.Position - MainCamera.Position).Length);
                });

                DrawableObjects.AddRange(TransparentObjects);
                TransparentObjects.Clear();
                #endregion

                foreach (Mesh v in DrawableObjects) // Отрисовываем все объекты
                {
                    v.CalculateModelMatrix();
                    v.ModelViewMatrix = v.ModelMatrix * MainCamera.GetViewMatrix();
                    v.ModelViewProjectionMatrix = v.ModelViewMatrix * MainCamera.GetProjectionMatrix();

                    if (v.Materials[0].CullFace) // Отрисовка только тех сторон, что повернуты к камере
                        GL.Enable(EnableCap.CullFace);
                    else
                        GL.Disable(EnableCap.CullFace);

                    if (v.Materials[0].Transparent) // Вкл. прозрачность только для прозрачных объектов
                    {
                        GL.Enable(EnableCap.AlphaTest);
                        GL.Enable(EnableCap.Blend);

                        GL.Enable(EnableCap.CullFace);
                        GL.CullFace(CullFaceMode.Front);
                        DrawObject(v);
                        GL.CullFace(CullFaceMode.Back);
                    }
                    else
                    {
                        GL.Disable(EnableCap.AlphaTest);
                        GL.Disable(EnableCap.Blend);
                    }

                    switch (Settings.Debug.DrawMode)
                    {
                        case DrawMode.Default:
                            DrawObject(v);
                            break;

                        case DrawMode.DrawNormals:
                            DrawObject(v);
                            DrawObject(v, DrawMode.DrawNormals);
                            break;

                        case DrawMode.WordNormal:
                            DrawObject(v, DrawMode.WordNormal);
                            break;
                    }
                }

                if (PostProcess.UsePostEffects)
                {
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    PostProcess.Draw();
                }
            }
        }

        static void OnUpdateFrame()
        {
            // Коеф. изменения позиций объектов относительно падения FPS
            float timeK = 60.0f / (float)FPS.RealFPS;

            // Проверяем нажатия клавиш каждый кадр, а не по прерыванию!
            KeybrdState = KeyboardState.GetState(Window);

            float camMoveSens = 5.0f * timeK;
            if (KeybrdState[Key.W])
                MainCamera.Move(0f, camMoveSens, 0f);
            if (KeybrdState[Key.A])
                MainCamera.Move(-camMoveSens, 0f, 0f);
            if (KeybrdState[Key.S])
                MainCamera.Move(0f, -camMoveSens, 0f);
            if (KeybrdState[Key.D])
                MainCamera.Move(camMoveSens, 0f, 0f);
            if (KeybrdState[Key.E])
                MainCamera.Move(0f, 0f, camMoveSens);
            if (KeybrdState[Key.Q])
                MainCamera.Move(0f, 0f, -camMoveSens);

            float camRotateSens = 2.0f * timeK;
            if (KeybrdState[Key.Left])
                MainCamera.AddRotation(camRotateSens, 0.0f);
            if (KeybrdState[Key.Right])
                MainCamera.AddRotation(-camRotateSens, 0.0f);
            if (KeybrdState[Key.Up])
                MainCamera.AddRotation(0.0f, camRotateSens);
            if (KeybrdState[Key.Down])
                MainCamera.AddRotation(0.0f, -camRotateSens);

            SkyCube.Position = MainCamera.Position;

            Angle += 0.02f * timeK;
            if (Angle > MathHelper.Pi * 2)
                Angle = -MathHelper.Pi * 2;
        }
    }
}