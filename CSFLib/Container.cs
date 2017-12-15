using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSFLib;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace CSFLib
{
    public class CSFContainer
    {
        public string Path { get;  set; }
        public List<CFile> Files = new List<CFile>();

        protected int HeaderSize { get; set; }

        private int nextOffset = 50008;

        private string Key { get; set; }

        private bool addRandomBuffers = false;

        public string FileName = "";

        private bool empty = true;

        private byte[] header1 = new byte[0];
        private byte[] header2 = new byte[0];
        private bool hiddenContainerLoaded = false;

        private FileStream FS;
  
        public CSFContainer() { }

        public CSFContainer(string key, bool addRandomBuffers = false)
        {
            this.Key = key;
            this.addRandomBuffers = addRandomBuffers;
        }

        /// <summary>
        /// Loads a container
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CSFContainer Load(string filename, string key, bool loadHidden = false)
        {
            var container = new CSFContainer(key);
            container.hiddenContainerLoaded = loadHidden;

            var buffer = ReadFileBytes(filename);

            container.Path = filename;
            int sizeHeader1 = BitConverter.ToInt32(new byte[4] { buffer[0], buffer[1], buffer[2], buffer[3] }, 0);
            int sizeHeader2 = BitConverter.ToInt32(new byte[4] { buffer[4], buffer[5], buffer[6], buffer[7] }, 0);

            container.HeaderSize = sizeHeader1;

            // Get the header
            var header1 = new byte[sizeHeader1];
            for (int i = 0; i < sizeHeader1; i++)
                header1[i] = buffer[i + 8];

            var header2 = new byte[sizeHeader2];
            for (int i = 0; i < sizeHeader2; i++)
                header2[i] = buffer[25000 + i + 8];

            container.header1 = header1;
            container.header2 = header2;

            byte[] header = null;
            if (loadHidden)
                header = AESThenHMAC.SimpleDecryptWithPassword(header2, container.Key);
            else
                header = AESThenHMAC.SimpleDecryptWithPassword(header1, container.Key);

            string header_str = ByteArrayToString(header);

            if (header_str == "empty")
                return container;

            var lines = header_str.Split("\n".ToCharArray());

            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                    continue;

                var separated = line.Split("|".ToCharArray());
                
        
                var offset = int.Parse(separated[1]);
                var len = int.Parse(separated[2]);
                var data = new byte[len];
                Array.Copy(buffer, offset, data, 0, len);
                container.Files.Add(new CFile { Name = separated[0], Offset = offset, Length = len, data = data, Dummy = ((separated[3] == "dummy") ? true : false) });
            }

            container.nextOffset = buffer.Length;

            return container;
        }



        public static CSFContainer LocalLoad(byte[] Localcontainer, string key ,string path, bool loadHidden = false)
        {
            var container = new CSFContainer(key);
            container.hiddenContainerLoaded = loadHidden;

            var buffer = Localcontainer;

        

            container.Path = path;
            int sizeHeader1 = BitConverter.ToInt32(new byte[4] { buffer[0], buffer[1], buffer[2], buffer[3] }, 0);
            int sizeHeader2 = BitConverter.ToInt32(new byte[4] { buffer[4], buffer[5], buffer[6], buffer[7] }, 0);

            container.HeaderSize = sizeHeader1;

            // Get the header
            var header1 = new byte[sizeHeader1];
            for (int i = 0; i < sizeHeader1; i++)
                header1[i] = buffer[i + 8];

            var header2 = new byte[sizeHeader2];
            for (int i = 0; i < sizeHeader2; i++)
                header2[i] = buffer[25000 + i + 8];

            container.header1 = header1;
            container.header2 = header2;

            byte[] header = null;
            if (loadHidden)
                header = AESThenHMAC.SimpleDecryptWithPassword(header2, container.Key);
            else
                header = AESThenHMAC.SimpleDecryptWithPassword(header1, container.Key);

            string header_str = ByteArrayToString(header);

            if (header_str == "empty")
                return container;

            var lines = header_str.Split("\n".ToCharArray());

            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                    continue;

                var separated = line.Split("|".ToCharArray());


                var offset = int.Parse(separated[1]);
                var len = int.Parse(separated[2]);
                var data = new byte[len];
                Array.Copy(buffer, offset, data, 0, len);
                container.Files.Add(new CFile { Name = separated[0], Offset = offset, Length = len, data = data, Dummy = ((separated[3] == "dummy") ? true : false) });
            }

            container.nextOffset = buffer.Length;


           

            return container;
        }


        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="filename"></param>
        public void Create(string filename, bool createHidden = false, string hiddenKey = null)
        {
            using (FileStream fs = File.Create(filename)){ }

            this.Path = filename;

            if (hiddenKey == null)
                hiddenKey = RandomString(32);

            this.header2 = AESThenHMAC.SimpleEncryptWithPassword(GetBytes("empty"), hiddenKey) ;


            CFile cfile = new CFile { Name = "The container is empty", Length = 0, Offset = 0, data = AESThenHMAC.SimpleEncryptWithPassword(GetBytes("empty"), this.Key), Dummy = false };
            cfile.Length = 0;

            this.Files.Add(cfile);

            this.Save();


        }

        /// <summary>
        /// Saves the current state of the container.
        /// </summary>
        public void Save()
        {
            byte[] header = GetBytes(FileListToString());

            header = AESThenHMAC.SimpleEncryptWithPassword(header, this.Key);

            if (hiddenContainerLoaded)
                header2 = header;
            else
                header1 = header;

            int j = 0;
            using (FileStream fileStream = new FileStream(this.Path, FileMode.Create))
            {
                var size1 = System.BitConverter.GetBytes(header1.Length);
                fileStream.Write(size1, 0, size1.Length);

                var size2 = System.BitConverter.GetBytes(header2.Length);
                fileStream.Write(size2, 0, size2.Length);

                Random rand = new Random();

                j = 0;
                // Write the data to the file, byte by byte.
                for (int i = 0; i < header1.Length; i++)
                    fileStream.WriteByte(header1[i]);
                j += header1.Length;
                for (int i = j; i < 25000; i++)
                    fileStream.WriteByte((byte)rand.Next());

                j = 0;
                for (int i = 0; i < header2.Length; i++)
                    fileStream.WriteByte(header2[i]);
                j += header2.Length;
                for (int i = j; i < 25000; i++)
                    fileStream.WriteByte((byte)rand.Next());

                foreach (var cfile in this.Files)
                {
                    for (int i = 0; i < cfile.data.Length; i++)
                        fileStream.WriteByte(cfile.data[i]);
                }
            }
        }

        /// <summary>
        /// Adds a new file to the container
        /// </summary>
        /// <param name="path"></param>
        public void AddFile(string path)
        {

            if(this.Files.ElementAt(0).Name == "The container is empty")
                this.Files.RemoveAt(0);

            FileInfo info = new FileInfo(path);

            CFile cfile = new CFile { Name = info.Name, Length = (int)info.Length, Offset = this.nextOffset, data = AESThenHMAC.SimpleEncryptWithPassword(ReadFileBytes(path), this.Key), Dummy = false };
            cfile.Length = cfile.data.Length;

            this.Files.Add(cfile);

            this.Save();

            nextOffset += (int)cfile.Length;

            if (this.addRandomBuffers)
            {
                int randSize = (int)(cfile.Length * (0.05f + new Random().NextDouble() / 10f));
                var data = AESThenHMAC.SimpleEncryptWithPassword(CreateArrayWithRandomContent(randSize), this.Key);
                var dummy = new CFile { Name = "", Length = data.Length, Offset = this.nextOffset, data = data, Dummy = true };
                this.Files.Add(dummy);
                this.Save();
                nextOffset += (int)dummy.Length;
            }
        }

        /// <summary>
        /// Extracts a filed from the container at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="destination"></param>
        public void ExtractFile(int index, string destination)
        {
            var datab = AESThenHMAC.SimpleDecryptWithPassword(this.Files[index].data, this.Key);
            File.WriteAllBytes(destination, datab);
        }

        public void ExtractFileTemp(int index, string destination)
        {
            var datab = AESThenHMAC.SimpleDecryptWithPassword(this.Files[index].data, this.Key);
    
            FS = new FileStream(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
            
                FS.Write(datab, 0, datab.Length);
                Process.Start(destination);
            

        }


        /// <summary>
        /// Removes a file from the container at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveFile(int index)
        {
            this.Files.RemoveAt(index);

            for (int i = 1; i < this.Files.Count; i++)
                this.Files[i].Offset = this.Files[i - 1].Offset + this.Files[i - 1].Length;

            if(this.Files.Count == 0)
            {

                CFile cfile = new CFile { Name = "The container is empty", Length = 0, Offset = 0, data = AESThenHMAC.SimpleEncryptWithPassword(GetBytes("empty"), this.Key), Dummy = false };
                cfile.Length = 0;

                this.Files.Add(cfile);
            }
            this.Save();

            if(this.addRandomBuffers){
                RemoveDummy(index);
            }
        }


        public void RemoveDummy(int index)
        {
            this.Files.RemoveAt(index);

            for (int i = 1; i < this.Files.Count; i++)
                this.Files[i].Offset = this.Files[i - 1].Offset + this.Files[i - 1].Length;

            if (this.Files.Count == 0)
            {

                CFile cfile = new CFile { Name = "The container is empty", Length = 0, Offset = 0, data = AESThenHMAC.SimpleEncryptWithPassword(GetBytes("empty"), this.Key), Dummy = false };
                cfile.Length = 0;

                this.Files.Add(cfile);
            }
            this.Save();

        }

        /// <summary>
        /// Converts the file list into a string that will be used as a header for the container.
        /// </summary>
        /// <returns></returns>
        protected string FileListToString()
        {
            string list = "";

            foreach (var file in this.Files)
                list += file.Name + "|" + file.Offset + "|" + file.data.Length + "|" + ((file.Dummy) ? "dummy" : "file") + "\n";

            return list;
        }

        /// <summary>
        /// Converts a string into a byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
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
            return System.Text.Encoding.Unicode.GetString(buffer);
        }

        /// <summary>
        /// Creates an array with random content
        /// </summary>
        /// <param name="size">size of the array</param>
        /// <returns></returns>
        public byte[] CreateArrayWithRandomContent(int size)
        {
            byte[] randArray = new byte[size];
            new Random().NextBytes(randArray);
            return randArray;
        }

        /// <summary>
        /// Creates a random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    /// <summary>
    /// This class represents a file inside the container
    /// </summary>
    public class CFile
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public bool Dummy {get; set;} // Is it a real file, or just a dummy blob of random memory?

        public byte[] data;
    }
}
