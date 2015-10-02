using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace OpenGL_CS_Game
{
    class ObjVolume : Volume
    {
        Vector3[] vertices;
        Vector3[] normals;
        Vector2[] texturecoords;
        Vector4[] tangentses;
        int[] indexes;

        public ObjVolume()
            : base()
        {
            Material = new Material();
        }

        public override int VerticesCount { get { return vertices.Length; } }
        public override int NormalsCount { get { return normals.Length; } }
        public override int IndexesCount { get { return indexes.Length; } }
        public override int TextureCoordsCount { get { return texturecoords.Length; } }
        public override int TangentsesCount { get { return tangentses.Length; } }

        /// <summary>
        /// Получить вершины этого объекта
        /// </summary>
        /// <returns></returns>
        public override Vector3[] GetVertices()
        {
            return vertices;
        }

        /// <summary>
        /// Получить нормали этого объекта
        /// </summary>
        /// <returns></returns>
        public override Vector3[] GetNormals()
        {
            return normals;
        }

        /// <summary>
        /// Получить индексы что-бы нарисовать этот объект
        /// </summary>
        /// <returns>Array of indices offset to match buffered data</returns>
        public override int[] GetIndexes(int offset = 0)
        {
            int[] inds = new int[indexes.Length];

            for (int i = 0; i < indexes.Length; i++)
                inds[i] = indexes[i] + offset;

            return inds;
        }

        /// <summary>
        /// Получить текстурные координаты
        /// </summary>
        /// <returns></returns>
        public override Vector2[] GetTextureCoords()
        {
            return texturecoords;
        }

        public override Vector4[] GetTangentses()
        {
            return tangentses;
        }

        public static Vector4[] CalcTangentses(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords, int[] indexes)
        {
            Vector4[] Tangenses = new Vector4[vertices.Length];
            List<Vector3> tan1Accum = new List<Vector3>();
            List<Vector3> tan2Accum = new List<Vector3>();

            for (uint i = 0; i < vertices.Length; i++)
            {
                tan1Accum.Add(new Vector3(0.0f));
                tan2Accum.Add(new Vector3(0.0f));
            }

            // Compute the tangent vector
            for (int i = 0; i < indexes.Length; i += 3)
            {
                Vector3 p1 = vertices[indexes[i]];
                Vector3 p2 = vertices[indexes[i + 1]];
                Vector3 p3 = vertices[indexes[i + 2]];

                Vector2 tc1 = texCoords[indexes[i]];
                Vector2 tc2 = texCoords[indexes[i + 1]];
                Vector2 tc3 = texCoords[indexes[i + 2]];

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

                tan1Accum[indexes[i]] += tan1;
                tan1Accum[indexes[i + 1]] += tan1;
                tan1Accum[indexes[i + 2]] += tan1;
                tan2Accum[indexes[i]] += tan2;
                tan2Accum[indexes[i + 1]] += tan2;
                tan2Accum[indexes[i + 2]] += tan2;
            }

            for (int i = 0; i < vertices.Length; ++i)
            {
                Vector3 n = normals[i];
                Vector3 t1 = tan1Accum[i];
                Vector3 t2 = tan2Accum[i];

                // Gram-Schmidt orthogonalize
                Tangenses[i] = new Vector4(Vector3.Normalize(t1 - (Vector3.Dot(n, t1) * n)), 0.0f);
                // Store handedness in W
                Vector4 V_temp = Tangenses[i];
                
                if (Vector3.Dot(Vector3.Cross(n, t1), t2) < 0.0f)
                    V_temp.W = -1.0f;
                else
                    V_temp.W = 1.0f;

                Tangenses[i] = V_temp;
            }

            tan1Accum.Clear();
            tan2Accum.Clear();
            return Tangenses;
        }

        /// <summary>
        /// Рассчитывает матрицу модели из трансформаций
        /// </summary>
        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }

        /// <summary>
        /// Загрузить модель из файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>ObjVolume загружаемой моджели</returns>
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
            List<Vector2> TextureCoords = new List<Vector2>();
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
                                float x = float.Parse(lineparts[1]);
                                float y = float.Parse(lineparts[2]);
                                float z = float.Parse(lineparts[3]);
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
                                TextureCoords.Add(new Vector2(u, v));
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
            vol.texturecoords = new Vector2[FacesV.Count];
            vol.normals = new Vector3[FacesV.Count];
            vol.indexes = new int[FacesV.Count];

            for (int i = 0; i < FacesV.Count; i++)
            {
                vol.vertices[i] = Vertices[FacesV[i]];
                vol.texturecoords[i] = TextureCoords[FacesT[i]];
                vol.normals[i] = Normals[FacesN[i]];
                vol.indexes[i] = i;
            }

            vol.tangentses = CalcTangentses(vol.vertices, vol.normals, vol.texturecoords, vol.indexes);

            Vertices = null;
            TextureCoords = null;
            Normals = null;
            FacesV = null;
            FacesT = null;
            FacesN = null;
            return vol;
        }
    }
}
