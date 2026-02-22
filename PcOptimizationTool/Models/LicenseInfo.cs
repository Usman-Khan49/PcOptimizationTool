namespace PcOptimizationTool.Models
{
    public class LicenseInfo
    {
        public string LicenseKey  { get; set; } = string.Empty;
        public string Status      { get; set; } = "NotActivated";
        public DateTime ActivatedAt { get; set; }
    }
}
