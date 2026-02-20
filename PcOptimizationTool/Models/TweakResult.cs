namespace PcOptimizationTool.Models
{
    /// <summary>
    /// Result of applying or undoing a tweak
    /// </summary>
    public class TweakResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public string TweakId { get; set; } = string.Empty;
    }
}
