using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    static public class Fog
    {
        public static bool Enabled = false;
        public static float MaxDistance = 32f,
                            MinDistance = 16f;
        public static Vector3 Color = new Vector3(0.5f, 0.5f, 0.5f);
    }
}