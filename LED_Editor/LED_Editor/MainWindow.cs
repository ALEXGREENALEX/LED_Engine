using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace LED_Editor
{
    public partial class MainWindow : Form
    {
        bool loaded = false;
        uint Texture;

        public MainWindow()
        {
            SplashForm SplashScreen = new SplashForm();
            SplashScreen.ShowDialog();
            

            InitializeComponent();
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Form1.ActiveForm.Close();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            SetupViewport();
            //glControl1.Invalidate();
            //GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projection);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            //GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            Texture = (uint)LoadTexture(@"D:\KTU\Диплом\game\led_engine\LED_Engine\LED_Engine\GameData\Textures\brick-wall.jpg");
            SetupViewport();
            return;
        }

        static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, Texture);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1, -1);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1, -1);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1, 1);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1, 1);

            GL.End();

            glControl1.SwapBuffers();
        }

        private void materialBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextureEditor TextureEditorForm = new TextureEditor();
            TextureEditorForm.Show();
        }


    }
}
