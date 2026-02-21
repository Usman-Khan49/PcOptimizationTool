using PcOptimizationTool.Enums;

namespace PcOptimizationTool.Models
{
    /// <summary>
    /// Represents a single optimization tweak
    /// </summary>
    public class Tweak
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TweakCategory Category { get; set; }
        public TweakType Type { get; set; }
        public TweakStatus Status { get; set; }
        public bool IsEnabled { get; set; }
        public bool RequiresRestart { get; set; }
        public string? WarningMessage { get; set; }

        /// <summary>
        /// 1 = Windows Tweaks column, 2 = Big Boy Tweaks column, 3 = Secret Sauce column
        /// </summary>
        public int Panel { get; set; } = 1;

        /// <summary>
        /// Configuration data specific to the tweak type (JSON serialized)
        /// </summary>
        public TweakConfiguration Configuration { get; set; } = new();
    }
}
