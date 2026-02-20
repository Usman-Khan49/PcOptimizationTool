# PC Optimization Tool - Architecture

## Project Structure

```
PcOptimizationTool/
?
??? Data/
?   ??? tweaks.json              # Tweak definitions
?   ??? preferences.json         # User preferences (auto-generated)
?
??? Enums/
?   ??? TweakCategory.cs         # Tweak categories (Privacy, Performance, etc.)
?   ??? TweakType.cs             # Tweak types (Registry, Service, etc.)
?   ??? TweakStatus.cs           # Status tracking (Applied, NotApplied, etc.)
?
??? Models/
?   ??? Tweak.cs                 # Core tweak model
?   ??? TweakConfiguration.cs    # Configuration for different tweak types
?   ??? TweakResult.cs           # Result of applying/undoing tweaks
?
??? Interfaces/
?   ??? ITweakService.cs         # Service for applying tweaks
?   ??? IRegistryService.cs      # Registry operations abstraction
?   ??? ITweakRepository.cs      # Data persistence abstraction
?
??? Services/
?   ??? TweakService.cs          # Main business logic for tweaks
?   ??? RegistryService.cs       # Windows Registry operations
?   ??? TweakRepository.cs       # Load/save tweaks and preferences
?
??? ViewModels/
?   ??? BaseViewModel.cs         # Base MVVM ViewModel
?   ??? MainViewModel.cs         # Main window ViewModel
?
??? Helpers/
?   ??? RelayCommand.cs          # ICommand implementation
?   ??? ServiceLocator.cs        # Simple dependency injection
?
??? Views/
    ??? MainWindow.xaml          # Main application window
    ??? MainWindow.xaml.cs       # Code-behind
```

## Design Patterns

### MVVM (Model-View-ViewModel)
- **Models**: Plain data structures representing tweaks and configurations
- **Views**: XAML UI components
- **ViewModels**: Business logic and UI state management

### Repository Pattern
- `ITweakRepository`: Abstracts data storage (JSON files, future: database)

### Service Layer
- `ITweakService`: Orchestrates tweak application
- `IRegistryService`: Handles registry operations
- Services can be extended for other tweak types (PowerShell, Services, etc.)

### Dependency Injection
- Simple `ServiceLocator` for now
- Can be replaced with proper DI container (Microsoft.Extensions.DependencyInjection)

## Key Concepts

### Tweak Types
- **Registry**: Modify Windows Registry values
- **WindowsSetting**: Windows settings via APIs
- **Service**: Control Windows services
- **ScheduledTask**: Manage scheduled tasks
- **FileSystem**: File/folder operations
- **PowerShell**: Execute PowerShell commands

### Tweak Configuration
Each tweak has a flexible configuration object that stores:
- Registry paths and values
- Service names and states
- PowerShell commands
- Undo information

### Status Tracking
- **NotApplied**: Tweak is not currently active
- **Applied**: Tweak is active
- **PartiallyApplied**: Some parts applied
- **Failed**: Application failed
- **Unknown**: Cannot determine status

## Next Steps

1. **Implement Apply/Undo Logic** in `TweakService.cs`
2. **Create UI** for browsing and selecting tweaks
3. **Add Status Detection** to check current tweak states
4. **Implement Backup/Restore** functionality
5. **Add Logging** for debugging
6. **Create Preset Profiles** (Gaming, Privacy-Focused, etc.)
7. **Add Admin Elevation** detection and handling

## Usage Example

```csharp
// In ViewModel or code-behind
var tweakService = ServiceLocator.Instance.GetService<ITweakService>();

// Load tweaks
var tweaks = await tweakService.LoadTweaksAsync();

// Apply a tweak
var result = await tweakService.ApplyTweakAsync(tweak);

// Undo a tweak
var undoResult = await tweakService.UndoTweakAsync(tweak);
```

## Adding New Tweaks

Add entries to `Data/tweaks.json`:

```json
{
  "Id": "unique_id",
  "Name": "Display Name",
  "Description": "What this tweak does",
  "Category": 0,  // TweakCategory enum value
  "Type": 0,      // TweakType enum value
  "Status": 0,
  "IsEnabled": false,
  "RequiresRestart": false,
  "WarningMessage": "Optional warning",
  "Configuration": {
    "RegistryPath": "HKEY_LOCAL_MACHINE\\...",
    "ValueName": "ValueName",
    "ApplyValue": 0,
    "UndoValue": 1,
    "ValueType": "REG_DWORD"
  }
}
```
