using PcOptimizationTool.Enums;
using PcOptimizationTool.Models;

namespace PcOptimizationTool.ViewModels
{
    public class TweakViewModel : BaseViewModel
    {
        private readonly Tweak _tweak;
        private bool _isChecked;
        private TweakStatus _status;

        public TweakViewModel(Tweak tweak)
        {
            _tweak = tweak;
            _isChecked = tweak.IsEnabled;
            _status = tweak.Status;
        }

        public Tweak Tweak => _tweak;
        public string Name => _tweak.Name;
        public string Description => _tweak.Description;
        public TweakType Type => _tweak.Type;
        public bool RequiresRestart => _tweak.RequiresRestart;
        public string? WarningMessage => _tweak.WarningMessage;

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
