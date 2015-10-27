using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    class ObjVolume : Volume
    {
        int indexBufferID, vertexBufferID, normalBufferID, uvBufferID, tangentBufferID;
        int[] indeces;
        Vector3[] vertices, normals;
        Vector2[] uvs;
        Vector4[] tangents;

        public ObjVolume()
            : base()
        {
            Material = new Material();

            GenBuffers();
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

        public override Vector3[] GetVertices()
        {
            return vertices;
        }

        public override Vector3[] GetNormals()
        {
            return normals;
        }

        public override int[] GetIndeces(int offset = 0)
        {
            int[] inds = new int[indeces.Length];

            for (int i = 0; i < indeces.Length; i++)
                inds[i] = indeces[i] + offset;

            return inds;
        }

        public override Vector2[] GetUVs()
        {
            return uvs;
        }

        public override Vector4[] GetTangents()
        {
            return tangents;
        }

        public static void ComputeTangentBasis(Vector3[] Vertices, Vector3[] Normals, Vector2[] UVs, out Vector4[] Tangents)
        {
            Tangents = new Vector4[Vertices.Length];
            Vector3[] tan1Accum = new Vector3[Vertices.Length]; //Tangents
            Vector3[] tan2Accum = new Vector3[Vertices.Length]; //Bitangents

            // Compute the tangent vector
            for (int i = 0; i < Vertices.Length; i += 3)
            {
                Vector3 p1 = Vertices[i];
                Vector3 p2 = Vertices[i + 1];
                Vector3 p3 = Vertices[i + 2];

                Vector2 tc1 = UVs[i];
                Vector2 tc2 = UVs[i + 1];
                Vector2 tc3 = UVs[i + 2];

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

                tan1Accum[i] = tan1;
                tan1Accum[i + 1] = tan1;
                tan1Accum[i + 2] = tan1;

                tan2Accum[i] = tan2;
                tan2Accum[i + 1] = tan2;
                tan2Accum[i + 2] = tan2;
            }

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector3 n = Normals[i];
                Vector3 t1 = tan1Accum[i];
                Vector3 t2 = tan2Accum[i];

                // Gram-Schmidt orthogonalize. Store handedness in W.
                Tangents[i] = new Vector4(Vector3.Normalize(t1 - (Vector3.Dot(n, t1) * n)), 1.0f);

                if (Vector3.Dot(Vector3.Cross(n, t1), t2) < 0.0f)
                    Tangents[i].W = -1.0f;
            }
            tan1Accum = null;
            tan2Accum = null;
        }

        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }

        public static ObjVolume LoadFromFile(string filename)
        {
            ObjVolume obj = new ObjVolume();
            try
            {
                using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                    obj = LoadFromString(reader.ReadToEnd());
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("File not found: {0}\n\n" + e.Message, filename);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file: {0}\n\n" + e.Message, filename);
            }

            return obj;
        }

        public static ObjVolume LoadFromString(string obj)
        {
            // Отделите строки из загруженного файла
            List<String> lines = new List<string>(obj.Split('\n'));

            // Списки для хранения данных модели
            List<Vector3> Vertices = new List<Vector3>();
            List<Vector3> Normals = new List<Vector3>();
            List<Vector2> UVs = new List<Vector2>();
            List<int> FacesV = new List<int>();
            List<int> FacesT = new List<int>();
            List<int> FacesN = new List<int>();

            String FloatComa = (0.5f).ToString().Substring(1, 1);

            string[] lineparts;
            // Построчное считывание
            for (int i = 0; i < lines.Count; i++)
            {
                lineparts = lines[i].Replace(".", FloatComa).Replace("  ", " ").Trim().Split(' ');

                switch (lineparts[0])
                {
                    case "p": // Point
                        break;

                    case "v": // Vertex
                        try
                        {
                            if (lineparts.Length >= 4)
                            {
                                float x = float.Parse(lineparts[1]) * Game.UnitsScale;
                                float y = float.Parse(lineparts[2]) * Game.UnitsScale;
                                float z = float.Parse(lineparts[3]) * Game.UnitsScale;
                                Vertices.Add(new Vector3(x, y, z));
                                break;
                            }
                            else
                                break;
                        }
                        catch
                        {
                            MessageBox.Show("Error parsing Vertex in line " + (i + 1).ToString() + ": " + lines[i]);
                            break;
                        }

                    case "vt": // TexCoord
                        try
                        {
                            if (lineparts.Length >= 3)
                            {
                                float u = float.Parse(lineparts[1]);
                                float v = float.Parse(lineparts[2]);
                                UVs.Add(new Vector2(u, v));
                                break;
                            }
                            else
                                break;
                        }
                        catch
                        {
                            MessageBox.Show("Error parsing TextureCoords in line " + (i + 1).ToString() + ": " + lines[i]);
                            break;
                        }

                    case "vn": // Normal
                        try
                        {
                            if (lineparts.Length >= 4)
                            {
                                float nx = float.Parse(lineparts[1]);
                                float ny = float.Parse(lineparts[2]);
                                float nz = float.Parse(lineparts[3]);
                                Normals.Add(new Vector3(nx, ny, nz));
                                break;
                            }
                            else
                                break;
                        }
                        catch
                        {
                            MessageBox.Show("Error parsing Normals in line " + (i + 1).ToString() + ": " + lines[i]);
                            break;
                        }

                    case "f":
                        try
                        {
                            int[,] FaceVTN = new int[4, 3]; // VertexIndex/TextureCoordIndex/NormalIndex

                            if (lineparts.Length == 4 || lineparts.Length == 5) //Triangle or Quad
                            {
                                for (int j = 0; j < lineparts.Length - 1; j++)
                                {
                                    String[] FaceParams = lineparts[j + 1].Split('/');
                                    if (FaceParams.Length == 3) // If Vertex/TexCoords/Normals
                                    {
                                        foreach (string fp in FaceParams)
                                            if (fp.Trim() == String.Empty)
                                                throw new Exception();
                                    }
                                    else
                                        throw new Exception();

                                    for (int k = 0; k < 3; k++) //Add Vertex/TexCoords/Normals
                                        FaceVTN[j, k] = int.Parse(FaceParams[k]) - 1;
                                }

                                //Add first triangle if Triangle
                                for (int j = 0; j < 3; j++)
                                {
                                    FacesV.Add(FaceVTN[j, 0]);
                                    FacesT.Add(FaceVTN[j, 1]);
                                    FacesN.Add(FaceVTN[j, 2]);
                                }

                                //Add second triangle if Quad
                                if (lineparts.Length == 5)
                                    for (int j = 2; j < 5; j++)
                                    {
                                        FacesV.Add(FaceVTN[j % 4, 0]);
                                        FacesT.Add(FaceVTN[j % 4, 1]);
                                        FacesN.Add(FaceVTN[j % 4, 2]);
                                    }
                            }
                            else
                                throw new Exception();
                            break;
                        }
                        catch
                        {
                            MessageBox.Show("Error parsing Face in line " + (i + 1).ToString() + ": " + lines[i]);
                            break;
                        }
                }
            }

            // Создаем ObjVolume
            ObjVolume vol = new ObjVolume();
            vol.vertices = new Vector3[FacesV.Count];
            vol.uvs = new Vector2[FacesV.Count];
            vol.normals = new Vector3[FacesV.Count];
            vol.indeces = new int[FacesV.Count];

            for (int i = 0; i < FacesV.Count; i++)
            {
                vol.vertices[i] = Vertices[FacesV[i]];
                vol.uvs[i] = UVs[FacesT[i]];
                vol.normals[i] = Normals[FacesN[i]];
                vol.indeces[i] = i;
            }

            ComputeTangentBasis(vol.vertices, vol.normals, vol.uvs, out vol.tangents);

            BindBuffer_BufferData(vol);

            //Cleanup
            Vertices = null;
            UVs = null;
            Normals = null;
            FacesV = null;
            FacesT = null;
            FacesN = null;
            return vol;
        }

        public static void BindBuffer_BufferData(Volume vol)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vol.IndexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vol.IndecesCount * sizeof(int)), vol.GetIndeces(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vol.VertexBufferID);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vol.VerticesCount * Vector3.SizeInBytes), vol.GetVertices(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vol.NormalBufferID);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vol.NormalsCount * Vector3.SizeInBytes), vol.GetNormals(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vol.UVBufferID);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(vol.UVsCount * Vector2.SizeInBytes), vol.GetUVs(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vol.TangentBufferID);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(vol.TangentsCount * Vector4.SizeInBytes), vol.GetTangents(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
