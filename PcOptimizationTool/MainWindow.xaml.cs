using System.Windows;
using PcOptimizationTool.Helpers;
using PcOptimizationTool.ViewModels;

namespace PcOptimizationTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Instance.GetService<MainViewModel>();
        }
    }
}