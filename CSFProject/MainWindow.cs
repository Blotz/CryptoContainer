using CSFLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CSFProject
{
    public partial class MainWindow : Form
    {
        CSFContainer container = null;

        List<int> indices = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateContainerWindow frm = new CreateContainerWindow();
            if (frm.ShowDialog() != DialogResult.Cancel)
            {
                this.container = frm.container;
                FillFileList();
                label1.Text = "Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb";
                button3.Enabled = button4.Enabled = button5.Enabled = true;
             

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadContainerWindow frm = new LoadContainerWindow();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                this.container = frm.container;
                FillFileList();

                this.Text = "CSFProject - " + container.Path;

                label1.Text = "Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb";
                button3.Enabled = button4.Enabled = button5.Enabled = button7.Enabled = true;
            }
        }

        private void FillFileList()
        {
            indices.Clear();
            listViewFiles.Items.Clear();
            int i = 0, j = 0;
            foreach (var file in this.container.Files)
            {
                j++;

                if (file.Dummy)
                    continue;

                indices.Add(j - 1);
                listViewFiles.Items.Add(file.Name);
                listViewFiles.Items[i].SubItems.Add((file.Length / 1024).ToString() + " Kb");
                i++;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            container.AddFile(openFileDialog.FileName);
            FillFileList();

            label1.Text = "Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try {
                saveFileDialog.ShowDialog();
                container.ExtractFile(indices[listViewFiles.SelectedIndices[0]], saveFileDialog.FileName);
            } catch (Exception exc)
                {
                System.Windows.Forms.MessageBox.Show("Please select a file to extract");
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to permanently remove this file?", "CSFProject", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                container.RemoveFile(indices[listViewFiles.SelectedIndices[0]]);
                FillFileList();
                label1.Text = "Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.container == null)
            {

                SteganographyWindow ste = new SteganographyWindow(null, 0);
                ste.ShowDialog();
                //if (ste.ShowDialog() != DialogResult.Cancel)
                //{
                //    //this.container = ste.container;
                //    //FillFileList();
                //    //label1.Text = "Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb";
                //    //button3.Enabled = button4.Enabled = button5.Enabled = true;
                //    //this.Text = "CSFProject - " + container.Path;


                //}
            }
            else{
                SteganographyWindow ste = new SteganographyWindow(ReadFileBytes(this.container.Path), (int)(new System.IO.FileInfo(container.Path).Length / 1024));
                ste.ShowDialog();
                //if (ste.ShowDialog() != DialogResult.Cancel)
                //{
                //    this.container = ste.container;
                //    FillFileList();
                //    label1.Text = "Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb";
                //    button3.Enabled = button4.Enabled = button5.Enabled = true;

                //    this.Text = "CSFProject - " + container.Path;

                //}
            }

            
        }


        /// <summary>
        /// Reads all the bytes from a file
        /// </summary>
        /// <param name="filename">path</param>
        /// <returns></returns>
        static byte[] ReadFileBytes(string filename)
        {
            return System.IO.File.ReadAllBytes(filename);
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

            try
            {
                string fileName = System.IO.Directory.GetCurrentDirectory() + @"\" + listViewFiles.SelectedItems[0].Text;
                container.ExtractFileTemp(indices[listViewFiles.SelectedIndices[0]], fileName);
            }
            catch (Exception exc)
            {
                    System.Windows.Forms.MessageBox.Show("Please select a file to open");
            }


        }
    }
}
