using System.Collections.Generic;
using System.Linq;
using api.Models;
using ReactiveUI.Fody.Helpers;

namespace api.ViewModels
{
    public class FavsViewModel : ViewModelBase
    {
        private WaifuDb _db;
        [Reactive]public List<WaifuDbEntry> Favs { get; set; }
        public FavsViewModel(WaifuDb db)
        {
            _db = db;
            Favs = _db.FetchLiked().ToList();
        }
    }
}
