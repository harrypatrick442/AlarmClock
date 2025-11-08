using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AlarmClock.Display
{
    public partial class SevenSegmentDisplay : UserControl
    {
        public SevenSegmentDisplay()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateSegments();
        }

        private static readonly Dictionary<char, string> SegmentMap = new()
        {
            ['0'] = "ABCDEF",
            ['1'] = "BC",
            ['2'] = "ABGED",
            ['3'] = "ABCDG",
            ['4'] = "FGBC",
            ['5'] = "AFGCD",
            ['6'] = "AFGCDE",
            ['7'] = "ABC",
            ['8'] = "ABCDEFG",
            ['9'] = "ABCFG",
            ['A'] = "ABCEFG",
            ['B'] = "FGCDE",
            ['C'] = "AFED",
            ['D'] = "BGEDC",
            ['E'] = "AFGED",
            ['F'] = "AFGE",
            [' '] = ""
        };

        public static readonly DependencyProperty CharacterProperty =
            DependencyProperty.Register(nameof(Character), typeof(char), typeof(SevenSegmentDisplay),
                new FrameworkPropertyMetadata(' ', FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((SevenSegmentDisplay)d).UpdateSegments()));

        public char Character
        {
            get => (char)GetValue(CharacterProperty);
            set => SetValue(CharacterProperty, value);
        }

        public static readonly DependencyProperty IsDotOnProperty =
            DependencyProperty.Register(nameof(IsDotOn), typeof(bool), typeof(SevenSegmentDisplay),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((SevenSegmentDisplay)d).UpdateSegments()));

        public bool IsDotOn
        {
            get => (bool)GetValue(IsDotOnProperty);
            set => SetValue(IsDotOnProperty, value);
        }

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(SevenSegmentDisplay),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((SevenSegmentDisplay)d).UpdateColor()));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(SevenSegmentDisplay),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 60, 60)),
                    FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((SevenSegmentDisplay)d).UpdateColor()));

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(SevenSegmentDisplay),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(40, 20, 20)),
                    FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => ((SevenSegmentDisplay)d).UpdateColor()));

        public Brush OffBrush
        {
            get => (Brush)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        private void UpdateColor()
        {
            foreach (var seg in new[] { A, B, C, D, E, F, G })
            {
                seg.OnBrush = IsOn?OnBrush: OffBrush;
                seg.OffBrush = OffBrush;
            }
            DP.Fill = IsDotOn && IsOn ? OnBrush : OffBrush;
        }

        private void UpdateSegments()
        {
            char c = char.ToUpper(Character);
            if (!SegmentMap.TryGetValue(c, out string? onSegs))
                onSegs = "";

            void Set(LedSegment s, bool on) => s.IsOn = on;

            Set(A, onSegs.Contains('A'));
            Set(B, onSegs.Contains('B'));
            Set(C, onSegs.Contains('C'));
            Set(D, onSegs.Contains('D'));
            Set(E, onSegs.Contains('E'));
            Set(F, onSegs.Contains('F'));
            Set(G, onSegs.Contains('G'));

            // Dot glow
            DP.Fill = IsDotOn ? OnBrush : OffBrush;
            if (DP.Effect is DropShadowEffect glow)
            {
                if (OnBrush is SolidColorBrush scb)
                    glow.Color = scb.Color;
                glow.BlurRadius = IsDotOn ? 18 : 0;
                glow.Opacity = IsDotOn ? 0.7 : 0;
            }
        }
    }
}
