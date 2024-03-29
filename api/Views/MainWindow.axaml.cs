using System;
using api.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace api.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
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
            TransparencyLayer.Material.TintColor = ActualThemeVariant == ThemeVariant.Light ? Avalonia.Media.Color.FromRgb(255, 255, 255) : Avalonia.Media.Color.FromRgb(0, 0, 0);
        }
    }
}