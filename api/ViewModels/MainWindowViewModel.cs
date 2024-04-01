﻿using System;
using api.Models;
using ReactiveUI;


namespace api.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private ViewModelBase _contentViewModel;
        private WaifuDb _db;
        private bool _disposed = false;

        public MainWindowViewModel()
        {
            _db = new();
            _contentViewModel = new HomeViewModel(_db);
        }
        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }
        public void OpenFavs()
        {
            if (ContentViewModel is HomeViewModel vm)
                vm.Dispose();
            ContentViewModel = new FavsViewModel(_db);
        }
        public void OpenHome()
        {
            if (ContentViewModel is FavsViewModel vm)
            {
                _db.Update(vm.Favs);
            }
            ContentViewModel = new HomeViewModel(_db);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
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
