using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace api.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private static HttpClient httpClient = new HttpClient();
        [Reactive] public string Type { get; set; }
        [Reactive] public string? Category { get; set; }
        [Reactive] public List<string> Types { get; set; } = new() { "sfw", "nsfw" };
        [Reactive] public List<string> Categories { get; set; } = new() { "waifu", "neko", "shinobu", "megumin" };
        [Reactive] public Avalonia.Media.Imaging.Bitmap? Waifu { get; set; }
        public MainWindowViewModel()
        {
            Type = "sfw";
            Category = "waifu";
            GetWaifu();
        }
        public async void GetWaifu()
        {
            var response = await httpClient.GetStringAsync($"https://api.waifu.pics/{Type}/{Category}");
            var waifu = JsonSerializer.Deserialize<Waifu>(response);
            if (waifu is null)
                return;
            DownloadImage(waifu.url);
        }
        public void DownloadImage(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadDataAsync(new Uri(url));
                client.DownloadDataCompleted += DownloadComplete;
            }
        }
        private void DownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                byte[] bytes = e.Result;

                Stream stream = new MemoryStream(bytes);

                var image = new Avalonia.Media.Imaging.Bitmap(stream);
                Waifu = image;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Waifu = null;
            }
        }
    }
    public class Waifu
    {
        public string url { get; set; } = string.Empty;
    }
}
