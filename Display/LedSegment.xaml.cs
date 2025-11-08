using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AlarmClock.Display
{
    public partial class LedSegment : UserControl
    {
        public LedSegment()
        {
            InitializeComponent();
            SizeChanged += (_, __) => UpdateGeometry();
            Loaded += (_, __) => UpdateAll();
        }

        #region Dependency Properties

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(LedSegment),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public static readonly DependencyProperty OnBrushProperty =
            DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(LedSegment),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 64, 64)),
                    FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public Brush OnBrush
        {
            get => (Brush)GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public static readonly DependencyProperty OffBrushProperty =
            DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(LedSegment),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(60, 20, 20)),
                    FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public Brush OffBrush
        {
            get => (Brush)GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        /// <summary>Length of the segment (tip to tip along its axis).</summary>
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register(nameof(Length), typeof(double), typeof(LedSegment),
                new FrameworkPropertyMetadata(60.0, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public double Length
        {
            get => (double)GetValue(LengthProperty);
            set => SetValue(LengthProperty, value);
        }

        /// <summary>Thickness of the segment (perpendicular to the axis).</summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(LedSegment),
                new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        /// <summary>Bevel amount that cuts the tips (0..Thickness/2 recommended).</summary>
        public static readonly DependencyProperty BevelProperty =
            DependencyProperty.Register(nameof(Bevel), typeof(double), typeof(LedSegment),
                new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public double Bevel
        {
            get => (double)GetValue(BevelProperty);
            set => SetValue(BevelProperty, value);
        }

        /// <summary>Angle (degrees) to rotate the segment around its center.</summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double), typeof(LedSegment),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        /// <summary>Glow opacity multiplier when lit (0..1).</summary>
        public static readonly DependencyProperty GlowOpacityProperty =
            DependencyProperty.Register(nameof(GlowOpacity), typeof(double), typeof(LedSegment),
                new FrameworkPropertyMetadata(0.7, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public double GlowOpacity
        {
            get => (double)GetValue(GlowOpacityProperty);
            set => SetValue(GlowOpacityProperty, value);
        }

        /// <summary>Glow blur radius (px) when lit.</summary>
        public static readonly DependencyProperty GlowBlurRadiusProperty =
            DependencyProperty.Register(nameof(GlowBlurRadius), typeof(double), typeof(LedSegment),
                new FrameworkPropertyMetadata(18.0, FrameworkPropertyMetadataOptions.AffectsRender, OnDPChanged));

        public double GlowBlurRadius
        {
            get => (double)GetValue(GlowBlurRadiusProperty);
            set => SetValue(GlowBlurRadiusProperty, value);
        }

        #endregion

        private static void OnDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (LedSegment)d;
            if (e.Property == IsOnProperty || e.Property == OnBrushProperty || e.Property == OffBrushProperty ||
                e.Property == GlowOpacityProperty || e.Property == GlowBlurRadiusProperty)
            {
                ctrl.UpdateVisuals();
            }

            if (e.Property == LengthProperty || e.Property == ThicknessProperty ||
                e.Property == BevelProperty || e.Property == AngleProperty)
            {
                ctrl.UpdateGeometry();
            }
        }

        private void UpdateAll()
        {
            UpdateGeometry();
            UpdateVisuals();
        }

        /// <summary>
        /// Build a beveled-hex segment centered at (0,0), then rotate and translate to the control center.
        /// </summary>
        private void UpdateGeometry()
        {
            double L = Math.Max(0, Length);
            double T = Math.Max(1, Thickness);

            // define "tip" depth: how far the angled ends extend inwards
            // 135° ends => tip depth = T / 2
            double tip = T / 2;

            // Shape outline (pointed ends)
            // Left and right points are centered at +/-L/2
            Point[] pts =
            {
            new Point(-L/2, 0),                // far left tip
            new Point(-L/2 + tip, -T/2),       // upper-left bevel
            new Point( L/2 - tip, -T/2),       // upper-right bevel
            new Point( L/2, 0),                // far right tip
            new Point( L/2 - tip,  T/2),       // lower-right bevel
            new Point(-L/2 + tip,  T/2)        // lower-left bevel
        };

            double radians = Angle * Math.PI / 180.0;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double cx = ActualWidth / 2.0;
            double cy = ActualHeight / 2.0;

            var geom = new StreamGeometry();
        using (var ctx = geom.Open())
        {
            Point R(Point p) => new Point(p.X * cos - p.Y * sin + cx, p.X * sin + p.Y * cos + cy);

        ctx.BeginFigure(R(pts[0]), isFilled: true, isClosed: true);
            for (int i = 1; i<pts.Length; i++)
                ctx.LineTo(R(pts[i]), isStroked: false, isSmoothJoin: true);
        }
        geom.Freeze();

        SegmentPath.Data = geom;
    }


        private void UpdateVisuals()
        {
            // Fill
            SegmentPath.Fill = IsOn ? OnBrush : OffBrush;

            // Glow
            if (SegmentPath.Effect is DropShadowEffect glow)
            {
                var onColor = Colors.White;
                if (OnBrush is SolidColorBrush scb)
                    onColor = scb.Color;

                glow.Color = onColor;
                glow.BlurRadius = IsOn ? GlowBlurRadius : 0.0;
                glow.Opacity = IsOn ? GlowOpacity : 0.0;
            }
        }
    }
}
