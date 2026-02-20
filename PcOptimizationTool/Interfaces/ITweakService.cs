using PcOptimizationTool.Enums;
using PcOptimizationTool.Models;

namespace PcOptimizationTool.Interfaces
{
    /// <summary>
    /// Service for managing and applying tweaks
    /// </summary>
    public interface ITweakService
    {
        /// <summary>
        /// Load all available tweaks
        /// </summary>
        Task<IEnumerable<Tweak>> LoadTweaksAsync();
        
        /// <summary>
        /// Apply a single tweak
        /// </summary>
        Task<TweakResult> ApplyTweakAsync(Tweak tweak);
        
        /// <summary>
        /// Undo a single tweak
        /// </summary>
        Task<TweakResult> UndoTweakAsync(Tweak tweak);
        
        /// <summary>
        /// Apply multiple tweaks
        /// </summary>
        Task<IEnumerable<TweakResult>> ApplyTweaksAsync(IEnumerable<Tweak> tweaks);
        
        /// <summary>
        /// Check the current status of a tweak
        /// </summary>
        Task<TweakStatus> GetTweakStatusAsync(Tweak tweak);

        /// <summary>
        /// Persist which tweaks are enabled/disabled to preferences.json
        /// </summary>
        Task SavePreferencesAsync(IEnumerable<Tweak> tweaks);
    }
}
