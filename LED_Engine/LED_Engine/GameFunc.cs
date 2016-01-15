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
        public static Mesh SkyCube = null;
        public static float FOV = 50.0f;
        public static float zNear = 0.1f;
        public static float zFar = 10000.0f;

        static float Angle = MathHelper.DegreesToRadians(100.0f);

        public static List<Mesh> Objects = new List<Mesh>();
        public static List<Mesh> DebugObjects = new List<Mesh>();
        static List<Mesh> TransparentObjects = new List<Mesh>();
        static List<Mesh> DrawableObjects = new List<Mesh>();

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
            Glfw.WindowHint(WindowHint.Samples, Settings.Graphics.MSAASamples); //MSAA
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
            Settings.Window.IsFocused = true;
            #region Mouse jump fix
            Glfw.SetInputMode(Window, InputMode.CursorMode, CursorMode.CursorCaptured);
            Glfw.GetWindowPos(Window, out Settings.Window.X, out Settings.Window.Y);
            Glfw.SetCursorPos(Window,
                (double)(Settings.Window.X + Settings.Window.Width / 2),
                (double)(Settings.Window.Y + Settings.Window.Height / 2));
            #endregion

            Glfw.SwapInterval(Settings.Graphics.VSyncSwapInterval); //V-sync

            if (Settings.Graphics.MSAASamples > 0) // MSAA
                GL.Enable(EnableCap.Multisample);
            else
                GL.Disable(EnableCap.Multisample);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color4.CornflowerBlue);
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

            // Настраиваем проекцию камеры
            MainCamera.SetProjectionMatrix(ProjectionTypes.Perspective, (float)FB_Width, (float)FB_Height, zNear, zFar, FOV);

            GL.Viewport(0, 0, FB_Width, FB_Height);

            if (PostProcess.UsePostEffects)
                PostProcess.Rescale(FB_Width, FB_Height);
        }

        static void LoadResourcesFromMap_FAKE()
        {
            if (Settings.Debug.Enabled)
            {
                Mesh DebugPointLight = Mesh.LoadFromFile("DebugPointLight", Engine.CombinePaths(Settings.Paths.EngineMeshes, "PointLight.obj"));

                Mesh[] DebugLight = new Mesh[2];
                for (int i = 0; i < DebugLight.Length; i++)
                {
                    DebugLight[i] = new Mesh(DebugPointLight);
                    DebugLight[i].Materials.Add(Materials.Load("DebugPointLight"));
                }
                DebugObjects.AddRange(DebugLight);
            }

            Mesh plain = Mesh.MakePlain(100f);
            plain.Materials.Add(Materials.Load("Light"));
            plain.Position.Y = -0.5f;
            Objects.Add(plain);

            Mesh box = Mesh.MakeBox();
            box.Materials.Add(Materials.Load("Light"));
            box.Position.X = -2f;
            Objects.Add(box);

            Mesh Pipe_X_1 = Mesh.LoadFromFile("Pipe_X_1", Engine.CombinePaths(Settings.Paths.Meshes, "Pipe_X.obj"));
            //Pipe_X_1.Material = Materials.Load("BrickWall");
            Pipe_X_1.Materials.Add(Materials.Load("Refraction")); // Reflection Refraction
            Pipe_X_1.Position.Y += 1;
            Objects.Add(Pipe_X_1);

            Mesh Pipe_X_2 = new Mesh(Pipe_X_1);
            Pipe_X_2.GenBuffers();
            Pipe_X_2.BindBuffers();
            Pipe_X_2.Materials.Add(Materials.Load("Reflection"));
            Pipe_X_2.Position.Y += 1;
            Objects.Add(Pipe_X_2);

            Mesh obj_Keypad = Mesh.LoadFromFile("obj_Keypad", Engine.CombinePaths(Settings.Paths.Meshes, "Keypad.obj"));
            obj_Keypad.Materials.Add(Materials.Load("Keypad"));
            obj_Keypad.Position = new Vector3(1.6f, 0f, 0f);
            obj_Keypad.Scale = new Vector3(10f, 10f, 10f);
            Objects.Add(obj_Keypad);

            int a = 33;
            Model model1 = new Model();
            for (int i1 = 0; i1 < a; i1++)
                for (int i2 = 0; i2 < a; i2++)
                {
                    model1.Meshes.Add(new Mesh(obj_Keypad));
                    model1.Meshes[i1 * a + i2].GenBuffers();
                    model1.Meshes[i1 * a + i2].BindBuffers();
                    model1.Meshes[i1 * a + i2].Materials.Add(Materials.Load("Light")); //ReliefParallaxTest //Keypad
                    model1.Meshes[i1 * a + i2].Scale = new Vector3(10.0f, 10.0f, 10.0f);
                    model1.Meshes[i1 * a + i2].Position.X = (i1 - a / 2) * 5;
                    model1.Meshes[i1 * a + i2].Position.Z = (i2 - a / 2) * 5;
                    Objects.Add(model1.Meshes[i1 * a + i2]);
                }

            Mesh obj_Teapot = Mesh.LoadFromFile("obj_Teapot", Engine.CombinePaths(Settings.Paths.EngineMeshes, "Teapot.obj"));
            obj_Teapot.Materials.Add(Materials.Load("TransparentRedGlass"));
            obj_Teapot.Position.Z += 2;
            obj_Teapot.Scale = new Vector3(3f, 3f, 3f);
            Objects.Add(obj_Teapot);

            Mesh obj_Teapot2 = new Mesh(obj_Teapot);
            obj_Teapot2.Materials.Add(Materials.Load("BrickWall"));
            obj_Teapot2.Position.Z -= 2;
            obj_Teapot2.Scale = new Vector3(3f, 3f, 3f);
            Objects.Add(obj_Teapot2);

            Mesh obj_Teapot3 = new Mesh(obj_Teapot);
            obj_Teapot3.Materials.Add(Materials.Load("ReliefParallaxTest"));
            obj_Teapot3.Position.Z -= 3;
            obj_Teapot3.Scale = new Vector3(3f, 3f, 3f);
            Objects.Add(obj_Teapot3);
        }

        static void InitProgram()
        {
            // FOG
            Fog.Enabled = true;

            SkyCube = Mesh.MakeBox(zFar, true);
            SkyCube.Materials.Add(Materials.Load("SkyCubemap_Storforsen"));
            Objects.Add(SkyCube);

            MainCamera.MoveSpeed = 0.2f;
            MainCamera.Position = new Vector3(0.0f, 2.0f, 0.0f);
        }

        static void DrawObject(Mesh v, DrawMode DrawMode = DrawMode.Default)
        {
            // Активируем нужный TextureUnit и назначаем текстуру
            for (int i = 0; i < v.Materials[0].Textures.Length; i++)
            {
                if (v.Materials[0].Textures[i] != null)
                {
                    GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
                    GL.BindTexture(v.Materials[0].Textures[i].TextureTarget, v.Materials[0].Textures[i].ID);
                }
            }

            #region Работаем с шейдерами
            Shader shader = null;
            int TempLocation;

            switch (DrawMode)
            {
                case DrawMode.Default:
                    shader = v.Materials[0].Shader;
                    break;
                case DrawMode.DrawNormals:
                    shader = Shaders.GetShader("NormalVisualizer");
                    break;
                case DrawMode.WordNormal:
                    shader = Shaders.GetShader("WorldNormal");
                    break;
            }

            GL.UseProgram(shader.ProgramID);

            #region Камера и матрицы
            // Передаем шейдеру матрицу ModelMatrix, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ModelMatrix");
            if (TempLocation != -1)
                GL.UniformMatrix4(TempLocation, false, ref v.ModelMatrix);

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
                GL.UniformMatrix4(TempLocation, false, ref v.ModelViewMatrix);

            // Передаем шейдеру матрицу NormalMatrix, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("NormalMatrix");
            if (TempLocation != -1)
            {
                Matrix3 NormalMatrix = new Matrix3(v.ModelViewMatrix);
                NormalMatrix.Invert();
                NormalMatrix.Transpose();
                GL.UniformMatrix3(TempLocation, false, ref NormalMatrix);
            }

            // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
            TempLocation = shader.GetUniform("MVP");
            if (TempLocation != -1)
                GL.UniformMatrix4(TempLocation, false, ref v.ModelViewProjectionMatrix);

            // Передаем шейдеру позицию камеры, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("CameraPos");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, MainCamera.Position);
            #endregion

            #region Передача различных параметров шейдерам

            Vector3[] LightPosition = new Vector3[2];
            LightPosition[0] = new Vector3(5.5f * (float)Math.Cos(Angle), 2.0f + (float)Math.Sin(MathHelper.TwoPi + Angle), 5.5f * (float)Math.Sin(Angle));
            LightPosition[1] = new Vector3(10.5f * (float)Math.Sin(Angle), 2.0f + (float)Math.Cos(MathHelper.TwoPi + Angle), 3.5f * (float)Math.Cos(Angle));
            DebugObjects[0].Position = LightPosition[0];
            DebugObjects[1].Position = LightPosition[1];

            // Передаем шейдеру вектор Light Position (Eye coords), если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Light[0].Ld");
            if (TempLocation != -1)
            {
                for (int LightIndex = 0; LightIndex < 2; LightIndex++)
                {
                    string LightIndexStr = LightIndex.ToString();

                    LightPosition[LightIndex] = (new Vector4(LightPosition[LightIndex], 1.0f) * MainCamera.GetViewMatrix()).Xyz;

                    // Attenuation = LightIntensity / (Constant + Linear * Distance + Quadric * Distance^2)
                    const float k_Constant = 0.0f;
                    const float k_Linear = 0.5f;
                    const float k_Quadric = 0.01f;
                    GL.Uniform3(shader.GetUniform("Light[" + LightIndexStr + "].Att"), k_Constant, k_Linear, k_Quadric);
                    GL.Uniform3(shader.GetUniform("Light[" + LightIndexStr + "].Ld"), new Vector3(1.0f, 1.0f, 1.0f) * 2.0f);
                    GL.Uniform3(shader.GetUniform("Light[" + LightIndexStr + "].Ls"), 1.0f, 1.0f, 1.0f);
                    GL.Uniform3(shader.GetUniform("Light[" + LightIndexStr + "].Pos"), LightPosition[LightIndex]);
                }

                GL.Uniform3(shader.GetUniform("LightAmbient"), 0.0f, 0.0f, 0.0f);
            }

            // Передаем шейдеру массив тех TextureUnit-ов, которые исаользуются, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("TexUnits[0]");
            if (TempLocation != -1)
            {
                int[] Arr = new int[v.Materials[0].Textures.Length];
                for (int TUnit = 0; TUnit < v.Materials[0].Textures.Length; TUnit++)
                    Arr[TUnit] = (v.Materials[0].Textures[TUnit] != null ? 1 : 0);
                GL.Uniform1(TempLocation, Arr.Length, Arr);
            }

            // Передаем шейдеру вектор Diffuse reflectivity, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Kd");
            if (TempLocation != -1)
                GL.Uniform4(TempLocation, v.Materials[0].Kd);

            // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Ks");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, v.Materials[0].Ks);

            // Передаем шейдеру вектор Ambient reflectivity, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Ka");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, v.Materials[0].Ka);

            // Передаем шейдеру вектор Material Emissive, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.Ke");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, v.Materials[0].Ke);

            // Передаем шейдеру значение Specular shininess factor, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("Material.S");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, v.Materials[0].Shininess);

            // Передаем шейдеру значение ReflectFactor, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("ReflectFactor");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, v.Materials[0].ReflectionFactor);

            // Передаем шейдеру значение RefractiveIndex, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("RefractIndex");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, v.Materials[0].RefractiveIndex);

            // Передаем шейдеру вектор Material Scale Bias Shininess, если шейдер поддерживает это. (0.07, 0, 38 for Metal; 0.04, 0, 92 for Rock)
            TempLocation = shader.GetUniform("ScaleBiasShin");
            if (TempLocation != -1)
                GL.Uniform3(TempLocation, 0.04f, 0.0f, 92.0f);
            #endregion

            #region Туман
            TempLocation = shader.GetUniform("FogEnabled");
            if (TempLocation != -1)
                GL.Uniform1(TempLocation, Convert.ToInt32(Fog.Enabled && Settings.Graphics.FogEnabled));

            if (Settings.Graphics.FogEnabled && Fog.Enabled)
            {
                TempLocation = shader.GetUniform("FogMaxDist");
                if (TempLocation != -1)
                    GL.Uniform1(TempLocation, Fog.MaxDistance);

                TempLocation = shader.GetUniform("FogMinDist");
                if (TempLocation != -1)
                    GL.Uniform1(TempLocation, Fog.MinDistance);

                TempLocation = shader.GetUniform("FogColor");
                if (TempLocation != -1)
                    GL.Uniform3(TempLocation, Fog.Color);
            }
            #endregion
            #endregion

            shader.EnableVertexAttribArrays();

            #region Передаем шейдеру VertexPosition, VertexNormal, VertexUV, VertexTangents, VertexBiTangents
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
            GL.DrawElements(BeginMode.Triangles, v.IndexesCount, DrawElementsType.UnsignedInt, 0);

            // If you're using VAOs, then you should not disable attribute arrays, as they are encapsulated in the VAO.
            //shader.DisableVertexAttribArrays();
        }
    }
}
