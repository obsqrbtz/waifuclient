using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Media;
using Avalonia;
using Waifuclient.ViewModels;

namespace Waifuclient.Views
{
    public partial class FavsView : UserControl
    {
        public FavsView()
        {
            InitializeComponent();
            ActualThemeVariantChanged += ThemeVariantChanged;
            TransparencyLayer.Material.TintColor = ActualThemeVariant == ThemeVariant.Light
                ? Color.FromRgb(255, 255, 255)
                : Color.FromRgb(0, 0, 0);
        }
        private void ThemeVariantChanged(object? sender, EventArgs e)
        {
            TransparencyLayer.Material.TintColor = ActualThemeVariant == ThemeVariant.Light
                ? Color.FromRgb(255, 255, 255)
                : Color.FromRgb(0, 0, 0);
        }
        private void ThemeToggleClick(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button || Application.Current is null) return;
            Application.Current.RequestedThemeVariant =
                ActualThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
            TransparencyLayer.Material.TintColor = ActualThemeVariant == ThemeVariant.Light ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0);
        }

        private void PaginationSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is FavsViewModel context && Pagination.SelectedItem is int page)
            {
                context.SetThumbnails(page);
            }
        }
    }
}
