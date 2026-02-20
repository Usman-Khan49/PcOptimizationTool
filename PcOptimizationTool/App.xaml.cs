using System.Configuration;
using System.Data;
using System.Windows;
using PcOptimizationTool.Helpers;
using PcOptimizationTool.Interfaces;
using PcOptimizationTool.Services;
using PcOptimizationTool.ViewModels;

namespace PcOptimizationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;

            RegisterServices();
        }

        private void RegisterServices()
        {
            var serviceLocator = ServiceLocator.Instance;
            
            // Register repositories
            var tweakRepository = new TweakRepository("Data/tweaks.json", "Data/preferences.json");
            serviceLocator.RegisterService<ITweakRepository>(tweakRepository);
            
            // Register services
            var registryService = new RegistryService();
            serviceLocator.RegisterService<IRegistryService>(registryService);
            
            var tweakService = new TweakService(tweakRepository, registryService);
            serviceLocator.RegisterService<ITweakService>(tweakService);

            var restorePointService = new RestorePointService();
            serviceLocator.RegisterService<IRestorePointService>(restorePointService);

            // Register ViewModels
            var mainViewModel = new MainViewModel(tweakService);
            serviceLocator.RegisterService(mainViewModel);
        }
    }
}
