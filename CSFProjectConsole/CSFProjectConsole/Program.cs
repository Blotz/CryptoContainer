using CSFLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;

namespace CSFProjectConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int switchExpression = 0;
            CSFContainer container = null;
            bool submenu = false;

            while (true)
            {
                Console.WriteLine("[1] Create Container");
                Console.WriteLine("[2] Open Container");
                if (submenu)
                {
                    Console.WriteLine("[3] List Files");
                    Console.WriteLine("[4] Add File");
                    Console.WriteLine("[5] Extract File");
                    Console.WriteLine("[6] Delete File");
                };

                switchExpression = Convert.ToInt32(Console.ReadLine());
                if (!submenu)
                {
                    if (switchExpression > 2)
                        switchExpression = 7;
                }
                switch (switchExpression)
                {
                    case 1:
                        Console.WriteLine("[1]Create Container");
                        Console.WriteLine("Choose Container path:");
                        string name = Console.ReadLine();
                        Console.WriteLine("Choose Container Password:");
                        string pass = Console.ReadLine();
                        Console.WriteLine("Obfuscate the sizes of the files ? [Y/N]");
                        string decision = Console.ReadLine();
                        bool addRandomBuffers = false;
                        if (decision == "Y" | decision == "y")
                        {
                            addRandomBuffers = true;
                        }
                        CSFContainer container1 = new CSFContainer(pass, addRandomBuffers);
                        container = container1;                    
                        container.Create(name);
                        submenu = true;

                        break;
                    case 2:
                        Console.WriteLine("[2] Open Container");
                        Console.WriteLine("Container path:");
                        name = Console.ReadLine();
                        Console.WriteLine("Container Password:");
                        pass = Console.ReadLine();

                        try
                        {
                            var container2 = CSFContainer.Load(name,pass);
                            container = container2;

                        }
                        catch (Exception err)
                        {
                            Console.WriteLine("There was a problem loading this container. Please make sure that the key you provided is correct and check if the file you chose is a proper container." + "\n");
                            break;
                        }
                        submenu = true;
                        break;
                    case 3:
                        Console.WriteLine("[3] List Files");
                        foreach (var file in container.Files)
                        {
                            if (file.Dummy)
                            {
                                continue;
                            }
                            Console.Write(file.Name + "\n");
                        }
                        Console.Write("Total Size: " + (new System.IO.FileInfo(container.Path).Length / 1024).ToString() + " Kb" + "\n");
                        break;
                    case 4:
                        Console.WriteLine("[4] Add File");
                        Console.WriteLine("Write file path");
                        name = Console.ReadLine();
                        container.AddFile(name);
              
                        break;
                    case 5:
                        Console.WriteLine("[4] Extract File");
                        Console.WriteLine("Choose file");
                        int i = 1;
                        foreach (var file in container.Files)
                        {
                            if (file.Dummy)
                            {
                                i++;
                                continue;
                            }
                            Console.Write("[" + i + "]" + file.Name + "\n");
                            i++;

                        }
                        int filen = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Write path to extract");
                        string path = Console.ReadLine();
                        container.ExtractFile(filen-1, path);

                        break;
                    case 6:
                        Console.WriteLine("[5] Delete File");
                        Console.WriteLine("Choose File");
                        i = 1;
                        foreach (var file in container.Files)
                        {
                            if (file.Dummy)
                            {
                                i++;
                                continue;
                            }
                                
                            Console.Write("[" + i + "]" + file.Name + "\n");
                            i++;
                            
                        }
                        Console.WriteLine("Choose File");
                        i = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Are you sure you want to permanently remove this file ? [Y/N]");
                        string decision1 = Console.ReadLine();
                        if(decision1 == "Y"| decision1 == "y")
                        { 
                            container.RemoveFile(i-1);
                        }
                        break;
                    default:
                        Console.WriteLine("Option not recognizable");
                        break;
                }
            }
        }     
    } 
}
