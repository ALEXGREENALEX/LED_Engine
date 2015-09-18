using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace OpenGL_CS_Game
{
    /// <summary>
    /// Структура для хранения 3-ох uint, замена "Tuple<uint, uint, uint>"
    /// </summary>
    struct TupleUInt3
    {
        public uint Item1, Item2, Item3;
        public TupleUInt3(uint Item1, uint Item2, uint Item3)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
            this.Item3 = Item3;
        }
    }

    class ObjVolume : Volume
    {
        Vector3[] vertices;
        Vector3[] normals;
        Vector2[] texturecoords;
        Vector4[] tangentses;

        List<TupleUInt3> faces = new List<TupleUInt3>();

        public override int VerticesCount { get { return vertices.Length; } }
        public override int NormalsCount { get { return normals.Length; } }
        public override int FacesCount { get { return faces.Count * 3; } }
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
        /// <param name="offset">Номер первой вершины в объекте</param>
        /// <returns>Array of indices offset to match buffered data</returns>
        public override uint[] GetFaces(uint offset = 0)
        {
            List<uint> temp = new List<uint>();

            foreach (var face in faces)
            {
                temp.Add(face.Item1 + offset);
                temp.Add(face.Item2 + offset);
                temp.Add(face.Item3 + offset);
            }
            return temp.ToArray();
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
            uint[] faces = GetFaces();
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
            List<TupleUInt3> FacesV = new List<TupleUInt3>();
            List<TupleUInt3> FacesT = new List<TupleUInt3>();
            List<TupleUInt3> FacesN = new List<TupleUInt3>();

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
                            switch (lineparts.Length)
                            {
                                case 4: //Triangle
                                    TupleUInt3[] FaceVTN = new TupleUInt3[3]; // VertexIndex/TextureCoordIndex/NormalIndex

                                    for (int j = 1; j < lineparts.Length; j++)
                                    {
                                        String[] FaceParams = lineparts[j].Split('/');
                                        if (FaceParams.Length == 3)
                                        {
                                            foreach (string fp in FaceParams)
                                                if (fp.Trim() == String.Empty)
                                                    throw new Exception();
                                        }
                                        else
                                            throw new Exception();

                                        FaceVTN[j - 1].Item1 = uint.Parse(FaceParams[0]) - 1;
                                        FaceVTN[j - 1].Item2 = uint.Parse(FaceParams[1]) - 1;
                                        FaceVTN[j - 1].Item3 = uint.Parse(FaceParams[2]) - 1;
                                    }

                                    FacesV.Add(new TupleUInt3(FaceVTN[0].Item1, FaceVTN[1].Item1, FaceVTN[2].Item1));
                                    FacesT.Add(new TupleUInt3(FaceVTN[0].Item2, FaceVTN[1].Item2, FaceVTN[2].Item2));
                                    FacesN.Add(new TupleUInt3(FaceVTN[0].Item3, FaceVTN[1].Item3, FaceVTN[2].Item3));
                                    break;

                                //case 5: //Quad
                                //    ObjMesh.ObjQuad objQuad = new ObjMesh.ObjQuad();
                                //    objQuad.Index0 = ParseFaceParameter(lineparts[1]);
                                //    objQuad.Index1 = ParseFaceParameter(lineparts[2]);
                                //    objQuad.Index2 = ParseFaceParameter(lineparts[3]);
                                //    objQuad.Index3 = ParseFaceParameter(lineparts[4]);
                                //    objQuads.Add(objQuad);
                                //    break;

                                default:
                                    throw new Exception();
                            }
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

            for (int i = 0; i < FacesV.Count; i++)
            {
                VerticesResult.Add(Vertices[(int)FacesV[i].Item1]);
                VerticesResult.Add(Vertices[(int)FacesV[i].Item2]);
                VerticesResult.Add(Vertices[(int)FacesV[i].Item3]);

                TextureCoordsResult.Add(TextureCoords[(int)FacesT[i].Item1]);
                TextureCoordsResult.Add(TextureCoords[(int)FacesT[i].Item2]);
                TextureCoordsResult.Add(TextureCoords[(int)FacesT[i].Item3]);

                NormalsResult.Add(Normals[(int)FacesN[i].Item1]);
                NormalsResult.Add(Normals[(int)FacesN[i].Item2]);
                NormalsResult.Add(Normals[(int)FacesN[i].Item3]);

                FacesV[i] = new TupleUInt3((uint)i * 3, (uint)i * 3 + 1, (uint)i * 3 + 2);
            }

            vol.vertices = VerticesResult.ToArray();
            vol.texturecoords = TextureCoordsResult.ToArray();
            vol.normals = NormalsResult.ToArray();
            vol.faces = FacesV;
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
