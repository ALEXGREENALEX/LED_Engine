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

        int indecesArrayBuffer;
        Camera cam = new Camera();
        Vector2 lastMousePos = new Vector2();
        Vector2 lastMousePos_Delta = new Vector2();
        float time = 0.0f;
        float rotSpeed = (float)Math.PI / 4.0f;
        float Angle = MathHelper.DegreesToRadians(290.0f);
        double FPS;

        List<Volume> objects = new List<Volume>();
        Dictionary<int, int> textures = new Dictionary<int, int>();
        Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();

        void initProgram()
        {
            GL.GenBuffers(1, out indecesArrayBuffer);

            // Enable Depth Test
            GL.Enable(EnableCap.DepthTest);

            // Загружаем шейдеры
            shaders.Add("Default", new ShaderProgram(Properties.Resources.Default_VS, Properties.Resources.Default_FS, false));
            shaders.Add("PhongNormalMap", new ShaderProgram(Properties.Resources.PhongNormalMap_VS, Properties.Resources.PhongNormalMap_FS, false));

            // Загружаем текстуры
            GL.ActiveTexture(TextureUnit.Texture0);
            textures.Add(0, loadImage("brick-wall.jpg"));

            GL.ActiveTexture(TextureUnit.Texture1);
            textures.Add(1, loadImage("brick-wall_N.jpg"));

            //Отрисовка только тех сторон, что повернуты к камере нормалями.
            //GL.Enable(EnableCap.CullFace);

            // Создаем примитивы
            //Cube cube = new Cube();
            //Plain plain = new Plain();
            //objects.Add(cube);
            //objects.Add(plain);

            // Загружаем модели
            ObjVolume obj_Triangulated = ObjVolume.LoadFromFile("Model_Triangulated.obj");
            obj_Triangulated.ShaderName = "PhongNormalMap";
            obj_Triangulated.TextureID[1] = 1;
            ObjVolume obj_Quads = ObjVolume.LoadFromFile("Model_Quads.obj");
            obj_Quads.TextureID[0] = 0;
            objects.Add(obj_Triangulated);
            objects.Add(obj_Quads);

            // Отдаляем камеру от начала координат
            cam.Position = new Vector3(0.0f, 0.0f, 0.5f);
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
                    objects[0].Rotation = new Vector3(0.0f, 0.0f, 0.0f);
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

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            // Отрисовываем все объекты
            foreach (Volume v in objects)
            {
                v.CalculateModelMatrix();
                v.ViewProjectionMatrix = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 0.2f, 50.0f);
                v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewProjectionMatrix;

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textures[v.TextureID[0]]);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, textures[v.TextureID[1]]);
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, textures[v.TextureID[2]]);
                GL.ActiveTexture(TextureUnit.Texture3);
                GL.BindTexture(TextureTarget.Texture2D, textures[v.TextureID[3]]);
                
                // Подключаем шейдеры для отрисовки объекта
                GL.LinkProgram(shaders[v.ShaderName].ProgramID);
                GL.UseProgram(shaders[v.ShaderName].ProgramID);

                // Передаем шейдеру вектор Light Position, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetUniform("Light.Position") != -1)
                {
                    Vector4 V = new Vector4(10.0f * (float)Math.Cos(Angle), 1.0f, 10.0f * (float)Math.Sin(Angle), 1.0f);
                    Matrix4 M = cam.GetViewMatrix();
                    Vector4 LightPos = new Vector4(
                        M.M11 * V.X + M.M12 * V.Y + M.M13 * V.Z + M.M14 * V.W,
                        M.M21 * V.X + M.M22 * V.Y + M.M23 * V.Z + M.M24 * V.W,
                        M.M31 * V.X + M.M32 * V.Y + M.M33 * V.Z + M.M34 * V.W,
                        M.M41 * V.X + M.M42 * V.Y + M.M43 * V.Z + M.M44 * V.W);
                    GL.Uniform4(GL.GetUniformLocation(shaders[v.ShaderName].ProgramID, "Light.Position"), ref LightPos);
                }

                // Передаем шейдеру вектор Light Intensity, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetUniform("Light.Intensity") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.ShaderName].ProgramID, "Light.Intensity"), 0.9f, 0.9f, 0.9f);

                // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetUniform("Material.Ks") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.ShaderName].ProgramID, "Material.Ks"), 0.2f, 0.2f, 0.2f);

                // Передаем шейдеру вектор Ambient reflectivity, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetUniform("Material.Ka") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.ShaderName].ProgramID, "Material.Ka"), 0.1f, 0.1f, 0.1f);

                // Передаем шейдеру значение Specular shininess factor, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetUniform("Material.Shininess") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[v.ShaderName].ProgramID, "Material.Shininess"), 1.0f);

                // Передаем шейдеру матрицу ModelView, если шейдер поддерживает это.
                Matrix4 MV = cam.GetViewMatrix() * v.ModelMatrix;
                if (shaders[v.ShaderName].GetUniform("ModelViewMatrix") != -1)
                    GL.UniformMatrix4(shaders[v.ShaderName].GetUniform("ModelViewMatrix"), false, ref MV);

                // Передаем шейдеру матрицу NormalMatrix, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetUniform("NormalMatrix") != -1)
                {
                    Matrix3 NM = new Matrix3(MV.Row0.Xyz, MV.Row1.Xyz, MV.Row2.Xyz);
                    GL.UniformMatrix3(shaders[v.ShaderName].GetUniform("NormalMatrix"), false, ref NM);
                }

                // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
                if (shaders[v.ShaderName].GetUniform("MVP") != -1)
                    GL.UniformMatrix4(shaders[v.ShaderName].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

                // Передаем шейдеру буфер позицый вертексов, если шейдер поддерживает это (должна быть 100% поддержка).
                if (shaders[v.ShaderName].GetAttribute("VertexPosition") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.ShaderName].GetBuffer("VertexPosition"));
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetVertices().Length * Vector3.SizeInBytes), v.GetVertices(), BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[v.ShaderName].GetAttribute("VertexPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[v.ShaderName].GetAttribute("VertexPosition"));
                }

                // Передаем шейдеру буфер нормалей, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetAttribute("VertexNormal") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.ShaderName].GetBuffer("VertexNormal"));
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetNormals().Length * Vector3.SizeInBytes), v.GetNormals(), BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[v.ShaderName].GetAttribute("VertexNormal"), 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[v.ShaderName].GetAttribute("VertexNormal"));
                }

                // Передаем шейдеру буфер текстурных координат, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetAttribute("VertexTexCoord") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.ShaderName].GetBuffer("VertexTexCoord"));
                    GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetTextureCoords().Length * Vector2.SizeInBytes), v.GetTextureCoords(), BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[v.ShaderName].GetAttribute("VertexTexCoord"), 2, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[v.ShaderName].GetAttribute("VertexTexCoord"));
                }

                // Передаем шейдеру буфер тангенсов, если шейдер поддерживает это.
                if (shaders[v.ShaderName].GetAttribute("VertexTangent") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.ShaderName].GetBuffer("VertexTangent"));
                    GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetTangentses().Length * Vector4.SizeInBytes), v.GetTangentses(), BufferUsageHint.StaticDraw);
                    GL.BindVertexArray(0);
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indecesArrayBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(v.GetFaceIndeces().Length * sizeof(uint)), v.GetFaceIndeces(), BufferUsageHint.StaticDraw);

                GL.BindVertexArray(shaders[v.ShaderName].GetAttribute("VertexPosition"));
                GL.DrawElements(BeginMode.Triangles, v.FacesCount, DrawElementsType.UnsignedInt, 0 * sizeof(uint));
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

            // Обновляем позиции объектов
            time += (float)e.Time;

            if (Focused && OpenTK.Input.Mouse.GetState().LeftButton == OpenTK.Input.ButtonState.Pressed)
                lastMousePos_Delta = lastMousePos - new Vector2(OpenTK.Input.Mouse.GetCursorState().X, OpenTK.Input.Mouse.GetCursorState().Y);
            objects[0].Rotation += new Vector3(-0.0001f * time * lastMousePos_Delta.Y, -0.0001f * time * lastMousePos_Delta.X, 0);

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