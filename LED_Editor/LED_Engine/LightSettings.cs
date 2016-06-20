using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
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
            //if (e.KeyCode == Keys.Escape)
            //    this.Close();
        }

        public void ListsRefresh()
        {
            listBox_Lights.Items.Clear();
            foreach (var l in Lights.LIGHTS)
                listBox_Lights.Items.Add(l.Name);

            comboBox_Type.Items.Clear();
            comboBox_Type.Items.AddRange(Enum.GetNames(typeof(LightType)));

            if (listBox_Lights.Items.Count != 0 && listBox_Lights.Items.Count > LIndex)
                listBox_Lights.SelectedIndex = LIndex;
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
            Enable_Disable_TextBoxes();

            textBox_Name.Text = Lights.LIGHTS[LIndex].Name;

            textBox_PosX.Text = Lights.LIGHTS[LIndex].Position.X.ToString();
            textBox_PosY.Text = Lights.LIGHTS[LIndex].Position.Y.ToString();
            textBox_PosZ.Text = Lights.LIGHTS[LIndex].Position.Z.ToString();

            Vector2 DirectionYawPitch = Lights.LIGHTS[LIndex].Direction.ExtractYawPitch();
            textBox_DirYaw.Text = MathHelper.RadiansToDegrees(DirectionYawPitch.X).ToString();
            textBox_DirPitch.Text = MathHelper.RadiansToDegrees(DirectionYawPitch.Y).ToString();

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

        void Enable_Disable_TextBoxes()
        {
            //Enable All
            textBox_PosX.Enabled = true;
            textBox_PosY.Enabled = true;
            textBox_PosZ.Enabled = true;
            textBox_DirYaw.Enabled = true;
            textBox_DirPitch.Enabled = true;
            textBox_SpecR.Enabled = true;
            textBox_SpecG.Enabled = true;
            textBox_SpecB.Enabled = true;
            textBox_AttC.Enabled = true;
            textBox_AttL.Enabled = true;
            textBox_AttQ.Enabled = true;
            textBox_CutOFF.Enabled = true;
            textBox_Exp.Enabled = true;

            switch (Lights.LIGHTS[LIndex].Type)
            {
                case LightType.Ambient:
                    textBox_PosX.Enabled = false;
                    textBox_PosY.Enabled = false;
                    textBox_PosZ.Enabled = false;
                    textBox_DirYaw.Enabled = false;
                    textBox_DirPitch.Enabled = false;
                    textBox_SpecR.Enabled = false;
                    textBox_SpecG.Enabled = false;
                    textBox_SpecB.Enabled = false;
                    textBox_AttC.Enabled = false;
                    textBox_AttL.Enabled = false;
                    textBox_AttQ.Enabled = false;
                    textBox_CutOFF.Enabled = false;
                    textBox_Exp.Enabled = false;
                    break;
                case LightType.Directional:
                    textBox_PosX.Enabled = false;
                    textBox_PosY.Enabled = false;
                    textBox_PosZ.Enabled = false;
                    textBox_AttC.Enabled = false;
                    textBox_AttL.Enabled = false;
                    textBox_AttQ.Enabled = false;
                    textBox_CutOFF.Enabled = false;
                    textBox_Exp.Enabled = false;
                    break;
                case LightType.Point:
                    textBox_DirYaw.Enabled = false;
                    textBox_DirPitch.Enabled = false;
                    textBox_CutOFF.Enabled = false;
                    textBox_Exp.Enabled = false;
                    break;
                case LightType.Spot:
                    break;
            }
        }

        private void SomeChanged(object sender, EventArgs e)
        {
            if (LIndex != -1 && Lights.LIGHTS.Count > LIndex && !ChangeFreeze)
            {
                try
                {
                    Lights.LIGHTS[LIndex].Enabled = checkBox_LEnabled.Checked;
                    Lights.LIGHTS[LIndex].Type = (LightType)Enum.Parse(typeof(LightType), comboBox_Type.Text, true);
                    Enable_Disable_TextBoxes();
                    Lights.LIGHTS[LIndex].Name = textBox_Name.Text;

                    Lights.LIGHTS[LIndex].Position = new Vector3(
                        float.Parse(textBox_PosX.Text),
                        float.Parse(textBox_PosY.Text),
                        float.Parse(textBox_PosZ.Text));

                    Lights.LIGHTS[LIndex].Direction = Vector3.FromYawPitch(
                        MathHelper.DegreesToRadians(float.Parse(textBox_DirYaw.Text)),
                        MathHelper.DegreesToRadians(float.Parse(textBox_DirPitch.Text)));

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

        private void button_Add_Click(object sender, EventArgs e)
        {
            Light L = new Light();
            L.Name = "Light_" + DateTime.Now.ToLongTimeString();

            if (LIndex == -1)
                LIndex = Lights.LIGHTS.Count - 1;

            LIndex++;

            Lights.LIGHTS.Insert(LIndex, L);
            ListsRefresh();
        }

        private void button_Del_Click(object sender, EventArgs e)
        {
            if (LIndex != -1)
            {
                Lights.LIGHTS.RemoveAt(LIndex);
                LIndex--;
                if (LIndex == -1 && Lights.LIGHTS.Count > 0)
                    LIndex++;
                ListsRefresh();
            }
        }

        private void button_Serialize_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SerializeToXml<List<Light>>(Lights.LIGHTS));
            MessageBox.Show("Lights was Serialized to Clipboard");
        }

        private void button_Deserialize_Click(object sender, EventArgs e)
        {
            try
            {
                Lights.LIGHTS = DeserializeFromXml<List<Light>>(Clipboard.GetText());
                ListsRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string SerializeToXml<T>(T Obj)
        {
            StringBuilder SB = new StringBuilder();
            XmlWriter XmlWr = XmlWriter.Create(SB);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            ser.Serialize(XmlWr, Obj);
            return SB.ToString();
        }

        public static T DeserializeFromXml<T>(string XmlStr)
        {
            T result;
            var ser = new XmlSerializer(typeof(T));
            using (var tr = new StringReader(XmlStr))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

        private void button_MoveLUp_Click(object sender, EventArgs e)
        {
            if (LIndex > 0)
            {
                Light L = Lights.LIGHTS[LIndex];
                Lights.LIGHTS[LIndex] = Lights.LIGHTS[LIndex - 1];
                Lights.LIGHTS[LIndex - 1] = L;
                LIndex--;
                ListsRefresh();
                listBox_Lights.SelectedIndex = LIndex;
            }
        }

        private void button_MoveLDown_Click(object sender, EventArgs e)
        {
            if (LIndex < listBox_Lights.Items.Count - 1)
            {
                Light L = Lights.LIGHTS[LIndex];
                Lights.LIGHTS[LIndex] = Lights.LIGHTS[LIndex + 1];
                Lights.LIGHTS[LIndex + 1] = L;
                LIndex++;
                ListsRefresh();
                listBox_Lights.SelectedIndex = LIndex;
            }
        }
    }
}
