using System;
using System.Collections.Generic;
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
    class Game : GameWindow
    {
        public Game() :
            base(640, 480, new GraphicsMode(32, 24, 0, 4))
        {
        }

        int indecesArrayBuffer;
        Camera cam = new Camera();
        float FOV = MathHelper.DegreesToRadians(70.0f);
        Vector2 lastMousePos = new Vector2();
        Vector2 lastMousePos_Delta = new Vector2();
        float time = 0.0f;
        float rotSpeed = (float)Math.PI / 4.0f;
        float Angle = MathHelper.DegreesToRadians(100.0f);
        double FPS;

        List<Volume> objects = new List<Volume>();
        Dictionary<string, int> textures = new Dictionary<string, int>();
        Dictionary<string, Material> materials = new Dictionary<string, Material>();
        Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();

        void LoadConfigAndResources()
        {
            String FloatComa = (0.5f).ToString().Substring(1, 1); // Символ запятой в float ("," или ".")
            string StartupPath = Application.StartupPath;

            XmlDocument XML = new XmlDocument();
            XML.Load(Path.Combine(StartupPath, "config.xml"));
            Title = XML.DocumentElement.SelectSingleNode("Title").InnerText;

            XmlNode NodePath = XML.DocumentElement.SelectSingleNode("Path");
            string GameDataPath = Path.Combine(StartupPath, NodePath.SelectSingleNode("GameData").InnerText);

            // Проверка на запуск из студии!!!
            if (!Directory.Exists(GameDataPath))
            {
                StartupPath = Path.GetDirectoryName(Path.GetDirectoryName(StartupPath));
                GameDataPath = Path.Combine(StartupPath, NodePath.SelectSingleNode("GameData").InnerText);
            }

            // Находим пути к ресурсам
            string MeshesPath = Path.Combine(GameDataPath, NodePath.SelectSingleNode("Meshes").InnerText);
            string ShadersPath = Path.Combine(GameDataPath, NodePath.SelectSingleNode("Shaders").InnerText);
            string TexturesPath = Path.Combine(GameDataPath, NodePath.SelectSingleNode("Textures").InnerText);

            #region Загружаем шейдеры
            XML.Load(Path.Combine(GameDataPath, "shaders.xml"));
            XmlNodeList xmlNodeList = XML.DocumentElement.SelectNodes("Shader");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                shaders.Add(xmlNode.SelectSingleNode("Name").InnerText, new ShaderProgram(
                    Path.Combine(ShadersPath, xmlNode.SelectSingleNode("VertexShader").InnerText),
                    Path.Combine(ShadersPath, xmlNode.SelectSingleNode("FragmentShader").InnerText), true));
            }
            #endregion

            #region Загружаем текстуры
            XML.Load(Path.Combine(GameDataPath, "textures.xml"));
            xmlNodeList = XML.DocumentElement.SelectNodes("Texture");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                try
                {
                    string StrMagFilter = String.Empty;
                    string StrMinFilter = String.Empty;

                    if (xmlNode.SelectNodes("TextureMagFilter").Count > 0)
                        StrMagFilter = xmlNode.SelectSingleNode("TextureMagFilter").InnerText;
                    if (xmlNode.SelectNodes("TextureMinFilter").Count > 0)
                        StrMinFilter = xmlNode.SelectSingleNode("TextureMinFilter").InnerText;

                    // Исли не указаны текстурные фильтры, тогда
                    if (StrMagFilter == String.Empty && StrMinFilter == String.Empty)
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, loadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText)));
                    else if (StrMagFilter == String.Empty)
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, loadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText),
                            TextureMagFilter.Linear,
                            (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true)));
                    else if (StrMinFilter == String.Empty)
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, loadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText),
                            (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true)));
                    else
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, loadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText),
                            (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true),
                            (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true)));
                }
                catch
                {
                    MessageBox.Show("Texture Loading Error!");
                }
            }
            #endregion

            #region Загружаем материалы
            XML.Load(Path.Combine(GameDataPath, "materials.xml"));
            xmlNodeList = XML.DocumentElement.SelectNodes("Material");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                try
                {
                    string[] StrArrTMP;
                    string StrTMP;

                    string Name = xmlNode.SelectSingleNode("Name").InnerText;
                    materials.Add(Name, new Material());

                    if (xmlNode.SelectNodes("Shader").Count > 0)
                        materials[Name].ShaderName = xmlNode.SelectSingleNode("Shader").InnerText;

                    int TexturesCount = xmlNode.SelectNodes("Texture").Count;
                    if (TexturesCount > 0 && TexturesCount <= 32)
                    {
                        XmlNodeList xmlNodesTMP = xmlNode.SelectNodes("Texture");
                        for (int i = 0; i < xmlNodesTMP.Count; i++)
                            materials[Name].SetTexture(i, xmlNodesTMP[i].InnerText);
                    }

                    if (xmlNode.SelectNodes("CullFace").Count > 0)
                        materials[Name].CullFace = bool.Parse(xmlNode.SelectSingleNode("CullFace").InnerText);

                    StrTMP = String.Empty;
                    if (xmlNode.SelectNodes("SpecularReflectivity").Count > 0)
                        StrTMP = xmlNode.SelectSingleNode("SpecularReflectivity").InnerText;
                    StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                    if (StrArrTMP.Length == 3)
                        materials[Name].SpecularReflectivity = new Vector3(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]));

                    StrTMP = String.Empty;
                    if (xmlNode.SelectNodes("AmbientReflectivity").Count > 0)
                        StrTMP = xmlNode.SelectSingleNode("AmbientReflectivity").InnerText;
                    StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                    if (StrArrTMP.Length == 3)
                        materials[Name].AmbientReflectivity = new Vector3(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]));

                    if (xmlNode.SelectNodes("SpecularShininess").Count > 0)
                        materials[Name].SpecularShininess = float.Parse(xmlNode.SelectSingleNode("SpecularShininess")
                            .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));
                }
                catch
                {
                    MessageBox.Show("Material Loading Error!");
                }
            }
            #endregion

            ObjVolume obj_Triangulated = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Model_Triangulated.obj"));
            obj_Triangulated.Material = materials["BrickWall"];

            ObjVolume obj_Quads = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Model_Quads.obj"));
            obj_Quads.Material = materials["BrickWall"];

            ObjVolume obj_Keypad = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Keypad.obj"));
            obj_Keypad.Material = materials["Keypad"];
            obj_Keypad.Position.Z = -10;

            objects.Add(obj_Triangulated);
            objects.Add(obj_Quads);
            objects.Add(obj_Keypad);
        }

        void initProgram()
        {
            // Загружаем конфигурацию и ресурсы
            LoadConfigAndResources();

            GL.GenBuffers(1, out indecesArrayBuffer);

            // Enable Depth Test
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            // Создаем примитивы
            //Cube cube = new Cube();
            //objects.Add(cube);

            //Plain plain = new Plain();
            //objects.Add(plain);

            // Отдаляем камеру от начала координат
            cam.Position = new Vector3(0.0f, 0.0f, 1.5f);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initProgram();
            GL.ClearColor(Color.CornflowerBlue);
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
                v.ViewProjectionMatrix = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(FOV, ClientSize.Width / (float)ClientSize.Height, 0.2f, 100.0f);
                v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewProjectionMatrix;

                // Отрисовка только тех сторон, что повернуты к камере
                if (v.Material.CullFace)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);

                // Активируем нужный TextureUnit и назначаем текстуру
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textures[v.Material.GetTexture(0)]);
                for (int i = 1; i < v.Material.TexturesCount; i++)
                {
                    GL.ActiveTexture((TextureUnit)(0x84C0 + i));
                    GL.BindTexture(TextureTarget.Texture2D, textures[v.Material.GetTexture(i)]);
                }

                // Подключаем шейдеры для отрисовки объекта
                GL.LinkProgram(shaders[v.Material.ShaderName].ProgramID);
                GL.UseProgram(shaders[v.Material.ShaderName].ProgramID);

                // Передаем шейдеру вектор Light Position, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetUniform("Light.Position") != -1)
                {
                    Vector4 V = new Vector4(10.0f * (float)Math.Cos(Angle), 1.0f, 10.0f * (float)Math.Sin(Angle), 1.0f);
                    Matrix4 M = cam.GetViewMatrix();
                    Vector4 LightPos = new Vector4(
                        M.M11 * V.X + M.M12 * V.Y + M.M13 * V.Z + M.M14 * V.W,
                        M.M21 * V.X + M.M22 * V.Y + M.M23 * V.Z + M.M24 * V.W,
                        M.M31 * V.X + M.M32 * V.Y + M.M33 * V.Z + M.M34 * V.W,
                        M.M41 * V.X + M.M42 * V.Y + M.M43 * V.Z + M.M44 * V.W);
                    GL.Uniform4(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Light.Position"), ref LightPos);
                }

                // Передаем шейдеру вектор Light Intensity, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetUniform("Light.Intensity") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Light.Intensity"), 0.9f, 0.9f, 0.9f);

                // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetUniform("Material.Ks") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Ks"), v.Material.SpecularReflectivity);

                // Передаем шейдеру вектор Ambient reflectivity, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetUniform("Material.Ka") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Ka"), v.Material.AmbientReflectivity);

                // Передаем шейдеру значение Specular shininess factor, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetUniform("Material.Shininess") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Shininess"), v.Material.SpecularShininess);

                // Передаем шейдеру матрицу ModelView, если шейдер поддерживает это.
                Matrix4 MV = cam.GetViewMatrix() * v.ModelMatrix;
                if (shaders[v.Material.ShaderName].GetUniform("ModelViewMatrix") != -1)
                    GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("ModelViewMatrix"), false, ref MV);

                // Передаем шейдеру матрицу NormalMatrix, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetUniform("NormalMatrix") != -1)
                {
                    Matrix3 NM = new Matrix3(MV.Row0.Xyz, MV.Row1.Xyz, MV.Row2.Xyz);
                    GL.UniformMatrix3(shaders[v.Material.ShaderName].GetUniform("NormalMatrix"), false, ref NM);
                }

                // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
                if (shaders[v.Material.ShaderName].GetUniform("MVP") != -1)
                    GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

                // Передаем шейдеру буфер позицый вертексов, если шейдер поддерживает это (должна быть 100% поддержка).
                if (shaders[v.Material.ShaderName].GetAttribute("VertexPosition") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.Material.ShaderName].GetBuffer("VertexPosition"));
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetVertices().Length * Vector3.SizeInBytes), v.GetVertices(), BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[v.Material.ShaderName].GetAttribute("VertexPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[v.Material.ShaderName].GetAttribute("VertexPosition"));
                }

                // Передаем шейдеру буфер нормалей, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetAttribute("VertexNormal") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.Material.ShaderName].GetBuffer("VertexNormal"));
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetNormals().Length * Vector3.SizeInBytes), v.GetNormals(), BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[v.Material.ShaderName].GetAttribute("VertexNormal"), 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[v.Material.ShaderName].GetAttribute("VertexNormal"));
                }

                // Передаем шейдеру буфер текстурных координат, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetAttribute("VertexTexCoord") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.Material.ShaderName].GetBuffer("VertexTexCoord"));
                    GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetTextureCoords().Length * Vector2.SizeInBytes), v.GetTextureCoords(), BufferUsageHint.StaticDraw);
                    GL.VertexAttribPointer(shaders[v.Material.ShaderName].GetAttribute("VertexTexCoord"), 2, VertexAttribPointerType.Float, false, 0, 0);
                    GL.EnableVertexAttribArray(shaders[v.Material.ShaderName].GetAttribute("VertexTexCoord"));
                }

                // Передаем шейдеру буфер тангенсов, если шейдер поддерживает это.
                if (shaders[v.Material.ShaderName].GetAttribute("VertexTangent") != -1)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[v.Material.ShaderName].GetBuffer("VertexTangent"));
                    GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetTangentses().Length * Vector4.SizeInBytes), v.GetTangentses(), BufferUsageHint.StaticDraw);
                    GL.BindVertexArray(0);
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indecesArrayBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(v.GetFaceIndeces().Length * sizeof(uint)), v.GetFaceIndeces(), BufferUsageHint.StaticDraw);

                GL.BindVertexArray(shaders[v.Material.ShaderName].GetAttribute("VertexPosition"));
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

        int loadImage(Bitmap image, TextureMagFilter MagFilter = TextureMagFilter.Linear, TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear)
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        int loadImage(string filename, TextureMagFilter MagFilter = TextureMagFilter.Linear, TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear)
        {
            try
            {
                Bitmap file = new Bitmap(filename);
                return loadImage(file, MagFilter, MinFilter);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Error: Cannot load image \"lilename\"\n\n" + e.Message);
                return -1;
            }
        }
    }
}