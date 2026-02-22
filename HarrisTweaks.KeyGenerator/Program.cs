using System.Security.Cryptography;
using System.Text;

// ══════════════════════════════════════════════════════════════════════════════
//  HarrisTweaks License Key Generator
//  ─────────────────────────────────
//  Uses the hardcoded RSA-2048 private key to sign a fixed payload.
//  Each run produces a unique license key (RSA-PSS uses random salt).
//  The matching public key is embedded in the WPF app.
// ══════════════════════════════════════════════════════════════════════════════

const string LicensePayload = "HARRIS-TWEAKS-PRO";

const string PrivateKeyPem = """
    -----BEGIN RSA PRIVATE KEY-----
    MIIEpAIBAAKCAQEA08/Vw6YEDtGj83TQc1A2BIMw90aq1+uAAtBKVvImJ8so1gyj
    pKRGv0ed6/Js0PZ8RKGW2uCDCxWAc6k3JGkuMIo92B9ydLwD876xCBfdPK7nrVkU
    kKs6L/cbmU/aVrJ8hhvy+e2Y4S9wEd6WBDPsOXCigp97rpO7RUGtTh4SHAd3OxeW
    iga/4HXvkb4+9Z6IYOkQdSK6xfkMakQOqmFzjqlAnIWGNFVGPd/ONrgesSQUIhSw
    /4ufreb5LACb16qKw7dmDhvm9FYEffI8e9LVSJEuqMpnuEGB0vvBu/SMFBt/srzy
    uiLmu1JHyv1cDei1pD+H8O9BqGzphuvZQ1+uDQIDAQABAoIBAQDDKFvtSKm0fbRy
    PM9TcDRVNAuJyrLe/z2TWhckxTgisi2OC1c7+6Cz3WjEXTmlz3XwRnKYeQRE0iro
    ZqFICyFj1/vjhhYPOKzriGfU7SgLIZyVO96W7S1PxH7JY6no98uERXw6i5Cw6uR3
    kn7moHCm/ADAGfH7iGKkDT+BGnPVHd5XMwBKeyDUJlpxl42a4KrQ0YEsJcEh8RoG
    F23hogiIiizkJiEVujhMzoR8j43y37DgMY8dMPUEcG1cuRXl+YGcRo9PcbwR7Hqt
    nt2Z0RXbLWvpIw9LvbQDVy1LWu/BLgcW24bskFvX05dbuXNYYZlBVQXqn9Mw0TPI
    NvOMoviBAoGBAPNasmX+8lDR/8Ksq1gjzcc67ayirQCltzMLfnK/EWEmR3lQ3F0X
    J401JW3GtNGnSS7YNUJ9NqZVF6AGnXalv34NmDEDrMgnfMTcljKSp7Hq1zg4g1d8
    hIAX71clYzE3ttvQXdmZ+ODFXeb/EdWD5DrAkYklUYJVAA0Vh3XmuqXHAoGBAN7R
    iMUbcmbaEIAXdwafbFaTwCnnL/BcAhqG2tLkmQRA9bK/2FmQYM8eIlvHKRtf8c6i
    C3U61gzKvtt25QiS2ndjMiaz4BScG86CugHNS37jV7YJpr1HfG5k2o6HVqH3Q6N0
    4PBkThMV5/fczpCjbwm7dleFlrO5SoSyDxKgPv2LAoGAbam4bOgnkC4iADtJd42i
    5J30jvA73+fNl64dTF432byjRtMeisCtbR++/yolP7kP7JE35v+AChKcc9unR9lH
    YumDZMsQKJ5KxnfVI3MdKLr4Q3iMD3eoSgT/MRlDCuHdgE70i+Or2LZ4K2lUQxCN
    X8B5lQEBKydcv4itI39XKu8CgYAeVJVyh0buWzatyQPpwd/EdT5ONesfo9ahd8Pn
    uzhdDN6lI1o27uFOsEJrDdQ5prjKsOIwZf7nJN4QD8IjhaH5aGZWQFv7Ujiyxra9
    Mg3ZyuaQoAWzdDyq5k2d4yxWxcrOQEWyU2URUgUOh18S7zxZLcxxI30dH9bV80r7
    9RY9+wKBgQDoQmk6dqFoU5DEqlH7rlita6UGyXxmU7re5twVfndenxr9a7qXJ7Bz
    ybyOpQ5kJ7vsrfcuh5+jm/m/h5yYs4s0SM5EA8PMI1f82jJ5k4tNAVXBZqSlBAxr
    Z7Dpo+oUrSPqgTpie3TC4B+C8fSeJz6Xeb5bytOed4wX+PBnNcsDZA==
    -----END RSA PRIVATE KEY-----
    """;

Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  Harris Tweaks  —  License Key Generator  ");
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine();

try
{
    using var rsa = RSA.Create();
    rsa.ImportFromPem(PrivateKeyPem);

    var payload    = Encoding.UTF8.GetBytes(LicensePayload);
    var signature  = rsa.SignData(payload, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
    var licenseKey = Convert.ToBase64String(signature);

    Console.WriteLine("  ┌─────────────────────────────────────┐");
    Console.WriteLine("  │           LICENSE KEY                │");
    Console.WriteLine("  └─────────────────────────────────────┘");
    Console.WriteLine();
    Console.WriteLine(licenseKey);
    Console.WriteLine();
    Console.WriteLine("  Give the key above to the customer.");
    Console.WriteLine("  They paste it into the Harris Tweaks activation dialog.");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
