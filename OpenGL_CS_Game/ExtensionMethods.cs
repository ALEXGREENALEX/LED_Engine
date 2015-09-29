/*using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

namespace OpenTK
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static Vector3 Mult(this Matrix3 Matrix, Vector3 Vector)
        {
            Vector3 result;
            Mult(ref Matrix, ref Vector, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref Matrix3 Matrix, ref Vector3 Vector, out Vector3 result)
        {
            float lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z,
            lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z,
            lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z,
            rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z,
            rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z,
            rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z;

            result.Row0.X = ((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31);
            result.Row0.Y = ((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32);
            result.Row0.Z = ((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33);
            result.Row1.X = ((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31);
            result.Row1.Y = ((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32);
            result.Row1.Z = ((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33);
            result.Row2.X = ((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31);
            result.Row2.Y = ((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32);
            result.Row2.Z = ((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix3d which holds the result of the multiplication</returns>
        public static Vector3 operator *(Matrix3 left, Vector3 right)
        {
            return Matrix3.Mult(left, right);
        }
    }
}*/