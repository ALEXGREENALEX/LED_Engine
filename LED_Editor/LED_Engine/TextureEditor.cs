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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace LED_Engine
{
    public partial class TextureEditor : Form
    {
        public static List<Texture> TexturesListParse = new List<Texture>(); // All Textures
        public static int TexturesListIndexCounter;
        public static string EnginePath;
        public static string TexturePath;
        public static string EngineTexturePath;
        public static string TextureXMLPath;
        public static string EngineTextureXMLPath;
        public static string CubeMapPath;
        public static string EngineCubeMapPath;
        public static string CubeMapXMLPath;
        public static string EngineCubeMapXMLPath;
        public static string CutTexturePath;
        public static string CutEngineTexturePath;
        public static string CutCubeMapPath;
        public static string CutEngineCubeMapPath;
        public static string MaterialXMLPath;
        public static string EngineMaterialXMLPath;
        public static string ShaderXMLPath;
        public static string EngineShaderPath;
        public static string EngineShaderXMLPath;
        public static uint OpenGLTexture;
        public static string OpenGLPath;
        public static int id; //GLtexture id

        public TextureEditor()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; //For fix parsing values like "0.5" and "0,5"

            TexturesListParse = Textures.GetTexturesList();
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

            /////////////////////////////ПЕРЕПИСАТЬ! ИСПОЛЬЗУЕТСЯ ТОЛЬКО ДЛЯ ТЕСТОВ
            EnginePath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData";
            TexturePath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Textures";
            EngineTexturePath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Textures";
            CubeMapPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Cubemaps";
            EngineCubeMapPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Cubemaps";
            TextureXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Textures.xml";
            EngineTextureXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Textures.xml";
            CubeMapXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Cubemaps.xml";
            EngineCubeMapXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Cubemaps.xml";
            OpenGLPath = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Textures\brick-wall.jpg";
            MaterialXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Materials.xml";
            EngineMaterialXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Materials.xml";
            ShaderXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Shaders.xml";
            EngineShaderXMLPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Shaders.xml";
            EngineShaderPath = "D:/KTU/Диплом/game/led_engine/LED_Engine/LED_Engine/GameData/Engine/Shaders";

            CutTexturePath = TexturePath.TrimEnd(new char[] { '\\', '/' });
            CutEngineTexturePath = EngineTexturePath.TrimEnd(new char[] { '\\', '/' });
            CutTexturePath = CubeMapPath.TrimEnd(new char[] { '\\', '/' });
            CutEngineTexturePath = EngineCubeMapPath.TrimEnd(new char[] { '\\', '/' });

            Engine.GetGLSettings();


            ////Загружаем engine текстуры
            //Textures.LoadTexturesList(EngineTextureXMLPath, EngineTexturePath, true);

            ////Загружаем НЕ engine текстуры
            //Textures.LoadTexturesList(TextureXMLPath, TexturePath, false);

            ////Загружаем engine cubemap-текстуры
            //Textures.LoadCubemapTexturesList(EngineCubeMapXMLPath, EngineCubeMapPath, true);

            ////Загружаем НЕ engine cubemap-текстуры
            //Textures.LoadCubemapTexturesList(CubeMapXMLPath, CubeMapPath, false);

            //Shaders.LoadShadersList(EngineShaderXMLPath, EngineShaderPath, true);

            //Materials.LoadMaterialsList(EngineMaterialXMLPath, true);
            //Materials.LoadMaterialsList(MaterialXMLPath, false);

            foreach (Texture element in TexturesListParse)
            {
                if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                    TexturesListBox.Items.Add(element.Name);
            }
        }

        #region OpenGL Stuff
        private void SetupViewport()
        {
            int w = TexturesGLControl.Width;
            int h = TexturesGLControl.Height;

            MainCamera.SetProjectionMatrix(ProjectionTypes.Perspective, w, h, zNear, zFar, FOV);
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private void TexturesGLControl_Resize(object sender, EventArgs e)
        {
            SetupViewport();
            TexturesGLControl.Invalidate();
        }

        private void TexturesGLControl_Load(object sender, EventArgs e)
        {
            //TEMPLATE
            CubemapFiles[0] = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Cubemaps\Storforsen\posx.jpg";
            CubemapFiles[1] = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Cubemaps\Storforsen\negx.jpg";
            CubemapFiles[2] = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Cubemaps\Storforsen\posy.jpg";
            CubemapFiles[3] = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Cubemaps\Storforsen\negy.jpg";
            CubemapFiles[4] = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Cubemaps\Storforsen\posz.jpg";
            CubemapFiles[5] = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Cubemaps\Storforsen\negz.jpg";
            TextureFile = @"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Textures\brick-wall.jpg";


            TexturesGLControl.MakeCurrent();
            GL.ClearColor(Color.White);

            Shader1.LoadShader();
            Material1.Shader = Shader1;

            //TexPlain = Mesh.MakePlain(TexturesGLControl.Width, TexturesGLControl.Height);
            TexPlain = Mesh.MakePlain(438, 383);
            TexPlain.CalculateMatrices(MainCamera);
            TexPlain.Position.Z = -410.0f;
            TexPlain.Rotation = new Vector3((float)Math.PI / 2f, 0.0f, 0.0f);

            MainCamera.Orientation = new Vector3((float)Math.PI, 0f, 0f);
            Material1.TextureID = Load_BlankTexture2D();
            TexPlain.Material = Material1;

            SkyCube = Mesh.MakeBox(zFar * 2.0f / (float)Math.Sqrt(3.0), true);
            SkyCube.CalculateMatrices(MainCamera);

            SetupViewport();
        }

        private void TexturesGLControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.CullFace);

            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                DrawObject(TexPlain);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                DrawObject(SkyCube);
            }
            TexturesGLControl.SwapBuffers();
        }

        #endregion

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

                foreach (Texture tex in TexturesListParse)
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
                //Log.WriteLineRed("Textures.LoadTexturesList() Exception.");
                //Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                //Log.WriteLineYellow(e.Message);
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

                foreach (Texture tex in TexturesListParse)
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
                //Log.WriteLineRed("Textures.LoadTexturesList() Exception.");
                //Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                //Log.WriteLineYellow(e.Message);
            }
        }

        private void TexturesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MainCamera.Orientation = new Vector3((float)Math.PI, 0f, 0f);
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
                        NameTextBox.Text = TexturesListParse[TexturesListBox.SelectedIndex].Name;
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMagFilter == true)
                            MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString("Undefined");
                        if (TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMinFilter == true)
                            MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString("Undefined");
                    }
                    catch
                    {

                    }

                    TexturesGLControl.Show();
                    string path = String.Empty;

                    try
                    {
                        path = TexturesListParse[TexturesListBox.SelectedIndex].File;
                        OpenGLPath = path;
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (TexturesListParse[TexturesListBox.SelectedIndex].EngineContent == true)
                        {
                            FileTextBox.BackColor = Color.White;
                            FileTextBox.Text = Engine.FixPath(path.Substring(CutEngineTexturePath.Length + 1, path.Length - CutEngineTexturePath.Length - 1));
                        }
                        if (TexturesListParse[TexturesListBox.SelectedIndex].EngineContent == false)
                        {
                            FileTextBox.BackColor = Color.White;
                            FileTextBox.Text = Engine.FixPath(path.Substring(CutTexturePath.Length + 1, path.Length - CutTexturePath.Length - 1));
                        }
                    }
                    catch
                    {
                        FileTextBox.BackColor = Color.Tomato;
                        FileTextBox.Text = "Error";
                        TexturesGLControl.Hide();
                    }
                    try
                    {
                        MainCamera.Orientation = new Vector3((float)Math.PI, 0f, 0f);
                        Material1.TextureID = Load_Texture2D(path);
                        TexPlain.Material = Material1;
                        TexturesGLControl.Invalidate();
                    }
                    catch
                    {
                        FileTextBox.BackColor = Color.Tomato;
                        FileTextBox.Text = "Error";
                        TexturesGLControl.Hide();
                    }
                    try
                    {
                        if (TexturesListParse[TexturesListBox.SelectedIndex].EngineContent == true)
                            EngineCheckBox.Checked = true;
                        if (TexturesListParse[TexturesListBox.SelectedIndex].EngineContent == false)
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
                    NameTextBox.Text = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name;

                    TexturesGLControl.Show();

                    try
                    {
                        if (TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMagFilter == true)
                            MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString("Undefined");
                        if (TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMinFilter == true)
                            MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString("Undefined");
                    }
                    catch
                    {

                    }

                    try
                    {
                        path = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0];
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
                        path = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1];
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
                        path = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2];
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
                        path = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3];
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
                        path = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4];
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
                        path = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5];
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
                        MainCamera.Orientation = new Vector3((float)Math.PI, 0f, 0f);
                        CubemapFiles = TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles;
                        Material1.TextureID = Load_TextureCubemap();
                        SkyCube.Material = Material1;
                        TexturesGLControl.Invalidate();
                    }
                    catch
                    {

                    }
                    if (TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent == true)
                        EngineCheckBox.Checked = true;
                    if (TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent == false)
                        EngineCheckBox.Checked = false;
                }
                try
                {
                    foreach (var item in MagFilterComboBox.Items)
                    {
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                        {
                            if (TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMagFilter == false)
                            {
                                if (item.ToString() == TexturesListParse[TexturesListBox.SelectedIndex].MagFilter.ToString())
                                {
                                    MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString(TexturesListParse[TexturesListBox.SelectedIndex].MagFilter.ToString());
                                }
                            }
                        }
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                        {
                            if (TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMagFilter == false)
                            {
                                if (item.ToString() == TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString())
                                {
                                    MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString(TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString());
                                }
                            }
                        }
                    }

                    foreach (var item in MinFilterComboBox.Items)
                    {
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                        {
                            if (TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMinFilter == false)
                            {
                                if (item.ToString() == TexturesListParse[TexturesListBox.SelectedIndex].MinFilter.ToString())
                                {
                                    MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString(TexturesListParse[TexturesListBox.SelectedIndex].MinFilter.ToString());
                                }
                            }
                        }
                        if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                        {
                            if (TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMinFilter == false)
                            {
                                if (item.ToString() == TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString())
                                {
                                    MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString(TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString());
                                }
                            }
                        }
                    }
                }
                catch
                {
                    
                }
            }
            catch
            {

            }
        }

        private void TextureEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            TexturesListParse.Clear();
            TexturesListBox.Items.Clear();
            MagFilterComboBox.Items.Clear();
            MinFilterComboBox.Items.Clear();
            FileTextBox.Clear();
            GL.DeleteTexture(Material1.TextureID);
            Shader1.Free();
            SkyCube.Free();
            TexPlain.Free();
            TexturesGLControl.Dispose();
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
                //TexturesGLControl.MakeCurrent();
                this.Size = new System.Drawing.Size(798, 482);
                this.MinimumSize = new System.Drawing.Size(798, 482);

                TextureFilePanel.Show();
                CubemapTexturePanel.Hide();
                TexturesListBox.Items.Clear();
                MagFilterComboBox.SelectedIndex = -1;
                MinFilterComboBox.SelectedIndex = -1;
                FileTextBox.Clear();
                TexturesGLControl.Hide();
                foreach (Texture element in TexturesListParse)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                        TexturesListBox.Items.Add(element.Name);
                }
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                //CubeMapsGLControl.MakeCurrent();
                this.Size = new System.Drawing.Size(798, 604);
                this.MinimumSize = new System.Drawing.Size(798, 604);

                TexturesListIndexCounter = TexturesListBox.Items.Count;
                TextureFilePanel.Hide();
                CubemapTexturePanel.Show();
                TexturesListBox.Items.Clear();
                MagFilterComboBox.SelectedIndex = -1;
                MinFilterComboBox.SelectedIndex = -1;
                FileTextBox.Clear();
                TexturesGLControl.Hide();
                foreach (Texture element in TexturesListParse)
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
                TexturesListParse.Insert(TexturesListBox.Items.Count, newtexture);
                TexturesListBox.Items.Add(newtexture.Name);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                Texture newtexture = new Texture();
                newtexture.TextureTarget = Pencil.Gaming.Graphics.TextureTarget.TextureCubeMap;
                TexturesListParse.Add(newtexture);
                TexturesListBox.Items.Add(newtexture.Name);
            }
        }

        private void TexturesDeleteButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                TexturesListParse.RemoveAt(TexturesListBox.SelectedIndex);
                TexturesListBox.Items.RemoveAt(TexturesListBox.SelectedIndex);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                TexturesListParse.RemoveAt(TexturesListBox.SelectedIndex + TexturesListIndexCounter);
                TexturesListBox.Items.RemoveAt(TexturesListBox.SelectedIndex);
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                TexturesListParse[TexturesListBox.SelectedIndex].Name = NameTextBox.Text;

                if (MagFilterComboBox.SelectedItem.ToString() == "Undefined")
                    TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMagFilter = true;
                if (MinFilterComboBox.SelectedItem.ToString() == "Undefined")
                    TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMinFilter = true;

                foreach (Pencil.Gaming.Graphics.TextureMagFilter items in Enum.GetValues(typeof(TextureMagFilter)))
                {
                    if (items.ToString() == MagFilterComboBox.SelectedItem.ToString())
                    {
                        TexturesListParse[TexturesListBox.SelectedIndex].MagFilter = items;
                        TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMagFilter = false;
                    }
                }

                foreach (Pencil.Gaming.Graphics.TextureMinFilter items in Enum.GetValues(typeof(TextureMinFilter)))
                {
                    if (items.ToString() == MinFilterComboBox.SelectedItem.ToString())
                    {
                        TexturesListParse[TexturesListBox.SelectedIndex].MinFilter = items;
                        TexturesListParse[TexturesListBox.SelectedIndex].UndefinedMinFilter = false;
                    }
                }

                if (EngineCheckBox.Checked == true)
                {
                    TexturesListParse[TexturesListBox.SelectedIndex].EngineContent = true;
                    TexturesListParse[TexturesListBox.SelectedIndex].File = Engine.CombinePaths(EngineTexturePath, FileTextBox.Text);
                }
                if (EngineCheckBox.Checked == false)
                {
                    TexturesListParse[TexturesListBox.SelectedIndex].EngineContent = false;
                    TexturesListParse[TexturesListBox.SelectedIndex].File = Engine.CombinePaths(TexturePath, FileTextBox.Text);
                }

                int tempind = TexturesListBox.SelectedIndex;
                TexturesListBox.Items.Clear();
                foreach (Texture element in TexturesListParse)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.Texture2D)
                        TexturesListBox.Items.Add(element.Name);
                }
                TexturesListBox.SelectedIndex = tempind;
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name = NameTextBox.Text;

                if (MagFilterComboBox.SelectedItem.ToString() == "Undefined")
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMagFilter = true;
                if (MinFilterComboBox.SelectedItem.ToString() == "Undefined")
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMinFilter = true;

                foreach (Pencil.Gaming.Graphics.TextureMagFilter items in Enum.GetValues(typeof(TextureMagFilter)))
                {
                    if (items.ToString() == MagFilterComboBox.SelectedItem.ToString())
                    {
                        TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter = items;
                        TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMagFilter = false;
                    }
                }

                foreach (Pencil.Gaming.Graphics.TextureMinFilter items in Enum.GetValues(typeof(TextureMinFilter)))
                {
                    if (items.ToString() == MinFilterComboBox.SelectedItem.ToString())
                    {
                        TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter = items;
                        TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].UndefinedMinFilter = false;
                    }
                }

                if (EngineCheckBox.Checked == true)
                {
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent = true;
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0] = Engine.CombinePaths(EngineCubeMapPath, CubeMapFileTextBox1.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1] = Engine.CombinePaths(EngineCubeMapPath, CubeMapFileTextBox2.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2] = Engine.CombinePaths(EngineCubeMapPath, CubeMapFileTextBox3.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3] = Engine.CombinePaths(EngineCubeMapPath, CubeMapFileTextBox4.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4] = Engine.CombinePaths(EngineCubeMapPath, CubeMapFileTextBox5.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5] = Engine.CombinePaths(EngineCubeMapPath, CubeMapFileTextBox6.Text);
                }
                if (EngineCheckBox.Checked == false)
                {
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent = false;
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0] = Engine.CombinePaths(CubeMapPath, CubeMapFileTextBox1.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1] = Engine.CombinePaths(CubeMapPath, CubeMapFileTextBox2.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2] = Engine.CombinePaths(CubeMapPath, CubeMapFileTextBox3.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3] = Engine.CombinePaths(CubeMapPath, CubeMapFileTextBox4.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4] = Engine.CombinePaths(CubeMapPath, CubeMapFileTextBox5.Text);
                    TexturesListParse[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5] = Engine.CombinePaths(CubeMapPath, CubeMapFileTextBox6.Text);
                }
                int tempind = TexturesListBox.SelectedIndex;
                TexturesListBox.Items.Clear();
                foreach (Texture element in TexturesListParse)
                {
                    if (element.TextureTarget == Pencil.Gaming.Graphics.TextureTarget.TextureCubeMap)
                        TexturesListBox.Items.Add(element.Name);
                }
                TexturesListBox.SelectedIndex = tempind;
            }
        }

        private void TexturesGLControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Cursor = Cursors.Hand;
                    double deltaX = TexturesGLControl.Location.X + TexturesGLControl.Width / 2 - e.X;
                    double deltaY = TexturesGLControl.Location.X + TexturesGLControl.Height / 2 - e.Y;
                    MainCamera.AddRotation((float)deltaX / 100, (float)deltaY / 100);
                    TexturesGLControl.Invalidate();
                }
            }
        }

        private void TexturesGLControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }
}
