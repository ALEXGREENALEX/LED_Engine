using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            result.X = Vector.X * Matrix.M11 + Vector.Y * Matrix.M12 + Vector.Z * Matrix.M13;
            result.Y = Vector.X * Matrix.M21 + Vector.Y * Matrix.M22 + Vector.Z * Matrix.M23;
            result.Z = Vector.X * Matrix.M31 + Vector.Y * Matrix.M32 + Vector.Z * Matrix.M33;
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
            result.X = Matrix.M11 * Vector.X + Matrix.M21 * Vector.Y + Matrix.M31 * Vector.Z;
            result.Y = Matrix.M12 * Vector.X + Matrix.M22 * Vector.Y + Matrix.M32 * Vector.Z;
            result.Z = Matrix.M13 * Vector.X + Matrix.M23 * Vector.Y + Matrix.M33 * Vector.Z;
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
            result.X = Vector.X * Matrix.M11 + Vector.Y * Matrix.M12 + Vector.Z * Matrix.M13 + Vector.W * Matrix.M14;
            result.Y = Vector.X * Matrix.M21 + Vector.Y * Matrix.M22 + Vector.Z * Matrix.M23 + Vector.W * Matrix.M24;
            result.Z = Vector.X * Matrix.M31 + Vector.Y * Matrix.M32 + Vector.Z * Matrix.M33 + Vector.W * Matrix.M34;
            result.W = Vector.X * Matrix.M41 + Vector.Y * Matrix.M42 + Vector.Z * Matrix.M43 + Vector.W * Matrix.M44;
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
            result.X = Matrix.M11 * Vector.X + Matrix.M21 * Vector.Y + Matrix.M31 * Vector.Z + Matrix.M41 * Vector.W;
            result.Y = Matrix.M12 * Vector.X + Matrix.M22 * Vector.Y + Matrix.M32 * Vector.Z + Matrix.M42 * Vector.W;
            result.Z = Matrix.M13 * Vector.X + Matrix.M23 * Vector.Y + Matrix.M33 * Vector.Z + Matrix.M43 * Vector.W;
            result.W = Matrix.M14 * Vector.X + Matrix.M24 * Vector.Y + Matrix.M34 * Vector.Z + Matrix.M44 * Vector.W;
            return result;
        }
    }
}