using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Waifuclient.Models;
using Avalonia.Media.Imaging;
using LiteDB;
using ReactiveUI.Fody.Helpers;
using Avalonia.Controls;
using ReactiveUI;
using Avalonia.Threading;
using Waifuclient.Views;

namespace Waifuclient.ViewModels
{
    public class FavViewItem
    {
        public WaifuDbEntry? DbEntry { get; set; }
        public Bitmap? Image { get; set; }
    }
    public class FavsViewModel : ViewModelBase
    {
        private WaifuDb _db;
        private static ApiWrapper _apiWrapper;
        private HttpClient _httpClient;
        private int _maxItems = 30;
        private int _currentItem = 0;
        private int _pagesCount = 1;
        private List<int> _pages = [1];

        public event EventHandler? ThumbnailsCreated;

        public List<int> Pages { get => _pages; set => this.RaiseAndSetIfChanged(ref _pages, value); }
        [Reactive] public List<FavViewItem> Waifus { get; set; }
        public List<WaifuDbEntry> Favs { get; set; }
        [Reactive] public string Type { get; set; }
        [Reactive] public string? Category { get; set; }
        [Reactive] public List<string> Types { get; set; } = ["all", ..ApiWrapper.Types];
        [Reactive] public List<string> Categories { get; set; } = ["all", ..ApiWrapper.SfwCategories];
        public FavsViewModel(WaifuDb db, ApiWrapper apiWrapper)
        {
            Waifus = [];
            Pages = [1];
            Type = Types[0];
            Category = Categories[0];
            _db = db;
            _apiWrapper = apiWrapper;
            _httpClient = new();
            Favs = FetchCategory("all");
            SetPages();
            SetThumbnails(1);
            ThumbnailsCreated += FavsViewModelThumbnailsCreated;
            //Favs = _db.FetchAll().ToList();
        }
        private void FavsViewModelThumbnailsCreated(object? sender, EventArgs e)
        {
            SetPages();
        }
        public void SetPages()
        {
            _pages = [1];
            _pagesCount = Favs.Count / _maxItems;
            if (Favs.Count % _maxItems > 0)
            {
                _pagesCount++;
            }
            for (int i = 2; i <= _pagesCount; i++)
            {
                _pages.Add(i);
            }
            _pages = new(_pages);
            this.RaisePropertyChanged(nameof(Pages));
        }
        public List<WaifuDbEntry> FetchCategory(string category)
        {
            if (category != "all")
            {
                return _db.FetchCategoryLiked(category).ToList();
            }
            else
            {
                return _db.FetchLiked().ToList();
            }
        }
        private async Task<Bitmap?> DownloadImage(string url)
        {
            Bitmap? bitmap = null;
            try
            {
                if (_httpClient is null)
                    return null;
                byte[] imgBuff = await _httpClient.GetByteArrayAsync(url);
                var stream = new MemoryStream(imgBuff);
                if (stream is not null)
                {
                    bitmap = new Bitmap(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return bitmap;
        }
        public async void SetThumbnails(int page)
        {
            if (Favs.Count == 0)
            {
                Waifus = [];
                _pages = [1];
                return;
            }
            _currentItem = page * _maxItems - _maxItems;
            int max = _currentItem + _maxItems;
            if (Favs.Count < max)
            {
                max = Favs.Count;
            }
            Waifus = [];
            var storage = _db.Db.GetStorage<int>();
            for (int i = _currentItem; i < max; i++)
            {
                // TODO: deal with running task if new category is requested while previous is still loading
                await Task.Run(async () =>
                {
                    FavViewItem fav = new()
                    {
                        DbEntry = Favs[i],
                        Image = await CreateThumbnail(storage, Favs[i].Id)
                    };
                    if (fav.Image is null)
                    {
                        Stream? stream = await _apiWrapper.GetImageStream(Favs[i].Url);
                        if (stream is not null)
                        {
                            fav.Image = await CreateThumbnail(stream);
                            stream.Position = 0;
                            storage.Upload(Favs[i].Id, Favs[i].Url.Split('/')[^1], stream);
                        }
                    }
                    Waifus.Add(fav);
                    Waifus = new(Waifus);
                });
            }
        }
        private static async Task<Bitmap?> CreateThumbnail(ILiteStorage<int> storage, int id)
        {
            return await Task.Run(() =>
            {
                Stream stream = new MemoryStream();
                try
                {
                    storage.Download(id, stream);
                }
                catch
                {
                    return null;
                }
                stream.Position = 0;
                var bitmap = new Bitmap(stream);
                double scale = (450.00 / bitmap.Size.Height);
                return bitmap.CreateScaledBitmap(new Avalonia.PixelSize((int)(bitmap.Size.Width * scale), (int)(bitmap.Size.Height * scale)));
            });
        }
        private static async Task<Bitmap> CreateThumbnail(Stream stream)
        {
            return await Task.Run(() =>
            {
                stream.Position = 0;
                var bitmap = new Bitmap(stream);
                double scale = (450.00 / bitmap.Size.Height);
                return bitmap.CreateScaledBitmap(new Avalonia.PixelSize((int)(bitmap.Size.Width * scale), (int)(bitmap.Size.Height * scale)));
            });
        }

    }
}
