﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
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

                    #region Kd DiffuseReflectivity (Color)
                    xmlNodeList = xmlNode.SelectNodes("Color");
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
                    #endregion

                    #region Ks SpecularReflectivity
                    xmlNodeList = xmlNode.SelectNodes("SpecularReflectivity");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] SpecularReflectivity = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (SpecularReflectivity.Length == 3)
                        {
                            float R = float.Parse(SpecularReflectivity[0]);
                            float G = float.Parse(SpecularReflectivity[1]);
                            float B = float.Parse(SpecularReflectivity[2]);
                            material.Ks = new Vector3(R, G, B);
                        }
                    }
                    #endregion

                    #region Ka AmbientReflectivity
                    xmlNodeList = xmlNode.SelectNodes("AmbientReflectivity");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] AmbientReflectivity = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (AmbientReflectivity.Length == 3)
                        {
                            float R = float.Parse(AmbientReflectivity[0]);
                            float G = float.Parse(AmbientReflectivity[1]);
                            float B = float.Parse(AmbientReflectivity[2]);
                            material.Ka = new Vector3(R, G, B);
                        }
                    }
                    #endregion

                    #region Ke Emissive Color
                    xmlNodeList = xmlNode.SelectNodes("EmissiveColor");
                    if (xmlNodeList.Count > 0)
                    {
                        string[] EmissiveColor = xmlNodeList.Item(0).InnerText.Split(
                            new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (EmissiveColor.Length == 3)
                        {
                            float R = float.Parse(EmissiveColor[0]);
                            float G = float.Parse(EmissiveColor[1]);
                            float B = float.Parse(EmissiveColor[2]);
                            material.Ke = new Vector3(R, G, B);
                        }
                    }
                    #endregion

                    #region Specular Shininess
                    xmlNodeList = xmlNode.SelectNodes("SpecularShininess");
                    if (xmlNodeList.Count > 0)
                        material.Shininess = float.Parse(xmlNodeList.Item(0).InnerText);
                    #endregion

                    #region Reflection Factor
                    xmlNodeList = xmlNode.SelectNodes("ReflectionFactor");
                    if (xmlNodeList.Count > 0)
                        material.ReflectionFactor = float.Parse(xmlNodeList.Item(0).InnerText);
                    #endregion

                    #region Refractive Index
                    xmlNodeList = xmlNode.SelectNodes("RefractiveIndex");
                    if (xmlNodeList.Count > 0)
                        material.RefractiveIndex = float.Parse(xmlNodeList.Item(0).InnerText);
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

        public static Material Load(string Name)
        {
            try
            {
                Material M = GetMaterial(Name);

                if (M == null)
                {
                    foreach (var i in MaterialsList)
                        if (i.Name.GetHashCode() == Name.GetHashCode())
                            M = i;
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

        public static Material GetMaterial(string Name)
        {
            foreach (var i in MATERIALS)
                if (i.Name.GetHashCode() == Name.GetHashCode())
                    return i;
            return null;
        }

        public static void Unload(string Name)
        {
            Material M = GetMaterial(Name);
            if (M != null)
            {
                M.UseCounter--;

                if (M.UseCounter == 0)
                {
                    if (!M.EngineContent)
                        MATERIALS.Remove(M);
                }
            }
        }

        public static void Free(bool WithEngineContent = false)
        {
            try
            {
                if (WithEngineContent)
                {
                    MATERIALS.Clear();
                    MaterialsList.Clear();
                }
                else
                {
                    int Count = MATERIALS.Count;
                    for (int i = 0; i < Count; i++)
                        if (!MATERIALS[i].EngineContent)
                        {
                            MATERIALS.RemoveAt(i);
                            Count--;
                        }
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Materials.Free() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }
    }

    public class Material
    {
        public uint UseCounter = 0;
        public bool EngineContent = false;

        static Random R = new Random();
        public string Name = String.Empty;
        public Shader Shader;
        public bool CullFace = true;
        public bool Transparent = false;
        public Vector4 Kd = new Vector4((float)R.NextDouble(), (float)R.NextDouble(), (float)R.NextDouble(), 1.0f);
        public Vector3 Ka = new Vector3(0.1f, 0.1f, 0.1f);
        public Vector3 Ks = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 Ke = new Vector3(0.0f, 0.0f, 0.0f);
        public float Shininess = 1.0f;

        /// <summary>
        /// Коефициент отражения (процент отраженного света). Диапазон 0.0 - 1.0.<br>
        /// По умолчанию = 0.5;
        /// Данный параметр позволяет выбирать между отражением (при 1.0) ИЛИ преломлением (при 0.0)!
        /// Если приломления не будет (RefractiveIndex = 1), как и отражений (ReflectionFactor = 0),
        /// то объект будет невидимым!
        /// </summary>
        public float ReflectionFactor = 0.5f;

        /// <summary>
        /// Коефициент преломлнения. Диапазон 0.0 - 1.0.
        /// По умолчанию = 1; Refraction = Угол1/Угол2. Рекомендуется брать примерно от 0.9 до 1.1.
        /// Работает только при ReflectionFactor меньше 1.0!
        /// </summary>
        public float RefractiveIndex = 1.0f;

        public Texture[] Textures = new Texture[Settings.GL.TextureImageUnits];

        public Material()
        {
        }

        public Material(Material OriginalMaterial, bool SaveColor = false)
        {
            if (SaveColor)
                Kd = OriginalMaterial.Kd;
            else
                Kd = new Vector4((float)R.NextDouble(), (float)R.NextDouble(), (float)R.NextDouble(), 1.0f);

            Shader = OriginalMaterial.Shader;
            CullFace = OriginalMaterial.CullFace;
            Transparent = OriginalMaterial.Transparent;
            Ks = OriginalMaterial.Ks;
            Ka = OriginalMaterial.Ka;
            Ke = OriginalMaterial.Ke;
            Shininess = OriginalMaterial.Shininess;
            ReflectionFactor = OriginalMaterial.ReflectionFactor;
            RefractiveIndex = OriginalMaterial.RefractiveIndex;

            Textures = new Texture[OriginalMaterial.Textures.Length];
            OriginalMaterial.Textures.CopyTo(Textures, 0);
        }
    }
}
