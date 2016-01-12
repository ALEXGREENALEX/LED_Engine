using System;
using System.Collections.Generic;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Fog
    {
        public static bool Enabled = true;
        public static float MaxDistance = 80f,
                            MinDistance = 32f;
        public static Vector3 Color = new Vector3(0.5f, 0.5f, 0.5f);
    }
}