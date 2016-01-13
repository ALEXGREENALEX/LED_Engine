using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LED_Editor
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }

        private void StartupTimer_Tick(object sender, EventArgs e)
        {
            StartupTimer.Stop();
            this.Close();        
        }
    }
}
