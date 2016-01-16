using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            glControl1.Invalidate();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            SetupViewport();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) // Play nice
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Color3(Color.Yellow);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(0, 1);
            GL.Vertex2(-1, -1);
            GL.Vertex2(1, -1);
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
