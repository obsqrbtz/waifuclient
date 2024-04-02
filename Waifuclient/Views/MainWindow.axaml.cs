using System;
using Waifuclient.Models;
using Waifuclient.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace Waifuclient.Views
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