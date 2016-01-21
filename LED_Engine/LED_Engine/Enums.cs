using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LED_Engine
{
    public enum DrawMode
    {
        Default,
        Depth,
        Diffuse,
        Light,
        Normals,
        Position,
        Reflection,
        AmbientOclusion,
        Emissive,
        Specular,
        Shininess,
    };
    public enum ActorType
    {
        Static,
        Movable,
        Physical,
    };
}
