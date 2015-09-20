using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_CS_Game
{
    class Plain : Volume
    {
        public Plain() : base()
        {
            VerticesCount = 4;
            NormalsCount = 4;
            FacesCount = 6;
            TextureCoordsCount = 4;
            TangentsesCount = 4;
            ShaderName = "Default";
            TextureID = new int[] { 0, 0, 0, 0 };
        }

        public override Vector3[] GetVertices()
        {
            return new Vector3[] {
                new Vector3(-1f, -1.0f, 0.0f), 
                new Vector3(1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f)
            };
        }

        public override Vector3[] GetNormals()
        {
            return new Vector3[] {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),

            };
        }

        public override uint[] GetFaceIndeces(uint offset = 0)
        {
            uint[] inds = new uint[] { 0, 1, 2, 0, 2, 3 };

            if (offset != 0)
                for (int i = 0; i < inds.Length; i++)
                    inds[i] += offset;

            return inds;
        }

        public override Vector2[] GetTextureCoords()
        {
            return new Vector2[] {
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f)
            };
        }

        public override Vector4[] GetTangentses()
        {
            return new Vector4[] {
                new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, -1.0f)
            };
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }
    }
}