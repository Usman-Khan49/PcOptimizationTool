namespace PcOptimizationTool.Models
{
    /// <summary>
    /// Base configuration for a tweak
    /// </summary>
    public class TweakConfiguration
    {
        /// <summary>
        /// Registry path (for Registry tweaks)
        /// </summary>
        public string? RegistryPath { get; set; }
        
        /// <summary>
        /// Registry value name
        /// </summary>
        public string? ValueName { get; set; }
        
        /// <summary>
        /// Value to set when applying the tweak
        /// </summary>
        public object? ApplyValue { get; set; }
        
        /// <summary>
        /// Value to set when undoing the tweak (null to delete)
        /// </summary>
        public object? UndoValue { get; set; }
        
        /// <summary>
        /// Registry value type (REG_DWORD, REG_SZ, etc.)
        /// </summary>
        public string? ValueType { get; set; }
        
        /// <summary>
        /// PowerShell command to execute (for PowerShell tweaks)
        /// </summary>
        public string? PowerShellCommand { get; set; }
        
        /// <summary>
        /// Undo command for PowerShell tweaks
        /// </summary>
        public string? PowerShellUndoCommand { get; set; }
        
        /// <summary>
        /// Service name (for Service tweaks)
        /// </summary>
        public string? ServiceName { get; set; }
        
        /// <summary>
        /// Desired service state
        /// </summary>
        public string? ServiceState { get; set; }

        /// <summary>
        /// Multiple registry entries (used when a tweak spans more than one key/value)
        /// </summary>
        public List<RegistryEntry>? RegistryEntries { get; set; }

        /// <summary>
        /// Multiple service entries (used when a tweak configures more than one service)
        /// </summary>
        public List<ServiceEntry>? ServiceEntries { get; set; }
    }
}
