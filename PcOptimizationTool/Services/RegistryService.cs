using Microsoft.Win32;
using PcOptimizationTool.Interfaces;

namespace PcOptimizationTool.Services
{
    /// <summary>
    /// Service for registry operations
    /// </summary>
    public class RegistryService : IRegistryService
    {
        public object? ReadValue(string keyPath, string valueName)
        {
            try
            {
                var (rootKey, subKeyPath) = ParseRegistryPath(keyPath);
                using var key = rootKey.OpenSubKey(subKeyPath, false);
                return key?.GetValue(valueName);
            }
            catch
            {
                return null;
            }
        }

        public bool WriteValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                var (rootKey, subKeyPath) = ParseRegistryPath(keyPath);
                using var key = rootKey.CreateSubKey(subKeyPath, true);
                key?.SetValue(valueName, value, valueKind);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteValue(string keyPath, string valueName)
        {
            try
            {
                var (rootKey, subKeyPath) = ParseRegistryPath(keyPath);
                using var key = rootKey.OpenSubKey(subKeyPath, true);
                key?.DeleteValue(valueName, false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool KeyExists(string keyPath)
        {
            try
            {
                var (rootKey, subKeyPath) = ParseRegistryPath(keyPath);
                using var key = rootKey.OpenSubKey(subKeyPath, false);
                return key != null;
            }
            catch
            {
                return false;
            }
        }

        public bool ValueExists(string keyPath, string valueName)
        {
            try
            {
                var (rootKey, subKeyPath) = ParseRegistryPath(keyPath);
                using var key = rootKey.OpenSubKey(subKeyPath, false);
                return key?.GetValue(valueName) != null;
            }
            catch
            {
                return false;
            }
        }

        private (RegistryKey rootKey, string subKeyPath) ParseRegistryPath(string fullPath)
        {
            var parts = fullPath.Split('\\', 2);
            var rootKeyName = parts[0];
            var subKeyPath = parts.Length > 1 ? parts[1] : string.Empty;

            RegistryKey rootKey = rootKeyName.ToUpper() switch
            {
                "HKEY_LOCAL_MACHINE" or "HKLM" => Registry.LocalMachine,
                "HKEY_CURRENT_USER" or "HKCU" => Registry.CurrentUser,
                "HKEY_CLASSES_ROOT" or "HKCR" => Registry.ClassesRoot,
                "HKEY_USERS" or "HKU" => Registry.Users,
                "HKEY_CURRENT_CONFIG" or "HKCC" => Registry.CurrentConfig,
                _ => throw new ArgumentException($"Unknown registry root: {rootKeyName}")
            };

            return (rootKey, subKeyPath);
        }
    }
}
