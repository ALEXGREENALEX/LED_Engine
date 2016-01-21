using System;
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
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        float fov = 90.0f;
        float znear = 0.2f;
        float zfar = 100.0f;
        float width = 800.0f;
        float height = 600.0f;
        ProjectionTypes projectionType = ProjectionTypes.Perspective;
        Matrix4 projectionMatrix;

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

        /// <summary>
        /// Создать матрицу вида для этой камеры
        /// </summary>
        /// <returns>Матрица вида, смотрящая в напрвлении камеры</returns>
        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }

        /// <summary>
        /// Смещение положения камеры в координатах относительно его текущей ориентации
        /// </summary>
        /// <param name="x">Движение камеры по земле (влево / вправо)</param>
        /// <param name="y">Движение вдоль оси камеры (вперед / назад)</param>
        /// <param name="z">Движение по высоте</param>
        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin((float)Orientation.X), 0, (float)Math.Cos((float)Orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

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

            Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
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
