using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace LED_Engine
{
    public static class Textures
    {
        public static List<Texture> TEXTURES = new List<Texture>(); // Loaded Textures
        public static List<Texture> TexturesList = new List<Texture>(); // All Textures

        public static void LoadTexturesList(string XmlFile, string TexturePath, bool EngineContent)
        {
            ///////////////////////////////////////////////////////////////
            Log.WriteLine(ConsoleColor.Cyan, "TextureWrapMode.ClampToEdge");
            ///////////////////////////////////////////////////////////////
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

                    string File = Engine.CombinePaths(TexturePath, xmlNode.SelectSingleNode("File").InnerText);
                    texture.File = File;

                    if (xmlNode.SelectNodes("GenerateMipmap").Count > 0)
                        texture.GenerateMipmap = Convert.ToBoolean(xmlNode.SelectSingleNode("GenerateMipmap").InnerText);

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
                        if (!texture.GenerateMipmap)
                            texture.MinFilter = TextureMinFilter.Linear;
                    }

                    TexturesList.Add(texture);
                }
                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Textures.LoadTexturesList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                Log.WriteLineYellow(e.Message);
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
                        Dir = Engine.CombinePaths(CubemapTexturePath, xmlNode.SelectSingleNode("Dir").InnerText);

                    XmlNodeList FileNode = xmlNode.SelectNodes("File");
                    if (FileNode.Count >= 6)
                    {
                        for (int f = 0; f < 6; f++)
                            texture.CubemapFiles[f] = Engine.CombinePaths(Dir, FileNode.Item(f).InnerText);
                    }
                    else
                        throw new Exception("Cubemap: \"" + Name + "\" Files count < 6!!!");

                    if (xmlNode.SelectNodes("GenerateMipmap").Count > 0)
                        texture.GenerateMipmap = Convert.ToBoolean(xmlNode.SelectSingleNode("GenerateMipmap").InnerText);

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
                        if (!texture.GenerateMipmap)
                            texture.MinFilter = TextureMinFilter.Linear;
                    }

                    TexturesList.Add(texture);
                }
                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Textures.LoadTexturesList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                Log.WriteLineYellow(e.Message);
            }
        }

        public static Texture GetTexture(string Name)
        {
            foreach (var i in TEXTURES)
                if (i.Name.GetHashCode() == Name.GetHashCode())
                    return i;
            return null;
        }

        public static Texture GetTexture(int ID)
        {
            foreach (var i in TEXTURES)
                if (i.ID == ID)
                    return i;
            return null;
        }

        public static Texture Load(string Name)
        {
            Texture T = GetTexture(Name);

            if (T == null)
            {
                foreach (var i in TexturesList)
                    if (i.Name.GetHashCode() == Name.GetHashCode())
                        T = i;
                if (T == null)
                    return null;
            }

            if (T.UseCounter == 0)
            {
                switch (T.TextureTarget)
                {
                    case TextureTarget.Texture2D:
                        T.Load_Texture2D();
                        break;

                    case TextureTarget.TextureCubeMap:
                        T.Load_TextureCubemap();
                        break;

                    default:
                        T.Load_Texture2D();
                        break;
                }

                TEXTURES.Add(T);
            }

            T.UseCounter++;
            return T;
        }

        public static void Unload(string Name)
        {
            Unload(GetTexture(Name).ID);
        }

        public static void Unload(int ID)
        {
            Texture T = GetTexture(ID);
            if (T != null)
            {
                T.UseCounter--;

                if (T.UseCounter == 0)
                {
                    T.Free();
                    if (!T.EngineContent)
                        TEXTURES.Remove(T);
                }
            }
        }

        public static void Free(bool WithEngineContent = false)
        {
            try
            {
                if (WithEngineContent)
                {
                    foreach (var i in TEXTURES)
                        i.Free();

                    TEXTURES.Clear();
                    TexturesList.Clear();
                }
                else
                {
                    int Count = TEXTURES.Count;
                    for (int i = 0; i < Count; i++)
                        if (!TEXTURES[i].EngineContent)
                        {
                            TEXTURES[i].Free();
                            TEXTURES.RemoveAt(i);
                            Count--;
                        }
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Textures.Free() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }
    }

    public class Texture
    {
        public uint UseCounter = 0;
        public bool EngineContent = false;

        public int ID = 0;
        public bool GenerateMipmap = true;
        public TextureTarget TextureTarget = TextureTarget.Texture2D;
        public TextureMagFilter MagFilter = TextureMagFilter.Linear;
        public TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear;
        public string Name = String.Empty;
        public string File = String.Empty;
        public string[] CubemapFiles = new string[6];

        public void Load_Texture2D()
        {
            try
            {
                Load_Texture2D(new Bitmap(File));
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Error: Cannot load Texture (\"{0}\") from File: \"{1}\".", Name, File);
                Log.WriteLineYellow(e.Message);
                Free();
            }
        }

        public void Load_Texture2D(Bitmap Image)
        {
            try
            {
                Free();

                ID = GL.GenTexture();
                GL.BindTexture(TextureTarget, ID);

                // Отображаем текстуру по Y
                Image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = Image.LockBits(new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    Pencil.Gaming.Graphics.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                Image.UnlockBits(data);
                data = null;

                //Filter parameters
                GL.TexParameter(TextureTarget, TextureParameterName.TextureMagFilter, (int)MagFilter);
                GL.TexParameter(TextureTarget, TextureParameterName.TextureMinFilter, (int)MinFilter);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

                if (GenerateMipmap)
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Error: Cannot load texture. Name: \"{0}\".", Name);
                Log.WriteLineYellow(e.Message);
                Free();
            }
        }

        public void Load_TextureCubemap()
        {
            try
            {
                if (CubemapFiles.Length == 6)
                {
                    Free();

                    Bitmap[] BMPs = new Bitmap[CubemapFiles.Length];
                    for (int i = 0; i < BMPs.Length; i++)
                        BMPs[i] = new Bitmap(CubemapFiles[i]);

                    Load_TextureCubemap(BMPs);
                }
                else
                    throw new Exception("CubemapTextures Files Count != 6");
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Error: Cannot load CubemapTexture \"{0}\" from Files:", Name);
                for (int i = 0; i < CubemapFiles.Length; i++)
                    Log.WriteLine((ConsoleColor)(i+1), "File{0}: \"{1}\"", i, CubemapFiles[i]);
                Log.WriteLineYellow(e.Message);
                Free();
            }
        }

        public void Load_TextureCubemap(Bitmap[] CubemapTextures)
        {
            try
            {
                if (CubemapTextures.Length == 6)
                {
                    Free();

                    ID = GL.GenTexture();
                    //GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget, ID);

                    // Изменяем, отображаем и вращаем текстуры
                    for (int i = 0; i < CubemapTextures.Length; i++)
                    {
                        if (i < 2 || i > 3)
                            CubemapTextures[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        if (i == 2 || i == 3)
                            CubemapTextures[i].RotateFlip(RotateFlipType.Rotate270FlipY);
                        if (i == 2)
                            CubemapTextures[i].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }

                    TextureTarget[] targets = new TextureTarget[]
                    {
                        TextureTarget.TextureCubeMapPositiveX, TextureTarget.TextureCubeMapNegativeX,
                        TextureTarget.TextureCubeMapNegativeY, TextureTarget.TextureCubeMapPositiveY,
                        TextureTarget.TextureCubeMapPositiveZ, TextureTarget.TextureCubeMapNegativeZ
                    };

                    BitmapData data;
                    // Загружаем все текстуры граней
                    for (int i = 0; i < CubemapTextures.Length; i++)
                    {
                        data = CubemapTextures[i].LockBits(new System.Drawing.Rectangle(0, 0, CubemapTextures[i].Width,
                            CubemapTextures[i].Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexImage2D(targets[5 - i], 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0,
                            Pencil.Gaming.Graphics.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                        CubemapTextures[i].UnlockBits(data);
                    }
                    data = null;

                    //Filter parameters
                    GL.TexParameter(TextureTarget, TextureParameterName.TextureMagFilter, (int)MagFilter);
                    GL.TexParameter(TextureTarget, TextureParameterName.TextureMinFilter, (int)MinFilter);
                    GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

                    if (GenerateMipmap)
                        GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                }
                else
                    throw new Exception("CubemapTextures Count != 6");
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Error: Cannot load CubemapTexture. Name: \"{0}\".", Name);
                Log.WriteLineYellow(e.Message);
                Free();
            }
        }

        public void Free()
        {
            GL.DeleteTexture(ID);
            ID = 0;
            UseCounter = 0;
        }
    }
}
