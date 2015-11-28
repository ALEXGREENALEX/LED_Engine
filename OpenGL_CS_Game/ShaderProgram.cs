using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
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

        public AttributeInfo(string Name, int Address)
        {
            name = Name;
            address = Address;
        }
    }

    public class BuffersInfo
    {
        public string Name = "";
        public uint Value = 0;

        public BuffersInfo() { }
        public BuffersInfo(string name, uint value)
        {
            Name = name;
            Value = value;
        }
    }

    class ShaderProgram
    {
        public int ProgramID = -1;
        public int VShaderID = -1;
        public int FShaderID = -1;
        public int AttributeCount = 0;
        public int UniformCount = 0;

        public List<AttributeInfo> Attributes = new List<AttributeInfo>();
        public List<UniformInfo> Uniforms = new List<UniformInfo>();
        public List<BuffersInfo> Buffers = new List<BuffersInfo>();

        public ShaderProgram()
        {
            ProgramID = GL.CreateProgram();
        }

        private void loadShader(String code, ShaderType type, out int address)
        {
            address = GL.CreateShader(type);
            GL.ShaderSource(address, code);
            GL.CompileShader(address);
            GL.AttachShader(ProgramID, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        public void LoadShaderFromString(String code, ShaderType type)
        {
            if (type == ShaderType.VertexShader)
            {
                loadShader(code, type, out VShaderID);
            }
            else if (type == ShaderType.FragmentShader)
            {
                loadShader(code, type, out FShaderID);
            }
        }

        public void LoadShaderFromFile(String filename, ShaderType type)
        {
            try
            {
                string Shader = File.ReadAllText(filename);

                while (Shader.ToLower().IndexOf("#include") != -1)
                    Shader = ShaderInclude(Shader, filename);

                if (type == ShaderType.VertexShader)
                    loadShader(Shader, type, out VShaderID);
                else if (type == ShaderType.FragmentShader)
                    loadShader(Shader, type, out FShaderID);
            }
            catch
            {
                MessageBox.Show("\"LoadShaderFromFile\" Error!\n\n" + filename);
            }
        }

        static string ShaderInclude(string Shader, string ShaderPath)
        {
            try
            {
                const string ConstCommentString = "//";
                const string ConstCommentEndLine = "\n";
                const string ConstCommentStart = "/*";
                const string ConstCommentStop = "*/";

                int Start, End, Len;
                do
                {
                    Start = Shader.IndexOf(ConstCommentStart);
                    End = Shader.IndexOf(ConstCommentStop, Start + ConstCommentStart.Length);
                    Len = End - Start + ConstCommentStop.Length;
                    if (Start < End && Start != -1 && End != -1)
                        Shader = Shader.Remove(Start, Len);
                } while (Start != -1 && End != -1);

                do
                {
                    Start = Shader.IndexOf(ConstCommentString);
                    End = Shader.IndexOf(ConstCommentEndLine, Start + ConstCommentString.Length);
                    Len = End - Start;
                    if (Start < End && Start != -1 && End != -1)
                        Shader = Shader.Remove(Start, Len);
                } while (Start != -1 && End != -1);
            }
            catch
            {
                MessageBox.Show("Shader Include (Delete Comments) Error!\n" + ShaderPath);
            }

            try
            {
                
                const string ConstInclude = "#include";
                int Start = Shader.ToLower().IndexOf(ConstInclude);
                int InsertPos = Start;

                if (InsertPos == -1) //Include not found
                    return Shader;

                Start = Shader.IndexOf('(', Start) + 1;
                int End = Shader.IndexOf(')', Start);

                string Include = Shader.Substring(Start, End - Start);
                Shader = Shader.Remove(InsertPos, End - InsertPos + 1);

                string[] IncArgs = Include.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (IncArgs.Length > 0 && IncArgs.Length <= 3)
                {
                    int A = IncArgs[0].IndexOf('\"') + 1;
                    int B = IncArgs[0].LastIndexOf('\"');
                    string NewShaderPath = Path.Combine(Path.GetDirectoryName(ShaderPath), IncArgs[0].Substring(A, B - A));

                    switch (IncArgs.Length)
                    {
                        case 1:
                            Shader = Shader.Insert(InsertPos, File.ReadAllText(NewShaderPath));
                            break;
                        case 2:
                            A = Convert.ToInt32(IncArgs[1]) - 1;
                            Shader = Shader.Insert(InsertPos, File.ReadAllLines(NewShaderPath)[A]);
                            break;
                        case 3:
                            A = Convert.ToInt32(IncArgs[1]) - 1;
                            B = Convert.ToInt32(IncArgs[2]) - 1;
                            string[] Lines = File.ReadAllLines(NewShaderPath);
                            for (int i = A + 1; i <= B; i++)
                                Lines[A] += "\r\n" + Lines[i];
                            Shader = Shader.Insert(InsertPos, Lines[A]);
                            break;
                    }
                }
                else
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Shader Include Error!\n" + ShaderPath);
            }
            return Shader;
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
                Uniforms.Add(info);
                info.address = GL.GetUniformLocation(ProgramID, info.name);
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

        public ShaderProgram(String vshader, String fshader, bool fromFile = false)
        {
            ProgramID = GL.CreateProgram();

            if (fromFile)
            {
                LoadShaderFromFile(vshader, ShaderType.VertexShader);
                LoadShaderFromFile(fshader, ShaderType.FragmentShader);
            }
            else
            {
                LoadShaderFromString(vshader, ShaderType.VertexShader);
                LoadShaderFromString(fshader, ShaderType.FragmentShader);
            }

            Link();
            GenBuffers();
        }
    }
}
