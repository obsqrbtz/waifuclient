using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Media;
using Avalonia;
using Waifuclient.ViewModels;
using Waifuclient.Models;
using ReactiveUI;
using Avalonia.Threading;

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
                if (e.RemovedItems.Count == 0 && page != 1 || e.RemovedItems.Count == 1)
                    context.SetThumbnails(page);
            }
        }
        private void TypeSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is FavsViewModel context)
            {
                if (TypeCb.SelectedItem is string category)
                {
                    context.Categories = category switch
                    {
                        "sfw" => ["all", ..ApiWrapper.SfwCategories],
                        "nsfw" => ["all", ..ApiWrapper.NsfwCategories],
                        _ => ["all", ..ApiWrapper.SfwCategories, ..ApiWrapper.NsfwCategories]
                    };
                    CategoryCb.SelectedIndex = 0;
                }
            }
        }

        private void CategorySelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is FavsViewModel context && e.RemovedItems.Count == 1)
            {
                if (CategoryCb.SelectedItem is string category)
                {
                    Pagination.SelectionChanged -= PaginationSelectionChanged;
                    context.Favs = context.FetchCategory(category);
                    context.SetPages();
                    context.SetThumbnails(1);
                    Pagination.SelectedIndex = 0;
                    Pagination.SelectionChanged += PaginationSelectionChanged;
                }
            }
        }
    }
}
