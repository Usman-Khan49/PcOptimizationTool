namespace PcOptimizationTool.Enums
{
    /// <summary>
    /// Type of tweak indicating how it should be applied
    /// </summary>
    public enum TweakType
    {
        Registry,
        WindowsSetting,
        Service,
        ScheduledTask,
        FileSystem,
        PowerShell,
        Combined,
        Choice
    }
}
