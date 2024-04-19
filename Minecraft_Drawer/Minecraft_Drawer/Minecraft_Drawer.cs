using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;


namespace Minecraft_Drawer
{
    class Minecraft_Drawer
    {
        
        static Dictionary<Color, string> colorsDictionary = new Dictionary<Color, string>();

        static void populateDictionary()
        {
            colorsDictionary.Add(Color.FromArgb(238, 238, 238), "white_wool");
            colorsDictionary.Add(Color.FromArgb(235, 131, 60), "orange_wool");
            colorsDictionary.Add(Color.FromArgb(184, 56, 195), "magenta_wool");
            colorsDictionary.Add(Color.FromArgb(111, 144, 214), "light_blue_wool");
            colorsDictionary.Add(Color.FromArgb(222, 207, 42), "yellow_wool");
            colorsDictionary.Add(Color.FromArgb(60, 195, 48), "lime_wool");
            colorsDictionary.Add(Color.FromArgb(219, 138, 160), "pink_wool");
            colorsDictionary.Add(Color.FromArgb(76, 76, 76), "gray_wool");
            colorsDictionary.Add(Color.FromArgb(163, 170, 170), "light_gray_wool");
            colorsDictionary.Add(Color.FromArgb(45, 134, 172), "cyan_wool");
            colorsDictionary.Add(Color.FromArgb(148, 77, 210), "purple_wool");
            colorsDictionary.Add(Color.FromArgb(45, 59, 178), "blue_wool");
            colorsDictionary.Add(Color.FromArgb(90, 54, 29), "brown_wool");
            colorsDictionary.Add(Color.FromArgb(64, 89, 28), "green_wool");
            colorsDictionary.Add(Color.FromArgb(171, 47, 42), "red_wool");
            colorsDictionary.Add(Color.FromArgb(14, 14, 14), "black_wool");
        }

