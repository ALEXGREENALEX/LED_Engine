using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGL_CS_Game
{
    class Game : GameWindow
    {
        public Game() :
            base(640, 480, new GraphicsMode(32, 24, 0, 8), "OpenGL(OpenTK) C# Game")
        {
        }

        Vector3[] vertdata, normdata;
        Vector2[] texcoorddata;
        uint[] indicedata;
        Vector4[] tangdata;
        int ibo_elements;
        Camera cam = new Camera();
        Vector2 lastMousePos = new Vector2();
        Vector2 lastMousePos_Delta = new Vector2();
        float time = 0.0f;
        float rotSpeed = (float)Math.PI / 4.0f;
        float Angle = MathHelper.DegreesToRadians(100.0f);
        double FPS;

        List<Volume> objects = new List<Volume>();
        Dictionary<string, int> textures = new Dictionary<string, int>();
        Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();
        string activeShader = "NormalMap";

        void initProgram()
        {
            GL.GenBuffers(1, out ibo_elements);

            // Enable Depth Test
            GL.Enable(EnableCap.DepthTest);

            // Загружаем шейдеры
            shaders.Add("NormalMap", new ShaderProgram(Properties.Resources.VS_NormalMap, Properties.Resources.FS_NormalMap, false));

            // Подключаем шейдеры
            GL.LinkProgram(shaders[activeShader].ProgramID);
            GL.UseProgram(shaders[activeShader].ProgramID);

            // Загружаем текстуры
            GL.ActiveTexture(TextureUnit.Texture0);
            textures.Add("DiffuseMap", loadImage("brick-wall.jpg"));

            GL.ActiveTexture(TextureUnit.Texture1);
            textures.Add("NormalMap", loadImage("brick-wall_N.jpg"));

            //Отрисовка только тех сторон, что повернуты к камере нормалями.
            //GL.Enable(EnableCap.CullFace);

            /// Создаем примитивы
            Cube tc = new Cube();
            objects.Add(tc);

            // Отдаляем камеру от начала координат
            cam.Position = new Vector3(0.0f, 0.0f, 1.5f);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initProgram();

            GL.ClearColor(Color.CornflowerBlue);
            GL.PointSize(5f);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (Focused)
                if (e.Button == MouseButton.Left)
                    lastMousePos = new Vector2(OpenTK.Input.Mouse.GetCursorState().X, OpenTK.Input.Mouse.GetCursorState().Y);
                else if (e.Button == MouseButton.Middle)
                {
                    lastMousePos_Delta = new Vector2(0.0f, 0.0f);
                    objects[0].Rotation = new Vector3(0.0f, 0.0f, 0);
                }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            objects[0].Scale += new Vector3(e.Delta / 10.0f, e.Delta / 10.0f, e.Delta / 10.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // FPS
            FPS = 1.0 / e.Time;
            Title = "FPS: " + FPS.ToString("0.00");

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<uint> inds = new List<uint>();
            List<Vector2> texcoords = new List<Vector2>();
            List<Vector4> tangentses = new List<Vector4>();

            // Assemble vertex and indice data for all volumes
            int vertcount = 0;
            foreach (Volume v in objects)
            {
                verts.AddRange(v.GetVerts());
                norms.AddRange(v.GetNormals());
                inds.AddRange(v.GetIndices((uint)vertcount));
                texcoords.AddRange(v.GetTextureCoords());
                tangentses.AddRange(v.GetTangentses());
                vertcount += v.VertCount;
            }

            vertdata = verts.ToArray();
            normdata = norms.ToArray();
            indicedata = inds.ToArray();
            texcoorddata = texcoords.ToArray();
            tangdata = tangentses.ToArray();

            // Обновляем позиции объектов
            time += (float)e.Time;

            if (Focused && OpenTK.Input.Mouse.GetState().LeftButton == OpenTK.Input.ButtonState.Pressed)
                lastMousePos_Delta = lastMousePos - new Vector2(OpenTK.Input.Mouse.GetCursorState().X, OpenTK.Input.Mouse.GetCursorState().Y);
            objects[0].Rotation += new Vector3(-0.0001f * time * lastMousePos_Delta.Y, -0.0001f * time * lastMousePos_Delta.X, 0);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            int indiceat = 0;

            // Отрисовываем все объекты
            foreach (Volume v in objects)
            {
                v.CalculateModelMatrix();
                v.ViewProjectionMatrix = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 0.2f, 50.0f);
                v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewProjectionMatrix;

                if (shaders[activeShader].GetUniform("Light.Position") != -1)
                {
                    Vector4 V = new Vector4(10.0f * (float)Math.Cos(Angle), 1.0f, 10.0f * (float)Math.Sin(Angle), 1.0f);
                    Matrix4 M = cam.GetViewMatrix();
                    Vector4 LightPos = new Vector4(
                        M.M11 * V.X + M.M12 * V.Y + M.M13 * V.Z + M.M14 * V.W,
                        M.M21 * V.X + M.M22 * V.Y + M.M23 * V.Z + M.M24 * V.W,
                        M.M31 * V.X + M.M32 * V.Y + M.M33 * V.Z + M.M34 * V.W,
                        M.M41 * V.X + M.M42 * V.Y + M.M43 * V.Z + M.M44 * V.W);
                    GL.Uniform4(GL.GetUniformLocation(shaders[activeShader].ProgramID, "Light.Position"), ref LightPos);
                }

                if (shaders[activeShader].GetUniform("Light.Intensity") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[activeShader].ProgramID, "Light.Intensity"), 0.9f, 0.9f, 0.9f);

                if (shaders[activeShader].GetUniform("Material.Ks") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[activeShader].ProgramID, "Material.Ks"), 0.2f, 0.2f, 0.2f);
                if (shaders[activeShader].GetUniform("Material.Ka") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[activeShader].ProgramID, "Material.Ka"), 0.1f, 0.1f, 0.1f);
                if (shaders[activeShader].GetUniform("Material.Shininess") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[activeShader].ProgramID, "Material.Shininess"), 1.0f);

                //////////// SetMatrices //////////
                Matrix4 MV = cam.GetViewMatrix() * v.ModelMatrix;
                if (shaders[activeShader].GetUniform("ModelViewMatrix") != -1)
                    GL.UniformMatrix4(shaders[activeShader].GetUniform("ModelViewMatrix"), false, ref MV);

                if (shaders[activeShader].GetUniform("NormalMatrix") != -1)
                {
                    Matrix3 NM = new Matrix3(MV.Row0.Xyz, MV.Row1.Xyz, MV.Row2.Xyz);
                    GL.UniformMatrix3(shaders[activeShader].GetUniform("NormalMatrix"), false, ref NM);
                }

                if (shaders[activeShader].GetUniform("MVP") != -1)
                    GL.UniformMatrix4(shaders[activeShader].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

                if (shaders[activeShader].GetAttribute("VertexPosition") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("VertexPosition"));
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[activeShader].GetAttribute("VertexPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[activeShader].GetAttribute("VertexPosition"));
                }

                if (shaders[activeShader].GetAttribute("VertexNormal") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("VertexNormal"));
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(normdata.Length * Vector3.SizeInBytes), normdata, BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[activeShader].GetAttribute("VertexNormal"), 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[activeShader].GetAttribute("VertexNormal"));
                }

                // Buffer texture coordinates if shader supports it
                if (shaders[activeShader].GetAttribute("VertexTexCoord") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("VertexTexCoord"));
                    GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(texcoorddata.Length * Vector2.SizeInBytes), texcoorddata, BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[activeShader].GetAttribute("VertexTexCoord"), 2, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[activeShader].GetAttribute("VertexTexCoord"));
                }

                if (shaders[activeShader].GetAttribute("VertexTangent") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("VertexTangent"));
                    GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(tangdata.Length * Vector4.SizeInBytes), tangdata, BufferUsageHint.StaticDraw);
                    GL.BindVertexArray(0);
                }
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicedata.Length * sizeof(uint)), indicedata, BufferUsageHint.StaticDraw);
                
                GL.BindVertexArray(shaders[activeShader].GetAttribute("VertexPosition"));
                GL.DrawElements(BeginMode.Triangles, v.IndiceCount, DrawElementsType.UnsignedInt, indiceat * sizeof(uint));
                indiceat += v.IndiceCount;
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

            Angle += rotSpeed * (float)e.Time;
            if (Angle > MathHelper.TwoPi)
                Angle -= MathHelper.TwoPi;
        }

        int loadImage(Bitmap image)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            //// Allocate storage
            //GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, image.Width, image.Height);
            //// Copy data into storage
            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, image.Width, image.Height,
            //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            //Filter params
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        int loadImage(string filename)
        {
            try
            {
                Bitmap file = new Bitmap(filename);
                return loadImage(file);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Error: Cannot load image \"lilename\"\n\n" + e.Message);
                return -1;
            }
        }
    }
}