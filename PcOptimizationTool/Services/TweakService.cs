using System.Diagnostics;
using System.Text;
using Microsoft.Win32;
using PcOptimizationTool.Enums;
using PcOptimizationTool.Interfaces;
using PcOptimizationTool.Models;

namespace PcOptimizationTool.Services
{
    public class TweakService : ITweakService
    {
        private readonly ITweakRepository _tweakRepository;
        private readonly IRegistryService _registryService;

        public TweakService(ITweakRepository tweakRepository, IRegistryService registryService)
        {
            _tweakRepository = tweakRepository;
            _registryService = registryService;
        }

        public async Task<IEnumerable<Tweak>> LoadTweaksAsync()
        {
            var tweaks = (await _tweakRepository.GetAllTweaksAsync()).ToList();
            var preferences = await _tweakRepository.LoadTweakPreferencesAsync();

            foreach (var tweak in tweaks)
            {
                if (preferences.TryGetValue(tweak.Id, out bool isEnabled))
                    tweak.IsEnabled = isEnabled;

                tweak.Status = await GetTweakStatusAsync(tweak);
            }

            return tweaks;
        }

        public async Task<TweakResult> ApplyTweakAsync(Tweak tweak)
        {
            return tweak.Type switch
            {
                TweakType.Registry or TweakType.Choice => ApplyRegistryTweak(tweak),
                TweakType.PowerShell => await RunPowerShellTweakAsync(tweak, apply: true),
                TweakType.Service => await RunServiceTweakAsync(tweak, apply: true),
                TweakType.Combined => await ApplyCombinedTweakAsync(tweak),
                _ => Fail(tweak.Id, $"Tweak type '{tweak.Type}' is not supported yet")
            };
        }

        public async Task<TweakResult> UndoTweakAsync(Tweak tweak)
        {
            return tweak.Type switch
            {
                TweakType.Registry or TweakType.Choice => UndoRegistryTweak(tweak),
                TweakType.PowerShell => await RunPowerShellTweakAsync(tweak, apply: false),
                TweakType.Service => await RunServiceTweakAsync(tweak, apply: false),
                TweakType.Combined => await UndoCombinedTweakAsync(tweak),
                _ => Fail(tweak.Id, $"Tweak type '{tweak.Type}' is not supported yet")
            };
        }

        public async Task<IEnumerable<TweakResult>> ApplyTweaksAsync(IEnumerable<Tweak> tweaks)
        {
            var results = new List<TweakResult>();
            foreach (var tweak in tweaks)
                results.Add(await ApplyTweakAsync(tweak));
            return results;
        }

        public async Task SavePreferencesAsync(IEnumerable<Tweak> tweaks)
        {
            await _tweakRepository.SaveTweakPreferencesAsync(tweaks);
        }

        public async Task<TweakStatus> GetTweakStatusAsync(Tweak tweak)
        {
            if (tweak.Type is not (TweakType.Registry or TweakType.Combined or TweakType.Choice))
                return TweakStatus.Unknown;

            var config = tweak.Configuration;

            if (config.RegistryEntries is { Count: > 0 })
            {
                foreach (var entry in config.RegistryEntries)
                {
                    if (entry.ApplyValue == null) continue;
                    var kind = ParseValueKind(entry.ValueType);
                    var current = _registryService.ReadValue(entry.Path, entry.Name);
                    if (current == null) return TweakStatus.NotApplied;
                    if (current.ToString() != ConvertToRegistryValue(entry.ApplyValue, kind).ToString())
                        return TweakStatus.NotApplied;
                }
                await Task.CompletedTask;
                return TweakStatus.Applied;
            }

            if (config.RegistryPath == null || config.ValueName == null || config.ApplyValue == null)
                return TweakStatus.Unknown;

            var singleCurrent = _registryService.ReadValue(config.RegistryPath, config.ValueName);
            if (singleCurrent == null)
                return TweakStatus.NotApplied;

            var singleKind = ParseValueKind(config.ValueType ?? "REG_DWORD");
            var expected = ConvertToRegistryValue(config.ApplyValue, singleKind);

            await Task.CompletedTask;
            return singleCurrent.ToString() == expected.ToString() ? TweakStatus.Applied : TweakStatus.NotApplied;
        }

        // ── Registry ─────────────────────────────────────────────────────────

