using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Drawing.Imaging;
using Newtonsoft.Json;


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
                return 95; // High quality for small images
            }
            else if (imageHeight <= 600)
            {
                return 75; // Medium quality for medium-sized images
            }
            else
            {
                return 50; // Lower quality for large images
            }
        }

        static void renderImage(StreamWriter stdin, Image img)
        {
            Bitmap bmp = (Bitmap)img;
            
            // coordinates - from where to start rendering the image
            int X = 5;
            int Y = 84;
            int Z = 7;

            for (int i = bmp.Height - 1; i > 0; i--)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    string cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z, "replace");

                    int bestColorIndex = approximateColor(bmp.GetPixel(j, i));
                    stdin.WriteLine(cmdTemplate + colorsDictionary.ElementAt(bestColorIndex).Value);
                    X++;

                }
                Y++;
                X -= bmp.Width;

            }
        }

        static void Main(string[] args)
        {
            using (Process mcServerProc = new Process())
            {
                string configPath = "serverpath.json";

                if (!File.Exists(configPath))
                {
                    Console.WriteLine("Error: Configuration file not found.");
                    return;
                }

                try
                {
                    string json = File.ReadAllText(configPath);
                    dynamic config = JsonConvert.DeserializeObject(json);

                    string serverPath = config["configPath"];

                    // path to your minecraft server
                    Console.WriteLine("Server path: " + serverPath);
                    mcServerProc.StartInfo.WorkingDirectory = serverPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading configuration file: " + ex.Message);

                }
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

                mcServerProc.Start();

                // starts reading form stdout & stderr
                mcServerProc.BeginOutputReadLine();
                mcServerProc.BeginErrorReadLine();

                // writes to the command prompt (cm d.exe) the line to execute the jar file (start the server)
                mcServerProc.StandardInput.WriteLine("java -Xmx1024M -Xms1024M -jar server.jar nogui");

                Thread.Sleep(15000); // waiting for server to start

                // adds available blocks and their colors to the dictionary
                populateDictionary();

                // renders an image in the game
                string imagePath = Path.Combine(mcServerProc.StartInfo.WorkingDirectory, "images", "ed.jpg");
               /* try
                {
                     
                    Image image = Image.FromFile(imagePath);
                    renderImage(mcServerProc.StandardInput, image); // Move renderImage call inside the try block
                }
                catch (FileNotFoundException ex)
                {
                    // Handle the exception (e.g., log the error, display an error message)
                    Console.WriteLine("File not found: " + ex.Message);
                }
                */
                // keeps the command prompt alive until you type 'stop'
                // otherwise this will close and the server keeps running
                while (true)
                {
                    if (Console.ReadLine().Contains("retry")) { 
                        try
                        {

                        Image image = Image.FromFile(imagePath);
                            int height = image.Height;
                            if (height > 256)
                            {
                                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                Encoder myEncoder = Encoder.Quality;

                                // Create an EncoderParameters object
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                                // Set the compression level (0-100)
                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L); // Adjust the quality level as needed
                                myEncoderParameters.Param[0] = myEncoderParameter;

                                Bitmap compressedImage = new Bitmap(image.Width, image.Height);

                                using (Graphics g = Graphics.FromImage(compressedImage))
                                {
                                    // Draw the compressed image onto the graphics object
                                    g.DrawImage(image, new Rectangle(0, 0, compressedImage.Width, compressedImage.Height));
                                }
                                image = compressedImage;
                            }
                                renderImage(mcServerProc.StandardInput, image); // Move renderImage call inside the try block
                         }
                        catch (FileNotFoundException ex)
                        {
                        // Handle the exception (e.g., log the error, display an error message)
                        Console.WriteLine("File not found: " + ex.Message);
                        }
                    }
                    if (Console.ReadLine().Contains("stop"))
                        break;
                }

                mcServerProc.StandardInput.WriteLine("stop"); // stops the server

                Thread.Sleep(3000);
            }
        }
    }
}