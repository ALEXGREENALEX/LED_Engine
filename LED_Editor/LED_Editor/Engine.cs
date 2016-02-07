using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace LED_Editor
{
    public static class Engine
    {
        public static void LoadConfig()
        {
            try
            {
                Settings.Paths.StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                XmlDocument XML = new XmlDocument();
                XmlNode TempNode;
                string TempPathStr;

                #region Загружаем настройки из "Config.xml"
                Debug.WriteLine("Loading settings from \"Config.xml\".");
                XML.Load(Engine.CombinePaths(Settings.Paths.StartupPath, "Config.xml"));

                Settings.Window.Title = XML.DocumentElement.SelectSingleNode("Title").InnerText;

                Settings.Window.Fullscreen = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("Fullscreen").InnerText);
                Settings.Window.FullscreenWindowed = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("FullscreenWindowed").InnerText);
                Settings.Window.CenterWindow = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("CenterWindow").InnerText);
                Settings.Window.Borders = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("WindowBorders").InnerText);
                Settings.Window.ActiveMonitor = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("Monitor").InnerText);

                Settings.Window.X = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("WindowX").InnerText);
                Settings.Window.Y = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("WindowY").InnerText);
                Settings.Window.Width = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("Width").InnerText);
                Settings.Window.Height = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("Height").InnerText);
                Settings.Window.OriginalX = Settings.Window.X;
                Settings.Window.OriginalY = Settings.Window.Y;
                Settings.Window.OriginalWidth = Settings.Window.Width;
                Settings.Window.OriginalHeight = Settings.Window.Height;

                Settings.Window.RefreshRate = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("RefreshRate").InnerText);
                Settings.Window.RefreshRateLimit = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("RefreshRateLimit").InnerText);

                try
                {
                    string[] Bits = XML.DocumentElement.SelectSingleNode("MonitorDepthBits").InnerText.Split(new char[] { ',', '.', ';' });
                    Settings.Window.RedBits = Convert.ToInt32(Bits[0].Trim());
                    Settings.Window.GreenBits = Convert.ToInt32(Bits[1].Trim());
                    Settings.Window.BlueBits = Convert.ToInt32(Bits[2].Trim());
                }
                catch { }

                Settings.Graphics.VSyncSwapInterval = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("VSyncSwapInterval").InnerText);
                Settings.Graphics.MSAASamples = Convert.ToInt32(XML.DocumentElement.SelectSingleNode("MSAASamples").InnerText);
                Settings.Graphics.FXAA.Enabled = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("FXAAEnabled").InnerText);

                Settings.Graphics.UsePostEffects = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("UsePostEffects").InnerText);
                Settings.Graphics.Fog.Enabled = Convert.ToBoolean(XML.DocumentElement.SelectSingleNode("FogEnabled").InnerText);
                #endregion

                #region Debug
                TempNode = XML.DocumentElement.SelectSingleNode("Debug");
                Settings.Debug.Enabled = Convert.ToBoolean(TempNode.SelectSingleNode("Enabled").InnerText);
                Settings.Debug.ShowFPS = Convert.ToBoolean(TempNode.SelectSingleNode("ShowFPS").InnerText);
                Settings.Debug.DrawDebugObjects = Convert.ToBoolean(TempNode.SelectSingleNode("DrawDebugObjects").InnerText);
                #endregion

                #region Определяем пути к папкам с ресурсами
                TempNode = XML.DocumentElement.SelectSingleNode("Paths");
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("GameData").InnerText);
                Settings.Paths.GameDataPath = Engine.CombinePaths(Settings.Paths.StartupPath, TempPathStr);

                // Проверка на запуск из студии или из папки "Bin"!!!
                if (!Directory.Exists(Settings.Paths.GameDataPath))
                {
                    Settings.Paths.StartupPath = Path.GetDirectoryName(Settings.Paths.StartupPath);
                    Settings.Paths.GameDataPath = Engine.CombinePaths(Settings.Paths.StartupPath, TempPathStr);
                    if (!Directory.Exists(Settings.Paths.GameDataPath))
                    {
                        Settings.Paths.StartupPath = Path.GetDirectoryName(Settings.Paths.StartupPath);
                        Settings.Paths.GameDataPath = Engine.CombinePaths(Settings.Paths.StartupPath, TempPathStr);
                    }
                }

                // Находим пути к ресурсам
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("EngineContentFolder").InnerText);
                Settings.Paths.EngineContentPath = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);

                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("EngineMaps").InnerText);
                Settings.Paths.EngineMaps = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("EngineMeshes").InnerText);
                Settings.Paths.EngineMeshes = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("EngineTextures").InnerText);
                Settings.Paths.EngineTextures = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("EngineCubemapTextures").InnerText);
                Settings.Paths.EngineCubemapTextures = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("EngineShaders").InnerText);
                Settings.Paths.EngineShaders = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);

                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Maps").InnerText);
                Settings.Paths.Maps = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Meshes").InnerText);
                Settings.Paths.Meshes = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Textures").InnerText);
                Settings.Paths.Textures = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("CubemapTextures").InnerText);
                Settings.Paths.CubemapTextures = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Shaders").InnerText);
                Settings.Paths.Shaders = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                #endregion

                #region Определяем пути к "*.xml" с инф. про ресурсы
                TempNode = XML.DocumentElement.SelectSingleNode("EngineContent");
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Textures").InnerText);
                Settings.Paths.ContentFiles.EngineTextures = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("CubemapTextures").InnerText);
                Settings.Paths.ContentFiles.EngineCubemapTextures = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Materials").InnerText);
                Settings.Paths.ContentFiles.EngineMaterials = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Meshes").InnerText);
                Settings.Paths.ContentFiles.EngineMeshes = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Shaders").InnerText);
                Settings.Paths.ContentFiles.EngineShaders = Engine.CombinePaths(Settings.Paths.EngineContentPath, TempPathStr);

                TempNode = XML.DocumentElement.SelectSingleNode("Content");
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Textures").InnerText);
                Settings.Paths.ContentFiles.Textures = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("CubemapTextures").InnerText);
                Settings.Paths.ContentFiles.CubemapTextures = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Materials").InnerText);
                Settings.Paths.ContentFiles.Materials = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Meshes").InnerText);
                Settings.Paths.ContentFiles.Meshes = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                TempPathStr = Engine.FixPath(TempNode.SelectSingleNode("Shaders").InnerText);
                Settings.Paths.ContentFiles.Shaders = Engine.CombinePaths(Settings.Paths.GameDataPath, TempPathStr);
                #endregion
                Debug.WriteLine("Done.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Engine.LoadConfig() Exception.");
                Debug.WriteLine(e.Message);
            }
        }

        public static void ApplyConfig()
        {
            //FPS.ShowFPS = Settings.Debug.ShowFPS;
            FBO.UsePostEffects = Settings.Graphics.UsePostEffects;
        }

        public static void GetGLSettings()
        {
            Settings.GL.MaxTextureImageUnits = GL.GetInteger(GetPName.MaxTextureImageUnits); //FS TextureImageUnits Only
            Settings.GL.MaxVertexTextureImageUnits = GL.GetInteger(GetPName.MaxVertexTextureImageUnits); //VS
            Settings.GL.MaxGeometryTextureImageUnits = GL.GetInteger(GetPName.MaxGeometryTextureImageUnits); //GS

            Settings.GL.TextureImageUnits = Math.Min(Math.Min(Settings.GL.MaxTextureImageUnits,
                Settings.GL.MaxVertexTextureImageUnits), Settings.GL.MaxGeometryTextureImageUnits);
        }

        public static void LoadContentLists()
        {
            try
            {
                #region Paths
                string[] Paths = new string[] { Settings.Paths.EngineContentPath, Settings.Paths.GameDataPath };

                string[] MaterialsConfigs = new string[] { Settings.Paths.ContentFiles.EngineMaterials, Settings.Paths.ContentFiles.Materials };
                string[] MeshesConfigs = new string[] { Settings.Paths.ContentFiles.EngineMeshes, Settings.Paths.ContentFiles.Meshes };
                string[] ShadersConfigs = new string[] { Settings.Paths.ContentFiles.EngineShaders, Settings.Paths.ContentFiles.Shaders };
                string[] TexturesConfigs = new string[] { Settings.Paths.ContentFiles.EngineTextures, Settings.Paths.ContentFiles.Textures };
                string[] CubemapTexturesConfigs = new string[] { Settings.Paths.ContentFiles.EngineCubemapTextures, Settings.Paths.ContentFiles.CubemapTextures };

                string[] MapsPaths = new string[] { Settings.Paths.EngineMaps, Settings.Paths.Maps };
                string[] MeshesPaths = new string[] { Settings.Paths.EngineMeshes, Settings.Paths.Meshes };
                string[] ShadersPaths = new string[] { Settings.Paths.EngineShaders, Settings.Paths.Shaders };
                string[] TexturesPaths = new string[] { Settings.Paths.EngineTextures, Settings.Paths.Textures };
                string[] CubemapTexturesPaths = new string[] { Settings.Paths.EngineCubemapTextures, Settings.Paths.CubemapTextures };
                #endregion

                #region Load Lists
                ClearLists();

                for (int i = 0; i < Paths.Length; i++)
                {
                    bool EngineContent = false;
                    if (i == 0)
                        EngineContent = true;

                    Textures.LoadTexturesList(TexturesConfigs[i], TexturesPaths[i], EngineContent);
                }

                for (int i = 0; i < Paths.Length; i++)
                {
                    bool EngineContent = false;
                    if (i == 0)
                        EngineContent = true;

                    Textures.LoadCubemapTexturesList(CubemapTexturesConfigs[i], CubemapTexturesPaths[i], EngineContent);
                }

                for (int i = 0; i < Paths.Length; i++)
                {
                    bool EngineContent = false;
                    if (i == 0)
                        EngineContent = true;

                    Shaders.LoadShadersList(ShadersConfigs[i], ShadersPaths[i], EngineContent);
                }

                for (int i = 0; i < Paths.Length; i++)
                {
                    bool EngineContent = false;
                    if (i == 0)
                        EngineContent = true;

                    Materials.LoadMaterialsList(MaterialsConfigs[i], EngineContent);
                }

                for (int i = 0; i < Paths.Length; i++)
                {
                    bool EngineContent = false;
                    if (i == 0)
                        EngineContent = true;

                    Meshes.LoadMeshesList(MeshesConfigs[i], MeshesPaths[i], EngineContent);
                }

                for (int i = 0; i < Paths.Length; i++)
                {
                    bool EngineContent = false;
                    if (i == 0)
                        EngineContent = true;

                    Maps.LoadMapList(MapsPaths[i], EngineContent);
                }
                #endregion
            }
            catch (Exception e)
            {
                Debug.WriteLine("Engine.LoadContentLists() Exception.");
                Debug.WriteLine(e.Message);
            }
        }

        public static void ClearLists()
        {
            FBO.Shaders.Clear();
            Textures.TexturesList.Clear();
            Shaders.ShadersList.Clear();
            Materials.MaterialsList.Clear();
            Meshes.MeshesList.Clear();
            Maps.MapsList.Clear();
        }

        public static void LoadEngineContent()
        {
            foreach (var i in Textures.TexturesList)
                if (i.EngineContent)
                    Textures.Load(i.Name);

            foreach (var i in Shaders.ShadersList)
                if (i.EngineContent)
                    Shaders.Load(i.Name);

            foreach (var i in Materials.MaterialsList)
                if (i.EngineContent)
                    Materials.Load(i.Name);

            if (Settings.Debug.Enabled)
            {
                //DebugAmbientLight = Mesh.LoadFromFile("DebugAmbientLight", Engine.CombinePaths(Settings.Paths.EngineMeshes, "DebugAmbientLight.obj"));
                //DebugDirectionalLight = Mesh.LoadFromFile("DebugDirectionalLight", Engine.CombinePaths(Settings.Paths.EngineMeshes, "DebugDirectionalLight.obj"));
                //DebugPointLight = Mesh.LoadFromFile("DebugPointLight", Engine.CombinePaths(Settings.Paths.EngineMeshes, "DebugPointLight.obj"));
                //DebugSpotLight = Mesh.LoadFromFile("DebugSpotLight", Engine.CombinePaths(Settings.Paths.EngineMeshes, "DebugSpotLight.obj"));

                //DebugAmbientLight.Parts[0].Material = Materials.Load("DebugAmbientLight");
                //DebugDirectionalLight.Parts[0].Material = Materials.Load("DebugDirectionalLight");
                //DebugPointLight.Parts[0].Material = Materials.Load("DebugPointLight");
                //DebugSpotLight.Parts[0].Material = Materials.Load("DebugSpotLight");
            }
        }

        /// <summary>
        /// Fix path if it have different Directory Separator Chars ('/' and '\').
        /// </summary>
        /// <param name="Path">Original path</param>
        /// <returns>Fixed path</returns>
        public static string FixPath(string Path)
        {
            return Path.Replace('/', System.IO.Path.DirectorySeparatorChar).
                Replace('\\', System.IO.Path.DirectorySeparatorChar).Trim();
        }

        public static string CombinePaths(string Path1, string Path2)
        {
            Path1 = FixPath(Path1).Trim().TrimEnd(new char[] { '\\', '/' });
            Path2 = FixPath(Path2).Trim().TrimStart(new char[] { '\\', '/' });
            return Path1 + System.IO.Path.DirectorySeparatorChar + Path2;
        }
    }
}