using System.IO;
using Newtonsoft.Json;
using PcOptimizationTool.Interfaces;
using PcOptimizationTool.Models;

namespace PcOptimizationTool.Services
{
    /// <summary>
    /// Repository for loading and saving tweak definitions.
    /// Uses Newtonsoft.Json so object? fields (ApplyValue, UndoValue)
    /// deserialize as proper CLR primitives (long, string) instead of JsonElement.
    /// </summary>
    public class TweakRepository : ITweakRepository
    {
        private readonly string _tweaksFilePath;
        private readonly string _preferencesFilePath;

        public TweakRepository(string tweaksFilePath = "tweaks.json", string preferencesFilePath = "preferences.json")
        {
            _tweaksFilePath = tweaksFilePath;
            _preferencesFilePath = preferencesFilePath;
        }

        public async Task<IEnumerable<Tweak>> GetAllTweaksAsync()
        {
            try
            {
                if (!File.Exists(_tweaksFilePath))
                    return new List<Tweak>();

                var json = await File.ReadAllTextAsync(_tweaksFilePath);
                return JsonConvert.DeserializeObject<List<Tweak>>(json) ?? new List<Tweak>();
            }
            catch
            {
                return new List<Tweak>();
            }
        }

        public async Task<Tweak?> GetTweakByIdAsync(string id)
        {
            var tweaks = await GetAllTweaksAsync();
            return tweaks.FirstOrDefault(t => t.Id == id);
        }

        public async Task SaveTweakPreferencesAsync(IEnumerable<Tweak> tweaks)
        {
            try
            {
                var preferences = tweaks.ToDictionary(t => t.Id, t => t.IsEnabled);
                var json = JsonConvert.SerializeObject(preferences, Formatting.Indented);
                await File.WriteAllTextAsync(_preferencesFilePath, json);
            }
            catch { }
        }

        public async Task<Dictionary<string, bool>> LoadTweakPreferencesAsync()
        {
            try
            {
                if (!File.Exists(_preferencesFilePath))
                    return new Dictionary<string, bool>();

                var json = await File.ReadAllTextAsync(_preferencesFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, bool>>(json) ?? new Dictionary<string, bool>();
            }
            catch
            {
                return new Dictionary<string, bool>();
            }
        }
    }
}
