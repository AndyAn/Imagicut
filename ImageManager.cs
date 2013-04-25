using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace Imagicut
{
    class ImageManager
    {
        /// <summary>
        /// <para>crop-image -?</para>
        /// <para>crop-image -help</para>
        /// <para>crop-image -padding 32  #left = top = right = bottom = 32</para>
        /// <para>crop-image -padding 32,24  #left = right = 32, top = bottom = 24</para>
        /// <para>crop-image -padding 32,24,32,24  #left = 32, top = 24, right = 32, bottom = 24</para>
        /// </summary>
        /// <param name="args"></param>
        internal static void Crop(string[] args)
        {
            Padding padding = new Padding() { Left = 0, Top = 0, Right = 0, Bottom = 0 };
            string lcArg = "";

            #region Process arguments

            foreach (string arg in args)
            {
                lcArg = arg.ToLower();

                if (lcArg.StartsWith("-p"))
                {
                    // set width
                    string[] p = lcArg.Substring(2).Trim().Split(new char[] { ',', ' ' });

                    if (p.Length == 0 || p.Length == 3 || p.Length > 4 || new Regex("[^\\d]", RegexOptions.IgnoreCase).IsMatch(string.Join("", p)))
                    {
                        Console.WriteLine("Invalid parameters of padding.\n");
                        return;
                    }
                    else if (p.Length == 1)
                    {
                        padding.Left = padding.Top = padding.Right = padding.Bottom = int.Parse(p[0]);
                    }
                    else if (p.Length == 2)
                    {
                        padding.Left = padding.Right = int.Parse(p[0]);
                        padding.Top = padding.Bottom = int.Parse(p[1]);
                    }
                    else if (p.Length == 4)
                    {
                        padding.Left = int.Parse(p[0]);
                        padding.Top = int.Parse(p[1]);
                        padding.Right = int.Parse(p[2]);
                        padding.Bottom = int.Parse(p[3]);
                    }
                }
                else if (lcArg.Equals("-?") || lcArg.Equals("-help"))
                {
                    Console.WriteLine("HELP:\n    Crop loaded image with specified padding.\n");
                    Console.WriteLine("    crop-image [-p [padding value]] [-?|help]");
                    Console.WriteLine("    Cmdlet alias: crop\n");
                    Console.WriteLine("    -p\tPadding that will be cut off from the loaded images.\n\tThere are three pattern:");
                    Console.WriteLine("\t    - A parameter, the padding of Left / Top / Right / Bottom will");
                    Console.WriteLine("\t      share the value.");
                    Console.WriteLine("\t    - Two parameters, the padding of Left & Right will share the first");
                    Console.WriteLine("\t      parameter. Top & Bottom will share the second parameter.");
                    Console.WriteLine("\t    - Four parameters, the padding of Left / Top / Right / Right will");
                    Console.WriteLine("\t      use separated parameters.");
                    Console.WriteLine("    -?\tHelp of the cmdlet.");
                    Console.WriteLine("    -help\tHelp of the cmdlet.\n");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid parameters.\n");
                    return;
                }
            }

            #endregion

            #region Crop Images

            string output = Path.Combine(DataPool.WorkingDirectory, "output");
            FSOpt.CreateDirectory(output);
            Console.WriteLine();

            int max = DataPool.Images.Count;
            int cur = 0;

            foreach (string file in DataPool.Images)
            {
                cur++;

                // crop image
                Image image = Image.FromFile(file);
                Image bitmap = new Bitmap(image.Width - padding.Left - padding.Right, image.Height - padding.Top - padding.Bottom);

                Graphics g = Graphics.FromImage(bitmap);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.Clear(Color.White);
                g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(padding.Left, padding.Top, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);

                image.Dispose();
                image = null;
                g.Dispose();
                g = null;
                
                bitmap.Save(Path.Combine(output, Path.GetFileName(file)), System.Drawing.Imaging.ImageFormat.Jpeg);
                bitmap.Dispose();
                bitmap = null;

                ProgressStatus(cur, max);
                GC.Collect();
            }

            Console.WriteLine(string.Format("{0} images are processed.\n", max));

            #endregion
        }

        internal static void Resize(string[] args)
        {
            bool isScale = true;
            int width = 0;
            int height = 0;
            string lcArg = "";

            #region Process arguments

            foreach (string arg in args)
            {
                lcArg = arg.ToLower();

                if (lcArg.StartsWith("-m"))
                {
                    // set mode as "scale" or "noscale", default is "scale"
                    isScale = (lcArg.Substring(2).Trim() == "scale");
                }
                else if (lcArg.StartsWith("-w"))
                {
                    // set width
                    int.TryParse(lcArg.Substring(2).Trim(), out width);
                }
                else if (lcArg.StartsWith("-h"))
                {
                    // set height
                    int.TryParse(lcArg.Substring(2).Trim(), out height);
                }
                else if (lcArg.Equals("-?") || lcArg.Equals("-help"))
                {
                    Console.WriteLine("HELP:\n    Resize loaded image with specified width and height.\n");
                    Console.WriteLine("    resize-image [-m [resize mode]] -w [width] -h [height] [-?|help]");
                    Console.WriteLine("    Cmdlet alias: resize\n");
                    Console.WriteLine("    -m\tResize mode, scale or noscale. If use noscale mode, image will be");
                    Console.WriteLine("    \tresized to the given width and height (squished).");
                    Console.WriteLine("    -w\tWidth that the image will be resized to.");
                    Console.WriteLine("    -h\tHeight that the image will be resized to");
                    Console.WriteLine("    -?\tHelp of the cmdlet.");
                    Console.WriteLine("    -help\tHelp of the cmdlet.\n");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid parameters.\n");
                    return;
                }
            }

            if (width == 0 || height == 0)
            {
                Console.WriteLine(string.Format("Parameter \"{0}{1}{2}\" {3} wrong format or not set.\n", 
                                                (width == 0 ? "Width" : ""),
                                                (height == 0 && width == 0 ? "\" & \"" : ""),
                                                (height == 0 ? "Height" : ""),
                                                (height == 0 && width == 0 ? "are" : "is")));
                return;
            }

            #endregion

            #region Resize Images

            string output = Path.Combine(DataPool.WorkingDirectory, "output");
            FSOpt.CreateDirectory(output);
            Console.WriteLine();

            int max = DataPool.Images.Count;
            int cur = 0;
            Size size = new Size();

            foreach (string file in DataPool.Images)
            {
                cur++;

                Image image = Image.FromFile(file);
                if (isScale)
                {
                    if ((double)image.Width / image.Height > (double)width / height)
                    {
                        size.Width = width;
                        size.Height = (width * image.Height) / image.Width;
                    }
                    else
                    {
                        size.Height = height;
                        size.Width = (height * image.Width) / image.Height;
                    }
                }
                else
                {
                    size.Width = width;
                    size.Height = height;
                }

                // resize image
                Image bitmap = new Bitmap(size.Width, size.Height);

                Graphics g = Graphics.FromImage(bitmap);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.Clear(Color.White);
                g.DrawImage(image, new Rectangle(0, 0, size.Width, size.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

                image.Dispose();
                image = null;

                g.Dispose();
                g = null;

                bitmap.Save(Path.Combine(output, Path.GetFileName(file)), System.Drawing.Imaging.ImageFormat.Jpeg);
                bitmap.Dispose();
                bitmap = null;

                ProgressStatus(cur, max);
                GC.Collect();
            }

            Console.WriteLine(string.Format("{0} images are processed.\n", max));

            #endregion
        }

        internal static void Replace(string[] args)
        {
            string lcArg = "";
            string oldString = "";
            string newString = "";

            #region Process Parameters

            foreach (string arg in args)
            {
                lcArg = arg.ToLower();

                if (lcArg.StartsWith("-f"))
                {
                    oldString = lcArg.Substring(2).Trim().Replace("\"", "");
                }
                else if (lcArg.StartsWith("-r"))
                {
                    newString = lcArg.Substring(2).Trim().Replace("\"", "");
                }
                else if (lcArg.Equals("-?") || lcArg.Equals("-help") || string.IsNullOrEmpty(lcArg))
                {
                    Console.WriteLine("HELP:\n    Replace file content with given search-string and replace-string.\n");
                    Console.WriteLine("    replace-text [-find [string]] [-replace [string]] [-?|help]");
                    Console.WriteLine("    Cmdlet alias: replace\n");
                    Console.WriteLine("    -f\tFind-string need to be found.");
                    Console.WriteLine("    -r\tReplace-string will be replaced.");
                    Console.WriteLine("    -?\tHelp of the cmdlet.");
                    Console.WriteLine("    -help\tHelp of the cmdlet.\n");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid parameters.\n");
                    return;
                }
            }

            #endregion

            #region Replace Files

            int max = DataPool.Images.Count;
            int cur = 0;

            foreach (string file in DataPool.Images)
            {
                cur++;

                FSOpt.FileReplace(file, oldString, newString);

                ProgressStatus(cur, max);
            }

            Console.WriteLine(string.Format("{0} files replaced.\n", max));

            #endregion
        }

        internal static void SetWaterMark(string[] args)
        {
            Padding margin = new Padding() { Left = 0, Top = 0, Right = 0, Bottom = 0 };
            string lcArg = "";
            string text = "";
            string imageFile = "";
            float transparent = 0.5f;
            string position = "bottom-right";

            #region Process arguments

            foreach (string arg in args)
            {
                lcArg = arg.ToLower();

                if (lcArg.StartsWith("-m"))
                {
                    // set margin
                    int temp = int.MinValue;
                    int.TryParse(lcArg.Substring(2).Trim(), out temp);

                    if (new Regex("[^\\d]", RegexOptions.IgnoreCase).IsMatch(lcArg.Substring(2).Trim()))
                    {
                        Console.WriteLine("Invalid parameters of padding.\n");
                        return;
                    }
                    else
                    {
                        margin.Left = margin.Top = margin.Right = margin.Bottom = temp;
                    }
                }
                else if (lcArg.StartsWith("-t"))
                {
                    // set watermark transparency
                    float.TryParse(lcArg.Substring(2).Trim(), out transparent);
                }
                else if (lcArg.StartsWith("-p"))
                {
                    // set watermark position
                    position = lcArg.Substring(2).Trim().Trim(new char[] { '"' });
                }
                else if (lcArg.StartsWith("-s"))
                {
                    // set mode as "text"
                    text = lcArg.Substring(2).Trim();
                }
                else if (lcArg.StartsWith("-f"))
                {
                    // set mode as "image"
                    imageFile = lcArg.Substring(2).Trim();
                    if (!Path.IsPathRooted(imageFile))
                    {
                        imageFile = Path.Combine(DataPool.WorkingDirectory, imageFile);
                    }
                }
                else if (lcArg.Equals("-?") || lcArg.Equals("-help"))
                {
                    Console.WriteLine("HELP:\n    Set watermark on loaded image, two choices provided: Text and Image.\n");
                    Console.WriteLine("    set-watermark [-t [text watermark]] [-f [filename]] [-m [margin value]] [-p [position]] [-?|help]");
                    Console.WriteLine("    Cmdlet alias: wmark\n");
                    Console.WriteLine("    -s\tText for setting watermark. Default is empty.");
                    Console.WriteLine("    -f\tImage file for setting watermark. This is priored parameter if set.");
                    Console.WriteLine("    -t\tSet transparency of matermark image. Value range is 0 to 1.");
                    Console.WriteLine("    \tDefault is 0.5.");
                    Console.WriteLine("    -m\tSet margin of image or text. This will be effected according to the");
                    Console.WriteLine("    \tdifferent position. Usually attached to the closest edge");
                    Console.WriteLine("    -p\tSet position of watermark. 9 positions in below:");
                    Console.WriteLine("\t    +-------------+----------+--------------+");
                    Console.WriteLine("\t    |  left-top   |   top    |   right-top  |");
                    Console.WriteLine("\t    +-------------+----------+--------------+");
                    Console.WriteLine("\t    |    left     |  center  |     right    |");
                    Console.WriteLine("\t    +-------------+----------+--------------+");
                    Console.WriteLine("\t    | left-bottom |  bottom  | right-bottom |");
                    Console.WriteLine("\t    +-------------+----------+--------------+");
                    Console.WriteLine("    \tDefault position is \"right-bottom\".");
                    Console.WriteLine("    -?\tHelp of the cmdlet.");
                    Console.WriteLine("    -help\tHelp of the cmdlet.\n");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid parameters.\n");
                    return;
                }
            }

            #endregion

            #region Set Watermark

            string output = Path.Combine(DataPool.WorkingDirectory, "output");
            int max = DataPool.Images.Count;
            int cur = 0;

            foreach (string file in DataPool.Images)
            {
                cur++;

                Image image = Image.FromFile(file);

                if (imageFile != "")
                {
                    #region Image Watermark

                    if (File.Exists(imageFile))
                    {
                        // get image file
                        using (Image wrImage = Image.FromFile(imageFile))
                        {
                            // check iamge - the original image must larger than watermark
                            if (image.Width >= wrImage.Width && image.Height >= wrImage.Height)
                            {
                                Graphics g = Graphics.FromImage(image);

                                // set transparency
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = { 
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  transparent, 0.0f},// set transparency to 0.5
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                PointF point = GetWaterMarkPosition(position, wrImage.Size, image.Size, margin);

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                g.DrawImage(wrImage, new Rectangle((int)point.X, (int)point.Y, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                g.Dispose();
                                g = null;
                            }
                            wrImage.Dispose();
                        }
                    }

                    #endregion
                }
                else if (!string.IsNullOrEmpty(text))
                {
                    #region Text Watermark

                    using (Graphics g = Graphics.FromImage(image))
                    {
                        Font fontWater = new Font("微软雅黑", 36);
                        Brush brushWater = new SolidBrush(Color.Violet);
                        SizeF size = g.MeasureString(text, fontWater);

                        PointF point = GetWaterMarkPosition(position, size.ToSize(), image.Size, margin);

                        g.DrawString(text, fontWater, brushWater, point);
                        g.Dispose();
                    }

                    #endregion
                }

                image.Save(Path.Combine(output, Path.GetFileName(file)), System.Drawing.Imaging.ImageFormat.Jpeg);
                image.Dispose();
                image = null;

                ProgressStatus(cur, max);
                GC.Collect();
            }

            Console.WriteLine(string.Format("{0} images are processed.\n", max));

            #endregion
        }

        private static PointF GetWaterMarkPosition(string position, Size wmSize, Size imageSize, Padding margin)
        {
            PointF point = new PointF();

            switch (position)
            {
                case "left":
                    point.X = margin.Left;
                    point.Y = ((float)imageSize.Height - (float)wmSize.Height) / 2;
                    break;
                case "left-top":
                    point.X = margin.Left;
                    point.Y = margin.Top;
                    break;
                case "top":
                    point.X = ((float)imageSize.Width - (float)wmSize.Width) / 2;
                    point.Y = margin.Top;
                    break;
                case "right-top":
                    point.X = imageSize.Width - wmSize.Width - margin.Right;
                    point.Y = margin.Top;
                    break;
                case "right":
                    point.X = imageSize.Width - wmSize.Width - margin.Right;
                    point.Y = ((float)imageSize.Height - (float)wmSize.Height) / 2;
                    break;
                case "bottom":
                    point.X = ((float)imageSize.Width - (float)wmSize.Width) / 2;
                    point.Y = imageSize.Height - wmSize.Height - margin.Bottom;
                    break;
                case "left-bottom":
                    point.X = margin.Left;
                    point.Y = imageSize.Height - wmSize.Height - margin.Bottom;
                    break;
                case "center":
                    point.X = ((float)imageSize.Width - (float)wmSize.Width) / 2;
                    point.Y = ((float)imageSize.Height - (float)wmSize.Height) / 2;
                    break;
                case "right-bottom":
                default:
                    point.X = imageSize.Width - wmSize.Width - margin.Right;
                    point.Y = imageSize.Height - wmSize.Height - margin.Bottom;
                    break;
            }

            return point;
        }

        private static void ProgressStatus(int current, int maxCount)
        {
            int scrWidth = Console.WindowWidth - 18;
            string progressBar = "Progress: {0}% [{1}]{2}";

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(string.Format(progressBar, (current * 100 / maxCount).ToString().PadLeft(3), new string('#', (current * scrWidth / maxCount)) + new string(' ', scrWidth - (current * scrWidth / maxCount)), (current == maxCount ? "\n" : "")));
        }

        internal static void Help()
        {
            Console.WriteLine(AssemblyInfo.Title + " on HELP:\n    Here are the cmdlets current supported:\n");
            Console.WriteLine("\tcmdlet: set-path\talias: path, pwd");
            Console.WriteLine("\tcmdlet: load-image\talias: load-file, load");
            Console.WriteLine("\tcmdlet: list-image\talias: list-file, list");
            Console.WriteLine("\tcmdlet: release-memory\talias: free");
            Console.WriteLine("\tcmdlet: crop-image\talias: crop");
            Console.WriteLine("\tcmdlet: resize-image\talias: resize");
            Console.WriteLine("\tcmdlet: add-watermark\talias: wmark");
            Console.WriteLine("\tcmdlet: replace-text\talias: replace");
            Console.WriteLine("\tcmdlet: help-man\talias: help, ?");
            Console.WriteLine("\tcmdlet: clear-screen\t\talias: clear, cls");
            Console.WriteLine("\tcmdlet: exit\t\talias: quit\n");
            Console.WriteLine("    Please use -? or -help to get more details of each cmdlets.\n");
        }
    }
}
