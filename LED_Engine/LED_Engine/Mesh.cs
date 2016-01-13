using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Meshes
    {
        public static List<Mesh> MESHES = new List<Mesh>(); // Loaded Meshes
        public static List<Mesh> MeshesList = new List<Mesh>(); // All Meshes

        public static void LoadMeshesList(string XmlFile, string MeshPath, bool EngineContent)
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;

                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("Mesh");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Mesh M = new Mesh();
                    M.EngineContent = EngineContent;
                    M.Name = xmlNode.SelectSingleNode("Name").InnerText;
                    M.FileName = Engine.CombinePaths(MeshPath, xmlNode.SelectSingleNode("File").InnerText);

                    MeshesList.Add(M);
                }

            }
            catch (Exception e)
            {
                Log.WriteLineRed("Models.LoadMeshesList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                Log.WriteLineYellow(e.Message);
            }
        }

        public static Mesh GetMesh(string Name)
        {
            foreach (var i in MESHES)
                if (i.Name.GetHashCode() == Name.GetHashCode())
                    return i;
            return null;
        }

        public static Mesh LoadMesh(string Name, bool UseSmoothingGroups = false)
        {
            try
            {
                Mesh M = GetMesh(Name);

                if (M == null)
                {
                    foreach (var i in MeshesList)
                        if (i.Name.GetHashCode() == Name.GetHashCode())
                            M = i;
                    if (M == null)
                        return null;
                }

                if (M.UseCounter == 0)
                {
                    for (int i = 0; i < M.Materials.Count; i++)
                        Materials.Load(M.Materials[i].Name);

                    MESHES.Add(M);
                }

                M.UseCounter++;
                return M;
            }
            catch
            {
                Log.WriteLineRed("Meshes.LoadMesh() Exception, Name: \"{0}\"", Name);
                return null;
            }
        }

        public static void Unload(string Name)
        {
            Mesh M = GetMesh(Name);
            if (M != null)
            {
                M.UseCounter--;

                if (M.UseCounter == 0)
                {
                    if (!M.EngineContent)
                        MESHES.Remove(M);
                }
            }
        }

        public static void Free(bool WithEngineContent = false)
        {
            try
            {
                if (WithEngineContent)
                {
                    MESHES.Clear();
                    MeshesList.Clear();
                }
                else
                {
                    int Count = MESHES.Count;
                    for (int i = 0; i < Count; i++)
                        if (!MESHES[i].EngineContent)
                        {
                            MESHES.RemoveAt(i);
                            Count--;
                        }
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Meshes.Free() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }
    }

    class Face
    {
        public int[] V = new int[3];
        public int[] VT = new int[3];
        public int[] VN = new int[3];
        public int MaterialID = -1;
        public UInt32 SmoothingGroup = 0;

        public int V0
        {
            get { return V[0]; }
            set { V[0] = value; }
        }

        public int V1
        {
            get { return V[1]; }
            set { V[1] = value; }
        }

        public int V2
        {
            get { return V[2]; }
            set { V[2] = value; }
        }

        public int VT0
        {
            get { return VT[0]; }
            set { VT[0] = value; }
        }

        public int VT1
        {
            get { return VT[1]; }
            set { VT[1] = value; }
        }

        public int VT2
        {
            get { return VT[2]; }
            set { VT[2] = value; }
        }

        public int VN0
        {
            get { return VN[0]; }
            set { VN[0] = value; }
        }

        public int VN1
        {
            get { return VN[1]; }
            set { VN[1] = value; }
        }

        public int VN2
        {
            get { return VN[2]; }
            set { VN[2] = value; }
        }
    }

    class FacePart
    {
        public int V = 0, VN = 0;
        public UInt32 SmoothingGroup = 0;

        public FacePart()
        {
        }

        public FacePart(int V)
        {
            this.V = V;
        }

        public FacePart(UInt32 SmoothingGroup)
        {
            this.SmoothingGroup = SmoothingGroup;
        }

        public FacePart(int V, int VN)
        {
            this.V = V;
            this.VN = VN;
        }

        public FacePart(int V, int VN, UInt32 SmoothingGroup)
        {
            this.V = V;
            this.VN = VN;
            this.SmoothingGroup = SmoothingGroup;
        }
    }

    public class Mesh
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ModelViewMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        public int IndexBufferID, VertexBufferID, NormalBufferID, UVBufferID, TangentBufferID;

        public Vector3[] Vertexes, Normals, Tangents;
        public Vector2[] UVs;
        int[] indexes;

        public List<Material> Materials = new List<Material>();

        public string Name = String.Empty;
        public string FileName = String.Empty;
        public uint UseCounter = 0;
        public bool EngineContent = false;

        public Mesh()
        {

        }

        public Mesh(Mesh Original, bool CopyMaterials = false)
        {
            indexes = new int[Original.IndexesCount];
            Vertexes = new Vector3[Original.Vertexes.Length];
            Normals = new Vector3[Original.Normals.Length];
            UVs = new Vector2[Original.UVs.Length];
            Tangents = new Vector3[Original.Tangents.Length];

            Original.Indexes().CopyTo(indexes, 0);
            Original.Vertexes.CopyTo(Vertexes, 0);
            Original.Normals.CopyTo(Normals, 0);
            Original.UVs.CopyTo(UVs, 0);
            Original.Tangents.CopyTo(Tangents, 0);

            if (CopyMaterials)
                Materials.AddRange(Original.Materials);

            Name = Original.Name;
            FileName = Original.FileName;

            Position = Original.Position;
            Rotation = Original.Rotation;
            Scale = Original.Scale;

            GenBuffers();
            BindBuffers();
        }

        public void GenBuffers()
        {
            FreeBuffers();

            IndexBufferID = GL.GenBuffer();
            VertexBufferID = GL.GenBuffer();
            NormalBufferID = GL.GenBuffer();
            UVBufferID = GL.GenBuffer();
            TangentBufferID = GL.GenBuffer();
        }

        public void BindBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Vertexes.Length * Vector3.SizeInBytes), Vertexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBufferID);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Normals.Length * Vector3.SizeInBytes), Normals, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, UVBufferID);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(UVs.Length * Vector2.SizeInBytes), UVs, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, TangentBufferID);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Tangents.Length * Vector3.SizeInBytes), Tangents, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(IndexesCount * sizeof(int)), Indexes(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void FreeBuffers()
        {
            GL.DeleteBuffer(IndexBufferID);
            GL.DeleteBuffer(VertexBufferID);
            GL.DeleteBuffer(NormalBufferID);
            GL.DeleteBuffer(UVBufferID);
            GL.DeleteBuffer(TangentBufferID);

            IndexBufferID = 0;
            VertexBufferID = 0;
            NormalBufferID = 0;
            UVBufferID = 0;
            TangentBufferID = 0;
        }

        public void Free()
        {
            FreeBuffers();

            indexes = null;
            Vertexes = null;
            Normals = null;
            UVs = null;
            Tangents = null;
        }

        public int IndexesCount { get { return indexes.Length; } }

        public int[] Indexes(int offset = 0)
        {
            int[] inds = new int[indexes.Length];

            for (int i = 0; i < indexes.Length; i++)
                inds[i] = indexes[i] + offset;

            return inds;
        }

        public void Indexes(int[] Indexes)
        {
            indexes = Indexes;
        }

        public void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }

        static void ComputeTangentBasis(Vector3[] Vertices, Vector3[] Normals, Vector2[] UVs, out Vector3[] Tangents)
        {
            Tangents = new Vector3[Vertices.Length];
            Vector3[] Tangents2 = new Vector3[Tangents.Length];

            try
            {
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

                    Tangents[i] = new Vector3(
                        (t2 * q1.X - t1 * q2.X) * r,
                        (t2 * q1.Y - t1 * q2.Y) * r,
                        (t2 * q1.Z - t1 * q2.Z) * r);

                    Tangents2[i] = new Vector3(
                        (s1 * q2.X - s2 * q1.X) * r,
                        (s1 * q2.Y - s2 * q1.Y) * r,
                        (s1 * q2.Z - s2 * q1.Z) * r);

                    Tangents[i + 1] = Tangents[i];
                    Tangents[i + 2] = Tangents[i];
                    Tangents2[i + 1] = Tangents2[i];
                    Tangents2[i + 2] = Tangents2[i];
                }

                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vector3 n = Normals[i];
                    Vector3 t1 = Tangents[i];
                    Vector3 t2 = Tangents2[i];

                    // Gram-Schmidt orthogonalize.
                    Tangents[i] = Vector3.Normalize(t1 - (Vector3.Dot(n, t1) * n));

                    if (Vector3.Dot(Vector3.Cross(n, t1), t2) < 0.0f)
                        Tangents[i] *= -1.0f;
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Mesh.ComputeTangentBasis() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }

        static void SmoothNormals(ref List<Face> Faces, ref List<Vector3> Normals)
        {
            try
            {
                #region Make FacePart
                List<FacePart> FacePart = new List<FacePart>();
                for (int i = 0; i < Faces.Count * 3; i++)
                {
                    if (Faces[i / 3].SmoothingGroup > 0)
                    {
                        int V = Faces[i / 3].V[i % 3];
                        int VN = Faces[i / 3].VN[i % 3];
                        UInt32 SG = Faces[i / 3].SmoothingGroup;
                        FacePart.Add(new FacePart(V, VN, SG));
                    }
                }
                #endregion

                #region Select -> Dictionary<V_index, List<N_index>>
                Dictionary<int, List<int>> Selected = new Dictionary<int, List<int>>(); // Dictionary<V_index, List<N_index>>
                List<int> Added_ij = new List<int>();

                for (int i = 0; i < FacePart.Count; i++)
                {
                    for (int j = 0; j < FacePart.Count; j++)
                    {
                        if ((i != j) && (FacePart[i].V == FacePart[j].V) && ((FacePart[i].SmoothingGroup & FacePart[j].SmoothingGroup) > 0))
                        {
                            if (Selected.ContainsKey(FacePart[i].V))
                            {
                                if (!Added_ij.Contains(i))
                                {
                                    Selected[FacePart[i].V].Add(FacePart[i].VN);
                                    Added_ij.Add(i);
                                }

                                if (!Added_ij.Contains(j))
                                {
                                    Selected[FacePart[i].V].Add(FacePart[j].VN);
                                    Added_ij.Add(j);
                                }
                            }
                            else
                            {
                                List<int> L = new List<int>();
                                L.Add(FacePart[i].VN);
                                L.Add(FacePart[j].VN);
                                Selected.Add(FacePart[i].V, L);

                                Added_ij.Add(i);
                                Added_ij.Add(j);
                            }
                        }
                    }
                }
                #endregion

                foreach (var item in Selected)
                {
                    Vector3 SmoothedNormal = new Vector3();

                    for (int i = 0; i < item.Value.Count; i++)
                        SmoothedNormal += Normals[item.Value[i]];

                    SmoothedNormal.Normalize();

                    for (int i = 0; i < FacePart.Count; i++)
                        if (FacePart[i].V == item.Key)
                        {
                            Normals.Add(SmoothedNormal);
                            Faces[i / 3].VN[i % 3] = Normals.Count - 1;
                        }
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("ObjVolume.SmoothNormals() Exception:");
                Log.WriteLineYellow("Message: \"{0}\"\n" + e.Message);
            }
        }

        public static Mesh LoadFromFile(string Name, string FileName, bool UseSmoothingGroups = false)
        {
            Mesh obj = null;
            try
            {
                obj = LoadFromString(Name, File.ReadAllText(FileName), UseSmoothingGroups);
                obj.FileName = FileName;
            }
            catch (FileNotFoundException e)
            {
                Log.WriteLineRed("Mesh.LoadFromFile() FileNotFoundException:");
                Log.WriteLineYellow("File not found: \"{0}\"\n\"{1}\"", FileName, e.Message);
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Mesh.LoadFromFile() Exception:");
                Log.WriteLineYellow("Error in file: \"{0}\"\n\"{1}\"", FileName, e.Message);
            }
            return obj;
        }

        public static Mesh LoadFromString(string Name, string obj, bool UseSmoothingGroups = false)
        {
            try
            {
                List<string> Lines = new List<string>(obj.Split('\n'));
                Dictionary<string, int> Materials = new Dictionary<string, int>();
                UInt32 CurrentSmoothingGroup = 0;
                int CurrentMaterial = 0;

                #region Remove all comments, empty string; convert ToLower()
                for (int i = 0; i < Lines.Count; i++)
                {
                    string L = Lines[i].Trim(new char[] { ' ', '\r' });
                    if (L.Length == 0 || (L.Length > 0 && L.Substring(0, 1) == "#"))
                    {
                        Lines.RemoveAt(i);
                        i--;
                    }
                    else
                        Lines[i] = L.ToLower();
                }
                #endregion

                #region Triangulate
                List<String> LinesTriangulated = new List<string>();
                for (int i = 0; i < Lines.Count; i++)
                {
                    if (Lines[i].Substring(0, 1) == "f")
                    {
                        string[] FaceParts = Lines[i].Split(new char[] { ' ' });
                        if (FaceParts.Length > 4)
                            for (int j = 1; j < FaceParts.Length - 2; j++)
                                LinesTriangulated.Add(FaceParts[0] + " " + FaceParts[1] + " " + FaceParts[j + 1] + " " + FaceParts[j + 2]);
                        else
                            LinesTriangulated.Add(Lines[i]);
                    }
                    else
                        LinesTriangulated.Add(Lines[i]);
                }
                Lines = LinesTriangulated;
                LinesTriangulated = null;
                #endregion

                // Списки для хранения данных модели
                List<Vector3> Vertices = new List<Vector3>();
                List<Vector3> Normals = new List<Vector3>();
                List<Vector2> UVs = new List<Vector2>();
                List<Face> Faces = new List<Face>();

                string[] lineparts;
                for (int i = 0; i < Lines.Count; i++)
                {
                    #region Parse Lines
                    lineparts = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lineparts.Length > 1)
                    {
                        switch (lineparts[0])
                        {
                            case "v":
                                #region Vertex
                                try
                                {
                                    if (lineparts.Length >= 4)
                                    {
                                        float x = float.Parse(lineparts[1]) * Settings.UnitsScale;
                                        float y = float.Parse(lineparts[2]) * Settings.UnitsScale;
                                        float z = float.Parse(lineparts[3]) * Settings.UnitsScale;
                                        Vertices.Add(new Vector3(x, y, z));
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("Error parsing Vertex in line " + (i + 1).ToString() + ": " + Lines[i]);
                                }
                                #endregion
                                break;

                            case "vt":
                                #region TexCoord
                                try
                                {
                                    if (lineparts.Length >= 3)
                                    {
                                        float u = float.Parse(lineparts[1]);
                                        float v = float.Parse(lineparts[2]);
                                        UVs.Add(new Vector2(u, v));
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("Error parsing TextureCoords in line " + (i + 1).ToString() + ": " + Lines[i]);
                                }
                                #endregion
                                break;

                            case "vn":
                                #region Normal
                                try
                                {
                                    if (lineparts.Length >= 4)
                                    {
                                        float nx = float.Parse(lineparts[1]);
                                        float ny = float.Parse(lineparts[2]);
                                        float nz = float.Parse(lineparts[3]);
                                        Normals.Add(new Vector3(nx, ny, nz));
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("Error parsing Normals in line " + (i + 1).ToString() + ": " + Lines[i]);
                                }
                                #endregion
                                break;

                            case "f":
                                #region Face
                                try
                                {
                                    Face Face = new Face();

                                    for (int j = 0; j < lineparts.Length - 1; j++)
                                    {
                                        String[] FaceParams = lineparts[j + 1].Split('/'); // v, v/vt, v//vn, v/vt/vn
                                        int FaceV_Index, FaceVT_Index, FaceVN_Index;

                                        switch (FaceParams.Length)
                                        {
                                            case 1: // "v"
                                                FaceV_Index = int.Parse(FaceParams[0]);
                                                if (FaceV_Index > 0)
                                                    Face.V[j] = FaceV_Index - 1;
                                                else if (FaceV_Index < 0)
                                                    Face.V[j] = Vertices.Count + FaceV_Index;
                                                break;

                                            case 2: // "v/vt"
                                                FaceV_Index = int.Parse(FaceParams[0]);
                                                if (FaceV_Index > 0)
                                                    Face.V[j] = FaceV_Index - 1;
                                                else if (FaceV_Index < 0)
                                                    Face.V[j] = Vertices.Count + FaceV_Index;

                                                FaceVT_Index = int.Parse(FaceParams[1]);
                                                if (FaceVT_Index > 0)
                                                    Face.VT[j] = FaceVT_Index - 1;
                                                else if (FaceVT_Index < 0)
                                                    Face.VT[j] = UVs.Count + FaceVT_Index;
                                                break;

                                            case 3: // "v//vn"
                                                if (FaceParams[1].Trim() == String.Empty)
                                                {
                                                    FaceV_Index = int.Parse(FaceParams[0]);
                                                    if (FaceV_Index > 0)
                                                        Face.V[j] = FaceV_Index - 1;
                                                    else if (FaceV_Index < 0)
                                                        Face.V[j] = Vertices.Count + FaceV_Index;

                                                    FaceVN_Index = int.Parse(FaceParams[2]);
                                                    if (FaceVN_Index > 0)
                                                        Face.VN[j] = FaceVN_Index - 1;
                                                    else if (FaceVN_Index < 0)
                                                        Face.VN[j] = Normals.Count + FaceVN_Index;
                                                }
                                                else // "v/vt/vn"
                                                {
                                                    FaceV_Index = int.Parse(FaceParams[0]);
                                                    if (FaceV_Index > 0)
                                                        Face.V[j] = FaceV_Index - 1;
                                                    else if (FaceV_Index < 0)
                                                        Face.V[j] = Vertices.Count + FaceV_Index;

                                                    FaceVT_Index = int.Parse(FaceParams[1]);
                                                    if (FaceVT_Index > 0)
                                                        Face.VT[j] = FaceVT_Index - 1;
                                                    else if (FaceVT_Index < 0)
                                                        Face.VT[j] = UVs.Count + FaceVT_Index;

                                                    FaceVN_Index = int.Parse(FaceParams[2]);
                                                    if (FaceVN_Index > 0)
                                                        Face.VN[j] = FaceVN_Index - 1;
                                                    else if (FaceVN_Index < 0)
                                                        Face.VN[j] = Normals.Count + FaceVN_Index;
                                                }
                                                break;
                                        }
                                    }
                                    Face.SmoothingGroup = CurrentSmoothingGroup;
                                    Face.MaterialID = CurrentMaterial;
                                    Faces.Add(Face);
                                }
                                catch
                                {
                                    MessageBox.Show("Error parsing Face in line " + (i + 1).ToString() + ": " + Lines[i]);
                                }
                                #endregion
                                break;

                            case "s":
                                #region SmoothingGroup
                                if (lineparts[1] == "off")
                                    CurrentSmoothingGroup = 0;
                                else
                                    CurrentSmoothingGroup = Convert.ToUInt32(lineparts[1]);
                                #endregion
                                break;

                            case "usemtl":
                                #region Material
                                if (Materials.ContainsKey(lineparts[1]))
                                    CurrentMaterial = Materials[lineparts[1]];
                                else
                                {
                                    Materials.Add(lineparts[1], Materials.Count);
                                    CurrentMaterial = Materials.Count - 1;
                                }
                                #endregion
                                break;
                        }
                    }
                    #endregion
                }

                // Если нет текстурных координат, то создаем одну.
                if (UVs.Count == 0)
                    UVs.Add(new Vector2());

                // Создаем ObjVolume
                Mesh vol = new Mesh();
                vol.Name = Name;
                vol.Vertexes = new Vector3[Faces.Count * 3];
                vol.UVs = new Vector2[Faces.Count * 3];
                vol.Normals = new Vector3[Faces.Count * 3];
                vol.indexes = new int[Faces.Count * 3];

                #region If need Calc Normals
                if (Normals.Count == 0)
                {
                    for (int i = 0; i < Faces.Count; i++)
                    {
                        Vector3 U = Vertices[Faces[i].V1] - Vertices[Faces[i].V0];
                        Vector3 V = Vertices[Faces[i].V2] - Vertices[Faces[i].V0];
                        Normals.Add(Vector3.Cross(U, V));
                        Faces[i].VN0 = i;
                        Faces[i].VN1 = i;
                        Faces[i].VN2 = i;
                    }
                }
                #endregion

                // Если нужно, используем группы сглаживания
                if (UseSmoothingGroups)
                    SmoothNormals(ref Faces, ref Normals);

                for (int i = 0; i < Faces.Count * 3; i++)
                {
                    vol.Vertexes[i] = Vertices[Faces[i / 3].V[i % 3]];
                    vol.UVs[i] = UVs[Faces[i / 3].VT[i % 3]];
                    vol.Normals[i] = Normals[Faces[i / 3].VN[i % 3]];
                    vol.indexes[i] = i;
                }

                ComputeTangentBasis(vol.Vertexes, vol.Normals, vol.UVs, out vol.Tangents);

                vol.GenBuffers();
                vol.BindBuffers();

                //Cleanup
                Vertices = null;
                UVs = null;
                Normals = null;
                Faces = null;
                Materials = null;
                return vol;
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Mesh.LoadFromString() Exception:");
                Log.WriteLineYellow("Message: \"{0}\"", e.Message);
                return null;
            }
        }

        public static Mesh MakePlain(float Sides = 1.0f)
        {
            return MakePlain(Sides, Sides);
        }

        public static Mesh MakePlain(float SideA , float SideB)
        {
            if (SideA < 0.0f)
                SideA = -SideA;
            if (SideB < 0.0f)
                SideB = -SideB;

            float HalfA = SideA / 2.0f;
            float HalfB = SideB / 2.0f;

            Mesh plain = new Mesh();
            plain.Name = "Plain";

            #region Vertexes, Normals, UVs, Indexes
            plain.Vertexes = new Vector3[]
            {
                new Vector3(-HalfA,  0, -HalfB),
                new Vector3(-HalfA,  0,  HalfB),
                new Vector3( HalfA,  0,  HalfB),
                new Vector3(-HalfA,  0, -HalfB),
                new Vector3( HalfA,  0,  HalfB),
                new Vector3( HalfA,  0, -HalfB)
            };

            plain.Normals = new Vector3[]
            {
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };

            plain.UVs = new Vector2[] {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
            };

            plain.Indexes(new int[] { 0, 1, 2, 3, 4, 5 });
            #endregion

            ComputeTangentBasis(plain.Vertexes, plain.Normals, plain.UVs, out plain.Tangents);

            plain.GenBuffers();
            plain.BindBuffers();

            return plain;
        }

        public static Mesh MakeBox(float Sides = 1.0f, bool FlipPolygons = false)
        {
            return MakeBox(Sides, Sides, Sides, FlipPolygons);
        }

        public static Mesh MakeBox(float SideA, float SideB, float SideC, bool FlipPolygons = false)
        {
            if (SideA < 0.0f)
                SideA = -SideA;
            if (SideB <= 0.0f)
                SideB = -SideB;
            if (SideC <= 0.0f)
                SideC = -SideC;

            float HalfA = SideA / 2.0f;
            float HalfB = SideB / 2.0f;
            float HalfC = SideC / 2.0f;

            Mesh box = new Mesh();
            box.Name = "Box";

            #region Vertexes, Normals, UVs, Indexes
            if (FlipPolygons)
            {
                box.Vertexes = new Vector3[] {
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    new Vector3( HalfA, -HalfB,  HalfC), //5
                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3(-HalfA, -HalfB, -HalfC), //0
                    new Vector3(-HalfA, -HalfB,  HalfC), //1

                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3( HalfA, -HalfB,  HalfC), //5
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    
                    new Vector3( HalfA, -HalfB,  HalfC), //5
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3( HalfA, -HalfB,  HalfC), //5

                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3(-HalfA, -HalfB, -HalfC), //0
                    new Vector3( HalfA, -HalfB, -HalfC), //4

                    new Vector3(-HalfA, -HalfB, -HalfC), //0
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    new Vector3(-HalfA, -HalfB, -HalfC) }; //0

                box.Normals = new Vector3[] {
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

                box.UVs = new Vector2[] {
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
                    new Vector2(0, 0)};
            }
            else
            {
                box.Vertexes = new Vector3[] {
                    new Vector3(-HalfA, -HalfB,  HalfC),
                    new Vector3(-HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB,  HalfC),
                    new Vector3(-HalfA, -HalfB,  HalfC),

                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),

                    new Vector3(-HalfA, -HalfB,  HalfC),
                    new Vector3( HalfA, -HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA, -HalfB,  HalfC),

                    new Vector3( HalfA, -HalfB,  HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA, -HalfB,  HalfC),

                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3(-HalfA, -HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),

                    new Vector3(-HalfA, -HalfB, -HalfC),
                    new Vector3(-HalfA, -HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA, -HalfB, -HalfC) };

                box.Normals = new Vector3[] {
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

                box.UVs = new Vector2[] {
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

            box.Indexes(new int[] {
                    0, 1, 2, 3, 4, 5,
                    6, 7, 8, 9, 10, 11,
                    12, 13, 14, 15, 16, 17,
                    18, 19, 20, 21, 22, 23,
                    24, 25, 26, 27, 28, 29,
                    30, 31, 32, 33, 34, 35 });
            #endregion

            ComputeTangentBasis(box.Vertexes, box.Normals, box.UVs, out box.Tangents);

            box.GenBuffers();
            box.BindBuffers();

            return box;
        }
    }
}
