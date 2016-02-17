using System;
using System.Collections.Generic;
using System.Text;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class FrustumCulling
    {
        public static float[,] Frustum = new float[6, 4];

        public static void ExtractFrustum(Matrix4 MVP)
        {
            // Left Frustum Plain
            Frustum[0, 0] = MVP.M14 - MVP.M11;
            Frustum[0, 1] = MVP.M24 - MVP.M21;
            Frustum[0, 2] = MVP.M34 - MVP.M31;
            Frustum[0, 3] = MVP.M44 - MVP.M41;
            NormalizeFrustumPlane(0);

            // Right Frustum Plain
            Frustum[1, 0] = MVP.M14 + MVP.M11;
            Frustum[1, 1] = MVP.M24 + MVP.M21;
            Frustum[1, 2] = MVP.M34 + MVP.M31;
            Frustum[1, 3] = MVP.M44 + MVP.M41;
            NormalizeFrustumPlane(1);

            // Bottom Frustum Plain
            Frustum[2, 0] = MVP.M14 + MVP.M12;
            Frustum[2, 1] = MVP.M24 + MVP.M22;
            Frustum[2, 2] = MVP.M34 + MVP.M32;
            Frustum[2, 3] = MVP.M44 + MVP.M42;
            NormalizeFrustumPlane(2);

            // Top Frustum Plain
            Frustum[3, 0] = MVP.M14 - MVP.M12;
            Frustum[3, 1] = MVP.M24 - MVP.M22;
            Frustum[3, 2] = MVP.M34 - MVP.M32;
            Frustum[3, 3] = MVP.M44 - MVP.M42;
            NormalizeFrustumPlane(3);

            // Near Frustum Plain
            Frustum[4, 0] = MVP.M14 - MVP.M13;
            Frustum[4, 1] = MVP.M24 - MVP.M23;
            Frustum[4, 2] = MVP.M34 - MVP.M33;
            Frustum[4, 3] = MVP.M44 - MVP.M43;
            NormalizeFrustumPlane(4);

            // Far Frustum Plain
            Frustum[5, 0] = MVP.M14 + MVP.M13;
            Frustum[5, 1] = MVP.M24 + MVP.M23;
            Frustum[5, 2] = MVP.M34 + MVP.M33;
            Frustum[5, 3] = MVP.M44 + MVP.M43;
            NormalizeFrustumPlane(5);
        }

        static void NormalizeFrustumPlane(int Side)
        {
            float Len = (float)Math.Sqrt(Frustum[Side, 0] * Frustum[Side, 0] + Frustum[Side, 1] * Frustum[Side, 1] + Frustum[Side, 2] * Frustum[Side, 2]);
            Frustum[Side, 0] /= Len;
            Frustum[Side, 1] /= Len;
            Frustum[Side, 2] /= Len;
            Frustum[Side, 3] /= Len;
        }

        #region Point
        static public bool PointInFrustum(Vector3 Position)
        {
            for (int i = 0; i < 6; i++)
                if (Frustum[i, 0] * Position.X + Frustum[i, 1] * Position.Y + Frustum[i, 2] * Position.Z + Frustum[i, 3] <= 0)
                    return false;
            return true;
        }

        static bool PointInFrustum(float X, float Y, float Z)
        {
            for (int i = 0; i < 6; i++)
                if (Frustum[i, 0] * X + Frustum[i, 1] * Y + Frustum[i, 2] * Z + Frustum[i, 3] <= 0)
                    return false;
            return true;
        }
        #endregion

        #region Sphere
        public static bool SphereInFrustum(Vector3 Position, float Radius)
        {
            for (int i = 0; i < 6; ++i)
                if (Frustum[i, 0] * Position.X + Frustum[i, 1] * Position.Y + Frustum[i, 2] * Position.Z + Frustum[i, 3] <= -Radius)
                    return false;
            return true;
        }

        public static bool SphereInFrustum(float X, float Y, float Z, float Radius)
        {
            for (int i = 0; i < 6; ++i)
                if (Frustum[i, 0] * X + Frustum[i, 1] * Y + Frustum[i, 2] * Z + Frustum[i, 3] <= -Radius)
                    return false;
            return true;
        }

        public static int SphereInFrustumIntersect(Vector3 Position, float Radius)
        {
            int c = 0;
            float d;
            for (int i = 0; i < 6; i++)
            {
                d = Frustum[i, 0] * Position.X + Frustum[i, 1] * Position.Y + Frustum[i, 2] * Position.Z + Frustum[i, 3];
                if (d <= -Radius)
                    return 0;
                if (d > Radius)
                    c++;
            }
            return (c == 6) ? 2 : 1;
        }

        public static int SphereInFrustumIntersect(float X, float Y, float Z, float Radius)
        {
            int c = 0;
            float d;
            for (int i = 0; i < 6; i++)
            {
                d = Frustum[i, 0] * X + Frustum[i, 1] * Y + Frustum[i, 2] * Z + Frustum[i, 3];
                if (d <= -Radius)
                    return 0;
                if (d > Radius)
                    c++;
            }
            return (c == 6) ? 2 : 1;
        }
        #endregion

        #region Box
        public static bool BoxInFrustum(Vector3 Position, float Size)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public static bool BoxInFrustum(float X, float Y, float Z, float Size)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    continue;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public static int BoxInFrustumIntersect(Vector3 Position, float Size)
        {
            int c;
            int c2 = 0;
            for (int i = 0; i < 6; i++)
            {
                c = 0;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y - Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X - Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (Position.X + Size) + Frustum[i, 1] * (Position.Y + Size) + Frustum[i, 2] * (Position.Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (c == 0)
                    return 0;
                if (c == 8)
                    c2++;
            }
            return (c2 == 6) ? 2 : 1;
        }

        public static int BoxInFrustumIntersect(float X, float Y, float Z, float Size)
        {
            int c;
            int c2 = 0;
            for (int i = 0; i < 6; i++)
            {
                c = 0;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z - Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y - Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X - Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (Frustum[i, 0] * (X + Size) + Frustum[i, 1] * (Y + Size) + Frustum[i, 2] * (Z + Size) + Frustum[i, 3] > 0)
                    c++;
                if (c == 0)
                    return 0;
                if (c == 8)
                    c2++;
            }
            return (c2 == 6) ? 2 : 1;
        }
        #endregion
    }
}
