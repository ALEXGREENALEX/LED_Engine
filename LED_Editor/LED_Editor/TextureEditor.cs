using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace LED_Editor
{
    public partial class TextureEditor : Form
    {
        public static List<Texture> TexturesList = new List<Texture>(); // All Textures
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
        public static uint OpenGLTexture;
        public static string OpenGLPath;
        public static int id; //GLtexture id

        bool loaded = false;

        public TextureEditor()
        {
            InitializeComponent();

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

            CutTexturePath = TexturePath.TrimEnd(new char[] { '\\', '/' });
            CutEngineTexturePath = EngineTexturePath.TrimEnd(new char[] { '\\', '/' });
            CutTexturePath = CubeMapPath.TrimEnd(new char[] { '\\', '/' });
            CutEngineTexturePath = EngineCubeMapPath.TrimEnd(new char[] { '\\', '/' });

            //Загружаем engine текстуры
            LoadTexturesList(EngineTextureXMLPath, EngineTexturePath, true);

            //Загружаем НЕ engine текстуры
            LoadTexturesList(TextureXMLPath, TexturePath, false);

            //Загружаем engine cubemap-текстуры
            LoadCubemapTexturesList(EngineCubeMapXMLPath, EngineCubeMapPath, true);

            //Загружаем НЕ engine cubemap-текстуры
            LoadCubemapTexturesList(CubeMapXMLPath, CubeMapPath, false);

            foreach (Texture element in TexturesList)
            {
                if (element.TextureTarget == TextureTarget.Texture2D)
                    TexturesListBox.Items.Add(element.Name);
            }

            //TexturesListIndexCounter = TexturesListBox.Items.Count;

        }

        #region OpenGL Stuff
        private void SetupViewport()
        {
            int w = TexturesGLControl.Width;
            int h = TexturesGLControl.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }
        static int LoadTextureToControl(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }

        static void DeleteTextureFromControl()
        {
            GL.DeleteTexture(id);
            id = 0;
        }

        private void TexturesGLControl_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            SetupViewport();
            TexturesGLControl.Invalidate();
        }

        private void TextureEditor_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            
            //OpenGLTexture = (uint)LoadTextureToControl(@"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Textures\brick-wall.jpg");
            SetupViewport();
            return;
        }

        private void TexturesGLControl_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1, -1);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1, -1);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1, 1);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1, 1);

            GL.End();

            TexturesGLControl.SwapBuffers();
        }
        public static void DrawTextureToControl(string File)
        {
            DeleteTextureFromControl();
            OpenGLTexture = (uint)LoadTextureToControl(File);
        }
        #endregion

        public static void LoadTexturesList(string XmlFile, string TexturePath, bool EngineContent)
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;

                #region Load Textures List
                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("Texture");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Texture texture = new Texture();
                    texture.EngineContent = EngineContent;

                    texture.TextureTarget = TextureTarget.Texture2D;

                    string Name = xmlNode.SelectSingleNode("Name").InnerText;
                    texture.Name = Name;

                    string File = CombinePaths(TexturePath, xmlNode.SelectSingleNode("File").InnerText);
                    texture.File = File;

                    texture.EngineContent = EngineContent;

                    //if (xmlNode.SelectNodes("GenerateMipmap").Count > 0)
                    //    texture.GenerateMipmap = Convert.ToBoolean(xmlNode.SelectSingleNode("GenerateMipmap").InnerText);

                    if (xmlNode.SelectNodes("MagFilter").Count > 0)
                    {
                        string StrMagFilter = xmlNode.SelectSingleNode("MagFilter").InnerText;
                        texture.MagFilter = (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true);
                    }

                    if (xmlNode.SelectNodes("MinFilter").Count > 0)
                    {
                        string StrMinFilter = xmlNode.SelectSingleNode("MinFilter").InnerText;
                        texture.MinFilter = (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true);
                    }
                    else
                    {
                        texture.MinFilter = TextureMinFilter.Linear;
                    }
                    TexturesList.Add(texture);
                }
                #endregion
            }
            catch (Exception e)
            {
                //Log.WriteLineRed("Textures.LoadTexturesList() Exception.");
                //Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                //Log.WriteLineYellow(e.Message);
            }
        }

        public static void LoadCubemapTexturesList(string XmlFile, string CubemapTexturePath, bool EngineContent)
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;

                #region Load CubemapTextures
                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("CubemapTexture");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Texture texture = new Texture();
                    texture.EngineContent = EngineContent;
                    texture.TextureTarget = TextureTarget.TextureCubeMap;

                    string Name = xmlNode.SelectSingleNode("Name").InnerText;
                    texture.Name = Name;

                    string Dir = String.Empty;
                    if (xmlNode.SelectNodes("Dir").Count > 0)
                    {
                        Dir = CombinePaths(CubemapTexturePath, xmlNode.SelectSingleNode("Dir").InnerText);
                        texture.CubemapDir = Dir;
                    }

                    XmlNodeList FileNode = xmlNode.SelectNodes("File");
                    if (FileNode.Count >= 6)
                    {
                        for (int f = 0; f < 6; f++)
                            texture.CubemapFiles[f] = CombinePaths(Dir, FileNode.Item(f).InnerText);
                    }
                    else
                        throw new Exception("Cubemap: \"" + Name + "\" Files count < 6!!!");

                    //if (xmlNode.SelectNodes("GenerateMipmap").Count > 0)
                    //    texture.GenerateMipmap = Convert.ToBoolean(xmlNode.SelectSingleNode("GenerateMipmap").InnerText);

                    if (xmlNode.SelectNodes("MagFilter").Count > 0)
                    {
                        string StrMagFilter = xmlNode.SelectSingleNode("MagFilter").InnerText;
                        texture.MagFilter = (TextureMagFilter)Enum.Parse(typeof(TextureMagFilter), StrMagFilter, true);

                    }

                    if (xmlNode.SelectNodes("MinFilter").Count > 0)
                    {
                        string StrMinFilter = xmlNode.SelectSingleNode("MinFilter").InnerText;
                        texture.MinFilter = (TextureMinFilter)Enum.Parse(typeof(TextureMinFilter), StrMinFilter, true);
                    }
                    else
                    {
                        //if (!texture.GenerateMipmap)
                        texture.MinFilter = TextureMinFilter.Linear;
                    }

                    TexturesList.Add(texture);

                }
                #endregion
            }
            catch
            {

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

                foreach (Texture tex in TexturesList)
                {
                    if (tex.TextureTarget == TextureTarget.Texture2D)
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

                foreach (Texture tex in TexturesList)
                {
                    if (tex.TextureTarget == TextureTarget.TextureCubeMap)
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
                            string dir = tex.CubemapDir;
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
                            string dir = tex.CubemapDir;
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

        #region Fix and combine paths
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



        #endregion

        public class Texture
        {
            //public uint UseCounter = 0;
            public bool EngineContent = false;

            public int ID = 0;
            //public bool GenerateMipmap = true;
            public TextureTarget TextureTarget = TextureTarget.Texture2D;
            public TextureMagFilter MagFilter = TextureMagFilter.Linear;
            public TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear;
            public string Name = String.Empty;
            public string File = String.Empty;
            public string[] CubemapFiles = new string[6];
            public string CubemapDir = String.Empty;
        }

        private void TexturesListBox_SelectedIndexChanged(object sender, EventArgs e)
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
                    NameTextBox.Text = TexturesList[TexturesListBox.SelectedIndex].Name;
                }
                catch
                {

                }
                
                TexturesGLControl.Show();
                string path = String.Empty;

                try
                {
                    path = TexturesList[TexturesListBox.SelectedIndex].File;
                    OpenGLPath = path;
                }
                catch
                {

                }

                try
                {
                    if (TexturesList[TexturesListBox.SelectedIndex].EngineContent == true)
                    {
                        FileTextBox.BackColor = Color.White;
                        FileTextBox.Text = FixPath(path.Substring(CutEngineTexturePath.Length + 1, path.Length - CutEngineTexturePath.Length - 1));
                    }
                    if (TexturesList[TexturesListBox.SelectedIndex].EngineContent == false)
                    {
                        FileTextBox.BackColor = Color.White;
                        FileTextBox.Text = FixPath(path.Substring(CutTexturePath.Length + 1, path.Length - CutTexturePath.Length - 1));
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
                    DrawTextureToControl(path);
                    TexturesGLControl.Invalidate();
                    //TexturesGLControl.SwapBuffers();
                }
                catch
                {
                    FileTextBox.BackColor = Color.Tomato;
                    FileTextBox.Text = "Error";
                    TexturesGLControl.Hide();
                }
                try
                {
                    if (TexturesList[TexturesListBox.SelectedIndex].EngineContent == true)
                        EngineCheckBox.Checked = true;
                    if (TexturesList[TexturesListBox.SelectedIndex].EngineContent == false)
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
                NameTextBox.Text = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name;

                try
                {
                    path = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0];
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
                    path = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1];
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
                    path = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2];
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
                    path = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3];
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
                    path = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4];
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
                    path = TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5];
                    CubeMapFileTextBox6.BackColor = Color.White;
                    CubeMapFileTextBox6.Text = path.Substring(path.LastIndexOf("Cubemaps"));
                }
                catch
                {
                    CubeMapFileTextBox6.BackColor = Color.Tomato;
                    CubeMapFileTextBox6.Text = "Error";
                }

                if (TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent == true)
                    EngineCheckBox.Checked = true;
                if (TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent == false)
                    EngineCheckBox.Checked = false;
            }
            try
            {
                foreach (var item in MagFilterComboBox.Items)
                {
                    if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                    {
                        if (item.ToString() == TexturesList[TexturesListBox.SelectedIndex].MagFilter.ToString())
                        {
                            MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString(TexturesList[TexturesListBox.SelectedIndex].MagFilter.ToString());
                        }
                    }
                    if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                    {
                        if (item.ToString() == TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString())
                        {
                            MagFilterComboBox.SelectedIndex = MagFilterComboBox.FindString(TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MagFilter.ToString());
                        }
                    }
                }

                foreach (var item in MinFilterComboBox.Items)
                {
                    if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
                    {
                        if (item.ToString() == TexturesList[TexturesListBox.SelectedIndex].MinFilter.ToString())
                        {
                            MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString(TexturesList[TexturesListBox.SelectedIndex].MinFilter.ToString());
                        }
                    }
                    if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
                    {
                        if (item.ToString() == TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString())
                        {
                            MinFilterComboBox.SelectedIndex = MinFilterComboBox.FindString(TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].MinFilter.ToString());
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void TextureEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            TexturesList.Clear();
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
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                TextureFilePanel.Show();
                CubemapTexturePanel.Hide();
                TexturesListBox.Items.Clear();
                MagFilterComboBox.SelectedIndex = -1;
                MinFilterComboBox.SelectedIndex = -1;
                FileTextBox.Clear();
                TexturesGLControl.Hide();
                foreach (Texture element in TexturesList)
                {
                    if (element.TextureTarget == TextureTarget.Texture2D)
                        TexturesListBox.Items.Add(element.Name);
                }
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                TexturesListIndexCounter = TexturesListBox.Items.Count;
                TextureFilePanel.Hide();
                CubemapTexturePanel.Show();
                TexturesListBox.Items.Clear();
                MagFilterComboBox.SelectedIndex = -1;
                MinFilterComboBox.SelectedIndex = -1;
                FileTextBox.Clear();
                TexturesGLControl.Hide();
                foreach (Texture element in TexturesList)
                {
                    if (element.TextureTarget == TextureTarget.TextureCubeMap)
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
                newtexture.TextureTarget = TextureTarget.Texture2D;
                TexturesList.Insert(TexturesListBox.Items.Count, newtexture);
                TexturesListBox.Items.Add(newtexture.Name);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                Texture newtexture = new Texture();
                newtexture.TextureTarget = TextureTarget.TextureCubeMap;
                TexturesList.Add(newtexture);
                TexturesListBox.Items.Add(newtexture.Name);
            }
        }

        private void TexturesDeleteButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                TexturesList.RemoveAt(TexturesListBox.SelectedIndex);
                TexturesListBox.Items.RemoveAt(TexturesListBox.SelectedIndex);
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                TexturesList.RemoveAt(TexturesListBox.SelectedIndex + TexturesListIndexCounter);
                TexturesListBox.Items.RemoveAt(TexturesListBox.SelectedIndex);
            }
        }

        private void MagFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                //TexturesList[TexturesListBox.SelectedIndex].MagFilter = 
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[0])
            {
                //if (TexturesList[TexturesListBox.SelectedIndex].EngineContent == true)
                //    TexturesList[TexturesListBox.SelectedIndex].File = CombinePaths(EngineTexturePath, FileTextBox.Text);

                //if (TexturesList[TexturesListBox.SelectedIndex].EngineContent == false)
                //    TexturesList[TexturesListBox.SelectedIndex].File = CombinePaths(TexturePath, FileTextBox.Text);

                TexturesList[TexturesListBox.SelectedIndex].Name = NameTextBox.Text;
                if (EngineCheckBox.Checked == true)
                {
                    TexturesList[TexturesListBox.SelectedIndex].EngineContent = true;
                    TexturesList[TexturesListBox.SelectedIndex].File = CombinePaths(EngineTexturePath, FileTextBox.Text);
                }
                if (EngineCheckBox.Checked == false)
                {
                    TexturesList[TexturesListBox.SelectedIndex].EngineContent = false;
                    TexturesList[TexturesListBox.SelectedIndex].File = CombinePaths(TexturePath, FileTextBox.Text);
                }

                int tempind = TexturesListBox.SelectedIndex;
                TexturesListBox.Items.Clear();
                foreach (Texture element in TexturesList)
                {
                    if (element.TextureTarget == TextureTarget.Texture2D)
                        TexturesListBox.Items.Add(element.Name);
                }
                TexturesListBox.SelectedIndex = tempind;
            }
            if (MaterialTabs.SelectedTab == MaterialTabs.TabPages[1])
            {
                TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].Name = NameTextBox.Text;
                if (EngineCheckBox.Checked == true)
                {
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent = true;
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0] = CombinePaths(EngineCubeMapPath, CubeMapFileTextBox1.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1] = CombinePaths(EngineCubeMapPath, CubeMapFileTextBox2.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2] = CombinePaths(EngineCubeMapPath, CubeMapFileTextBox3.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3] = CombinePaths(EngineCubeMapPath, CubeMapFileTextBox4.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4] = CombinePaths(EngineCubeMapPath, CubeMapFileTextBox5.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5] = CombinePaths(EngineCubeMapPath, CubeMapFileTextBox6.Text);
                }
                if (EngineCheckBox.Checked == false)
                {
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].EngineContent = false;
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[0] = CombinePaths(CubeMapPath, CubeMapFileTextBox1.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[1] = CombinePaths(CubeMapPath, CubeMapFileTextBox2.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[2] = CombinePaths(CubeMapPath, CubeMapFileTextBox3.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[3] = CombinePaths(CubeMapPath, CubeMapFileTextBox4.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[4] = CombinePaths(CubeMapPath, CubeMapFileTextBox5.Text);
                    TexturesList[TexturesListBox.SelectedIndex + TexturesListIndexCounter].CubemapFiles[5] = CombinePaths(CubeMapPath, CubeMapFileTextBox6.Text);
                }
                int tempind = TexturesListBox.SelectedIndex;
                TexturesListBox.Items.Clear();
                foreach (Texture element in TexturesList)
                {
                    if (element.TextureTarget == TextureTarget.TextureCubeMap)
                        TexturesListBox.Items.Add(element.Name);
                }
                TexturesListBox.SelectedIndex = tempind;
            }
        }
    }
}
