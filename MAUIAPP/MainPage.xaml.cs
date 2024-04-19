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


namespace MAUIAPP
{
    
    public partial class MainPage : ContentPage
    {
        public String prompt;
        public MainPage()
        {
            prompt = "";
            InitializeComponent();
            btnPreview.IsEnabled = false;
            btnAddtoMinecraft.IsEnabled = false;
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

        }

        private async void btnPreview_Clicked(object sender, EventArgs e)
        {
            prompt = promptInput.Text.Trim();
            btnPreview.IsEnabled = false;
            promptInput.IsEnabled = false;

            ImageResult response = await CallApi();
            Bitmap map = Base64StringToBitmap(response.Data[0].Base64Data);

            string filename = "MineCraftImage.png";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(directory, filename);
            
            

            filepath = Path.Combine(directory, filename);

            map.Save(filepath, ImageFormat.Png);
            
            prevImage.Source = filepath;
            btnPreview.IsEnabled = true;
            promptInput.IsEnabled = true;


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
