using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using Color = System.Drawing.Color;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.Xml;
using Microsoft.Maui.Controls;
using Application = System.Net.Mime.MediaTypeNames.Application;
namespace MAUIAPP
{
    public partial class Coords : ContentPage
    {
        public static Dictionary<Color, string> colorsDictionary = new Dictionary<Color, string>();
        public static string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public string configPath = Path.Combine(exeDirectory, "serverpath.json");
        public Process mcServerProc;

        public int xCoord;
        public int yCoord;
        public int zCoord; 
        public Coords()
        {
            InitializeComponent();
        }

        private void submitButton_Clicked(object sender, EventArgs e)
        {
             string jso = File.ReadAllText(configPath);
             dynamic confi = JsonConvert.DeserializeObject(jso);
             string imgPath = confi["imgPath"];
             string imagePath = Path.Combine(mcServerProc.StartInfo.WorkingDirectory, imgPath, "MineCraftImage.png");
            try
            {
                Debug.WriteLine("Reading image file...");

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
                    Debug.WriteLine("Original image dimensions: " + width + " x " + height);
                    Debug.WriteLine("Compressed image dimensions: " + newWidth + " x " + newHeight);
                    Bitmap compressedImage = new Bitmap(newWidth, newHeight);

                    using (Graphics g = Graphics.FromImage(compressedImage))
                    {
                        // Draw the compressed image onto the graphics object
                        g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));
                    }
                    image = compressedImage;
                }



                /*Previous enter commands were here
                 */


                //renderImage(mcServerProc.StandardInput, image, X, Y, Z); // Move renderImage call inside the try block


                Debug.WriteLine("Rendering image in the game...");

                renderImage(mcServerProc.StandardInput, image, xCoord, yCoord, zCoord);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("File not found: " + ex.Message);
            }
        }

        public void StartServer()
        {
            Debug.WriteLine("exedir: " + exeDirectory);
            Debug.WriteLine("configPath: " + configPath);
            Debug.WriteLine("Starting Minecraft_Drawer application...");

            mcServerProc = new Process();

            Debug.WriteLine("Creating process for Minecraft server...");


            if (!File.Exists(configPath))
            {
                Debug.WriteLine("Error: Configuration file not found.");
                return;
            }

            try
            {
                Debug.WriteLine("Reading configuration file...");

                string json = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(json);

                string serverPath = config["configPath"];

                // path to your minecraft server
                Debug.WriteLine("Server path: " + serverPath);
                mcServerProc.StartInfo.WorkingDirectory = serverPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading configuration file: " + ex.ToString());
                return;
            }

            Debug.WriteLine("Preparing to start Minecraft server process...");

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

            Debug.WriteLine("Starting Minecraft server process...");

            Debug.WriteLine(mcServerProc.Start());

            // starts reading from stdout & stderr
            mcServerProc.BeginOutputReadLine();
            Debug.WriteLine("OutputReadLine");
            mcServerProc.BeginErrorReadLine();
            Debug.WriteLine("errReadLine");

            // writes to the command prompt (cmd.exe) the line to execute the jar file (start the server)
            mcServerProc.StandardInput.WriteLine("java -Xmx2g -Xms1024M -jar server.jar nogui");

            Debug.WriteLine("Waiting for server to start (15 seconds)...");

            // adds available blocks and their colors to the dictionary
            populateDictionary(); 

            // renders an image in the game

        }


            //TELL USER SERVER IS READY TO JOIN--------------------------------------------

            
        static void populateDictionary()
        {
            //wool
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

            //concrete
            colorsDictionary.Add(Color.FromArgb(99, 31, 154), "purple_concrete");
            colorsDictionary.Add(Color.FromArgb(84, 28, 124), "black_concrete");
            colorsDictionary.Add(Color.FromArgb(39, 44, 140), "blue_concrete");
            colorsDictionary.Add(Color.FromArgb(68, 91, 28), "green_concrete");
            colorsDictionary.Add(Color.FromArgb(242, 177, 22), "yellow_concrete");
            colorsDictionary.Add(Color.FromArgb(139, 27, 27), "red_concrete");
            colorsDictionary.Add(Color.FromArgb(219, 92, 4), "orange_concrete");
            colorsDictionary.Add(Color.FromArgb(164, 172, 172), "white_concrete");
        }


        

        public void renderImage(StreamWriter stdin, Image img, int X, int Y, int Z)
        {
            Bitmap bmp = (Bitmap)img;
            Debug.WriteLine("Enter + or - for determining Z axis. (- across Z, + across X)");
            string userInput = "-";

           
                if (userInput.Equals("+"))
                {
                    for (int i = bmp.Height - 1; i > 0; i--)
                    {
                        for (int j = 0; j < bmp.Width; j++)
                        {

                            string cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z, "replace");
                            Debug.WriteLine(cmdTemplate);

                            int bestColorIndex = approximateColor(bmp.GetPixel(j, i));
                            stdin.WriteLine(cmdTemplate + colorsDictionary.ElementAt(bestColorIndex).Value);

                            X++;

                        }
                        Y++;
                        X -= bmp.Width;
                    }
                }
                else if (userInput.Equals("-"))
                {
                    for (int i = bmp.Height - 1; i > 0; i--)
                    {
                        for (int j = 0; j < bmp.Width; j++)
                        {

                            string cmdTemplate = String.Format("/setblock {0} {1} {2} ", X, Y, Z, "replace");

                            int bestColorIndex = approximateColor(bmp.GetPixel(j, i));
                            stdin.WriteLine(cmdTemplate + colorsDictionary.ElementAt(bestColorIndex).Value);

                            Z++;

                        }
                        Y++;
                        Z -= bmp.Width;
                    }
                }
                
            
        }

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

        private void entryx_Completed(object sender, EventArgs e)
        {
            
            if (int.TryParse(entryx.Text, out xCoord))
            {
                // Parsing successful, number variable now holds the parsed integer value
                Debug.WriteLine("Parsed number: " + xCoord);
            }
            else
            {
                // Parsing failed, userInput is not a valid integer
                Debug.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        private void entryy_Completed(object sender, EventArgs e)
        {
            if (int.TryParse(entryy.Text, out yCoord))
            {
                // Parsing successful, number variable now holds the parsed integer value
                Debug.WriteLine("Parsed number: " + yCoord);
            }
            else
            {
                // Parsing failed, userInput is not a valid integer
                Debug.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        private void entryz_Completed(object sender, EventArgs e)
        {
            if (int.TryParse(entryz.Text, out zCoord))
            {
                // Parsing successful, number variable now holds the parsed integer value
                Debug.WriteLine("Parsed number: " + zCoord);
            }
            else
            {
                // Parsing failed, userInput is not a valid integer
                Debug.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        private void quitButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Stopping the server...");
            mcServerProc.StandardInput.WriteLine("stop"); // stops the server
            Debug.WriteLine("Stopped the server!");

        }

        private void startServerButton_Clicked(object sender, EventArgs e)
        {
            StartServer();
        }
    }
}

