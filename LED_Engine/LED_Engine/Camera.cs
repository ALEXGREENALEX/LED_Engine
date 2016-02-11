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
            
            // Variant 1
            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);

            // Variant 2
            //Vector3 target = Position + lookat;
            //Vector3 f = Vector3.Normalize(target - Position);
            //Vector3 s = Vector3.Normalize(Vector3.Cross(f, Vector3.UnitY));
            //Vector3 u = Vector3.Normalize(Vector3.Cross(s, f));

            //Matrix4 mat = Matrix4.Identity;

            //mat.M11 = s.X;
            //mat.M21 = s.Y;
            //mat.M31 = s.Z;

            //mat.M12 = u.X;
            //mat.M22 = u.Y;
            //mat.M32 = u.Z;

            //mat.M13 = -f.X;
            //mat.M23 = -f.Y;
            //mat.M33 = -f.Z;

            //mat.M14 = 0.0f;
            //mat.M24 = 0.0f;
            //mat.M34 = 0.0f;

            //mat.M41 = -Vector3.Dot(s, Position);
            //mat.M42 = -Vector3.Dot(u, Position);
            //mat.M43 = Vector3.Dot(f, Position);
            //mat.M44 = 1.0f;
            //return mat;
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
            offset.Y += z; //offset.Y += Orientation.Y / 4;

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

            Orientation.X = (Orientation.X + x) % (MathHelper.TwoPi);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, MathHelper.PiOver2 - 0.01f), -MathHelper.PiOver2 + 0.01f);
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
