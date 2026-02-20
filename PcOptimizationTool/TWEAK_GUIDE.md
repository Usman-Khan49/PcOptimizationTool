# Quick Reference: Adding Tweaks

## Tweak Configuration Examples

### Registry Tweak
```json
{
  "Id": "unique_identifier",
  "Name": "Display Name",
  "Description": "What this does",
  "Category": 0,  // 0=Privacy, 1=Performance, 2=UI, 3=Network, 4=Security, 5=Services, 6=Other
  "Type": 0,      // 0=Registry
  "Status": 0,
  "IsEnabled": false,
  "RequiresRestart": true,
  "WarningMessage": "Optional warning text",
  "Configuration": {
    "RegistryPath": "HKEY_LOCAL_MACHINE\\SOFTWARE\\Path",
    "ValueName": "ValueName",
    "ApplyValue": 0,
    "UndoValue": 1,
    "ValueType": "REG_DWORD"  // REG_DWORD, REG_SZ, REG_BINARY, etc.
  }
}
```

### PowerShell Tweak
```json
{
  "Id": "unique_identifier",
  "Name": "Display Name",
  "Description": "What this does",
  "Category": 1,
  "Type": 5,      // 5=PowerShell
  "Status": 0,
  "IsEnabled": false,
  "RequiresRestart": false,
  "WarningMessage": null,
  "Configuration": {
    "PowerShellCommand": "Set-Service -Name 'ServiceName' -StartupType Disabled",
    "PowerShellUndoCommand": "Set-Service -Name 'ServiceName' -StartupType Automatic"
  }
}
```

### Service Tweak
```json
{
  "Id": "unique_identifier",
  "Name": "Display Name",
  "Description": "What this does",
  "Category": 5,
  "Type": 2,      // 2=Service
  "Status": 0,
  "IsEnabled": false,
  "RequiresRestart": false,
  "WarningMessage": null,
  "Configuration": {
    "ServiceName": "DiagTrack",
    "ServiceState": "Disabled"  // Disabled, Manual, Automatic
  }
}
```

## Enum Reference

### TweakCategory
```
0 = Privacy
1 = Performance
2 = UI
3 = Network
4 = Security
5 = Services
6 = Other
```

### TweakType
```
0 = Registry
1 = WindowsSetting
2 = Service
3 = ScheduledTask
4 = FileSystem
5 = PowerShell
```

### TweakStatus
```
0 = NotApplied
1 = Applied
2 = PartiallyApplied
3 = Failed
4 = Unknown
```

## Common Registry Paths

### Privacy
- `HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection`
- `HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Privacy`
- `HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\AdvertisingInfo`

### Performance
- `HKEY_CURRENT_USER\\Control Panel\\Desktop`
- `HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VisualEffects`
- `HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management`

### UI/Appearance
- `HKEY_CURRENT_USER\\Control Panel\\Desktop\\WindowMetrics`
- `HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize`
- `HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\DWM`

### Services/Startup
- `HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run`
- `HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run`

## Registry Value Types

- `REG_SZ`: String value
- `REG_DWORD`: 32-bit number
- `REG_QWORD`: 64-bit number
- `REG_BINARY`: Binary data
- `REG_MULTI_SZ`: Multiple strings
- `REG_EXPAND_SZ`: Expandable string (with environment variables)
