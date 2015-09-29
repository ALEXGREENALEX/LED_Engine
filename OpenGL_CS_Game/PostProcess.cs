using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    static public class PostProcess
    {
        public static uint fbo, fbo_texture, rbo_depth, vbo_fbo_vertices;
        static string shaderName;

        public static void Init(string ShaderName, int ScrWidth, int ScrHeight)
        {
            shaderName = ShaderName;

            /* Texture */
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out PostProcess.fbo_texture);
            GL.BindTexture(TextureTarget.Texture2D, fbo_texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ScrWidth, ScrHeight, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            /* Depth buffer */
            GL.GenRenderbuffers(1, out rbo_depth);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo_depth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, ScrWidth, ScrHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            /* Framebuffer to link everything together */
            GL.GenFramebuffers(1, out fbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, fbo_texture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo_depth);

            FramebufferErrorCode status;
            if ((status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)) != FramebufferErrorCode.FramebufferComplete)
                MessageBox.Show("GL.CheckFramebufferStatus: error " + status.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            float[] fbo_vertices = new float[] {
                -1f, -1f,
                 1f, -1f,
                -1f,  1f,
                 1f,  1f
            };

            GL.GenBuffers(1, out vbo_fbo_vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_fbo_vertices);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(fbo_vertices.Length * sizeof(float)), fbo_vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static string ShaderName
        {
            get { return shaderName; }
            set { shaderName = value; }
        }

        public static void Draw()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(Game.shaders[shaderName].ProgramID);
            GL.BindTexture(TextureTarget.Texture2D, fbo_texture);
            GL.Uniform1(Game.shaders[shaderName].GetAttribute("fbo_texture"), 0);
            GL.EnableVertexAttribArray(Game.shaders[shaderName].GetAttribute("v_coord"));

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_fbo_vertices);
            GL.VertexAttribPointer(Game.shaders[shaderName].GetAttribute("v_coord"), 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.DisableVertexAttribArray(Game.shaders[shaderName].GetAttribute("v_coord"));
        }

        public static void Rescale(int ScrWidth, int ScrHeight)
        {
            // Rescale FBO and RBO as well
            GL.BindTexture(TextureTarget.Texture2D, fbo_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ScrWidth, ScrHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo_depth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, ScrWidth, ScrHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public static void FreeResources()
        {
            GL.DeleteBuffers(1, ref vbo_fbo_vertices);
            GL.DeleteRenderbuffers(1, ref rbo_depth);
            GL.DeleteTextures(1, ref fbo_texture);
            GL.DeleteFramebuffers(1,  ref fbo);
        }
    }
}