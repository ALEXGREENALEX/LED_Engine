using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGL_CS_Game
{
    partial class Game : GameWindow
    {
        public Game() :
            base(800, 600, new GraphicsMode(32, 24, 0, 8), "Hello") // MSAA = 8
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initProgram();
            CursorVisible = false;
            GL.ClearColor(Color.CornflowerBlue);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            // Настраиваем проэкцию (Угол обзора, Мин и Макс расстояния рендера)
            MainCamera.SetProjectionMatrix(ProjectionTypes.Perspective, (float)ClientSize.Width, (float)ClientSize.Height, zNear, zFar, FOV);

            if (UsePostEffects)
                PostProcess.Rescale(Width, Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this.Focused)
            {
                if (e.Key == Key.Escape)
                    Exit();
                else if (e.Alt && e.Key == Key.Enter)
                    if (this.WindowState == WindowState.Fullscreen)
                        this.WindowState = WindowState.Normal;
                    else
                        this.WindowState = WindowState.Fullscreen;
            }
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);

            if (Focused)
                ResetCursor();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (Focused)
            {
                // FPS
                if (ShowFPS)
                {
                    FPS = 1.0 / e.Time;
                    Title = "FPS: " + FPS.ToString("0.00");
                }

                // Bind FBO for PostProcess
                if (UsePostEffects)
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, PostProcess.fbo);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Copy All Objects to DrawableObjects
                DrawableObjects.Clear();
                DrawableObjects.AddRange(Objects);
                foreach (var pref in Prefabs)
                    DrawableObjects.AddRange(pref.Objects);
                if (Debug)
                    DrawableObjects.AddRange(DebugObjects);

                #region Сортировка прозрачных объектов
                for (int i = 0; i < DrawableObjects.Count; i++)
                    if (DrawableObjects[i].Material.Transparent)
                    {
                        TransparentObjects.Add(DrawableObjects[i]);
                        DrawableObjects.RemoveAt(i);
                        i--;
                    }

                TransparentObjects.Sort(delegate(Volume x, Volume y)
                {
                    return (y.Position - MainCamera.Position).Length.CompareTo((x.Position - MainCamera.Position).Length);
                });

                DrawableObjects.AddRange(TransparentObjects);
                TransparentObjects.Clear();
                #endregion

                // Отрисовываем все объекты
                foreach (Volume v in DrawableObjects)
                {
                    v.CalculateModelMatrix();
                    v.ViewProjectionMatrix = MainCamera.GetViewMatrix() * MainCamera.GetProjectionMatrix();
                    v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewProjectionMatrix;

                    // Отрисовка только тех сторон, что повернуты к камере
                    if (v.Material.CullFace)
                        GL.Enable(EnableCap.CullFace);
                    else
                        GL.Disable(EnableCap.CullFace);

                    // Вкл. прозрачность только для прозрачных объектов
                    if (v.Material.Transparent)
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

                    DrawObject(v);
                }

                //PostProcess Bind MAIN FrameBuffer(Screen) & Draw to Screen
                if (UsePostEffects)
                {
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    PostProcess.Draw();
                }

                GL.Flush();
                SwapBuffers();
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Focused)
            {
                Vector2 delta = LastMousePos - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
                LastMousePos += delta;
                MainCamera.AddRotation(delta.X, delta.Y);
                ResetCursor();

                // Проверяем нажатия клавиш каждый кадр, а не по прерыванию!
                KeyboardState KbdState = OpenTK.Input.Keyboard.GetState();
                if (KbdState.IsKeyDown(Key.W))
                    MainCamera.Move(0f, 1.0f, 0f);
                if (KbdState.IsKeyDown(Key.A))
                    MainCamera.Move(-1.0f, 0f, 0f);
                if (KbdState.IsKeyDown(Key.S))
                    MainCamera.Move(0f, -1.0f, 0f);
                if (KbdState.IsKeyDown(Key.D))
                    MainCamera.Move(1.0f, 0f, 0f);
                if (KbdState.IsKeyDown(Key.E))
                    MainCamera.Move(0f, 0f, 1.0f);
                if (KbdState.IsKeyDown(Key.Q))
                    MainCamera.Move(0f, 0f, -1.0f);

                float camSens = 2.0f;
                if (KbdState.IsKeyDown(Key.Left))
                    MainCamera.AddRotation(camSens, 0.0f);
                if (KbdState.IsKeyDown(Key.Right))
                    MainCamera.AddRotation(-camSens, 0.0f);
                if (KbdState.IsKeyDown(Key.Up))
                    MainCamera.AddRotation(0.0f, camSens);
                if (KbdState.IsKeyDown(Key.Down))
                    MainCamera.AddRotation(0.0f, -camSens);

                // Обновляем позиции объектов
                time += (float)e.Time;
                //objects[0].Rotation += new Vector3(0, -0.01f, 0);
                //Prefabs[0].Rotation = new Vector3(0.4f * (float)e.Time, 0.0f, 0.0f);

                Angle += rotSpeed * (float)e.Time;
                if (Angle > MathHelper.TwoPi)
                    Angle -= MathHelper.TwoPi;
            }
        }
    }
}