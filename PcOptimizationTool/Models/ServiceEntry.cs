namespace PcOptimizationTool.Models
{
    /// <summary>
    /// A single service entry used within a multi-service tweak
    /// </summary>
    public class ServiceEntry
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Startup type to apply (Manual, Automatic, AutomaticDelayedStart, Disabled)
        /// </summary>
        public string StartupType { get; set; } = "Manual";

        /// <summary>
        /// Original startup type to restore on undo
        /// </summary>
        public string OriginalType { get; set; } = "Manual";
    }
}
