using PcOptimizationTool.Models;

namespace PcOptimizationTool.Interfaces
{
    /// <summary>
    /// Repository for loading and saving tweak definitions
    /// </summary>
    public interface ITweakRepository
    {
        /// <summary>
        /// Load all tweaks from storage (JSON file, database, etc.)
        /// </summary>
        Task<IEnumerable<Tweak>> GetAllTweaksAsync();
        
        /// <summary>
        /// Get a specific tweak by ID
        /// </summary>
        Task<Tweak?> GetTweakByIdAsync(string id);
        
        /// <summary>
        /// Save user preferences for enabled/disabled tweaks
        /// </summary>
        Task SaveTweakPreferencesAsync(IEnumerable<Tweak> tweaks);
        
        /// <summary>
        /// Load user preferences
        /// </summary>
        Task<Dictionary<string, bool>> LoadTweakPreferencesAsync();
    }
}
