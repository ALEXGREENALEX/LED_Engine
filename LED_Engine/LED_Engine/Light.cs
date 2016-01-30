using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Lights
    {
        public static List<Light> LIGHTS = new List<Light>();

        // TEST
        public static void MakeSomeLightsForTEST()
        {
            LIGHTS.Clear();

            Light L_Amb = new Light(LightType.Ambient);
            Light L_Dir = new Light(LightType.Directional);
            Light L_Point = new Light(LightType.Point);
            Light L_Spot = new Light(LightType.Spot);

            L_Amb.Diffuse = new Vector3(0.2f, 0.2f, 0.2f);

            L_Dir.Direction = new Vector3(0.0f, 1.0f, 0.0f);
            L_Dir.Diffuse = new Vector3(0.01f, 0.01f, 0.0f);
            L_Dir.Specular = new Vector3(0.01f, 0.3f, 0.9f);

            L_Point.Position = new Vector3(2.0f);
            L_Point.Diffuse = new Vector3(0.3f);
            L_Point.Specular = new Vector3(0.3f);

            L_Spot.Position = new Vector3(-2.0f, 2.0f, -1.0f);
            L_Spot.Direction = new Vector3(0.0f, -1.2f, 1.0f);
            L_Spot.Diffuse = new Vector3(1.0f, 1.0f, 1.0f);
            L_Spot.Specular = new Vector3(0.5f, 0.5f, 0.5f);

            LIGHTS.Add(L_Amb);
            LIGHTS.Add(L_Dir);
            LIGHTS.Add(L_Point);
            LIGHTS.Add(L_Spot);
        }
    }

    public enum LightType
    {
        Ambient,
        Directional,
        Point,
        Spot,
    }

    public class Light
    {
        LightType type;

        public bool Enabled = true;
        public Vector3 Diffuse = new Vector3(1.0f); //Diffuse intensity
        public Vector3 Specular = new Vector3(1.0f); //Specular intensity
        public Vector3 Position = new Vector3(0.0f); //Position in eye coords
        public Vector3 Direction = new Vector3(0.0f);
        public Vector3 Attenuation = new Vector3(0.0f, 0.5f, 0.01f); //Attenuation: Constant, Linear, Quadric
        float cutOff = 45.0f;
        public float Exponent = 2.0f;

        public Light(LightType LType = LightType.Point)
        {
            Type = LType;
        }

        public LightType Type
        {
            get { return type; }
            set
            {
                type = value;

                switch (type)
                {
                    case LightType.Ambient:
                        Diffuse = new Vector3(0.0f);
                        break;
                    case LightType.Directional:
                        Direction = new Vector3(0.0f, 1.0f, 0.0f);
                        break;
                    case LightType.Spot:
                        Direction = new Vector3(0.0f, -1.0f, 0.0f);
                        ClampSpotLightAngle();
                        break;
                }
            }
        }

        void ClampSpotLightAngle()
        {
            if (cutOff > 90.0f)
                cutOff = 90.0f;
            if (cutOff < 0.0f)
                cutOff = 0.0f;
        }

        public float CutOFF
        {
            get { return cutOff; }
            set
            {
                cutOff = value;
                ClampSpotLightAngle();
            }
        }
    }
}
