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
        public Vector2 YawPitch = new Vector2((float)Math.PI, 0f);
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
            Vector3 Dir = Vector3.FromYawPitch(YawPitch.X, YawPitch.Y);
            return Matrix4.LookAt(Position, Position + Dir, Vector3.UnitY);
        }

        public void Move(float X, float Y, float Z)
        {
            Vector3 Forward = new Vector3((float)Math.Sin(YawPitch.X), 0, (float)Math.Cos(YawPitch.X));
            Vector3 Right = new Vector3(-Forward.Z, 0, Forward.X);

            Vector3 Offset = X * Right + Y * Forward;
            Offset.Y += Z; //offset.Y += Orientation.Y / 4;

            Offset.NormalizeFast();
            Offset *= MoveSpeed;

            Position += Offset;
        }

        /// <summary>
        /// Добавляет вращение от движения мыши к ориентации камеры
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void AddRotation(float X, float Y)
        {
            X *= MouseSensitivity;
            Y *= MouseSensitivity;

            YawPitch.X = (YawPitch.X + X) % MathHelper.TwoPi;
            YawPitch.Y = Math.Max(Math.Min(YawPitch.Y + Y, MathHelper.PiOver2 - 0.01f), -MathHelper.PiOver2 + 0.01f);
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
