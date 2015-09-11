using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Test1
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
            using (StreamReader sr = new StreamReader(filename))
            {
                if (type == ShaderType.VertexShader)
                {
                    loadShader(sr.ReadToEnd(), type, out VShaderID);
                }
                else if (type == ShaderType.FragmentShader)
                {
                    loadShader(sr.ReadToEnd(), type, out FShaderID);
                }
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
