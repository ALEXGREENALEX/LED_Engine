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
    }
}
