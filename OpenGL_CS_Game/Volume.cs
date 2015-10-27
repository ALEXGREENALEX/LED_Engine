using OpenTK;

namespace OpenGL_CS_Game
{
    /// <summary>
    /// Объект, состоящий из вершин
    /// </summary>
    public abstract class Volume
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public virtual int IndexBufferID { get; set; }
        public virtual int VertexBufferID { get; set; }
        public virtual int NormalBufferID { get; set; }
        public virtual int UVBufferID { get; set; }
        public virtual int TangentBufferID { get; set; }

        public virtual int IndecesCount { get; set; }
        public virtual int VerticesCount { get; set; }
        public virtual int NormalsCount { get; set; }
        public virtual int UVsCount { get; set; }
        public virtual int TangentsCount { get; set; }

        public virtual Material Material { get; set; }
        
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        public abstract void FreeBuffers();
        public abstract void GenBuffers();
        public abstract Vector3[] GetVertices();
        public abstract Vector2[] GetUVs();
        public abstract Vector3[] GetNormals();
        public abstract int[] GetIndeces(int offset = 0);
        public abstract Vector4[] GetTangents();
        public abstract void CalculateModelMatrix();
    }
}
