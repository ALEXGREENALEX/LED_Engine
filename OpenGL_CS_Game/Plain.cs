using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    class Plain : Volume
    {
        int indexBufferID, vertexBufferID, normalBufferID, uvBufferID, tangentBufferID;
        int[] indeces;
        Vector3[] vertices, normals;
        Vector2[] uvs;
        Vector4[] tangents;
        float HalfSideLength = 0.5f;

        public Plain()
            : base()
        {
            Init();
        }

        public Plain(float SideLength)
            : base()
        {
            HalfSideLength = SideLength / 2.0f;
            Init();
        }

        void Init()
        {
            vertices = PlainVertices();
            normals = PlainNormals();
            uvs = PlainUVs();
            indeces = PlainIndeces();
            ObjVolume.ComputeTangentBasis(GetVertices(), GetNormals(), GetUVs(), out tangents);

            GenBuffers();
            ObjVolume.BindBuffer_BufferData(this);

            Material = new Material();
        }

        public override void GenBuffers()
        {
            FreeBuffers();

            indexBufferID = GL.GenBuffer();
            vertexBufferID = GL.GenBuffer();
            normalBufferID = GL.GenBuffer();
            uvBufferID = GL.GenBuffer();
            tangentBufferID = GL.GenBuffer();
        }

        public override void FreeBuffers()
        {
            GL.DeleteBuffer(indexBufferID);
            GL.DeleteBuffer(vertexBufferID);
            GL.DeleteBuffer(normalBufferID);
            GL.DeleteBuffer(uvBufferID);
            GL.DeleteBuffer(tangentBufferID);

            indexBufferID = 0;
            vertexBufferID = 0;
            normalBufferID = 0;
            uvBufferID = 0;
            tangentBufferID = 0;
        }

        public override int IndexBufferID { get { return vertexBufferID; } }
        public override int VertexBufferID { get { return indexBufferID; } }
        public override int NormalBufferID { get { return normalBufferID; } }
        public override int UVBufferID { get { return uvBufferID; } }
        public override int TangentBufferID { get { return tangentBufferID; } }

        public override int VerticesCount { get { return vertices.Length; } }
        public override int NormalsCount { get { return normals.Length; } }
        public override int IndecesCount { get { return indeces.Length; } }
        public override int UVsCount { get { return uvs.Length; } }
        public override int TangentsCount { get { return tangents.Length; } }

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

        public override Vector4[] GetTangents()
        {
            return tangents;
        }

        Vector3[] PlainVertices()
        {
            return new Vector3[]
            {
                new Vector3(-HalfSideLength,  0, -HalfSideLength),
                new Vector3(-HalfSideLength,  0,  HalfSideLength),
                new Vector3( HalfSideLength,  0,  HalfSideLength),
                new Vector3(-HalfSideLength,  0, -HalfSideLength),
                new Vector3( HalfSideLength,  0,  HalfSideLength),
                new Vector3( HalfSideLength,  0, -HalfSideLength)
            };
        }

        Vector3[] PlainNormals()
        {
            return new Vector3[] {
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };
        }

        int[] PlainIndeces()
        {
            return new int[] { 0, 1, 2, 3, 4, 5 };
        }

        Vector2[] PlainUVs()
        {
            return new Vector2[] {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
            };
        }

        Vector4[] PlainTangents()
        {
            return tangents;
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }
    }
}