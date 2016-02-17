using System;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public enum ProjectionTypes
    {
        Orthographic,
        Perspective
    }

    public class Camera
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Direction = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        float fov = 90.0f;
        float znear = 0.2f;
        float zfar = 100.0f;
        float width = 800.0f;
        float height = 600.0f;
        ProjectionTypes projectionType = ProjectionTypes.Perspective;
        Matrix4 projectionMatrix = Matrix4.Identity;

        public Camera()
        {
            SetProjectionMatrix();
        }

        public Camera(ProjectionTypes Projection)
        {
            projectionType = Projection;
            SetProjectionMatrix();
        }

        public Camera(ProjectionTypes Projection, float Width, float Height, float zNear, float zFar, float FOV = 90.0f)
        {
            projectionType = Projection;
            width = Width;
            height = Height;
            znear = zNear;
            zfar = zFar;
            fov = FOV;
            SetProjectionMatrix();
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 Dir = new Vector3();
            Dir.X = (float)(Math.Cos(Direction.Y) * Math.Sin(Direction.X));
            Dir.Y = (float)Math.Sin(Direction.Y);
            Dir.Z = (float)(Math.Cos(Direction.Y) * Math.Cos(Direction.X));

            return Matrix4.LookAt(Position, Position + Dir, Vector3.UnitY);
          
        }

        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin(Direction.X), 0, (float)Math.Cos(Direction.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z; //offset.Y += Orientation.Y / 4;

            offset.NormalizeFast();
            offset *= MoveSpeed;

            Position += offset;
        }

        /// <summary>
        /// Добавляет вращение от движения мыши к ориентации камеры
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddRotation(float x, float y)
        {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Direction.X = (Direction.X + x) % (MathHelper.TwoPi);
            Direction.Y = Math.Max(Math.Min(Direction.Y + y, MathHelper.PiOver2 - 0.01f), -MathHelper.PiOver2 + 0.01f);
        }

        public float FOV
        {
            get { return fov; }
            set
            {
                fov = value;
                SetProjectionMatrix();
            }
        }

        public float zNear
        {
            get { return znear; }
            set
            {
                znear = value;
                SetProjectionMatrix();
            }
        }

        public float zFar
        {
            get { return zfar; }
            set
            {
                zfar = value;
                SetProjectionMatrix();
            }
        }

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                SetProjectionMatrix();
            }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                SetProjectionMatrix();
            }
        }

        public float AspectRatio
        {
            get
            {
                if (height != 0.0f)
                    return width / height;
                else
                    return 0.0f;
            }
        }

        public ProjectionTypes ProjectionType
        {
            get { return projectionType; }
            set
            {
                projectionType = value;
                SetProjectionMatrix();
            }
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        void SetProjectionMatrix()
        {
            if (ProjectionType == ProjectionTypes.Perspective)
                projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), Width / Height, zNear, zFar);
            else
                projectionMatrix = Matrix4.CreateOrthographic(Width, Height, zNear, zFar);
        }

        public void SetProjectionMatrix(ProjectionTypes Projection, float Width, float Height, float zNear, float zFar, float FOV = 90.0f)
        {
            projectionType = Projection;
            width = Width;
            height = Height;
            znear = zNear;
            zfar = zFar;
            fov = FOV;
            SetProjectionMatrix();
        }
    }
}
