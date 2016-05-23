using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public partial class EditorTopForm : Form
    {
        public LightSettings lightSettings = new LightSettings();
        public EditorPropetriesForm EditorObjectForm = new EditorPropetriesForm();

        public EditorTopForm()
        {
            InitializeComponent();
        }

        public static void WriteMapData(string XmlFile)
        {
            try
            {
                XmlTextWriter textWritter = new XmlTextWriter(XmlFile, Encoding.UTF8);
                //textWritter.WriteStartDocument();
                textWritter.WriteStartElement("Map");
                textWritter.WriteEndElement();
                textWritter.Close();

                XmlDocument XML = new XmlDocument();

                #region Write Map data

                XML.Load(XmlFile);

                XmlNode mapname = XML.CreateElement("Name");
                mapname.InnerText = Maps.Name;
                XML.DocumentElement.AppendChild(mapname);

                XmlNode mapdescription = XML.CreateElement("Description");
                mapdescription.InnerText = Maps.Description;
                XML.DocumentElement.AppendChild(mapdescription);

                XmlNode mapplayerstart = XML.CreateElement("PlayerStart");
                XML.DocumentElement.AppendChild(mapplayerstart);

                XmlNode mapplayerstartpos = XML.CreateElement("Position");
                mapplayerstartpos.InnerText = Maps.PlayerStartPos.X + " " + Maps.PlayerStartPos.Y + " " + Maps.PlayerStartPos.Z;
                mapplayerstart.AppendChild(mapplayerstartpos);

                XmlNode mapplayerstartrot = XML.CreateElement("Rotation");
                mapplayerstartrot.InnerText = Maps.PlayerStartRot.X + " " + Maps.PlayerStartRot.Y;
                mapplayerstart.AppendChild(mapplayerstartrot);

                XmlNode Camera = XML.CreateElement("Camera");
                XML.DocumentElement.AppendChild(Camera);

                XmlNode CameraNear = XML.CreateElement("Near");
                CameraNear.InnerText = Game.MainCamera.zNear.ToString();
                Camera.AppendChild(CameraNear);

                XmlNode CameraFar = XML.CreateElement("Far");
                CameraFar.InnerText = Game.MainCamera.zFar.ToString();
                Camera.AppendChild(CameraFar);

                XmlNode CameraFOV = XML.CreateElement("FOV");
                CameraFOV.InnerText = Game.MainCamera.FOV.ToString();
                Camera.AppendChild(CameraFOV);

                XmlNode CameraMoveSpeed = XML.CreateElement("MoveSpeed");
                CameraMoveSpeed.InnerText = Game.MainCamera.MoveSpeed.ToString();
                Camera.AppendChild(CameraMoveSpeed);

                XmlNode Skybox = XML.CreateElement("Skybox");
                Skybox.InnerText = Game.SkyBox.Parts[0].Material.Name;
                XML.DocumentElement.AppendChild(Skybox);

                XmlNode Fog = XML.CreateElement("Fog");
                XML.DocumentElement.AppendChild(Fog);

                XmlNode FogEnabled = XML.CreateElement("Enabled");
                FogEnabled.InnerText = Settings.Graphics.Fog.Enabled.ToString();
                Fog.AppendChild(FogEnabled);

                XmlNode FogColor = XML.CreateElement("Color");
                FogColor.InnerText = Settings.Graphics.Fog.Color.X + " " + Settings.Graphics.Fog.Color.Y + " " + Settings.Graphics.Fog.Color.Z;
                Fog.AppendChild(FogColor);

                XmlNode FogDistanceMin = XML.CreateElement("DistanceMin");
                FogDistanceMin.InnerText = Settings.Graphics.Fog.MinDistance.ToString();
                Fog.AppendChild(FogDistanceMin);

                XmlNode FogDistanceMax = XML.CreateElement("DistanceMax");
                FogDistanceMax.InnerText = Settings.Graphics.Fog.MaxDistance.ToString();
                Fog.AppendChild(FogDistanceMax);

                foreach (Model mdl in Models.MODELS)
                {
                            XmlNode element = XML.CreateElement("Model");
                            XML.DocumentElement.AppendChild(element); // указываем родителя
                            //XmlAttribute attribute = XML.CreateAttribute("number"); // создаём атрибут
                            //attribute.Value = "1"; // устанавливаем значение атрибута
                            //element.Attributes.Append(attribute); // добавляем атрибут

                            XmlNode subElement1 = XML.CreateElement("Name"); // даём имя
                            subElement1.InnerText = mdl.Name; // и значение
                            element.AppendChild(subElement1); // и указываем кому принадлежит

                            XmlNode subElement2 = XML.CreateElement("Visible"); // даём имя
                            subElement2.InnerText = mdl.Visible.ToString(); // и значение
                            element.AppendChild(subElement2); // и указываем кому принадлежит
                            foreach (Mesh msh in mdl.Meshes)
                            {
                                XmlNode meshelement = XML.CreateElement("Mesh");
                                element.AppendChild(meshelement); // указываем родителя

                                XmlNode meshSubElement1 = XML.CreateElement("Name"); // даём имя
                                meshSubElement1.InnerText = msh.Name; // и значение
                                meshelement.AppendChild(meshSubElement1); // и указываем кому принадлежит

                                XmlNode meshSubElement2 = XML.CreateElement("Type"); // даём имя
                                meshSubElement2.InnerText = msh.MeshName; // и значение
                                meshelement.AppendChild(meshSubElement2); // и указываем кому принадлежит

                                XmlNode meshSubElement3 = XML.CreateElement("Mesh"); // даём имя
                                meshSubElement3.InnerText = msh.MeshName; // и значение
                                meshelement.AppendChild(meshSubElement3); // и указываем кому принадлежит

                                XmlNode meshSubElement4 = XML.CreateElement("Visible"); // даём имя
                                meshSubElement4.InnerText = msh.Visible.ToString(); // и значение
                                meshelement.AppendChild(meshSubElement4); // и указываем кому принадлежит

                                XmlNode meshSubElement5 = XML.CreateElement("Position"); // даём имя
                                meshSubElement5.InnerText = msh.Position.X + " " + msh.Position.Y + " " + msh.Position.Z; // и значение
                                meshelement.AppendChild(meshSubElement5); // и указываем кому принадлежит

                                XmlNode meshSubElement6 = XML.CreateElement("Rotation"); // даём имя
                                meshSubElement6.InnerText = MathHelper.RadiansToDegrees(msh.Rotation.X) + " " + MathHelper.RadiansToDegrees(msh.Rotation.Y) + " " + MathHelper.RadiansToDegrees(msh.Rotation.Z); // и значение
                                meshelement.AppendChild(meshSubElement6); // и указываем кому принадлежит

                                XmlNode meshSubElement8 = XML.CreateElement("Scale"); // даём имя
                                meshSubElement8.InnerText = msh.Scale.X + " " + msh.Scale.Y + " " + msh.Scale.Z; // и значение
                                meshelement.AppendChild(meshSubElement8); // и указываем кому принадлежит

                                XmlNode materialselement = XML.CreateElement("Materials");
                                meshelement.AppendChild(materialselement); // указываем родителя

                                foreach (MeshPart mpt in msh.Parts)
                                {
                                    XmlNode materialSubElement1 = XML.CreateElement("Material"); // даём имя
                                    materialSubElement1.InnerText = mpt.Material.Name; // и значение
                                    materialselement.AppendChild(materialSubElement1); // и указываем кому принадлежит
                                }
                            }
                }

                foreach (Light lgt in Lights.LIGHTS)
                {

                }

                XML.Save(XmlFile);
                #endregion
            }
            catch (Exception e)
            {
                //Log.WriteLineRed("Textures.LoadTexturesList() Exception.");
                //Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                //Log.WriteLineYellow(e.Message);
            }
        }

        private void EditorTopForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Glfw.SetWindowShouldClose(Game.Window, true);
        }

        private void materialBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TextureEditor НУЖНО ДОПИСАТЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //TextureEditor TextureEditorForm = new TextureEditor();
            //TextureEditorForm.Show();
        }

        private void lIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lIToolStripMenuItem.Checked == true)
                lightSettings.Show();
            else
                lightSettings.Hide();
        }

        private void meshBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (meshBrowserToolStripMenuItem.Checked == true)
                EditorObjectForm.Show();
            else
                EditorObjectForm.Hide();
        }

        private void EditorTopForm_Load(object sender, EventArgs e)
        {

        }

        private void EditorTopForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lightSettings.Close();
            EditorObjectForm.Close();
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            WriteMapData("D:/mappp.xml");
        }

    }
}
