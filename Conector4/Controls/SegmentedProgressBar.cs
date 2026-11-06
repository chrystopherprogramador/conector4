using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Conector4.Controls
{
    /// <summary>
    /// Controle de barra de progresso segmentada para Windows Forms
    /// Replica o visual de uma barra de progresso com segmentos individuais, gradiente azul e porcentagem
    /// </summary>
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class SegmentedProgressBar : UserControl
    {
        private double _value = 0;
        private double _maximum = 100;
        private int _segmentCount = 30;
        private bool _showPercentage = true;

        public SegmentedProgressBar()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Height = 50;
            this.Width = 600;
        }

        /// <summary>
        /// Valor atual do progresso (0 a Maximum)
        /// </summary>
        [Category("Behavior")]
        [Description("Valor atual do progresso (0 a Maximum)")]
        [DefaultValue(0.0)]
        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = Math.Max(0, Math.Min(_maximum, value));
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Valor máximo do progresso
        /// </summary>
        [Category("Behavior")]
        [Description("Valor máximo do progresso")]
        [DefaultValue(100.0)]
        public double Maximum
        {
            get { return _maximum; }
            set
            {
                if (_maximum != value && value > 0)
                {
                    _maximum = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Número de segmentos na barra
        /// </summary>
        [Category("Appearance")]
        [Description("Número de segmentos na barra")]
        [DefaultValue(30)]
        public int SegmentCount
        {
            get { return _segmentCount; }
            set
            {
                if (_segmentCount != value && value > 0)
                {
                    _segmentCount = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Exibir ou ocultar o texto de porcentagem
        /// </summary>
        [Category("Appearance")]
        [Description("Exibir ou ocultar o texto de porcentagem")]
        [DefaultValue(true)]
        public bool ShowPercentage
        {
            get { return _showPercentage; }
            set
            {
                if (_showPercentage != value)
                {
                    _showPercentage = value;
                    this.Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            double percentage = Maximum > 0 ? (Value / Maximum) * 100 : 0;
            percentage = Math.Max(0, Math.Min(100, percentage));

            // Borda externa cinza
            using (Pen outerPen = new Pen(Color.FromArgb(128, 128, 128), 3))
            {
                g.DrawRectangle(outerPen, 0, 0, Width - 1, Height - 1);
            }

            // Fundo interno preto
            Rectangle innerRect = new Rectangle(4, 4, Width - 8, Height - 8);
            using (SolidBrush blackBrush = new SolidBrush(Color.Black))
            {
                g.FillRectangle(blackBrush, innerRect);
            }

            // Área dos segmentos
            int textAreaWidth = 150; // Espaço reservado para o texto
            int segmentAreaWidth = Width - textAreaWidth - 12;
            float segmentWidth = (segmentAreaWidth - (_segmentCount + 1) * 2f) / _segmentCount;
            float segmentHeight = Height - 12;
            int filledSegments = (int)Math.Round((percentage / 100.0) * _segmentCount);

            // Desenhar segmentos
            for (int i = 0; i < _segmentCount; i++)
            {
                float x = 6 + i * (segmentWidth + 2);
                float y = 6;

                RectangleF segmentRect = new RectangleF(x, y, segmentWidth, segmentHeight);

                if (i < filledSegments)
                {
                    // Segmento preenchido com gradiente azul
                    using (LinearGradientBrush blueBrush = new LinearGradientBrush(
                        segmentRect,
                        Color.FromArgb(100, 180, 255),
                        Color.FromArgb(0, 80, 180),
                        LinearGradientMode.Vertical))
                    {
                        ColorBlend colorBlend = new ColorBlend(3);
                        colorBlend.Colors = new Color[]
                        {
                            Color.FromArgb(100, 180, 255),
                            Color.FromArgb(0, 120, 215),
                            Color.FromArgb(0, 80, 180)
                        };
                        colorBlend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                        blueBrush.InterpolationColors = colorBlend;

                        g.FillRectangle(blueBrush, segmentRect);
                    }
                }
                else
                {
                    // Segmento vazio (cinza escuro)
                    using (SolidBrush grayBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                    {
                        g.FillRectangle(grayBrush, segmentRect);
                    }
                }
            }

            // Desenhar texto de porcentagem
            if (ShowPercentage)
            {
                string text = $"{percentage:0}%";
                using (Font font = new Font(FontFamily.GenericSansSerif, Height * 0.4f, FontStyle.Bold))
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    float textX = Width - textSize.Width - 15;
                    float textY = (Height - textSize.Height) / 2;

                    g.DrawString(text, font, whiteBrush, textX, textY);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
