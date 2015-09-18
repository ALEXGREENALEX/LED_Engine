using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace OpenGL_CS_Game
{
    /// <summary>
    /// Структура для хранения 3-ох int, замена "Tuple<int, int, int>"
    /// </summary>
    public struct TupleInt3
    {
        public int Item1, Item2, Item3;
        public TupleInt3(int item1, int item2, int item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }

    class ObjVolume : Volume
    {
        Vector3[] vertices;
        Vector3[] normals;
        Vector2[] texturecoords;
        Vector4[] tangentses;

        List<TupleInt3> faces = new List<TupleInt3>();

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
                temp.Add((uint)(face.Item1 + offset));
                temp.Add((uint)(face.Item2 + offset));
                temp.Add((uint)(face.Item3 + offset));
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
            List<Vector4> TangensesList = new List<Vector4>();
            List<Vector3> tan1Accum = new List<Vector3>();
            List<Vector3> tan2Accum = new List<Vector3>();

            for (uint i = 0; i < points.Length; i++)
            {
                tan1Accum.Add(new Vector3(0.0f));
                tan2Accum.Add(new Vector3(0.0f));
                TangensesList.Add(new Vector4(0.0f));
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
            tangentses = TangensesList.ToArray();
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
            List<Vector3> Verts = new List<Vector3>();
            List<Vector3> Normals = new List<Vector3>();
            List<Vector2> TextureCoords = new List<Vector2>();
            List<TupleInt3> Faces = new List<TupleInt3>();

            String FloatComa = (0.5f).ToString().Substring(1, 1);

            // Построчное считывание
            foreach (String line in lines)
            {
                if (line.StartsWith("v ")) // Определение вершин
                {
                    // Подготавливаем строку (удаляем начало, пробелы в начале, конце...)
                    String temp = line.Substring(2).Replace(".", FloatComa).Replace("  ", " ").Trim();

                    Vector3 vec = new Vector3();

                    if (temp.Length - temp.Replace(" ", "").Length == 2) // Проверка, достаточно ли элементов для вершины
                    {
                        String[] vertparts = temp.Split(' ');

                        // Попытка разобрать каждую часть вертекса
                        bool success = float.TryParse(vertparts[0], out vec.X);
                        success |= float.TryParse(vertparts[1], out vec.Y);
                        success |= float.TryParse(vertparts[2], out vec.Z);

                        if (!success)
                            MessageBox.Show("Error parsing vertex: " + line.ToString());
                        else
                            Verts.Add(vec);
                    }
                }
                else if (line.StartsWith("f ")) // Определение граней
                {
                    // Подготавливаем строку (удаляем начало, пробелы в начале, конце...)
                    String temp = line.Substring(2).Replace(".", FloatComa).Replace("  ", " ").Trim();

                    if (temp.Length - temp.Replace(" ", "").Length == 2) // Проверка, достаточно ли элементов для грани
                    {
                        String[] faceparts = temp.Split(' ');

                        int i1, i2, i3;

                        // Попытка разобрать каждую часть грани
                        bool success = int.TryParse(faceparts[0].Substring(0, faceparts[0].IndexOf('/')), out i1);
                        success |= int.TryParse(faceparts[1].Substring(0, faceparts[1].IndexOf('/')), out i2);
                        success |= int.TryParse(faceparts[2].Substring(0, faceparts[2].IndexOf('/')), out i3);

                        if (!success)
                            MessageBox.Show("Error parsing face: " + line.ToString());
                        else
                            // Decrement to get zero-based vertex numbers
                            Faces.Add(new TupleInt3(--i1, --i2, --i3));
                    }
                }
                else if (line.StartsWith("vn ")) // Определение нормалей
                {
                    // Подготавливаем строку (удаляем начало, пробелы в начале, конце...)
                    String temp = line.Substring(3).Replace(".", FloatComa).Replace("  ", " ").Trim();

                    if (temp.Length - temp.Replace(" ", "").Length == 2) // Проверка, достаточно ли элементов
                    {
                        String[] NormalParts = temp.Split(' ');

                        float n1, n2, n3;

                        // Попытка разобрать каждую часть
                        bool success = float.TryParse(NormalParts[0], out n1);
                        success |= float.TryParse(NormalParts[1], out n2);
                        success |= float.TryParse(NormalParts[2], out n3);

                        if (!success)
                            MessageBox.Show("Error parsing normal: " + line.ToString());
                        else
                            Normals.Add(new Vector3(n1, n2, n3));
                    }
                }
                else if (line.StartsWith("vt ")) // Определение текстурных координат
                {
                    // Подготавливаем строку (удаляем начало, пробелы в начале, конце...)
                    String temp = line.Substring(3).Replace(".", FloatComa).Replace("  ", " ").Trim();

                    if (temp.Length - temp.Replace(" ", "").Length == 1) // Проверка, достаточно ли элементов
                    {
                        String[] TextureCoordsParts = temp.Split(' ');

                        float t1, t2;

                        // Попытка разобрать каждую часть
                        bool success = float.TryParse(TextureCoordsParts[0], out t1);
                        success |= float.TryParse(TextureCoordsParts[1], out t2);
                        
                        if (!success)
                            MessageBox.Show("Error parsing TextureCoords: " + line.ToString());
                        else
                            TextureCoords.Add(new Vector2(t1, t2));
                    }
                }
            }

            // Создаем ObjVolume
            ObjVolume vol = new ObjVolume();
            vol.vertices = Verts.ToArray();
            vol.normals = Normals.ToArray();
            vol.faces = new List<TupleInt3>(Faces);
            vol.texturecoords = TextureCoords.ToArray();
            vol.CalcTangentses();
            return vol;
        }
    }
}
