using Microsoft.Maui.Controls;
using OpenAI_API;
using OpenAI_API.Images;
using System.Drawing.Imaging;
using System.Reflection;
using System.Drawing;
using Microsoft.Maui.ApplicationModel;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection.Emit;
using Windows.System;
using Microsoft.Maui.Storage;
using Plugin.Maui.Audio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using Color = System.Drawing.Color;
using System.Xml;
using Microsoft.Maui.Controls;
using Application = System.Net.Mime.MediaTypeNames.Application;

namespace MAUIAPP
{   
    public partial class MainPage : ContentPage
    {
        bool isRunning = false;
        Process process = new Process();
        string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public String prompt;
        public MainPage()
        {
            prompt = "";
            InitializeComponent();
            btnPreview.IsEnabled = false;
            btnAddtoMinecraft.IsEnabled = true;
        }

        private void entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            prompt = promptInput.Text.Trim();
            btnPreview.IsEnabled = true;
        }

        private void entry_Completed(object sender, EventArgs e)
        {
            
        }

        private void btnAddtoMinecraft_Clicked(object sender, EventArgs e)
        {
            backButton.IsEnabled = true;
            backButton.IsVisible = true;            
            prevImage.IsVisible = false;
            lblTitle.IsVisible = false;
            promptInput.IsVisible = false;
            btnPreview.IsVisible = false;
            btnAddtoMinecraft.IsVisible = false;
            prevImage.IsEnabled = false;
            lblTitle.IsEnabled = false;
            promptInput.IsEnabled = false;
            btnPreview.IsEnabled = false;
            btnAddtoMinecraft.IsEnabled = false;

            if (isRunning)
            {
                onFlip(true);
            }
            else
            {
                onFlip(false);
            }
        }


        private void onFlip(bool myBool)
        {
            entryx.IsEnabled = myBool;
            entryy.IsEnabled = myBool;
            entryz.IsEnabled = myBool;
            entryx.IsVisible = myBool;
            entryy.IsVisible = myBool;
            entryz.IsVisible = myBool;
            myLabel.IsVisible = myBool;
            myLabel.IsEnabled = myBool;
            quitButton.IsVisible = myBool;
            quitButton.IsEnabled = myBool;
            submitButton.IsEnabled = myBool;
            submitButton.IsVisible = myBool;

            startServerButton.IsEnabled = !myBool;
            startServerButton.IsVisible = !myBool;
        }
        private async void btnPreview_Clicked(object sender, EventArgs e)
        {
           

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                AudioPlayerViewModel playerViewModel = new AudioPlayerViewModel();
                playerViewModel.PlayClickAudio();
                activityIndicator.IsRunning = true;
                activityIndicator.IsVisible = true;
                lblTitle.Text = "Your image will be ready in a few moments!";
                prevImage.IsVisible = false;
                prompt = promptInput.Text.Trim();
                btnPreview.IsEnabled = false;
                promptInput.IsEnabled = false;

                // There is internet connection, proceed with API call
                ImageResult response = await CallApi();
                Bitmap map = Base64StringToBitmap(response.Data[0].Base64Data);
                string filename = "MineCraftImage.png";
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Console.WriteLine(directory.ToString());
                string filepath = Path.Combine(directory, filename);



                filepath = Path.Combine(directory, filename);

                map.Save(filepath, ImageFormat.Png);

                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                prevImage.Source = filepath;
                playerViewModel.StopAudio();
                btnPreview.IsEnabled = true;
                promptInput.IsEnabled = true;
                prevImage.IsVisible = true;
                lblTitle.Text = "Export to Minecraft or generate another image!";
            }
            else
            {
                // There is no internet connection, show an error
                await DisplayAlert("Error", "No internet connection.", "OK");
            }
        }


        async Task<ImageResult> CallApi()
        {
            OpenAIAPI api = new OpenAIAPI("");
            //sk-proj-JzSN7WJ2gjhQWO0ECtSdT3BlbkFJfxhIldr18NrUvlu14ypm\r\n
            var result = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(prompt, OpenAI_API.Models.Model.DALLE3, ImageSize._1024x1792, "hd", null, ImageResponseFormat.B64_json));
            return result;
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;

            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }

        public class AudioPlayerViewModel
        {
            private IAudioPlayer audioPlayer;

            public async void PlayClickAudio()
            {
                audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("Resources/Sounds/jeopardy-themelq.mp3"));
                audioPlayer.Play();
            }

            public void StopAudio()
            {
                audioPlayer?.Stop();
            }
        }

        public static Dictionary<Color, string> colorsDictionary = new Dictionary<Color, string>();
        public static string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public Process mcServerProc;

        public int xCoord;
        public int yCoord;
        public int zCoord;
        public static bool dicAdded;

        private void submitButton_Clicked(object sender, EventArgs e)
        {
            string imagePath = Path.Combine(mcServerProc.StartInfo.WorkingDirectory, exeDirectory, "MineCraftImage.png");
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
            Debug.WriteLine("Starting Minecraft_Drawer application...");

            mcServerProc = new Process();

            Debug.WriteLine("Creating process for Minecraft server...");


          
            mcServerProc.StartInfo.WorkingDirectory = exeDirectory;
           

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
            if(!dicAdded)
            {
                populateDictionary();
            }
            

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

            dicAdded = true;
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
            onFlip(false);
            isRunning = false;
        }

        private void startServerButton_Clicked(object sender, EventArgs e)
        {
            StartServer();
            onFlip(true);
            isRunning = true;
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            entryx.IsEnabled = false;
            entryy.IsEnabled = false;
            entryz.IsEnabled = false;
            submitButton.IsEnabled = false;
            quitButton.IsEnabled = false;
            startServerButton.IsEnabled = false;
            entryx.IsVisible = false;
            entryy.IsVisible = false;
            entryz.IsVisible = false;
            submitButton.IsVisible = false;
            quitButton.IsVisible = false;
            startServerButton.IsVisible = false;
            myLabel.IsVisible = false;
            myLabel.IsEnabled = false;
            prevImage.IsVisible = true;
            lblTitle.IsVisible = true;
            promptInput.IsVisible = true;
            btnPreview.IsVisible = true;
            btnAddtoMinecraft.IsVisible = true;
            prevImage.IsEnabled = true;
            lblTitle.IsEnabled = true;
            promptInput.IsEnabled = true;
            btnPreview.IsEnabled = true;
            btnAddtoMinecraft.IsEnabled = true;
            backButton.IsEnabled = false;
            backButton.IsVisible = false;
        }
    }
}
    