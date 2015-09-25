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
            FacesCount = 36;
            TextureCoordsCount = 24;
            TangentsesCount = 24;

            tangenses = CalcTangentses();
            Material = new Material();
        }

        public Cube(float SideLength)
            : base()
        {
            HalfSideLength = SideLength / 2.0f;

            VerticesCount = 24;
            NormalsCount = 24;
            FacesCount = 36;
            TextureCoordsCount = 24;
            TangentsesCount = 24;

            tangenses = CalcTangentses();
            Material = new Material();
        }

        Vector4[] tangenses;
        float HalfSideLength = 0.5f;

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

        public override uint[] GetFaceIndeces(uint offset = 0)
        {
            uint[] inds = new uint[] {
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

        private Vector4[] CalcTangentses()
        {
            Vector3[] points = GetVertices();
            Vector3[] normals = GetNormals();
            uint[] faces = GetFaceIndeces();
            Vector2[] texCoords = GetTextureCoords();
            Vector4[] Tangenses = new Vector4[points.Length];
            List<Vector3> tan1Accum = new List<Vector3>();
            List<Vector3> tan2Accum = new List<Vector3>();

            for (uint i = 0; i < points.Length; i++)
            {
                tan1Accum.Add(new Vector3(0.0f));
                tan2Accum.Add(new Vector3(0.0f));
            }

            // Compute the tangent vector
            for (int i = 0; i < faces.Length; i += 3)
            {
                Vector3 p1 = points[(int)faces[i]];
                Vector3 p2 = points[(int)faces[i + 1]];
                Vector3 p3 = points[(int)faces[i + 2]];

                Vector2 tc1 = texCoords[(int)faces[i]];
                Vector2 tc2 = texCoords[(int)faces[i + 1]];
                Vector2 tc3 = texCoords[(int)faces[i + 2]];

                Vector3 q1 = p2 - p1;
                Vector3 q2 = p3 - p1;
                float s1 = tc2.X - tc1.X, s2 = tc3.X - tc1.X;
                float t1 = tc2.Y - tc1.Y, t2 = tc3.Y - tc1.Y;
                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 tan1 = new Vector3(
                    (t2 * q1.X - t1 * q2.X) * r,
                    (t2 * q1.Y - t1 * q2.Y) * r,
                    (t2 * q1.Z - t1 * q2.Z) * r);

                Vector3 tan2 = new Vector3(
                    (s1 * q2.X - s2 * q1.X) * r,
                    (s1 * q2.Y - s2 * q1.Y) * r,
                    (s1 * q2.Z - s2 * q1.Z) * r);

                tan1Accum[(int)faces[i]] += tan1;
                tan1Accum[(int)faces[i + 1]] += tan1;
                tan1Accum[(int)faces[i + 2]] += tan1;
                tan2Accum[(int)faces[i]] += tan2;
                tan2Accum[(int)faces[i + 1]] += tan2;
                tan2Accum[(int)faces[i + 2]] += tan2;
            }

            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 n = normals[i];
                Vector3 t1 = tan1Accum[i];
                Vector3 t2 = tan2Accum[i];

                // Gram-Schmidt orthogonalize
                Tangenses[i] = new Vector4(Vector3.Normalize(t1 - (Vector3.Dot(n, t1) * n)), 0.0f);
                // Store handedness in W
                Vector4 V_temp = Tangenses[i];
                V_temp.W = (Vector3.Dot(Vector3.Cross(n, t1), t2) < 0.0f) ? -1.0f : 1.0f;
                Tangenses[i] = V_temp;
            }

            tan1Accum.Clear();
            tan2Accum.Clear();
            return Tangenses;
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