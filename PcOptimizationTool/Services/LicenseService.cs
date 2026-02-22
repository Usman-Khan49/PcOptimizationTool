using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PcOptimizationTool.Enums;
using PcOptimizationTool.Interfaces;
using PcOptimizationTool.Models;

namespace PcOptimizationTool.Services
{
    public class LicenseService : ILicenseService
    {
        // ── Storage paths ──────────────────────────────────────────────────────
        private static readonly string AppDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "HarrisTweaks");

        private static readonly string LicenseFilePath =
            Path.Combine(AppDataFolder, "license.dat");

        // ── Encryption key (AES-256) ───────────────────────────────────────────
        // The real security comes from RSA; this just prevents casual file editing.
        private static readonly byte[] EncryptionKey =
            SHA256.HashData(Encoding.UTF8.GetBytes("HarrisTweaks-OfflineLicense-2024-Xk9!mP#2"));

        // ── The fixed payload that is signed to produce a license key ─────────
        private const string LicensePayload = "HARRIS-TWEAKS-PRO";

        // ── Embedded public key resource name ─────────────────────────────────
        private const string PublicKeyResourceName = "PcOptimizationTool.Resources.public_key.pem";

        // ── State ──────────────────────────────────────────────────────────────
        private LicenseStatus _currentStatus = LicenseStatus.NotActivated;

        public LicenseStatus CurrentStatus => _currentStatus;
        public bool IsActivated => _currentStatus == LicenseStatus.Activated;

        public LicenseService()
        {
            LoadSavedLicense();
        }

        // ── Public API ─────────────────────────────────────────────────────────

        public bool ValidateAndActivate(string licenseKey)
        {
            if (!VerifySignature(licenseKey))
                return false;

            try
            {
                Directory.CreateDirectory(AppDataFolder);

                var info = new LicenseInfo
                {
                    LicenseKey  = licenseKey,
                    Status      = "Activated",
                    ActivatedAt = DateTime.UtcNow
                };

                var json      = JsonSerializer.Serialize(info);
                var encrypted = Encrypt(json);
                File.WriteAllBytes(LicenseFilePath, encrypted);

                _currentStatus = LicenseStatus.Activated;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ── Private helpers ────────────────────────────────────────────────────

        private void LoadSavedLicense()
        {
            try
            {
                if (!File.Exists(LicenseFilePath))
                {
                    _currentStatus = LicenseStatus.NotActivated;
                    return;
                }

                var encrypted = File.ReadAllBytes(LicenseFilePath);
                var json      = Decrypt(encrypted);
                var info      = JsonSerializer.Deserialize<LicenseInfo>(json);

                if (info?.Status == "Activated" && !string.IsNullOrWhiteSpace(info.LicenseKey)
                    && VerifySignature(info.LicenseKey))
                {
                    _currentStatus = LicenseStatus.Activated;
                }
                else
                {
                    _currentStatus = LicenseStatus.NotActivated;
                }
            }
            catch
            {
                _currentStatus = LicenseStatus.NotActivated;
            }
        }

        private bool VerifySignature(string licenseKey)
        {
            try
            {
                var publicKeyPem = LoadPublicKeyPem();
                if (string.IsNullOrWhiteSpace(publicKeyPem) || publicKeyPem.StartsWith("PLACEHOLDER"))
                    return false;

                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                var payload   = Encoding.UTF8.GetBytes(LicensePayload);
                var signature = Convert.FromBase64String(licenseKey.Trim());

                return rsa.VerifyData(payload, signature,
                                      HashAlgorithmName.SHA256,
                                      RSASignaturePadding.Pss);
            }
            catch
            {
                return false;
            }
        }

        private static string LoadPublicKeyPem()
        {
            var assembly = typeof(LicenseService).Assembly;

            // Try exact resource name first
            var stream = assembly.GetManifestResourceStream(PublicKeyResourceName);

            // Fallback: search for any resource ending with public_key.pem
            if (stream == null)
            {
                var match = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("public_key.pem", StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    stream = assembly.GetManifestResourceStream(match);
            }

            if (stream == null) return string.Empty;

            using (stream)
            using (var reader = new StreamReader(stream))
            {
                var pem = reader.ReadToEnd().Trim();

                // Auto-wrap raw Base64 with PEM headers if missing
                if (!pem.StartsWith("-----"))
                    pem = $"-----BEGIN PUBLIC KEY-----\n{pem}\n-----END PUBLIC KEY-----";

                return pem;
            }
        }

        // ── AES-256-CBC encryption ─────────────────────────────────────────────

        private static byte[] Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = EncryptionKey;
            aes.GenerateIV();

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);          // first 16 bytes = IV

            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            var bytes = Encoding.UTF8.GetBytes(plainText);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        private static string Decrypt(byte[] cipherData)
        {
            using var aes = Aes.Create();
            aes.Key = EncryptionKey;

            var iv = new byte[16];
            Array.Copy(cipherData, 0, iv, 0, 16);
            aes.IV = iv;

            using var ms     = new MemoryStream(cipherData, 16, cipherData.Length - 16);
            using var cs     = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);
            return reader.ReadToEnd();
        }
    }
}
