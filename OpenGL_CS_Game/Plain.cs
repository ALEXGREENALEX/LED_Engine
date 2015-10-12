using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_CS_Game
{
    class Plain : Volume
    {
        Vector4[] tangents;
        float HalfSideLength = Game.UnitsScale * 50.0f;

        public Plain()
            : base()
        {
            VerticesCount = 4;
            NormalsCount = 4;
            IndecesCount = 6;
            UVsCount = 4;
            TangentsCount = 4;

            ObjVolume.ComputeTangentBasis(GetVertices(), GetNormals(), GetUVs(), out tangents);
            Material = new Material();
        }

        public Plain(float SideLength)
            : base()
        {
            HalfSideLength = SideLength / 2.0f * Game.UnitsScale;

            VerticesCount = 4;
            NormalsCount = 4;
            IndecesCount = 6;
            UVsCount = 4;
            TangentsCount = 4;

            ObjVolume.ComputeTangentBasis(GetVertices(), GetNormals(), GetUVs(), out tangents);
            Material = new Material();
        }

        public override Vector3[] GetVertices()
        {
            return new Vector3[] {
                new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength)
            };
        }

        public override Vector3[] GetNormals()
        {
            return new Vector3[] {
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
            };
        }

        public override int[] GetIndeces(int offset = 0)
        {
            int[] inds = new int[] { 0, 1, 2, 0, 2, 3 };

            if (offset != 0)
                for (int i = 0; i < inds.Length; i++)
                    inds[i] += offset;

            return inds;
        }

        public override Vector2[] GetUVs()
        {
            return new Vector2[] {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
            };
        }

        public override Vector4[] GetTangents()
        {
            return tangents;
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }
    }
}