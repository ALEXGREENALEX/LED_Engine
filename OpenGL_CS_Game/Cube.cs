using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_CS_Game
{
    class Cube : Volume
    {
        Vector4[] tangents;
        float HalfSideLength = Game.UnitsScale * 50.0f; // По умолчанию куб со сторонами 100 см.
        bool Flipped = false;

        public Cube(bool FlipPolygons = false)
            : base()
        {
            Flipped = FlipPolygons;
            VerticesCount = 36;
            NormalsCount = 36;
            IndecesCount = 36;
            UVsCount = 36;
            TangentsCount = 36;

            ObjVolume.ComputeTangentBasis(GetVertices(), GetNormals(), GetUVs(), out tangents);
            Material = new Material();
        }

        public Cube(float SideLength, bool FlipPolygons = false)
            : base()
        {
            HalfSideLength = SideLength / 2.0f * Game.UnitsScale;
            Flipped = FlipPolygons;

            VerticesCount = 36;
            NormalsCount = 36;
            IndecesCount = 36;
            UVsCount = 36;
            TangentsCount = 36;

            ObjVolume.ComputeTangentBasis(GetVertices(), GetNormals(), GetUVs(), out tangents);
            Material = new Material();
        }

        public override Vector3[] GetVertices()
        {
            if (Flipped)
                return new Vector3[] {
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength) };
            else
                return new Vector3[] {
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),

                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),

                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),

                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),

                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),

                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                    new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                    new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength) };
        }

        public override Vector3[] GetNormals()
        {
            if (Flipped)
                return new Vector3[] {
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),

                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),

                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),

                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),

                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),

                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0) };
            else
                return new Vector3[] {
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),

                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),

                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),

                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),

                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),

                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0) };
        }

        public override int[] GetIndeces(int offset = 0)
        {
            int[] inds = new int[] {
                0, 1, 2, 3, 4, 5,
                6, 7, 8, 9, 10, 11,
                12, 13, 14, 15, 16, 17,
                18, 19, 20, 21, 22, 23,
                24, 25, 26, 27, 28, 29,
                30, 31, 32, 33, 34, 35 };

            if (offset != 0)
                for (int i = 0; i < inds.Length; i++)
                    inds[i] += offset;

            return inds;
        }

        public override Vector2[] GetUVs()
        {
            if (Flipped)
                return new Vector2[] {
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0) };
            else
                return new Vector2[] {
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0) };
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