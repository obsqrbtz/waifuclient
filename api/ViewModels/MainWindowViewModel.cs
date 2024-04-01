using System;
using System.Collections.Generic;
using System.IO;
using api.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace api.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IDisposable
    {
        private static ApiWrapper? _apiWrapper;
        private WaifuDb _db;
        private bool _disposed = false;
        [Reactive] public string Type { get; set; }
        [Reactive] public string? Category { get; set; }
        [Reactive] public List<string> Types { get; set; } = ApiWrapper.Types;
        [Reactive] public List<string> Categories { get; set; } = ApiWrapper.SfwCategories;
        [Reactive] public Bitmap? Waifu { get; set; }

        public MainWindowViewModel()
        {
            Type = Types[0];
            Category = Categories[0];
            _apiWrapper = new();
            _db = new();
            //NextWaifu();
        }

        public static void SaveImage(Bitmap waifu, string path) => waifu.Save(path);
        public async void NextWaifu()
        {
            if (_apiWrapper is null || Category is null)
                return;
            Stream? stream = await _apiWrapper.GetWaifu(Type, Category);
            if (_apiWrapper.Url is not null && !_db.EntryExists(_apiWrapper.Url))
            {
                _db.Add(_apiWrapper.Url, Type, Category);
            }
            if (stream is not null)
                Waifu = new Bitmap(stream);
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
                FileTypeChoices = new[] { FilePickerFileTypes.ImageAll },
            });
            if (file is not null)
                SaveImage(Waifu, file.Path.LocalPath);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _apiWrapper is not null)
                {
                    _db.Dispose();
                    _apiWrapper.Dispose();
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MainWindowViewModel()
        {
            Dispose(false);
        }
    }
}
