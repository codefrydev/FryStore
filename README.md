# FryWorld Resources & Plugin Ecosystem Hub

Official open-source repository hosting the **Remote Plugin Marketplace**, **Reference Plugin SDK**, and **Typography Font CDN** for **[FryWorld](https://github.com/codefrydev/FryWorld)** (the modern .NET 10 cross-platform Avalonia desktop OS and studio suite).

[![Plugins Catalog](https://img.shields.io/badge/Marketplace-8%20Plugins-blue.svg)](plugins/catalog.json)
[![Fonts](https://img.shields.io/badge/Fonts-67%20Binaries-green.svg)](fonts/)
[![License](https://img.shields.io/badge/License-MIT%20%2F%20OFL-orange.svg)](LICENSE)

---


## 🧩 1. FryWorld Plugin Marketplace

FryWorld features a dynamic microkernel architecture where extensions mount directly into the application runtime (`shell.overlay`, ribbon tabs, status bar, command palette, and tools studio) without requiring full application restarts.

---

## 📦 3. Building, Packaging & Publishing

### Automated CLI Tooling

Package all plugins into their `.fryplugin` release archives and synchronize metadata:

```bash
# Package all plugins in examples/ and update catalog sizes
python3 tools/package_plugin.py --all

# Or package a single plugin
python3 tools/package_plugin.py SnakePlugin
```

### Validate Ecosystem Integrity

Before committing or pushing, run the comprehensive integrity validator:

```bash
python3 tools/validate_plugins.py
```
This automatically verifies:
- `plugins/catalog.json` syntax, schema, and unique IDs
- Parity between catalog entries and `plugins/<id>/` distribution folders
- `.fryplugin` zip archive contents (`plugin.json` and compiled `.dll` assemblies)
- Solution and standalone runner health across all plugins in `examples/`

---

## 🔤 4. Typography & Font Resources

Official typography and multilingual font library distributed via CDN for **FryPDF**.

- **Total Fonts**: 67 TrueType & OpenType font files
- **Total Library Size**: ~80.7 MB
- **Raw CDN Endpoint**:
  ```
  https://raw.githubusercontent.com/codefrydev/FryStore/refs/heads/main/fonts/{fileName}
  ```

### Usage in FryPDF
Fonts are fetched on demand via `FontPackageService.cs`:
```csharp
public const string FontCdnBaseUrl = "https://raw.githubusercontent.com/codefrydev/FryStore/refs/heads/main/fonts";
```
Downloaded fonts are cached locally in `AppData/FryPDF/FontPackages/` and dynamically registered into QuestPDF and Avalonia graphics pipelines without requiring application restarts.

### Font Verification
Verify all font binaries with:
```bash
python3 tools/verify_fonts.py
```

### Licensing Information
All fonts in this repository are distributed under permissive open-source licenses:
- **SIL Open Font License 1.1** (64 fonts): See [`OFL.txt`](./OFL.txt)
- **Apache License 2.0** (`Roboto.ttf`, `RobotoMono.ttf`): See [`LICENSE-Apache-2.0.txt`](./LICENSE-Apache-2.0.txt)
- **Ubuntu Font Licence 1.0** (`Ubuntu.ttf`): See [`LICENSE-Ubuntu.txt`](./LICENSE-Ubuntu.txt)

---
