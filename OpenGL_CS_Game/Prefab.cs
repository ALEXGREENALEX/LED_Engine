using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    class Prefab
    {
        Vector3 position = Vector3.Zero;
        Vector3 rotation = Vector3.Zero;
        Vector3 scale = Vector3.One;

        public string Name = String.Empty;
        public List<Volume> Objects = new List<Volume>();

        public Prefab()
        {
        }

        public Prefab(Volume Object)
        {
            Objects.Add(Object);
        }

        public Prefab(Volume[] ObjectsArray)
        {
            Objects.AddRange(ObjectsArray);
        }

        public Prefab(List<Volume> ObjectsList)
        {
            Objects = ObjectsList;
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                for (int i = 0; i < Objects.Count; i++)
                    Objects[i].Position += position;
            }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                for (int i = 0; i < Objects.Count; i++)
                {
                    Objects[i].Rotation += rotation;
                    // Лажа с поворотом, нужно доделать
                }
            }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                for (int i = 0; i < Objects.Count; i++)
                    Objects[i].Scale *= scale;
            }
        }
    }
}
