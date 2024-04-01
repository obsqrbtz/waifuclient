using System;
using api.Models;
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
        }

        private void WindowClosing(object? sender, WindowClosingEventArgs e)
        {
            if (DataContext is MainWindowViewModel context) 
            {
                context.Dispose();
            }
        }
    }
}