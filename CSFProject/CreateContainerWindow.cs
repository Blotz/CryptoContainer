using CSFLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSFProject
{
    public partial class CreateContainerWindow : Form
    {
        public CSFContainer container = null;

        public CreateContainerWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxKey.Text.Length < 12) {
                System.Windows.Forms.MessageBox.Show("Must have a password of at least 12 characters!", "Error - Password too short" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxKey.Text != textBox2.Text)
            {
                System.Windows.Forms.MessageBox.Show("There is a mismatch between the key that you provided and the key confirmation.", "Error - Keys mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox2.Checked)
            {

                if (textBoxKey.Text.Length < 12)
                {
                    System.Windows.Forms.MessageBox.Show("Must have a hidden key of at least 12 characters!", "Error - Hidden key too short", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                    if (textBoxKey.Text != textBox2.Text)
                {
                    System.Windows.Forms.MessageBox.Show("There is a mismatch between the hidden key that you provided and the hidden key confirmation.", "Error - Keys mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CSFContainer container = new CSFContainer(textBoxKey.Text, checkBox1.Checked);
                this.container = container;
                container.Create(textBox1.Text, true, textBox3.Text);
            }
            else
            {
                CSFContainer container = new CSFContainer(textBoxKey.Text, checkBox1.Checked);
                this.container = container;
                container.Create(textBox1.Text);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();

            textBox1.Text = saveFileDialog.FileName;
        }

        private void CreateContainerWindow_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = textBox4.Enabled = checkBox2.Checked;
        }
    }
}
