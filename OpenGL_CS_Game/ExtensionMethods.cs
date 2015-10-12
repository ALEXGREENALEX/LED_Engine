using System;
using System.Collections.Generic;
using System.Xml;
using OpenTK;

namespace OpenTK
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Умножаем Вектор(слева) на Матрицу(справа).
        /// </summary>
        /// <param name="Vector">Vector3 (Слева)</param>
        /// <param name="Matrix">Matrix3 (Справа)</param>
        /// <returns>Vector3</returns>
        public static Vector3 Mult(this Vector3 Vector, Matrix3 Matrix)
        {
            Vector3 result = new Vector3();
            result.X = Vector.X * Matrix.M11 + Vector.Y * Matrix.M21 + Vector.Z * Matrix.M31;
            result.Y = Vector.X * Matrix.M12 + Vector.Y * Matrix.M22 + Vector.Z * Matrix.M32;
            result.Z = Vector.X * Matrix.M13 + Vector.Y * Matrix.M23 + Vector.Z * Matrix.M33;
            return result;
        }

        /// <summary>
        /// Умножаем Матрицу(слева) на Вектор(справа).
        /// </summary>
        /// <param name="Matrix">Matrix3 (Слева)</param>
        /// <param name="Vector">Vector3 (Справа)</param>
        /// <returns>Vector3</returns>
        public static Vector3 Mult(this Matrix3 Matrix, Vector3 Vector)
        {
            Vector3 result = new Vector3();
            result.X = Matrix.M11 * Vector.X + Matrix.M12 * Vector.Y + Matrix.M13 * Vector.Z;
            result.Y = Matrix.M21 * Vector.X + Matrix.M22 * Vector.Y + Matrix.M23 * Vector.Z;
            result.Z = Matrix.M31 * Vector.X + Matrix.M32 * Vector.Y + Matrix.M33 * Vector.Z;
            return result;
        }

        /// <summary>
        /// Умножаем Вектор(слева) на Матрицу(справа).
        /// </summary>
        /// <param name="Vector">Vector4 (Слева)</param>
        /// <param name="Matrix">Matrix4 (Справа)</param>
        /// <returns>Vector4</returns>
        public static Vector4 Mult(this Vector4 Vector, Matrix4 Matrix)
        {
            Vector4 result = new Vector4();
            result.X = Vector.X * Matrix.M11 + Vector.Y * Matrix.M21 + Vector.Z * Matrix.M31 + Vector.W * Matrix.M41;
            result.Y = Vector.X * Matrix.M12 + Vector.Y * Matrix.M22 + Vector.Z * Matrix.M32 + Vector.W * Matrix.M42;
            result.Z = Vector.X * Matrix.M13 + Vector.Y * Matrix.M23 + Vector.Z * Matrix.M33 + Vector.W * Matrix.M43;
            result.W = Vector.X * Matrix.M14 + Vector.Y * Matrix.M24 + Vector.Z * Matrix.M34 + Vector.W * Matrix.M44;
            return result;
        }

        /// <summary>
        /// Умножаем Матрицу(слева) на Вектор(справа).
        /// </summary>
        /// <param name="Matrix">Matrix4 (Слева)</param>
        /// <param name="Vector">Vector4 (Справа)</param>
        /// <returns>Vector4</returns>
        public static Vector4 Mult(this Matrix4 Matrix, Vector4 Vector)
        {
            Vector4 result = new Vector4();
            result.X = Matrix.M11 * Vector.X + Matrix.M12 * Vector.Y + Matrix.M13 * Vector.Z + Matrix.M14 * Vector.W;
            result.Y = Matrix.M21 * Vector.X + Matrix.M22 * Vector.Y + Matrix.M23 * Vector.Z + Matrix.M24 * Vector.W;
            result.Z = Matrix.M31 * Vector.X + Matrix.M32 * Vector.Y + Matrix.M33 * Vector.Z + Matrix.M34 * Vector.W;
            result.W = Matrix.M41 * Vector.X + Matrix.M42 * Vector.Y + Matrix.M43 * Vector.Z + Matrix.M44 * Vector.W;
            return result;
        }

        //public static XmlNodeList Append(this XmlNodeList ListBase, XmlNodeList List)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(@"<MyXml></MyXml>");

        //    XmlDocumentFragment xfrag = doc.CreateDocumentFragment();

        //    for (int i = 0; i < ListBase.Count; i++)
        //    {
        //        xfrag.InnerXml = ListBase.Item(i).ParentNode.InnerXml;
        //        doc.DocumentElement.AppendChild(xfrag);
        //    }

        //    for (int i = 0; i < List.Count; i++)
        //    {
        //        xfrag.InnerXml = List.Item(i).ParentNode.InnerXml;
        //        doc.DocumentElement.AppendChild(xfrag);
        //    }

        //    return doc.DocumentElement.ChildNodes;
        //}
    }
}