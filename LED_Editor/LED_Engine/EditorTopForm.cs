using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pencil.Gaming;

namespace LED_Engine
{
    public partial class EditorTopForm : Form
    {
        public EditorTopForm()
        {
            InitializeComponent();
        }

        private void EditorTopForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Glfw.SetWindowShouldClose(Game.Window, true);
        }

        private void materialBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextureEditor TextureEditorForm = new TextureEditor();
            TextureEditorForm.Show();
        }

    }
}
