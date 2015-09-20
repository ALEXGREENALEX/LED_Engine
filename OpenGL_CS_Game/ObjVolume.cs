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
        uint[] faces;

        string shaderName = String.Empty;
        string[] textures = { String.Empty, String.Empty, String.Empty, String.Empty };

        public override int VerticesCount { get { return vertices.Length; } }
        public override int NormalsCount { get { return normals.Length; } }
        public override int FacesCount { get { return faces.Length; } }
        public override int TextureCoordsCount { get { return texturecoords.Length; } }
        public override int TangentsesCount { get { return tangentses.Length; } }

        public override string ShaderName
        {
            get { return shaderName; }
            set { shaderName = value; }
        }

        public override string[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }

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
        public override uint[] GetFaceIndeces(uint offset = 0)
        {
            uint[] inds = new uint[faces.Length];

            for (int i = 0; i < faces.Length; i++)
                inds[i] = faces[i] + offset;

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

        private void CalcTangentses()
        {
            Vector3[] points = GetVertices();
            Vector3[] normals = GetNormals();
            uint[] faces = GetFaceIndeces();
            Vector2[] texCoords = GetTextureCoords();
            Vector4[] TangensesList = new Vector4[points.Length];
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
                TangensesList[i] = new Vector4(Vector3.Normalize(t1 - (Vector3.Dot(n, t1) * n)), 0.0f);
                // Store handedness in W
                Vector4 V_temp = TangensesList[i];
                V_temp.W = (Vector3.Dot(Vector3.Cross(n, t1), t2) < 0.0f) ? -1.0f : 1.0f;
                TangensesList[i] = V_temp;
            }

            tan1Accum.Clear();
            tan2Accum.Clear();
            tangentses = TangensesList;
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
            List<Vector3> VerticesResult = new List<Vector3>();
            List<Vector3> NormalsResult = new List<Vector3>();
            List<Vector2> TextureCoordsResult = new List<Vector2>();
            List<uint> FacesV = new List<uint>();
            List<uint> FacesT = new List<uint>();
            List<uint> FacesN = new List<uint>();

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
                            uint[,] FaceVTN = new uint[4, 3]; // VertexIndex/TextureCoordIndex/NormalIndex

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
                                        FaceVTN[j, k] = uint.Parse(FaceParams[k]) - 1;
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

            for (int i = 0; i < FacesV.Count; i += 3)
            {
                VerticesResult.Add(Vertices[(int)FacesV[i]]);
                VerticesResult.Add(Vertices[(int)FacesV[i + 1]]);
                VerticesResult.Add(Vertices[(int)FacesV[i + 2]]);

                TextureCoordsResult.Add(TextureCoords[(int)FacesT[i]]);
                TextureCoordsResult.Add(TextureCoords[(int)FacesT[i + 1]]);
                TextureCoordsResult.Add(TextureCoords[(int)FacesT[i + 2]]);

                NormalsResult.Add(Normals[(int)FacesN[i]]);
                NormalsResult.Add(Normals[(int)FacesN[i + 1]]);
                NormalsResult.Add(Normals[(int)FacesN[i + 2]]);

                FacesV[i] = (uint)i;
                FacesV[i + 1] = (uint)i + 1;
                FacesV[i + 2] = (uint)i + 2;
            }

            vol.vertices = VerticesResult.ToArray();
            vol.texturecoords = TextureCoordsResult.ToArray();
            vol.normals = NormalsResult.ToArray();
            vol.faces = FacesV.ToArray();
            vol.CalcTangentses();

            Vertices = null;
            TextureCoords = null;
            Normals = null;
            VerticesResult = null;
            TextureCoordsResult = null;
            NormalsResult = null;
            FacesV = null;
            FacesT = null;
            FacesN = null;
            return vol;
        }
    }
}
