using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Conector4.Control
{
    public class SegmentedProgressBar : System.Windows.Controls.Control
    {
        static SegmentedProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SegmentedProgressBar), new FrameworkPropertyMetadata(typeof(SegmentedProgressBar)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SegmentedProgressBar),
                new PropertyMetadata(0.0, OnValueChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SegmentedProgressBar),
                new PropertyMetadata(100.0, OnValueChanged));

        public static readonly DependencyProperty SegmentCountProperty =
            DependencyProperty.Register("SegmentCount", typeof(int), typeof(SegmentedProgressBar),
                new PropertyMetadata(30, OnSegmentCountChanged));

        public static readonly DependencyProperty ShowPercentageProperty =
            DependencyProperty.Register("ShowPercentage", typeof(bool), typeof(SegmentedProgressBar),
                new PropertyMetadata(true));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public int SegmentCount
        {
            get { return (int)GetValue(SegmentCountProperty); }
            set { SetValue(SegmentCountProperty, value); }
        }

        public bool ShowPercentage
        {
            get { return (bool)GetValue(ShowPercentageProperty); }
            set { SetValue(ShowPercentageProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentedProgressBar progressBar)
            {
                progressBar.InvalidateVisual();
            }
        }

        private static void OnSegmentCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentedProgressBar progressBar)
            {
                progressBar.InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double percentage = Maximum > 0 ? (Value / Maximum) * 100 : 0;
            percentage = Math.Max(0, Math.Min(100, percentage));

            // Fundo externo (borda cinza)
            Rect outerRect = new Rect(0, 0, ActualWidth, ActualHeight);
            drawingContext.DrawRectangle(Brushes.Gray, new Pen(Brushes.DarkGray, 2), outerRect);

            // Fundo interno (preto)
            Rect innerRect = new Rect(4, 4, ActualWidth - 8, ActualHeight - 8);
            drawingContext.DrawRectangle(Brushes.Black, null, innerRect);

            // Área para os segmentos
            double segmentAreaWidth = ActualWidth - 200; // Reserva espaço para o texto
            double segmentWidth = (segmentAreaWidth - (SegmentCount + 1) * 2) / SegmentCount;
            double segmentHeight = ActualHeight - 12;
            int filledSegments = (int)Math.Round((percentage / 100.0) * SegmentCount);

            // Desenha os segmentos
            for (int i = 0; i < SegmentCount; i++)
            {
                double x = 6 + i * (segmentWidth + 2);
                double y = 6;

                Rect segmentRect = new Rect(x, y, segmentWidth, segmentHeight);

                if (i < filledSegments)
                {
                    // Segmento preenchido com gradiente azul
                    LinearGradientBrush blueBrush = new LinearGradientBrush();
                    blueBrush.StartPoint = new Point(0, 0);
                    blueBrush.EndPoint = new Point(0, 1);
                    blueBrush.GradientStops.Add(new GradientStop(Color.FromRgb(100, 180, 255), 0.0));
                    blueBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 120, 215), 0.5));
                    blueBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 80, 180), 1.0));

                    drawingContext.DrawRectangle(blueBrush, null, segmentRect);
                }
                else
                {
                    // Segmento vazio (cinza escuro)
                    drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(60, 60, 60)), null, segmentRect);
                }
            }

            // Desenha o texto de porcentagem
            if (ShowPercentage)
            {
                FormattedText formattedText = new FormattedText(
                    $"{percentage:0}%",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                    ActualHeight * 0.6,
                    Brushes.White,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                double textX = ActualWidth - formattedText.Width - 20;
                double textY = (ActualHeight - formattedText.Height) / 2;

                drawingContext.DrawText(formattedText, new Point(textX, textY));
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(
                double.IsInfinity(constraint.Width) ? 400 : constraint.Width,
                double.IsInfinity(constraint.Height) ? 50 : constraint.Height);
        }
    }
}
