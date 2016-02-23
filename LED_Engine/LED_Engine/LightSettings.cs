using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;

namespace LED_Engine
{
    public partial class LightSettings : Form
    {
        int LIndex = -1;
        bool ChangeFreeze = false;

        public LightSettings()
        {
            InitializeComponent();
            ListsRefresh();
        }

        private void LightSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        void ListsRefresh()
        {
            listBox_Lights.Items.Clear();
            foreach (var l in Lights.LIGHTS)
                listBox_Lights.Items.Add(l.Name);

            comboBox_Type.Items.Clear();
            comboBox_Type.Items.AddRange(Enum.GetNames(typeof(LightType)));
        }

        private void checkedListBox_Lights_Click(object sender, EventArgs e)
        {
            LIndex = listBox_Lights.SelectedIndex;
            if (LIndex != -1)
            {
                ChangeFreeze = true;
                GetValues();
                ChangeFreeze = false;
            }
        }

        void GetValues()
        {
            checkBox_LEnabled.Checked = Lights.LIGHTS[LIndex].Enabled;
            comboBox_Type.Text = Lights.LIGHTS[LIndex].Type.ToString();
            textBox_Name.Text = Lights.LIGHTS[LIndex].Name;
            textBox_PosX.Text = Lights.LIGHTS[LIndex].Position.X.ToString();
            textBox_PosY.Text = Lights.LIGHTS[LIndex].Position.Y.ToString();
            textBox_PosZ.Text = Lights.LIGHTS[LIndex].Position.Z.ToString();
            textBox_DirX.Text = MathHelper.RadiansToDegrees(Lights.LIGHTS[LIndex].Direction.X).ToString();
            textBox_DirY.Text = MathHelper.RadiansToDegrees(Lights.LIGHTS[LIndex].Direction.Y).ToString();
            textBox_DirZ.Text = MathHelper.RadiansToDegrees(Lights.LIGHTS[LIndex].Direction.Z).ToString();
            textBox_DiffR.Text = Lights.LIGHTS[LIndex].Diffuse.X.ToString();
            textBox_DiffG.Text = Lights.LIGHTS[LIndex].Diffuse.Y.ToString();
            textBox_DiffB.Text = Lights.LIGHTS[LIndex].Diffuse.Z.ToString();
            textBox_SpecR.Text = Lights.LIGHTS[LIndex].Specular.X.ToString();
            textBox_SpecG.Text = Lights.LIGHTS[LIndex].Specular.Y.ToString();
            textBox_SpecB.Text = Lights.LIGHTS[LIndex].Specular.Z.ToString();
            textBox_AttC.Text = Lights.LIGHTS[LIndex].Attenuation.X.ToString();
            textBox_AttL.Text = Lights.LIGHTS[LIndex].Attenuation.Y.ToString();
            textBox_AttQ.Text = Lights.LIGHTS[LIndex].Attenuation.Z.ToString();
            textBox_CutOFF.Text = Lights.LIGHTS[LIndex].CutOFF.ToString();
            textBox_Exp.Text = Lights.LIGHTS[LIndex].Exponent.ToString();
        }

        private void SomeChanged(object sender, EventArgs e)
        {
            if (LIndex != -1 && Lights.LIGHTS.Count > LIndex && !ChangeFreeze)
            {
                try
                {
                    Lights.LIGHTS[LIndex].Enabled = checkBox_LEnabled.Checked;
                    Lights.LIGHTS[LIndex].Type = (LightType)Enum.Parse(typeof(LightType), comboBox_Type.Text, true);
                    Lights.LIGHTS[LIndex].Name = textBox_Name.Text;

                    Lights.LIGHTS[LIndex].Position = new Vector3(
                        float.Parse(textBox_PosX.Text),
                        float.Parse(textBox_PosY.Text),
                        float.Parse(textBox_PosZ.Text));

                    Lights.LIGHTS[LIndex].Direction = new Vector3(
                        MathHelper.DegreesToRadians(float.Parse(textBox_DirX.Text)),
                        MathHelper.DegreesToRadians(float.Parse(textBox_DirY.Text)),
                        MathHelper.DegreesToRadians(float.Parse(textBox_DirZ.Text)));

                    Lights.LIGHTS[LIndex].Diffuse = new Vector3(
                        float.Parse(textBox_DiffR.Text),
                        float.Parse(textBox_DiffG.Text),
                        float.Parse(textBox_DiffB.Text));

                    Lights.LIGHTS[LIndex].Specular = new Vector3(
                        float.Parse(textBox_SpecR.Text),
                        float.Parse(textBox_SpecG.Text),
                        float.Parse(textBox_SpecB.Text));

                    Lights.LIGHTS[LIndex].Attenuation = new Vector3(
                        float.Parse(textBox_AttC.Text),
                        float.Parse(textBox_AttL.Text),
                        float.Parse(textBox_AttQ.Text));

                    Lights.LIGHTS[LIndex].CutOFF = float.Parse(textBox_CutOFF.Text);
                    Lights.LIGHTS[LIndex].Exponent = float.Parse(textBox_Exp.Text);

                    if (listBox_Lights.Items[LIndex].ToString() != textBox_Name.Text)
                        listBox_Lights.Items[LIndex] = textBox_Name.Text;
                }
                catch { }
            }
        }

        private void button_Del_Click(object sender, EventArgs e)
        {
            if (LIndex != -1)
            {
                Lights.LIGHTS.RemoveAt(LIndex);
                listBox_Lights.Items.RemoveAt(LIndex);
                LIndex = -1;
            }
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            Light L = new Light();
            L.Name = "Light_" + DateTime.Now.ToLongTimeString();
            Lights.LIGHTS.Add(L);
            listBox_Lights.Items.Add(L.Name);
        }
    }
}
