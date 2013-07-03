using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Imagicut
{
    internal class DataPool
    {
        private static List<string> images = new List<string>();
        private static List<string> imgExts = new List<string>() { "jpg", "jpeg", "png", "gif", "bmp" };

        internal static void SetPath(string[] args)
        {
            if (args.Length == 1)
            {
                string lcArg = args[0];
                if (lcArg.StartsWith("-p"))
                {
                    string path = lcArg.Substring(2).Trim();

                    if (FSOpt.IsDirectory(path))
                    {
                        WorkingDirectory = path.Trim(new char[] { '"' });
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid parameter.\n");
                    }
                }
                else if (lcArg.Equals("-?") || lcArg.Equals("-help"))
                {
                    Console.WriteLine("HELP:\n    Set the working directory.\n");
                    Console.WriteLine("    set-path [-p [directory name]] [-?|help]");
                    Console.WriteLine("    Cmdlet alias: path\n");
                    Console.WriteLine("    -p\tThe name of working directory.");
                    Console.WriteLine("    -?\tHelp of the cmdlet.");
                    Console.WriteLine("    -help\tHelp of the cmdlet.\n");
                }
                else if (string.IsNullOrEmpty(args[0]))
                {
                    // use current directory
                    Console.WriteLine(WorkingDirectory + "\n");
                }
                else
                {
                    Console.WriteLine("Invalid parameter.\n");
                }
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("Only one parameter can be accepted.\n");
            }
        }

        internal static void Load(string[] args)
        {
            SearchOption so = SearchOption.TopDirectoryOnly;

            string lcArg = "";
            string file = WorkingDirectory;

            #region Process Parameters

            foreach (string arg in args)
            {
                lcArg = arg.ToLower();

                if (lcArg.StartsWith("-r"))
                {
                    so = SearchOption.AllDirectories;
                }
                else if (lcArg.StartsWith("-t"))
                {
                    imgExts = new List<string>(lcArg.Substring(2).Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else if (lcArg.StartsWith("-f"))
                {
                    file = lcArg.Substring(2).Trim().Trim(new char[] { '\\', '"' });
                    if (!Path.IsPathRooted(file))
                    {
                        file = Path.Combine(WorkingDirectory, file);
                    }
                    imgExts = new List<string>() { Path.GetExtension(file).Replace(".", "") };
                }
                else if (lcArg.Equals("-?") || lcArg.Equals("-help"))
                {
                    Console.WriteLine("HELP:\n    Load files into the workset.\n");
                    Console.WriteLine("    load-file [-r] [-t [file-type1 file-type2...]] [-f [filename]] [-?|help]");
                    Console.WriteLine("    Cmdlet alias: load-image, load\n");
                    Console.WriteLine("    -r\tRecursive load file.");
                    Console.WriteLine("    -t\tTypes of files will be loaded.");
                    Console.WriteLine("    -f\tSpecial file will be loaded.");
                    Console.WriteLine("    -?\tHelp of the cmdlet.");
                    Console.WriteLine("    -help\tHelp of the cmdlet.\n");
                    return;
                }
                else if (string.IsNullOrEmpty(lcArg))
                {
                    // use default file types
                }
                else
                {
                    Console.WriteLine("Invalid parameters.\n");
                    return;
                }
            }

            #endregion

            #region Loal Files

            int count = 0;
            if (FSOpt.IsDirectory(file))
            {
                string[] files;
                foreach (string ext in imgExts)
                {
                    files = Directory.GetFiles(file, "*." + ext, so);
                    count = files.Length;
                    foreach (string fs in files)
                    {
                        if (images.FindIndex(f => f.ToLower() == fs.ToLower()) == -1)
                        {
                            images.Add(fs);
                        }
                    }
                }
            }
            else if (FSOpt.IsFile(file))
            {
                if (imgExts.Contains(Path.GetExtension(file).Replace(".", "")))
                {
                    if (images.FindIndex(f => f.ToLower() == file.ToLower()) == -1)
                    {
                        count = 1;
                        images.Add(file);
                    }
                }
            }

            #endregion

            Console.WriteLine(string.Format("{0} image{2} loaded. Totally loaded {1} image{3}.\n", count, images.Count, (count > 1 ? "s" : ""), (images.Count > 1 ? "s" : "")));
        }

        internal static void Release()
        {
            Console.WriteLine(string.Format("{0} image{1} released.\n", images.Count, (images.Count > 1 ? "s" : "")));

            images.Clear();
            images = null;
            GC.Collect();
            images = new List<string>();
        }

        internal static void List()
        {
            if (images.Count > 0)
            {
                foreach (string file in images)
                {
                    Console.WriteLine(file);
                }
            }
            else
            {
                Console.WriteLine("No image loaded.");
            }

            Console.WriteLine();
        }

        public static List<string> Images { get { return images; } }

        public static string WorkingDirectory { get; private set; }
    }
}
