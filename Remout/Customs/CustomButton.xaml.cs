using System.Windows;
using Remout.Models;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using UserControl = System.Windows.Controls.UserControl;
using Remout.Services;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Remout.Customs
{
    /// <summary>
    /// Interaction logic for CustomButton.xaml
    /// </summary>
    public partial class CustomButton : UserControl
    {
        public DelegateCommand<object> Command
        {
            get => (DelegateCommand<object>)GetValue(CommandProperty); 
            set => SetValue(CommandProperty, value); 
        }
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public UIElement CustomButtonContent { get; set; }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(nameof(Command), typeof(DelegateCommand<object>), typeof(CustomButton), null);
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(nameof(CommandParameter), typeof(object), typeof(CustomButton), null);
        public static readonly DependencyProperty CustomButtonContentProperty =
            DependencyProperty.RegisterAttached(nameof(CustomButtonContent), typeof(UIElement), typeof(CustomButton), new PropertyMetadata(OnCustomButtonContentPropertySet));

        private static void OnCustomButtonContentPropertySet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var customButton = (CustomButton)d;
            customButton.Content = (UIElement)e.NewValue;
        }

        private bool _isMouseDown;

        public CustomButton()
        {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _isMouseDown = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseDown = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!_isMouseDown) return;
            _isMouseDown = false;
            Command.Execute(CommandParameter);
        }
    }
}
