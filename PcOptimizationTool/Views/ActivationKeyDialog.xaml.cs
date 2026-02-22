using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using PcOptimizationTool.Interfaces;

namespace PcOptimizationTool.Views
{
    public partial class ActivationKeyDialog : Window
    {
        private readonly ILicenseService _licenseService;

        public ActivationKeyDialog(ILicenseService licenseService)
        {
            InitializeComponent();
            _licenseService = licenseService;
            Loaded += (_, _) => KeyTextBox.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void KeyTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(KeyTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            var key = KeyTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                StatusText.Text = "Please enter a key.";
                StatusText.Foreground = Brushes.OrangeRed;
                return;
            }

            ActivateButton.IsEnabled = false;
            StatusText.Foreground = Brushes.Gray;
            StatusText.Text = "Verifying key...";

            var success = _licenseService.ValidateAndActivate(key);

            if (success)
            {
                StatusText.Foreground = Brushes.Green;
                StatusText.Text = "✓ Activated successfully!";
                Task.Delay(1000).ContinueWith(_ => Dispatcher.Invoke(() => DialogResult = true));
            }
            else
            {
                StatusText.Foreground = Brushes.OrangeRed;
                StatusText.Text = "Invalid key. Please check and try again.";
                ActivateButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
