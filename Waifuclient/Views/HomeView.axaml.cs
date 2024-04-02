using Waifuclient.Models;
using Waifuclient.ViewModels;
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Media;
using Avalonia;

namespace Waifuclient.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
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

        private void TypeSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is HomeViewModel context)
            {
                if (TypeCb.SelectedItem is string category)
                {
                    context.Categories = category == "nsfw"
                        ? [.. ApiWrapper.NsfwCategories]
                        : [.. ApiWrapper.SfwCategories];
                    CategoryCb.SelectedIndex = 0;
                }
            }
        }
    }
}
