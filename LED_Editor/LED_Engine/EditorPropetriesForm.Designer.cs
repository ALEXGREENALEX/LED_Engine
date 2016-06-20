namespace LED_Engine
{
    partial class EditorPropetriesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorPropetriesForm));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.ModelsComboBox1 = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.objcountlabel = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.ObjToolsPanel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.ObjMaterialComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.MaterialsComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.MeshComboBox = new System.Windows.Forms.ComboBox();
            this.VisibleCheckBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SclBoxZ = new System.Windows.Forms.TextBox();
            this.SclBoxY = new System.Windows.Forms.TextBox();
            this.SclBoxX = new System.Windows.Forms.TextBox();
            this.PropetriesApplyButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.RotBoxZ = new System.Windows.Forms.TextBox();
            this.RotBoxY = new System.Windows.Forms.TextBox();
            this.RotBoxX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ZtextBox3 = new System.Windows.Forms.TextBox();
            this.YtextBox2 = new System.Windows.Forms.TextBox();
            this.XtextBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.ObjToolsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(284, 556);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Models";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ObjToolsPanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(278, 537);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.ModelsComboBox1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(272, 262);
            this.panel1.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 21);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(272, 210);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // ModelsComboBox1
            // 
            this.ModelsComboBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ModelsComboBox1.FormattingEnabled = true;
            this.ModelsComboBox1.Location = new System.Drawing.Point(0, 0);
            this.ModelsComboBox1.Name = "ModelsComboBox1";
            this.ModelsComboBox1.Size = new System.Drawing.Size(272, 21);
            this.ModelsComboBox1.TabIndex = 6;
            this.ModelsComboBox1.SelectedIndexChanged += new System.EventHandler(this.ModelsComboBox1_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.objcountlabel);
            this.panel2.Controls.Add(this.AddButton);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.RemoveButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 231);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(272, 31);
            this.panel2.TabIndex = 5;
            // 
            // objcountlabel
            // 
            this.objcountlabel.AutoSize = true;
            this.objcountlabel.Location = new System.Drawing.Point(205, 10);
            this.objcountlabel.Name = "objcountlabel";
            this.objcountlabel.Size = new System.Drawing.Size(23, 13);
            this.objcountlabel.TabIndex = 34;
            this.objcountlabel.Text = "null";
            this.objcountlabel.Visible = false;
            // 
            // AddButton
            // 
            this.AddButton.Image = ((System.Drawing.Image)(resources.GetObject("AddButton.Image")));
            this.AddButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AddButton.Location = new System.Drawing.Point(3, 4);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(51, 24);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = "Add";
            this.AddButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.Location = new System.Drawing.Point(172, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(27, 24);
            this.button4.TabIndex = 4;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(139, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(27, 24);
            this.button3.TabIndex = 3;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // RemoveButton
            // 
            this.RemoveButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveButton.Image")));
            this.RemoveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.RemoveButton.Location = new System.Drawing.Point(60, 4);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(73, 24);
            this.RemoveButton.TabIndex = 2;
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // ObjToolsPanel
            // 
            this.ObjToolsPanel.Controls.Add(this.label7);
            this.ObjToolsPanel.Controls.Add(this.ObjMaterialComboBox);
            this.ObjToolsPanel.Controls.Add(this.label6);
            this.ObjToolsPanel.Controls.Add(this.MaterialsComboBox);
            this.ObjToolsPanel.Controls.Add(this.label3);
            this.ObjToolsPanel.Controls.Add(this.NameBox);
            this.ObjToolsPanel.Controls.Add(this.label5);
            this.ObjToolsPanel.Controls.Add(this.MeshComboBox);
            this.ObjToolsPanel.Controls.Add(this.VisibleCheckBox1);
            this.ObjToolsPanel.Controls.Add(this.label4);
            this.ObjToolsPanel.Controls.Add(this.SclBoxZ);
            this.ObjToolsPanel.Controls.Add(this.SclBoxY);
            this.ObjToolsPanel.Controls.Add(this.SclBoxX);
            this.ObjToolsPanel.Controls.Add(this.PropetriesApplyButton);
            this.ObjToolsPanel.Controls.Add(this.label2);
            this.ObjToolsPanel.Controls.Add(this.RotBoxZ);
            this.ObjToolsPanel.Controls.Add(this.RotBoxY);
            this.ObjToolsPanel.Controls.Add(this.RotBoxX);
            this.ObjToolsPanel.Controls.Add(this.label1);
            this.ObjToolsPanel.Controls.Add(this.ZtextBox3);
            this.ObjToolsPanel.Controls.Add(this.YtextBox2);
            this.ObjToolsPanel.Controls.Add(this.XtextBox1);
            this.ObjToolsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjToolsPanel.Location = new System.Drawing.Point(3, 271);
            this.ObjToolsPanel.Name = "ObjToolsPanel";
            this.ObjToolsPanel.Size = new System.Drawing.Size(272, 263);
            this.ObjToolsPanel.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Cursor = System.Windows.Forms.Cursors.Default;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(3, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 17);
            this.label7.TabIndex = 37;
            this.label7.Text = "Tex";
            // 
            // ObjMaterialComboBox
            // 
            this.ObjMaterialComboBox.FormattingEnabled = true;
            this.ObjMaterialComboBox.Location = new System.Drawing.Point(54, 79);
            this.ObjMaterialComboBox.Name = "ObjMaterialComboBox";
            this.ObjMaterialComboBox.Size = new System.Drawing.Size(212, 21);
            this.ObjMaterialComboBox.TabIndex = 36;
            this.ObjMaterialComboBox.SelectedIndexChanged += new System.EventHandler(this.ObjMaterialComboBox_SelectedIndexChanged);
            this.ObjMaterialComboBox.Click += new System.EventHandler(this.ObjMaterialComboBox_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(3, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 17);
            this.label6.TabIndex = 35;
            this.label6.Text = "Part";
            // 
            // MaterialsComboBox
            // 
            this.MaterialsComboBox.Enabled = false;
            this.MaterialsComboBox.FormattingEnabled = true;
            this.MaterialsComboBox.Location = new System.Drawing.Point(54, 106);
            this.MaterialsComboBox.Name = "MaterialsComboBox";
            this.MaterialsComboBox.Size = new System.Drawing.Size(212, 21);
            this.MaterialsComboBox.TabIndex = 34;
            this.MaterialsComboBox.SelectedIndexChanged += new System.EventHandler(this.MaterialsComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 17);
            this.label3.TabIndex = 33;
            this.label3.Text = "Mesh";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(54, 26);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(212, 20);
            this.NameBox.TabIndex = 32;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(3, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 17);
            this.label5.TabIndex = 31;
            this.label5.Text = "Name";
            // 
            // MeshComboBox
            // 
            this.MeshComboBox.FormattingEnabled = true;
            this.MeshComboBox.Location = new System.Drawing.Point(54, 52);
            this.MeshComboBox.Name = "MeshComboBox";
            this.MeshComboBox.Size = new System.Drawing.Size(212, 21);
            this.MeshComboBox.TabIndex = 30;
            this.MeshComboBox.SelectedIndexChanged += new System.EventHandler(this.MeshComboBox_SelectedIndexChanged);
            // 
            // VisibleCheckBox1
            // 
            this.VisibleCheckBox1.AutoSize = true;
            this.VisibleCheckBox1.Location = new System.Drawing.Point(6, 3);
            this.VisibleCheckBox1.Name = "VisibleCheckBox1";
            this.VisibleCheckBox1.Size = new System.Drawing.Size(56, 17);
            this.VisibleCheckBox1.TabIndex = 29;
            this.VisibleCheckBox1.Text = "Visible";
            this.VisibleCheckBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(3, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 17);
            this.label4.TabIndex = 28;
            this.label4.Text = "Scl";
            // 
            // SclBoxZ
            // 
            this.SclBoxZ.Location = new System.Drawing.Point(139, 200);
            this.SclBoxZ.Name = "SclBoxZ";
            this.SclBoxZ.Size = new System.Drawing.Size(36, 20);
            this.SclBoxZ.TabIndex = 27;
            // 
            // SclBoxY
            // 
            this.SclBoxY.Location = new System.Drawing.Point(97, 200);
            this.SclBoxY.Name = "SclBoxY";
            this.SclBoxY.Size = new System.Drawing.Size(36, 20);
            this.SclBoxY.TabIndex = 26;
            // 
            // SclBoxX
            // 
            this.SclBoxX.Location = new System.Drawing.Point(54, 200);
            this.SclBoxX.Name = "SclBoxX";
            this.SclBoxX.Size = new System.Drawing.Size(36, 20);
            this.SclBoxX.TabIndex = 25;
            // 
            // PropetriesApplyButton
            // 
            this.PropetriesApplyButton.Location = new System.Drawing.Point(6, 234);
            this.PropetriesApplyButton.Name = "PropetriesApplyButton";
            this.PropetriesApplyButton.Size = new System.Drawing.Size(75, 23);
            this.PropetriesApplyButton.TabIndex = 24;
            this.PropetriesApplyButton.Text = "Apply";
            this.PropetriesApplyButton.UseVisualStyleBackColor = true;
            this.PropetriesApplyButton.Click += new System.EventHandler(this.PropetriesApplyButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "Rot";
            // 
            // RotBoxZ
            // 
            this.RotBoxZ.Location = new System.Drawing.Point(139, 174);
            this.RotBoxZ.Name = "RotBoxZ";
            this.RotBoxZ.Size = new System.Drawing.Size(36, 20);
            this.RotBoxZ.TabIndex = 22;
            // 
            // RotBoxY
            // 
            this.RotBoxY.Location = new System.Drawing.Point(97, 174);
            this.RotBoxY.Name = "RotBoxY";
            this.RotBoxY.Size = new System.Drawing.Size(36, 20);
            this.RotBoxY.TabIndex = 21;
            // 
            // RotBoxX
            // 
            this.RotBoxX.Location = new System.Drawing.Point(54, 174);
            this.RotBoxX.Name = "RotBoxX";
            this.RotBoxX.Size = new System.Drawing.Size(36, 20);
            this.RotBoxX.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Pos";
            // 
            // ZtextBox3
            // 
            this.ZtextBox3.Location = new System.Drawing.Point(139, 148);
            this.ZtextBox3.Name = "ZtextBox3";
            this.ZtextBox3.Size = new System.Drawing.Size(36, 20);
            this.ZtextBox3.TabIndex = 16;
            // 
            // YtextBox2
            // 
            this.YtextBox2.Location = new System.Drawing.Point(97, 148);
            this.YtextBox2.Name = "YtextBox2";
            this.YtextBox2.Size = new System.Drawing.Size(36, 20);
            this.YtextBox2.TabIndex = 15;
            // 
            // XtextBox1
            // 
            this.XtextBox1.Location = new System.Drawing.Point(54, 148);
            this.XtextBox1.Name = "XtextBox1";
            this.XtextBox1.Size = new System.Drawing.Size(36, 20);
            this.XtextBox1.TabIndex = 14;
            // 
            // EditorPropetriesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 556);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditorPropetriesForm";
            this.Text = "EditorPropetriesForm";
            this.Load += new System.EventHandler(this.EditorPropetriesForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ObjToolsPanel.ResumeLayout(false);
            this.ObjToolsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Panel ObjToolsPanel;
        private System.Windows.Forms.Button PropetriesApplyButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ZtextBox3;
        private System.Windows.Forms.TextBox YtextBox2;
        private System.Windows.Forms.TextBox XtextBox1;
        private System.Windows.Forms.ComboBox ModelsComboBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox SclBoxZ;
        private System.Windows.Forms.TextBox SclBoxY;
        private System.Windows.Forms.TextBox SclBoxX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RotBoxZ;
        private System.Windows.Forms.TextBox RotBoxY;
        private System.Windows.Forms.TextBox RotBoxX;
        private System.Windows.Forms.CheckBox VisibleCheckBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox MeshComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Label objcountlabel;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox MaterialsComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox ObjMaterialComboBox;
    }
}