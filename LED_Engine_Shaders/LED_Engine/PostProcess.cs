using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace LED_Engine
{
    static public class PostProcess
    {
        public static bool Initialized = false;
        public static bool UsePostEffects = true;
        public static int ShaderIndex = 0; // It's not a Shader.ProgramID !!!
        public static List<Shader> Shaders = new List<Shader>();
        public static int fbo, fbo_texture, rbo_depth, vbo_fbo_vertices;
        static int ScreenWidth, ScreenHeight;

        public static void Init(int ScrWidth, int ScrHeight)
        {
            if (Initialized)
                Free();

            if (Shaders.Count == 0)
            {
                Free();
                UsePostEffects = false;
                return;
            }
            else
                foreach (var i in Shaders)
                    i.LoadShader();

            ScreenWidth = ScrWidth;
            ScreenHeight = ScrHeight;

            fbo_texture = GL.GenTexture(); // Texture
            rbo_depth = GL.GenRenderbuffer(); // Depth buffer
            _Rescale(); //Rescale Texture, Depth buffer

            fbo = GL.GenFramebuffer(); // Framebuffer to link everything together
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, fbo_texture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo_depth);

            FramebufferErrorCode status;
            if ((status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
                Log.WriteLineRed("GL.CheckFramebufferStatus: error {0}", status.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            float[] fbo_vertexes = new float[] { -1f, -1f, 1f, -1f, -1f,  1f, 1f,  1f }; // Screen vertexes positions

            vbo_fbo_vertices = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_fbo_vertices);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(fbo_vertexes.Length * sizeof(float)), fbo_vertexes, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static void Rescale(int ScrWidth, int ScrHeight)
        {
            if (Initialized)
            {
                ScreenWidth = ScrWidth;
                ScreenHeight = ScrHeight;
                _Rescale();
            }
        }

        static void _Rescale()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, fbo_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ScreenWidth, ScreenHeight, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo_depth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, ScreenWidth, ScreenHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public static void Draw()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(Shaders[ShaderIndex].ProgramID);

            // Передаем параметры шейдерам пост процесса
            if (Shaders[ShaderIndex].GetUniform("ScreenSize") != -1)
                GL.Uniform2(GL.GetUniformLocation(Shaders[ShaderIndex].ProgramID, "ScreenSize"), (float)ScreenWidth, (float)ScreenWidth);

            if (Shaders[ShaderIndex].GetUniform("FXAAEnabled") != -1)
                GL.Uniform1(GL.GetUniformLocation(Shaders[ShaderIndex].ProgramID, "FXAAEnabled"), Convert.ToInt32(Settings.Graphics.FXAAEnabled));

            if (Shaders[ShaderIndex].GetUniform("SepiaEnabled") != -1)
                GL.Uniform1(GL.GetUniformLocation(Shaders[ShaderIndex].ProgramID, "SepiaEnabled"), Convert.ToInt32(0));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, fbo_texture);

            Shaders[ShaderIndex].EnableVertexAttribArrays();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_fbo_vertices);
            GL.VertexAttribPointer(Shaders[ShaderIndex].GetAttribute("v_UV"), 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            //Shaders[ShaderProgramNumber].DisableVertexAttribArrays();
        }

        public static void Free()
        {
            if (Initialized)
            {
                for (int i = 0; i < Shaders.Count; i++)
                    Shaders[i].Free();
                Shaders.Clear();
                ShaderIndex = 0;

                GL.DeleteBuffer(vbo_fbo_vertices);
                GL.DeleteRenderbuffer(rbo_depth);
                GL.DeleteTexture(fbo_texture);
                GL.DeleteFramebuffer(fbo);

                fbo = 0;
                fbo_texture = 0;
                rbo_depth = 0;
                vbo_fbo_vertices = 0;

                Initialized = false;
            }
        }
    }
}