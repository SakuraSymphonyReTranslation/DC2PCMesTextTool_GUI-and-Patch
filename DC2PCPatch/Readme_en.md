# DC2PC Patch — Installation and Build

[Bahasa Indonesia](README.md) | **English**

This patch and launcher are tailored specifically for **Da Capo 2 Plus Communication** (D.C.II P.C. ～Da Capo II～ Plus Communication). Main source files: `src/dc2pcpatch.cpp` and `src/launcher.cpp`.

## Installation

Download `Released_patch.rar` from [Releases](https://github.com/SakuraSymphonyReTranslation/DC2PCMesTextTool_GUI-and-Patch/releases/latest), or use the package under `build/Released`.

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

Translated scripts are installed separately into `id_Data\MES` or `eng_data\MES` under `<GAME_DIR>` according to language choice.

The tested reference executable has SHA256:

```text
4bbcc3bc0f5eabb560340e5fc6dd05b87ab71aa992e0ab77e669784107f01c92
```

## Patch Features

- **DVD Check Bypass:** Prevents optical CD/DVD check failures on modern Windows 10/11 and Linux/Wine/Proton/GameHub platforms.
- **Popup Text Hooks:** Translates 28px bitmap string confirmation prompts (Save, Load, Return, Exit).
- **File Redirection:** Reads translated scripts from `id_Data\MES` without altering original game files in `AdvData`.
- **Font & Layout Customization:** Configure dialogue font, backlog font, sizes, and line spacing through `DC2PCPatch.ini`.

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

Binaries will be output to the `build/` directory (`DC2PCLauncher.exe` and `DC2PCPatch.dll`).
