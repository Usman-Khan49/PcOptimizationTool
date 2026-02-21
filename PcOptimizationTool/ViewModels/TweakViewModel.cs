using PcOptimizationTool.Enums;
using PcOptimizationTool.Models;

namespace PcOptimizationTool.ViewModels
{
    public class TweakViewModel : BaseViewModel
    {
        private readonly Tweak _tweak;
        private bool _isChecked;
        private TweakStatus _status;
        private TweakOption? _selectedOption;

        public TweakViewModel(Tweak tweak)
        {
            _tweak = tweak;
            _isChecked = tweak.IsEnabled;
            _status = tweak.Status;

            if (tweak.Type == TweakType.Choice && tweak.Configuration.Options?.Count > 0)
            {
                _selectedOption = tweak.Configuration.Options
                    .FirstOrDefault(o => o.Value?.ToString() == tweak.Configuration.ApplyValue?.ToString())
                    ?? tweak.Configuration.Options[0];

                _tweak.Configuration.ApplyValue = _selectedOption.Value;
            }
        }

        public Tweak Tweak => _tweak;
        public string Name => _tweak.Name;
        public string Description => _tweak.Description;
        public TweakType Type => _tweak.Type;
        public bool RequiresRestart => _tweak.RequiresRestart;
        public string? WarningMessage => _tweak.WarningMessage;
        public bool IsChoiceTweak => _tweak.Type == TweakType.Choice;
        public IReadOnlyList<TweakOption> Options => _tweak.Configuration.Options ?? [];

        public TweakOption? SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (SetProperty(ref _selectedOption, value) && value != null)
                    _tweak.Configuration.ApplyValue = value.Value;
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (SetProperty(ref _isChecked, value))
                    _tweak.IsEnabled = value;
            }
        }

        public TweakStatus Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                    _tweak.Status = value;
            }
        }
    }
}
