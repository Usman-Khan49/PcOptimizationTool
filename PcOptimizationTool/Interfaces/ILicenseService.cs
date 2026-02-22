using PcOptimizationTool.Enums;

namespace PcOptimizationTool.Interfaces
{
    public interface ILicenseService
    {
        LicenseStatus CurrentStatus { get; }
        bool IsActivated { get; }

        /// <summary>
        /// Verifies the RSA signature, persists the key to AppData (encrypted),
        /// and updates CurrentStatus if valid.
        /// </summary>
        bool ValidateAndActivate(string licenseKey);
    }
}
