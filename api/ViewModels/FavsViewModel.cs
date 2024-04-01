using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models;
using Avalonia.Media.Imaging;
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
            SetBitmaps();
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
        private async void SetBitmaps()
        {
            int max = Favs.Count <= 30 ? Favs.Count : 30;
            for(int i = 0; i < max; i++)
            {
                var bitmap = await DownloadImage(Favs[i].Url);
                if (bitmap is not null)
                {
                    Waifus.Add(bitmap);
                    Waifus = new(Waifus);
                }
            }
        }
    }
}
