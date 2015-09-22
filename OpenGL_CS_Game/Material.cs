using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace OpenGL_CS_Game
{
    public class Material
    {
        string name = String.Empty;
        string shaderName = String.Empty;
        int texturesCount = 0;
        string[] textures = new string[32];
        Vector3 specularReflectivity = new Vector3(0.2f, 0.2f, 0.2-f);
        Vector3 ambientReflectivity = new Vector3(0.1f, 0.1f, 0.1f);
        float specularShininess = 1.0f;

        public Material()
        {
            for (int i = 0; i < textures.Length; i++)
                textures[i] = String.Empty;
        }

        public Material(string Name)
        {
            name = Name;

            for (int i = 0; i < textures.Length; i++)
                textures[i] = String.Empty;
        }

        public Material(string Name, string ShaderName)
        {
            name = Name;
            shaderName = ShaderName;

            for (int i = 0; i < textures.Length; i++)
                textures[i] = String.Empty;
        }

        public Material(string Name, string ShaderName, string[] Textures)
        {
            name = Name;
            shaderName = ShaderName;
            textures = Textures;
        }

        public Material(string Name, string ShaderName, string[] Textures, Vector3 SpecularReflectivity, Vector3 AmbientReflectivity, float SpecularShininess)
        {
            name = Name;
            shaderName = ShaderName;
            textures = Textures;
            specularReflectivity = SpecularReflectivity;
            ambientReflectivity = AmbientReflectivity;
            specularShininess = SpecularShininess;
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

        public string ShaderName
        {
            get { return shaderName; }
            set { shaderName = value; }
        }
    }
}
