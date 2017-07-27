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
    public partial class SaveContainerWindow : Form
    {

        public CSFContainer container = null;
        private byte[] containerBytes = null;
        private string path = null;

        public SaveContainerWindow(byte[] container) {

            this.containerBytes = container;
            InitializeComponent();


        }

        public SaveContainerWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var container = CSFContainer.LocalLoad(this.containerBytes, textBoxKey.Text,path );
                container.Save();
                this.container = container;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception err)
            {
                try
                {
                    var container = CSFContainer.LocalLoad(this.containerBytes, textBoxKey.Text, path, true);
                    container.Save();
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

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog2.FileName = "";
            saveFileDialog2.ShowDialog();

            textBox1.Text = saveFileDialog2.FileName;
            path = saveFileDialog2.FileName;
        }

        

        
    }
}
