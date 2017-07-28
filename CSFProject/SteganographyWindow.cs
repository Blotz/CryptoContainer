using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSFLib;
using System.Drawing.Imaging;
using System.Collections;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace CSFProject
{
    public partial class SteganographyWindow : Form
    {

        public CSFContainer container = null;
        public byte[] containerBytes = null;
        private double image_max_info = 0;
        private int container_size = 0;
        private Bitmap image_ini = null;
        private Bitmap image_final = null;
        private string texto = null;
        private List<Bitmap> files = new List<Bitmap>();
        private List<Bitmap> files_final = new List<Bitmap>();


        public SteganographyWindow(byte[] containerBytes, int size)
        {
            this.containerBytes = containerBytes;
            InitializeComponent();
            container_size = size;
            label8.Text = size.ToString() + " KBs";
            texto = System.IO.File.ReadAllText(@"C:\Users\Blotz\Desktop\mttexto.txt");
   

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void SteganographyWindow_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void size_max_image()
        {

            this.image_max_info = 0;

            foreach (Bitmap i in files)
            {

                this.image_max_info = this.image_max_info + (i.Height * i.Width * 3 / 8 / 1024);
            }


        }


        private int image_size(Bitmap bmp)//////retorna nr de BITes!!
        {

            return bmp.Height * bmp.Width * 3 / 8;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.jpeg; *.png; *.bmp)|*.jpg; *.png; *.bmp";
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();



            //    try {


            //        int size = System.Text.Encoding.Unicode.GetByteCount(texto);
            //        label8.Text = (size / 1024 ).ToString() + " Kb"; ;

            //        pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);

            //        image_ini = (Bitmap)pictureBox1.Image;

            //        label7.Text = pictureBox1.Image.Size.ToString();

            //        image_max_info = pictureBox1.Image.Size.Height * pictureBox1.Image.Size.Width * 3 / 8 / 1024;


            //        label6.Text = "Your image can store at maximum " + image_max_info + " KBs";

            //        if (image_max_info < container_size)
            //        {
            //            label8.ForeColor = System.Drawing.Color.Red;
            //        }
            //        else {
            //            label8.ForeColor = System.Drawing.Color.Green;
            //        }
            //    }
            //    catch { }
        }


        private void hide_Click(object sender, EventArgs e)
        {

            if (container_size < this.image_max_info)
            {

                string finalText =  ByteArrayToString(containerBytes);
                string textTemp = null;


                bool last = false;
                int image_Totalsize = 0;
             


                for (int x = 0; x < files.Count(); x++)
                {

                    finalText = x.ToString() + " " + finalText;
                    image_Totalsize = image_size(files.ElementAt(x));

                    if (image_Totalsize > finalText.Length)
                    {


                        if (last)
                        {

                            finalText = x.ToString() + " ";

                        }
                        else
                        {
                            textTemp = finalText;
                        }

                        String name = "Photo" + x.ToString() + ".png";
                        image_final = SteganographyHelper.embedText(textTemp, files.ElementAt(x));
                        image_final.Save(@"C:\Users\Blotz\Desktop\" + name, ImageFormat.Png);
                        finalText = "";
                        last = true;




                    }
                    else {

                        textTemp = finalText.Substring(0, image_Totalsize);
                        finalText = finalText.Substring(image_Totalsize);

                        String name = "Photo" + x.ToString() + ".png";
                        image_final = SteganographyHelper.embedText(textTemp, files.ElementAt(x));
                        image_final.Save(@"C:\Users\Blotz\Desktop\" + name, ImageFormat.Png);


                    }

                  



                }

                this.files.Clear();
                imageList1.Images.Clear();




                //SaveFileDialog save_dialog = new SaveFileDialog();
                //save_dialog.Filter = "Png Image|*.png|Bitmap Image|*.bmp";

                //if (save_dialog.ShowDialog() == DialogResult.OK)
                //{
                //    switch (save_dialog.FilterIndex)
                //    {
                //        case 0:
                //            {
                //                image_final.Save(save_dialog.FileName, ImageFormat.Png);
                //            }
                //            break;
                //        case 1:
                //            {
                //                image_final.Save(save_dialog.FileName, ImageFormat.Bmp);
                //            }
                //            break;
                //    }

                //}

                MessageBox.Show("DONE!");
            }
            else
            {

                MessageBox.Show("Error: Not enough images for the size of the container. Please add more images");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // image_ini = (Bitmap)pictureBox1.Image;

            string finalText = null;
            string text;

           // List<String> temp_list = new List<String>(files.Count());

            string[] temp_list = new string[files.Count()];


            try
            {
                for (int x = 0; x < files.Count(); x++)
                {


                    text = SteganographyHelper.extractText(this.files.ElementAt(x));

                    string temp = text.Substring(text.IndexOf(" ") + 1);

                    temp_list[Int32.Parse(text.Substring(0, 1))] = temp;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            foreach (String text_temp in temp_list)
            {
                finalText = string.Concat(finalText, text_temp);
            }
            

            MessageBox.Show("DONE!");

            button3.Enabled = true;


            this.files.Clear();
            imageList1.Images.Clear();


            System.IO.File.WriteAllText(@"C:\Users\Blotz\Desktop\WriteLines.txt", finalText);

            this.containerBytes = GetBytes(finalText);



            //try
            //{
            //   // extractedText = Crypto.DecryptStringAES(extractedText, passwordTextBox.Text);
            //}
            //catch
            //{
            //    MessageBox.Show("Wrong password", "Error");

            //    return;
            //}


            //dataTextBox.Text = extractedText;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.IO.Stream myStream;
            OpenFileDialog thisDialog = new OpenFileDialog();


            thisDialog.InitialDirectory = "d:\\";
            thisDialog.Filter = "Image files (*.png)|*.png";
            thisDialog.FilterIndex = 2;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;
            thisDialog.Title = "Please Select Source your Image(s)";

            if (thisDialog.ShowDialog() == DialogResult.OK)
            {

                foreach (String file in thisDialog.FileNames)
                {

                    try
                    {
                        if ((myStream = thisDialog.OpenFile()) != null)
                        {
                            using (myStream)
                            {
                                this.files.Add((Bitmap)Image.FromFile(file));
                                imageList1.Images.Add((Bitmap)Image.FromFile(file));



                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
   
            this.listView1.View = View.Tile;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.imageList1.ImageSize = new Size(25, 32);
            this.listView1.LargeImageList = this.imageList1;


            this.listView1.Items.Clear();
            for (int j = 0; j < this.imageList1.Images.Count; j++)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = j;
                this.listView1.Items.Add(item);
            }


            size_max_image();
            label2.Text = image_max_info.ToString() + "Kbs";
                

            if (image_max_info < container_size)
            {
                label8.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                label8.ForeColor = System.Drawing.Color.Green;
            }


        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {

           // saveFileDialog1.ShowDialog();

            SaveContainerWindow ste = new SaveContainerWindow(this.containerBytes);
            if (ste.ShowDialog() != DialogResult.Cancel)
            {
                this.container = ste.container;

                this.DialogResult = DialogResult.OK;

                this.Close();


            }




            //container = CSFContainer.Load(openFileDialog1.FileName, textBoxKey.Text);


            //System.IO.File.WriteAllText(container.Path, container);

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

        /// <summary>
        /// Converts a byte array into a string
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static string ByteArrayToString(byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        private static byte[] GetBytes(string str)
        {
        
            return Convert.FromBase64String(str);
        }


    }
}
