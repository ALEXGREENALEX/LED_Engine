using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Xml;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace LED_Engine
{
    public static class Shaders
    {
        public static List<Shader> SHADERS = new List<Shader>(); // Loaded Shaders
        public static List<Shader> ShadersList = new List<Shader>(); // All Shaders

        public static void LoadShadersList(string XmlFile, string ShaderPath, bool EngineContent)
        {
            try
            {
                XmlDocument XML = new XmlDocument();
                XmlNodeList xmlNodeList;

                #region Shaders
                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("Shader");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Shader S = new Shader();
                    S.EngineContent = EngineContent;

                    S.Name = xmlNode.SelectSingleNode("Name").InnerText;
                    S.VS_File = Engine.CombinePaths(ShaderPath, xmlNode.SelectSingleNode("VertexShader").InnerText);
                    S.FS_File = Engine.CombinePaths(ShaderPath, xmlNode.SelectSingleNode("FragmentShader").InnerText);

                    if (xmlNode.SelectNodes("GeometryShader").Count > 0)
                        S.GS_File = Engine.CombinePaths(ShaderPath, xmlNode.SelectSingleNode("GeometryShader").InnerText);

                    ShadersList.Add(S);
                }
                #endregion

                #region RTT Shaders
                XML.Load(XmlFile);
                xmlNodeList = XML.DocumentElement.SelectNodes("RTT_Shader");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    Shader S = new Shader();
                    S.EngineContent = EngineContent;

                    S.Name = xmlNode.SelectSingleNode("Name").InnerText;
                    S.VS_File = Engine.CombinePaths(ShaderPath, xmlNode.SelectSingleNode("VertexShader").InnerText);
                    S.FS_File = Engine.CombinePaths(ShaderPath, xmlNode.SelectSingleNode("FragmentShader").InnerText);

                    if (xmlNode.SelectNodes("GeometryShader").Count > 0)
                        S.GS_File = Engine.CombinePaths(ShaderPath, xmlNode.SelectSingleNode("GeometryShader").InnerText);

                    FBO.Shaders.Add(S);
                }
                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Shaders.LoadShadersList() Exception.");
                Log.WriteLineRed("XmlFile: \"{0}\", EngineContent: \"{1}\"", XmlFile, EngineContent);
                Log.WriteLineYellow(e.Message);
            }
        }

        public static Shader Load(string Name)
        {
            try
            {
                Shader S = GetShader(Name);

                if (S == null)
                {
                    foreach (var i in ShadersList)
                        if (i.Name.GetHashCode() == Name.GetHashCode())
                            S = i;
                    if (S == null)
                        return null;
                }

                if (S.UseCounter == 0)
                {
                    S.LoadShader();
                    SHADERS.Add(S);
                }

                S.UseCounter++;

                // Проверка шейдера на ошибки
                string InfoLog = GL.GetProgramInfoLog((int)(S.ProgramID));
                if (InfoLog.Trim() != String.Empty)
                {
                    Log.WriteLineYellow("ShaderProgramInfoLog:");
                    Log.WriteLine(InfoLog);
                }

                return S;
            }
            catch(Exception e)
            {
                Log.WriteLineRed("Shaders.Load() Exception, Name: \"{0}\"", Name);
                Log.WriteLineYellow("Message: \n", e.Message);
                return null;
            }
        }

        public static Shader GetShader(string Name)
        {
            foreach (var i in SHADERS)
                if (i.Name.GetHashCode() == Name.GetHashCode())
                    return i;
            return null;
        }

        public static Shader GetShader(uint ProgramID)
        {
            foreach (var i in SHADERS)
                if (i.ProgramID == ProgramID)
                    return i;
            return null;
        }

        public static void Unload(string Name)
        {
            Unload(GetShader(Name).ProgramID);
        }

        public static void Unload(uint ProgramID)
        {
            try
            {
                Shader S = GetShader(ProgramID);
                if (S != null)
                {
                    S.UseCounter--;

                    if (S.UseCounter == 0)
                    {
                        S.Free();
                        if (!S.EngineContent)
                            SHADERS.Remove(S);
                    }
                }
            }
            catch
            {
                Log.WriteLineRed("Shaders.Delete() Exception, ProgramID: \"{0}\"", ProgramID);
            }
        }

        public static void Free(bool WithEngineContent = false)
        {
            try
            {
                if (WithEngineContent)
                {
                    foreach (var i in SHADERS)
                        i.Free();

                    SHADERS.Clear();
                    ShadersList.Clear();
                }
                else
                {
                    int Count = SHADERS.Count;
                    for (int i = 0; i < Count; i++)
                        if (!SHADERS[i].EngineContent)
                        {
                            SHADERS[i].Free();
                            SHADERS.RemoveAt(i);
                            Count--;
                        }
                }
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Shaders.Free() Exception.");
                Log.WriteLineYellow(e.Message);
            }
        }
    }

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

    public class Shader
    {
        const string StrTextureUnit = "TextureUnit"; // In "FragmentShader.glsl" -> "uniform sampler2D TextureUnit..."
        public uint UseCounter = 0;
        public bool EngineContent = false;

        public uint ProgramID = 0;
        public uint VShaderID = 0;
        public uint GShaderID = 0;
        public uint FShaderID = 0;
        public int AttributeCount = 0;
        public int UniformCount = 0;
        public string Name = String.Empty;
        public string VS_File = String.Empty;
        public string FS_File = String.Empty;
        public string GS_File = String.Empty;

        public List<AttributeInfo> Attributes = new List<AttributeInfo>();
        public List<UniformInfo> Uniforms = new List<UniformInfo>();
        public List<BuffersInfo> Buffers = new List<BuffersInfo>();

        public void LoadShader()
        {
            ProgramID = GL.CreateProgram();

            string VSCode, FSCode;
            string GSCode = String.Empty;

            VSCode = LoadShaderFromFile(VS_File, ShaderType.VertexShader);
            FSCode = LoadShaderFromFile(FS_File, ShaderType.FragmentShader);

            if (GS_File != String.Empty)
                GSCode = LoadShaderFromFile(GS_File, ShaderType.GeometryShader);

            Link();
            GenBuffers();
            TextureUnits();

            #region Check for Errors
            string InfoLog = GL.GetProgramInfoLog((int)ProgramID);
            if (InfoLog != String.Empty)
            {
                Log.WriteLineRed("Shader Program Error: \"{0}\"", Name);
                Log.WriteLine(InfoLog);

                Log.WriteLineYellow("Vertex shader code:");
                Log.WriteWithLineNumbers(VSCode);

                Log.WriteLineYellow("Fragment shader code:");
                Log.WriteWithLineNumbers(FSCode);

                if (GS_File != String.Empty)
                {
                    Log.WriteLineYellow("Geometry shader code:");
                    Log.WriteWithLineNumbers(GSCode);
                }
            }
            else
            {
                VSCode = null;
                FSCode = null;
                GSCode = null;
            }
            #endregion
        }

        void loadShader(String code, ShaderType type, out uint shader)
        {
            shader = GL.CreateShader(type);
            GL.ShaderSource(shader, code);
            GL.CompileShader(shader);
            GL.AttachShader(ProgramID, shader);
        }

        void LoadShaderFromString(String code, ShaderType type)
        {
            if (type == ShaderType.VertexShader)
                loadShader(code, type, out VShaderID);
            else if (type == ShaderType.FragmentShader)
                loadShader(code, type, out FShaderID);
            else if (type == ShaderType.GeometryShader)
                loadShader(code, type, out GShaderID);
        }

        string LoadShaderFromFile(String FileName, ShaderType type)
        {
            string Shader = String.Empty;
            try
            {
                Shader = File.ReadAllText(FileName);

                while (Shader.ToLower().IndexOf("#include") != -1)
                    Shader = ShaderInclude(Shader, FileName);

                LoadShaderFromString(Shader, type);
                return Shader;
            }
            catch
            {
                Log.WriteLineRed("LoadShaderFromFile(\"{0}\") Exception!\n", FileName);
                if (Shader != String.Empty)
                {
                    Log.WriteLineRed(Shader);
                    return Shader;
                }
                else
                    return FileName;
            }
        }

        /// <summary>
        /// Include Function for Shaders
        /// </summary>
        /// <param name="Str">Shader code with #include("") inside (or without)</param>
        /// <param name="FullFileName">FILE with full Path for #include("FILE") for every recursive iteration.</param>
        /// <param name="StrBuffer">DON'T TOUCH IT! Text from previous iteration, it used for iterations only!</param>
        /// <returns>Shader code with included files.</returns>
        static string ShaderInclude(string Str, string FullFileName, string StrBuffer = "")
        {
            try
            {
                Str = DeleteComments(Str);

                #region #include(), #include_once()
                #region Extract "#include(...)";
                string NewShaderFullPath = String.Empty;
                string TextToInclude = String.Empty;

                bool IncludeOnce = false; // "#include_once"?
                bool IncludeString = false; // "#include_string"?
                const string ConstInclude = "#include";
                const char ConstQuoteMark = '\"';


                int Start = Str.ToLower().IndexOf(ConstInclude);
                int InsertPos = Start;

                if (InsertPos == -1) // "#include" not found
                    return Str;

                Start = Str.IndexOf('(', Start) + 1;
                int End = Str.IndexOf(')', Start);

                // if "#include_once"
                if (Str.Substring(InsertPos, Start - InsertPos).ToLower().IndexOf("once") != -1)
                    IncludeOnce = true;
                else
                    IncludeOnce = false;

                // if "#include_string"
                if (Str.Substring(InsertPos, Start - InsertPos).ToLower().IndexOf("string") != -1)
                    IncludeString = true;
                else
                    IncludeString = false;

                // Remove "#include()" from Shader
                string IncludeArgs = Str.Substring(Start, End - Start);
                Str = Str.Remove(InsertPos, End - InsertPos + 1);
                #endregion

                #region TextToInclude
                if (IncludeString)
                {
                    #region Select string from Arg to TextToInclude
                    int A = IncludeArgs.IndexOf(ConstQuoteMark) + 1;
                    int B = IncludeArgs.LastIndexOf(ConstQuoteMark);
                    TextToInclude = IncludeArgs.Substring(A, B - A);
                    #endregion
                }
                else
                {
                    #region Select string from File in Arg[0] to TextToInclude
                    string[] IncArgs = IncludeArgs.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (IncArgs.Length > 0 && IncArgs.Length <= 3)
                    {
                        int A = IncArgs[0].IndexOf(ConstQuoteMark) + 1;
                        int B = IncArgs[0].LastIndexOf(ConstQuoteMark);
                        NewShaderFullPath = Engine.CombinePaths(Path.GetDirectoryName(FullFileName), IncArgs[0].Substring(A, B - A));

                        switch (IncArgs.Length)
                        {
                            case 1:
                                TextToInclude = File.ReadAllText(NewShaderFullPath);
                                break;

                            case 2:
                                A = Convert.ToInt32(IncArgs[1]) - 1;
                                TextToInclude = File.ReadAllLines(NewShaderFullPath)[A];
                                break;

                            case 3:
                                A = Convert.ToInt32(IncArgs[1]) - 1;
                                B = Convert.ToInt32(IncArgs[2]) - 1;
                                string[] Lines = File.ReadAllLines(NewShaderFullPath);
                                for (int i = A + 1; i <= B; i++)
                                    Lines[A] += "\r\n" + Lines[i];
                                TextToInclude = Lines[A];
                                break;
                        }
                    }
                    else
                        throw new Exception();
                    #endregion
                }
                #endregion

                while (TextToInclude.ToLower().Contains(ConstInclude))
                    TextToInclude = ShaderInclude(TextToInclude, NewShaderFullPath, Str);

                #region IncludeOnce
                if (IncludeOnce)
                {
                    if (!(Str + StrBuffer).Contains(TextToInclude))
                        Str = Str.Insert(InsertPos, TextToInclude);
                }
                else
                    Str = Str.Insert(InsertPos, TextToInclude);
                #endregion
                #endregion
            }
            catch
            {
                Log.WriteLineRed("Shader Include Error!");
                Log.WriteLineYellow("Shader: {0}", Str);
            }
            return Str;
        }

        static string DeleteComments(string Str)
        {
            try
            {
                const string ConstCommentString = "//";
                const string ConstCommentEndLine = "\n";
                const string ConstCommentStart = "/*";
                const string ConstCommentStop = "*/";

                int Start, StartIndex, End, Len;
                do
                {
                    Start = Str.IndexOf(ConstCommentStart);
                    End = -1;
                    if (Start != -1)
                    {
                        StartIndex = Start + ConstCommentStart.Length;
                        if (StartIndex < Str.Length)
                            End = Str.IndexOf(ConstCommentStop, StartIndex);

                        Len = End - Start + ConstCommentStop.Length;
                        if (Start < End)
                            Str = Str.Remove(Start, Len);
                    }
                } while (Start != -1 && End != -1);

                do
                {
                    Start = Str.IndexOf(ConstCommentString);
                    End = Str.Length;
                    if (Start != -1)
                    {
                        StartIndex = Start + ConstCommentString.Length;
                        if (StartIndex < Str.Length)
                            End = Str.IndexOf(ConstCommentEndLine, StartIndex);
                        if (End == -1)
                            End = Str.Length;

                        Len = End - Start;
                        if (Start < End)
                            Str = Str.Remove(Start, Len);
                    }
                } while (Start != -1);
            }
            catch (Exception e)
            {
                Log.WriteLineRed("Delete Comments Error!\n{0}", e.Message);
                Log.WriteLine(Str);
            }
            return Str;
        }

        public void Link()
        {
            GL.LinkProgram(ProgramID);

            GL.GetProgram(ProgramID, ProgramParameter.ActiveAttributes, out AttributeCount);
            GL.GetProgram(ProgramID, ProgramParameter.ActiveUniforms, out UniformCount);

            for (uint i = 0; i < AttributeCount; i++)
            {
                AttributeInfo info = new AttributeInfo();
                int length = 0;

                StringBuilder name = new StringBuilder();

                GL.GetActiveAttrib(ProgramID, i, 256, out length, out info.size, out info.type, name);

                info.name = name.ToString();
                info.address = GL.GetAttribLocation(ProgramID, info.name);
                Attributes.Add(info);
            }

            for (uint i = 0; i < UniformCount; i++)
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

            for (int i = 0; i < Settings.GL.TextureImageUnits; i++)
            {
                int TextureUnitLocation = GetUniform(StrTextureUnit + i.ToString());
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
                GL.DetachShader(ProgramID, GShaderID);
                GL.DetachShader(ProgramID, FShaderID);

                GL.DeleteShader(VShaderID);
                GL.DeleteShader(GShaderID);
                GL.DeleteShader(FShaderID);

                GL.DeleteProgram(ProgramID);

                for (int i = 0; i < Buffers.Count; i++)
                    GL.DeleteBuffer(Buffers[i].Value);

                Attributes.Clear();
                Uniforms.Clear();
                AttributeCount = 0;
                UniformCount = 0;

                VShaderID = 0;
                GShaderID = 0;
                FShaderID = 0;
                ProgramID = 0;

                UseCounter = 0;
            }
            catch
            {
                Log.WriteLineRed("Shader.Free() Exception");
            }
        }
    }
}
