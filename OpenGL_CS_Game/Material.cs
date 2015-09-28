using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    public class Material
    {
        string shaderName = String.Empty;
        static Random R = new Random();
        Vector4 color = new Vector4((float)R.NextDouble(), (float)R.NextDouble(), (float)R.NextDouble(), 1.0f);
        int texturesCount = 0;
        string[] textures = new string[32];
        bool cullFace = true;
        bool transparent = false;
        Vector3 specularReflectivity = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 ambientReflectivity = new Vector3(0.1f, 0.1f, 0.1f);
        float specularShininess = 1.0f;
        float reflectionFactor = 0.5f;
        float refractiveIndex = 1.0f;

        public Material()
        {
            for (int i = 0; i < textures.Length; i++)
                textures[i] = null;
        }

        public Material(string ShaderName)
        {
            shaderName = ShaderName;

            for (int i = 0; i < textures.Length; i++)
                textures[i] = null;
        }

        public Material(string ShaderName, Vector4 MaterialColor)
        {
            shaderName = ShaderName;
            color = MaterialColor;

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

        public Vector4 Color
        {
            get { return color; }
            set { color = value; }
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
                    if (textures[i] != null)
                        texturesCount++;
                return true;
            }
            else
                return false;
        }

        public string GetTexture(int TextureUnit)
        {
            if (TextureUnit < 0 || TextureUnit > 31)
                return null;
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

        public bool Transparent
        {
            get { return transparent; }
            set { transparent = value; }
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

        /// <summary>
        /// Коефициент отражения (процент отраженного света). Диапазон 0.0 - 1.0.<br>
        /// По умолчанию = 0.5;
        /// Данный параметр позволяет выбирать между отражением (при 1.0) ИЛИ преломлением (при 0.0)!
        /// Если приломления не будет (RefractiveIndex = 1), как и отражений (ReflectionFactor = 0),
        /// то объект будет невидимым!
        /// </summary>
        public float ReflectionFactor
        {
            get { return reflectionFactor; }
            set { reflectionFactor = value; }
        }

        /// <summary>
        /// Коефициент преломлнения. Диапазон 0.0 - 1.0.
        /// По умолчанию = 1; Refraction = Угол1/Угол2. Рекомендуется брать примерно от 0.9 до 1.1.
        /// Работает только при ReflectionFactor меньше 1.0!
        /// </summary>
        public float RefractiveIndex
        {
            get { return refractiveIndex; }
            set { refractiveIndex = value; }
        }
    }
}
