using System;
using System.Collections.Generic;
using System.Text;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    // Based on, but don't work:
    // http://pmg.org.ru/nehe/nehex2.htm
    // http://masandilov.ru/opengl/opengl-frustum
    // http://www.crownandcutlass.com/features/technicaldetails/frustum.html

    public static class FrustumCulling
    {
        public static float[,] Frustum = new float[6, 4];

        public static void ExtractFrustum(Matrix4 MVP)
        {
            // Left Frustum Plain
            Frustum[0, 0] = MVP.M41 + MVP.M11;
            Frustum[0, 1] = MVP.M42 + MVP.M12;
            Frustum[0, 2] = MVP.M43 + MVP.M13;
            Frustum[0, 3] = MVP.M44 + MVP.M14;
            NormalizeFrustumPlane(0);

            // Right Frustum Plain
            Frustum[1, 0] = MVP.M41 - MVP.M11;
            Frustum[1, 1] = MVP.M42 - MVP.M12;
            Frustum[1, 2] = MVP.M43 - MVP.M13;
            Frustum[1, 3] = MVP.M44 - MVP.M14;
            NormalizeFrustumPlane(1);

            // Bottom Frustum Plain
            Frustum[2, 0] = MVP.M41 + MVP.M21;
            Frustum[2, 1] = MVP.M42 + MVP.M22;
            Frustum[2, 2] = MVP.M43 + MVP.M23;
            Frustum[2, 3] = MVP.M44 + MVP.M24;
            NormalizeFrustumPlane(2);

            // Top Frustum Plain
            Frustum[3, 0] = MVP.M41 - MVP.M21;
            Frustum[3, 1] = MVP.M42 - MVP.M22;
            Frustum[3, 2] = MVP.M43 - MVP.M23;
            Frustum[3, 3] = MVP.M44 - MVP.M24;
            NormalizeFrustumPlane(3);

            // Near Frustum Plain
            Frustum[4, 0] = MVP.M41 + MVP.M31;
            Frustum[4, 1] = MVP.M42 + MVP.M32;
            Frustum[4, 2] = MVP.M43 + MVP.M33;
            Frustum[4, 3] = MVP.M44 + MVP.M34;
            NormalizeFrustumPlane(4);

            // Far Frustum Plain
            Frustum[5, 0] = MVP.M41 - MVP.M31;
            Frustum[5, 1] = MVP.M42 - MVP.M32;
            Frustum[5, 2] = MVP.M43 - MVP.M33;
            Frustum[5, 3] = MVP.M44 - MVP.M34;
            NormalizeFrustumPlane(5);
        }

        public static void ExtractFrustum_(Matrix4 MVP)
        {
            // Left Frustum Plain
            Frustum[0, 0] = MVP.M14 + MVP.M11;
            Frustum[0, 1] = MVP.M24 + MVP.M21;
            Frustum[0, 2] = MVP.M34 + MVP.M31;
            Frustum[0, 3] = MVP.M44 + MVP.M41;
            NormalizeFrustumPlane(0);

            // Right Frustum Plain
            Frustum[1, 0] = MVP.M14 - MVP.M11;
            Frustum[1, 1] = MVP.M24 - MVP.M21;
            Frustum[1, 2] = MVP.M34 - MVP.M31;
            Frustum[1, 3] = MVP.M44 - MVP.M41;
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

            // Front Frustum Plain
            Frustum[4, 0] = MVP.M14 + MVP.M13;
            Frustum[4, 1] = MVP.M24 + MVP.M23;
            Frustum[4, 2] = MVP.M34 + MVP.M33;
            Frustum[4, 3] = MVP.M44 + MVP.M43;
            NormalizeFrustumPlane(4);

            // Back Frustum Plain
            Frustum[5, 0] = MVP.M14 - MVP.M13;
            Frustum[5, 1] = MVP.M24 - MVP.M23;
            Frustum[5, 2] = MVP.M34 - MVP.M33;
            Frustum[5, 3] = MVP.M44 - MVP.M43;
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

        static public bool PointInFrustum(Vector3 Position)
        {
            return PointInFrustum(Position.X, Position.Y, Position.Z);
        }

        static bool PointInFrustum(float X, float Y, float Z)
        {
            for (int i = 0; i < 6; i++)
                if (Frustum[i, 0] * X + Frustum[i, 1] * Y + Frustum[i, 2] * Z + Frustum[i, 3] <= 0)
                    return false;
            return true;
        }

        public static bool SphereInFrustum(Vector3 Position, float Radius)
        {
            return SphereInFrustum(Position.X, Position.Y, Position.Z, Radius);
        }

        public static bool SphereInFrustum(float X, float Y, float Z, float Radius)
        {
            for (int i = 0; i < 6; i++)
                if (Frustum[i, 0] * X + Frustum[i, 1] * Y + Frustum[i, 2] * Z + Frustum[i, 3] < -Radius)
                    return false;
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


        // With Intersection (0 - out of Frustrum, 1 - Intersect, 2 - Fully inside Frustrum)
        public static int SphereInFrustumIntersect(Vector3 Position, float Radius)
        {
            return SphereInFrustumIntersect(Position.X, Position.Z, Position.Z, Radius);
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

        public static int BoxInFrustumIntersect(Vector3 Position, float Size)
        {
            return BoxInFrustumIntersect(Position.X, Position.Y, Position.Z, Size);
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

        // false if fully outside, true if inside or intersects
        //public static bool boxInFrustum(BoundingBox box)
        //{
        //    int Out;
        //    // check box outside/inside of frustum
        //    for(int i = 0; i < 6; i++)
        //    {
        //        Out = 0;
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.NegativeX, box.NegativeY, box.NegativeZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.PositiveX, box.NegativeY, box.NegativeZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.NegativeX, box.PositiveY, box.NegativeZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.PositiveX, box.PositiveY, box.NegativeZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.NegativeX, box.NegativeY, box.PositiveZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.PositiveX, box.NegativeY, box.PositiveZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.NegativeX, box.PositiveY, box.PositiveZ, 1.0f)) < 0.0) ? 1 : 0);
        //        Out += ((Vector4.Dot(new Vector4(Frustum[i, 0], Frustum[i, 1], Frustum[i, 2], Frustum[i, 3]), new Vector4(box.PositiveX, box.PositiveY, box.PositiveZ, 1.0f)) < 0.0) ? 1 : 0);

        //        if (Out == 8)  
        //            return false;
        //    }

        //    // check frustum outside/inside box
        //    Out = 0;
        //    for( int i = 0; i < 8; i++)
        //        Out += ((fru.mPoints[i].x > box.PositiveX) ? 1 : 0);
        //    if (Out == 8)
        //        return false;

        //    Out = 0;
        //    for( int i = 0; i < 8; i++)
        //        Out += ((fru.mPoints[i].x < box.NegativeX) ? 1 : 0);
        //    if (Out == 8)
        //        return false;

        //    Out = 0;
        //    for (int i = 0; i < 8; i++)
        //        Out += ((fru.mPoints[i].y > box.PositiveY) ? 1 : 0);
        //    if (Out == 8)
        //        return false;

        //    Out = 0; for (int i = 0; i < 8; i++)
        //        Out += ((fru.mPoints[i].y < box.NegativeY) ? 1 : 0);
        //    if (Out == 8)
        //        return false;

        //    Out = 0;
        //    for (int i = 0; i < 8; i++)
        //        Out += ((fru.mPoints[i].z > box.PositiveZ) ? 1 : 0);
        //    if (Out == 8)
        //        return false;

        //    Out = 0;
        //    for (int i = 0; i < 8; i++)
        //        Out += ((fru.mPoints[i].z < box.NegativeZ) ? 1 : 0);
        //    if (Out == 8)
        //        return false;

        //    return true;
        //}
    }

    public class Frustum
    {
        private readonly float[] _clipMatrix = new float[16];
        private readonly float[,] _frustum = new float[6, 4];

        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;

        public enum ClippingPlane : int
        {
            Right = 0,
            Left = 1,
            Bottom = 2,
            Top = 3,
            Back = 4,
            Front = 5
        }

        private void NormalizePlane(float[,] frustum, int side)
        {
            float magnitude = (float)Math.Sqrt((frustum[side, 0] * frustum[side, 0]) + (frustum[side, 1] * frustum[side, 1])
                                                + (frustum[side, 2] * frustum[side, 2]));
            frustum[side, 0] /= magnitude;
            frustum[side, 1] /= magnitude;
            frustum[side, 2] /= magnitude;
            frustum[side, 3] /= magnitude;
        }

        public bool PointVsFrustum(float x, float y, float z)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this._frustum[i, 0] * x + this._frustum[i, 1] * y + this._frustum[i, 2] * z + this._frustum[i, 3] <= 0.0f)
                {
                    return false;
                }
            }
            return true;
        }

        public bool PointVsFrustum(Vector3 location)
        {
            for (int i = 0; i < 6; i++)
            {
                if (this._frustum[i, 0] * location.X + this._frustum[i, 1] * location.Y + this._frustum[i, 2] * location.Z + this._frustum[i, 3] <= 0.0f)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SphereVsFrustum(float x, float y, float z, float radius)
        {
            for (int p = 0; p < 6; p++)
            {
                float d = _frustum[p, 0] * x + _frustum[p, 1] * y + _frustum[p, 2] * z + _frustum[p, 3];
                if (d <= -radius)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SphereVsFrustum(Vector3 location, float radius)
        {
            for (int p = 0; p < 6; p++)
            {
                float d = _frustum[p, 0] * location.X + _frustum[p, 1] * location.Y + _frustum[p, 2] * location.Z + _frustum[p, 3];
                if (d <= -radius)
                {
                    return false;
                }
            }
            return true;
        }

        public bool VolumeVsFrustum(float x, float y, float z, float width, float height, float length)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y - height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + width) + _frustum[i, B] * (y + height) + _frustum[i, C] * (z + length) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public bool VolumeVsFrustum(Vector3 Pos, BoundingBox Box)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (Pos.X - Box.LenX) + _frustum[i, B] * (Pos.Y - Box.LenY) + _frustum[i, C] * (Pos.Z - Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + Box.LenX) + _frustum[i, B] * (Pos.Y - Box.LenY) + _frustum[i, C] * (Pos.Z - Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X - Box.LenX) + _frustum[i, B] * (Pos.Y + Box.LenY) + _frustum[i, C] * (Pos.Z - Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + Box.LenX) + _frustum[i, B] * (Pos.Y + Box.LenY) + _frustum[i, C] * (Pos.Z - Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X - Box.LenX) + _frustum[i, B] * (Pos.Y - Box.LenY) + _frustum[i, C] * (Pos.Z + Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + Box.LenX) + _frustum[i, B] * (Pos.Y - Box.LenY) + _frustum[i, C] * (Pos.Z + Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X - Box.LenX) + _frustum[i, B] * (Pos.Y + Box.LenY) + _frustum[i, C] * (Pos.Z + Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + Box.LenX) + _frustum[i, B] * (Pos.Y + Box.LenY) + _frustum[i, C] * (Pos.Z + Box.LenZ) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public bool VolumeVsFrustum(Vector3 Pos, float width, float height, float length)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (Pos.X - width) + _frustum[i, B] * (Pos.Y - height) + _frustum[i, C] * (Pos.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + width) + _frustum[i, B] * (Pos.Y - height) + _frustum[i, C] * (Pos.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X - width) + _frustum[i, B] * (Pos.Y + height) + _frustum[i, C] * (Pos.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + width) + _frustum[i, B] * (Pos.Y + height) + _frustum[i, C] * (Pos.Z - length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X - width) + _frustum[i, B] * (Pos.Y - height) + _frustum[i, C] * (Pos.Z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + width) + _frustum[i, B] * (Pos.Y - height) + _frustum[i, C] * (Pos.Z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X - width) + _frustum[i, B] * (Pos.Y + height) + _frustum[i, C] * (Pos.Z + length) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (Pos.X + width) + _frustum[i, B] * (Pos.Y + height) + _frustum[i, C] * (Pos.Z + length) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }

        public bool CubeVsFrustum(float x, float y, float z, float size)
        {
            for (int i = 0; i < 6; i++)
            {
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z - size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y - size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x - size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                if (_frustum[i, A] * (x + size) + _frustum[i, B] * (y + size) + _frustum[i, C] * (z + size) + _frustum[i, D] > 0)
                    continue;
                return false;
            }
            return true;
        }


        public void CalculateFrustum(Matrix4 projectionMatrix, Matrix4 modelViewMatrix)
        {
            _clipMatrix[0] = (modelViewMatrix.M11 * projectionMatrix.M11) + (modelViewMatrix.M12 * projectionMatrix.M21) + (modelViewMatrix.M13 * projectionMatrix.M31) + (modelViewMatrix.M14 * projectionMatrix.M41);
            _clipMatrix[1] = (modelViewMatrix.M11 * projectionMatrix.M12) + (modelViewMatrix.M12 * projectionMatrix.M22) + (modelViewMatrix.M13 * projectionMatrix.M32) + (modelViewMatrix.M14 * projectionMatrix.M42);
            _clipMatrix[2] = (modelViewMatrix.M11 * projectionMatrix.M13) + (modelViewMatrix.M12 * projectionMatrix.M23) + (modelViewMatrix.M13 * projectionMatrix.M33) + (modelViewMatrix.M14 * projectionMatrix.M43);
            _clipMatrix[3] = (modelViewMatrix.M11 * projectionMatrix.M14) + (modelViewMatrix.M12 * projectionMatrix.M24) + (modelViewMatrix.M13 * projectionMatrix.M34) + (modelViewMatrix.M14 * projectionMatrix.M44);

            _clipMatrix[4] = (modelViewMatrix.M21 * projectionMatrix.M11) + (modelViewMatrix.M22 * projectionMatrix.M21) + (modelViewMatrix.M23 * projectionMatrix.M31) + (modelViewMatrix.M24 * projectionMatrix.M41);
            _clipMatrix[5] = (modelViewMatrix.M21 * projectionMatrix.M12) + (modelViewMatrix.M22 * projectionMatrix.M22) + (modelViewMatrix.M23 * projectionMatrix.M32) + (modelViewMatrix.M24 * projectionMatrix.M42);
            _clipMatrix[6] = (modelViewMatrix.M21 * projectionMatrix.M13) + (modelViewMatrix.M22 * projectionMatrix.M23) + (modelViewMatrix.M23 * projectionMatrix.M33) + (modelViewMatrix.M24 * projectionMatrix.M43);
            _clipMatrix[7] = (modelViewMatrix.M21 * projectionMatrix.M14) + (modelViewMatrix.M22 * projectionMatrix.M24) + (modelViewMatrix.M23 * projectionMatrix.M34) + (modelViewMatrix.M24 * projectionMatrix.M44);

            _clipMatrix[8] = (modelViewMatrix.M31 * projectionMatrix.M11) + (modelViewMatrix.M32 * projectionMatrix.M21) + (modelViewMatrix.M33 * projectionMatrix.M31) + (modelViewMatrix.M34 * projectionMatrix.M41);
            _clipMatrix[9] = (modelViewMatrix.M31 * projectionMatrix.M12) + (modelViewMatrix.M32 * projectionMatrix.M22) + (modelViewMatrix.M33 * projectionMatrix.M32) + (modelViewMatrix.M34 * projectionMatrix.M42);
            _clipMatrix[10] = (modelViewMatrix.M31 * projectionMatrix.M13) + (modelViewMatrix.M32 * projectionMatrix.M23) + (modelViewMatrix.M33 * projectionMatrix.M33) + (modelViewMatrix.M34 * projectionMatrix.M43);
            _clipMatrix[11] = (modelViewMatrix.M31 * projectionMatrix.M14) + (modelViewMatrix.M32 * projectionMatrix.M24) + (modelViewMatrix.M33 * projectionMatrix.M34) + (modelViewMatrix.M34 * projectionMatrix.M44);

            _clipMatrix[12] = (modelViewMatrix.M41 * projectionMatrix.M11) + (modelViewMatrix.M42 * projectionMatrix.M21) + (modelViewMatrix.M43 * projectionMatrix.M31) + (modelViewMatrix.M44 * projectionMatrix.M41);
            _clipMatrix[13] = (modelViewMatrix.M41 * projectionMatrix.M12) + (modelViewMatrix.M42 * projectionMatrix.M22) + (modelViewMatrix.M43 * projectionMatrix.M32) + (modelViewMatrix.M44 * projectionMatrix.M42);
            _clipMatrix[14] = (modelViewMatrix.M41 * projectionMatrix.M13) + (modelViewMatrix.M42 * projectionMatrix.M23) + (modelViewMatrix.M43 * projectionMatrix.M33) + (modelViewMatrix.M44 * projectionMatrix.M43);
            _clipMatrix[15] = (modelViewMatrix.M41 * projectionMatrix.M14) + (modelViewMatrix.M42 * projectionMatrix.M24) + (modelViewMatrix.M43 * projectionMatrix.M34) + (modelViewMatrix.M44 * projectionMatrix.M44);

            _frustum[(int)ClippingPlane.Right, 0] = _clipMatrix[3] - _clipMatrix[0];
            _frustum[(int)ClippingPlane.Right, 1] = _clipMatrix[7] - _clipMatrix[4];
            _frustum[(int)ClippingPlane.Right, 2] = _clipMatrix[11] - _clipMatrix[8];
            _frustum[(int)ClippingPlane.Right, 3] = _clipMatrix[15] - _clipMatrix[12];
            NormalizePlane(_frustum, (int)ClippingPlane.Right);

            _frustum[(int)ClippingPlane.Left, 0] = _clipMatrix[3] + _clipMatrix[0];
            _frustum[(int)ClippingPlane.Left, 1] = _clipMatrix[7] + _clipMatrix[4];
            _frustum[(int)ClippingPlane.Left, 2] = _clipMatrix[11] + _clipMatrix[8];
            _frustum[(int)ClippingPlane.Left, 3] = _clipMatrix[15] + _clipMatrix[12];
            NormalizePlane(_frustum, (int)ClippingPlane.Left);

            _frustum[(int)ClippingPlane.Bottom, 0] = _clipMatrix[3] + _clipMatrix[1];
            _frustum[(int)ClippingPlane.Bottom, 1] = _clipMatrix[7] + _clipMatrix[5];
            _frustum[(int)ClippingPlane.Bottom, 2] = _clipMatrix[11] + _clipMatrix[9];
            _frustum[(int)ClippingPlane.Bottom, 3] = _clipMatrix[15] + _clipMatrix[13];
            NormalizePlane(_frustum, (int)ClippingPlane.Bottom);

            _frustum[(int)ClippingPlane.Top, 0] = _clipMatrix[3] - _clipMatrix[1];
            _frustum[(int)ClippingPlane.Top, 1] = _clipMatrix[7] - _clipMatrix[5];
            _frustum[(int)ClippingPlane.Top, 2] = _clipMatrix[11] - _clipMatrix[9];
            _frustum[(int)ClippingPlane.Top, 3] = _clipMatrix[15] - _clipMatrix[13];
            NormalizePlane(_frustum, (int)ClippingPlane.Top);

            _frustum[(int)ClippingPlane.Back, 0] = _clipMatrix[3] - _clipMatrix[2];
            _frustum[(int)ClippingPlane.Back, 1] = _clipMatrix[7] - _clipMatrix[6];
            _frustum[(int)ClippingPlane.Back, 2] = _clipMatrix[11] - _clipMatrix[10];
            _frustum[(int)ClippingPlane.Back, 3] = _clipMatrix[15] - _clipMatrix[14];
            NormalizePlane(_frustum, (int)ClippingPlane.Back);

            _frustum[(int)ClippingPlane.Front, 0] = _clipMatrix[3] + _clipMatrix[2];
            _frustum[(int)ClippingPlane.Front, 1] = _clipMatrix[7] + _clipMatrix[6];
            _frustum[(int)ClippingPlane.Front, 2] = _clipMatrix[11] + _clipMatrix[10];
            _frustum[(int)ClippingPlane.Front, 3] = _clipMatrix[15] + _clipMatrix[14];
            NormalizePlane(_frustum, (int)ClippingPlane.Front);
        }
    }

    public static class Frustum3
    {
        public static bool frustrumCheck(Mesh m, Camera Cam)
        {
            Vector4 vSpacePos = new Vector4(m.Position, 1) * m.ModelViewProjectionMatrix;

            //
            float boundingSphereR = 0.0f;
            for (int i = 0; i < m.Parts.Count; i++)
                boundingSphereR = (float)Math.Max(boundingSphereR, m.Parts[i].BoundingSphere.Outer);
            //

            float range = boundingSphereR; //m.boundingSphere;

            float distToDrawAble = (Cam.Position - m.Position).Length;

            if (distToDrawAble < range * 1.5f)
                return true;

            if (distToDrawAble - range > Cam.zFar)
                return false;

            if (vSpacePos.W <= 0)
                return false;

            range /= vSpacePos.W * 0.6f;

            if (float.IsNaN(range) || float.IsInfinity(range))
                return false;

            vSpacePos /= vSpacePos.W;

            return (
                vSpacePos.X < (1f + range) &&
                vSpacePos.X > -(1f + range) &&
                vSpacePos.Y < (1f + range * Cam.AspectRatio) &&
                vSpacePos.Y > -(1f + range * Cam.AspectRatio)
                );
        }
    }
}
