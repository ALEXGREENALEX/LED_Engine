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
        bool Debug = false;
        bool ShowFPS = false;
        bool UsePostEffects = true;
        Stopwatch StopWatch = new Stopwatch(); // Таймер для подсчета времени выполнения алгоритмов

        uint IndexBufferId;
        int ActiveShader;

        public static Camera MainCamera = new Camera();
        public static float FOV = 70.0f;
        public static float zNear = 0.3f;
        public static float zFar = 1000000.0f;

        Vector2 LastMousePos;

        float time = 0.0f;
        float rotSpeed = (float)Math.PI / 4.0f;
        float Angle = MathHelper.DegreesToRadians(100.0f);
        double FPS;

        public static List<Volume> Objects = new List<Volume>();
        public static List<Volume> DebugObjects = new List<Volume>();
        public static List<Prefab> Prefabs = new List<Prefab>();
        List<Volume> TransparentObjects = new List<Volume>();
        List<Volume> DrawableObjects = new List<Volume>();
        public static Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        public static Dictionary<string, Material> Materials = new Dictionary<string, Material>();
        public static Dictionary<string, ShaderProgram> Shaders = new Dictionary<string, ShaderProgram>();

        void LoadConfigAndResources()
        {
            String FloatComa = (0.5f).ToString().Substring(1, 1); // Символ запятой в float ("," или ".")
            string StartupPath = Application.StartupPath;

            XmlDocument XML = new XmlDocument();
            XML.Load(Path.Combine(StartupPath, "Config.xml"));
            Title = XML.DocumentElement.SelectSingleNode("Title").InnerText;
            Debug = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("Debug").InnerText);
            ShowFPS = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("ShowFPS").InnerText);
            UsePostEffects = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("UsePostEffects").InnerText);

            #region Определяем пути
            XmlNode NodePath = XML.DocumentElement.SelectSingleNode("Paths");
            string GameDataPath = Path.Combine(StartupPath, NodePath.SelectSingleNode("GameData").InnerText);

            // Проверка на запуск из студии!!!
            if (!Directory.Exists(GameDataPath))
            {
                StartupPath = Path.GetDirectoryName(Path.GetDirectoryName(StartupPath));
                GameDataPath = Path.Combine(StartupPath, NodePath.SelectSingleNode("GameData").InnerText);
            }

            // Находим пути к ресурсам
            string EngineContentPath = Path.Combine(GameDataPath, NodePath.SelectSingleNode("EngineContent").InnerText);

            string[] MeshesPaths = new string[]
            {
                Path.Combine(EngineContentPath, NodePath.SelectSingleNode("EngineMeshes").InnerText),
                Path.Combine(GameDataPath, NodePath.SelectSingleNode("Meshes").InnerText)
            };

            string[] ShadersPaths = new string[]
            {
                Path.Combine(EngineContentPath, NodePath.SelectSingleNode("EngineShaders").InnerText),
                Path.Combine(GameDataPath, NodePath.SelectSingleNode("Shaders").InnerText)
            };

            string[] TexturesPaths = new string[]
            {
                Path.Combine(EngineContentPath, NodePath.SelectSingleNode("EngineTextures").InnerText),
                Path.Combine(GameDataPath, NodePath.SelectSingleNode("Textures").InnerText)
            };

            string[] CubemapTexturesPaths = new string[]
            {
                Path.Combine(EngineContentPath, NodePath.SelectSingleNode("EngineCubemapTextures").InnerText),
                Path.Combine(GameDataPath, NodePath.SelectSingleNode("CubemapTextures").InnerText)
            };
            #endregion

            string[] Paths = new string[]
            {
                EngineContentPath,
                GameDataPath
            };

            string[] TexturesConfigs = new string[Paths.Length];
            string[] CubemapTexturesConfigs = new string[Paths.Length];
            string[] MaterialsConfigs = new string[Paths.Length];
            string[] ShadersConfigs = new string[Paths.Length];

            for (int i = 0; i < Paths.Length; i++)
            {
                if (i == 0)
                    NodePath = XML.DocumentElement.SelectSingleNode("EngineConfigFiles");
                else if (i == 1)
                    NodePath = XML.DocumentElement.SelectSingleNode("ConfigFiles");

                TexturesConfigs[i] = Path.Combine(Paths[i], NodePath.SelectSingleNode("Textures").InnerText);
                CubemapTexturesConfigs[i] = Path.Combine(Paths[i], NodePath.SelectSingleNode("CubemapTextures").InnerText);
                MaterialsConfigs[i] = Path.Combine(Paths[i], NodePath.SelectSingleNode("Materials").InnerText);
                ShadersConfigs[i] = Path.Combine(Paths[i], NodePath.SelectSingleNode("Shaders").InnerText);
            }

            for (int i = 0; i < Paths.Length; i++)
            {
                #region Загружаем шейдеры
                XML.Load(ShadersConfigs[i]);
                XmlNodeList xmlNodeList = XML.DocumentElement.SelectNodes("Shader");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Shaders.Add(xmlNode.SelectSingleNode("Name").InnerText, new ShaderProgram(
                        Path.Combine(ShadersPaths[i], xmlNode.SelectSingleNode("VertexShader").InnerText),
                        Path.Combine(ShadersPaths[i], xmlNode.SelectSingleNode("FragmentShader").InnerText), true));
                }

                // Проверка шейдеров на ошибки
                foreach (var key in Shaders.Keys)
                {
                    string InfoLog = GL.GetProgramInfoLog(Shaders[key].ProgramID);
                    if (InfoLog != String.Empty)
                        MessageBox.Show(InfoLog, "Shader Program Error: " + key);
                }
                #endregion

                #region Загружаем текстуры
                XML.Load(TexturesConfigs[i]);
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
                            Textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                                Path.Combine(TexturesPaths[i], xmlNode.SelectSingleNode("File").InnerText))));
                        }
                        else if (StrMagFilter == String.Empty)
                        {
                            Textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                                Path.Combine(TexturesPaths[i], xmlNode.SelectSingleNode("File").InnerText),
                                TextureMagFilter.Linear,
                                (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true))));
                        }
                        else if (StrMinFilter == String.Empty)
                        {
                            Textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                                Path.Combine(TexturesPaths[i], xmlNode.SelectSingleNode("File").InnerText),
                                (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true))));
                        }
                        else
                        {
                            Textures.Add(xmlNode.SelectSingleNode("Name").InnerText, new Texture(Texture.LoadImage(
                                Path.Combine(TexturesPaths[i], xmlNode.SelectSingleNode("File").InnerText),
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

                #region CubemapTextures
                XML.Load(CubemapTexturesConfigs[i]);
                xmlNodeList = XML.DocumentElement.SelectNodes("CubemapTexture");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    try
                    {
                        string Name = xmlNode.SelectSingleNode("Name").InnerText;
                        string CubemapDir = Path.Combine(CubemapTexturesPaths[i], xmlNode.SelectSingleNode("SubDir").InnerText);
                        XmlNodeList TMPXmlNodeList = xmlNode.SelectNodes("Texture");

                        if (TMPXmlNodeList.Count == 6)
                        {
                            Bitmap[] CubemapsBMP = new Bitmap[TMPXmlNodeList.Count];

                            int SideSize = -1;
                            if (xmlNode.SelectNodes("SideSize").Count > 0 && xmlNode.SelectSingleNode("SideSize").InnerText.Trim() != String.Empty)
                                SideSize = int.Parse(xmlNode.SelectSingleNode("SideSize").InnerText);

                            for (int j = 0; j < TMPXmlNodeList.Count; j++)
                                CubemapsBMP[j] = new Bitmap(Path.Combine(CubemapDir, TMPXmlNodeList[j].InnerText));

                            if (SideSize > 0)
                                for (int j = 0; j < TMPXmlNodeList.Count; j++)
                                    CubemapsBMP[j] = new Bitmap(CubemapsBMP[j], SideSize, SideSize);

                            Textures.Add(Name, new Texture(Texture.LoadCubemap(CubemapsBMP), TextureTarget.TextureCubeMap));
                            CubemapsBMP = null;
                        }
                        else
                            new Exception();
                    }
                    catch
                    {
                        MessageBox.Show("CubemapTexture Loading Error!");
                    }
                }
                #endregion

                #region Загружаем материалы
                XML.Load(MaterialsConfigs[i]);
                xmlNodeList = XML.DocumentElement.SelectNodes("Material");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    try
                    {
                        string[] StrArrTMP;
                        string StrTMP;

                        string Name = xmlNode.SelectSingleNode("Name").InnerText;
                        Materials.Add(Name, new Material());

                        if (xmlNode.SelectNodes("Shader").Count > 0)
                            Materials[Name].ShaderName = xmlNode.SelectSingleNode("Shader").InnerText;

                        if (xmlNode.SelectNodes("Color").Count > 0)
                        {
                            StrTMP = String.Empty;
                            StrTMP = xmlNode.SelectSingleNode("Color").InnerText;
                            StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                            if (StrArrTMP.Length == 4)
                                Materials[Name].Color = new Vector4(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]), float.Parse(StrArrTMP[3]));
                        }

                        int TexturesCount = xmlNode.SelectNodes("Texture").Count;
                        if (TexturesCount > 0 && TexturesCount <= 32)
                        {
                            XmlNodeList xmlNodesTMP = xmlNode.SelectNodes("Texture");
                            for (int j = 0; j < xmlNodesTMP.Count; j++)
                                Materials[Name].SetTexture(j, xmlNodesTMP[j].InnerText);
                        }

                        TexturesCount = xmlNode.SelectNodes("CubemapTexture").Count;
                        if (TexturesCount > 0 && TexturesCount <= 32)
                        {
                            XmlNodeList xmlNodesTMP = xmlNode.SelectNodes("CubemapTexture");
                            for (int j = 0; j < xmlNodesTMP.Count; j++)
                                Materials[Name].SetTexture(j, xmlNodesTMP[j].InnerText);
                        }

                        if (xmlNode.SelectNodes("CullFace").Count > 0)
                            Materials[Name].CullFace = bool.Parse(xmlNode.SelectSingleNode("CullFace").InnerText);

                        if (xmlNode.SelectNodes("Transparent").Count > 0)
                            Materials[Name].Transparent = bool.Parse(xmlNode.SelectSingleNode("Transparent").InnerText);

                        if (xmlNode.SelectNodes("SpecularReflectivity").Count > 0)
                        {
                            StrTMP = String.Empty;
                            StrTMP = xmlNode.SelectSingleNode("SpecularReflectivity").InnerText;
                            StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                            if (StrArrTMP.Length == 3)
                                Materials[Name].SpecularReflectivity = new Vector3(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]));
                        }

                        if (xmlNode.SelectNodes("AmbientReflectivity").Count > 0)
                        {
                            StrTMP = String.Empty;
                            StrTMP = xmlNode.SelectSingleNode("AmbientReflectivity").InnerText;
                            StrArrTMP = StrTMP.Trim().Replace(".", FloatComa).Replace(",", FloatComa).Split(new char[] { ' ' });
                            if (StrArrTMP.Length == 3)
                                Materials[Name].AmbientReflectivity = new Vector3(float.Parse(StrArrTMP[0]), float.Parse(StrArrTMP[1]), float.Parse(StrArrTMP[2]));
                        }

                        if (xmlNode.SelectNodes("SpecularShininess").Count > 0)
                            Materials[Name].SpecularShininess = float.Parse(xmlNode.SelectSingleNode("SpecularShininess")
                                .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));

                        if (xmlNode.SelectNodes("ReflectionFactor").Count > 0)
                            Materials[Name].ReflectionFactor = float.Parse(xmlNode.SelectSingleNode("ReflectionFactor")
                                .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));

                        if (xmlNode.SelectNodes("RefractiveIndex").Count > 0)
                            Materials[Name].RefractiveIndex = float.Parse(xmlNode.SelectSingleNode("RefractiveIndex")
                                .InnerText.Trim().Replace(".", FloatComa).Replace(",", FloatComa));
                    }
                    catch
                    {
                        MessageBox.Show("Material Loading Error!");
                    }
                }
                #endregion
            }

            ObjVolume Pipe_X_1 = ObjVolume.LoadFromFile(Path.Combine(MeshesPaths[1], "Pipe_X.obj"));
            //Pipe_X_1.Material = materials["BrickWall"];
            Pipe_X_1.Material = Materials["Refraction"]; // Reflection Refraction
            Pipe_X_1.Position.Y += 100;
            Objects.Add(Pipe_X_1);

            ObjVolume Pipe_X_2 = ObjVolume.LoadFromFile(Path.Combine(MeshesPaths[1], "Pipe_X.obj"));
            Pipe_X_2.Material = Materials["Reflection"];
            Pipe_X_2.Position.Y += 200;
            Objects.Add(Pipe_X_2);

            ObjVolume obj_Keypad = ObjVolume.LoadFromFile(Path.Combine(MeshesPaths[1], "Keypad.obj"));
            obj_Keypad.Material = Materials["Keypad"];
            obj_Keypad.Position = new Vector3(160f, 0f, 0f);
            obj_Keypad.Scale = new Vector3(10f, 10f, 10f);
            Objects.Add(obj_Keypad);

            Fog.Enabled = true;
            int a = 24;
            Prefab prefab1 = new Prefab(new Cube[a * a]);
            for (int i1 = 0; i1 < a; i1++)
                for (int i2 = 0; i2 < a; i2++)
                {
                    prefab1.Objects[i1 * a + i2] = new Cube();
                    prefab1.Objects[i1 * a + i2].Material = Materials["BrickWall"];
                    prefab1.Objects[i1 * a + i2].Position.X = (i1 - a / 2) * 400;
                    prefab1.Objects[i1 * a + i2].Position.Z = (i2 - a / 2) * 400;
                }
            Prefabs.Add(prefab1);

            ObjVolume obj_Teapot = ObjVolume.LoadFromFile(Path.Combine(MeshesPaths[0], "Teapot.obj"));
            obj_Teapot.Material = Materials["TransparentRedGlass"];
            obj_Teapot.Position.Z += 200;
            obj_Teapot.Scale = new Vector3(3f, 3f, 3f);
            Objects.Add(obj_Teapot);

            ObjVolume obj_Teapot2 = ObjVolume.LoadFromFile(Path.Combine(MeshesPaths[0], "Teapot.obj"));
            obj_Teapot2.Material = Materials["BrickWall"];
            obj_Teapot2.Position.Z -= 200;
            obj_Teapot2.Scale = new Vector3(3f, 3f, 3f);
            Objects.Add(obj_Teapot2);

            ObjVolume DebugLight = ObjVolume.LoadFromFile(Path.Combine(MeshesPaths[0], "DebugLight.obj"));
            DebugLight.Material = Materials["DebugLight"];
            DebugObjects.Add(DebugLight);
        }

        void initProgram()
        {
            // Загружаем конфигурацию и ресурсы
            LoadConfigAndResources();

            // Настраиваем проекцию камеры
            MainCamera.SetProjectionMatrix(ProjectionTypes.Perspective, (float)ClientSize.Width, (float)ClientSize.Height, zNear, zFar, FOV);
            MainCamera.MoveSpeed = 10f;

            GL.GenBuffers(1, out IndexBufferId);

            // PostProcess Init - Create back-buffer, used for post-processing
            if (UsePostEffects)
                PostProcess.Init("PostProcessFXAA", Width, Height);

            // Включаем тест глубины
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // Функция смешивания цветов для прозрачных материалов
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.One);

            // Создаем примитивы
            Cube cube = new Cube(zFar);
            cube.Material = Materials["SkyCubemap_Storforsen"];
            Objects.Add(cube);

            //Plain plain = new Plain();
            //objects.Add(plain);

            // Отдаляем камеру от начала координат
            MainCamera.Position = new Vector3(0.0f, 200.0f, 0.0f);
        }

        void ResetCursor()
        {
            OpenTK.Input.Mouse.SetPosition(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
            LastMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }

        void DrawObject(Volume v)
        {
            // Активируем нужный TextureUnit и назначаем текстуру
            if (v.Material.TexturesCount > 0)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(Textures[v.Material.GetTexture(0)].TextureTarget, Textures[v.Material.GetTexture(0)].TextureID);
                for (int i = 1; i < v.Material.TexturesCount; i++)
                {
                    GL.ActiveTexture((TextureUnit)(0x84C0 + i));
                    GL.BindTexture(Textures[v.Material.GetTexture(i)].TextureTarget, Textures[v.Material.GetTexture(i)].TextureID);
                }
            }

            #region Работаем с шейдерами
            // Выбираем шейдеры для отрисовки объекта
            if (ActiveShader != Shaders[v.Material.ShaderName].ProgramID)
            {
                ActiveShader = Shaders[v.Material.ShaderName].ProgramID;
                GL.UseProgram(ActiveShader);
            }

            // Передаем шейдеру вектор Light Position, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("Light.Position") != -1)
            {
                Vector4 L = new Vector4(1000.0f * (float)Math.Cos(Angle), 0.0f, 1000.0f * (float)Math.Sin(Angle), 1.0f);
                Matrix4 V = MainCamera.GetViewMatrix();
                DebugObjects[0].Position = L.Xyz;
                GL.Uniform4(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Light.Position"), L.Mult(V));
            }

            // Передаем шейдеру вектор Light Intensity, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("Light.Intensity") != -1)
                GL.Uniform3(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Light.Intensity"), 0.9f, 0.9f, 0.9f);

            // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("Material.Kd") != -1)
                GL.Uniform3(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Material.Kd"), new Vector3(0.9f, 0.5f, 0.3f));

            // Передаем шейдеру вектор Specular reflectivity, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("Material.Ks") != -1)
                GL.Uniform3(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Material.Ks"), v.Material.SpecularReflectivity);

            // Передаем шейдеру вектор Ambient reflectivity, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("Material.Ka") != -1)
                GL.Uniform3(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Material.Ka"), v.Material.AmbientReflectivity);

            // Передаем шейдеру значение Specular shininess factor, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("Material.Shininess") != -1)
                GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Material.Shininess"), v.Material.SpecularShininess);

            // Передаем шейдеру значение цвета материала, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("MaterialColor") != -1)
                GL.Uniform4(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "MaterialColor"), v.Material.Color);

            // Передаем шейдеру значение ReflectFactor, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("ReflectionFactor") != -1)
                GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "ReflectionFactor"), v.Material.ReflectionFactor);

            // Передаем шейдеру значение RefractiveIndex, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("RefractiveIndex") != -1)
                GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "RefractiveIndex"), v.Material.RefractiveIndex);

            // Передаем шейдеру матрицу ModelMatrix, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("ModelMatrix") != -1)
                GL.UniformMatrix4(Shaders[v.Material.ShaderName].GetUniform("ModelMatrix"), false, ref v.ModelMatrix);

            // Передаем шейдеру матрицу ModelViewMatrix, если шейдер поддерживает это.
            Matrix4 MV = MainCamera.GetViewMatrix() * v.ModelMatrix;
            if (Shaders[v.Material.ShaderName].GetUniform("ModelViewMatrix") != -1)
                GL.UniformMatrix4(Shaders[v.Material.ShaderName].GetUniform("ModelViewMatrix"), false, ref MV);

            //// Передаем шейдеру матрицу ProjectionMatrix, если шейдер поддерживает это.
            //if (shaders[v.Material.ShaderName].GetUniform("ProjectionMatrix") != -1)
            //    GL.UniformMatrix4(shaders[v.Material.ShaderName].GetUniform("ProjectionMatrix"), false, ref ProjectionMatrix);

            // Передаем шейдеру матрицу NormalMatrix, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("NormalMatrix") != -1)
            {
                Matrix3 NM = new Matrix3(MV.Row0.Xyz, MV.Row1.Xyz, MV.Row2.Xyz);
                GL.UniformMatrix3(Shaders[v.Material.ShaderName].GetUniform("NormalMatrix"), false, ref NM);
            }

            // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
            if (Shaders[v.Material.ShaderName].GetUniform("MVP") != -1)
                GL.UniformMatrix4(Shaders[v.Material.ShaderName].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

            // Передаем шейдеру позицию камеры, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetUniform("WorldCameraPosition") != -1)
                GL.Uniform3(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "WorldCameraPosition"), MainCamera.Position);

            /////////ТУМАН/////////////////////////
            if (Fog.Enabled)
            {
                if (Shaders[v.Material.ShaderName].GetUniform("FogEnabled") != -1)
                    GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "FogEnabled"), Convert.ToInt32(Fog.Enabled));
                if (Shaders[v.Material.ShaderName].GetUniform("Fog.MaxDist") != -1)
                    GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Fog.MaxDist"), Fog.MaxDistance);
                if (Shaders[v.Material.ShaderName].GetUniform("Fog.MinDist") != -1)
                    GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Fog.MinDist"), Fog.MinDistance);
                if (Shaders[v.Material.ShaderName].GetUniform("Fog.Color") != -1)
                    GL.Uniform3(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "Fog.Color"), Fog.Color);
            }
            else
                if (Shaders[v.Material.ShaderName].GetUniform("FogEnabled") != -1)
                    GL.Uniform1(GL.GetUniformLocation(Shaders[v.Material.ShaderName].ProgramID, "FogEnabled"), Convert.ToInt32(Fog.Enabled));

            #region Передаем шейдеру VertexPosition, VertexNormal, VertexTexCoord, VertexTangent
            // Передаем шейдеру буфер позицый вертексов, если шейдер поддерживает это (должна быть 100% поддержка).
            if (Shaders[v.Material.ShaderName].GetAttribute("VertexPosition") != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Shaders[v.Material.ShaderName].GetBuffer("VertexPosition"));
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetVertices().Length * Vector3.SizeInBytes), v.GetVertices(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(Shaders[v.Material.ShaderName].GetAttribute("VertexPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(Shaders[v.Material.ShaderName].GetAttribute("VertexPosition"));
            }

            // Передаем шейдеру буфер нормалей, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetAttribute("VertexNormal") != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Shaders[v.Material.ShaderName].GetBuffer("VertexNormal"));
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetNormals().Length * Vector3.SizeInBytes), v.GetNormals(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(Shaders[v.Material.ShaderName].GetAttribute("VertexNormal"), 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(Shaders[v.Material.ShaderName].GetAttribute("VertexNormal"));
            }

            // Передаем шейдеру буфер текстурных координат, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetAttribute("VertexTexCoord") != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Shaders[v.Material.ShaderName].GetBuffer("VertexTexCoord"));
                GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetTextureCoords().Length * Vector2.SizeInBytes), v.GetTextureCoords(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(Shaders[v.Material.ShaderName].GetAttribute("VertexTexCoord"), 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(Shaders[v.Material.ShaderName].GetAttribute("VertexTexCoord"));
            }

            // Передаем шейдеру буфер тангенсов, если шейдер поддерживает это.
            if (Shaders[v.Material.ShaderName].GetAttribute("VertexTangent") != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Shaders[v.Material.ShaderName].GetBuffer("VertexTangent"));
                GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(v.GetTangentses().Length * Vector4.SizeInBytes), v.GetTangentses(), BufferUsageHint.StaticDraw);
                GL.BindVertexArray(0);
            }
            #endregion
            #endregion

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(v.GetIndexes().Length * sizeof(int)), v.GetIndexes(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(Shaders[v.Material.ShaderName].GetAttribute("VertexPosition"));
            GL.DrawElements(BeginMode.Triangles, v.IndexesCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
