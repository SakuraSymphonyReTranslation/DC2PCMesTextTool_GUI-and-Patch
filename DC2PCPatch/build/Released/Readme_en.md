# DC2PC Patch — Installation and Build

[Bahasa Indonesia](README.md) | **English**

This patch and launcher are tailored specifically for **Da Capo 2 Plus Communication** (D.C.II P.C. ～Da Capo II～ Plus Communication). Main source files: `src/dc2pcpatch.cpp` and `src/launcher.cpp`.

## Installation

Download `Released_patch.rar` from [release v1.0.0](https://github.com/SakuraSymphonyReTranslation/DC2PCMesTextTool_GUI-and-Patch/releases/tag/v1.0.0), or use the package under `build/Released`.

`<GAME_DIR>` means the player's chosen installation folder containing `DC2PC.EXE` and `AdvData`. Replace this placeholder with your game's location; no specific drive is mandatory.

1. Close the game and back up files to be replaced.
2. Extract the package contents directly into `<GAME_DIR>`.
3. Run **`<GAME_DIR>\DC2PCLauncher.exe`** with `<GAME_DIR>` as the working directory; shortcuts must set **Start in** to that folder.
4. The launcher automatically injects the patch DLL into the game process. Embedded Locale Emulator support ensures the game runs within a stable Japanese codepage (CP932) environment.

Package contents:
- `DC2PC.EXE` (reference game executable)
- `DC2PCLauncher.exe` (injection launcher)
- `DC2PCPatch.dll` (runtime hook & patch)
- `DC2PCPatch.ini` (font & spacing configuration)
- `LoaderDll.dll` & `LocaleEmulator.dll` (locale libraries)
- `registry\dc2pc registry.reg` (registry setup helper)
- `README.md` and `Readme_en.md` (Indonesian and English guides)

Translated scripts are installed separately into `id_Data\MES` or `eng_data\MES` under `<GAME_DIR>` according to language choice.

The tested reference executable has SHA256:

```text
4bbcc3bc0f5eabb560340e5fc6dd05b87ab71aa992e0ab77e669784107f01c92
```

## Patch Features

- **Choices:** Proportional text tidies the space before `○` and between words. Labels are measured again when translations change, with font size adjusted from 28px down to 16px to fit the button width.
- **Launcher Icon:** `DC2PCLauncher.exe` uses `icon.ico` from the source tree.
- **DVD Check Bypass:** Prevents optical CD/DVD check failures on modern Windows 10/11 and Linux/Wine/Proton/GameHub platforms.
- **Popup Text Hooks:** Translates 28px bitmap string confirmation prompts (Save, Load, Return, Exit).
- **File Redirection:** Reads translated scripts from `id_Data\MES` without altering original game files in `AdvData`.
- **Font & Layout Customization:** Configure dialogue font, backlog font, sizes, and line spacing through `DC2PCPatch.ini`.

The choice renderer is active for translated languages. This choice update leaves button dimensions, popup notifications, textboxes, and INI settings unchanged. If a label is still too long at 16px, the native renderer is used; shorten that choice's wording.

When editing JSON, use the updated importer/exporter from the same release. Choices stay on one line, spacing is normalized, and a deleted `○` marker is restored from the original MES template. See the [tool guide](https://github.com/SakuraSymphonyReTranslation/DC2PCMesTextTool_GUI-and-Patch/blob/v1.0.0/Readme_en.md#editing-choices) for details.

## Compiling / Building

Requires Visual Studio 2022 (MSVC x86 Native Tools) and CMake 3.15+.

From the Visual Studio x86 Native Tools Command Prompt:

```cmd
build.bat
```

Or using the automated agent build script:

```cmd
build_agent_dc2pc.bat
```

Binaries are written to `build/Release` (`DC2PCLauncher.exe` and `DC2PCPatch.dll`). The distribution package is under `build/Released/Released_patch.rar`.

The repository includes the complete `build/` directory: Visual Studio solution, CMake cache, intermediate files, and packages. The cache contains paths from the build machine. When compiling in another location or on another computer, use a new build directory from the `DC2PCPatch` folder:

```cmd
cmake -S . -B build-local -G "Visual Studio 17 2022" -A Win32
cmake --build build-local --config Release
```

These commands write their output to `build-local/Release`. The launcher resource reads `icon.ico` from the source tree, embedding that icon during compilation.
