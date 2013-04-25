using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Imagicut
{
    internal static class FSOpt
    {
        public static bool IsDirectory(string filePath)
        {
            bool result = false;

            try
            {
                if (File.Exists(filePath))
                {
                    result = ((File.GetAttributes(filePath) | FileAttributes.Directory) == FileAttributes.Directory);
                }
                else
                {
                    result = (filePath.IndexOf(".", filePath.LastIndexOf(@"\")) == -1);
                }
            }
            catch { }

            return result;
        }

        public static bool IsFile(string filePath)
        {
            bool result = false;

            try
            {
                result = File.Exists(filePath);
            }
            catch { }


            return result;
        }

        public static void CreateDirectory(string directory)
        {
            string basePath = "";

            try
            {
                if (directory.StartsWith(@"\\"))
                {
                    // UNC Path
                    basePath = @"\\" + directory.Split('\\')[2];
                }
                else
                {
                    // Hard Disk Drive
                    basePath = directory.Split('\\')[0];
                }

                if (Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(directory);
                }
                else
                {
                    throw new DirectoryNotFoundException("Path root doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void FileReplace(string filePath, string original, string replacement)
        {
            string text = "";
            using (StreamReader sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();
            }

            text = text.Replace(original, replacement);

            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                sw.Write(text);
            }
        }

        internal static List<string> GetSubItems(string path)
        {
            List<string> files = new List<string>();

            if (File.Exists(path))
            {
                files.Add(path);
            }
            else if (Directory.Exists(path))
            {
                files.AddRange(Directory.GetFileSystemEntries(path, "*.*", SearchOption.TopDirectoryOnly));
            }
            else
            {
                try
                {
                    path = Path.GetFileName(path);
                    if (Directory.Exists(path))
                    {
                        files.AddRange(Directory.GetFileSystemEntries(path, "*.*", SearchOption.TopDirectoryOnly));
                    }
                }
                catch { }
            }

            return files;
        }
    }
}
