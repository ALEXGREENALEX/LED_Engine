using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LED_Engine
{
    public partial class EditorPropetriesForm : Form
    {
        public EditorPropetriesForm()
        {
            InitializeComponent();
        }

        private void EditorPropetriesForm_Load(object sender, EventArgs e)
        {
            foreach (Model element in Models.MODELS)
            {
                ModelsComboBox1.Items.Add(element.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Mesh element in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes)
            {
                NameTextBox.Text = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].MeshName;
                try
                {
                    string path = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].FileName;
                    ObjFileTextBox.Text = path.Substring(path.LastIndexOf("Meshes"));
                }
                catch
                {
                    ObjFileTextBox.Clear();
                }

                //Engine content
                if (Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].EngineContent == true)
                    EngineCheckBox1.Checked = true;
                else
                    EngineCheckBox1.Checked = false;

                //Visible
                if (Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Visible == true)
                    VisibleCheckBox1.Checked = true;
                else
                    VisibleCheckBox1.Checked = false;

                //Position
                XtextBox1.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Position.X);
                YtextBox2.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Position.Y);
                ZtextBox3.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Position.Z);

                //Rotation
                RotBoxX.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.X);
                RotBoxY.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.Y);
                RotBoxZ.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.Z);

                //Scale
                SclBoxX.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.X);
                SclBoxY.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.Y);
                SclBoxZ.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.Z);
            }
                
        }

        private void ModelsComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (Mesh element in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes)
            {
                listBox1.Items.Add(element.MeshName);
            }
        }
    }
}
