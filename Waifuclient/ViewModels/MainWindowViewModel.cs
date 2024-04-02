using System;
using Waifuclient.Models;
using Avalonia.Controls;
using ReactiveUI;


namespace Waifuclient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private ViewModelBase _contentViewModel;
        private WaifuDb _db;
        private ApiWrapper _apiWrapper;
        private bool _disposed = false;
        private string _dbPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\waifu.db";

        public MainWindowViewModel()
        {
            _apiWrapper = new();
            if (Design.IsDesignMode)
                _db = new("_waifu.db");
            else
                _db = new(_dbPath);
            _contentViewModel = new HomeViewModel(_db, _apiWrapper);
        }
        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }
        public void OpenFavs()
        {
            if (ContentViewModel is HomeViewModel vm)
            ContentViewModel = new FavsViewModel(_db, _apiWrapper);
        }
        public void OpenHome()
        {
            if (ContentViewModel is FavsViewModel vm)
            {
                _db.Update(vm.Favs);
            }
            ContentViewModel = new HomeViewModel(_db, _apiWrapper);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
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
