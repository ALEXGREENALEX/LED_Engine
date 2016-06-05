using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace LED_Engine
{
    public partial class TextureEditor : Form
    {
        public static int TexturesListIndexCounter;


        public TextureEditor()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; //For fix parsing values like "0.5" and "0,5"

            MagFilterComboBox.Items.Add("Undefined");
            MinFilterComboBox.Items.Add("Undefined");

            foreach (TextureMagFilter items in Enum.GetValues(typeof(TextureMagFilter)))
            {
                MagFilterComboBox.Items.Add(items);
            }

            foreach (TextureMinFilter items in Enum.GetValues(typeof(TextureMinFilter)))
            {
                MinFilterComboBox.Items.Add(items);
            }  

            foreach (Texture element in Textures.TexturesList)
            {
                if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                    TexturesListBox.Items.Add(element.Name);
            }
        }

        public static void WriteTexturesList(string XmlFile, string XmlFileEngine)
        {
            try
            {
                XmlTextWriter textWritter = new XmlTextWriter(XmlFile, Encoding.UTF8);
                XmlTextWriter EngineTextWritter = new XmlTextWriter(XmlFileEngine, Encoding.UTF8);
                //textWritter.WriteStartDocument();
                textWritter.WriteStartElement("Textures");
                textWritter.WriteEndElement();
                textWritter.Close();

                EngineTextWritter.WriteStartElement("Textures");
                EngineTextWritter.WriteEndElement();
                EngineTextWritter.Close();

                XmlDocument XML = new XmlDocument();
                XmlDocument EngineXML = new XmlDocument();

                #region Write Textures List

                XML.Load(XmlFile);
                EngineXML.Load(XmlFileEngine);

                foreach (Texture tex in Textures.TexturesList)
                {
                    if (tex.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                    {
                        if (tex.EngineContent == false)
                        {
                            XmlNode element = XML.CreateElement("Texture");
                            XML.DocumentElement.AppendChild(element); // указываем родителя
                            //XmlAttribute attribute = XML.CreateAttribute("number"); // создаём атрибут
                            //attribute.Value = "1"; // устанавливаем значение атрибута
                            //element.Attributes.Append(attribute); // добавляем атрибут

                            XmlNode subElement1 = XML.CreateElement("Name"); // даём имя
                            subElement1.InnerText = tex.Name; // и значение
                            element.AppendChild(subElement1); // и указываем кому принадлежит

                            XmlNode subElement2 = XML.CreateElement("File"); // даём имя
                            string path = tex.File;
                            subElement2.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement2); // и указываем кому принадлежит
                        }

                        if (tex.EngineContent == true)
                        {
                            XmlNode element = EngineXML.CreateElement("Texture");
                            EngineXML.DocumentElement.AppendChild(element); // указываем родителя
                            //XmlAttribute attribute = XML.CreateAttribute("number"); // создаём атрибут
                            //attribute.Value = "1"; // устанавливаем значение атрибута
                            //element.Attributes.Append(attribute); // добавляем атрибут

                            XmlNode subElement1 = EngineXML.CreateElement("Name"); // даём имя
                            subElement1.InnerText = tex.Name; // и значение
                            element.AppendChild(subElement1); // и указываем кому принадлежит

                            XmlNode subElement2 = EngineXML.CreateElement("File"); // даём имя
                            string path = tex.File;
                            subElement2.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement2); // и указываем кому принадлежит
                        }
                    }
                }

                XML.Save(XmlFile);
                EngineXML.Save(XmlFileEngine);
                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLineRed("TextureEditor.WriteTexturesList() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }

        public static void WriteCubemapList(string XmlFile, string XmlFileEngine)
        {
            try
            {
                XmlTextWriter textWritter = new XmlTextWriter(XmlFile, Encoding.UTF8);
                XmlTextWriter EngineTextWritter = new XmlTextWriter(XmlFileEngine, Encoding.UTF8);

                //textWritter.WriteStartDocument();
                textWritter.WriteStartElement("CubemapTextures");
                textWritter.WriteEndElement();
                textWritter.Close();

                EngineTextWritter.WriteStartElement("CubemapTextures");
                EngineTextWritter.WriteEndElement();
                EngineTextWritter.Close();

                XmlDocument XML = new XmlDocument();
                XmlDocument EngineXML = new XmlDocument();

                #region Write CubeMap List

                XML.Load(XmlFile);
                EngineXML.Load(XmlFileEngine);

                foreach (Texture tex in Textures.TexturesList)
                {
                    if (tex.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.TextureCubeMap)
                    {
                        if (tex.EngineContent == false)
                        {
                            XmlNode element = XML.CreateElement("CubemapTexture");
                            XML.DocumentElement.AppendChild(element); // указываем родителя
                            //XmlAttribute attribute = XML.CreateAttribute("number"); // создаём атрибут
                            //attribute.Value = "1"; // устанавливаем значение атрибута
                            //element.Attributes.Append(attribute); // добавляем атрибут

                            XmlNode subElement1 = XML.CreateElement("Name"); // даём имя
                            subElement1.InnerText = tex.Name; // и значение
                            element.AppendChild(subElement1); // и указываем кому принадлежит

                            XmlNode subElement2 = XML.CreateElement("Dir"); // даём имя
                            string dir = System.IO.Path.GetDirectoryName(tex.CubemapFiles[0]);
                            subElement2.InnerText = System.IO.Path.GetFileName(dir); // и значение
                            element.AppendChild(subElement2); // и указываем кому принадлежит

                            XmlNode subElement3 = XML.CreateElement("File"); // даём имя
                            string path = tex.CubemapFiles[0];
                            subElement3.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement3); // и указываем кому принадлежит

                            XmlNode subElement4 = XML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[1];
                            subElement4.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement4); // и указываем кому принадлежит

                            XmlNode subElement5 = XML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[2];
                            subElement5.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement5); // и указываем кому принадлежит

                            XmlNode subElement6 = XML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[3];
                            subElement6.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement6); // и указываем кому принадлежит

                            XmlNode subElement7 = XML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[4];
                            subElement7.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement7); // и указываем кому принадлежит

                            XmlNode subElement8 = XML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[5];
                            subElement8.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement8); // и указываем кому принадлежит

                            ////////ДОПИСАТЬ!!!!!
                            XmlNode subElement9 = XML.CreateElement("SideSize"); // даём имя
                            subElement9.InnerText = ""; // и значение
                            element.AppendChild(subElement9); // и указываем кому принадлежит

                        }
                        if (tex.EngineContent == true)
                        {
                            XmlNode element = EngineXML.CreateElement("CubemapTexture");
                            EngineXML.DocumentElement.AppendChild(element); // указываем родителя
                            //XmlAttribute attribute = XML.CreateAttribute("number"); // создаём атрибут
                            //attribute.Value = "1"; // устанавливаем значение атрибута
                            //element.Attributes.Append(attribute); // добавляем атрибут

                            XmlNode subElement1 = EngineXML.CreateElement("Name"); // даём имя
                            subElement1.InnerText = tex.Name; // и значение
                            element.AppendChild(subElement1); // и указываем кому принадлежит

                            XmlNode subElement2 = EngineXML.CreateElement("Dir"); // даём имя
                            string dir = System.IO.Path.GetDirectoryName(tex.CubemapFiles[0]);
                            subElement2.InnerText = System.IO.Path.GetFileName(dir); // и значение
                            element.AppendChild(subElement2); // и указываем кому принадлежит

                            XmlNode subElement3 = EngineXML.CreateElement("File"); // даём имя
                            string path = tex.CubemapFiles[0];
                            subElement3.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement3); // и указываем кому принадлежит

                            XmlNode subElement4 = EngineXML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[1];
                            subElement4.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement4); // и указываем кому принадлежит

                            XmlNode subElement5 = EngineXML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[2];
                            subElement5.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement5); // и указываем кому принадлежит

                            XmlNode subElement6 = EngineXML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[3];
                            subElement6.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement6); // и указываем кому принадлежит

                            XmlNode subElement7 = EngineXML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[4];
                            subElement7.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement7); // и указываем кому принадлежит

                            XmlNode subElement8 = EngineXML.CreateElement("File"); // даём имя
                            path = tex.CubemapFiles[5];
                            subElement8.InnerText = System.IO.Path.GetFileName(path); // и значение
                            element.AppendChild(subElement8); // и указываем кому принадлежит

                            ////////ДОПИСАТЬ!!!!!
                            XmlNode subElement9 = EngineXML.CreateElement("SideSize"); // даём имя
                            subElement9.InnerText = ""; // и значение
                            element.AppendChild(subElement9); // и указываем кому принадлежит
                        }
                    }
                }

                XML.Save(XmlFile);
                EngineXML.Save(XmlFileEngine);
                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLineRed("TextureEditor.WriteCubemapList() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }

        private void TexturesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                {
                    #region Enable controls
                    NameTextBox.Enabled = true;
                    MagFilterComboBox.Enabled = true;
                    MinFilterComboBox.Enabled = true;
                    EngineCheckBox.Enabled = true;
                    ApplyButton.Enabled = true;
                    FileButton.Enabled = true;
                    FileTextBox.Enabled = true;
                    #endregion
                    try
                    {
                        NameTextBox.Text = Textures.TexturesList[TexturesListBox.SelectedIndex].Name;
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex].MagFilter.ToString() == String.Empty)
                            MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString("Undefined");
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex].MinFilter.ToString() == String.Empty)
                            MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString("Undefined");
                    }
                    catch
                    {

                    }

                    pictureBox1.Show();
                    string path = String.Empty;

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex].File;
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex].EngineContent == true)
                        {
                            FileTextBox.BackColor = Color.White;
                            FileTextBox.Text = Engine.FixPath(path.Substring(Settings.Paths.EngineTextures.Length + 1, path.Length - Settings.Paths.EngineTextures.Length - 1));
                        }
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex].EngineContent == false)
                        {
                            FileTextBox.BackColor = Color.White;
                            FileTextBox.Text = Engine.FixPath(path.Substring(Settings.Paths.Textures.Length + 1, path.Length - Settings.Paths.Textures.Length - 1));
                        }
                    }
                    catch
                    {
                        FileTextBox.BackColor = Color.Tomato;
                        FileTextBox.Text = "Error";
                        pictureBox1.Hide();
                    }
                    try
                    {
                        pictureBox1.Image = Image.FromFile(path);
                    }
                    catch
                    {
                        FileTextBox.BackColor = Color.Tomato;
                        FileTextBox.Text = "Error";
                        pictureBox1.Hide();
                    }
                    try
                    {
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex].EngineContent == true)
                            EngineCheckBox.Checked = true;
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex].EngineContent == false)
                            EngineCheckBox.Checked = false;
                    }
                    catch
                    {

                    }
                }

                if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                {
                    #region Enable controls
                    NameTextBox.Enabled = true;
                    MagFilterComboBox.Enabled = true;
                    MinFilterComboBox.Enabled = true;
                    EngineCheckBox.Enabled = true;
                    ApplyButton.Enabled = true;
                    CubeMapFileButton1.Enabled = true;
                    CubeMapFileTextBox1.Enabled = true;
                    CubeMapFileButton2.Enabled = true;
                    CubeMapFileTextBox2.Enabled = true;
                    CubeMapFileButton3.Enabled = true;
                    CubeMapFileTextBox3.Enabled = true;
                    CubeMapFileButton4.Enabled = true;
                    CubeMapFileTextBox4.Enabled = true;
                    CubeMapFileButton5.Enabled = true;
                    CubeMapFileTextBox5.Enabled = true;
                    CubeMapFileButton6.Enabled = true;
                    CubeMapFileTextBox6.Enabled = true;
                    #endregion
                    string path = String.Empty;
                    NameTextBox.Text = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name;

                    pictureBox1.Show();

                    try
                    {
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString() == String.Empty)
                            MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString("Undefined");
                        if (Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString() == String.Empty)
                            MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString("Undefined");
                    }
                    catch
                    {

                    }

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0];
                        CubeMapFileTextBox1.BackColor = Color.White;
                        CubeMapFileTextBox1.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                    }
                    catch
                    {
                        CubeMapFileTextBox1.BackColor = Color.Tomato;
                        CubeMapFileTextBox1.Text = "Error";
                    }

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1];
                        CubeMapFileTextBox2.BackColor = Color.White;
                        CubeMapFileTextBox2.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                    }
                    catch
                    {
                        CubeMapFileTextBox2.BackColor = Color.Tomato;
                        CubeMapFileTextBox2.Text = "Error";
                    }

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2];
                        CubeMapFileTextBox3.BackColor = Color.White;
                        CubeMapFileTextBox3.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                    }
                    catch
                    {
                        CubeMapFileTextBox3.BackColor = Color.Tomato;
                        CubeMapFileTextBox3.Text = "Error";
                    }

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3];
                        CubeMapFileTextBox4.BackColor = Color.White;
                        CubeMapFileTextBox4.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                    }
                    catch
                    {
                        CubeMapFileTextBox4.BackColor = Color.Tomato;
                        CubeMapFileTextBox4.Text = "Error";
                    }

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4];
                        CubeMapFileTextBox5.BackColor = Color.White;
                        CubeMapFileTextBox5.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                    }
                    catch
                    {
                        CubeMapFileTextBox5.BackColor = Color.Tomato;
                        CubeMapFileTextBox5.Text = "Error";
                    }

                    try
                    {
                        path = Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5];
                        CubeMapFileTextBox6.BackColor = Color.White;
                        CubeMapFileTextBox6.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                    }
                    catch
                    {
                        CubeMapFileTextBox6.BackColor = Color.Tomato;
                        CubeMapFileTextBox6.Text = "Error";
                    }

                    try
                    {
                        pictureBox1.Image = Image.FromFile(path);
                        Game.SkyBox.Free();
                        Game.SkyBox = new Mesh();
                        MeshPart MPart = MeshPart.MakeBox(Game.MainCamera.zFar * 2.0f / (float)Math.Sqrt(3.0), true); // Cube diagonal = side / sqrt(3)
                        MPart.Material = Materials.Load(Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name);
                        Game.SkyBox.Parts.Add(MPart);
                        Game.SkyBox.CalcBoundingObjects();
                    }
                    catch
                    {

                    }
                    if (Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent == true)
                        EngineCheckBox.Checked = true;
                    if (Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent == false)
                        EngineCheckBox.Checked = false;
                }
                try
                {
                    foreach (var item in MagFilterComboBox.Items)
                    {
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                        {
                            if (Textures.TexturesList[TexturesListBox.SelectedIndex].MagFilter.ToString() != String.Empty)
                            {
                                if (item.ToString() == Textures.TexturesList[TexturesListBox.SelectedIndex].MagFilter.ToString())
                                {
                                    MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString(Textures.TexturesList[TexturesListBox.SelectedIndex].MagFilter.ToString());
                                }
                            }
                        }
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                        {
                            if (Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString() != String.Empty)
                            {
                                if (item.ToString() == Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString())
                                {
                                    MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString(Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString());
                                }
                            }
                        }
                    }

                    foreach (var item in MinFilterComboBox.Items)
                    {
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                        {
                            if (Textures.TexturesList[TexturesListBox.SelectedIndex].MinFilter.ToString() != String.Empty)
                            {
                                if (item.ToString() == Textures.TexturesList[TexturesListBox.SelectedIndex].MinFilter.ToString())
                                {
                                    MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString(Textures.TexturesList[TexturesListBox.SelectedIndex].MinFilter.ToString());
                                }
                            }
                        }
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                        {
                            if (Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString() != String.Empty)
                            {
                                if (item.ToString() == Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString())
                                {
                                    MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString(Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString());
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //Log.WriteLineRed("Texture Load Error.");
                }
            }
            catch
            {
                //Log.WriteLineRed("Texture Load Error.");
            }
        }

        private void TextureEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            TexturesListBox.Items.Clear();
            MagFilterComboBox.Items.Clear();
            MinFilterComboBox.Items.Clear();
            FileTextBox.Clear();
        }

        private void MaterialTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Disable controls
            NameTextBox.Enabled = false;
            MagFilterComboBox.Enabled = false;
            MinFilterComboBox.Enabled = false;
            EngineCheckBox.Enabled = false;
            ApplyButton.Enabled = false;
            FileButton.Enabled = false;
            FileTextBox.Enabled = false;
            CubeMapFileButton1.Enabled = false;
            CubeMapFileTextBox1.Enabled = false;
            CubeMapFileButton2.Enabled = false;
            CubeMapFileTextBox2.Enabled = false;
            CubeMapFileButton3.Enabled = false;
            CubeMapFileTextBox3.Enabled = false;
            CubeMapFileButton4.Enabled = false;
            CubeMapFileTextBox4.Enabled = false;
            CubeMapFileButton5.Enabled = false;
            CubeMapFileTextBox5.Enabled = false;
            CubeMapFileButton6.Enabled = false;
            CubeMapFileTextBox6.Enabled = false;

            #endregion
            EngineCheckBox.Checked = false;
            NameTextBox.Clear();
            this.MinimumSize = new System.Drawing.Size(0, 0);
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                this.Size = new System.Drawing.Size(798, 482);
                this.MinimumSize = new System.Drawing.Size(798, 482);

                TextureFilePanel.Show();
                CubemapTexturePanel.Hide();
                TexturesListBox.Items.Clear();
                MagFilterComboBox.SelectedIndex = -1;
                MinFilterComboBox.SelectedIndex = -1;
                FileTextBox.Clear();
                pictureBox1.Hide();
                foreach (Texture element in Textures.TexturesList)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                        TexturesListBox.Items.Add(element.Name);
                }
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                this.Size = new System.Drawing.Size(798, 604);
                this.MinimumSize = new System.Drawing.Size(798, 604);

                TexturesListIndexCounter = TexturesListBox.Items.Count;
                TextureFilePanel.Hide();
                CubemapTexturePanel.Show();
                TexturesListBox.Items.Clear();
                MagFilterComboBox.SelectedIndex = -1;
                MinFilterComboBox.SelectedIndex = -1;
                FileTextBox.Clear();
                pictureBox1.Hide();
                foreach (Texture element in Textures.TexturesList)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.TextureCubeMap)
                        TexturesListBox.Items.Add(element.Name);
                }

            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            WriteTexturesList("D:/textures.xml", "D:/enginetextures.xml");
            WriteCubemapList("D:/cubemaps.xml", "D:/enginecubemaps.xml");
        }

        private void TexturesAddButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                Texture newtexture = new Texture();
                newtexture.TextureTarget = Pencil.Gaming.Graphics.TextureTarget.Texture2D;
                Textures.TexturesList.Insert(TexturesListBox.Items.Count, newtexture);
                TexturesListBox.Items.Add(newtexture.Name);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                Texture newtexture = new Texture();
                newtexture.TextureTarget = Pencil.Gaming.Graphics.TextureTarget.TextureCubeMap;
                Textures.TexturesList.Add(newtexture);
                TexturesListBox.Items.Add(newtexture.Name);
            }
        }

        private void TexturesDeleteButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                Textures.TexturesList.RemoveAt(TexturesListBox.SelectedIndex);
                TexturesListBox.Items.RemoveAt(TexturesListBox.SelectedIndex);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                Textures.TexturesList.RemoveAt(TexturesListBox.SelectedIndex + TexturesListIndexCounter);
                TexturesListBox.Items.RemoveAt(TexturesListBox.SelectedIndex);
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                Textures.TexturesList[TexturesListBox.SelectedIndex].Name = NameTextBox.Text;

                foreach (Pencil.Gaming.Graphics.TextureMagFilter items in Enum.GetValues(typeof(TextureMagFilter)))
                {
                    if (items.ToString() == MagFilterComboBox.SelectedItem.ToString())
                    {
                        Textures.TexturesList[TexturesListBox.SelectedIndex].MagFilter = items;
                    }
                }

                foreach (Pencil.Gaming.Graphics.TextureMinFilter items in Enum.GetValues(typeof(TextureMinFilter)))
                {
                    if (items.ToString() == MinFilterComboBox.SelectedItem.ToString())
                    {
                        Textures.TexturesList[TexturesListBox.SelectedIndex].MinFilter = items;
                    }
                }

                if (EngineCheckBox.Checked == true)
                {
                    Textures.TexturesList[TexturesListBox.SelectedIndex].EngineContent = true;
                    Textures.TexturesList[TexturesListBox.SelectedIndex].File = Engine.CombinePaths(Settings.Paths.EngineTextures, FileTextBox.Text);
                }
                if (EngineCheckBox.Checked == false)
                {
                    Textures.TexturesList[TexturesListBox.SelectedIndex].EngineContent = false;
                    Textures.TexturesList[TexturesListBox.SelectedIndex].File = Engine.CombinePaths(Settings.Paths.Textures, FileTextBox.Text);
                }

                int tempind = TexturesListBox.SelectedIndex;
                TexturesListBox.Items.Clear();
                foreach (Texture element in Textures.TexturesList)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                        TexturesListBox.Items.Add(element.Name);
                }
                TexturesListBox.SelectedIndex = tempind;
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name = NameTextBox.Text;

                foreach (Pencil.Gaming.Graphics.TextureMagFilter items in Enum.GetValues(typeof(TextureMagFilter)))
                {
                    if (items.ToString() == MagFilterComboBox.SelectedItem.ToString())
                    {
                        Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter = items;
                    }
                }

                foreach (Pencil.Gaming.Graphics.TextureMinFilter items in Enum.GetValues(typeof(TextureMinFilter)))
                {
                    if (items.ToString() == MinFilterComboBox.SelectedItem.ToString())
                    {
                        Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter = items;
                    }
                }

                if (EngineCheckBox.Checked == true)
                {
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent = true;
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0] = Engine.CombinePaths(Settings.Paths.EngineCubemapTextures, CubeMapFileTextBox1.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1] = Engine.CombinePaths(Settings.Paths.EngineCubemapTextures, CubeMapFileTextBox2.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2] = Engine.CombinePaths(Settings.Paths.EngineCubemapTextures, CubeMapFileTextBox3.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3] = Engine.CombinePaths(Settings.Paths.EngineCubemapTextures, CubeMapFileTextBox4.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4] = Engine.CombinePaths(Settings.Paths.EngineCubemapTextures, CubeMapFileTextBox5.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5] = Engine.CombinePaths(Settings.Paths.EngineCubemapTextures, CubeMapFileTextBox6.Text);
                }
                if (EngineCheckBox.Checked == false)
                {
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent = false;
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0] = Engine.CombinePaths(Settings.Paths.CubemapTextures, CubeMapFileTextBox1.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1] = Engine.CombinePaths(Settings.Paths.CubemapTextures, CubeMapFileTextBox2.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2] = Engine.CombinePaths(Settings.Paths.CubemapTextures, CubeMapFileTextBox3.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3] = Engine.CombinePaths(Settings.Paths.CubemapTextures, CubeMapFileTextBox4.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4] = Engine.CombinePaths(Settings.Paths.CubemapTextures, CubeMapFileTextBox5.Text);
                    Textures.TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5] = Engine.CombinePaths(Settings.Paths.CubemapTextures, CubeMapFileTextBox6.Text);
                }
                int tempind = TexturesListBox.SelectedIndex;
                TexturesListBox.Items.Clear();
                foreach (Texture element in Textures.TexturesList)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.TextureCubeMap)
                        TexturesListBox.Items.Add(element.Name);
                }
                TexturesListBox.SelectedIndex = tempind;
            }
        }
    }
}
