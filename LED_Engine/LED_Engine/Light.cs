using System;
using System.Collections.Generic;
using System.Text;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Lights
    {
        public static List<Light> LIGHTS = new List<Light>();
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
        LightType type = LightType.Point;

        public bool Enabled = true;
        public string Name = String.Empty;
        public Vector3 Diffuse = new Vector3(1.0f); //Diffuse intensity
        public Vector3 Specular = new Vector3(1.0f); //Specular intensity
        public Vector3 Position = new Vector3(0.0f); //Position in eye coords
        public Vector3 direction = new Vector3(0.0f, MathHelper.PiOver2, 0.0f);
        public Vector3 Attenuation = new Vector3(0.0f, 1.0f, 0.0f); //Attenuation: Constant, Linear, Quadric
        float cutOff = 45.0f;
        public float Exponent = 2.0f;

        public Light()
        {
        }

        public Light(LightType LType)
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
                        Direction = new Vector3(0.0f, MathHelper.PiOver2, 0.0f);
                        break;
                    case LightType.Spot:
                        Direction = new Vector3(0.0f, -1.0f, 0.0f);
                        ClampSpotLightAngle();
                        break;
                }
            }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                direction.Normalize();
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
