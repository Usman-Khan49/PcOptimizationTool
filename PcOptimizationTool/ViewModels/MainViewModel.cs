using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using PcOptimizationTool.Enums;
using PcOptimizationTool.Helpers;
using PcOptimizationTool.Interfaces;
using PcOptimizationTool.Views;

namespace PcOptimizationTool.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITweakService _tweakService;
        private readonly IRestorePointService _restorePointService;
        private readonly ILicenseService _licenseService;
        private bool _isLoading;
        private string _statusMessage = "Ready";
        private List<TweakViewModel> _lastAppliedTweaks = new();
        private int _totalTweaksCount;
        private int _appliedTweaksCount;
        private DateTime? _lastAppliedAt;
        private bool _restorePointPromptedThisSession;

        public MainViewModel(ITweakService tweakService, IRestorePointService restorePointService, ILicenseService licenseService)
        {
            _tweakService = tweakService;
            _restorePointService = restorePointService;
            _licenseService = licenseService;
            SystemTweaks = new ObservableCollection<TweakViewModel>();
            SecretSauceTweaks = new ObservableCollection<TweakViewModel>();
            ServiceTweaks = new ObservableCollection<TweakViewModel>();

            ApplySelectedTweaksCommand = new RelayCommand(async () => await ApplySelectedTweaksAsync());
            UndoSelectedTweaksCommand = new RelayCommand(async () => await UndoSelectedTweaksAsync());
            UndoLastChangesCommand = new RelayCommand(async () => await UndoLastChangesAsync());
            ShowActivationDialogCommand = new RelayCommand(ShowActivationDialog);

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

        public int TotalTweaksCount
        {
            get => _totalTweaksCount;
            set => SetProperty(ref _totalTweaksCount, value);
        }

        public int AppliedTweaksCount
        {
            get => _appliedTweaksCount;
            set
            {
                if (SetProperty(ref _appliedTweaksCount, value))
                {
                    OnPropertyChanged(nameof(OptimizationLevelText));
                    OnPropertyChanged(nameof(OptimizationProgressApplied));
                    OnPropertyChanged(nameof(OptimizationProgressRemaining));
                }
            }
        }

        public string OptimizationLevelText
        {
            get
            {
                if (_totalTweaksCount == 0) return "LVL 0";
                var pct = (double)_appliedTweaksCount / _totalTweaksCount;
                var level = pct switch
                {
                    0            => 0,
                    < 0.25       => 1,
                    < 0.50       => 2,
                    < 0.75       => 3,
                    < 1.0        => 4,
                    _            => 5
                };
                return $"LVL {level}";
            }
        }

        public GridLength OptimizationProgressApplied
        {
            get
            {
                if (_totalTweaksCount == 0) return new GridLength(1, GridUnitType.Star);
                return new GridLength(_appliedTweaksCount, GridUnitType.Star);
            }
        }

        public GridLength OptimizationProgressRemaining
        {
            get
            {
                if (_totalTweaksCount == 0) return new GridLength(1, GridUnitType.Star);
                var remaining = Math.Max(0, _totalTweaksCount - _appliedTweaksCount);
                return new GridLength(remaining, GridUnitType.Star);
            }
        }

        public string LastUpdatedText
        {
            get
            {
                if (_lastAppliedAt == null) return "LAST UPDATE: NEVER";
                var elapsed = DateTime.Now - _lastAppliedAt.Value;
                if (elapsed.TotalSeconds < 60)  return "LAST UPDATE: JUST NOW";
                if (elapsed.TotalMinutes < 60)  return $"LAST UPDATE: {(int)elapsed.TotalMinutes} MIN AGO";
                if (elapsed.TotalHours < 24)    return $"LAST UPDATE: {(int)elapsed.TotalHours} HR AGO";
                return $"LAST UPDATE: {(int)elapsed.TotalDays} DAY(S) AGO";
            }
        }

        public bool IsProUnlocked => _licenseService.IsActivated;
        public bool IsProLocked   => !_licenseService.IsActivated;

        public ICommand ApplySelectedTweaksCommand { get; }
        public ICommand UndoSelectedTweaksCommand { get; }
        public ICommand UndoLastChangesCommand { get; }
        public ICommand ShowActivationDialogCommand { get; }

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
                    vm.PropertyChanged += (_, e) =>
                    {
                        if (e.PropertyName == nameof(TweakViewModel.IsChecked))
                            RefreshCounts();
                    };

                    if (tweak.Panel == 2)
                        SystemTweaks.Add(vm);       // Left         — Big Boy Tweaks
                    else if (tweak.Panel == 3)
                        SecretSauceTweaks.Add(vm);  // Right top    — Secret Sauce
                    else
                        ServiceTweaks.Add(vm);      // Right bottom — Windows Tweaks
                }

                RefreshCounts();
                StatusMessage = $"Loaded {TotalTweaksCount} tweaks — Ready";
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

            // Prompt for restore point once per session
            if (!_restorePointPromptedThisSession)
            {
                _restorePointPromptedThisSession = true;
                var dialog = new RestorePointDialog(_restorePointService)
                {
                    Owner = Application.Current.MainWindow
                };
                // If the user cancels the dialog entirely we still continue applying
                dialog.ShowDialog();
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

                _lastAppliedAt = DateTime.Now;
                OnPropertyChanged(nameof(LastUpdatedText));

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
                    private void RefreshCounts()
                    {
                        var all = SystemTweaks.Concat(SecretSauceTweaks).Concat(ServiceTweaks);
                        TotalTweaksCount = all.Count();
                        AppliedTweaksCount = all.Count(t => t.IsChecked);
                    }

                    private static void SetAllChecked(IEnumerable<TweakViewModel> tweaks, bool value)
                    {
                        foreach (var vm in tweaks)
                            vm.IsChecked = value;
                    }

        private void ShowActivationDialog()
        {
            var dialog = new ActivationKeyDialog(_licenseService)
            {
                Owner = Application.Current.MainWindow
            };
            if (dialog.ShowDialog() == true)
            {
                OnPropertyChanged(nameof(IsProUnlocked));
                OnPropertyChanged(nameof(IsProLocked));
            }
        }
    }
}
