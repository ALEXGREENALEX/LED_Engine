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
        public static int
            ShaderIndex_P1_Orig,
            ShaderIndex_P1,
            ShaderIndex_PP; // It's not a Shader.ProgramID !!!

        public static int // Debug Shaders Indexes
            DebugShaderIndex_Depth,
            DebugShaderIndex_Diffuse,
            DebugShaderIndex_Light,
            DebugShaderIndex_Normals,
            DebugShaderIndex_AmbientOclusion,
            DebugShaderIndex_Emissive,
            DebugShaderIndex_Specular;

        public static List<Shader> Shaders = new List<Shader>();

        public static int
            FBO_P1, //Pass1 (G-Buffer)
            FBO_PP; //PostProcess
        public static bool UsePostEffects = true;

        static DrawBuffersEnum[] ColorAttachments_P1 = new DrawBuffersEnum[] {
            DrawBuffersEnum.ColorAttachment0, //Diffuse
            DrawBuffersEnum.ColorAttachment1, //Normals.xy, Emissive.xy
            DrawBuffersEnum.ColorAttachment2, //Position, Emissive.z
            DrawBuffersEnum.ColorAttachment3, //Specular, Shininess
            DrawBuffersEnum.ColorAttachment4};//AO, FREE

        //Pass0 OUT textures
        public static int Depth;
        public static int[] Textures_P1 = new int[ColorAttachments_P1.Length];

        //Pass1 OUT Textures
        public static int Texture_PP;

        static int ScreenWidth, ScreenHeight;
        static int VBO;
        static float[] FBO_Vertexes = new float[] {
            -1.0f, -1.0f,
             1.0f, -1.0f,
            -1.0f,  1.0f,
             1.0f,  1.0f }; // Screen vertexes positions

        public static void Init(int ScrWidth, int ScrHeight)
        {
            Free();
            FramebufferErrorCode FramebufferStatus;

            ScreenWidth = ScrWidth;
            ScreenHeight = ScrHeight;

            #region Shaders
            for (int i = 0; i < Shaders.Count; i++)
            {
                Shaders[i].LoadShader();

                switch (Shaders[i].Name)
                {
                    case "Pass1":
                        ShaderIndex_P1 = i;
                        ShaderIndex_P1_Orig = i;
                        break;
                    case "PostProcess":
                        ShaderIndex_PP = i;
                        break;

                    case "Debug_Depth":
                        DebugShaderIndex_Depth = i;
                        break;
                    case "Debug_Diffuse":
                        DebugShaderIndex_Diffuse = i;
                        break;
                    case "Debug_Light":
                        DebugShaderIndex_Light = i;
                        break;
                    case "Debug_Normals":
                        DebugShaderIndex_Normals = i;
                        break;
                    case "Debug_AmbientOclusion":
                        DebugShaderIndex_AmbientOclusion = i;
                        break;
                    case "Debug_Emissive":
                        DebugShaderIndex_Emissive = i;
                        break;
                    case "Debug_Specular":
                        DebugShaderIndex_Specular = i;
                        break;
                }
            }
            #endregion

            #region Gen Textures for Deffered rendering, Post Processesing
            Depth = GL.GenTexture();
            GL.GenTextures(Textures_P1.Length, Textures_P1);
            Texture_PP = GL.GenTexture();

            Rescale();
            #endregion

            #region Generate FBO's
            //Pass1 (G-Buffer)
            FBO_P1 = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO_P1);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, Depth, 0);
            for (int i = 0; i < Textures_P1.Length; i++)
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext + i, TextureTarget.Texture2D, Textures_P1[i], 0);
            GL.DrawBuffers(ColorAttachments_P1.Length, ColorAttachments_P1);

            FramebufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (FramebufferStatus != FramebufferErrorCode.FramebufferCompleteExt)
                Log.WriteLineRed("GL.CheckFramebufferStatus: \"FBO_P1\" error {0}", FramebufferStatus.ToString());

            //PostProcess FrameBuffer
            FBO_PP = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO_PP);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, Depth, 0);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, Texture_PP, 0);

            FramebufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (FramebufferStatus != FramebufferErrorCode.FramebufferCompleteExt)
                Log.WriteLineRed("GL.CheckFramebufferStatus: \"FBO_PP\" error {0}", FramebufferStatus.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //Bind default FrameBuffer
            #endregion

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(FBO_Vertexes.Length * sizeof(float)), FBO_Vertexes, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        static void Rescale()
        {
            #region Depth
            // Depth
            GL.BindTexture(TextureTarget.Texture2D, Depth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, ScreenWidth, ScreenHeight, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            #endregion

            #region Pass1
            for (int i = 0; i < Textures_P1.Length; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, Textures_P1[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, ScreenWidth, ScreenHeight, 0,
                    PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }
            #endregion

            #region Post Process
            GL.BindTexture(TextureTarget.Texture2D, Texture_PP);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, ScreenWidth, ScreenHeight, 0,
                PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            #endregion

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void Draw_P1()
        {
            // Bind PostProcess FBO
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBO_PP);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(Color4.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(Shaders[ShaderIndex_P1].ProgramID);
            int TempLocation;

            //TempLocation = Shaders[ShaderIndex_P1].GetUniform("ScreenSize");
            //if (TempLocation != -1)
            //   GL.Uniform2(TempLocation, (float)ScreenWidth, (float)ScreenHeight);

            TempLocation = Shaders[ShaderIndex_P1].GetUniform("ClipPlanes");
            if (TempLocation != -1) //zNear, zFar
                GL.Uniform2(TempLocation, Game.MainCamera.zNear, Game.MainCamera.zFar);

            TempLocation = Shaders[ShaderIndex_P1].GetUniform("ViewMatrixInv");
            if (TempLocation != -1)
            {
                Matrix4 ViewInv = Game.MainCamera.GetViewMatrix().Inverted();
                GL.UniformMatrix4(TempLocation, false, ref ViewInv);
            }

            TempLocation = Shaders[ShaderIndex_P1].GetUniform("ProjectionMatrix");
            if (TempLocation != -1)
            {
                Matrix4 ProjecionMatrix = Game.MainCamera.GetProjectionMatrix();
                GL.UniformMatrix4(TempLocation, false, ref ProjecionMatrix);
            }

            //TempLocation = Shaders[ShaderIndex_P1].GetUniform("Time");
            //if (TempLocation != -1)
            //{
            //    float Time = (float)Glfw.GetTime();
            //    GL.Uniform1(TempLocation, Time);
            //}

            #region Lights
            TempLocation = Shaders[ShaderIndex_P1].GetUniform("LightA");
            if (TempLocation != -1)
            {
                string IndexStr;
                List<Light> L_Ambient = new List<Light>();
                List<Light> L_Directional = new List<Light>();
                List<Light> L_Point = new List<Light>();
                List<Light> L_Spot = new List<Light>();

                foreach (Light L in Lights.LIGHTS)
                {
                    if (L.Enabled)
                    {
                        switch (L.Type)
                        {
                            case LightType.Ambient:
                                L_Ambient.Add(L);
                                break;
                            case LightType.Directional:
                                L_Directional.Add(L);
                                break;
                            case LightType.Point:
                                L_Point.Add(L);
                                break;
                            case LightType.Spot:
                                L_Spot.Add(L);
                                break;
                        }
                    }
                }

                GL.Uniform1(Shaders[ShaderIndex_P1].GetUniform("LDcount"), L_Directional.Count);
                GL.Uniform1(Shaders[ShaderIndex_P1].GetUniform("LPcount"), L_Point.Count);
                GL.Uniform1(Shaders[ShaderIndex_P1].GetUniform("LScount"), L_Spot.Count);

                // AmbientLight
                Vector3 AmbientColor = new Vector3(0.0f);
                foreach (Light L in L_Ambient)
                    AmbientColor += L.Diffuse;
                GL.Uniform3(TempLocation, AmbientColor);

                // DirectionalLight
                for (int i = 0; i < L_Directional.Count; i++)
                {
                    IndexStr = i.ToString();
                    Vector3 LightDirection = (new Vector4(L_Directional[i].Direction.Normalized(), 0.0f) * Game.MainCamera.GetViewMatrix()).Xyz;
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightD[" + IndexStr + "].Dir"), LightDirection);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightD[" + IndexStr + "].Ld"), L_Directional[i].Diffuse);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightD[" + IndexStr + "].Ls"), L_Directional[i].Specular);
                }

                // PointLight
                for (int i = 0; i < L_Point.Count; i++)
                {
                    IndexStr = i.ToString();
                    Vector3 LightPosition = (new Vector4(L_Point[i].Position, 1.0f) * Game.MainCamera.GetViewMatrix()).Xyz;
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightP[" + IndexStr + "].Pos"), LightPosition);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightP[" + IndexStr + "].Ld"), L_Point[i].Diffuse);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightP[" + IndexStr + "].Ls"), L_Point[i].Specular);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightP[" + IndexStr + "].Att"), L_Point[i].Attenuation);
                }

                // SpotLight
                for (int i = 0; i < L_Spot.Count; i++)
                {
                    IndexStr = i.ToString();

                    Vector3 LightPosition = (new Vector4(L_Spot[i].Position, 1.0f) * Game.MainCamera.GetViewMatrix()).Xyz;
                    Vector3 LightDirection = (new Vector4(L_Spot[i].Direction.Normalized(), 0.0f) * Game.MainCamera.GetViewMatrix()).Xyz;
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Pos"), LightPosition);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Dir"), LightDirection);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Ld"), L_Spot[i].Diffuse);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Ls"), L_Spot[i].Specular);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Att"), L_Spot[i].Attenuation);
                    GL.Uniform1(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Cut"), L_Spot[i].CutOFF);
                    GL.Uniform1(Shaders[ShaderIndex_P1].GetUniform("LightS[" + IndexStr + "].Exp"), L_Spot[i].Exponent);
                }
            }
            #endregion

            #region Fog
            TempLocation = Shaders[ShaderIndex_P1].GetUniform("FogMinMaxDistance");
            if (TempLocation != -1)
            {
                if (Settings.Graphics.Fog.Enabled && Settings.Graphics.Fog.UseFogOnMap)
                {
                    GL.Uniform2(TempLocation, Settings.Graphics.Fog.MinDistance, Settings.Graphics.Fog.MaxDistance);
                    GL.Uniform3(Shaders[ShaderIndex_P1].GetUniform("FogColor"), Settings.Graphics.Fog.Color);
                }
                else
                    GL.Uniform2(TempLocation, -1.0f, 0.0f); // Disable Fog
            }
            #endregion

            //Bind Textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Depth);
            for (int i = 0; i < Textures_P1.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture1 + i);
                GL.BindTexture(TextureTarget.Texture2D, Textures_P1[i]);
            }

            Shaders[ShaderIndex_P1].EnableVertexAttribArrays();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(Shaders[ShaderIndex_P1].GetAttribute("v_UV"), 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            //Shaders[ShaderIndex_P1].DisableVertexAttribArrays();
        }

        public static void Draw_PostPocess()
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); //Bind default FrameBuffer
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(Color4.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(Shaders[ShaderIndex_PP].ProgramID);

            if (UsePostEffects)
            {
                int TempLocation = Shaders[ShaderIndex_PP].GetUniform("ScreenSize");
                if (TempLocation != -1)
                    GL.Uniform2(TempLocation, (float)ScreenWidth, (float)ScreenHeight);

                //TempLocation = Shaders[ShaderIndex_PP].GetUniform("ClipPlanes");
                //if (TempLocation != -1) //zNear, zFar
                //    GL.Uniform2(TempLocation, Game.MainCamera.zNear, Game.MainCamera.zFar);

                TempLocation = Shaders[ShaderIndex_PP].GetUniform("FXAAEnabled");
                if (TempLocation != -1)
                {
                    GL.Uniform1(TempLocation, Convert.ToInt32(Settings.Graphics.FXAA.Enabled));

                    if (Settings.Graphics.FXAA.Enabled)
                        GL.Uniform3(Shaders[ShaderIndex_PP].GetUniform("FXAASettings"),
                            Settings.Graphics.FXAA.Subpix,
                            Settings.Graphics.FXAA.EdgeThreshold,
                            Settings.Graphics.FXAA.EdgeThresholdMin);
                }

                TempLocation = Shaders[ShaderIndex_PP].GetUniform("VignetteSettings");
                if (TempLocation != -1)
                {
                    if (Settings.Graphics.Vignette.Enabled)
                        GL.Uniform3(TempLocation, Settings.Graphics.Vignette.Radius,
                            Settings.Graphics.Vignette.Softness, Settings.Graphics.Vignette.Opacity);
                    else
                        GL.Uniform3(TempLocation, 0.0f, 0.0f, 0.0f); //Disable Vignette
                }

                TempLocation = Shaders[ShaderIndex_PP].GetUniform("SepiaGrayscale");
                if (TempLocation != -1)
                {
                    float Sepia = 0.0f;
                    float GrayScale = 0.0f;

                    if (Settings.Graphics.Sepia.Enabled)
                        Sepia = Settings.Graphics.Sepia.Opacity;

                    if (Settings.Graphics.GrayScale.Enabled)
                        GrayScale = Settings.Graphics.GrayScale.Opacity;

                    GL.Uniform2(Shaders[ShaderIndex_PP].GetUniform("SepiaGrayscale"), Sepia, GrayScale);
                }
            }

            //Bind Textures
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture_PP);

            Shaders[ShaderIndex_PP].EnableVertexAttribArrays();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexAttribPointer(Shaders[ShaderIndex_PP].GetAttribute("v_UV"), 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            //Shaders[PP_ShaderIndex].DisableVertexAttribArrays();
        }

        public static void Free()
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            for (int i = 0; i < Shaders.Count; i++)
                Shaders[i].Free();

            ShaderIndex_P1 = 0;
            ShaderIndex_PP = 0;

            for (int i = 0; i < Textures_P1.Length; i++)
            {
                GL.DeleteTexture(Textures_P1[i]);
                Textures_P1[i] = 0;
            }

            GL.DeleteTexture(Depth);
            GL.DeleteTexture(Texture_PP);
            Depth = 0;
            Texture_PP = 0;

            GL.DeleteBuffer(VBO);
            GL.DeleteFramebuffer(FBO_P1);
            GL.DeleteFramebuffer(FBO_PP);

            VBO = 0;
            FBO_P1 = 0;
            FBO_PP = 0;
        }
    }
}