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
            GL.ClearColor(Color.CornflowerBlue);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            // Настраиваем проэкцию (Угол обзора, Мин и Макс расстояния рендера)
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, ClientSize.Width / (float)ClientSize.Height, zNear, zFar);

            if (UsePostEffects)
                PostProcess.Rescale(Width, Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();
            else if (e.Alt && e.Key == Key.Enter)
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // FPS
            FPS = 1.0 / e.Time;
            Title = "FPS: " + FPS.ToString("0.00");

            // Bind FBO for PostProcess
            if (UsePostEffects)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, PostProcess.fbo);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            #region Сортировка прозрачных объектов
            for (int i = 0; i < objects.Count; i++)
                if (objects[i].Material.Transparent)
                {
                    transparentObjects.Add(objects[i]);
                    objects.RemoveAt(i);
                    i--;
                }

            transparentObjects.Sort(delegate(Volume x, Volume y)
            {
                return (y.Position - cam.Position).Length.CompareTo((x.Position - cam.Position).Length);
            });

            objects.AddRange(transparentObjects);
            transparentObjects.Clear();
            #endregion

            // Отрисовываем все объекты
            foreach (Volume v in objects)
            {
                v.CalculateModelMatrix();
                v.ViewProjectionMatrix = cam.GetViewMatrix() * ProjectionMatrix;
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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Проверяем нажатия клавиш каждый кадр, а не по прерыванию!
            KeyboardState KbdState = OpenTK.Input.Keyboard.GetState();
            if (KbdState.IsKeyDown(Key.W))
                cam.Move(0f, 0.01f, 0f);
            if (KbdState.IsKeyDown(Key.A))
                cam.Move(-0.01f, 0f, 0f);
            if (KbdState.IsKeyDown(Key.S))
                cam.Move(0f, -0.01f, 0f);
            if (KbdState.IsKeyDown(Key.D))
                cam.Move(0.01f, 0f, 0f);
            if (KbdState.IsKeyDown(Key.E))
                cam.Move(0f, 0f, 0.01f);
            if (KbdState.IsKeyDown(Key.Q))
                cam.Move(0f, 0f, -0.01f);

            float camSens = 2.0f;
            if (KbdState.IsKeyDown(Key.Left))
                cam.AddRotation(camSens, 0.0f);
            if (KbdState.IsKeyDown(Key.Right))
                cam.AddRotation(-camSens, 0.0f);
            if (KbdState.IsKeyDown(Key.Up))
                cam.AddRotation(0.0f, camSens);
            if (KbdState.IsKeyDown(Key.Down))
                cam.AddRotation(0.0f, -camSens);

            // Обновляем позиции объектов
            time += (float)e.Time;
            objects[0].Rotation += new Vector3(0, -0.01f, 0);

            Angle += rotSpeed * (float)e.Time;
            if (Angle > MathHelper.TwoPi)
                Angle -= MathHelper.TwoPi;
        }
    }
}