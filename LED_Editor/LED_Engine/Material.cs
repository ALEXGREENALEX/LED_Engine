﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Materials
    {
        public static List<Material> MATERIALS = new List<Material>(); // Loaded Materials
        public static List<Material> MaterialsList = new List<Material>(); // All Materials

        public static void LoadMaterialsList(string XmlFile, bool EngineContent)
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;

                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("Material");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Material material = new Material();

                    #region Parse
                    material.Name = xmlNode.SelectSingleNode("Name").InnerText;
                    material.EngineContent = EngineContent;

                    #region Shaders
                    string ShaderName = xmlNode.SelectSingleNode("Shader").InnerText;
                    for (int i = 0; i < Shaders.ShadersList.Count; i++)
                        if (Shaders.ShadersList[i].Name == ShaderName)
                        {
                            material.Shader = Shaders.ShadersList[i];
                            break;
                        }

                    if (material.Shader == null)
                        throw new Exception("LoadMaterialList() Parse Shaders Exception.");
                    #endregion

                    #region Textures
                    xmlNodeList = xmlNode.SelectNodes("Texture");
                    if (xmlNodeList.Count > 0)
                    {
                        for (int i = 0; i < xmlNodeList.Count; i++)
                        {
                            string TextureName = xmlNodeList.Item(i).SelectSingleNode("Name").InnerText;
                            uint TextureUnit = Convert.ToUInt32(xmlNodeList.Item(i).SelectSingleNode("TextureUnit").InnerText);

                            if (TextureUnit > material.Textures.Length | TextureName == null)
                                continue;

                            bool Found = false;
                            for (int j = 0; j < Textures.TexturesList.Count; j++)
                            {
                                if (Textures.TexturesList[j].Name == TextureName)
                                {
                                    material.Textures[TextureUnit] = Textures.TexturesList[j];
                                    Found = true;
                                    break;
                                }
                            }

                            if (!Found)
                                throw new Exception("LoadMaterialList() Parse Textures Exception. Name:\"" + TextureName + "\"");
                        }
                    }
                    #endregion

                    #region CullFace
                    xmlNodeList = xmlNode.SelectNodes("CullFace");
                    if (xmlNodeList.Count > 0)
                        material.CullFace = Convert.ToBoolean(xmlNodeList.Item(0).InnerText);
                    #endregion

                    #region Transparent
                    xmlNodeList = xmlNode.SelectNodes("Transparent");
                    if (xmlNodeList.Count > 0)
                        material.Transparent = Convert.ToBoolean(xmlNodeList.Item(0).InnerText);
                    #endregion

                    #region Kd Diffuse Color (Diffuse Reflectivity)
                    xmlNodeList = xmlNode.SelectNodes("Kd");
                    if (xmlNodeList.Count > 0)
                    {
                        try
                        {
                            string[] Color = xmlNodeList.Item(0).InnerText.Split(
                                new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            float R, G, B, A;
                            switch (Color.Length)
                            {
                                case 1:
                                    R = float.Parse(Color[0]);
                                    material.Kd = new Vector4(R, R, R, 1.0f);
                                    break;
                                case 3:
                                    R = float.Parse(Color[0]);
                                    G = float.Parse(Color[1]);
                                    B = float.Parse(Color[2]);
                                    material.Kd = new Vector4(R, G, B, 1.0f);
                                    break;
                                case 4:
                                    R = float.Parse(Color[0]);
                                    G = float.Parse(Color[1]);
                                    B = float.Parse(Color[2]);
                                    A = float.Parse(Color[3]);
                                    material.Kd = new Vector4(R, G, B, A);
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        int Count = 0;
                        for (int i = 0; i < material.Textures.Length; i++)
                            if (material.Textures[i] != null)
                                Count++;
                        if (Count > 0)
                            material.Kd = new Vector4(1.0f);
                    }
                    #endregion

                    #region Ks Specular Reflectivity
                    xmlNodeList = xmlNode.SelectNodes("Ks");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] SpecularReflectivity = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        float R, G, B;
                        switch (SpecularReflectivity.Length)
                        {
                            case 1:
                                R = float.Parse(SpecularReflectivity[0]);
                                material.Ks = new Vector3(R, R, R);
                                break;
                            case 3:
                                R = float.Parse(SpecularReflectivity[0]);
                                G = float.Parse(SpecularReflectivity[1]);
                                B = float.Parse(SpecularReflectivity[2]);
                                material.Ks = new Vector3(R, G, B);
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    #region Ka Ambient Reflectivity
                    xmlNodeList = xmlNode.SelectNodes("Ka");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] AmbientReflectivity = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        float R, G, B;
                        switch (AmbientReflectivity.Length)
                        {
                            case 1:
                                R = float.Parse(AmbientReflectivity[0]);
                                material.Ka = new Vector3(R, R, R);
                                break;
                            case 3:
                                R = float.Parse(AmbientReflectivity[0]);
                                G = float.Parse(AmbientReflectivity[1]);
                                B = float.Parse(AmbientReflectivity[2]);
                                material.Ka = new Vector3(R, G, B);
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    #region Ke Emissive Color
                    xmlNodeList = xmlNode.SelectNodes("Ke");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] EmissiveColor = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        float R, G, B;
                        switch (EmissiveColor.Length)
                        {
                            case 1:
                                R = float.Parse(EmissiveColor[0]);
                                material.Ke = new Vector3(R, R, R);
                                break;
                            case 3:
                                R = float.Parse(EmissiveColor[0]);
                                G = float.Parse(EmissiveColor[1]);
                                B = float.Parse(EmissiveColor[2]);
                                material.Ke = new Vector3(R, G, B);
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    #region Shininess (Specular Shininess)
                    xmlNodeList = xmlNode.SelectNodes("Shininess");
                    if (xmlNodeList.Count > 0)
                        material.Shininess = float.Parse(xmlNodeList.Item(0).InnerText);
                    #endregion

                    #region Reflection
                    xmlNodeList = xmlNode.SelectNodes("Reflection");
                    if (xmlNodeList.Count > 0)
                        material.Reflection = float.Parse(xmlNodeList.Item(0).InnerText);
                    #endregion

                    #region ParallaxScale
                    xmlNodeList = xmlNode.SelectNodes("ParallaxScale");
                    if (xmlNodeList.Count > 0)
                        material.ParallaxScale = float.Parse(xmlNodeList.Item(0).InnerText);
                    #endregion
                    #endregion

                    MaterialsList.Add(material);
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Materials.LoadMaterialsList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                Log.WriteLineYellow(e.Message);
            }
        }

        public static Material GetMaterial(string Name)
        {
            foreach (var i in MATERIALS)
                if (i.Name.GetHashCode() == Name.GetHashCode())
                    return i;
            return null;
        }

        public static Material Load(string Name)
        {
            try
            {
                Material M = GetMaterial(Name);

                if (M == null)
                {
                    foreach (var v in MaterialsList)
                        if (v.Name.GetHashCode() == Name.GetHashCode())
                            M = v;
                    if (M == null)
                        return null;
                }

                if (M.UseCounter == 0)
                {
                    Shaders.Load(M.Shader.Name);

                    for (int i = 0; i < M.Textures.Length; i++)
                        if (M.Textures[i] != null)
                            Textures.Load(M.Textures[i].Name);

                    MATERIALS.Add(M);
                }

                M.UseCounter++;
                return M;
            }
            catch
            {
                Log.WriteLineRed("Materials.Load() Exception, Name: \"{0}\"", Name);
                return null;
            }
        }

        public static void Unload(string Name)
        {
            Unload(GetMaterial(Name));
        }

        public static void Unload(Material M)
        {
            if (M != null)
            {
                if (M.UseCounter > 0)
                {
                    M.UseCounter--;

                    if (M.UseCounter == 0)
                    {
                        M.Free();
                        MATERIALS.Remove(M);
                    }
                }
            }
        }
    }

    public class Material
    {
        public uint UseCounter = 0;
        public bool EngineContent = false;

        public string Name = String.Empty;
        public Shader Shader;
        public bool CullFace = true;
        public bool Transparent = false;

        static Random R = new Random();
        public Vector4 Kd = new Vector4((float)R.NextDouble(), (float)R.NextDouble(), (float)R.NextDouble(), 1.0f);
        public Vector3 Ks = new Vector3(0.2f);
        public Vector3 Ka = new Vector3(1.0f);
        public Vector3 Ke = new Vector3(0.0f);
        public float Shininess = 50.0f;

        /// <summary>
        /// Коефициент отражения (процент отраженного света). Диапазон 0.0 - 1.0. По умолчанию = 0.5.
        /// </summary>
        public float Reflection = 0.0f;

        public float ParallaxScale = 0.04f;

        public Texture[] Textures = new Texture[Settings.GL.TextureImageUnits];

        public Material()
        {
        }

        public Material(Material Original)
        {
            Shader = Shaders.Load(Original.Shader.Name);

            CullFace = Original.CullFace;
            Transparent = Original.Transparent;
            Ka = Original.Ka;
            Kd = Original.Kd;
            Ks = Original.Ks;
            Ke = Original.Ke;
            Shininess = Original.Shininess;
            Reflection = Original.Reflection;
            ParallaxScale = Original.ParallaxScale;

            Textures = new Texture[Original.Textures.Length];
            for (int i = 0; i < Textures.Length; i++)
                if (Original.Textures[i] != null)
                    Textures[i] = LED_Engine.Textures.Load(Original.Textures[i].Name);
        }

        public void Free()
        {
            Shaders.Unload(Shader);

            for (int i = 0; i < Textures.Length; i++)
                LED_Engine.Textures.Unload(Textures[i]);

            UseCounter = 0;
        }
    }
}
