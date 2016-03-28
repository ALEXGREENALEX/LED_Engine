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
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ObjToolsPanel = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.FileButton = new System.Windows.Forms.Button();
            this.ObjFileTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ZtextBox3 = new System.Windows.Forms.TextBox();
            this.YtextBox2 = new System.Windows.Forms.TextBox();
            this.XtextBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.EngineCheckBox1 = new System.Windows.Forms.CheckBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.ModelsComboBox1 = new System.Windows.Forms.ComboBox();
            this.RotBoxX = new System.Windows.Forms.TextBox();
            this.RotBoxY = new System.Windows.Forms.TextBox();
            this.RotBoxZ = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SclBoxZ = new System.Windows.Forms.TextBox();
            this.SclBoxY = new System.Windows.Forms.TextBox();
            this.SclBoxX = new System.Windows.Forms.TextBox();
            this.VisibleCheckBox1 = new System.Windows.Forms.CheckBox();
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
            this.groupBox2.Size = new System.Drawing.Size(284, 429);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(278, 410);
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
            this.panel1.Size = new System.Drawing.Size(272, 199);
            this.panel1.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 21);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(272, 147);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 168);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(272, 31);
            this.panel2.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.Location = new System.Drawing.Point(172, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(27, 24);
            this.button4.TabIndex = 4;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.Location = new System.Drawing.Point(139, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(27, 24);
            this.button3.TabIndex = 3;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(60, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 24);
            this.button2.TabIndex = 2;
            this.button2.Text = "Remove";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ObjToolsPanel
            // 
            this.ObjToolsPanel.Controls.Add(this.VisibleCheckBox1);
            this.ObjToolsPanel.Controls.Add(this.label4);
            this.ObjToolsPanel.Controls.Add(this.SclBoxZ);
            this.ObjToolsPanel.Controls.Add(this.SclBoxY);
            this.ObjToolsPanel.Controls.Add(this.SclBoxX);
            this.ObjToolsPanel.Controls.Add(this.button5);
            this.ObjToolsPanel.Controls.Add(this.label2);
            this.ObjToolsPanel.Controls.Add(this.RotBoxZ);
            this.ObjToolsPanel.Controls.Add(this.RotBoxY);
            this.ObjToolsPanel.Controls.Add(this.RotBoxX);
            this.ObjToolsPanel.Controls.Add(this.FileButton);
            this.ObjToolsPanel.Controls.Add(this.ObjFileTextBox);
            this.ObjToolsPanel.Controls.Add(this.label1);
            this.ObjToolsPanel.Controls.Add(this.ZtextBox3);
            this.ObjToolsPanel.Controls.Add(this.YtextBox2);
            this.ObjToolsPanel.Controls.Add(this.XtextBox1);
            this.ObjToolsPanel.Controls.Add(this.label3);
            this.ObjToolsPanel.Controls.Add(this.EngineCheckBox1);
            this.ObjToolsPanel.Controls.Add(this.NameTextBox);
            this.ObjToolsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjToolsPanel.Location = new System.Drawing.Point(3, 208);
            this.ObjToolsPanel.Name = "ObjToolsPanel";
            this.ObjToolsPanel.Size = new System.Drawing.Size(272, 199);
            this.ObjToolsPanel.TabIndex = 3;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(6, 170);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 24;
            this.button5.Text = "Apply";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // FileButton
            // 
            this.FileButton.Location = new System.Drawing.Point(206, 52);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(60, 20);
            this.FileButton.TabIndex = 19;
            this.FileButton.Text = "Browse";
            this.FileButton.UseVisualStyleBackColor = true;
            // 
            // ObjFileTextBox
            // 
            this.ObjFileTextBox.Location = new System.Drawing.Point(6, 52);
            this.ObjFileTextBox.Name = "ObjFileTextBox";
            this.ObjFileTextBox.Size = new System.Drawing.Size(193, 20);
            this.ObjFileTextBox.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Pos";
            // 
            // ZtextBox3
            // 
            this.ZtextBox3.Location = new System.Drawing.Point(139, 83);
            this.ZtextBox3.Name = "ZtextBox3";
            this.ZtextBox3.Size = new System.Drawing.Size(36, 20);
            this.ZtextBox3.TabIndex = 16;
            // 
            // YtextBox2
            // 
            this.YtextBox2.Location = new System.Drawing.Point(97, 83);
            this.YtextBox2.Name = "YtextBox2";
            this.YtextBox2.Size = new System.Drawing.Size(36, 20);
            this.YtextBox2.TabIndex = 15;
            // 
            // XtextBox1
            // 
            this.XtextBox1.Location = new System.Drawing.Point(54, 83);
            this.XtextBox1.Name = "XtextBox1";
            this.XtextBox1.Size = new System.Drawing.Size(36, 20);
            this.XtextBox1.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Name";
            // 
            // EngineCheckBox1
            // 
            this.EngineCheckBox1.AutoSize = true;
            this.EngineCheckBox1.Location = new System.Drawing.Point(3, 3);
            this.EngineCheckBox1.Name = "EngineCheckBox1";
            this.EngineCheckBox1.Size = new System.Drawing.Size(59, 17);
            this.EngineCheckBox1.TabIndex = 4;
            this.EngineCheckBox1.Text = "Engine";
            this.EngineCheckBox1.UseVisualStyleBackColor = true;
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(54, 26);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(212, 20);
            this.NameTextBox.TabIndex = 3;
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
            // RotBoxX
            // 
            this.RotBoxX.Location = new System.Drawing.Point(54, 109);
            this.RotBoxX.Name = "RotBoxX";
            this.RotBoxX.Size = new System.Drawing.Size(36, 20);
            this.RotBoxX.TabIndex = 20;
            // 
            // RotBoxY
            // 
            this.RotBoxY.Location = new System.Drawing.Point(97, 109);
            this.RotBoxY.Name = "RotBoxY";
            this.RotBoxY.Size = new System.Drawing.Size(36, 20);
            this.RotBoxY.TabIndex = 21;
            // 
            // RotBoxZ
            // 
            this.RotBoxZ.Location = new System.Drawing.Point(139, 109);
            this.RotBoxZ.Name = "RotBoxZ";
            this.RotBoxZ.Size = new System.Drawing.Size(36, 20);
            this.RotBoxZ.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "Rot";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(3, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 17);
            this.label4.TabIndex = 28;
            this.label4.Text = "Scl";
            // 
            // SclBoxZ
            // 
            this.SclBoxZ.Location = new System.Drawing.Point(139, 135);
            this.SclBoxZ.Name = "SclBoxZ";
            this.SclBoxZ.Size = new System.Drawing.Size(36, 20);
            this.SclBoxZ.TabIndex = 27;
            // 
            // SclBoxY
            // 
            this.SclBoxY.Location = new System.Drawing.Point(97, 135);
            this.SclBoxY.Name = "SclBoxY";
            this.SclBoxY.Size = new System.Drawing.Size(36, 20);
            this.SclBoxY.TabIndex = 26;
            // 
            // SclBoxX
            // 
            this.SclBoxX.Location = new System.Drawing.Point(54, 135);
            this.SclBoxX.Name = "SclBoxX";
            this.SclBoxX.Size = new System.Drawing.Size(36, 20);
            this.SclBoxX.TabIndex = 25;
            // 
            // VisibleCheckBox1
            // 
            this.VisibleCheckBox1.AutoSize = true;
            this.VisibleCheckBox1.Location = new System.Drawing.Point(60, 3);
            this.VisibleCheckBox1.Name = "VisibleCheckBox1";
            this.VisibleCheckBox1.Size = new System.Drawing.Size(56, 17);
            this.VisibleCheckBox1.TabIndex = 29;
            this.VisibleCheckBox1.Text = "Visible";
            this.VisibleCheckBox1.UseVisualStyleBackColor = true;
            // 
            // EditorPropetriesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 429);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel ObjToolsPanel;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button FileButton;
        private System.Windows.Forms.TextBox ObjFileTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ZtextBox3;
        private System.Windows.Forms.TextBox YtextBox2;
        private System.Windows.Forms.TextBox XtextBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox EngineCheckBox1;
        private System.Windows.Forms.TextBox NameTextBox;
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
    }
}