        // gets the closest available color (decides what block color should be used for a pixel)
        static int approximateColor(Color pixelColor)
        {
            double minError = 99999; double currentError = 0;
            int bestColorIndex = 0;

            for (int i = 0; i < colorsDictionary.Count; i++)
            {
                Color blockColor = colorsDictionary.ElementAt(i).Key;

                // k nearest neighbor
                currentError = Math.Sqrt(Math.Pow(pixelColor.R - blockColor.R, 2) + Math.Pow(pixelColor.G - blockColor.G, 2) + Math.Pow(pixelColor.B - blockColor.B, 2));

                if (currentError < minError)
                {
                    minError = currentError;
                    bestColorIndex = i;
                }

            }

            return bestColorIndex;
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        private static int GetCompressionQuality(int imageHeight)
        {
            // Adjust the compression quality based on image height
            if (imageHeight <= 400)
            {
                return 50; // High quality for small images
            }
            else if (imageHeight <= 600)
            {
                return 25; // Medium quality for medium-sized images
            }
            else
            {
                return 12; // Lower quality for large images
            }
        }
       

        

        static void renderImage(StreamWriter stdin, Image img, int X, int Y, int Z)
        {
            Bitmap bmp = (Bitmap)img;
            bool incorrectInput = true;

            while (incorrectInput)
            {
                Console.WriteLine("Enter + or - for determining Z axis. (- across Z, + across X)");
                string userInput = Console.ReadLine();
                if (userInput.Equals("+"))
                {
                    incorrectInput = false;
                    for (int i = bmp.Height - 1; i > 0; i--)
                    {
                        for (int j = 0; j < bmp.Width; j++)
                        {
                            
                            string cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z, "replace");

                            int bestColorIndex = approximateColor(bmp.GetPixel(j, i));
                            stdin.WriteLine(cmdTemplate + colorsDictionary.ElementAt(bestColorIndex).Value);
                          /*  for (int k = 1; k <= 2; k++)
                            {
                                cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z+k, "destroy");
                                string airBlock = "air";
                                stdin.WriteLine(cmdTemplate + airBlock);
                                
                                cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z-k, "destroy");
                                airBlock = "air";
                                stdin.WriteLine(cmdTemplate + airBlock);
                                Thread.Sleep(1);
                            }*/
                            X++;

                        }
                        Y++;
                        X -= bmp.Width;
                    }
                }
                else if (userInput.Equals("-"))
                {
                    incorrectInput = false;
                    for (int i = bmp.Height - 1; i > 0; i--)
                    {
                        for (int j = 0; j < bmp.Width; j++)
                        {
                            
                            string cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z, "replace");

                            int bestColorIndex = approximateColor(bmp.GetPixel(j, i));
                            stdin.WriteLine(cmdTemplate + colorsDictionary.ElementAt(bestColorIndex).Value);
                           /* for (int k = 1; k <= 2; k++)
                            {
                                cmdTemplate = String.Format("/setblock {0} {1} {2} ", X + k, Y, Z, "destroy");
                                string airBlock = "air";
                                stdin.WriteLine(cmdTemplate + airBlock);
                                
                                cmdTemplate = String.Format("/setblock {0} {1} {2} ", X - k, Y, Z, "destroy");
                                airBlock = "air";
                                stdin.WriteLine(cmdTemplate + airBlock);
                                Thread.Sleep(1);
                            }*/
                            Z++;

                        }
                        Y++;
                        Z -= bmp.Width;
                    }
                }
                else
                {
                    incorrectInput = true;
                    Console.WriteLine("Please enter a +/-");
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Minecraft_Drawer application...");

            using (Process mcServerProc = new Process())
            {
                Console.WriteLine("Creating process for Minecraft server...");

                string configPath = @"serverpath.json";

                if (!File.Exists(configPath))
                {
                    Console.WriteLine("Error: Configuration file not found.");
                    return;
                }

                try
                {
                    Console.WriteLine("Reading configuration file...");

                    string json = File.ReadAllText(configPath);
                    dynamic config = JsonConvert.DeserializeObject(json);

                    string serverPath = config["configPath"];

                    // path to your minecraft server
                    Console.WriteLine("Server path: " + serverPath);
                    mcServerProc.StartInfo.WorkingDirectory = serverPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading configuration file: " + ex.ToString());
                    return;
                }

                Console.WriteLine("Preparing to start Minecraft server process...");

                // path to cmd.exe
                mcServerProc.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                // bunch of settings, to allow redirection of stdin / stdout / stderr
                mcServerProc.StartInfo.UseShellExecute = false;
                mcServerProc.StartInfo.RedirectStandardInput = true;
                mcServerProc.StartInfo.RedirectStandardOutput = true;
                mcServerProc.StartInfo.RedirectStandardError = true;
                mcServerProc.StartInfo.CreateNoWindow = true;

                // displaying stderr and stdout in our console
                mcServerProc.OutputDataReceived += (sender, e) => { Console.WriteLine(e.Data); };
                mcServerProc.ErrorDataReceived += (sender, e) => { Console.WriteLine(e.Data); };

                Console.WriteLine("Starting Minecraft server process...");

                mcServerProc.Start();

                // starts reading from stdout & stderr
                mcServerProc.BeginOutputReadLine();
                mcServerProc.BeginErrorReadLine();

                // writes to the command prompt (cmd.exe) the line to execute the jar file (start the server)
                mcServerProc.StandardInput.WriteLine("java -Xmx2g -Xms1024M -jar server.jar nogui");

                Console.WriteLine("Waiting for server to start (15 seconds)...");

                Thread.Sleep(15000); // waiting for server to start

                // adds available blocks and their colors to the dictionary
                populateDictionary();

                // renders an image in the game
                string imagePath = Path.Combine(mcServerProc.StartInfo.WorkingDirectory, "images", "MineCraftImage.png");

                // keeps the command prompt alive until you type 'stop'
                // otherwise, this will close and the server keeps running
                while (true)
                {
                    Console.WriteLine("Waiting for user input...");

                    string userInput = Console.ReadLine();

                    if (userInput.Contains("retry"))
                    {
                        try
                        {
                            Console.WriteLine("Reading image file...");

                            Image image = Image.FromFile(imagePath);
                            int height = image.Height;
                            int width = image.Width;
                            int maxHeight = 300;
                            int maxWidth = 300;

                            if (height > maxHeight || image.Width > maxWidth)
                            {
                                double widthRatio = (double)maxWidth / image.Width;
                                double heightRatio = (double)maxHeight / height;
                                double ratio = Math.Min(widthRatio, heightRatio);

                                int newWidth = (int)(image.Width * ratio);
                                int newHeight = (int)(height * ratio);
                                Console.WriteLine("Original image dimensions: " + width + " x " + height);
                                Console.WriteLine("Compressed image dimensions: " + newWidth + " x " + newHeight);
                                Bitmap compressedImage = new Bitmap(newWidth, newHeight);

                                using (Graphics g = Graphics.FromImage(compressedImage))
                                {
                                    // Draw the compressed image onto the graphics object
                                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
                                }
                                image = compressedImage;
                            }
                            Console.WriteLine("Enter X coordinate");
                            userInput = Console.ReadLine();
                            int X;
                            if (int.TryParse(userInput, out X))
                            {
                                // Parsing successful, number variable now holds the parsed integer value
                                Console.WriteLine("Parsed number: " + X);
                            }
                            else
                            {
                                // Parsing failed, userInput is not a valid integer
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                            Console.WriteLine("Enter Y coordinate");
                            userInput = Console.ReadLine();
                            int Y;
                            if (int.TryParse(userInput, out Y))
                            {
                                // Parsing successful, number variable now holds the parsed integer value
                                Console.WriteLine("Parsed number: " + Y);
                            }
                            else
                            {
                                // Parsing failed, userInput is not a valid integer
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                            Console.WriteLine("Enter Z coordinate");
                            int Z;
                            if (int.TryParse(userInput, out Z))
                            {
                                // Parsing successful, number variable now holds the parsed integer value
                                Console.WriteLine("Parsed number: " + Z);
                            }
                            else
                            {
                                // Parsing failed, userInput is not a valid integer
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                            userInput = Console.ReadLine();
                            renderImage(mcServerProc.StandardInput, image, X, Y, Z); // Move renderImage call inside the try block


                            Console.WriteLine("Rendering image in the game...");

                            renderImage(mcServerProc.StandardInput, image, X, Y, Z);
                        }
                        catch (FileNotFoundException ex)
                        {
                            Console.WriteLine("File not found: " + ex.Message);
                        }
                    }
                    else if (userInput.Contains("stop"))
                    {
                        Console.WriteLine("Stopping the server...");
                        mcServerProc.StandardInput.WriteLine("stop"); // stops the server
                        break;
                    }
                }

                Console.WriteLine("Exiting Minecraft_Drawer application...");
            }
        }
    }
}