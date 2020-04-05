using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hangout.Server.WebServices
{
    public partial class SelectForm : Form
    {
        private string selectedComboBox1 = "";

        public string SelectedComboBox1
        {
            get { return selectedComboBox1; }
        }

        public SelectForm(List<string> dropDownList, string label)
        {
            InitializeComponent();

            foreach (string item in dropDownList)
            {
                comboBox1.Items.Add(item);
            }

            label1.Text = label;
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            selectedComboBox1 = (string)comboBox1.SelectedItem;
            this.Close();
        }
    }
}
