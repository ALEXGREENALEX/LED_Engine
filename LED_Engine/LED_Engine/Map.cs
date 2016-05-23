using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Maps
    {
        public static Dictionary<string, string> MapsList = new Dictionary<string, string>(); // Dictionary<MapName, Path>
        public static List<string> EngineMapsList = new List<string>();

        public static string Name = String.Empty;
        public static string Description = String.Empty;

        public static void LoadMapList(string MapPath, bool EngineContent)
        {
            string[] Maps = Directory.GetFiles(MapPath, "*.xml", SearchOption.TopDirectoryOnly);

            XmlDocument XML = new XmlDocument();

            for (int i = 0; i < Maps.Length; i++)
            {
                try
                {
                    XML.Load(Maps[i]);
                    string Name = XML.DocumentElement.SelectSingleNode("Name").InnerText;
                    if (Name != null)
                    {
                        MapsList.Add(Name, Maps[i]);

                        if (EngineContent)
                            EngineMapsList.Add(Name);
                    }
                }

                catch (Exception e)
                {
                    Log.WriteLineRed("Maps.LoadMapList() Exception.");
                    Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", Maps[i], EngineContent);
                    Log.WriteLineYellow("Maybe some XML file in Map directory is not a valid map.");
                    Log.WriteLineYellow(e.Message);
                }
            }
        }

        public static void LoadMap(string MapName = "")
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNode xmlNode;
                XmlNodeList xmlNodeList;

                XML.Load(MapsList[MapName]);

                Maps.Name = XML.DocumentElement.SelectSingleNode("Name").InnerText;

                Log.WriteLine("Loading map: \"{0}\"", MapName);

                if (XML.DocumentElement.SelectNodes("Description").Count > 0)
                    Maps.Description = XML.DocumentElement.SelectSingleNode("Description").InnerText;

                #region PlayerStart
                if (XML.DocumentElement.SelectNodes("PlayerStart").Count > 0)
                {
                    xmlNode = XML.DocumentElement.SelectSingleNode("PlayerStart");

                    if (xmlNode.SelectNodes("Position").Count > 0)
                    {
                        string[] Position = xmlNode.SelectSingleNode("Position").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Game.MainCamera.Position = new Vector3(float.Parse(Position[0]), float.Parse(Position[1]), float.Parse(Position[2]));
                    }

                    if (xmlNode.SelectNodes("Rotation").Count > 0)
                    {
                        string[] Rotation = xmlNode.SelectSingleNode("Rotation").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Game.MainCamera.YawPitch = new Vector2(
                            MathHelper.DegreesToRadians(-float.Parse(Rotation[0])),
                            MathHelper.DegreesToRadians(float.Parse(Rotation[1])));
                    }
                }
                #endregion

                #region Camera
                if (XML.DocumentElement.SelectNodes("Camera").Count > 0)
                {
                    xmlNode = XML.DocumentElement.SelectSingleNode("Camera");

                    if (xmlNode.SelectNodes("Near").Count > 0)
                        Game.MainCamera.zNear = float.Parse(xmlNode.SelectSingleNode("Near").InnerText);
                    if (xmlNode.SelectNodes("Far").Count > 0)
                        Game.MainCamera.zFar = float.Parse(xmlNode.SelectSingleNode("Far").InnerText);
                    if (xmlNode.SelectNodes("FOV").Count > 0)
                        Game.MainCamera.FOV = float.Parse(xmlNode.SelectSingleNode("FOV").InnerText);
                    if (xmlNode.SelectNodes("MoveSpeed").Count > 0)
                        Game.MainCamera.MoveSpeed = float.Parse(xmlNode.SelectSingleNode("MoveSpeed").InnerText);
                }
                #endregion

                #region SkyBox
                if (XML.DocumentElement.SelectNodes("Skybox").Count > 0)
                {
                    string SkyBox = XML.DocumentElement.SelectSingleNode("Skybox").InnerText;

                    if (SkyBox != null && SkyBox != String.Empty)
                    {
                        Game.SkyBox = new Mesh();
                        MeshPart MPart = MeshPart.MakeBox(Game.MainCamera.zFar * 2.0f / (float)Math.Sqrt(3.0), true); // Cube diagonal = side / sqrt(3)
                        MPart.Material = Materials.Load(SkyBox);
                        Game.SkyBox.Parts.Add(MPart);
                        Game.SkyBox.CalcBoundingObjects();
                    }
                }
                #endregion

                #region Fog
                if (XML.DocumentElement.SelectNodes("Fog").Count > 0)
                {
                    xmlNode = XML.DocumentElement.SelectSingleNode("Fog");

                    if (xmlNode.SelectNodes("Enabled").Count > 0)
                        Settings.Graphics.Fog.UseFogOnMap = Convert.ToBoolean(xmlNode.SelectSingleNode("Enabled").InnerText);

                    if (xmlNode.SelectNodes("Color").Count > 0)
                    {
                        try
                        {
                            string[] Color = xmlNode.SelectSingleNode("Color").InnerText.Split(
                                new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

                            float R, G, B;
                            switch (Color.Length)
                            {
                                case 1:
                                    R = float.Parse(Color[0]);
                                    Settings.Graphics.Fog.Color = new Vector3(R, R, R);
                                    break;
                                case 3:
                                    R = float.Parse(Color[0]);
                                    G = float.Parse(Color[1]);
                                    B = float.Parse(Color[2]);
                                    Settings.Graphics.Fog.Color = new Vector3(R, G, B);
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch { }
                    }

                    if (xmlNode.SelectNodes("DistanceMin").Count > 0)
                        Settings.Graphics.Fog.MinDistance = float.Parse(xmlNode.SelectSingleNode("DistanceMin").InnerText);

                    if (xmlNode.SelectNodes("DistanceMax").Count > 0)
                        Settings.Graphics.Fog.MaxDistance = float.Parse(xmlNode.SelectSingleNode("DistanceMax").InnerText);
                }

                #endregion

                #region Models
                foreach (XmlNode xmlNodeModel in XML.DocumentElement.SelectNodes("Model"))
                {
                    Model model = new Model();
                    model.Name = xmlNodeModel.SelectSingleNode("Name").InnerText;

                    if (xmlNodeModel.SelectNodes("Visible").Count > 0)
                        model.Visible = Convert.ToBoolean(xmlNodeModel.SelectSingleNode("Visible").InnerText);

                    foreach (XmlNode xmlNodeMesh in xmlNodeModel.SelectNodes("Mesh"))
                    {
                        Mesh mesh = new Mesh();
                        string Name = xmlNodeMesh.SelectSingleNode("Name").InnerText;

                        MeshType meshType = MeshType.Mesh;

                        if (xmlNodeMesh.SelectNodes("Type").Count > 0)
                        {
                            string meshTypeStr = xmlNodeMesh.SelectSingleNode("Type").InnerText;

                            try
                            {
                                meshType = (MeshType)Enum.Parse(typeof(MeshType), meshTypeStr, true);
                            }
                            catch
                            {
                                meshType = MeshType.Mesh;
                            }
                        }

                        float SideA, SideB, SideC;
                        MeshPart mp;
                        bool FlipPolygons = false;
                        switch (meshType)
                        {
                            case MeshType.Plain:
                                mesh.Name = Name;
                                mesh.MeshName = meshType.ToString();
                                SideA = float.Parse(xmlNodeMesh.SelectSingleNode("SideA").InnerText);
                                SideB = float.Parse(xmlNodeMesh.SelectSingleNode("SideB").InnerText);
                                mp = MeshPart.MakePlain(SideA, SideB);
                                mesh.Parts.Add(mp);
                                mesh.CalcBoundingObjects();
                                break;

                            case MeshType.Box:
                                mesh.Name = Name;
                                mesh.MeshName = meshType.ToString();
                                SideA = float.Parse(xmlNodeMesh.SelectSingleNode("SideA").InnerText);
                                SideB = float.Parse(xmlNodeMesh.SelectSingleNode("SideB").InnerText);
                                SideC = float.Parse(xmlNodeMesh.SelectSingleNode("SideC").InnerText);

                                if (xmlNodeMesh.SelectNodes("FlipPolygons").Count > 0)
                                    FlipPolygons = Convert.ToBoolean(xmlNodeMesh.SelectSingleNode("FlipPolygons").InnerText);

                                mp = MeshPart.MakeBox(SideA, SideB, SideC, FlipPolygons);
                                mesh.Parts.Add(mp);
                                mesh.CalcBoundingObjects();
                                break;

                            case MeshType.Mesh:
                            default:
                                string MeshName = xmlNodeMesh.SelectSingleNode("Mesh").InnerText;
                                mesh = Meshes.Load(Name, MeshName);
                                break;
                        }

                        string[] Pos = xmlNodeMesh.SelectSingleNode("Position").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        mesh.Position = new Vector3(float.Parse(Pos[0]), float.Parse(Pos[1]), float.Parse(Pos[2]));

                        string[] Rot = xmlNodeMesh.SelectSingleNode("Rotation").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        mesh.Rotation = new Vector3(MathHelper.DegreesToRadians(float.Parse(Rot[0])),
                            MathHelper.DegreesToRadians(float.Parse(Rot[1])), MathHelper.DegreesToRadians(float.Parse(Rot[2])));

                        string[] Scl = xmlNodeMesh.SelectSingleNode("Scale").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        mesh.Scale = new Vector3(float.Parse(Scl[0]), float.Parse(Scl[1]), float.Parse(Scl[2]));

                        XmlNodeList xmlMaterials = xmlNodeMesh.SelectSingleNode("Materials").SelectNodes("Material");
                        int Count = Math.Min(xmlMaterials.Count, mesh.Parts.Count);
                        for (int i = 0; i < Count; i++)
                            mesh.Parts[i].Material = Materials.Load(xmlMaterials[i].InnerText);

                        if (Count < mesh.Parts.Count && Count > 0)
                        {
                            for (int i = Count; i < mesh.Parts.Count; i++)
                                mesh.Parts[i].Material = mesh.Parts[0].Material;
                        }

                        model.Meshes.Add(mesh);
                    }

                    Models.MODELS.Add(model);
                }
                #endregion

                #region Light
                foreach (XmlNode xmlNodeLight in XML.DocumentElement.SelectNodes("Light"))
                {
                    Light light = new Light();
                    light.Name = xmlNodeLight.SelectSingleNode("Name").InnerText;

                    if (xmlNodeLight.SelectNodes("Enabled").Count > 0)
                        light.Enabled = Convert.ToBoolean(xmlNodeLight.SelectSingleNode("Enabled").InnerText);

                    LightType lightType = LightType.Point;
                    if (xmlNodeLight.SelectNodes("Type").Count > 0)
                    {
                        string meshTypeStr = xmlNodeLight.SelectSingleNode("Type").InnerText;

                        try
                        {
                            lightType = (LightType)Enum.Parse(typeof(LightType), meshTypeStr, true);
                        }
                        catch
                        {
                            lightType = LightType.Point;
                        }

                        light.Type = lightType;
                    }

                    #region Diffuse Light
                    xmlNodeList = xmlNodeLight.SelectNodes("Diffuse");
                    if (xmlNodeList.Count > 0)
                    {
                        try
                        {
                            string[] Diffuse = xmlNodeList.Item(0).InnerText.Split(
                                new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            float R, G, B;
                            switch (Diffuse.Length)
                            {
                                case 1:
                                    R = float.Parse(Diffuse[0]);
                                    light.Diffuse = new Vector3(R, R, R);
                                    break;
                                case 3:
                                    R = float.Parse(Diffuse[0]);
                                    G = float.Parse(Diffuse[1]);
                                    B = float.Parse(Diffuse[2]);
                                    light.Diffuse = new Vector3(R, G, B);
                                    break;
                            }
                        }
                        catch { }
                    }
                    #endregion

                    #region Specular Light
                    xmlNodeList = xmlNodeLight.SelectNodes("Specular");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] Specular = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        float R, G, B;
                        switch (Specular.Length)
                        {
                            case 1:
                                R = float.Parse(Specular[0]);
                                light.Specular = new Vector3(R, R, R);
                                break;
                            case 3:
                                R = float.Parse(Specular[0]);
                                G = float.Parse(Specular[1]);
                                B = float.Parse(Specular[2]);
                                light.Specular = new Vector3(R, G, B);
                                break;
                        }
                    }
                    #endregion

                    #region Position
                    xmlNodeList = xmlNodeLight.SelectNodes("Position");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] Pos = xmlNodeList.Item(0).InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        light.Position = new Vector3(float.Parse(Pos[0]), float.Parse(Pos[1]), float.Parse(Pos[2]));
                    }
                    #endregion

                    #region Direction
                    xmlNodeList = xmlNodeLight.SelectNodes("Direction");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] Direct = xmlNodeList.Item(0).InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        light.Direction = Vector3.FromYawPitch(
                            MathHelper.DegreesToRadians(float.Parse(Direct[0])),
                            MathHelper.DegreesToRadians(float.Parse(Direct[1])));
                    }
                    #endregion

                    #region Attenuation
                    xmlNodeList = xmlNodeLight.SelectNodes("Attenuation");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] Atten = xmlNodeList.Item(0).InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        light.Attenuation = new Vector3(float.Parse(Atten[0]), float.Parse(Atten[1]), float.Parse(Atten[2]));
                    }
                    #endregion

                    if (xmlNodeLight.SelectNodes("CutOFF").Count > 0)
                        light.CutOFF = float.Parse(xmlNodeLight.SelectSingleNode("CutOFF").InnerText);

                    if (xmlNodeLight.SelectNodes("Exponent").Count > 0)
                        light.Exponent = float.Parse(xmlNodeLight.SelectSingleNode("Exponent").InnerText);

                    Lights.LIGHTS.Add(light);
                }
                #endregion

                #region Sounds
                //xmlNodeList = XML.DocumentElement.SelectSingleNode("Sounds").SelectNodes("Sound");

                //foreach (XmlNode xmlNodeSound in xmlNodeList)
                //{
                //    string SndName = xmlNodeSound.SelectSingleNode("Name").InnerText;
                //    bool SndEnabled = Convert.ToBoolean(xmlNodeSound.SelectSingleNode("Enabled").InnerText);
                //    string SndFile = xmlNodeSound.SelectSingleNode("File").InnerText;
                //    int SndVolume = Convert.ToInt32(xmlNodeSound.SelectSingleNode("Volume").InnerText);
                //    int SndRadius = Convert.ToInt32(xmlNodeSound.SelectSingleNode("Radius").InnerText);

                //    string[] SndPos = xmlNodeSound.SelectSingleNode("Position").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //    Vector3 SndPosition = new Vector3(
                //        float.Parse(SndPos[0]),
                //        float.Parse(SndPos[1]),
                //        float.Parse(SndPos[2]));
                //}
                #endregion

                Log.WriteLineGreen("Done.");
            }
            catch
            {
                Log.WriteLineRed("Map \"{0}\" loading Error!\n\t", MapName);
            }
        }

        public static void Free(bool WithEngineContent = false)
        {
            Name = String.Empty;
            Description = String.Empty;

            Lights.LIGHTS.Clear();
            Game.SkyBox.Free();
            Game.MainCamera = new Camera();

            for (int i = 0; i < Models.MODELS.Count; i++)
                Models.MODELS[i].Free();
            Models.MODELS.Clear();

            int Count = Meshes.MESHES.Count;
            for (int i = 0; i < Count; i++)
                if (!Meshes.MESHES[i].EngineContent)
                {
                    Meshes.MESHES[i].Free();
                    Meshes.MESHES.RemoveAt(i);
                    Count--;
                }

            Count = Materials.MATERIALS.Count;
            for (int i = 0; i < Count; i++)
                if (!Materials.MATERIALS[i].EngineContent)
                {
                    Materials.MATERIALS[i].Free();
                    Materials.MATERIALS.RemoveAt(i);
                    Count--;
                }

            Count = Textures.TEXTURES.Count;
            for (int i = 0; i < Count; i++)
                if (!Textures.TEXTURES[i].EngineContent)
                {
                    Textures.TEXTURES[i].Free();
                    Textures.TEXTURES.RemoveAt(i);
                    Count--;
                }

            Count = Shaders.SHADERS.Count;
            for (int i = 0; i < Count; i++)
                if (!Shaders.SHADERS[i].EngineContent)
                {
                    Shaders.SHADERS[i].Free();
                    Shaders.SHADERS.RemoveAt(i);
                    Count--;
                }

            if (WithEngineContent)
            {
                for (int i = 0; i < Meshes.MESHES.Count; i++)
                    Meshes.MESHES[i].Free();
                Meshes.MESHES.Clear();

                for (int i = 0; i < Meshes.MeshesList.Count; i++)
                    Meshes.MeshesList[i].Free();
                Meshes.MeshesList.Clear();

                for (int i = 0; i < Materials.MATERIALS.Count; i++)
                    Materials.MATERIALS[i].Free();
                Materials.MATERIALS.Clear();

                for (int i = 0; i < Materials.MaterialsList.Count; i++)
                    Materials.MaterialsList[i].Free();
                Materials.MaterialsList.Clear();

                for (int i = 0; i < Textures.TEXTURES.Count; i++)
                    Textures.TEXTURES[i].Free();
                Textures.TEXTURES.Clear();

                for (int i = 0; i < Textures.TexturesList.Count; i++)
                    Textures.TexturesList[i].Free();
                Textures.TexturesList.Clear();

                for (int i = 0; i < Shaders.SHADERS.Count; i++)
                    Shaders.SHADERS[i].Free();
                Shaders.SHADERS.Clear();

                for (int i = 0; i < Shaders.ShadersList.Count; i++)
                    Shaders.ShadersList[i].Free();
                Shaders.ShadersList.Clear();
            }
        }
    }
}
