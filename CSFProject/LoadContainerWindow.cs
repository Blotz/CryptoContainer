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
    public partial class LoadContainerWindow : Form
    {
        public CSFContainer container = null;

        public LoadContainerWindow()
        {
            InitializeComponent();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    this.DialogResult = DialogResult.Cancel;
        //    this.Close();
        //}

        //private void hide_Click(object sender, EventArgs e)
        //{
        //    CSFContainer container = new CSFContainer(textBoxKey.Text);
        //    this.container = container;
        //    container.Create(textBox1.Text);

        //    this.DialogResult = DialogResult.OK;
        //    this.Close();
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{

        //}

        private void CreateContainerWindow_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                var container = CSFContainer.Load(openFileDialog.FileName, textBoxKey.Text);
                this.container = container;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception err)
            {
                try
                {
                    var container = CSFContainer.Load(openFileDialog.FileName, textBoxKey.Text, true);
                    this.container = container;

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception err2)
                {
                    //System.Windows.Forms.MessageBox.Show(err2.ToString());
                    System.Windows.Forms.MessageBox.Show("There was a problem loading this container. Please make sure that the key you provided is correct and check if the file you chose is a proper container.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.ShowDialog();

            textBox1.Text = openFileDialog.FileName;
        }
    }
}
