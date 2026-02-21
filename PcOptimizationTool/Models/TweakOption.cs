namespace PcOptimizationTool.Models
{
    /// <summary>
    /// A single selectable value for a Choice-type tweak
    /// </summary>
    public class TweakOption
    {
        public string Label { get; set; } = string.Empty;
        public object? Value { get; set; }
    }
}
