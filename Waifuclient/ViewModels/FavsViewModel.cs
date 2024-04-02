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
        private HttpClient _httpClient;
        private int _maxItems = 30;
        private int _currentItem = 0;
        private int _pages = 1;

        [Reactive] public List<int> Pages { get; set; }
        [Reactive] public List<FavViewItem> Waifus { get; set; }
        public List<WaifuDbEntry> Favs { get; set; }
        public FavsViewModel(WaifuDb db)
        {
            Waifus = [];
            Pages = [];
            _db = db;
            _httpClient = new();
            Favs = _db.FetchLiked().ToList();
            _pages = Favs.Count / _maxItems;
            if (Favs.Count % _maxItems > 0)
            {
                _pages++;
            } 
            for (int i = 1; i <= _pages; i++)
            {
                Pages.Add(i);
            }
            Pages = [.. Pages];
            SetThumbnails(1);
            //Favs = _db.FetchAll().ToList();
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
                await Task.Run(async () =>
                {
                    Waifus.Add(new() 
                    { 
                        DbEntry = Favs[i], 
                        Image = await CreateThumbnail(storage, Favs[i].Id) 
                    });
                    Waifus = new(Waifus);
                });
            }
        }
        private static async Task<Bitmap> CreateThumbnail(ILiteStorage<int> storage, int id)
        {
            return await Task.Run(() =>
            {
                Stream stream = new MemoryStream();
                storage.Download(id, stream);
                stream.Position = 0;
                var bitmap = new Bitmap(stream);
                double scale = (450.00 / bitmap.Size.Height);
                return bitmap.CreateScaledBitmap(new Avalonia.PixelSize((int)(bitmap.Size.Width * scale), (int)(bitmap.Size.Height * scale)));
            });
        }

    }
}
