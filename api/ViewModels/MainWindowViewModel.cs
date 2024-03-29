using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace api.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        [Reactive] public string Type { get; set; }
        [Reactive] public string? Category { get; set; }
        [Reactive] public List<string> Types { get; set; } = ["sfw", "nsfw"];
        [Reactive] public List<string> Categories { get; set; }
        [Reactive] public Avalonia.Media.Imaging.Bitmap? Waifu { get; set; }
        public List<string> SfwCategories { get; set; } =
        [
            "waifu",
            "neko",
            "shinobu",
            "megumin",
            "bully",
            "cuddle",
            "cry",
            "hug",
            "awoo",
            "kiss",
            "lick",
            "pat",
            "smug",
            "bonk",
            "yeet",
            "blush",
            "smile",
            "wave",
            "highfive",
            "handhold",
            "nom",
            "bite",
            "glomp",
            "slap",
            "kill",
            "kick",
            "happy",
            "wink",
            "poke",
            "dance",
            "cringe"
        ];
        public List<string> NsfwCategories { get; set; } =
        [
            "waifu",
            "neko",
            "trap",
            "blowjob"
        ];
        public MainWindowViewModel()
        {
            Categories = [.. SfwCategories];
            Type = "sfw";
            Category = "waifu";
            GetWaifu();
        }
        public async void GetWaifu()
        {
            var response = await _httpClient.GetStringAsync($"https://api.waifu.pics/{Type}/{Category}");
            var waifu = JsonSerializer.Deserialize<Waifu>(response);
            if (waifu is null)
                return;
            DownloadImage(waifu.url);
        }
        public async void DownloadImage(string url)
        {
            try
            {
                byte[] imgBuff = await _httpClient.GetByteArrayAsync(url);
                Stream stream = new MemoryStream(imgBuff);
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
