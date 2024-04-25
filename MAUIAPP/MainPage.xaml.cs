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
using System.Diagnostics;



namespace MAUIAPP
{
    
    public partial class MainPage : ContentPage
    {
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

        private async void btnAddtoMinecraft_Clicked(object sender, EventArgs e)
        {
            //process.StartInfo.FileName = "C:\\Users\\iosdr\\Documents\\GitHub\\Minecraft-AI-Structure-Generator\\Minecraft_Drawer\\Minecraft_Drawer\\bin\\Debug\\Minecraft_Drawer.exe";
            //process.StartInfo.Arguments = "-n";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            //process.Start("C:\\Users\\iosdr\\Documents\\GitHub\\Minecraft-AI-Structure-Generator\\Minecraft_Drawer\\Minecraft_Drawer\\bin\\Debug\\Minecraft_Drawer.exe");
            
            //System.Diagnostics.Process.Start("");
            process = new Process();
            process.StartInfo.FileName = ("C:\\Users\\iosdr\\Documents\\GitHub\\Minecraft-AI-Structure-Generator\\Minecraft_Drawer\\Minecraft_Drawer\\bin\\Debug\\Minecraft_Drawer.exe");
            process.StartInfo.WorkingDirectory = "C:\\Users\\iosdr\\Documents\\GitHub\\Minecraft-AI-Structure-Generator\\Minecraft_Drawer\\Minecraft_Drawer\\bin\\Debug";
            process.StartInfo.UseShellExecute = true;
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
            await Navigation.PushAsync(new Coords());
        }

        private async void btnPreview_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;
            lblTitle.Text = "Your image will be ready in a few moments!";
            prevImage.IsVisible = false;
            prompt = promptInput.Text.Trim();
            btnPreview.IsEnabled = false;
            promptInput.IsEnabled = false;

            ImageResult response = await CallApi();
            Bitmap map = Base64StringToBitmap(response.Data[0].Base64Data);

            string filename = "MineCraftImage.png";
            directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(directory.ToString());
            string filepath = Path.Combine(directory, filename);
            
            

            filepath = Path.Combine(directory, filename);

            map.Save(filepath, ImageFormat.Png);
            

            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible= false;            
            prevImage.Source = filepath;
            btnPreview.IsEnabled = true;
            promptInput.IsEnabled = true;
            prevImage.IsVisible = true;
            lblTitle.Text = "Export to Minecraft or generate another image!";


        }

        async Task<ImageResult> CallApi()
        {
            OpenAIAPI api = new OpenAIAPI("");
            
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
    }

}
