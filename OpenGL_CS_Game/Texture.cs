using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_CS_Game
{
    public class Texture
    {
        TextureTarget textureTarget = TextureTarget.Texture2D;
        int textureID = -1;

        public Texture()
        {
        }

        public Texture(int TextureID)
        {
            textureID = TextureID;
        }

        public Texture(int TextureID, TextureTarget TextureTarget = TextureTarget.Texture2D)
        {
            textureID = TextureID;
            textureTarget = TextureTarget;
        }

        public int TextureID
        {
            get { return textureID; }
            set { textureID = value; }
        }

        public TextureTarget TextureTarget
        {
            get { return textureTarget; }
            set { textureTarget = value; }
        }

        public static int LoadImage(string filename, TextureMagFilter MagFilter = TextureMagFilter.Linear, TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear)
        {
            try
            {
                Bitmap file = new Bitmap(filename);
                return LoadImage(file, MagFilter, MinFilter);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Error: Cannot load image.\n" + e.Message);
                return -1;
            }
        }

        public static int LoadImage(Bitmap image, TextureMagFilter MagFilter = TextureMagFilter.Linear, TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear)
        {
            // Отображаем текстуру по Y
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            //// Allocate storage
            //GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, image.Width, image.Height);
            //// Copy data into storage
            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, image.Width, image.Height,
            //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            //Filter params
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        public static int LoadCubeMap(Bitmap[] CubeMapTextures)
        {
            if (CubeMapTextures.Length == 6)
            {
                // Изменяем, отображаем и вращаем текстуры
                for (int i = 0; i < CubeMapTextures.Length; i++)
                {
                    if (i < 2 || i > 3)
                        CubeMapTextures[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    if (i == 2 || i == 3)
                        CubeMapTextures[i].RotateFlip(RotateFlipType.Rotate270FlipY);
                    if (i == 2)
                        CubeMapTextures[i].RotateFlip(RotateFlipType.Rotate180FlipNone);
                }

                int texID = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.TextureCubeMap, texID);

                TextureTarget[] targets = new TextureTarget[]
                {
                    TextureTarget.TextureCubeMapPositiveX, TextureTarget.TextureCubeMapNegativeX,
                    TextureTarget.TextureCubeMapNegativeY, TextureTarget.TextureCubeMapPositiveY,
                    TextureTarget.TextureCubeMapPositiveZ, TextureTarget.TextureCubeMapNegativeZ
                };

                // Allocate storage
                GL.TexStorage2D(TextureTarget2d.TextureCubeMap, 1, SizedInternalFormat.Rgba8,
                    CubeMapTextures[0].Width, CubeMapTextures[0].Height);

                BitmapData data;
                // Загружаем все текстуры граней
                for (int i = 0; i < CubeMapTextures.Length; i++)
                {
                    data = CubeMapTextures[i].LockBits(new System.Drawing.Rectangle(0, 0, CubeMapTextures[i].Width,
                        CubeMapTextures[i].Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexSubImage2D(targets[5 - i], 0, 0, 0, data.Width, data.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    CubeMapTextures[i].UnlockBits(data);
                }

                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

                //GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                return texID;
            }
            else
                return -1;
        }
    }
}
