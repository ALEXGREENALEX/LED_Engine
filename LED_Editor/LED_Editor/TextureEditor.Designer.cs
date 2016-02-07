namespace LED_Editor
{
    partial class TextureEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureEditor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.EngineCheckBox = new System.Windows.Forms.CheckBox();
            this.TexturesDeleteButton = new System.Windows.Forms.Button();
            this.TexturesAddButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.CubemapTexturePanel = new System.Windows.Forms.Panel();
            this.CubeMapFileButton6 = new System.Windows.Forms.Button();
            this.CubeMapFileButton5 = new System.Windows.Forms.Button();
            this.CubeMapFileButton4 = new System.Windows.Forms.Button();
            this.CubeMapFileButton3 = new System.Windows.Forms.Button();
            this.CubeMapFileButton2 = new System.Windows.Forms.Button();
            this.CubeMapFileButton1 = new System.Windows.Forms.Button();
            this.CubeMapFileTextBox6 = new System.Windows.Forms.TextBox();
            this.CubeMapFileTextBox5 = new System.Windows.Forms.TextBox();
            this.CubeMapFileTextBox4 = new System.Windows.Forms.TextBox();
            this.CubeMapFileTextBox3 = new System.Windows.Forms.TextBox();
            this.CubeMapFileTextBox2 = new System.Windows.Forms.TextBox();
            this.CubeMapFileTextBox1 = new System.Windows.Forms.TextBox();
            this.TextureFilePanel = new System.Windows.Forms.Panel();
            this.FileButton = new System.Windows.Forms.Button();
            this.FileTextBox = new System.Windows.Forms.TextBox();
            this.FiltersPanel = new System.Windows.Forms.Panel();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.MinFilterComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MagFilterComboBox = new System.Windows.Forms.ComboBox();
            this.TexturesListBox = new System.Windows.Forms.ListBox();
            this.MaterialTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.TexturesGLControl = new OpenTK.GLControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.RevertButton = new System.Windows.Forms.ToolStripButton();
            this.GLControlPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.CubemapTexturePanel.SuspendLayout();
            this.TextureFilePanel.SuspendLayout();
            this.FiltersPanel.SuspendLayout();
            this.MaterialTabs.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.GLControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.NameTextBox);
            this.panel1.Controls.Add(this.EngineCheckBox);
            this.panel1.Controls.Add(this.TexturesDeleteButton);
            this.panel1.Controls.Add(this.TexturesAddButton);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.TexturesListBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(442, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 405);
            this.panel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(6, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Name";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Enabled = false;
            this.NameTextBox.Location = new System.Drawing.Point(57, 214);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(207, 20);
            this.NameTextBox.TabIndex = 11;
            // 
            // EngineCheckBox
            // 
            this.EngineCheckBox.AutoSize = true;
            this.EngineCheckBox.Enabled = false;
            this.EngineCheckBox.Location = new System.Drawing.Point(269, 191);
            this.EngineCheckBox.Name = "EngineCheckBox";
            this.EngineCheckBox.Size = new System.Drawing.Size(59, 17);
            this.EngineCheckBox.TabIndex = 10;
            this.EngineCheckBox.Text = "Engine";
            this.EngineCheckBox.UseVisualStyleBackColor = true;
            // 
            // TexturesDeleteButton
            // 
            this.TexturesDeleteButton.Location = new System.Drawing.Point(90, 185);
            this.TexturesDeleteButton.Name = "TexturesDeleteButton";
            this.TexturesDeleteButton.Size = new System.Drawing.Size(75, 23);
            this.TexturesDeleteButton.TabIndex = 9;
            this.TexturesDeleteButton.Text = "Delete";
            this.TexturesDeleteButton.UseVisualStyleBackColor = true;
            this.TexturesDeleteButton.Click += new System.EventHandler(this.TexturesDeleteButton_Click);
            // 
            // TexturesAddButton
            // 
            this.TexturesAddButton.Location = new System.Drawing.Point(9, 185);
            this.TexturesAddButton.Name = "TexturesAddButton";
            this.TexturesAddButton.Size = new System.Drawing.Size(75, 23);
            this.TexturesAddButton.TabIndex = 8;
            this.TexturesAddButton.Text = "Add";
            this.TexturesAddButton.UseVisualStyleBackColor = true;
            this.TexturesAddButton.Click += new System.EventHandler(this.TexturesAddButton_Click);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Location = new System.Drawing.Point(0, 240);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(337, 335);
            this.panel2.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.CubemapTexturePanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TextureFilePanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FiltersPanel, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(337, 335);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // CubemapTexturePanel
            // 
            this.CubemapTexturePanel.AutoSize = true;
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileButton6);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileButton5);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileButton4);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileButton3);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileButton2);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileButton1);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileTextBox6);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileTextBox5);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileTextBox4);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileTextBox3);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileTextBox2);
            this.CubemapTexturePanel.Controls.Add(this.CubeMapFileTextBox1);
            this.CubemapTexturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CubemapTexturePanel.Location = new System.Drawing.Point(3, 35);
            this.CubemapTexturePanel.Name = "CubemapTexturePanel";
            this.CubemapTexturePanel.Size = new System.Drawing.Size(330, 156);
            this.CubemapTexturePanel.TabIndex = 2;
            this.CubemapTexturePanel.Visible = false;
            // 
            // CubeMapFileButton6
            // 
            this.CubeMapFileButton6.Enabled = false;
            this.CubeMapFileButton6.Location = new System.Drawing.Point(267, 133);
            this.CubeMapFileButton6.Name = "CubeMapFileButton6";
            this.CubeMapFileButton6.Size = new System.Drawing.Size(60, 20);
            this.CubeMapFileButton6.TabIndex = 13;
            this.CubeMapFileButton6.Text = "Browse";
            this.CubeMapFileButton6.UseVisualStyleBackColor = true;
            // 
            // CubeMapFileButton5
            // 
            this.CubeMapFileButton5.Enabled = false;
            this.CubeMapFileButton5.Location = new System.Drawing.Point(267, 107);
            this.CubeMapFileButton5.Name = "CubeMapFileButton5";
            this.CubeMapFileButton5.Size = new System.Drawing.Size(60, 20);
            this.CubeMapFileButton5.TabIndex = 12;
            this.CubeMapFileButton5.Text = "Browse";
            this.CubeMapFileButton5.UseVisualStyleBackColor = true;
            // 
            // CubeMapFileButton4
            // 
            this.CubeMapFileButton4.Enabled = false;
            this.CubeMapFileButton4.Location = new System.Drawing.Point(267, 81);
            this.CubeMapFileButton4.Name = "CubeMapFileButton4";
            this.CubeMapFileButton4.Size = new System.Drawing.Size(60, 20);
            this.CubeMapFileButton4.TabIndex = 11;
            this.CubeMapFileButton4.Text = "Browse";
            this.CubeMapFileButton4.UseVisualStyleBackColor = true;
            // 
            // CubeMapFileButton3
            // 
            this.CubeMapFileButton3.Enabled = false;
            this.CubeMapFileButton3.Location = new System.Drawing.Point(267, 55);
            this.CubeMapFileButton3.Name = "CubeMapFileButton3";
            this.CubeMapFileButton3.Size = new System.Drawing.Size(60, 20);
            this.CubeMapFileButton3.TabIndex = 10;
            this.CubeMapFileButton3.Text = "Browse";
            this.CubeMapFileButton3.UseVisualStyleBackColor = true;
            // 
            // CubeMapFileButton2
            // 
            this.CubeMapFileButton2.Enabled = false;
            this.CubeMapFileButton2.Location = new System.Drawing.Point(267, 29);
            this.CubeMapFileButton2.Name = "CubeMapFileButton2";
            this.CubeMapFileButton2.Size = new System.Drawing.Size(60, 20);
            this.CubeMapFileButton2.TabIndex = 9;
            this.CubeMapFileButton2.Text = "Browse";
            this.CubeMapFileButton2.UseVisualStyleBackColor = true;
            // 
            // CubeMapFileButton1
            // 
            this.CubeMapFileButton1.Enabled = false;
            this.CubeMapFileButton1.Location = new System.Drawing.Point(267, 3);
            this.CubeMapFileButton1.Name = "CubeMapFileButton1";
            this.CubeMapFileButton1.Size = new System.Drawing.Size(60, 20);
            this.CubeMapFileButton1.TabIndex = 8;
            this.CubeMapFileButton1.Text = "Browse";
            this.CubeMapFileButton1.UseVisualStyleBackColor = true;
            // 
            // CubeMapFileTextBox6
            // 
            this.CubeMapFileTextBox6.Enabled = false;
            this.CubeMapFileTextBox6.Location = new System.Drawing.Point(3, 133);
            this.CubeMapFileTextBox6.Name = "CubeMapFileTextBox6";
            this.CubeMapFileTextBox6.Size = new System.Drawing.Size(258, 20);
            this.CubeMapFileTextBox6.TabIndex = 5;
            // 
            // CubeMapFileTextBox5
            // 
            this.CubeMapFileTextBox5.Enabled = false;
            this.CubeMapFileTextBox5.Location = new System.Drawing.Point(3, 107);
            this.CubeMapFileTextBox5.Name = "CubeMapFileTextBox5";
            this.CubeMapFileTextBox5.Size = new System.Drawing.Size(258, 20);
            this.CubeMapFileTextBox5.TabIndex = 4;
            // 
            // CubeMapFileTextBox4
            // 
            this.CubeMapFileTextBox4.Enabled = false;
            this.CubeMapFileTextBox4.Location = new System.Drawing.Point(3, 81);
            this.CubeMapFileTextBox4.Name = "CubeMapFileTextBox4";
            this.CubeMapFileTextBox4.Size = new System.Drawing.Size(258, 20);
            this.CubeMapFileTextBox4.TabIndex = 3;
            // 
            // CubeMapFileTextBox3
            // 
            this.CubeMapFileTextBox3.Enabled = false;
            this.CubeMapFileTextBox3.Location = new System.Drawing.Point(3, 55);
            this.CubeMapFileTextBox3.Name = "CubeMapFileTextBox3";
            this.CubeMapFileTextBox3.Size = new System.Drawing.Size(258, 20);
            this.CubeMapFileTextBox3.TabIndex = 2;
            // 
            // CubeMapFileTextBox2
            // 
            this.CubeMapFileTextBox2.Enabled = false;
            this.CubeMapFileTextBox2.Location = new System.Drawing.Point(3, 29);
            this.CubeMapFileTextBox2.Name = "CubeMapFileTextBox2";
            this.CubeMapFileTextBox2.Size = new System.Drawing.Size(258, 20);
            this.CubeMapFileTextBox2.TabIndex = 1;
            // 
            // CubeMapFileTextBox1
            // 
            this.CubeMapFileTextBox1.Enabled = false;
            this.CubeMapFileTextBox1.Location = new System.Drawing.Point(3, 3);
            this.CubeMapFileTextBox1.Name = "CubeMapFileTextBox1";
            this.CubeMapFileTextBox1.Size = new System.Drawing.Size(258, 20);
            this.CubeMapFileTextBox1.TabIndex = 0;
            // 
            // TextureFilePanel
            // 
            this.TextureFilePanel.AutoSize = true;
            this.TextureFilePanel.Controls.Add(this.FileButton);
            this.TextureFilePanel.Controls.Add(this.FileTextBox);
            this.TextureFilePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextureFilePanel.Location = new System.Drawing.Point(3, 3);
            this.TextureFilePanel.Name = "TextureFilePanel";
            this.TextureFilePanel.Size = new System.Drawing.Size(330, 26);
            this.TextureFilePanel.TabIndex = 3;
            // 
            // FileButton
            // 
            this.FileButton.Enabled = false;
            this.FileButton.Location = new System.Drawing.Point(267, 3);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(60, 20);
            this.FileButton.TabIndex = 7;
            this.FileButton.Text = "Browse";
            this.FileButton.UseVisualStyleBackColor = true;
            // 
            // FileTextBox
            // 
            this.FileTextBox.Enabled = false;
            this.FileTextBox.Location = new System.Drawing.Point(3, 3);
            this.FileTextBox.Name = "FileTextBox";
            this.FileTextBox.Size = new System.Drawing.Size(258, 20);
            this.FileTextBox.TabIndex = 6;
            // 
            // FiltersPanel
            // 
            this.FiltersPanel.AutoSize = true;
            this.FiltersPanel.Controls.Add(this.ApplyButton);
            this.FiltersPanel.Controls.Add(this.label2);
            this.FiltersPanel.Controls.Add(this.MinFilterComboBox);
            this.FiltersPanel.Controls.Add(this.label1);
            this.FiltersPanel.Controls.Add(this.MagFilterComboBox);
            this.FiltersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FiltersPanel.Location = new System.Drawing.Point(3, 197);
            this.FiltersPanel.Name = "FiltersPanel";
            this.FiltersPanel.Size = new System.Drawing.Size(330, 112);
            this.FiltersPanel.TabIndex = 4;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Enabled = false;
            this.ApplyButton.Location = new System.Drawing.Point(3, 83);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 15;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "MinFilter";
            // 
            // MinFilterComboBox
            // 
            this.MinFilterComboBox.Enabled = false;
            this.MinFilterComboBox.FormattingEnabled = true;
            this.MinFilterComboBox.Location = new System.Drawing.Point(3, 56);
            this.MinFilterComboBox.Name = "MinFilterComboBox";
            this.MinFilterComboBox.Size = new System.Drawing.Size(318, 21);
            this.MinFilterComboBox.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "MagFilter";
            // 
            // MagFilterComboBox
            // 
            this.MagFilterComboBox.Enabled = false;
            this.MagFilterComboBox.FormattingEnabled = true;
            this.MagFilterComboBox.Location = new System.Drawing.Point(3, 16);
            this.MagFilterComboBox.Name = "MagFilterComboBox";
            this.MagFilterComboBox.Size = new System.Drawing.Size(318, 21);
            this.MagFilterComboBox.TabIndex = 11;
            // 
            // TexturesListBox
            // 
            this.TexturesListBox.FormattingEnabled = true;
            this.TexturesListBox.Location = new System.Drawing.Point(7, 20);
            this.TexturesListBox.Name = "TexturesListBox";
            this.TexturesListBox.Size = new System.Drawing.Size(325, 160);
            this.TexturesListBox.TabIndex = 0;
            this.TexturesListBox.SelectedIndexChanged += new System.EventHandler(this.TexturesListBox_SelectedIndexChanged);
            // 
            // MaterialTabs
            // 
            this.MaterialTabs.Controls.Add(this.tabPage1);
            this.MaterialTabs.Controls.Add(this.tabPage3);
            this.MaterialTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.MaterialTabs.Location = new System.Drawing.Point(0, 39);
            this.MaterialTabs.Name = "MaterialTabs";
            this.MaterialTabs.SelectedIndex = 0;
            this.MaterialTabs.Size = new System.Drawing.Size(442, 18);
            this.MaterialTabs.TabIndex = 5;
            this.MaterialTabs.SelectedIndexChanged += new System.EventHandler(this.MaterialTabs_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(434, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Texture";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(453, 0);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "CubeMap";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // TexturesGLControl
            // 
            this.TexturesGLControl.BackColor = System.Drawing.Color.Black;
            this.TexturesGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TexturesGLControl.Location = new System.Drawing.Point(0, 0);
            this.TexturesGLControl.Name = "TexturesGLControl";
            this.TexturesGLControl.Size = new System.Drawing.Size(438, 383);
            this.TexturesGLControl.TabIndex = 0;
            this.TexturesGLControl.VSync = false;
            this.TexturesGLControl.Load += new System.EventHandler(this.TexturesGLControl_Load);
            this.TexturesGLControl.Paint += new System.Windows.Forms.PaintEventHandler(this.TexturesGLControl_Paint);
            this.TexturesGLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TexturesGLControl_MouseMove);
            this.TexturesGLControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TexturesGLControl_MouseUp);
            this.TexturesGLControl.Resize += new System.EventHandler(this.TexturesGLControl_Resize);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveButton,
            this.RevertButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(782, 39);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Margin = new System.Windows.Forms.Padding(6, 1, 0, 2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(36, 36);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // RevertButton
            // 
            this.RevertButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RevertButton.Image = ((System.Drawing.Image)(resources.GetObject("RevertButton.Image")));
            this.RevertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RevertButton.Name = "RevertButton";
            this.RevertButton.Size = new System.Drawing.Size(36, 36);
            this.RevertButton.Text = "Reload";
            // 
            // GLControlPanel
            // 
            this.GLControlPanel.BackColor = System.Drawing.SystemColors.Window;
            this.GLControlPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GLControlPanel.Controls.Add(this.TexturesGLControl);
            this.GLControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GLControlPanel.Location = new System.Drawing.Point(0, 57);
            this.GLControlPanel.Name = "GLControlPanel";
            this.GLControlPanel.Size = new System.Drawing.Size(442, 387);
            this.GLControlPanel.TabIndex = 6;
            // 
            // TextureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 444);
            this.Controls.Add(this.GLControlPanel);
            this.Controls.Add(this.MaterialTabs);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(798, 482);
            this.Name = "TextureEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TextureEditor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TextureEditor_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.CubemapTexturePanel.ResumeLayout(false);
            this.CubemapTexturePanel.PerformLayout();
            this.TextureFilePanel.ResumeLayout(false);
            this.TextureFilePanel.PerformLayout();
            this.FiltersPanel.ResumeLayout(false);
            this.FiltersPanel.PerformLayout();
            this.MaterialTabs.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.GLControlPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox TexturesListBox;
        private System.Windows.Forms.TabControl MaterialTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel CubemapTexturePanel;
        private System.Windows.Forms.TextBox FileTextBox;
        private System.Windows.Forms.TextBox CubeMapFileTextBox6;
        private System.Windows.Forms.TextBox CubeMapFileTextBox5;
        private System.Windows.Forms.TextBox CubeMapFileTextBox4;
        private System.Windows.Forms.TextBox CubeMapFileTextBox3;
        private System.Windows.Forms.TextBox CubeMapFileTextBox2;
        private System.Windows.Forms.TextBox CubeMapFileTextBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel TextureFilePanel;
        private System.Windows.Forms.Panel FiltersPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox MinFilterComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MagFilterComboBox;
        private System.Windows.Forms.Button TexturesDeleteButton;
        private System.Windows.Forms.Button TexturesAddButton;
        private System.Windows.Forms.CheckBox EngineCheckBox;
        private System.Windows.Forms.Button CubeMapFileButton6;
        private System.Windows.Forms.Button CubeMapFileButton5;
        private System.Windows.Forms.Button CubeMapFileButton4;
        private System.Windows.Forms.Button CubeMapFileButton3;
        private System.Windows.Forms.Button CubeMapFileButton2;
        private System.Windows.Forms.Button CubeMapFileButton1;
        private System.Windows.Forms.Button FileButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripButton RevertButton;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label label3;
        private OpenTK.GLControl TexturesGLControl;
        private System.Windows.Forms.Panel GLControlPanel;
    }
}