        private TweakResult ApplyRegistryTweak(Tweak tweak)
        {
            var config = tweak.Configuration;

            if (config.RegistryEntries is { Count: > 0 })
            {
                foreach (var entry in config.RegistryEntries)
                {
                    var kind = ParseValueKind(entry.ValueType);
                    bool ok = entry.ApplyValue == null
                        ? _registryService.DeleteValue(entry.Path, entry.Name)
                        : _registryService.WriteValue(entry.Path, entry.Name, ConvertToRegistryValue(entry.ApplyValue, kind), kind);
                    if (!ok)
                        return Fail(tweak.Id, $"Failed to write registry value '{entry.Name}' — make sure the app is running as administrator");
                }
                return Ok(tweak.Id, "Registry tweak applied");
            }

            if (config.RegistryPath == null || config.ValueName == null)
                return Fail(tweak.Id, "Invalid registry configuration: missing path or value name");

            var singleKind = ParseValueKind(config.ValueType ?? "REG_DWORD");
            bool success = config.ApplyValue == null
                ? _registryService.DeleteValue(config.RegistryPath, config.ValueName)
                : _registryService.WriteValue(config.RegistryPath, config.ValueName, ConvertToRegistryValue(config.ApplyValue, singleKind), singleKind);

            return success
                ? Ok(tweak.Id, "Registry tweak applied")
                : Fail(tweak.Id, "Failed to write registry value — make sure the app is running as administrator");
        }

        private TweakResult UndoRegistryTweak(Tweak tweak)
        {
            var config = tweak.Configuration;

            if (config.RegistryEntries is { Count: > 0 })
            {
                foreach (var entry in config.RegistryEntries)
                {
                    var kind = ParseValueKind(entry.ValueType);
                    bool ok = entry.UndoValue == null
                        ? _registryService.DeleteValue(entry.Path, entry.Name)
                        : _registryService.WriteValue(entry.Path, entry.Name, ConvertToRegistryValue(entry.UndoValue, kind), kind);
                    if (!ok)
                        return Fail(tweak.Id, $"Failed to revert registry value '{entry.Name}'");
                }
                return Ok(tweak.Id, "Registry tweak reverted");
            }

            if (config.RegistryPath == null || config.ValueName == null)
                return Fail(tweak.Id, "Invalid registry configuration: missing path or value name");

            var singleKind = ParseValueKind(config.ValueType ?? "REG_DWORD");
            bool success = config.UndoValue == null
                ? _registryService.DeleteValue(config.RegistryPath, config.ValueName)
                : _registryService.WriteValue(config.RegistryPath, config.ValueName, ConvertToRegistryValue(config.UndoValue, singleKind), singleKind);

            return success
                ? Ok(tweak.Id, "Registry tweak reverted")
                : Fail(tweak.Id, "Failed to revert registry value");
        }

        // ── Combined (Registry + PowerShell) ────────────────────────────────

        private async Task<TweakResult> ApplyCombinedTweakAsync(Tweak tweak)
        {
            var regResult = ApplyRegistryTweak(tweak);
            if (!regResult.Success) return regResult;

            if (!string.IsNullOrWhiteSpace(tweak.Configuration.PowerShellCommand))
                return await RunPowerShellTweakAsync(tweak, apply: true);

            return Ok(tweak.Id, "Combined tweak applied");
        }

        private async Task<TweakResult> UndoCombinedTweakAsync(Tweak tweak)
        {
            var regResult = UndoRegistryTweak(tweak);
            if (!regResult.Success) return regResult;

            if (!string.IsNullOrWhiteSpace(tweak.Configuration.PowerShellUndoCommand))
                return await RunPowerShellTweakAsync(tweak, apply: false);

            return Ok(tweak.Id, "Combined tweak reverted");
        }

        // ── PowerShell ────────────────────────────────────────────────────────

        private async Task<TweakResult> RunPowerShellTweakAsync(Tweak tweak, bool apply)
        {
            var command = apply
                ? tweak.Configuration.PowerShellCommand
                : tweak.Configuration.PowerShellUndoCommand;

            if (string.IsNullOrWhiteSpace(command))
                return Fail(tweak.Id, $"No PowerShell {(apply ? "apply" : "undo")} command configured");

            // Suppress progress stream (prevents CLIXML leaking into stdout)
            // and suppress errors globally — individual commands add their own handling
            var fullCommand = $"$ProgressPreference='SilentlyContinue'; $ErrorActionPreference='SilentlyContinue'; {command}";
            var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(fullCommand));

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -NonInteractive -EncodedCommand {encoded}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            process.Start();

