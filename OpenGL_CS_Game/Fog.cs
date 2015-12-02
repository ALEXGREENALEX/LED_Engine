using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    static public class Fog
    {
        public static bool Enabled = true;
        public static float MaxDistance = 80f,
                            MinDistance = 32f;
        public static Vector3 Color = new Vector3(0.5f, 0.5f, 0.5f);
    }
}