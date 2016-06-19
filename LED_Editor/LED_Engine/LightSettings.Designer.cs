namespace LED_Engine
{
    partial class LightSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LightSettings));
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Del = new System.Windows.Forms.Button();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.textBox_PosX = new System.Windows.Forms.TextBox();
            this.textBox_PosY = new System.Windows.Forms.TextBox();
            this.textBox_PosZ = new System.Windows.Forms.TextBox();
            this.textBox_DirYaw = new System.Windows.Forms.TextBox();
            this.textBox_DirPitch = new System.Windows.Forms.TextBox();
            this.comboBox_Type = new System.Windows.Forms.ComboBox();
            this.textBox_DiffB = new System.Windows.Forms.TextBox();
            this.textBox_DiffG = new System.Windows.Forms.TextBox();
            this.textBox_DiffR = new System.Windows.Forms.TextBox();
            this.textBox_SpecB = new System.Windows.Forms.TextBox();
            this.textBox_SpecG = new System.Windows.Forms.TextBox();
            this.textBox_SpecR = new System.Windows.Forms.TextBox();
            this.textBox_AttQ = new System.Windows.Forms.TextBox();
            this.textBox_AttL = new System.Windows.Forms.TextBox();
            this.textBox_AttC = new System.Windows.Forms.TextBox();
            this.textBox_CutOFF = new System.Windows.Forms.TextBox();
            this.textBox_Exp = new System.Windows.Forms.TextBox();
            this.listBox_Lights = new System.Windows.Forms.ListBox();
            this.checkBox_LEnabled = new System.Windows.Forms.CheckBox();
            this.button_Serialize = new System.Windows.Forms.Button();
            this.button_Deserialize = new System.Windows.Forms.Button();
            this.button_MoveLDown = new System.Windows.Forms.Button();
            this.button_MoveLUp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(12, 165);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(20, 20);
            this.button_Add.TabIndex = 1;
            this.button_Add.Text = "+";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_Del
            // 
            this.button_Del.Location = new System.Drawing.Point(38, 166);
            this.button_Del.Name = "button_Del";
            this.button_Del.Size = new System.Drawing.Size(20, 20);
            this.button_Del.TabIndex = 2;
            this.button_Del.Text = "-";
            this.button_Del.UseVisualStyleBackColor = true;
            this.button_Del.Click += new System.EventHandler(this.button_Del_Click);
            // 
            // textBox_Name
            // 
            this.textBox_Name.Location = new System.Drawing.Point(238, 165);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(286, 20);
            this.textBox_Name.TabIndex = 4;
            this.textBox_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_Name.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_PosX
            // 
            this.textBox_PosX.Location = new System.Drawing.Point(238, 12);
            this.textBox_PosX.Name = "textBox_PosX";
            this.textBox_PosX.Size = new System.Drawing.Size(140, 20);
            this.textBox_PosX.TabIndex = 5;
            this.textBox_PosX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_PosX.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_PosY
            // 
            this.textBox_PosY.Location = new System.Drawing.Point(384, 12);
            this.textBox_PosY.Name = "textBox_PosY";
            this.textBox_PosY.Size = new System.Drawing.Size(140, 20);
            this.textBox_PosY.TabIndex = 6;
            this.textBox_PosY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_PosY.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_PosZ
            // 
            this.textBox_PosZ.Location = new System.Drawing.Point(530, 12);
            this.textBox_PosZ.Name = "textBox_PosZ";
            this.textBox_PosZ.Size = new System.Drawing.Size(140, 20);
            this.textBox_PosZ.TabIndex = 7;
            this.textBox_PosZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_PosZ.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_DirYaw
            // 
            this.textBox_DirYaw.Location = new System.Drawing.Point(238, 38);
            this.textBox_DirYaw.Name = "textBox_DirYaw";
            this.textBox_DirYaw.Size = new System.Drawing.Size(140, 20);
            this.textBox_DirYaw.TabIndex = 8;
            this.textBox_DirYaw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DirYaw.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_DirPitch
            // 
            this.textBox_DirPitch.Location = new System.Drawing.Point(384, 38);
            this.textBox_DirPitch.Name = "textBox_DirPitch";
            this.textBox_DirPitch.Size = new System.Drawing.Size(140, 20);
            this.textBox_DirPitch.TabIndex = 9;
            this.textBox_DirPitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DirPitch.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // comboBox_Type
            // 
            this.comboBox_Type.FormattingEnabled = true;
            this.comboBox_Type.Location = new System.Drawing.Point(64, 166);
            this.comboBox_Type.Name = "comboBox_Type";
            this.comboBox_Type.Size = new System.Drawing.Size(116, 21);
            this.comboBox_Type.TabIndex = 11;
            this.comboBox_Type.SelectedIndexChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_DiffB
            // 
            this.textBox_DiffB.Location = new System.Drawing.Point(530, 64);
            this.textBox_DiffB.Name = "textBox_DiffB";
            this.textBox_DiffB.Size = new System.Drawing.Size(140, 20);
            this.textBox_DiffB.TabIndex = 16;
            this.textBox_DiffB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DiffB.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_DiffG
            // 
            this.textBox_DiffG.Location = new System.Drawing.Point(384, 64);
            this.textBox_DiffG.Name = "textBox_DiffG";
            this.textBox_DiffG.Size = new System.Drawing.Size(140, 20);
            this.textBox_DiffG.TabIndex = 15;
            this.textBox_DiffG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DiffG.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_DiffR
            // 
            this.textBox_DiffR.Location = new System.Drawing.Point(238, 64);
            this.textBox_DiffR.Name = "textBox_DiffR";
            this.textBox_DiffR.Size = new System.Drawing.Size(140, 20);
            this.textBox_DiffR.TabIndex = 14;
            this.textBox_DiffR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DiffR.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_SpecB
            // 
            this.textBox_SpecB.Location = new System.Drawing.Point(530, 90);
            this.textBox_SpecB.Name = "textBox_SpecB";
            this.textBox_SpecB.Size = new System.Drawing.Size(140, 20);
            this.textBox_SpecB.TabIndex = 19;
            this.textBox_SpecB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_SpecB.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_SpecG
            // 
            this.textBox_SpecG.Location = new System.Drawing.Point(384, 90);
            this.textBox_SpecG.Name = "textBox_SpecG";
            this.textBox_SpecG.Size = new System.Drawing.Size(140, 20);
            this.textBox_SpecG.TabIndex = 18;
            this.textBox_SpecG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_SpecG.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_SpecR
            // 
            this.textBox_SpecR.Location = new System.Drawing.Point(238, 90);
            this.textBox_SpecR.Name = "textBox_SpecR";
            this.textBox_SpecR.Size = new System.Drawing.Size(140, 20);
            this.textBox_SpecR.TabIndex = 17;
            this.textBox_SpecR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_SpecR.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_AttQ
            // 
            this.textBox_AttQ.Location = new System.Drawing.Point(530, 116);
            this.textBox_AttQ.Name = "textBox_AttQ";
            this.textBox_AttQ.Size = new System.Drawing.Size(140, 20);
            this.textBox_AttQ.TabIndex = 22;
            this.textBox_AttQ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_AttQ.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_AttL
            // 
            this.textBox_AttL.Location = new System.Drawing.Point(384, 116);
            this.textBox_AttL.Name = "textBox_AttL";
            this.textBox_AttL.Size = new System.Drawing.Size(140, 20);
            this.textBox_AttL.TabIndex = 21;
            this.textBox_AttL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_AttL.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_AttC
            // 
            this.textBox_AttC.Location = new System.Drawing.Point(238, 116);
            this.textBox_AttC.Name = "textBox_AttC";
            this.textBox_AttC.Size = new System.Drawing.Size(140, 20);
            this.textBox_AttC.TabIndex = 20;
            this.textBox_AttC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_AttC.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_CutOFF
            // 
            this.textBox_CutOFF.Location = new System.Drawing.Point(238, 142);
            this.textBox_CutOFF.Name = "textBox_CutOFF";
            this.textBox_CutOFF.Size = new System.Drawing.Size(140, 20);
            this.textBox_CutOFF.TabIndex = 23;
            this.textBox_CutOFF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_CutOFF.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // textBox_Exp
            // 
            this.textBox_Exp.Location = new System.Drawing.Point(384, 142);
            this.textBox_Exp.Name = "textBox_Exp";
            this.textBox_Exp.Size = new System.Drawing.Size(140, 20);
            this.textBox_Exp.TabIndex = 24;
            this.textBox_Exp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_Exp.TextChanged += new System.EventHandler(this.SomeChanged);
            // 
            // listBox_Lights
            // 
            this.listBox_Lights.FormattingEnabled = true;
            this.listBox_Lights.Location = new System.Drawing.Point(12, 12);
            this.listBox_Lights.Name = "listBox_Lights";
            this.listBox_Lights.Size = new System.Drawing.Size(220, 147);
            this.listBox_Lights.TabIndex = 25;
            this.listBox_Lights.Click += new System.EventHandler(this.checkedListBox_Lights_Click);
            this.listBox_Lights.SelectedIndexChanged += new System.EventHandler(this.checkedListBox_Lights_Click);
            this.listBox_Lights.DoubleClick += new System.EventHandler(this.checkedListBox_Lights_Click);
            // 
            // checkBox_LEnabled
            // 
            this.checkBox_LEnabled.AutoSize = true;
            this.checkBox_LEnabled.Location = new System.Drawing.Point(530, 142);
            this.checkBox_LEnabled.Name = "checkBox_LEnabled";
            this.checkBox_LEnabled.Size = new System.Drawing.Size(65, 17);
            this.checkBox_LEnabled.TabIndex = 26;
            this.checkBox_LEnabled.Text = "Enabled";
            this.checkBox_LEnabled.UseVisualStyleBackColor = true;
            this.checkBox_LEnabled.CheckedChanged += new System.EventHandler(this.SomeChanged);
            // 
            // button_Serialize
            // 
            this.button_Serialize.Location = new System.Drawing.Point(530, 165);
            this.button_Serialize.Name = "button_Serialize";
            this.button_Serialize.Size = new System.Drawing.Size(65, 22);
            this.button_Serialize.TabIndex = 27;
            this.button_Serialize.Text = "Serialize";
            this.button_Serialize.UseVisualStyleBackColor = true;
            this.button_Serialize.Click += new System.EventHandler(this.button_Serialize_Click);
            // 
            // button_Deserialize
            // 
            this.button_Deserialize.Location = new System.Drawing.Point(601, 165);
            this.button_Deserialize.Name = "button_Deserialize";
            this.button_Deserialize.Size = new System.Drawing.Size(69, 22);
            this.button_Deserialize.TabIndex = 28;
            this.button_Deserialize.Text = "Deserialize";
            this.button_Deserialize.UseVisualStyleBackColor = true;
            this.button_Deserialize.Click += new System.EventHandler(this.button_Deserialize_Click);
            // 
            // button_MoveLDown
            // 
            this.button_MoveLDown.Location = new System.Drawing.Point(212, 165);
            this.button_MoveLDown.Name = "button_MoveLDown";
            this.button_MoveLDown.Size = new System.Drawing.Size(20, 20);
            this.button_MoveLDown.TabIndex = 29;
            this.button_MoveLDown.Text = "↓";
            this.button_MoveLDown.UseVisualStyleBackColor = true;
            this.button_MoveLDown.Click += new System.EventHandler(this.button_MoveLDown_Click);
            // 
            // button_MoveLUp
            // 
            this.button_MoveLUp.Location = new System.Drawing.Point(186, 165);
            this.button_MoveLUp.Name = "button_MoveLUp";
            this.button_MoveLUp.Size = new System.Drawing.Size(20, 20);
            this.button_MoveLUp.TabIndex = 30;
            this.button_MoveLUp.Text = "↑";
            this.button_MoveLUp.UseVisualStyleBackColor = true;
            this.button_MoveLUp.Click += new System.EventHandler(this.button_MoveLUp_Click);
            // 
            // LightSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 193);
            this.ControlBox = false;
            this.Controls.Add(this.button_MoveLUp);
            this.Controls.Add(this.button_MoveLDown);
            this.Controls.Add(this.button_Deserialize);
            this.Controls.Add(this.button_Serialize);
            this.Controls.Add(this.checkBox_LEnabled);
            this.Controls.Add(this.listBox_Lights);
            this.Controls.Add(this.textBox_Exp);
            this.Controls.Add(this.textBox_CutOFF);
            this.Controls.Add(this.textBox_AttQ);
            this.Controls.Add(this.textBox_AttL);
            this.Controls.Add(this.textBox_AttC);
            this.Controls.Add(this.textBox_SpecB);
            this.Controls.Add(this.textBox_SpecG);
            this.Controls.Add(this.textBox_SpecR);
            this.Controls.Add(this.textBox_DiffB);
            this.Controls.Add(this.textBox_DiffG);
            this.Controls.Add(this.textBox_DiffR);
            this.Controls.Add(this.comboBox_Type);
            this.Controls.Add(this.textBox_DirPitch);
            this.Controls.Add(this.textBox_DirYaw);
            this.Controls.Add(this.textBox_PosZ);
            this.Controls.Add(this.textBox_PosY);
            this.Controls.Add(this.textBox_PosX);
            this.Controls.Add(this.textBox_Name);
            this.Controls.Add(this.button_Del);
            this.Controls.Add(this.button_Add);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "LightSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Light Control";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LightSettings_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_Del;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.TextBox textBox_PosX;
        private System.Windows.Forms.TextBox textBox_PosY;
        private System.Windows.Forms.TextBox textBox_PosZ;
        private System.Windows.Forms.TextBox textBox_DirYaw;
        private System.Windows.Forms.TextBox textBox_DirPitch;
        private System.Windows.Forms.ComboBox comboBox_Type;
        private System.Windows.Forms.TextBox textBox_DiffB;
        private System.Windows.Forms.TextBox textBox_DiffG;
        private System.Windows.Forms.TextBox textBox_DiffR;
        private System.Windows.Forms.TextBox textBox_SpecB;
        private System.Windows.Forms.TextBox textBox_SpecG;
        private System.Windows.Forms.TextBox textBox_SpecR;
        private System.Windows.Forms.TextBox textBox_AttQ;
        private System.Windows.Forms.TextBox textBox_AttL;
        private System.Windows.Forms.TextBox textBox_AttC;
        private System.Windows.Forms.TextBox textBox_CutOFF;
        private System.Windows.Forms.TextBox textBox_Exp;
        private System.Windows.Forms.ListBox listBox_Lights;
        private System.Windows.Forms.CheckBox checkBox_LEnabled;
        private System.Windows.Forms.Button button_Serialize;
        private System.Windows.Forms.Button button_Deserialize;
        private System.Windows.Forms.Button button_MoveLDown;
        private System.Windows.Forms.Button button_MoveLUp;
    }
}