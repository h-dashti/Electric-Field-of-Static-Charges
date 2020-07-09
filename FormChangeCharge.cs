using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ElectricField__SaticalCharges
{
    public partial class FormChangeCharge : Form
    {
        public bool isChanged = false;
        public FormChangeCharge()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            isChanged = true;
            this.Close();
        }
    }
}