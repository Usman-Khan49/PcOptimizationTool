<div align="center">

# 🎮 HARRIS TWEAKS

### ⚡ MAX FPS. ZERO BLOAT. NO MERCY. ⚡

[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Desktop-blue?style=for-the-badge)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-Private-red?style=for-the-badge)]()
[![Platform](https://img.shields.io/badge/Platform-Windows%20x64-cyan?style=for-the-badge&logo=windows)](https://www.microsoft.com/windows)

<br/>

> **The ultimate PC optimization toolkit for gamers who refuse to lose.**
> Kill bloatware. Crush input delay. Dominate the lobby.

<br/>

![Main Tweaks Window](docs/screenshot.png)

<br/>

</div>

---

## 🕹️ WHAT IS THIS?

**Harris Tweaks** is a retro-styled Windows optimization tool built for competitive gamers. It strips your PC down to the bare metal — disabling telemetry, killing background junk, tweaking network stacks, and squeezing every last frame out of your hardware.

No cloud. No subscriptions. No BS. Just raw performance.

---

## ✨ FEATURES

### 🟦 ESSENTIAL TWEAKS
> The basics. Every gamer needs these.

- 🗑️ Delete temporary files & run disk cleanup
- 🔇 Disable telemetry, activity history & location tracking
- 🚫 Disable Consumer Features & PowerShell 7 telemetry
- ⚙️ Set services to manual for leaner boot
- 🖱️ Enable "End Task" with right-click
- 💾 Disable Windows Platform Binary Table (WPBT)

### 🟪 BIG BOY TWEAKS
> Go deeper. Rip out what you don't need.

- 🌐 Brave debloat & Edge debloat
- 📵 Disable background apps & notification tray/calendar
- 🎮 Disable fullscreen optimizations
- 🔒 Block Adobe network, Razer bloatware & Microsoft Copilot
- 🌍 Disable IPv6 & Teredo tunneling

### 🔥 SECRET SAUCE (PRO)
> The tweaks that separate the sweats from the casuals.

- ⚡ Win32 Priority Separation tuning
- 🧠 Additional system optimizations
- 📡 Decrease network delay & disable network throttling
- 🔋 Disable GPU energy driver & power throttling
- ⌨️ Fix keyboard input delay
- 🎯 Fortnite GPU priority (High Priority mode)

---

## 🖥️ SCREENSHOT

<div align="center">
<img src="docs/screenshot.png" alt="Harris Tweaks Main Window" width="800"/>
</div>

---

## 🏗️ TECH STACK

| Component | Technology |
|---|---|
| **Framework** | .NET 10 / WPF |
| **UI Theme** | ModernWPF + Custom retro pixel art |
| **Fonts** | Press Start 2P, Silkscreen, VT323 |
| **Licensing** | RSA-2048 PSS offline activation |
| **Storage** | AES-256 encrypted license file |
| **Architecture** | MVVM with service locator DI |

---

## 🚀 GETTING STARTED

### Prerequisites
- Windows 10/11 (x64)
- No .NET installation needed (self-contained build)

### Run the App
1. Download the latest release from [Releases](https://github.com/Usman-Khan49/PcOptimizationTool/releases)
2. Extract the zip
3. Run `PcOptimizationTool.exe` **as Administrator** (required for system tweaks)

### Build from Source
```bash
git clone https://github.com/Usman-Khan49/PcOptimizationTool.git
cd PcOptimizationTool
dotnet publish PcOptimizationTool/PcOptimizationTool.csproj -c Release -r win-x64
```

Output: `PcOptimizationTool/bin/Release/net10.0-windows/win-x64/publish/`

---

## 🔐 ACTIVATION

The **Secret Sauce** tweaks require a Pro license key.

1. Click **PREMIUM 🔒** or the **ENTER KEY** button
2. Paste your activation key
3. Hit **ACTIVATE**

Keys are verified offline using RSA-2048 signatures — no internet required, ever.

> To get a key, visit our Discord: **harris.tweaks/discord**

---

## 🛡️ SAFETY

- **Restore Points** — Harris Tweaks prompts you to create a system restore point before your first apply. Roll back anytime.
- **Undo Button** — Every tweak can be individually undone with one click.
- **No Permanent Changes** — All tweaks modify reversible registry keys and service states.

---

## 📁 PROJECT STRUCTURE

```
PcOptimizationTool/
├── Assets/              # Icons, logos, coin images
├── Data/                # tweaks.json (tweak definitions)
├── Enums/               # TweakType, TweakCategory, LicenseStatus
├── Fonts/               # Press Start 2P, Silkscreen, VT323
├── Helpers/             # RelayCommand, ServiceLocator, converters
├── Interfaces/          # Service contracts
├── Models/              # Tweak, TweakOption, LicenseInfo, etc.
├── Resources/           # Embedded public key for license verification
├── Services/            # TweakService, RegistryService, LicenseService
├── ViewModels/          # MainViewModel, TweakViewModel (MVVM)
├── Views/               # Activation & Restore Point dialogs
├── MainWindow.xaml      # Main UI
└── App.xaml             # App startup & DI registration

HarrisTweaks.KeyGenerator/
└── Program.cs           # Owner-only license key generation tool
```

---

## ⚠️ DISCLAIMER

> This tool modifies Windows registry keys and system services. **Always create a restore point** before applying tweaks. Use at your own risk. The developers are not responsible for any system instability caused by applied tweaks.

---

<div align="center">

**Built with 💜 by [Usman Khan](https://github.com/Usman-Khan49)**

*Press Start to dominate.*

</div>