            // Read both streams concurrently to avoid deadlocks
            var errorTask = process.StandardError.ReadToEndAsync();
            var outputTask = process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            var error = await errorTask;
            var output = await outputTask;

            if (process.ExitCode == 0)
                return Ok(tweak.Id, $"PowerShell tweak {(apply ? "applied" : "reverted")}");

            // Filter CLIXML progress output — it is not a real error
            static bool IsClixml(string s) => s.TrimStart().StartsWith("<Objs");
            var details = !string.IsNullOrWhiteSpace(error) && !IsClixml(error)
                ? error.Trim()
                : !string.IsNullOrWhiteSpace(output) && !IsClixml(output)
                    ? output.Trim()
                    : "Command exited with errors (some items may have been locked)";

            return new TweakResult
            {
                TweakId = tweak.Id,
                Success = false,
                Message = "PowerShell command failed",
                ErrorDetails = details
            };
        }

        // ── Service ───────────────────────────────────────────────────────────

        private async Task<TweakResult> RunServiceTweakAsync(Tweak tweak, bool apply)
        {
            var config = tweak.Configuration;

            if (config.ServiceEntries is { Count: > 0 })
            {
                foreach (var entry in config.ServiceEntries)
                {
                    var targetType = apply ? entry.StartupType : entry.OriginalType;
                    await RunProcessAsync("sc.exe", $"config \"{entry.Name}\" start= {MapServiceStartType(targetType)}");
                }
                return Ok(tweak.Id, $"Services {(apply ? "configured" : "restored")}");
            }

            if (string.IsNullOrWhiteSpace(config.ServiceName))
                return Fail(tweak.Id, "No service name configured");

            var targetState = apply
                ? config.ServiceState ?? "disabled"
                : config.UndoValue?.ToString() ?? "demand";

            var scStartType = MapServiceStartType(targetState);

            // Change startup type
            await RunProcessAsync("sc.exe", $"config \"{config.ServiceName}\" start= {scStartType}");

            // Stop the service if we are disabling it
            if (apply && targetState.Equals("disabled", StringComparison.OrdinalIgnoreCase))
                await RunProcessAsync("sc.exe", $"stop \"{config.ServiceName}\"");

            // Start the service if we are restoring it
            if (!apply && !targetState.Equals("disabled", StringComparison.OrdinalIgnoreCase))
                await RunProcessAsync("sc.exe", $"start \"{config.ServiceName}\"");

            return Ok(tweak.Id, $"Service '{config.ServiceName}' {(apply ? "disabled" : "restored")}");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static async Task<int> RunProcessAsync(string fileName, string arguments)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode;
        }

        private static RegistryValueKind ParseValueKind(string valueType) =>
            valueType.ToUpperInvariant() switch
            {
                "REG_DWORD" or "DWORD" => RegistryValueKind.DWord,
                "REG_QWORD" or "QWORD" => RegistryValueKind.QWord,
                "REG_SZ" or "SZ" or "STRING" => RegistryValueKind.String,
                "REG_EXPAND_SZ" or "EXPAND_SZ" or "EXPANDSTRING" => RegistryValueKind.ExpandString,
                "REG_BINARY" or "BINARY" => RegistryValueKind.Binary,
                "REG_MULTI_SZ" or "MULTI_SZ" or "MULTISTRING" => RegistryValueKind.MultiString,
                _ => RegistryValueKind.DWord
            };

        /// <summary>
        /// Converts a deserialized JSON value (Newtonsoft long/string) to the correct
        /// CLR type expected by the Windows Registry API.
        /// </summary>
        private static object ConvertToRegistryValue(object value, RegistryValueKind kind) =>
            kind switch
            {
                RegistryValueKind.DWord => Convert.ToInt32(value),
                RegistryValueKind.QWord => Convert.ToInt64(value),
                RegistryValueKind.String or RegistryValueKind.ExpandString => value.ToString() ?? string.Empty,
                _ => value
            };

        private static string MapServiceStartType(string state) =>
            state.ToLowerInvariant() switch
            {
                "disabled" => "disabled",
                "manual" or "demand" => "demand",
                "automatic" or "auto" => "auto",
                "automaticdelayedstart" or "delayed" or "automatic (delayed start)" => "delayed-auto",
                _ => "demand"
            };

        private static TweakResult Ok(string id, string message) =>
            new() { TweakId = id, Success = true, Message = message };

        private static TweakResult Fail(string id, string message) =>
            new() { TweakId = id, Success = false, Message = message };
    }
}
