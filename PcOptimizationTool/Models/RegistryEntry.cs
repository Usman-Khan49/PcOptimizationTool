namespace PcOptimizationTool.Models
{
    /// <summary>
    /// A single registry key/value pair used within a multi-entry registry tweak
    /// </summary>
    public class RegistryEntry
    {
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Value to write when applying the tweak (null to delete)
        /// </summary>
        public object? ApplyValue { get; set; }

        /// <summary>
        /// Value to write when undoing the tweak (null to delete)
        /// </summary>
        public object? UndoValue { get; set; }

        public string ValueType { get; set; } = "DWord";
    }
}
