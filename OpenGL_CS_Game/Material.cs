﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace OpenGL_CS_Game
{
    public class Material
    {
        string shaderName = String.Empty;
        int texturesCount = 0;
        string[] textures = new string[32];
        bool cullFace = true;
        Vector3 specularReflectivity = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 ambientReflectivity = new Vector3(0.1f, 0.1f, 0.1f);
        float specularShininess = 1.0f;

        public Material()
        {
            for (int i = 0; i < textures.Length; i++)
                textures[i] = String.Empty;
        }

        public Material(string ShaderName)
        {
            shaderName = ShaderName;

            for (int i = 0; i < textures.Length; i++)
                textures[i] = String.Empty;
        }

        public Material(string ShaderName, string[] Textures)
        {
            shaderName = ShaderName;
            textures = Textures;
        }

        public string ShaderName
        {
            get { return shaderName; }
            set { shaderName = value; }
        }

        public string[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        public bool SetTexture(int TextureUnit, string Texture)
        {
            if (TextureUnit >= 0 && TextureUnit < 32)
            {
                textures[TextureUnit] = Texture;
                texturesCount = 0;
                for (int i = 0; i < textures.Length; i++)
                    if (textures[i] != String.Empty)
                        texturesCount++;
                return true;
            }
            else
                return false;
        }

        public string GetTexture(int TextureUnit)
        {
            if (TextureUnit < 0 || TextureUnit > 31)
                return String.Empty;
            else
                return textures[TextureUnit];
        }

        public int TexturesCount
        {
            get { return texturesCount; }
        }

        public bool CullFace
        {
            get { return cullFace; }
            set { cullFace = value; }
        }

        public Vector3 SpecularReflectivity
        {
            get { return specularReflectivity; }
            set { specularReflectivity = value; }
        }

        public Vector3 AmbientReflectivity
        {
            get { return ambientReflectivity; }
            set { ambientReflectivity = value; }
        }

        public float SpecularShininess
        {
            get { return specularShininess; }
            set { specularShininess = value; }
        }
    }
}
