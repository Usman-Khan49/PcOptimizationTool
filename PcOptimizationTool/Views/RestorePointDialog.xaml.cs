using System.Windows;
using PcOptimizationTool.Interfaces;

namespace PcOptimizationTool.Views
{
    public partial class RestorePointDialog : Window
    {
        private readonly IRestorePointService _restorePointService;

        public bool RestorePointCreated { get; private set; }

        public RestorePointDialog(IRestorePointService restorePointService)
        {
            InitializeComponent();
            _restorePointService = restorePointService;

            if (!_restorePointService.IsRunningAsAdmin())
            {
                StatusText.Text = "⚠ Administrator privileges are required to create restore points. Please restart the app as administrator.";
                StatusText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                CreateButton.IsEnabled = false;
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                StatusText.Text = "Please enter a description.";
                StatusText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                return;
            }

            CreateButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            StatusText.Foreground = System.Windows.Media.Brushes.Gray;
            StatusText.Text = "Creating restore point, please wait...";

            var success = await _restorePointService.CreateRestorePointAsync(description);

            if (success)
            {
                RestorePointCreated = true;
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
                StatusText.Text = "✓ Restore point created successfully!";
                await Task.Delay(1500);
                DialogResult = true;
            }
            else
            {
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
                StatusText.Text = "✗ Failed to create restore point. Make sure the app is running as administrator.";
                CreateButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
