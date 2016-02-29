using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using PixelFormat = Pencil.Gaming.Graphics.PixelFormat;

namespace QuickFont
{
    class TexturePage : IDisposable
    {
        int gLTexID;
        int width;
        int height;

        public int GLTexID { get { return gLTexID; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public TexturePage(string filePath)
        {
            var bitmap = new QBitmap(filePath);
            CreateTexture(bitmap.bitmapData);
            bitmap.Free();
        }

        public TexturePage(BitmapData dataSource)
        {
            CreateTexture(dataSource);
        }

        private void CreateTexture(BitmapData dataSource)
        {
            width = dataSource.Width;
            height = dataSource.Height;

            Helper.SafeGLEnable(EnableCap.Texture2D, () =>
            {
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

                GL.GenTextures(1, out gLTexID);
                GL.BindTexture(TextureTarget.Texture2D, gLTexID);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                var rawData = ConvertBgraToRgba(dataSource);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, rawData);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            });
        }

        private static byte[] ConvertBgraToRgba(BitmapData dataSource)
        {
            var length = dataSource.Stride*dataSource.Height;

            var rawData = new byte[length];

            // Copy bitmap to byte[]
            Marshal.Copy(dataSource.Scan0, rawData, 0, length);

            for (var i = 0; i < rawData.Length; i = i + 4)
            {
                var temp1 = rawData[i];
                rawData[i] = rawData[i + 2];
                rawData[i + 2] = temp1;
            }
            return rawData;
        }

        public void Dispose()
        {
            GL.DeleteTexture(gLTexID);
        }
    }
}
