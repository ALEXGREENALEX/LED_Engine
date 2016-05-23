using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    partial class Game
    {
        public static Camera MainCamera = new Camera();
        public static Mesh SkyBox = new Mesh();

        public static Mesh DebugAmbientLight = new Mesh();
        public static Mesh DebugDirectionalLight = new Mesh();
        public static Mesh DebugPointLight = new Mesh();
        public static Mesh DebugSpotLight = new Mesh();

        static void ApplySettingsAndCreateWindow()
        {
            #region Apply render settings BEFORE window init
            if (Settings.Window.Borders)
                Glfw.WindowHint(WindowHint.Decorated, 1);
            else
                Glfw.WindowHint(WindowHint.Decorated, 0);

            if (Settings.Window.Fullscreen)
                Glfw.WindowHint(WindowHint.Floating, 1); // Always On Top
            else
                Glfw.WindowHint(WindowHint.Floating, 0);

            Glfw.WindowHint(WindowHint.Focused, 1); // Set Focus On Init
            Glfw.WindowHint(WindowHint.Resizeable, 1);
            Glfw.WindowHint(WindowHint.AutoIconify, 1); // Auto Mininize and Restore FullScreen Window
            #endregion

            if (Settings.Window.Fullscreen)
            {
                if (Settings.Window.FullscreenWindowed)
                {
                    #region Fullscreen Windowed
                    if (Settings.Window.Borders)
                    {
                        Settings.Window.X = Screen.PrimaryScreen.WorkingArea.X;
                        Settings.Window.Y = Screen.PrimaryScreen.WorkingArea.Y;
                        Settings.Window.Width = Screen.PrimaryScreen.WorkingArea.Width;
                        Settings.Window.Height = Screen.PrimaryScreen.WorkingArea.Height;
                    }
                    else
                    {
                        Settings.Window.X = Screen.PrimaryScreen.Bounds.X;
                        Settings.Window.Y = Screen.PrimaryScreen.Bounds.Y;
                        Settings.Window.Width = Screen.PrimaryScreen.Bounds.Width;
                        Settings.Window.Height = Screen.PrimaryScreen.Bounds.Height;
                    }

                    Window = CreateNewWindow(Settings.Window.Width, Settings.Window.Height, Settings.Window.Title,
                        GlfwMonitorPtr.Null, Window);

                    Glfw.SetWindowPos(Window, Settings.Window.X, Settings.Window.Y);
                    #endregion
                }
                else
                {
                    #region Real Fullscreen
                    Settings.Window.X = 0;
                    Settings.Window.Y = 0;
                    Settings.Window.Width = Settings.Window.OriginalWidth;
                    Settings.Window.Height = Settings.Window.OriginalHeight;

                    GlfwMonitorPtr Monitor;
                    GlfwVidMode VideoMode = new GlfwVidMode
                    {
                        RedBits = Settings.Window.RedBits,
                        GreenBits = Settings.Window.GreenBits,
                        BlueBits = Settings.Window.BlueBits,
                        Width = Settings.Window.Width,
                        Height = Settings.Window.Height,
                        RefreshRate = Settings.Window.RefreshRate
                    };

                    GlfwMonitorPtr[] MonitorsList = Glfw.GetMonitors();
                    GlfwVidMode[] VideoModes;

                    if (Settings.Window.ActiveMonitor >= 0 && MonitorsList.Length - 1 >= Settings.Window.ActiveMonitor)
                        Monitor = MonitorsList[Settings.Window.ActiveMonitor];
                    else
                        Monitor = Glfw.GetPrimaryMonitor();

                    VideoModes = Glfw.GetVideoModes(Monitor);

                    int VModeIndex = -1;
                    for (int i = 0; i < VideoModes.Length; i++)
                        if (VideoModes[i].Equals(VideoMode))
                            VModeIndex = i;

                    if (VModeIndex == -1)
                        VideoMode = Glfw.GetVideoMode(Monitor);

                    Glfw.WindowHint(WindowHint.RedBits, VideoMode.RedBits);
                    Glfw.WindowHint(WindowHint.GreenBits, VideoMode.GreenBits);
                    Glfw.WindowHint(WindowHint.BlueBits, VideoMode.BlueBits);
                    Glfw.WindowHint(WindowHint.RefreshRate, VideoMode.RefreshRate);

                    //Fullscreen refresh rate limit, work only if 'V-Sync=true'
                    Glfw.WindowHint(WindowHint.RefreshRate, Settings.Window.RefreshRateLimit);

                    Window = CreateNewWindow(Settings.Window.Width, Settings.Window.Height, Settings.Window.Title, Monitor, Window);
                    #endregion
                }
            }
            else // Fullscreen = False
            {
                #region Window Mode
                Window = CreateNewWindow(Settings.Window.Width, Settings.Window.Height, Settings.Window.Title,
                    GlfwMonitorPtr.Null, Window);

                Settings.Window.X = Settings.Window.OriginalX;
                Settings.Window.Y = Settings.Window.OriginalY;
                Settings.Window.Width = Settings.Window.OriginalWidth;
                Settings.Window.Height = Settings.Window.OriginalHeight;

                // Move Window To Center
                if (Settings.Window.CenterWindow)
                {
                    Settings.Window.X = (Screen.PrimaryScreen.WorkingArea.Width - Settings.Window.Width) / 2;
                    Settings.Window.Y = (Screen.PrimaryScreen.WorkingArea.Height - Settings.Window.Height) / 2;
                }

                Glfw.SetWindowSize(Window, Settings.Window.Width, Settings.Window.Height);
                Glfw.SetWindowPos(Window, Settings.Window.X, Settings.Window.Y);
                #endregion
            }

            Glfw.MakeContextCurrent(Window);

            #region Apply render settings AFTER window init
            Log.WriteLine("GPU: " + GL.GetString(StringName.Renderer));
            Settings.Window.IsFocused = true;
            #region Mouse jump fix
            Glfw.SetInputMode(Window, InputMode.CursorMode, CursorMode.CursorCaptured);
            Glfw.GetWindowPos(Window, out Settings.Window.X, out Settings.Window.Y);
            Glfw.SetCursorPos(Window,
                (double)(Settings.Window.X + Settings.Window.Width / 2),
                (double)(Settings.Window.Y + Settings.Window.Height / 2));
            #endregion

            Glfw.SwapInterval(Settings.Graphics.VSyncSwapInterval); //V-sync

            GL.ClearColor(Color4.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            #endregion
        }

        static GlfwWindowPtr CreateNewWindow(int Width, int Height, string Title, GlfwMonitorPtr Monitor, GlfwWindowPtr OldWindow)
        {
            GlfwWindowPtr NewWindow = Glfw.CreateWindow(Width, Height, Title, Monitor, OldWindow);
            Glfw.DestroyWindow(OldWindow);
            return NewWindow;
        }

        static void RescaleToWindowSize()
        {
            int FB_Width, FB_Height;
            Glfw.GetFramebufferSize(Window, out FB_Width, out FB_Height);

            if (FB_Width > 0 && FB_Height > 0)
            {
                FBO.Init(FB_Width, FB_Height);
                MainCamera.SetProjectionMatrix(ProjectionTypes.Perspective, FB_Width, FB_Height, MainCamera.zNear, MainCamera.zFar, MainCamera.FOV);
                GL.Viewport(0, 0, FB_Width, FB_Height);
                FPS.Font_Init();
                FPS.FPS_Font_ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, FB_Width, 0, FB_Height, -1.0f, 1.0f);
            }
        }

        static void Draw(Mesh m)
        {
            m.CalculateMatrices(MainCamera);
            FrustumCulling.ExtractFrustum(MainCamera.GetViewMatrix() * MainCamera.GetProjectionMatrix());
            float Scale = Math.Max(m.Scale.X, Math.Max(m.Scale.Y, m.Scale.Z));

            if (FrustumCulling.SphereInFrustum(m.Position, m.BoundingSphere.Outer * Scale))
                for (int i = 0; i < m.Parts.Count; i++)
                    if (FrustumCulling.SphereInFrustum(m.Position + m.Parts[i].BoundingSphere.Position, m.Parts[i].BoundingSphere.Outer * Scale))
                        Draw(m, m.Parts[i]);
        }

        static void Draw(Mesh m, MeshPart v)
        {
            if (v.Material.CullFace)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);

            int TempLocation;
            Shader shader = v.Material.Shader;
            GL.UseProgram(shader.ProgramID);

            // Активируем нужный TextureUnit и назначаем текстуру
            for (int i = 0; i < v.Material.Textures.Length; i++)
            {
                if (v.Material.Textures[i] != null)
                {
                    GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
                    GL.BindTexture(v.Material.Textures[i].TextureTarget, v.Material.Textures[i].ID);
                }
            }

            #region Работаем с шейдерами
            #region Камера и матрицы
            // Передаем шейдеру матрицу ModelMatrix, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ModelMatrix");
            if (TempLocation != -1)
                GL.UniformMatrix4(TempLocation, false, ref m.ModelMatrix);

            // Передаем шейдеру матрицу ViewMatrix, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ViewMatrix");
            if (TempLocation != -1)
            {
                Matrix4 V = MainCamera.GetViewMatrix();
                GL.UniformMatrix4(TempLocation, false, ref V);
            }

            // Передаем шейдеру матрицу ProjectionMatrix, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ProjectionMatrix");
            if (TempLocation != -1)
            {
                Matrix4 P = MainCamera.GetProjectionMatrix();
                GL.UniformMatrix4(TempLocation, false, ref P);
            }

            // Передаем шейдеру матрицу ModelView, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ModelView");
            if (TempLocation != -1)
                GL.UniformMatrix4(TempLocation, false, ref m.ModelViewMatrix);

            // Передаем шейдеру матрицу NormalMatrix, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("NormalMatrix");
            if (TempLocation != -1)
            {
                Matrix3 NormalMatrix = new Matrix3(m.ModelViewMatrix);
                NormalMatrix.Invert();
                NormalMatrix.Transpose();
                GL.UniformMatrix3(TempLocation, false, ref NormalMatrix);
            }

            // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
            TempLocation = shader.GetUniform("MVP");
            if (TempLocation != -1)
                GL.UniformMatrix4(TempLocation, false, ref m.ModelViewProjectionMatrix);

            // Передаем шейдеру позицию камеры, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("CameraPos");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, MainCamera.Position);
            #endregion

            #region Передача различных параметров шейдерам

            // Передаем шейдеру массив тех TextureUnit-ов, которые исаользуются, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("TexUnits[0]");
            if (TempLocation != -1)
            {
                int[] Arr = new int[v.Material.Textures.Length];
                for (int TUnit = 0; TUnit < v.Material.Textures.Length; TUnit++)
                    Arr[TUnit] = (v.Material.Textures[TUnit] != null ? 1 : 0);
                GL.Uniform1(TempLocation, Arr.Length, Arr);
            }

            // Передаем шейдеру вектор Diffuse reflectivity, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Kd");
            if (TempLocation != -1)
                GL.Uniform4(TempLocation, v.Material.Kd);

            // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Ks");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, v.Material.Ks);

            // Передаем шейдеру вектор Ambient reflectivity, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Ka");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, v.Material.Ka);

            // Передаем шейдеру вектор Material Emissive, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Ke");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, v.Material.Ke);

            // Передаем шейдеру значение Specular shininess factor, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.S");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, v.Material.Shininess);

            // Передаем шейдеру значение ReflectFactor, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Reflection");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, v.Material.Reflection);

            // Передаем шейдеру Parallax Scale, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ParallaxScale");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, v.Material.ParallaxScale);
            #endregion
            #endregion

            shader.EnableVertexAttribArrays();

            #region Передаем шейдеру VertexPosition, VertexNormal, VertexUV, VertexTangents
            // Передаем шейдеру буфер позицый вертексов, если шейдер поддерживает это (должна быть 100% поддержка).
            TempLocation = shader.GetAttribute("v_Position");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.VertexBufferID);
                GL.VertexAttribPointer(TempLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            }

            // Передаем шейдеру буфер нормалей, если шейдер поддерживает это.
            TempLocation = shader.GetAttribute("v_Normal");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.NormalBufferID);
                GL.VertexAttribPointer(TempLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            }

            // Передаем шейдеру буфер текстурных координат, если шейдер поддерживает это.
            TempLocation = shader.GetAttribute("v_UV");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.UVBufferID);
                GL.VertexAttribPointer(TempLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
            }

            // Передаем шейдеру буфер тангенсов, если шейдер поддерживает это.
            TempLocation = shader.GetAttribute("v_Tan");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.TangentBufferID);
                GL.VertexAttribPointer(TempLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            }
            #endregion

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, v.IndexBufferID);
            GL.DrawElements(BeginMode.Triangles, v.Indexes.Length, DrawElementsType.UnsignedInt, 0);

            // If you're using VAOs, then you should not disable attribute arrays, as they are encapsulated in the VAO.
            //shader.DisableVertexAttribArrays();
        }
    }
}
