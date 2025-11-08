using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AlarmClock.Display
{
    public partial class AlarmIndicator : UserControl
    {
        public AlarmIndicator()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateVisual();
        }

        public static readonly DependencyProperty IsIlluminatedProperty =
            DependencyProperty.Register(nameof(IsIlluminated), typeof(bool), typeof(AlarmIndicator),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((AlarmIndicator)d).UpdateVisual()));

        public bool IsIlluminated
        {
            get => (bool)GetValue(IsIlluminatedProperty);
            set => SetValue(IsIlluminatedProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AlarmIndicator),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((AlarmIndicator)d).UpdateVisual()));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register(nameof(Number), typeof(int), typeof(AlarmIndicator),
                new PropertyMetadata(1));

        public int Number
        {
            get => (int)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(AlarmIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 60, 60)),
                    FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((AlarmIndicator)d).UpdateVisual()));

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(AlarmIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(40, 20, 20)),
                    FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((AlarmIndicator)d).UpdateVisual()));

        public Brush OffBrush
        {
            get => (Brush)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        private void UpdateVisual()
        {
            bool illuminated = IsIlluminated;
            bool enabled = IsEnabled;

            Brush fill = illuminated ? OnBrush : OffBrush;

            BellShape.Fill = fill;
            NumberLabel.Foreground = fill;

            Cross1.Visibility = (illuminated && !enabled) ? Visibility.Visible : Visibility.Collapsed;
            Cross2.Visibility = (illuminated && !enabled) ? Visibility.Visible : Visibility.Collapsed;

            if (BellShape.Effect is DropShadowEffect glow && OnBrush is SolidColorBrush scb)
            {
                glow.Color = scb.Color;
                glow.BlurRadius = illuminated ? 18 : 0;
                glow.Opacity = illuminated ? 0.7 : 0;
            }
        }
    }
}
