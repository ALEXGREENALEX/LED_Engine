﻿using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_CS_Game
{
    class Cube : Volume
    {
        int texturesCount = 0;
        string[] textures = new string[32];

        public Cube()
            : base()
        {
            VerticesCount = 24;
            NormalsCount = 24;
            FacesCount = 36;
            TextureCoordsCount = 24;
            TangentsesCount = 24;

            ShaderName = String.Empty;

            for (int i = 0; i < textures.Length; i++)
                textures[i] = String.Empty;
        }

        public override Vector3[] GetVertices()
        {
            return new Vector3[] {
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),

                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),

                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),

                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),

                new Vector3(-0.5f, -0.5f,  -0.5f), 
                new Vector3(-0.5f, 0.5f,  0.5f), 
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),

                new Vector3(-0.5f, -0.5f,  -0.5f), 
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f)

            };
        }

        public override Vector3[] GetNormals()
        {
            return new Vector3[] {
                new Vector3(0.0f, 0.0f,  -1.0f),
                new Vector3(0.0f, 0.0f,  -1.0f),
                new Vector3(0.0f, 0.0f,  -1.0f),
                new Vector3(0.0f, 0.0f,  -1.0f),

                new Vector3(1.0f, 0.0f,  0.0f),
                new Vector3(1.0f, 0.0f,  0.0f),
                new Vector3(1.0f, 0.0f,  0.0f),
                new Vector3(1.0f, 0.0f,  0.0f),

                new Vector3(0.0f, 0.0f,  1.0f),
                new Vector3(0.0f, 0.0f,  1.0f),
                new Vector3(0.0f, 0.0f,  1.0f),
                new Vector3(0.0f, 0.0f,  1.0f),

                new Vector3(0.0f, 1.0f,  0.0f),
                new Vector3(0.0f, 1.0f,  0.0f),
                new Vector3(0.0f, 1.0f,  0.0f),
                new Vector3(0.0f, 1.0f,  0.0f),

                new Vector3(-1.0f, 0.0f,  0.0f),
                new Vector3(-1.0f, 0.0f,  0.0f),
                new Vector3(-1.0f, 0.0f,  0.0f),
                new Vector3(-1.0f, 0.0f,  0.0f),

                new Vector3(0.0f, -1.0f,  0.0f),
                new Vector3(0.0f, -1.0f,  0.0f),
                new Vector3(0.0f, -1.0f,  0.0f),
                new Vector3(0.0f, -1.0f,  0.0f),

            };
        }

        public override uint[] GetFaceIndeces(uint offset = 0)
        {
            uint[] inds = new uint[] {
                0,1,2,0,3,1,
                4,5,6,4,6,7,
                8,9,10,8,10,11,
                13,14,12,13,15,14,
                16,17,18,16,19,17,
                20,21,22,20,22,23
            };

            if (offset != 0)
                for (int i = 0; i < inds.Length; i++)
                    inds[i] += offset;

            return inds;
        }

        public override Vector2[] GetTextureCoords()
        {
            return new Vector2[] {
                new Vector2(0.0f, 0.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f),
 
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 0.0f),
                new Vector2(-1.0f, 1.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f)
            };
        }

        public override Vector4[] GetTangentses()
        {
            return new Vector4[] {
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, 0.0f, 0.0f, 1.0f),
                
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                
                new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f),
                new Vector4(0.0f, 0.0f, -1.0f, 1.0f)
            };
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }

        public override bool SetTexture(int TextureUnit, string Texture)
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

        public override string GetTexture(int TextureUnit)
        {
            if (TextureUnit < 0 || TextureUnit > 31)
                return String.Empty;
            else
                return textures[TextureUnit];
        }
        public override int TexturesCount
        {
            get { return texturesCount; }
        }
    }
}