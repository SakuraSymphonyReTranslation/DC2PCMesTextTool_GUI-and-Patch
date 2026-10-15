# DC2PC Patch — instalasi dan build

**Bahasa Indonesia** | [English](Readme_en.md)

Patch dan launcher ini dikhususkan untuk **Da Capo 2 Plus Communication** (D.C.II P.C. ～Da Capo II～ Plus Communication). Source utama: `src/dc2pcpatch.cpp` dan `src/launcher.cpp`.

## Instalasi

Unduh `Released_patch.rar` dari [rilis v1.0.0](https://github.com/SakuraSymphonyReTranslation/DC2PCMesTextTool_GUI-and-Patch/releases/tag/v1.0.0), atau gunakan paket di `build/Released`.

`<GAME_DIR>` berarti folder instalasi pilihan pemain yang berisi `DC2PC.EXE` dan `AdvData`. Ganti placeholder ini dengan lokasi game Anda; tidak ada drive yang diwajibkan.

1. Tutup game dan cadangkan berkas yang akan diganti.
2. Ekstrak isi paket langsung ke `<GAME_DIR>`.
3. Jalankan **`<GAME_DIR>\DC2PCLauncher.exe`** dengan working directory `<GAME_DIR>`; shortcut harus memakai folder itu pada **Start in**.
4. Launcher memuat DLL patch dan menginjeksikannya ke proses game secara otomatis. Dukungan Locale Emulator disematkan di dalam launcher untuk memastikan game berjalan dalam environment kode halaman Jepang (CP932) yang stabil.

Paket berisi:
- `DC2PC.EXE` (executable game referensi)
- `DC2PCLauncher.exe` (launcher injeksi)
- `DC2PCPatch.dll` (runtime hook & patch)
- `DC2PCPatch.ini` (konfigurasi font & spacing)
- `LoaderDll.dll` & `LocaleEmulator.dll` (pustaka locale)
- `registry\dc2pc registry.reg` (pengaturan registry pendukung)
- `README.md` dan `Readme_en.md` (petunjuk Indonesia dan Inggris)

Naskah terjemahan dipasang terpisah ke `id_Data\MES` atau `eng_data\MES` di dalam `<GAME_DIR>` sesuai pilihan bahasa.

Executable referensi yang diuji memiliki SHA256:

```text
4bbcc3bc0f5eabb560340e5fc6dd05b87ab71aa992e0ab77e669784107f01c92
```

## Fitur Patch

- **Choice / Pilihan:** Font proporsional merapikan jarak sebelum `○` dan antar kata. Teks diukur ulang saat terjemahan berubah, lalu ukuran font disesuaikan dari 28px hingga 16px mengikuti lebar tombol.
- **Ikon Launcher:** `DC2PCLauncher.exe` memakai `icon.ico` yang disertakan dalam source.
- **Bypass DVD check:** Menghindari galat permintaan CD/DVD optik di Windows 10/11 modern dan platform Linux/Wine/Proton/GameHub.
- **Hook Popup Text:** Menerjemahkan dialog konfirmasi 28px bitmap string (seperti Simpan, Muat, Kembali, Keluar).
- **Pengalihan File (File Redirection):** Membaca naskah terjemahan dari `id_Data\MES` tanpa mengubah file game asli di `AdvData`.
- **Pengaturan Font & Layout:** Melalui `DC2PCPatch.ini`, pemain dapat mengatur font dialog, backlog, ukuran teks, serta line spacing.

Renderer choice aktif pada bahasa terjemahan. Ukuran tombol, popup notification, textbox, dan pengaturan INI tidak diubah oleh pembaruan choice ini. Jika teks masih terlalu panjang pada 16px, renderer bawaan dipakai; pendekkan kalimat pilihan tersebut.

Saat mengedit JSON, gunakan importer/exporter terbaru dari rilis yang sama. Choice tetap satu baris, spasi dirapikan, dan penanda `○` yang terhapus dipulihkan dari template MES asli. Petunjuk lengkap tersedia di [panduan tool](../README.md#mengedit-choice--pilihan).

## Kompilasi / Build

Dibutuhkan Visual Studio 2022 (MSVC x86 Native Tools) dan CMake 3.15+.

Dari Command Prompt Visual Studio x86:

```cmd
build.bat
```

Atau menggunakan skrip build agent:

```cmd
build_agent_dc2pc.bat
```

Output biner berada di `build/Release` (`DC2PCLauncher.exe` dan `DC2PCPatch.dll`). Paket distribusi berada di `build/Released/Released_patch.rar`.

Seluruh isi `build/` disertakan di repositori, termasuk solution Visual Studio, cache CMake, berkas perantara, dan paket. Cache menyimpan lokasi mesin pembuatnya. Untuk kompilasi di lokasi atau komputer lain, gunakan direktori build baru dari folder `DC2PCPatch`:

```cmd
cmake -S . -B build-local -G "Visual Studio 17 2022" -A Win32
cmake --build build-local --config Release
```

Output dari perintah tersebut berada di `build-local/Release`. Resource launcher membaca `icon.ico` dari source, sehingga ikon ikut disematkan saat kompilasi.
