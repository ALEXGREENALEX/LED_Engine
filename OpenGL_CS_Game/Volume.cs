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

        public virtual int VerticesCount { get; set; }
        public virtual int TextureCoordsCount { get; set; }
        public virtual int NormalsCount { get; set; }
        public virtual int FacesCount { get; set; }
        public virtual int TangentsesCount { get; set; }

        public virtual string ShaderName { get; set; }
        public virtual string[] Textures { get; set; }
        
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        public abstract Vector3[] GetVertices();
        public abstract Vector2[] GetTextureCoords();
        public abstract Vector3[] GetNormals();
        public abstract uint[] GetFaceIndeces(uint offset = 0);
        public abstract Vector4[] GetTangentses();
        public abstract void CalculateModelMatrix();
    }
}
