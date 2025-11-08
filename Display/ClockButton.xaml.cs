using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AlarmClock.Display
{
    public partial class ClockButton : UserControl
    {
        public ClockButton()
        {
            InitializeComponent();
        }

        // --- Dependency Properties ---

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ClockButton),
                new PropertyMetadata("BUTTON"));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ClockButton));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(ClockButton));

        public ICommand MouseDownCommand
        {
            get => (ICommand)GetValue(MouseDownCommandProperty);
            set => SetValue(MouseDownCommandProperty, value);
        }

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.Register(nameof(MouseUpCommand), typeof(ICommand), typeof(ClockButton));

        public ICommand MouseUpCommand
        {
            get => (ICommand)GetValue(MouseUpCommandProperty);
            set => SetValue(MouseUpCommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ClockButton));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register(nameof(ButtonForeground), typeof(Brush), typeof(ClockButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xFF, 0x33, 0x33))));

        public Brush ButtonForeground
        {
            get => (Brush)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register(nameof(ButtonBackground), typeof(Brush), typeof(ClockButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x22, 0x08, 0x08))));

        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonBorderBrushProperty =
            DependencyProperty.Register(nameof(ButtonBorderBrush), typeof(Brush), typeof(ClockButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x55, 0x22, 0x22))));

        public Brush ButtonBorderBrush
        {
            get => (Brush)GetValue(ButtonBorderBrushProperty);
            set => SetValue(ButtonBorderBrushProperty, value);
        }

        public static readonly DependencyProperty ButtonBorderThicknessProperty =
            DependencyProperty.Register(nameof(ButtonBorderThickness), typeof(Thickness), typeof(ClockButton),
                new PropertyMetadata(new Thickness(2)));

        public Thickness ButtonBorderThickness
        {
            get => (Thickness)GetValue(ButtonBorderThicknessProperty);
            set => SetValue(ButtonBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty ButtonFontSizeProperty =
            DependencyProperty.Register(nameof(ButtonFontSize), typeof(double), typeof(ClockButton),
                new PropertyMetadata(18d));

        public double ButtonFontSize
        {
            get => (double)GetValue(ButtonFontSizeProperty);
            set => SetValue(ButtonFontSizeProperty, value);
        }

        // --- Event handlers for mouse commands ---
        private void PART_Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseDownCommand?.CanExecute(null) == true)
                MouseDownCommand.Execute(null);
        }

        private void PART_Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseUpCommand?.CanExecute(null) == true)
                MouseUpCommand.Execute(null);
        }
    }
}
