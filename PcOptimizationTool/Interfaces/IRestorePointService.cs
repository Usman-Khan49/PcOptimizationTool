namespace PcOptimizationTool.Interfaces
{
    public interface IRestorePointService
    {
        /// <summary>
        /// Creates a Windows system restore point with the given description
        /// </summary>
        Task<bool> CreateRestorePointAsync(string description);

        /// <summary>
        /// Returns true if the application is running with administrator privileges
        /// </summary>
        bool IsRunningAsAdmin();
    }
}
