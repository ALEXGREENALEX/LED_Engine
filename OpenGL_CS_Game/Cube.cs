using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    class Cube : Volume
    {
        int indexBufferID, vertexBufferID, normalBufferID, uvBufferID, tangentBufferID, bitangentBufferID;
        int[] indeces;
        Vector3[] vertices, normals;
        Vector2[] uvs;
        Vector3[] tangents, bitangents;
        float HalfSideLength = 0.5f;
        bool Flipped = false;

        public Cube(bool FlipPolygons = false)
            : base()
        {
            Flipped = FlipPolygons;
            Init();
        }

        public Cube(float SideLength, bool FlipPolygons = false)
            : base()
        {
            HalfSideLength = SideLength / 2.0f;
            Flipped = FlipPolygons;
            Init();
        }

        void Init()
        {
            vertices = CubeVertices();
            normals = CubeNormals();
            uvs = CubeUVs();
            indeces = CubeIndeces();
            ObjVolume.ComputeTangentBasis(GetVertices(), GetNormals(), GetUVs(), out tangents, out bitangents);

            GenBuffers();
            ObjVolume.BindBuffer_BufferData(this);

            Material = new Material();
        }

        public override void GenBuffers()
        {
            indexBufferID = GL.GenBuffer();
            vertexBufferID = GL.GenBuffer();
            normalBufferID = GL.GenBuffer();
            uvBufferID = GL.GenBuffer();
            tangentBufferID = GL.GenBuffer();
            bitangentBufferID = GL.GenBuffer();
        }
        public override void FreeBuffers()
        {
            GL.DeleteBuffer(indexBufferID);
            GL.DeleteBuffer(vertexBufferID);
            GL.DeleteBuffer(normalBufferID);
            GL.DeleteBuffer(uvBufferID);
            GL.DeleteBuffer(tangentBufferID);
            GL.DeleteBuffer(bitangentBufferID);

            indexBufferID = 0;
            vertexBufferID = 0;
            normalBufferID = 0;
            uvBufferID = 0;
            tangentBufferID = 0;
            bitangentBufferID = 0;
        }

        public override int IndexBufferID { get { return vertexBufferID; } }
        public override int VertexBufferID { get { return indexBufferID; } }
        public override int NormalBufferID { get { return normalBufferID; } }
        public override int UVBufferID { get { return uvBufferID; } }
        public override int TangentBufferID { get { return tangentBufferID; } }
        public override int BitangentBufferID { get { return bitangentBufferID; } }

        public override int IndecesCount { get { return indeces.Length; } }
        public override int VerticesCount { get { return vertices.Length; } }
        public override int NormalsCount { get { return normals.Length; } }
        public override int UVsCount { get { return uvs.Length; } }
        public override int TangentsCount { get { return tangents.Length; } }
        public override int BitangentsCount { get { return bitangents.Length; } }

        public override int[] GetIndeces(int offset = 0)
        {
            int[] inds = new int[indeces.Length];

            for (int i = 0; i < indeces.Length; i++)
                inds[i] = indeces[i] + offset;

            return inds;
        }

        public override Vector3[] GetVertices()
        {
            return vertices;
        }

        public override Vector3[] GetNormals()
        {
            return normals;
        }

        public override Vector2[] GetUVs()
        {
            return uvs;
        }

        public override Vector3[] GetTangents()
        {
            return tangents;
        }

        public override Vector3[] GetBitangents()
        {
            return bitangents;
        }

        int[] CubeIndeces()
        {
            return new int[] {
                0, 1, 2, 3, 4, 5,
                6, 7, 8, 9, 10, 11,
                12, 13, 14, 15, 16, 17,
                18, 19, 20, 21, 22, 23,
                24, 25, 26, 27, 28, 29,
                30, 31, 32, 33, 34, 35 };
        }

        Vector3[] CubeVertices()
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

        Vector3[] CubeNormals()
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

        Vector2[] CubeUVs()
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

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }
    }
}