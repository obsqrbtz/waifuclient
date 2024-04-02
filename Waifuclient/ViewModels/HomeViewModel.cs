using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Waifuclient.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LiteDB;
using System.Threading.Tasks;
using Avalonia.Controls;


namespace Waifuclient.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private static ApiWrapper? _apiWrapper;
        private WaifuDb _db;
        private bool _disposed = false;
        [Reactive] public bool Liked {get; set;}
        [Reactive] public string Type { get; set; }
        [Reactive] public string? Category { get; set; }
        [Reactive] public List<string> Types { get; set; } = ApiWrapper.Types;
        [Reactive] public List<string> Categories { get; set; } = ApiWrapper.SfwCategories;
        [Reactive] public Bitmap? Waifu { get; set; }

        public HomeViewModel(WaifuDb db, ApiWrapper apiWrapper)
        {
            _apiWrapper = apiWrapper;
            Type = Types[0];
            Category = Categories[0];
            _db = db;
            NextWaifu();
        }

        public static void SaveImage(Bitmap waifu, string path) => waifu.Save(path);
        public async void NextWaifu()
        {
            if (_apiWrapper is null || Category is null)
                return;
            await _apiWrapper.GetWaifuInfo(Type, Category);
            if (_apiWrapper.Url is null)
                return;
            WaifuDbEntry? waifu = _db.EntryExists(_apiWrapper.Url);
            Stream? stream = new MemoryStream();
            if (waifu is null)
            {
                _db.Add(_apiWrapper.Url, Type, Category);
                waifu = _db.EntryExists(_apiWrapper.Url);
                Liked = false;
                stream = await _apiWrapper.GetImageStream();
            }
            else
            {
                Liked = waifu.Liked;
                var storage = _db.Db.GetStorage<int>();
                storage.Download(waifu.Id, stream);
                stream.Position = 0;
            }
            if (stream is not null && waifu is not null)
            {
                Waifu = await CreateBitmap(stream);
                string filename = waifu.Url.Split('/')[^1]; 
                Waifu.Save(filename);
                var storage = _db.Db.GetStorage<int>();
                stream.Position = 0;
                storage.Upload(waifu.Id, filename, stream);
            }
        }
        private static async Task<Bitmap> CreateBitmap(Stream stream)
        {
            double windowHeight = 1080;
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
            if (mainWindow is not null)
                windowHeight = mainWindow.Height;
            return await Task.Run(() =>
            {
                stream.Position = 0;
                var bitmap = new Bitmap(stream);
                double scale = 1.00;
                if (bitmap.Size.Height >= windowHeight)
                    scale = (windowHeight / bitmap.Size.Height);
                return bitmap.CreateScaledBitmap(new PixelSize((int)(bitmap.Size.Width * scale), (int)(bitmap.Size.Height * scale)));
            });
        }
        public async void SaveClick()
        {
            if (_apiWrapper is null || string.IsNullOrEmpty(_apiWrapper.Url) || Waifu is null)
                return;
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
            var storageProvider = (mainWindow?.StorageProvider) ?? throw new Exception("StorageProvider for SaveFilePickerAsync is null");
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save file",
                SuggestedFileName = _apiWrapper.Url.Split('/')[^1],
                FileTypeChoices = [FilePickerFileTypes.ImageAll],
            });
            if (file is not null)
                SaveImage(Waifu, file.Path.LocalPath);
        }
        public void LikeClick()
        {
            if (_apiWrapper is not null && _apiWrapper.Url is not null)
                _db.ToggleLike(_apiWrapper.Url);
        }
    }
}
