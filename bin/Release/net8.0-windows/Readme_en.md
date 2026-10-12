# DC2PC MES Text Tool — GUI and CLI

[Bahasa Indonesia](README.md) | **English**

An importer/exporter for **D.C.II P.C. ~Da Capo II~ Plus Communication**.

This distribution supports **Da Capo 2 Plus Communication** only (`dc2pc`, MES version `0x9969`). The GUI, CLI, reference tables, patch, and launcher are tailored specifically for this game. The main patch source is located at `DC2PCPatch/src/dc2pcpatch.cpp`.

## Downloads and game folder

Download both packages from [GitHub Releases](https://github.com/SakuraSymphonyReTranslation/DC2PCMesTextTool_GUI-and-Patch/releases/latest):

- **DC2PCMesTextTool_GUI.rar**: MES ↔ JSON importer/exporter with a GUI and CLI. Includes Indonesian and English documentation.
- **Released_patch.rar**: launcher, patch DLL, INI configuration, and supporting runtime files.

`<GAME_DIR>` means **your game's installation folder**, which contains `DC2PC.EXE` and `AdvData`. It is a documentation placeholder: replace it with your actual game folder, without the `<` and `>` characters. No particular drive or installation path is required.

1. Close the game and back up any files that will be replaced, especially the executable and `DC2PCPatch.ini`.
2. Extract the contents of `Released_patch.rar` directly into `<GAME_DIR>`, so that `DC2PCLauncher.exe`, `DC2PCPatch.dll`, and `DC2PCPatch.ini` are next to `DC2PC.EXE`.
3. Run **`<GAME_DIR>\DC2PCLauncher.exe`** with `<GAME_DIR>` as the working directory. If you use a shortcut, set its **Start in** field to that folder.
4. Select the appropriate language in the patch settings (`Language=1` for Indonesian). Place Indonesian MES files in `<GAME_DIR>\id_Data\MES`; for English, use `<GAME_DIR>\eng_data\MES`.

```text
<GAME_DIR>/
├── DC2PC.EXE
├── DC2PCLauncher.exe
├── DC2PCPatch.dll
├── DC2PCPatch.ini
├── LoaderDll.dll
├── LocaleEmulator.dll
├── AdvData/
└── id_Data/
    └── MES/
```

The two archives provide the tool and patch runtime; they do not include the complete translated scripts. The repository's source folder can be placed anywhere. The patch package is also available under `DC2PCPatch/build/Released` in the source tree.

The tested reference executable has SHA256:

```text
4bbcc3bc0f5eabb560340e5fc6dd05b87ab71aa992e0ab77e669784107f01c92
```

## Opening the application

Extract `DC2PCMesTextTool_GUI.rar` to a folder of your choice and run `DC2PCMesTextTool_GUI.exe`. If you are using the source repository, run `bin\Release\net8.0-windows\DC2PCMesTextTool_GUI.exe` or the standalone executable in `bin\publish\DC2PCMesTextTool_GUI.exe`.
The ready-to-use package is located at: `bin\Release\net8.0-windows\DC2PCMesTextTool_GUI.rar`.
The application requires the .NET 8 Desktop Runtime.

## Single File

**MES → JSON (export)**

1. Set **Input MES/JSON** to the original MES file.
2. Set **Output**, or leave it empty to create the JSON file next to the input.
3. Click **Export (MES -> JSON)**.

The default output combines character names and dialogue according to the DC2PC structure:

```json
[
  { "message": "Narration." },
  { "name": "Otome", "message": "「Character dialogue.」" }
]
```

Both the Single File and Batch tabs export the combined format shown above. Text, Japanese/Indonesian names, and line breaks are preserved during export.

**JSON → MES (import)**

1. Set **Input MES/JSON** to the translated JSON file.
2. The original MES file with the same base name is automatically located next to the JSON file or in the specified MES folder.
3. Choose an **Output** file different from the original MES file.
4. Set **Word Wrap**: default is `70` in Single File, Batch, and the CLI; `0` preserves text and line breaks without reflowing them.
5. Click **Import (JSON -> MES)**.

The original template is required to preserve non-text bytecodes and label flow.

## Batch

- **Batch Export:** specify the source folder in **MES Folder** and set the output directory, then click **Batch Export (MES -> JSON)**.
- **Batch Import:** specify **JSON Folder**, the original **MES Folder**, and a separate **Output Folder**, then click **Batch Import (JSON -> MES)**.
- File base names must match: for example, `scene.json` uses `scene.mes` as its template.
- Files are processed sequentially, and progress/errors are logged in the bottom log box.

## JSON Cleaning Utility (penghilang/penghilang n.py)

The repository includes a Python text cleaning utility under `penghilang/`:
- Strips ruby text `{reading/kanji}` → `kanji`.
- Removes leading full-width spaces (`\u3000`) at the beginning of lines while **preserving** full-width spaces in the middle of sentences (critical for DC2PC's 2-byte Shift-JIS engine word-separation).

## Layout and Runtime Patch

- **`DC2PCPatch.dll`** is automatically injected by `DC2PCLauncher.exe`.
- **DVD check bypass:** The AdvSys32 engine scans for optical CD/DVD drives upon startup. The patch includes drive spoofing to allow seamless execution on optical-drive-less systems (including Linux/Wine/Proton and GameHub handhelds).
- **Popup Text Rendering:** Hooks ADV32's 28px bitmap string renderer for confirmation popups (Save, Load, Return, Exit) and displays translated text.
- **Font & Spacing:** Adjust font sizes, backlog layout, and spacing via `DC2PCPatch.ini`.
- **File Redirection:** Seamlessly redirects MES script loading to `id_Data\MES` (or `eng_data\MES`) without overwriting files inside `AdvData`.

For technical patch details, see the [patch README](DC2PCPatch/README.md).

## Validation and Encoding

- DC2PC scripts are encoded in **Shift-JIS (CP932)**.
- Encrypted bytecode strings are decrypted/re-encrypted using XOR key `0x20`.
- MES labels are recalculated based on the label table header (`labelCount * 4 + 4`).
- Unsupported Shift-JIS characters are validated to avoid crashes or text corruption in-game.

## CLI

From the executable's folder:

```powershell
.\DC2PCMesTextTool_GUI.exe export "scene.mes" "scene.json"
.\DC2PCMesTextTool_GUI.exe import "scene.json" "original\scene.mes" "new\scene.mes" -w 70
.\DC2PCMesTextTool_GUI.exe batch-export "original_mes_folder" "json_output"
.\DC2PCMesTextTool_GUI.exe batch-import "json_folder" "original_mes_folder" "output_mes_folder" -w 70
```

`-w` specifies the word wrap limit (default: 70, `0` to disable wrap). Drag-and-dropping `.mes` files directly onto the executable is also supported for quick exports.

## Building

From the project's source folder:

```powershell
dotnet build -c Release
```

Building the GUI requires the .NET 8 SDK. To build the C++ patch DLL and launcher, see the instructions in the [patch README](DC2PCPatch/README.md).
