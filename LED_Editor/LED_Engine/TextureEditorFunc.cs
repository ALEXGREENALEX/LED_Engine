using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace LED_Engine
{
    public partial class TextureEditor
    {
        #region Variables
        public static float FOV = 50.0f;
        public static float zNear = 0.1f;
        public static float zFar = 500.0f;
        static Material Material1 = new Material();
        static Shader Shader1 = new Shader();
        public static Camera MainCamera = new Camera();
        public static Mesh SkyCube = null;
        public static Mesh TexPlain = null;
        public string[] CubemapFiles = new string[6];
        public string TextureFile;
        public static int Cubemap;
        #endregion

        #region Camera
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
        #endregion

        #region Mesh
        public class Mesh
        {
            public Vector3 Position = Vector3.Zero;
            public Vector3 Rotation = Vector3.Zero;
            public Vector3 Scale = Vector3.One;

            public Matrix4 ModelMatrix = Matrix4.Identity;
            public Matrix4 ModelViewMatrix = Matrix4.Identity;
            public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

            public int IndexBufferID, VertexBufferID, NormalBufferID, UVBufferID;

            public Vector3[] Vertexes, Normals;
            public Vector2[] UVs;
            public int[] Indexes;

            public Material Material;

            public string Name = String.Empty;

            public void GenBuffers()
            {
                FreeBuffers();

                IndexBufferID = GL.GenBuffer();
                VertexBufferID = GL.GenBuffer();
                NormalBufferID = GL.GenBuffer();
                UVBufferID = GL.GenBuffer();
            }

            public void BindBuffers()
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Vertexes.Length * Vector3.SizeInBytes), Vertexes, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBufferID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Normals.Length * Vector3.SizeInBytes), Normals, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, UVBufferID);
                GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(UVs.Length * Vector2.SizeInBytes), UVs, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indexes.Length * sizeof(int)), Indexes, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            public void FreeBuffers()
            {
                GL.DeleteBuffer(IndexBufferID);
                GL.DeleteBuffer(VertexBufferID);
                GL.DeleteBuffer(NormalBufferID);
                GL.DeleteBuffer(UVBufferID);

                IndexBufferID = 0;
                VertexBufferID = 0;
                NormalBufferID = 0;
                UVBufferID = 0;
            }

            public void Free()
            {
                FreeBuffers();

                Indexes = null;
                Vertexes = null;
                Normals = null;
                UVs = null;
            }

            public void CalculateMatrices(Camera Camera)
            {
                ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
                ModelViewMatrix = ModelMatrix * Camera.GetViewMatrix();
                ModelViewProjectionMatrix = ModelViewMatrix * Camera.GetProjectionMatrix();
            }

            public static Mesh MakePlain(float Sides = 1.0f)
            {
                return MakePlain(Sides, Sides);
            }

            public static Mesh MakePlain(float SideA, float SideB)
            {
                if (SideA < 0.0f)
                    SideA = -SideA;
                if (SideB < 0.0f)
                    SideB = -SideB;

                float HalfA = SideA / 2.0f;
                float HalfB = SideB / 2.0f;

                Mesh plain = new Mesh();
                plain.Name = "Plain";

                #region Vertexes, Normals, UVs, Indexes
                plain.Vertexes = new Vector3[]
            {
                new Vector3(-HalfA,  0, -HalfB),
                new Vector3(-HalfA,  0,  HalfB),
                new Vector3( HalfA,  0,  HalfB),
                new Vector3(-HalfA,  0, -HalfB),
                new Vector3( HalfA,  0,  HalfB),
                new Vector3( HalfA,  0, -HalfB)
            };

                plain.Normals = new Vector3[]
            {
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
            };

                plain.UVs = new Vector2[] {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
            };

                plain.Indexes = new int[] { 0, 1, 2, 3, 4, 5 };
                #endregion

                plain.GenBuffers();
                plain.BindBuffers();

                return plain;
            }

            public static Mesh MakeBox(float Sides = 1.0f, bool FlipPolygons = false)
            {
                return MakeBox(Sides, Sides, Sides, FlipPolygons);
            }

            public static Mesh MakeBox(float SideA, float SideB, float SideC, bool FlipPolygons = false)
            {
                if (SideA < 0.0f)
                    SideA = -SideA;
                if (SideB <= 0.0f)
                    SideB = -SideB;
                if (SideC <= 0.0f)
                    SideC = -SideC;

                float HalfA = SideA / 2.0f;
                float HalfB = SideB / 2.0f;
                float HalfC = SideC / 2.0f;

                Mesh box = new Mesh();
                box.Name = "Box";

                #region Vertexes, Normals, UVs, Indexes
                if (FlipPolygons)
                {
                    box.Vertexes = new Vector3[] {
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    new Vector3( HalfA, -HalfB,  HalfC), //5
                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3(-HalfA, -HalfB, -HalfC), //0
                    new Vector3(-HalfA, -HalfB,  HalfC), //1

                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3( HalfA, -HalfB,  HalfC), //5
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    
                    new Vector3( HalfA, -HalfB,  HalfC), //5
                    new Vector3( HalfA,  HalfB,  HalfC), //7
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3( HalfA, -HalfB,  HalfC), //5

                    new Vector3( HalfA, -HalfB, -HalfC), //4
                    new Vector3( HalfA,  HalfB, -HalfC), //6
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3(-HalfA, -HalfB, -HalfC), //0
                    new Vector3( HalfA, -HalfB, -HalfC), //4

                    new Vector3(-HalfA, -HalfB, -HalfC), //0
                    new Vector3(-HalfA,  HalfB, -HalfC), //2
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3(-HalfA,  HalfB,  HalfC), //3
                    new Vector3(-HalfA, -HalfB,  HalfC), //1
                    new Vector3(-HalfA, -HalfB, -HalfC) }; //0

                    box.Normals = new Vector3[] {
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),

                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),

                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),

                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),

                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),

                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0) };

                    box.UVs = new Vector2[] {
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0)};
                }
                else
                {
                    box.Vertexes = new Vector3[] {
                    new Vector3(-HalfA, -HalfB,  HalfC),
                    new Vector3(-HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB,  HalfC),
                    new Vector3(-HalfA, -HalfB,  HalfC),

                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),

                    new Vector3(-HalfA, -HalfB,  HalfC),
                    new Vector3( HalfA, -HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA, -HalfB,  HalfC),

                    new Vector3( HalfA, -HalfB,  HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB,  HalfC),
                    new Vector3( HalfA, -HalfB,  HalfC),

                    new Vector3( HalfA, -HalfB, -HalfC),
                    new Vector3(-HalfA, -HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA,  HalfB, -HalfC),
                    new Vector3( HalfA, -HalfB, -HalfC),

                    new Vector3(-HalfA, -HalfB, -HalfC),
                    new Vector3(-HalfA, -HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB,  HalfC),
                    new Vector3(-HalfA,  HalfB, -HalfC),
                    new Vector3(-HalfA, -HalfB, -HalfC) };

                    box.Normals = new Vector3[] {
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),
                    new Vector3( 0, -1,  0),

                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),
                    new Vector3( 0,  1,  0),

                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),
                    new Vector3( 0,  0,  1),

                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),
                    new Vector3( 1,  0,  0),

                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),
                    new Vector3( 0,  0, -1),

                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0),
                    new Vector3(-1,  0,  0) };

                    box.UVs = new Vector2[] {
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0) };
                }

                box.Indexes = new int[] {
                    0, 1, 2, 3, 4, 5,
                    6, 7, 8, 9, 10, 11,
                    12, 13, 14, 15, 16, 17,
                    18, 19, 20, 21, 22, 23,
                    24, 25, 26, 27, 28, 29,
                    30, 31, 32, 33, 34, 35 };
                #endregion

                box.GenBuffers();
                box.BindBuffers();

                return box;
            }
        }
        #endregion

        #region Material
        public class Material
        {
            public string Name = String.Empty;
            public Shader Shader;
            public int TextureID = 0;

            public Material()
            {
            }
        }
        #endregion

        #region Shader
        public class UniformInfo
        {
            public String name = "";
            public int address = -1;
            public int size = 0;
            public ActiveUniformType type;

            public UniformInfo() { }

            public UniformInfo(string Name, int Address)
            {
                name = Name;
                address = Address;
            }
        }

        public class AttributeInfo
        {
            public String name = "";
            public int address = -1;
            public int size = 0;
            public ActiveAttribType type;

            public AttributeInfo() { }
        }

        public class BuffersInfo
        {
            public string Name = "";
            public uint Value = 0;

            public BuffersInfo(string name, uint value)
            {
                Name = name;
                Value = value;
            }
        }

        public class Shader
        {
            public int ProgramID = 0;
            public int VShaderID = 0;
            public int FShaderID = 0;
            public int AttributeCount = 0;
            public int UniformCount = 0;
            public string VS_File = String.Empty;
            public string FS_File = String.Empty;

            public List<AttributeInfo> Attributes = new List<AttributeInfo>();
            public List<UniformInfo> Uniforms = new List<UniformInfo>();
            public List<BuffersInfo> Buffers = new List<BuffersInfo>();

            public void LoadShader()
            {
                ProgramID = GL.CreateProgram();

                //string VSCode, FSCode;

                LoadShaderFromString(@"#version 330
                layout(location = 0) in vec3 v_Position;
                layout(location = 1) in vec3 v_Normal;
                layout(location = 2) in vec2 v_UV;

                uniform mat4 MVP;

                out vec2 f_UV;
                smooth out vec3 f_CubeUV;

                void main()
                {
                    f_UV = v_UV;
	                f_CubeUV = v_Position;
                    gl_Position = MVP * vec4(v_Position, 1.0);
                }
                ", ShaderType.VertexShader);

                LoadShaderFromString(@"#version 330
                uniform sampler2D TextureUnit0;
                uniform samplerCube TextureUnit1;

                in vec2 f_UV;
                smooth in vec3 f_CubeUV;

                uniform bool CubeMap;

                layout (location = 0) out vec4 FragColor;

                void main()
                {
	                if(CubeMap)
		            FragColor = texture(TextureUnit1, f_CubeUV);
	                else
		            FragColor = texture(TextureUnit0, f_UV);
                }
                ", ShaderType.FragmentShader);

                Link();
                GenBuffers();
                TextureUnits();
            }

            public void loadShader(String code, ShaderType type, out int shader)
            {
                shader = GL.CreateShader(type);
                GL.ShaderSource(shader, code);
                GL.CompileShader(shader);
                GL.AttachShader(ProgramID, shader);
            }

            public string LoadShaderFromString(String code, ShaderType type)
            {
                string Shader = String.Empty;
                try
                {
                    Shader = code;
                    if (type == ShaderType.VertexShader)
                        loadShader(code, type, out VShaderID);
                    else if (type == ShaderType.FragmentShader)
                        loadShader(code, type, out FShaderID);
                    return Shader;
                }
                catch
                {
                    return code;
                }
            }

            public void Link()
            {
                GL.LinkProgram(ProgramID);

                GL.GetProgram(ProgramID, GetProgramParameterName.ActiveAttributes, out AttributeCount);
                GL.GetProgram(ProgramID, GetProgramParameterName.ActiveUniforms, out UniformCount);

                for (int i = 0; i < AttributeCount; i++)
                {
                    AttributeInfo info = new AttributeInfo();
                    int length = 0;

                    StringBuilder name = new StringBuilder();

                    GL.GetActiveAttrib(ProgramID, i, 256, out length, out info.size, out info.type, name);

                    info.name = name.ToString();
                    info.address = GL.GetAttribLocation(ProgramID, info.name);
                    Attributes.Add(info);
                }

                for (int i = 0; i < UniformCount; i++)
                {
                    UniformInfo info = new UniformInfo();
                    int length = 0;

                    StringBuilder name = new StringBuilder();

                    GL.GetActiveUniform(ProgramID, i, 256, out length, out info.size, out info.type, name);

                    info.name = name.ToString();
                    info.address = GL.GetUniformLocation(ProgramID, info.name);
                    Uniforms.Add(info);
                }
            }

            public void GenBuffers()
            {
                for (int i = 0; i < Attributes.Count; i++)
                {
                    uint buffer = 0;
                    GL.GenBuffers(1, out buffer);

                    Buffers.Add(new BuffersInfo(Attributes[i].name, buffer));
                }

                for (int i = 0; i < Uniforms.Count; i++)
                {
                    uint buffer = 0;
                    GL.GenBuffers(1, out buffer);

                    Buffers.Add(new BuffersInfo(Uniforms[i].name, buffer));
                }
            }

            public void TextureUnits()
            {
                GL.UseProgram(ProgramID);

                for (int i = 0; i < 2; i++)
                {
                    int TextureUnitLocation = GetUniform("TextureUnit" + i.ToString());
                    if (TextureUnitLocation != -1)
                        GL.Uniform1(TextureUnitLocation, i);
                }

                GL.UseProgram(0);
            }

            public void EnableVertexAttribArrays()
            {
                for (int i = 0; i < Attributes.Count; i++)
                    GL.EnableVertexAttribArray(Attributes[i].address);
            }

            public void DisableVertexAttribArrays()
            {
                for (int i = 0; i < Attributes.Count; i++)
                    GL.DisableVertexAttribArray(Attributes[i].address);
            }

            public int GetAttribute(string name)
            {
                foreach (var item in Attributes)
                    if (item.name == name)
                        return item.address;
                return -1;
            }

            public int GetUniform(string name)
            {
                foreach (var item in Uniforms)
                    if (item.name == name)
                        return item.address;
                return -1;
            }

            public uint GetBuffer(string name)
            {
                foreach (var item in Buffers)
                    if (item.Name == name)
                        return item.Value;
                return 0;
            }

            public void Free()
            {
                try
                {
                    GL.DetachShader(ProgramID, VShaderID);
                    GL.DetachShader(ProgramID, FShaderID);

                    GL.DeleteShader(VShaderID);
                    GL.DeleteShader(FShaderID);

                    GL.DeleteProgram(ProgramID);

                    for (int i = 0; i < Buffers.Count; i++)
                        GL.DeleteBuffer(Buffers[i].Value);

                    Attributes.Clear();
                    Uniforms.Clear();
                    AttributeCount = 0;
                    UniformCount = 0;

                    VShaderID = 0;
                    FShaderID = 0;
                    ProgramID = 0;
                }
                catch
                {
                    Debug.WriteLine("Shader.Free() Exception");
                }
            }
        }
        #endregion

        #region Texture
        public int Load_Texture2D(string File)
        {
            try
            {
                Cubemap = 0;
                return Load_Texture2D(new Bitmap(File));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: Cannot load Texture from File: \"{1}\".", File);
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

        public int Load_Texture2D(Bitmap Image)
        {
            try
            {
                int ID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, ID);

                // Отображаем текстуру по Y
                Image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = Image.LockBits(new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                   OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                Image.UnlockBits(data);
                data = null;

                //Filter parameters
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                return ID;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: Cannot load texture");
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

        public int Load_BlankTexture2D()
        {
            try
            {
                Bitmap Image = new Bitmap(TexturesGLControl.Width, TexturesGLControl.Height);
                using (Graphics gfx = Graphics.FromImage(Image))
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    gfx.FillRectangle(brush, 0, 0, TexturesGLControl.Width, TexturesGLControl.Height);
                }

                int ID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, ID);

                // Отображаем текстуру по Y
                Image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = Image.LockBits(new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                   OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                Image.UnlockBits(data);
                data = null;

                //Filter parameters
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                //GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                return ID;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: Cannot load texture");
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

        public int Load_TextureCubemap()
        {
            try
            {
                Cubemap = 1;
                if (CubemapFiles.Length == 6)
                {
                    Bitmap[] BMPs = new Bitmap[6];
                    for (int i = 0; i < BMPs.Length; i++)
                        BMPs[i] = new Bitmap(CubemapFiles[i]);
                    return Load_TextureCubemap(BMPs);
                }
                else
                    throw new Exception("CubemapTextures Files Count != 6");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: Cannot load CubemapTexture");
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

        public int Load_TextureCubemap(Bitmap[] CubemapTextures)
        {
            try
            {
                int ID = 0;
                if (CubemapTextures.Length == 6)
                {
                    ID = GL.GenTexture();
                    //GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.TextureCubeMap, ID);

                    // Изменяем, отображаем и вращаем текстуры
                    for (int i = 0; i < CubemapTextures.Length; i++)
                    {
                        if (i < 2 || i > 3)
                            CubemapTextures[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                        if (i == 2 || i == 3)
                            CubemapTextures[i].RotateFlip(RotateFlipType.Rotate270FlipY);
                        if (i == 2)
                            CubemapTextures[i].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }

                    TextureTarget[] targets = new TextureTarget[]
                    {
                        TextureTarget.TextureCubeMapPositiveX, TextureTarget.TextureCubeMapNegativeX,
                        TextureTarget.TextureCubeMapNegativeY, TextureTarget.TextureCubeMapPositiveY,
                        TextureTarget.TextureCubeMapPositiveZ, TextureTarget.TextureCubeMapNegativeZ
                    };

                    BitmapData data;
                    // Загружаем все текстуры граней
                    for (int i = 0; i < CubemapTextures.Length; i++)
                    {
                        data = CubemapTextures[i].LockBits(new System.Drawing.Rectangle(0, 0, CubemapTextures[i].Width,
                            CubemapTextures[i].Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexImage2D(targets[5 - i], 0, PixelInternalFormat.Rgba8, data.Width, data.Height, 0,
                            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                        CubemapTextures[i].UnlockBits(data);
                    }
                    data = null;

                    //Filter parameters
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

                    GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                }
                return ID;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: Cannot load CubemapTexture.");
                Debug.WriteLine(e.Message);
                return -1;
            }
        }
        #endregion

        #region GameFunc
        public static void DrawObject(Mesh v)
        {
            v.CalculateMatrices(MainCamera);

            // Активируем нужный TextureUnit и назначаем текстуру
            if (Cubemap == 1)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.TextureCubeMap, v.Material.TextureID);
            }
            if (Cubemap == 0)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, v.Material.TextureID);
            }


            #region Работаем с шейдерами
            Shader shader = v.Material.Shader;
            int TempLocation;

            GL.UseProgram(shader.ProgramID);

            // Передаем шейдеру матрицу ModelViewProjection, если шейдер поддерживает это (должна быть 100% поддержка).
            TempLocation = shader.GetUniform("MVP");
            if (TempLocation != -1)
                GL.UniformMatrix4(TempLocation, false, ref v.ModelViewProjectionMatrix);

            #region Передача различных параметров шейдерам

            // Передаем шейдеру массив тех TextureUnit-ов, которые используются, если шейдер поддерживает это.
            TempLocation = shader.GetUniform("CubeMap");
            if (TempLocation != -1)
            {
                //if (Cubemap == 1)
                GL.Uniform1(TempLocation, Cubemap);
                //if (Cubemap == 0)
                //    GL.Uniform1(TempLocation, 0);
            }
            #endregion
            #endregion

            shader.EnableVertexAttribArrays();

            #region Передаем шейдеру VertexPosition, VertexNormal, VertexUV, VertexTangents, VertexBiTangents
            // Передаем шейдеру буфер позицый вертексов, если шейдер поддерживает это (должна быть 100% поддержка).
            TempLocation = shader.GetAttribute("v_Position");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.VertexBufferID);
                GL.VertexAttribPointer(TempLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            }

            // Передаем шейдеру буфер нормалей, если шейдер поддерживает это.
            TempLocation = shader.GetAttribute("v_Normal");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.NormalBufferID);
                GL.VertexAttribPointer(TempLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            }

            // Передаем шейдеру буфер текстурных координат, если шейдер поддерживает это.
            TempLocation = shader.GetAttribute("v_UV");
            if (TempLocation != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, v.UVBufferID);
                GL.VertexAttribPointer(TempLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
            }
            #endregion

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, v.IndexBufferID);
            GL.DrawElements(BeginMode.Triangles, v.Indexes.Length, DrawElementsType.UnsignedInt, 0);

            // If you're using VAOs, then you should not disable attribute arrays, as they are encapsulated in the VAO.
            //shader.DisableVertexAttribArrays();
        }
        #endregion
    }
}
