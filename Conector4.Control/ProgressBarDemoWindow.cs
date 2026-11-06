using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Conector4.Control
{
    /// <summary>
    /// Janela de exemplo para demonstrar o uso do SegmentedProgressBar
    /// </summary>
    public class ProgressBarDemoWindow : Window
    {
        public ProgressBarDemoWindow()
        {
            Title = "Segmented Progress Bar Demo";
            Width = 800;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var grid = new Grid
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Margin = new Thickness(20)
            };

            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            // Progress Bar 78%
            var progressBar1 = new SegmentedProgressBar
            {
                Height = 50,
                Margin = new Thickness(0, 10, 0, 10),
                Value = 78,
                Maximum = 100,
                SegmentCount = 30,
                ShowPercentage = true
            };

            // Progress Bar 50%
            var progressBar2 = new SegmentedProgressBar
            {
                Height = 50,
                Margin = new Thickness(0, 10, 0, 10),
                Value = 50,
                Maximum = 100,
                SegmentCount = 30,
                ShowPercentage = true
            };

            // Progress Bar 100%
            var progressBar3 = new SegmentedProgressBar
            {
                Height = 50,
                Margin = new Thickness(0, 10, 0, 10),
                Value = 100,
                Maximum = 100,
                SegmentCount = 30,
                ShowPercentage = true
            };

            // Progress Bar 25%
            var progressBar4 = new SegmentedProgressBar
            {
                Height = 50,
                Margin = new Thickness(0, 10, 0, 10),
                Value = 25,
                Maximum = 100,
                SegmentCount = 30,
                ShowPercentage = true
            };

            stackPanel.Children.Add(progressBar1);
            stackPanel.Children.Add(progressBar2);
            stackPanel.Children.Add(progressBar3);
            stackPanel.Children.Add(progressBar4);

            grid.Children.Add(stackPanel);
            Content = grid;
        }

        /// <summary>
        /// Método para testar a janela
        /// </summary>
        [STAThread]
        public static void ShowDemo()
        {
            var app = new Application();
            app.Run(new ProgressBarDemoWindow());
        }
    }
}
