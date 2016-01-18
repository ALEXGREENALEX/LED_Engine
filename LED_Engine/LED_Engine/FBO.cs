using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    static public class FBO
    {
        public static int G_ShaderIndex, PP_ShaderIndex; // It's not a Shader.ProgramID !!!
        public static List<Shader> Shaders = new List<Shader>();

        public static int FBO_G, FBO_PP; //G - G-Buffer render pass, PP - PostProcess render pass.
        public static bool UsePostEffects = true;

        static DrawBuffersEnum[] ColorAttachments = new DrawBuffersEnum[] {
            DrawBuffersEnum.ColorAttachment0, //Kd * DiffuseMap,        EyePosition.x
            DrawBuffersEnum.ColorAttachment1, //Normals * NormalMap,    EyePosition.y
            DrawBuffersEnum.ColorAttachment2, //Ke * EmissiveMap,       EyePosition.z
            DrawBuffersEnum.ColorAttachment3, //Ks * SpecularMap,       Shininess
            DrawBuffersEnum.ColorAttachment4};//Ka * Ambient Oclusion,  NOTHING

        public static int Depth_Texture; //Depth
        public static int[] G_Textures = new int[ColorAttachments.Length]; //G-Buffer Textures
        public static int PP_Texture; //PostProcess Texture

        static int ScreenWidth, ScreenHeight;
        static int VBO;
        static float[] FBO_Vertexes = new float[] {
            -1f, -1f,
             1f, -1f,
            -1f,  1f,
             1f,  1f }; // Screen vertexes positions

        public static void Init(int ScrWidth, int ScrHeight)
        {
            FramebufferErrorCode FramebufferStatus;

            ScreenWidth = ScrWidth;
            ScreenHeight = ScrHeight;

            for (int i = 0; i < Shaders.Count; i++)
            {
                Shaders[i].LoadShader();

                if (Shaders[i].Name == "FBO")
                    G_ShaderIndex = i;
                else if (Shaders[i].Name == "PostProcess")
                    PP_ShaderIndex = i;
            }

            //Gen Textures for Deffered rendering, Post Processesing
            Depth_Texture = GL.GenTexture();
            GL.GenTextures(G_Textures.Length, G_Textures);
            PP_Texture = GL.GenTexture();

            Rescale();

            //G-Buffers
            FBO_G = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO_G);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, Depth_Texture, 0);
            for (int i = 0; i < G_Textures.Length; i++)
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext + i, TextureTarget.Texture2D, G_Textures[i], 0);
            GL.DrawBuffers(G_Textures.Length, ColorAttachments);

            if ((FramebufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt)) != FramebufferErrorCode.FramebufferCompleteExt)
                Log.WriteLineRed("GL.CheckFramebufferStatus: \"FBO_G\" error {0}", FramebufferStatus.ToString());

            //PostProcess FrameBuffer
            FBO_PP = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO_PP);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, Depth_Texture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, PP_Texture, 0);

            if ((FramebufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt)) != FramebufferErrorCode.FramebufferCompleteExt)
                Log.WriteLineRed("GL.CheckFramebufferStatus: \"FBO_PP\" error {0}", FramebufferStatus.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //Bind default FrameBuffer

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(FBO_Vertexes.Length * sizeof(float)), FBO_Vertexes, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        static void Rescale()
        {
            #region Depth
            // Depth
            GL.BindTexture(TextureTarget.Texture2D, Depth_Texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, ScreenWidth, ScreenHeight, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            #endregion

            #region FBO_G
            for (int i = 0; i < G_Textures.Length; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, G_Textures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, ScreenWidth, ScreenHeight, 0,
                    PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            #endregion

            #region FBO_PP
            GL.BindTexture(TextureTarget.Texture2D, PP_Texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, ScreenWidth, ScreenHeight, 0,
                PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            #endregion
        }

        public static void Draw_G()
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO_PP);
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(Shaders[G_ShaderIndex].ProgramID);

            int TempLocation = Shaders[G_ShaderIndex].GetUniform("ScreenSize");
            if (TempLocation != -1)
                GL.Uniform2(TempLocation, (float)ScreenWidth, (float)ScreenWidth);

            TempLocation = Shaders[G_ShaderIndex].GetUniform("ClipPlanes");
            if (TempLocation != -1) //zNear, zFar
                GL.Uniform2(TempLocation, Game.MainCamera.zNear, Game.MainCamera.zFar);

            TempLocation = Shaders[G_ShaderIndex].GetUniform("CameraPos");
            /*if (TempLocation != -1)
            {
                Vector3 CameraPosEye = (new Vector4(Game.MainCamera.Position, 1.0f) * Game.MainCamera.GetViewMatrix()).Xyz;
                GL.Uniform3(TempLocation, ref CameraPosEye);
            }

            TempLocation = Shaders[G_ShaderIndex].GetUniform("InvProjMatrix");
            if (TempLocation != -1)
            {
                Matrix4 InvProjMatrix = Game.MainCamera.GetProjectionMatrix().Inverted();
                GL.UniformMatrix4(TempLocation, false, ref InvProjMatrix);
            }*/

            Vector3[] LightPosition = new Vector3[2];
            LightPosition[0] = new Vector3(5.5f * (float)Math.Cos(Game.Angle), 2.0f + (float)Math.Sin(MathHelper.TwoPi + Game.Angle), 5.5f * (float)Math.Sin(Game.Angle));
            LightPosition[1] = new Vector3(10.5f * (float)Math.Sin(Game.Angle), 2.0f + (float)Math.Cos(MathHelper.TwoPi + Game.Angle), 3.5f * (float)Math.Cos(Game.Angle));
            Game.DebugObjects[0].Position = LightPosition[0];
            Game.DebugObjects[1].Position = LightPosition[1];

            // Передаем шейдеру вектор Light Position (Eye coords), если шейдер поддерживает это.
            TempLocation = Shaders[G_ShaderIndex].GetUniform("Light[0].Ld");
            if (TempLocation != -1)
            {
                for (int LightIndex = 0; LightIndex < 2; LightIndex++)
                {
                    string LightIndexStr = LightIndex.ToString();

                    LightPosition[LightIndex] = (new Vector4(LightPosition[LightIndex], 1.0f) * Game.MainCamera.GetViewMatrix()).Xyz;

                    // Attenuation = LightIntensity / (Constant + Linear * Distance + Quadric * Distance^2)
                    const float k_Constant = 0.0f;
                    const float k_Linear = 0.5f;
                    const float k_Quadric = 0.01f;
                    GL.Uniform3(Shaders[G_ShaderIndex].GetUniform("Light[" + LightIndexStr + "].Att"), k_Constant, k_Linear, k_Quadric);
                    GL.Uniform3(Shaders[G_ShaderIndex].GetUniform("Light[" + LightIndexStr + "].Ld"), new Vector3(0.0f, 0.0f, 1.0f) * 1.0f);
                    GL.Uniform3(Shaders[G_ShaderIndex].GetUniform("Light[" + LightIndexStr + "].Ls"), 1.0f, 0.0f, 0.0f);
                    GL.Uniform3(Shaders[G_ShaderIndex].GetUniform("Light[" + LightIndexStr + "].Pos"), LightPosition[LightIndex]);
                }

                GL.Uniform3(Shaders[G_ShaderIndex].GetUniform("LightAmbient"), 0.0f, 0.0f, 0.0f);
            }

            //Bind Textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Depth_Texture);
            for (int i = 0; i < G_Textures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture1 + i);
                GL.BindTexture(TextureTarget.Texture2D, G_Textures[i]);
            }

            Shaders[G_ShaderIndex].EnableVertexAttribArrays();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(Shaders[G_ShaderIndex].GetAttribute("v_UV"), 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            //Shaders[G_ShaderIndex].DisableVertexAttribArrays();
        }

        public static void Draw_PP()
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); //Bind default FrameBuffer
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(Shaders[PP_ShaderIndex].ProgramID);

            if (UsePostEffects)
            {
                if (Shaders[PP_ShaderIndex].GetUniform("ScreenSize") != -1)
                    GL.Uniform2(GL.GetUniformLocation(Shaders[PP_ShaderIndex].ProgramID, "ScreenSize"), (float)ScreenWidth, (float)ScreenWidth);

                //if (Shaders[PP_ShaderIndex].GetUniform("ClipPlanes") != -1) //zNear, zFar
                //    GL.Uniform2(GL.GetUniformLocation(Shaders[PP_ShaderIndex].ProgramID, "ClipPlanes"), Game.MainCamera.zNear, Game.MainCamera.zFar);

                if (Shaders[PP_ShaderIndex].GetUniform("FXAAEnabled") != -1)
                    GL.Uniform1(GL.GetUniformLocation(Shaders[PP_ShaderIndex].ProgramID, "FXAAEnabled"), Convert.ToInt32(Settings.Graphics.FXAAEnabled));

                if (Shaders[PP_ShaderIndex].GetUniform("SepiaEnabled") != -1)
                    GL.Uniform1(GL.GetUniformLocation(Shaders[PP_ShaderIndex].ProgramID, "SepiaEnabled"), Convert.ToInt32(0));
            }

            //Bind Textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Depth_Texture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, PP_Texture);

            Shaders[PP_ShaderIndex].EnableVertexAttribArrays();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(Shaders[PP_ShaderIndex].GetAttribute("v_UV"), 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            //Shaders[PP_ShaderIndex].DisableVertexAttribArrays();
        }

        public static void Free(bool WithEngineContent = false)
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            for (int i = 0; i < Shaders.Count; i++)
                Shaders[i].Free();

            G_ShaderIndex = 0;
            PP_ShaderIndex = 0;

            for (int i = 0; i < G_Textures.Length; i++)
            {
                GL.DeleteTexture(G_Textures[i]);
                G_Textures[i] = 0;
            }

            GL.DeleteTexture(Depth_Texture);
            GL.DeleteTexture(PP_Texture);
            Depth_Texture = 0;
            PP_Texture = 0;

            GL.DeleteBuffer(VBO);
            GL.DeleteFramebuffer(FBO_G);
            GL.DeleteFramebuffer(FBO_PP);

            FBO_G = 0;
            FBO_PP = 0;
            VBO = 0;

            if (WithEngineContent)
                Shaders.Clear();
        }
    }
}