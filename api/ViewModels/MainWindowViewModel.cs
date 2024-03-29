using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace api.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private Waifu? _responseWaifu;
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
            _responseWaifu = JsonSerializer.Deserialize<Waifu>(response);
            if (_responseWaifu is null)
                return;
            DownloadImage(_responseWaifu.url);
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
        public void SaveImage(Bitmap waifu, string path)
        {
            waifu.Save(path);
        }
        public async void SaveClick()
        {
            if (_responseWaifu is null || Waifu is null)
                return;
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
            var storageProvider = (mainWindow?.StorageProvider) ?? throw new Exception("StorageProvider for SaveFilePickerAsync is null");
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save file",
                SuggestedFileName = _responseWaifu.url.Split('/')[^1],
                FileTypeChoices = new[] { FilePickerFileTypes.ImageAll },
            });
            if (file is not null)
                SaveImage(Waifu, file.Path.LocalPath);
        }
    }
    public class Waifu
    {
        public string url { get; set; } = string.Empty;
    }
}
