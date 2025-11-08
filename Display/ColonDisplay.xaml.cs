using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AlarmClock.Display
{
    public partial class ColonDisplay : UserControl
    {
        public ColonDisplay()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateVisual();
        }

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(ColonDisplay),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((ColonDisplay)d).UpdateVisual()));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(ColonDisplay),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 60, 60)),
                    FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((ColonDisplay)d).UpdateVisual()));

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(ColonDisplay),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(40, 20, 20)),
                    FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((ColonDisplay)d).UpdateVisual()));

        public Brush OffBrush
        {
            get => (Brush)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        private void UpdateVisual()
        {
            Brush fill = IsOn ? OnBrush : OffBrush;
            TopDot.Fill = fill;
            BottomDot.Fill = fill;

            static void SetGlow(DropShadowEffect glow, Brush brush, bool on)
            {
                if (brush is SolidColorBrush scb)
                    glow.Color = scb.Color;
                glow.BlurRadius = on ? 18 : 0;
                glow.Opacity = on ? 0.7 : 0;
            }

            SetGlow((DropShadowEffect)TopDot.Effect, OnBrush, IsOn);
            SetGlow((DropShadowEffect)BottomDot.Effect, OnBrush, IsOn);
        }
    }
}
