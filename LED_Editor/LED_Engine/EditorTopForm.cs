﻿using System;
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
        public TextureEditor TextureEditorForm = new TextureEditor();

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
                        meshSubElement2.InnerText = "Mesh"; // и значение
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
                    XmlNode Light = XML.CreateElement("Light");
                    XML.DocumentElement.AppendChild(Light); // указываем родителя

                    XmlNode LightName = XML.CreateElement("Name"); // даём имя
                    LightName.InnerText = lgt.Name; // и значение
                    Light.AppendChild(LightName); // и указываем кому принадлежит

                    XmlNode LightTp = XML.CreateElement("Type"); // даём имя
                    LightTp.InnerText = lgt.Type.ToString(); // и значение
                    Light.AppendChild(LightTp); // и указываем кому принадлежит

                    XmlNode LightEnabled = XML.CreateElement("Enabled"); // даём имя
                    LightEnabled.InnerText = lgt.Enabled.ToString(); // и значение
                    Light.AppendChild(LightEnabled); // и указываем кому принадлежит

                    XmlNode LightDiffuse = XML.CreateElement("Diffuse"); // даём имя
                    LightDiffuse.InnerText = lgt.Diffuse.X + " " + lgt.Diffuse.Y + " " + lgt.Diffuse.Z; // и значение
                    Light.AppendChild(LightDiffuse); // и указываем кому принадлежит

                    switch (lgt.Type)
                    {
                        case LightType.Directional:
                            XmlNode LightSpec = XML.CreateElement("Specular"); // даём имя
                            LightSpec.InnerText = lgt.Specular.X + " " + lgt.Specular.Y + " " + lgt.Specular.Z; // и значение
                            Light.AppendChild(LightSpec); // и указываем кому принадлежит
                            XmlNode LightDir = XML.CreateElement("Direction"); // даём имя
                            LightDir.InnerText = MathHelper.RadiansToDegrees(lgt.Direction.ExtractYawPitch().X) + " " + MathHelper.RadiansToDegrees(lgt.Direction.ExtractYawPitch().Y); // и значение
                            Light.AppendChild(LightDir); // и указываем кому принадлежит
                            break;
                        case LightType.Point:
                            XmlNode LightSpec2 = XML.CreateElement("Specular"); // даём имя
                            LightSpec2.InnerText = lgt.Specular.X + " " + lgt.Specular.Y + " " + lgt.Specular.Z; // и значение
                            Light.AppendChild(LightSpec2); // и указываем кому принадлежит
                            XmlNode LightPos = XML.CreateElement("Position"); // даём имя
                            LightPos.InnerText = lgt.Position.X + " " + lgt.Position.Y + " " + lgt.Position.Z; // и значение
                            Light.AppendChild(LightPos); // и указываем кому принадлежит
                            XmlNode LightAtt = XML.CreateElement("Attenuation"); // даём имя
                            LightAtt.InnerText = lgt.Attenuation.X + " " + lgt.Attenuation.Y + " " + lgt.Attenuation.Z; // и значение
                            Light.AppendChild(LightAtt); // и указываем кому принадлежит
                            break;
                        case LightType.Spot:
                            XmlNode LightSpec3 = XML.CreateElement("Specular"); // даём имя
                            LightSpec3.InnerText = lgt.Specular.X + " " + lgt.Specular.Y + " " + lgt.Specular.Z; // и значение
                            Light.AppendChild(LightSpec3); // и указываем кому принадлежит
                            XmlNode LightPos2 = XML.CreateElement("Position"); // даём имя
                            LightPos2.InnerText = lgt.Position.X + " " + lgt.Position.Y + " " + lgt.Position.Z; // и значение
                            Light.AppendChild(LightPos2); // и указываем кому принадлежит
                            XmlNode LightDir2 = XML.CreateElement("Direction"); // даём имя
                            LightDir2.InnerText = MathHelper.RadiansToDegrees(lgt.Direction.ExtractYawPitch().X) + " " + MathHelper.RadiansToDegrees(lgt.Direction.ExtractYawPitch().Y); // и значение
                            Light.AppendChild(LightDir2); // и указываем кому принадлежит
                            XmlNode LightAtt2 = XML.CreateElement("Attenuation"); // даём имя
                            LightAtt2.InnerText = lgt.Attenuation.X + " " + lgt.Attenuation.Y + " " + lgt.Attenuation.Z; // и значение
                            Light.AppendChild(LightAtt2); // и указываем кому принадлежит
                            XmlNode LightExp = XML.CreateElement("Exponent"); // даём имя
                            LightExp.InnerText = lgt.Exponent.ToString(); // и значение
                            Light.AppendChild(LightExp); // и указываем кому принадлежит
                            XmlNode LightCOFF = XML.CreateElement("CutOFF"); // даём имя
                            LightCOFF.InnerText = lgt.CutOFF.ToString(); // и значение
                            Light.AppendChild(LightCOFF); // и указываем кому принадлежит
                            break;
                    }
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
            if (materialBrowserToolStripMenuItem.Checked == true)
                TextureEditorForm.Show();
            else
                TextureEditorForm.Hide();
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
            MapNameTextBox.Text = Maps.Name;
        }

        private void EditorTopForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lightSettings.Close();
            EditorObjectForm.Close();
            TextureEditorForm.Close();
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            Maps.Name = MapNameTextBox.Text;
            WriteMapData(Engine.FixPath(Settings.Paths.Maps + "/" + Maps.Name + ".xml"));
        }

        private void MapNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            XmlDocument XML = new XmlDocument();

            try
            {
                XML.Load(openFileDialog1.FileName);
                string Name = XML.DocumentElement.SelectSingleNode("Name").InnerText;
                MapNameTextBox.Text = Name;
                if ((Name != null) && !Maps.MapsList.ContainsKey(Name))
                {
                    Maps.MapsList.Add(Name, openFileDialog1.FileName);
                    Maps.Free();
                    Maps.LoadMap(Name);
                }
                else
                {
                    Maps.Free();
                    Maps.LoadMap(Name);
                }
                lightSettings.ListsRefresh();
            }

            catch (Exception e1)
            {
                Log.WriteLineRed("Maps.LoadMapList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\"", openFileDialog1.FileName);
                Log.WriteLineYellow("Maybe some XML file in Map directory is not a valid map.");
                Log.WriteLineYellow(e1.Message);
            }
            
            //MessageBox.Show("Chosen file: " + openFileDialog1.FileName);
        }

        private void NewFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.File.Delete(Settings.Paths.Maps + "/New_map.xml");
            }
            catch
            {

            }

            System.IO.File.Copy(Settings.Paths.EngineMaps + "/Default/New_map.xml", Settings.Paths.Maps + "/New_map.xml");

            XmlDocument XML = new XmlDocument();

            try
            {
                XML.Load(Settings.Paths.Maps + "/New_map.xml");
                string Name = XML.DocumentElement.SelectSingleNode("Name").InnerText;
                MapNameTextBox.Text = Name;
                if ((Name != null) && !Maps.MapsList.ContainsKey(Name))
                {
                    Maps.MapsList.Add(Name, openFileDialog1.FileName);
                    Maps.Free();
                    Maps.LoadMap(Name);
                }
                else
                {
                    Maps.Free();
                    Maps.LoadMap(Name);
                }
                lightSettings.ListsRefresh();
            }

            catch (Exception e1)
            {
                Log.WriteLineRed("Maps.LoadMapList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\"", openFileDialog1.FileName);
                Log.WriteLineYellow("Maybe some XML file in Map directory is not a valid map.");
                Log.WriteLineYellow(e1.Message);
            }
        }

    }
}
