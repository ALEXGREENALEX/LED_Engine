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
            base(800, 600, new GraphicsMode(32, 24, 0, 4), "Hello") // MSAA = 4
        {
        }

        uint indecesArrayBuffer;

        Camera cam = new Camera();
        float FOV = MathHelper.DegreesToRadians(50.0f);
        float zNear = 0.3f;
        float zFar = 10000.0f;
        Matrix4 ProjectionMatrix;

        float time = 0.0f;
        float rotSpeed = (float)Math.PI / 4.0f;
        float Angle = MathHelper.DegreesToRadians(100.0f);
        double FPS;

        bool UsePostEffects = false;

        public static List<Volume> objects = new List<Volume>();
        List<Volume> transparentObjects = new List<Volume>();
        public static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        public static Dictionary<string, Material> materials = new Dictionary<string, Material>();
        public static Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();

        void LoadConfigAndResources()
        {
            String FloatComa = (0.5f).ToString().Substring(1, 1); // Символ запятой в float ("," или ".")
            string StartupPath = Application.StartupPath;

            XmlDocument XML = new XmlDocument();
            XML.Load(Path.Combine(StartupPath, "Config.xml"));
            Title = XML.DocumentElement.SelectSingleNode("Title").InnerText;

            XmlNode NodePath = XML.DocumentElement.SelectSingleNode("Paths");
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
            string CubeMapTexturesPath = Path.Combine(GameDataPath, NodePath.SelectSingleNode("CubeMapTextures").InnerText);

            NodePath = XML.DocumentElement.SelectSingleNode("ConfigFiles");
            string TexturesConfigFile = Path.Combine(GameDataPath, NodePath.SelectSingleNode("Textures").InnerText);
            string CubeMapTexturesConfigFile = Path.Combine(GameDataPath, NodePath.SelectSingleNode("CubeMapTextures").InnerText);
            string MaterialsConfigFile = Path.Combine(GameDataPath, NodePath.SelectSingleNode("Materials").InnerText);
            string ShadersConfigFile = Path.Combine(GameDataPath, NodePath.SelectSingleNode("Shaders").InnerText);

            #region Загружаем шейдеры
            XML.Load(ShadersConfigFile);
            XmlNodeList xmlNodeList = XML.DocumentElement.SelectNodes("Shader");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                shaders.Add(xmlNode.SelectSingleNode("Name").InnerText, new ShaderProgram(
                    Path.Combine(ShadersPath, xmlNode.SelectSingleNode("VertexShader").InnerText),
                    Path.Combine(ShadersPath, xmlNode.SelectSingleNode("FragmentShader").InnerText), true));
            }
            #endregion

            #region Загружаем текстуры
            XML.Load(TexturesConfigFile);
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
                    {
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText))));
                    }
                    else if (StrMagFilter == String.Empty)
                    {
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText),
                            TextureMagFilter.Linear,
                            (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true))));
                    }
                    else if (StrMinFilter == String.Empty)
                    {
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText),
                            (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true))));
                    }
                    else
                    {
                        textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                            Path.Combine(TexturesPath, xmlNode.SelectSingleNode("File").InnerText),
                            (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true),
                            (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true))));
                    }
                }
                catch
                {
                    MessageBox.Show("Texture Loading Error!");
                }
            }
            #endregion

            #region CubeMapTextures
            XML.Load(CubeMapTexturesConfigFile);
            xmlNodeList = XML.DocumentElement.SelectNodes("CubeMapTexture");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                try
                {
                    string Name = xmlNode.SelectSingleNode("Name").InnerText;
                    string CubeMapDir = Path.Combine(CubeMapTexturesPath, xmlNode.SelectSingleNode("SubDir").InnerText);
                    XmlNodeList TMPXmlNodeList = xmlNode.SelectNodes("Texture");

                    if (TMPXmlNodeList.Count == 6)
                    {
                        Bitmap[] CubeMapsBMP = new Bitmap[TMPXmlNodeList.Count];

                        int SideSize = -1;
                        if (xmlNode.SelectNodes("SideSize").Count > 0 && xmlNode.SelectSingleNode("SideSize").InnerText.Trim() != String.Empty)
                            SideSize = int.Parse(xmlNode.SelectSingleNode("SideSize").InnerText);

                        for (int i = 0; i < TMPXmlNodeList.Count; i++)
                            CubeMapsBMP[i] = new Bitmap(Path.Combine(CubeMapDir, TMPXmlNodeList[i].InnerText));

                        if (SideSize > 0)
                            for (int i = 0; i < TMPXmlNodeList.Count; i++)
                                CubeMapsBMP[i] = new Bitmap(CubeMapsBMP[i], SideSize, SideSize);

                        textures.Add(Name, new Texture(Texture.LoadCubeMap(CubeMapsBMP), TextureTarget.TextureCubeMap));
                        CubeMapsBMP = null;
                    }
                    else
                        new Exception();
                }
                catch
                {
                    MessageBox.Show("CubeMapTexture Loading Error!");
                }
            }
            #endregion

            #region Загружаем материалы
            XML.Load(MaterialsConfigFile);
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

                    if (xmlNode.SelectNodes("Color").Count > 0)
                    {
                        StrTMP = String.Empty;
                        StrTMP = xmlNode.SelectSingleNode("Color").InnerText;
                        StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                        if (StrArrTMP.Length == 4)
                            materials[Name].Color = new Vector4(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]), float.Parse(StrArrTMP[3]));
                    }

                    int TexturesCount = xmlNode.SelectNodes("Texture").Count;
                    if (TexturesCount > 0 && TexturesCount <= 32)
                    {
                        XmlNodeList xmlNodesTMP = xmlNode.SelectNodes("Texture");
                        for (int i = 0; i < xmlNodesTMP.Count; i++)
                            materials[Name].SetTexture(i, xmlNodesTMP[i].InnerText);
                    }

                    TexturesCount = xmlNode.SelectNodes("CubemapTexture").Count;
                    if (TexturesCount > 0 && TexturesCount <= 32)
                    {
                        XmlNodeList xmlNodesTMP = xmlNode.SelectNodes("CubemapTexture");
                        for (int i = 0; i < xmlNodesTMP.Count; i++)
                            materials[Name].SetTexture(i, xmlNodesTMP[i].InnerText);
                    }

                    if (xmlNode.SelectNodes("CullFace").Count > 0)
                        materials[Name].CullFace = bool.Parse(xmlNode.SelectSingleNode("CullFace").InnerText);

                    if (xmlNode.SelectNodes("Transparent").Count > 0)
                        materials[Name].Transparent = bool.Parse(xmlNode.SelectSingleNode("Transparent").InnerText);

                    if (xmlNode.SelectNodes("SpecularReflectivity").Count > 0)
                    {
                        StrTMP = String.Empty;
                        StrTMP = xmlNode.SelectSingleNode("SpecularReflectivity").InnerText;
                        StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                        if (StrArrTMP.Length == 3)
                            materials[Name].SpecularReflectivity = new Vector3(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]));
                    }

                    if (xmlNode.SelectNodes("AmbientReflectivity").Count > 0)
                    {
                        StrTMP = String.Empty;
                        StrTMP = xmlNode.SelectSingleNode("AmbientReflectivity").InnerText;
                        StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                        if (StrArrTMP.Length == 3)
                            materials[Name].AmbientReflectivity = new Vector3(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]));
                    }

                    if (xmlNode.SelectNodes("SpecularShininess").Count > 0)
                        materials[Name].SpecularShininess = float.Parse(xmlNode.SelectSingleNode("SpecularShininess")
                            .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));

                    if (xmlNode.SelectNodes("ReflectionFactor").Count > 0)
                        materials[Name].ReflectionFactor = float.Parse(xmlNode.SelectSingleNode("ReflectionFactor")
                            .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));

                    if (xmlNode.SelectNodes("RefractiveIndex").Count > 0)
                        materials[Name].RefractiveIndex = float.Parse(xmlNode.SelectSingleNode("RefractiveIndex")
                            .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));
                }
                catch
                {
                    MessageBox.Show("Material Loading Error!");
                }
            }
            #endregion

            ObjVolume obj_Triangulated = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Model_Triangles.obj"));
            //obj_Triangulated.Material = materials["BrickWall"];
            obj_Triangulated.Material = materials["Refraction"]; // Reflection Refraction

            ObjVolume obj_Quads = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Model_Quads.obj"));
            obj_Quads.Material = materials["Reflection"];
            obj_Quads.Position.Y += 1;

            ObjVolume obj_Keypad = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Keypad.obj"));
            obj_Keypad.Material = materials["Keypad"];
            obj_Keypad.Position = new Vector3(16f, 0f, 0f);

            Fog.Enabled = true;
            int a = 10;
            Cube[,] obj_cubes = new Cube[a, a];
            for (int i1 = 0; i1 < a; i1++)
                for (int i2 = 0; i2 < a; i2++)
                {
                    obj_cubes[i1, i2] = new Cube();
                    obj_cubes[i1, i2].Material = materials["Fog_test"];
                    obj_cubes[i1, i2].Position.X = (i1 - a/2) * 4;
                    obj_cubes[i1, i2].Position.Z = (i2 - a/2) * 4;
                    objects.Add(obj_cubes[i1, i2]);
                }

            ObjVolume obj_Teapot = ObjVolume.LoadFromFile(Path.Combine(MeshesPath, "Teapot.obj"));
            obj_Teapot.Material = materials["TransparentRedGlass"];
            obj_Teapot.Position.Z += 2;
            objects.Add(obj_Teapot);

            objects.Add(obj_Triangulated);
            objects.Add(obj_Quads);
            objects.Add(obj_Keypad);
        }

        void initProgram()
        {
            // Загружаем конфигурацию и ресурсы
            LoadConfigAndResources();

            GL.GenBuffers(1, out indecesArrayBuffer);

            // PostProcess Init - Create back-buffer, used for post-processing
            if (UsePostEffects)
                PostProcess.Init("PostProcess", Width, Height);

            // Включаем тест глубины
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // Функция смешивания цветов для прозрачных материалов
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.One);

            // Создаем примитивы
            Cube cube = new Cube(zFar);
            cube.Material = materials["SkyCubemap_Storforsen"];
            objects.Add(cube);

            //Plain plain = new Plain();
            //objects.Add(plain);

            // Отдаляем камеру от начала координат
            //cam.Position = new Vector3(0.0f, 0.0f, 1.5f);
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

            GL.Viewport(0, 0, Width, Height);

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

        void DrawObject(Volume v)
        {
            // Активируем нужный TextureUnit и назначаем текстуру
            if (v.Material.TexturesCount > 0)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(textures[v.Material.GetTexture(0)].TextureTarget, textures[v.Material.GetTexture(0)].TextureID);
                for (int i = 1; i < v.Material.TexturesCount; i++)
                {
                    GL.ActiveTexture((TextureUnit)(0x84C0 + i));
                    GL.BindTexture(textures[v.Material.GetTexture(i)].TextureTarget, textures[v.Material.GetTexture(i)].TextureID);
                }
            }

            #region Работаем с шейдерами
            // Подключаем шейдеры для отрисовки объекта
            GL.LinkProgram(shaders[v.Material.ShaderName].ProgramID);
            GL.UseProgram(shaders[v.Material.ShaderName].ProgramID);

            // Передаем шейдеру вектор Light Position, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("Light.Position") != -1)
            {
                Vector4 L = new Vector4(10.0f * (float)Math.Cos(Angle), 1.0f, 10.0f * (float)Math.Sin(Angle), 1.0f);
                Matrix4 V = cam.GetViewMatrix();
                GL.Uniform4(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Light.Position"), V.Mult(L));
            }

            // Передаем шейдеру вектор Light Intensity, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("Light.Intensity") != -1)
                GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Light.Intensity"), 0.9f, 0.9f, 0.9f);

            // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("Material.Kd") != -1)
                GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Kd"), new Vector3(0.9f, 0.5f, 0.3f));

            // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("Material.Ks") != -1)
                GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Ks"), v.Material.SpecularReflectivity);

            // Передаем шейдеру вектор Ambient reflectivity, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("Material.Ka") != -1)
                GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Ka"), v.Material.AmbientReflectivity);

            // Передаем шейдеру значение Specular shininess factor, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("Material.Shininess") != -1)
                GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Material.Shininess"), v.Material.SpecularShininess);

            // Передаем шейдеру значение цвета материала, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("MaterialColor") != -1)
                GL.Uniform4(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "MaterialColor"), v.Material.Color);

            // Передаем шейдеру значение ReflectFactor, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("ReflectionFactor") != -1)
                GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "ReflectionFactor"), v.Material.ReflectionFactor);

            // Передаем шейдеру значение RefractiveIndex, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("RefractiveIndex") != -1)
                GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "RefractiveIndex"), v.Material.RefractiveIndex);

            // Передаем шейдеру матрицу ModelMatrix, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("ModelMatrix") != -1)
                GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("ModelMatrix"), false, ref v.ModelMatrix);

            // Передаем шейдеру матрицу ModelViewMatrix, если шейдер поддерживает это.
            Matrix4 MV = cam.GetViewMatrix() * v.ModelMatrix;
            if (shaders[v.Material.ShaderName].GetUniform("ModelViewMatrix") != -1)
                GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("ModelViewMatrix"), false, ref MV);

            //// Передаем шейдеру матрицу ProjectionMatrix, если шейдер поддерживает это.
            //if (shaders[v.Material.ShaderName].GetUniform("ProjectionMatrix") != -1)
            //    GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("ProjectionMatrix"), false, ref ProjectionMatrix);

            // Передаем шейдеру матрицу NormalMatrix, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("NormalMatrix") != -1)
            {
                Matrix3 NM = new Matrix3(MV.Row0.Xyz, MV.Row1.Xyz, MV.Row2.Xyz);
                GL.UniformMatrix3(shaders[v.Material.ShaderName].GetUniform("NormalMatrix"), false, ref NM);
            }

            // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
            if (shaders[v.Material.ShaderName].GetUniform("MVP") != -1)
                GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

            // Передаем шейдеру позицию камеры, если шейдер поддерживает это.
            if (shaders[v.Material.ShaderName].GetUniform("WorldCameraPosition") != -1)
                GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "WorldCameraPosition"), cam.Position);

            /////////ТУМАН/////////////////////////
            if (Fog.Enabled)
            {
                if (shaders[v.Material.ShaderName].GetUniform("FogEnabled") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "FogEnabled"), Convert.ToInt32(Fog.Enabled));
                if (shaders[v.Material.ShaderName].GetUniform("Fog.MaxDist") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Fog.MaxDist"), Fog.MaxDistance);
                if (shaders[v.Material.ShaderName].GetUniform("Fog.MinDist") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Fog.MinDist"), Fog.MinDistance);
                if (shaders[v.Material.ShaderName].GetUniform("Fog.Color") != -1)
                    GL.Uniform3(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "Fog.Color"), Fog.Color);
            }
            else
                if (shaders[v.Material.ShaderName].GetUniform("FogEnabled") != -1)
                    GL.Uniform1(GL.GetUniformLocation(shaders[v.Material.ShaderName].ProgramID, "FogEnabled"), Convert.ToInt32(Fog.Enabled));

            #region Передаем шейдеру VertexPosition, VertexNormal, VertexTexCoord, VertexTangent
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
            #endregion
            #endregion

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indecesArrayBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(v.GetFaceIndeces().Length * sizeof(uint)), v.GetFaceIndeces(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(shaders[v.Material.ShaderName].GetAttribute("VertexPosition"));
            GL.DrawElements(BeginMode.Triangles, v.FacesCount, DrawElementsType.UnsignedInt, 0 * sizeof(uint));
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