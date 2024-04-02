using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models;
using Avalonia.Media.Imaging;
using DynamicData;
using LiteDB;
using ReactiveUI.Fody.Helpers;

namespace api.ViewModels
{
    public class FavsViewModel : ViewModelBase
    {
        private WaifuDb _db;
        private HttpClient _httpClient;

        [Reactive] public List<Bitmap?> Waifus { get; set; }
        [Reactive]public List<WaifuDbEntry> Favs { get; set; }
        public FavsViewModel(WaifuDb db)
        {
            Waifus = [];
            _db = db;
            _httpClient = new();
            Favs = _db.FetchLiked().ToList();
            SetThumbnails();
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
        private async void SetThumbnails()
        {
            int max = Favs.Count <= 30 ? Favs.Count : 30;
            var storage = _db.Db.GetStorage<int>();
            for (int i = 0; i < max; i++)
            {
                //var bitmap = await DownloadImage(Favs[i].Url);
                await Task.Run(async () =>
                {
                    Waifus.Add(await CreateThumbnail(storage, Favs[i].Id));
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
                double scale = (300.00 / bitmap.Size.Height);
                return bitmap.CreateScaledBitmap(new Avalonia.PixelSize((int)(bitmap.Size.Width * scale), (int)(bitmap.Size.Height * scale)));
            });
        }

    }
}
