using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public partial class EditorPropetriesForm : Form
    {
        static int usecounter = 1;

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
            foreach (Mesh element in Meshes.MeshesList)
            {
                MeshComboBox.Items.Add(element.MeshName);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //foreach (Mesh element in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes)
            //{
            NameBox.Text = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Name;
            try
            {
                //string path = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].FileName;
                //ObjFileTextBox.Text = path.Replace(Settings.Paths.Meshes,"");
                //ObjFileTextBox.Text = Settings.Paths.Meshes;
                MeshComboBox.SelectedIndex = MeshComboBox.FindString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].MeshName);
            }
            catch
            {
                //ObjFileTextBox.Clear();
            }

            ////Engine content
            //if (Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].EngineContent == true)
            //    EngineCheckBox1.Checked = true;
            //else
            //    EngineCheckBox1.Checked = false;

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
            RotBoxX.Text = Convert.ToString(MathHelper.RadiansToDegrees(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.X));
            RotBoxY.Text = Convert.ToString(MathHelper.RadiansToDegrees(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.Y));
            RotBoxZ.Text = Convert.ToString(MathHelper.RadiansToDegrees(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.Z));

            //Scale
            SclBoxX.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.X);
            SclBoxY.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.Y);
            SclBoxZ.Text = Convert.ToString(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.Z);
            //}

        }

        private void ModelsComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (Mesh element in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes)
            {
                listBox1.Items.Add(element.Name);
                objcountlabel.Visible = true;
                objcountlabel.Text = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes.Count().ToString() + " object(s)";
            }
        }

        private void PropetriesApplyButton_Click(object sender, EventArgs e)
        {
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].MeshName = MeshComboBox.GetItemText(MeshComboBox.SelectedItem);
            List<string> matlist = new List<string>();
            string tempmat = String.Empty;
            int Count = 0;
            foreach (MeshPart msh in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Parts)
            {
                if (msh.Material.ToString() != tempmat)
                {
                    tempmat = msh.Material.ToString();
                    matlist.Add(msh.Material.Name.ToString());
                    Count++;
                }
            }

            Meshes.Unload(Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex]);
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex] = Meshes.Load(NameBox.Text, MeshComboBox.GetItemText(MeshComboBox.SelectedItem));

            for (int i = 0; i < Count; i++)
                Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Parts[i].Material = Materials.Load(matlist[i]);

            if (Count < Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Parts.Count && Count > 0)
            {
                for (int i = Count; i < Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Parts.Count; i++)
                    Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Parts[i].Material = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Parts[0].Material;
            }

            try
            {
                //Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].FileName = Settings.Paths.Meshes + ObjFileTextBox.Text;
            }
            catch (Exception ex)
            {
                Log.WriteLineRed("Map saving error.");
                Log.WriteLineYellow(ex.Message);
                //ObjFileTextBox.Clear();
            }

            //Engine content
            //if (Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].EngineContent == true)
            //    EngineCheckBox1.Checked = true;
            //else
            //    EngineCheckBox1.Checked = false;

            //Visible
            if (VisibleCheckBox1.Checked == true)
                Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Visible = true;
            else
                Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Visible = false;

            //Position
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Position.X = (float)Convert.ToDouble(XtextBox1.Text);
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Position.Y = (float)Convert.ToDouble(YtextBox2.Text);
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Position.Z = (float)Convert.ToDouble(ZtextBox3.Text);

            //Rotation
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.X = MathHelper.DegreesToRadians((float)Convert.ToDouble(RotBoxX.Text));
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.Y = MathHelper.DegreesToRadians((float)Convert.ToDouble(RotBoxY.Text));
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Rotation.Z = MathHelper.DegreesToRadians((float)Convert.ToDouble(RotBoxZ.Text));

            //Scale
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.X = (float)Convert.ToDouble(SclBoxX.Text);
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.Y = (float)Convert.ToDouble(SclBoxY.Text);
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes[listBox1.SelectedIndex].Scale.Z = (float)Convert.ToDouble(SclBoxZ.Text);

            int temp_index = listBox1.SelectedIndex;
            listBox1.Items.Clear();
            foreach (Mesh element in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes)
            {
                listBox1.Items.Add(element.Name);
            }
            listBox1.SelectedIndex = temp_index;
            objcountlabel.Visible = true;
            objcountlabel.Text = Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes.Count().ToString() + " object(s)";
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes.RemoveAt(listBox1.SelectedIndex);
            int temp_index = listBox1.SelectedIndex;
            listBox1.Items.Clear();
            foreach (Mesh element in Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes)
            {
                listBox1.Items.Add(element.Name);
            }
            listBox1.SelectedIndex = temp_index;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Mesh AddNewMesh = new Mesh();
            AddNewMesh = Meshes.Load("NewObj" + usecounter, "DebugPointLight");
            usecounter++;
            int Count = AddNewMesh.Parts.Count();
            for (int i = 0; i < Count; i++)
                AddNewMesh.Parts[i].Material = Materials.Load("Checker");

            if (Count < AddNewMesh.Parts.Count && Count > 0)
            {
                for (int i = Count; i < AddNewMesh.Parts.Count; i++)
                    AddNewMesh.Parts[i].Material = AddNewMesh.Parts[0].Material;
            }
            Models.MODELS[ModelsComboBox1.SelectedIndex].Meshes.Add(AddNewMesh);
        }
    }
}
