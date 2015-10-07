using System;
using System.Collections.Generic;
using OpenTK;

namespace OpenGL_CS_Game
{
    class Cube : Volume
    {
        public Cube()
            : base()
        {
            VerticesCount = 24;
            NormalsCount = 24;
            IndexesCount = 36;
            TextureCoordsCount = 24;
            TangentsesCount = 24;

            tangenses = ObjVolume.CalcTangentses(GetVertices(), GetNormals(), GetTextureCoords(), GetIndexes());
            Material = new Material();
        }

        public Cube(float SideLength)
            : base()
        {
            HalfSideLength = SideLength / 2.0f;

            VerticesCount = 24;
            NormalsCount = 24;
            IndexesCount = 36;
            TextureCoordsCount = 24;
            TangentsesCount = 24;

            tangenses = ObjVolume.CalcTangentses(GetVertices(), GetNormals(), GetTextureCoords(), GetIndexes());
            Material = new Material();
        }

        Vector4[] tangenses;
        float HalfSideLength = 50.0f;

        public override Vector3[] GetVertices()
        {
            return new Vector3[] {
                // Front
                new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),

                // Right
                new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),
                new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),

                // Back
                new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),
                new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),

                // Left
                new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength),
                new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength),
                new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength),
                new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength),

                // Bottom
                new Vector3(-HalfSideLength, -HalfSideLength,  HalfSideLength), 
                new Vector3(-HalfSideLength, -HalfSideLength, -HalfSideLength), 
                new Vector3( HalfSideLength, -HalfSideLength, -HalfSideLength),
                new Vector3( HalfSideLength, -HalfSideLength,  HalfSideLength),

                // Top
                new Vector3(-HalfSideLength,  HalfSideLength,  HalfSideLength), 
                new Vector3( HalfSideLength,  HalfSideLength,  HalfSideLength),
                new Vector3( HalfSideLength,  HalfSideLength, -HalfSideLength),
                new Vector3(-HalfSideLength,  HalfSideLength, -HalfSideLength)
            };
        }

        public override Vector3[] GetNormals()
        {
            return new Vector3[] {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),

                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),

                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),

                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),

                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),

                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),

            };
        }

        public override int[] GetIndexes(int offset = 0)
        {
            int[] inds = new int[] {
                0,1,2,0,2,3,
                4,5,6,4,6,7,
                8,9,10,8,10,11,
                12,13,14,12,14,15,
                16,17,18,16,18,19,
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
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f)
            };
        }

        public override Vector4[] GetTangentses()
        {
            return tangenses;
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }
    }
}