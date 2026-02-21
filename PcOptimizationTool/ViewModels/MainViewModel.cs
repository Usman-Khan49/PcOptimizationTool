using System.Collections.ObjectModel;
using System.Windows.Input;
using PcOptimizationTool.Enums;
using PcOptimizationTool.Helpers;
using PcOptimizationTool.Interfaces;

namespace PcOptimizationTool.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITweakService _tweakService;
        private bool _isLoading;
        private string _statusMessage = "Ready";
        private List<TweakViewModel> _lastAppliedTweaks = new();

        public MainViewModel(ITweakService tweakService)
        {
            _tweakService = tweakService;
            SystemTweaks = new ObservableCollection<TweakViewModel>();
            SecretSauceTweaks = new ObservableCollection<TweakViewModel>();
            ServiceTweaks = new ObservableCollection<TweakViewModel>();

            ApplySelectedTweaksCommand = new RelayCommand(async () => await ApplySelectedTweaksAsync());
            UndoSelectedTweaksCommand = new RelayCommand(async () => await UndoSelectedTweaksAsync());
            UndoLastChangesCommand = new RelayCommand(async () => await UndoLastChangesAsync());

            SelectAllSystemCommand       = new RelayCommand(() => SetAllChecked(SystemTweaks, true));
            DeselectAllSystemCommand     = new RelayCommand(() => SetAllChecked(SystemTweaks, false));
            SelectAllSecretSauceCommand  = new RelayCommand(() => SetAllChecked(SecretSauceTweaks, true));
            DeselectAllSecretSauceCommand = new RelayCommand(() => SetAllChecked(SecretSauceTweaks, false));
            SelectAllServiceCommand      = new RelayCommand(() => SetAllChecked(ServiceTweaks, true));
            DeselectAllServiceCommand    = new RelayCommand(() => SetAllChecked(ServiceTweaks, false));

            _ = LoadTweaksAsync();
        }

        /// <summary>Left column — Big Boy Tweaks</summary>
        public ObservableCollection<TweakViewModel> SystemTweaks { get; }

        /// <summary>Right column top — Secret Sauce tweaks</summary>
        public ObservableCollection<TweakViewModel> SecretSauceTweaks { get; }

        /// <summary>Right column bottom — Windows Tweaks</summary>
        public ObservableCollection<TweakViewModel> ServiceTweaks { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand ApplySelectedTweaksCommand { get; }
        public ICommand UndoSelectedTweaksCommand { get; }
        public ICommand UndoLastChangesCommand { get; }

        public ICommand SelectAllSystemCommand { get; }
        public ICommand DeselectAllSystemCommand { get; }
        public ICommand SelectAllSecretSauceCommand { get; }
        public ICommand DeselectAllSecretSauceCommand { get; }
        public ICommand SelectAllServiceCommand { get; }
        public ICommand DeselectAllServiceCommand { get; }

        private async Task LoadTweaksAsync()
        {
            IsLoading = true;
            StatusMessage = "Loading tweaks...";

            try
            {
                var tweaks = await _tweakService.LoadTweaksAsync();

                SystemTweaks.Clear();
                SecretSauceTweaks.Clear();
                ServiceTweaks.Clear();

                foreach (var tweak in tweaks)
                {
                    var vm = new TweakViewModel(tweak);

                    if (tweak.Panel == 2)
                        SystemTweaks.Add(vm);       // Left         — Big Boy Tweaks
                    else if (tweak.Panel == 3)
                        SecretSauceTweaks.Add(vm);  // Right top    — Secret Sauce
                    else
                        ServiceTweaks.Add(vm);      // Right bottom — Windows Tweaks
                }

                StatusMessage = $"Loaded {SystemTweaks.Count + SecretSauceTweaks.Count + ServiceTweaks.Count} tweaks — Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading tweaks: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ApplySelectedTweaksAsync()
        {
            var selected = SystemTweaks.Concat(SecretSauceTweaks).Concat(ServiceTweaks).Where(t => t.IsChecked).ToList();

            if (selected.Count == 0)
            {
                StatusMessage = "No tweaks selected.";
                return;
            }

            IsLoading = true;
            StatusMessage = $"Applying {selected.Count} tweak(s)...";
            _lastAppliedTweaks = new List<TweakViewModel>(selected);

            try
            {
                var results = (await _tweakService.ApplyTweaksAsync(selected.Select(v => v.Tweak))).ToList();
                var successCount = results.Count(r => r.Success);
                var failCount = results.Count - successCount;
                var firstError = results.FirstOrDefault(r => !r.Success);

                if (failCount == 0)
                    StatusMessage = $"✓ Applied {successCount} tweak(s) successfully";
                else if (firstError?.ErrorDetails is { Length: > 0 } details)
                    StatusMessage = $"Applied {successCount}/{selected.Count} — Error: {details}";
                else
                    StatusMessage = $"Applied {successCount}/{selected.Count} — {firstError?.Message ?? "unknown error"}";

                await _tweakService.SavePreferencesAsync(
                    SystemTweaks.Concat(SecretSauceTweaks).Concat(ServiceTweaks).Select(v => v.Tweak));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UndoSelectedTweaksAsync()
        {
            var selected = SystemTweaks.Concat(SecretSauceTweaks).Concat(ServiceTweaks).Where(t => t.IsChecked).ToList();

            if (selected.Count == 0)
            {
                StatusMessage = "No tweaks selected.";
                return;
            }

            IsLoading = true;
            StatusMessage = $"Undoing {selected.Count} tweak(s)...";

            try
            {
                int successCount = 0;
                foreach (var vm in selected)
                {
                    var result = await _tweakService.UndoTweakAsync(vm.Tweak);
                    if (result.Success) successCount++;
                }

                StatusMessage = $"✓ Undone {successCount}/{selected.Count} tweak(s)";

                await _tweakService.SavePreferencesAsync(
                    SystemTweaks.Concat(SecretSauceTweaks).Concat(ServiceTweaks).Select(v => v.Tweak));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UndoLastChangesAsync()
        {
            if (_lastAppliedTweaks.Count == 0)
            {
                StatusMessage = "No recent changes to undo.";
                return;
            }

            IsLoading = true;
            StatusMessage = $"Reverting last {_lastAppliedTweaks.Count} change(s)...";

            try
            {
                int successCount = 0;
                foreach (var vm in _lastAppliedTweaks)
                {
                    var result = await _tweakService.UndoTweakAsync(vm.Tweak);
                    if (result.Success) successCount++;
                }

                StatusMessage = $"✓ Reverted {successCount}/{_lastAppliedTweaks.Count} tweak(s)";
                _lastAppliedTweaks.Clear();

                await _tweakService.SavePreferencesAsync(
                    SystemTweaks.Concat(SecretSauceTweaks).Concat(ServiceTweaks).Select(v => v.Tweak));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
            private static void SetAllChecked(IEnumerable<TweakViewModel> tweaks, bool value)
            {
                foreach (var vm in tweaks)
                    vm.IsChecked = value;
            }
        }
    }
