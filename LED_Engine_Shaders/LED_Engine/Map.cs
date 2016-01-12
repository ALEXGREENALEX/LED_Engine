using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static string SkyBox = String.Empty;

        public static void LoadMapList(string XmlFile, string MapPath, bool EngineContent)
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;
                MapsList.Clear();
                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("Map");

                foreach (XmlNode xmlNode in xmlNodeList)
                {

                    string Name = xmlNode.SelectSingleNode("Name").InnerText;
                    string File = xmlNode.SelectSingleNode("File").InnerText;
                    MapsList.Add(Name, Engine.CombinePaths(MapPath, File));

                    if (EngineContent)
                        EngineMapsList.Add(Name);
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Materials.LoadMaterialsList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                Log.WriteLineYellow(e.Message);
            }
        }

        public static void LoadMap(string MapName = "")
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;
                XmlNode xmlNode;

                XML.Load(MapsList[MapName]);
                Name = XML.DocumentElement.SelectSingleNode("MapName").InnerText;
                Description = XML.DocumentElement.SelectSingleNode("Description").InnerText;
                SkyBox = XML.DocumentElement.SelectSingleNode("Skybox").InnerText;

                #region PlayerStart
                xmlNode = XML.DocumentElement.SelectSingleNode("PlayerStart");
                string[] Position = xmlNode.SelectSingleNode("Position").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Game.MainCamera.Position = new Vector3(
                    float.Parse(Position[0]),
                    float.Parse(Position[1]),
                    float.Parse(Position[2]));

                string[] Rotation = xmlNode.SelectSingleNode("Rotation").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Game.MainCamera.Orientation = new Vector3(
                    float.Parse(Rotation[0]),
                    float.Parse(Rotation[1]),
                    float.Parse(Rotation[2]));
                #endregion

                #region Objects
                xmlNodeList = XML.DocumentElement.SelectSingleNode("Objects").SelectNodes("Object");

                foreach (XmlNode xmlNodeObject in xmlNodeList)
                {
                    string ObjName = xmlNodeObject.SelectSingleNode("Name").InnerText;

                    string[] ObjPos = xmlNodeObject.SelectSingleNode("Position").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Vector3 ObjPostion = new Vector3(
                        float.Parse(ObjPos[0]),
                        float.Parse(ObjPos[1]),
                        float.Parse(ObjPos[2]));

                    string[] ObjRot = xmlNodeObject.SelectSingleNode("Rotation").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Vector3 ObjRotation = new Vector3(
                        float.Parse(ObjRot[0]),
                        float.Parse(ObjRot[1]),
                        float.Parse(ObjRot[2]));

                    string[] ObjScl = xmlNodeObject.SelectSingleNode("Scale").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Vector3 ObjScale = new Vector3(
                        float.Parse(ObjScl[0]),
                        float.Parse(ObjScl[1]),
                        float.Parse(ObjScl[2]));

                    foreach (XmlNode xmlNodeMaterial in xmlNodeObject.SelectSingleNode("Materials"))
                    {
                        string MaterialName = xmlNodeMaterial.InnerText;
                    }
                }
                #endregion

                #region Sounds
                xmlNodeList = XML.DocumentElement.SelectSingleNode("Sounds").SelectNodes("Sound");

                foreach (XmlNode xmlNodeSound in xmlNodeList)
                {
                    string SndName = xmlNodeSound.SelectSingleNode("Name").InnerText;
                    bool SndEnabled = Convert.ToBoolean(xmlNodeSound.SelectSingleNode("Enabled").InnerText);
                    string SndFile = xmlNodeSound.SelectSingleNode("File").InnerText;
                    int SndVolume = Convert.ToInt32(xmlNodeSound.SelectSingleNode("Volume").InnerText);
                    int SndRadius = Convert.ToInt32(xmlNodeSound.SelectSingleNode("Radius").InnerText);

                    string[] SndPos = xmlNodeSound.SelectSingleNode("Position").InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Vector3 SndPosition = new Vector3(
                        float.Parse(SndPos[0]),
                        float.Parse(SndPos[1]),
                        float.Parse(SndPos[2]));
                }
                #endregion

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
            SkyBox = String.Empty;

            Shaders.Free(WithEngineContent);
            Models.Free(WithEngineContent);
            Materials.Free(WithEngineContent);
            Textures.Free(WithEngineContent);
        }
    }
}
