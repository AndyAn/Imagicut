using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Imagicut
{
    class Program
    {
        private static List<string> cmdList = new List<string>();
        private static int cmdIndex = -1;
        private static string IDENTIFIER_STRING = "]$ "; //string.Format("[{0}@{1}] > ", Environment.UserName, Environment.UserDomainName);

        static void Main(string[] args)
        {
            string command = "";

            Console.Title = AssemblyInfo.Title;
            Console.CursorSize = 90;
            Console.TreatControlCAsInput = true;
            Console.SetWindowSize(Console.WindowWidth * 2, (int)(Console.WindowHeight * 1.8f));
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight * 10);
            Console.SetWindowPosition(0, 0);

            Console.WriteLine(string.Format("{0} [Version {1}]", AssemblyInfo.Title, AssemblyInfo.Version));
            Console.WriteLine(string.Format("{0}  {1} {2}.", AssemblyInfo.Copyright, AssemblyInfo.Company, AssemblyInfo.Trademark));

            DataPool.SetPath(new string[] { "-p " + AssemblyInfo.WorkingDirectory });
            //tempPath = DataPool.WorkingDirectory;

            while (ParseCommand(command))
            {
                Console.Write(IDENTIFIER_STRING);
                command = ReadLine().Trim();
            }
        }

        private static Point GetPosition()
        {
            return new Point(Console.CursorLeft, Console.CursorTop);
        }

        #region Parse a Commandline

        private static bool ParseCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                string[] args = GetArgs(command);

                switch (GetCmdLet(command))
                {
                    case "set-path":
                    case "path":
                    case "pwd":
                        DataPool.SetPath(args);
                        break;
                    case "load-image":
                    case "load-file":
                    case "load":
                        DataPool.Load(args);
                        break;
                    case "list-image":
                    case "list-file":
                    case "list":
                        DataPool.List();
                        break;
                    case "release-memory":
                    case "free":
                        DataPool.Release();
                        break;
                    case "crop-image":
                    case "crop":
                        ImageManager.Crop(args);
                        break;
                    case "resize-image":
                    case "resize":
                        ImageManager.Resize(args);
                        break;
                    case "add-watermark":
                    case "wmark":
                        ImageManager.SetWaterMark(args);
                        break;
                    case "replace-text":
                    case "replace":
                        ImageManager.Replace(args);
                        break;
                    case "help-man":
                    case "help":
                    case "?":
                        ImageManager.Help();
                        break;
                    case "clear-screen":
                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;
                    case "exit":
                    case "quit":
                        return false;
                    default:
                        Console.WriteLine(string.Format("\"{0}\" is not a recognized command.\n", GetCmdLet(command)));
                        break;
                }
            }

            //AppDomain.CurrentDomain.SetData();

            return true;
        }

        private static string[] GetArgs(string command)
        {
            string cmdlet = command;
            int index = cmdlet.IndexOf(" ");

            if (index > -1)
            {
                List<string> args = new List<string>();
                bool isQuote = false;
                int argStart = 0;

                for (int i = index; i < cmdlet.Length; i++)
                {
                    switch (cmdlet[i])
                    {
                        case '"':
                            isQuote = !isQuote;
                            break;
                        case '-':

                            if (argStart > 0 && !isQuote)
                            {
                                args.Add(cmdlet.Substring(argStart, i - argStart));
                                argStart = i;
                            }
                            else if (!isQuote)
                            {
                                argStart = i;
                            }
                            break;
                    }

                    if (i == cmdlet.Length - 1)
                    {
                        args.Add(cmdlet.Substring(argStart, cmdlet.Length - argStart));
                    }
                }
                return args.ToArray();
            }
            else
            {
                return new string[] { "" };
            }
        }

        private static string GetCmdLet(string command)
        {
            string cmdlet = command.ToLower();
            int index = command.IndexOf(" ");

            return index > -1 ? cmdlet.Substring(0, index) : cmdlet;
        }

        #endregion

        #region Read a Commandline

        private static string GetPath(string line)
        {
            string[] args = GetArgs(line);
            string lastArg = args.Last().Trim();

            if (args.Length == 1 && lastArg.Trim().Length == 0) return null;

            int index = lastArg.IndexOf(' ');

            string path = (index == -1 ? lastArg.Substring(2) : lastArg.Substring(index + 1)).Replace("\"", "").Trim(new char[] { '"' });
            path = string.IsNullOrEmpty(path) ? DataPool.WorkingDirectory : path;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(DataPool.WorkingDirectory, path);
            }

            return path;
        }

        private static string GetNonPath(string line)
        {
            string lastArg = GetArgs(line).Last().Trim();
            int index = lastArg.IndexOf(' ');

            string path = (index == -1 ? lastArg.Substring(2) : lastArg.Substring(index + 1));

            return line.Substring(0, line.Length - path.Length);
        }

        private static string ReadLine()
        {
            ConsoleKey mode = ConsoleKey.NoName;
            ConsoleKeyInfo key;
            Point curPos = GetPosition();
            string line = "";
            string tempPath = "";
            List<string> pathList = new List<string>();
            int pathIndex = 0;
            bool isFirstTabPress = true;

            while (mode != ConsoleKey.Enter)
            {
                key = Console.ReadKey(true);
                mode = key.Key;

                switch (mode)
                {
                    case ConsoleKey.Escape:
                        #region Escape => CTRL + C
                        Console.WriteLine("\n");
                        line = "";
                        Console.Write(IDENTIFIER_STRING);
                        curPos.Y = Console.CursorTop;
                        break;
                        #endregion
                    case ConsoleKey.Tab:
                        #region Tab Key
                        if (isFirstTabPress)
                        {
                            tempPath = GetPath(line.Trim());
                            pathIndex = 0;
                            isFirstTabPress = false;
                        }

                        if (string.IsNullOrEmpty(tempPath)) break;

                        string dir = tempPath == DataPool.WorkingDirectory ? tempPath : Path.GetDirectoryName(tempPath);
                        if (string.IsNullOrEmpty(dir))
                        {
                            dir = tempPath;
                        }

                        // get command without path
                        string lineWithOutPath = GetNonPath(line);

                        pathList = FSOpt.GetSubItems(dir);

                        string[] files = pathList.Where(p => p.StartsWith(tempPath)).ToArray();
                        if (files.Length > 0)
                        {
                            pathList = files.ToList();
                        }
                        else
                        {
                            break;
                        }

                        dir = pathList[pathIndex++].Replace(DataPool.WorkingDirectory, "").TrimStart(new char[] { '\\' });
                        line = string.Format(dir.IndexOfAny(new char[] { ' ', '-' }) > -1 ? "{0}\"{1}\"" : "{0}{1}", lineWithOutPath, dir);
                        pathIndex = pathIndex == pathList.Count ? 0 : pathIndex;
                        Console.SetCursorPosition(curPos.X, curPos.Y);
                        Console.Write(new string(' ', Console.WindowWidth * 3));
                        Console.SetCursorPosition(curPos.X, curPos.Y);
                        Console.Write(line);
                        break;
                        #endregion
                    case ConsoleKey.Backspace:
                        #region Backspace Key
                        if (line.Length > 0)
                        {
                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(new string(' ', line.Length));
                            line = line.Remove(line.Length - 1, 1);
                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(line);

                            isFirstTabPress = true;
                        }
                        break;
                        #endregion
                    case ConsoleKey.Delete:
                        break;
                    case ConsoleKey.UpArrow:
                        #region UpArrow Key
                        if (cmdList.Count > 0)
                        {
                            cmdIndex -= (cmdIndex == 0 ? 0 : 1);

                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(new string(' ', cmdList[cmdIndex + (cmdIndex == cmdList.Count - 1 ? 0 : 1)].Length));
                            line = cmdList[cmdIndex];
                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(line);

                            isFirstTabPress = true;
                        }
                        break;
                        #endregion
                    case ConsoleKey.DownArrow:
                        #region DownArrow Key
                        if (cmdList.Count > 0)
                        {
                            cmdIndex += (cmdIndex == cmdList.Count - 1 ? 0 : 1);

                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(new string(' ', cmdList[cmdIndex - (cmdIndex == 0 ? 0 : 1)].Length));
                            line = cmdList[cmdIndex];
                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(line);

                            isFirstTabPress = true;
                        }
                        break;
                        #endregion
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.Home:
                    case ConsoleKey.End:
                        break;
                    case ConsoleKey.C:
                        #region CTRL + C
                        if (key.Modifiers == ConsoleModifiers.Control)
                        {
                            Console.WriteLine("\n");

                            line = "";
                            Console.Write(IDENTIFIER_STRING);
                            curPos.Y = Console.CursorTop;
                        }
                        else
                        {
                            line += key.KeyChar;
                            Console.SetCursorPosition(curPos.X, curPos.Y);
                            Console.Write(line);
                        }
                        break;
                        #endregion
                    default:
                        #region Other Keys
                        line += key.KeyChar < 32 ? "" : key.KeyChar.ToString();
                        Console.SetCursorPosition(curPos.X, curPos.Y);
                        Console.Write(line);

                        isFirstTabPress = true;
                        break;
                        #endregion
                }
            }

            curPos.Y++;
            Console.WriteLine();

            cmdList.Add(line.Trim());
            cmdIndex = cmdList.Count;

            return line;
        }

        #endregion
    }
}
