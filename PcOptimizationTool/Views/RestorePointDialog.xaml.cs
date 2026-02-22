using System.Windows;
using System.Windows.Media;
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
                StatusText.Text = "⚠ Run the app as administrator to create restore points.";
                StatusText.Foreground = Brushes.OrangeRed;
                CreateButton.IsEnabled = false;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                StatusText.Text = "Please enter a description.";
                StatusText.Foreground = Brushes.OrangeRed;
                return;
            }

            CreateButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            StatusText.Foreground = Brushes.Gray;
            StatusText.Text = "Creating restore point, please wait...";

            var success = await _restorePointService.CreateRestorePointAsync(description);

            if (success)
            {
                RestorePointCreated = true;
                StatusText.Foreground = Brushes.Green;
                StatusText.Text = "✓ Restore point created successfully!";
                await Task.Delay(1500);
                DialogResult = true;
            }
            else
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "✗ Failed. Make sure the app is running as administrator.";
                CreateButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DescriptionTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
