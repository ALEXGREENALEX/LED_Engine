using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

/// <summary>
/// �� ������������ !!!
/// </summary>
public class ObjMesh
{
    public ObjMesh(string fileName)
    {
        ObjMeshLoader.Load(this, fileName);
    }

    public ObjVertex[] Vertices
    {
        get { return vertices; }
        set { vertices = value; }
    }
    ObjVertex[] vertices;

    public ObjTriangle[] Triangles
    {
        get { return triangles; }
        set { triangles = value; }
    }
    ObjTriangle[] triangles;

    public ObjQuad[] Quads
    {
        get { return quads; }
        set { quads = value; }
    }
    ObjQuad[] quads;

    int verticesBufferId;
    int trianglesBufferId;
    int quadsBufferId;

    public void Prepare()
    {
        if (verticesBufferId == 0)
        {
            GL.GenBuffers(1, out verticesBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verticesBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(ObjVertex))), vertices, BufferUsageHint.StaticDraw);
        }

        if (trianglesBufferId == 0)
        {
            GL.GenBuffers(1, out trianglesBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, trianglesBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf(typeof(ObjTriangle))), triangles, BufferUsageHint.StaticDraw);
        }

        if (quadsBufferId == 0)
        {
            GL.GenBuffers(1, out quadsBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadsBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf(typeof(ObjQuad))), quads, BufferUsageHint.StaticDraw);
        }
    }

    public void Render()
    {
        Prepare();

        GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
        GL.EnableClientState(ArrayCap.VertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, verticesBufferId);
        GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf(typeof(ObjVertex)), IntPtr.Zero);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, trianglesBufferId);
        GL.DrawElements(PrimitiveType.Triangles, triangles.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

        if (quads.Length > 0)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadsBufferId);
            GL.DrawElements(PrimitiveType.Quads, quads.Length * 4, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        GL.PopClientAttrib();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjVertex
    {
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Vertex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjTriangle
    {
        public int Index0;
        public int Index1;
        public int Index2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjQuad
    {
        public int Index0;
        public int Index1;
        public int Index2;
        public int Index3;
    }
}
