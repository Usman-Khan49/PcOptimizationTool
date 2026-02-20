using System.Diagnostics;
using System.Security.Principal;
using PcOptimizationTool.Interfaces;

namespace PcOptimizationTool.Services
{
    public class RestorePointService : IRestorePointService
    {
        public bool IsRunningAsAdmin()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public async Task<bool> CreateRestorePointAsync(string description)
        {
            try
            {
                // Sanitize description to prevent command injection
                var sanitized = description.Replace("'", "").Replace("\"", "").Trim();

                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -NonInteractive -Command \"Enable-ComputerRestore -Drive 'C:\\'; Checkpoint-Computer -Description '{sanitized}' -RestorePointType 'MODIFY_SETTINGS'\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
