using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlarmClock.Display
{
    public partial class TimeDisplay : UserControl
    {
        public TimeDisplay()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateDisplay();
        }

        public static readonly DependencyProperty HoursProperty =
            DependencyProperty.Register(nameof(Hours), typeof(int), typeof(TimeDisplay),
                new PropertyMetadata(0, (d, e) => ((TimeDisplay)d).UpdateDisplay()));

        public int Hours
        {
            get => (int)GetValue(HoursProperty);
            set => SetValue(HoursProperty, value);
        }

        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register(nameof(Minutes), typeof(int), typeof(TimeDisplay),
                new PropertyMetadata(0, (d, e) => ((TimeDisplay)d).UpdateDisplay()));

        public int Minutes
        {
            get => (int)GetValue(MinutesProperty);
            set => SetValue(MinutesProperty, value);
        }

        public static readonly DependencyProperty OnProperty =
            DependencyProperty.Register(nameof(ColonOn), typeof(bool), typeof(TimeDisplay),
                new PropertyMetadata(true, (d, e) => ((TimeDisplay)d).UpdateDisplay()));

        public bool ColonOn
        {
            get => (bool)GetValue(OnProperty);
            set => SetValue(OnProperty, value);
        }

        public static readonly DependencyProperty NumbersOnProperty =
            DependencyProperty.Register(nameof(NumbersOn), typeof(bool), typeof(TimeDisplay),
                new PropertyMetadata(true, (d, e) => ((TimeDisplay)d).UpdateDisplay()));

        public bool NumbersOn
        {
            get => (bool)GetValue(NumbersOnProperty);
            set => SetValue(NumbersOnProperty, value);
        }

        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(TimeDisplay),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 60, 60)),
                    (d, e) => ((TimeDisplay)d).UpdateColors()));

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(TimeDisplay),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(40, 20, 20)),
                    (d, e) => ((TimeDisplay)d).UpdateColors()));

        public Brush OffBrush
        {
            get => (Brush)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        private void UpdateColors()
        {
            foreach (var seg in new[] { H1, H2, M1, M2 })
            {
                seg.OnBrush = OnBrush;
                seg.OffBrush = OffBrush;
            }

            Colon.OnBrush = OnBrush;
            Colon.OffBrush = OffBrush;
        }

        private void UpdateDisplay()
        {
            string h = Math.Abs(Hours).ToString("D2");
            string m = Math.Abs(Minutes).ToString("D2");

            H1.Character = h[0];
            H2.Character = h[1];
            M1.Character = m[0];
            M2.Character = m[1];
            Colon.IsOn = ColonOn;
            H1.IsOn = NumbersOn;
            H2.IsOn = NumbersOn;
            M1.IsOn = NumbersOn;
            M2.IsOn = NumbersOn;
        }
    }
}
