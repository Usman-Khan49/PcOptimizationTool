using Microsoft.Win32;

namespace PcOptimizationTool.Interfaces
{
    /// <summary>
    /// Service for registry operations
    /// </summary>
    public interface IRegistryService
    {
        /// <summary>
        /// Read a registry value
        /// </summary>
        object? ReadValue(string keyPath, string valueName);
        
        /// <summary>
        /// Write a registry value
        /// </summary>
        bool WriteValue(string keyPath, string valueName, object value, RegistryValueKind valueKind);
        
        /// <summary>
        /// Delete a registry value
        /// </summary>
        bool DeleteValue(string keyPath, string valueName);
        
        /// <summary>
        /// Check if a registry key exists
        /// </summary>
        bool KeyExists(string keyPath);
        
        /// <summary>
        /// Check if a registry value exists
        /// </summary>
        bool ValueExists(string keyPath, string valueName);
    }
}
