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
        AmbientOclusion,
        Emissive,
        Specular,
        ShininessReflection,
    };

    public enum MeshType
    {
        Mesh,
        Plain,
        Box,
    };

    public enum ActorType
    {
        Static,
        Movable,
        Physical,
    };
}
