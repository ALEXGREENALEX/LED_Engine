using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public static class Models
    {
        public static List<Model> MODELS = new List<Model>(); // Loaded Models

        public static Model GetModel(string Name)
        {
            foreach (var i in MODELS)
                if (i.Name.GetHashCode() == Name.GetHashCode())
                    return i;
            return null;
        }
    }

    public class Model
    {
        public string Name = String.Empty;
        public List<Mesh> Meshes = new List<Mesh>();
        public bool Visible = true;

        public void Free()
        {
            for (int i = 0; i < Meshes.Count; i++)
                LED_Engine.Meshes.Unload(Meshes[i]);
        }
    }
}
