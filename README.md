# DC2PC MES Text Tool — GUI dan CLI

**Bahasa Indonesia** | [English](Readme_en.md)

Importer/exporter untuk **D.C.II P.C. ～Da Capo II～ Plus Communication**.

Distribusi ini hanya mendukung **Da Capo 2 Plus Communication** (`dc2pc`, versi MES `0x9969`). GUI, CLI, tabel referensi, patch, dan launcher dikhususkan untuk game ini. Source utama patch: `DC2PCPatch/src/dc2pcpatch.cpp`.

## Unduhan dan folder instalasi

Unduh kedua paket terbaru dari [rilis v1.0.0](https://github.com/SakuraSymphonyReTranslation/DC2PCMesTextTool_GUI-and-Patch/releases/tag/v1.0.0):

- **DC2PCMesTextTool_GUI.rar**: importer/exporter MES ↔ JSON, GUI dan CLI. Termasuk dokumentasi bahasa Indonesia dan Inggris.
- **Released_patch.rar**: launcher, DLL patch, INI, dan berkas pendukung runtime.

`<GAME_DIR>` berarti **folder utama instalasi game pilihan pemain**, yaitu folder yang berisi `DC2PC.EXE` dan `AdvData`. Ini placeholder dokumentasi: ganti dengan lokasi game Anda, tanpa tanda `<` dan `>`. Tidak ada drive atau lokasi instalasi wajib.

1. Tutup game dan simpan salinan berkas lama yang akan diganti, terutama executable dan `DC2PCPatch.ini`.
2. Ekstrak isi `Released_patch.rar` langsung ke `<GAME_DIR>`, sehingga `DC2PCLauncher.exe`, `DC2PCPatch.dll`, dan `DC2PCPatch.ini` berada di sebelah `DC2PC.EXE`.
3. Jalankan **`<GAME_DIR>\DC2PCLauncher.exe`** dengan working directory `<GAME_DIR>`. Jika memakai shortcut, isi **Start in** dengan folder tersebut.
4. Pilih bahasa yang sesuai pada pengaturan patch (`Language=1` untuk Indonesia). Berkas MES Indonesia ditempatkan di `<GAME_DIR>\id_Data\MES`; untuk bahasa Inggris, gunakan `<GAME_DIR>\eng_data\MES`.

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

Kedua RAR menyediakan tool dan runtime patch; keduanya tidak berisi seluruh naskah terjemahan. Folder source repositori boleh ditempatkan di mana saja. Paket patch juga tersedia di `DC2PCPatch/build/Released` pada source.

Executable referensi yang diuji memiliki SHA256:

```text
4bbcc3bc0f5eabb560340e5fc6dd05b87ab71aa992e0ab77e669784107f01c92
```

## Buka aplikasi

Ekstrak `DC2PCMesTextTool_GUI.rar` ke folder pilihan Anda dan jalankan `DC2PCMesTextTool_GUI.exe`. Jika memakai source repositori, jalankan `bin\Release\net8.0-windows\DC2PCMesTextTool_GUI.exe` atau versi standalone di `bin\publish\DC2PCMesTextTool_GUI.exe`.
Paket siap pakai berada di folder: `bin\Release\net8.0-windows\DC2PCMesTextTool_GUI.rar`.
Aplikasi membutuhkan .NET 8 Desktop Runtime.

## Single File

**MES → JSON (exporter)**

1. Isi **Input MES/JSON** dengan MES asli.
2. Isi **Output**, atau kosongkan untuk membuat JSON di sebelah input.
3. Klik **Export (MES -> JSON)**.

Hasil default mengekspor nama karakter dan dialog berdasarkan format DC2PC:

```json
[
  { "message": "Narasi." },
  { "name": "Otome", "message": "「Dialog karakter.」" }
]
```

Ekspor dari tab Single File dan Batch menggunakan format di atas. Teks, nama Jepang/Indonesia, dan pemisah baris dipertahankan saat ekspor.

**JSON → MES (importer)**

1. Isi **Input MES/JSON** dengan JSON terjemahan.
2. File MES asli dengan nama dasar yang sama dicari otomatis di sebelah JSON atau folder MES terkait.
3. Tentukan file **Output** yang berbeda dari MES asli.
4. Pilih **Word Wrap** untuk dialog: default `70` pada Single File, Batch, dan CLI. Choice tetap satu baris.
5. Klik **Import (JSON -> MES)**.

Template asli dibutuhkan untuk menjaga bytecode dan label flow non-teks.

### Mengedit choice / pilihan

Choice diekspor dalam bentuk `{"message":"○ Teks pilihan"}`. Edit isi `message`
tanpa mengubah urutan entri JSON, lalu impor kembali menggunakan template MES
dengan nama yang sama. Aturan ini berlaku pada Single File, Batch, dan CLI.

- Spasi awal/akhir, spasi ganda, tab, dan pemisah baris choice dirapikan menjadi
  satu spasi. Huruf Latin fullwidth dikembalikan ke bentuk biasa.
- Selalu ada satu spasi setelah `○`. Jika penanda itu terhapus saat mengedit,
  importer memulihkannya ketika entri template MES asli merupakan choice.
- Choice tidak terkena pengaturan Word Wrap dialog dan tetap satu baris;
  isinya tidak dipotong oleh importer biasa. Choice kosong ditolak dengan pesan
  kesalahan agar pilihan tidak menjadi tombol tanpa teks.
- Ekspor ulang mempertahankan teks pilihan, termasuk tanda `~`. Choice tidak
  digabung sebagai nama karakter untuk dialog berikutnya.

Gunakan `DC2PCPatch.dll` dengan renderer choice terbaru di folder utama game.
DLL tersebut mengukur ulang teks yang sudah diedit dan menyesuaikan font dengan
lebar tombol (28–16px). Spasi di dalam MES tetap memakai encoding yang aman; JSON memakai
spasi biasa. Pembaruan tool ini tidak mengubah ukuran atau pengaturan popup
notification dan textbox.

## Batch

- **Batch Export:** isi **MES Folder** di kolom `MES Folder` dan tentukan output, lalu klik **Batch Export (MES -> JSON)**.
- **Batch Import:** isi **JSON Folder**, **MES Folder** asli, dan **Output Folder** yang berbeda, lalu klik **Batch Import (JSON -> MES)**.
- Nama file harus sama: misalnya `scene.json` memakai template `scene.mes`.
- File diproses satu per satu dan kemajuan serta log ditampilkan di panel log bawah.

## Pembersih JSON (penghilang/penghilang n.py)

Repositori menyertakan skrip Python pembersih di folder `penghilang/`:
- Menghapus ruby text `{reading/kanji}` → `kanji`.
- Menghapus leading fullwidth space (`\u3000`) di awal baris namun **mempertahankan** fullwidth space di tengah kalimat karena engine DC2PC membaca 2 byte per karakter (Shift-JIS) sebagai pemisah kata.

## Layout dan patch runtime

- **`DC2PCPatch.dll`** disuntikkan secara otomatis oleh `DC2PCLauncher.exe`.
- **Choice / Pilihan:** Merapikan jarak sebelum `○` dan antar kata dengan font proporsional. Ukuran font menyesuaikan teks terjemahan dan lebar tombol; ukuran tombol tetap sama.
- **Ikon Launcher:** `DC2PCLauncher.exe` memakai ikon dari `DC2PCPatch/icon.ico`.
- **Bypass DVD check:** Engine AdvSys32 DC2PC melakukan pengecekan drive CD/DVD optik saat startup. Patch menyertakan spoofing drive sehingga game dapat berjalan mulus di perangkat modern tanpa drive optik (termasuk Linux/Wine/Proton/GameHub).
- **Popup Text Rendering:** Render bitmap 28px pada pesan konfirmasi (seperti Save, Load, Exit) di-*hook* dan dilokalisasi ke bahasa Indonesia (misal: "muat permainan?", "simpan di sini?", "ingin keluar?").
- **Font & Spacing:** Pengaturan ukuran font dan posisi teks backlog dikonfigurasi melalui `DC2PCPatch.ini`.
- **File Redirection:** Arahkan pembacaan file skrip MES ke folder `id_Data\MES` (atau `eng_data\MES`) tanpa perlu menimpa berkas asli di `AdvData`.

Petunjuk teknis patch tersedia di [README patch](DC2PCPatch/README.md).

## Validasi dan encoding

- Engine DC2PC menggunakan encoding **Shift-JIS (CP932)**.
- String terenkripsi dalam bytecode didekripsi dengan menambahkan `0x20` dan dienkripsi dengan mengurangi `0x20` pada setiap byte (modulo 256).
- Label MES dihitung ulang berdasarkan struktur header tabel label (`labelCount * 4 + 4`).
- Choice kosong atau berisi karakter kontrol yang tidak didukung ditolak saat impor. Gunakan karakter yang tersedia dalam CP932 untuk terjemahan.

## CLI

Dari folder executable:

```powershell
.\DC2PCMesTextTool_GUI.exe export "scene.mes" "scene.json"
.\DC2PCMesTextTool_GUI.exe import "scene.json" "original\scene.mes" "new\scene.mes" -w 70
.\DC2PCMesTextTool_GUI.exe batch-export "original_mes_folder" "json_output"
.\DC2PCMesTextTool_GUI.exe batch-import "json_folder" "original_mes_folder" "output_mes_folder" -w 70
```

`-w` mengatur batas word wrap dialog (default: 70); choice tidak mengikuti batas ini. Drag-and-drop file `.mes` langsung ke executable juga didukung untuk ekspor cepat.

## Build

Dari folder source proyek:

```powershell
dotnet build -c Release
```

Build GUI membutuhkan .NET 8 SDK. Untuk build patch DLL dan launcher C++, lihat petunjuk di [README patch](DC2PCPatch/README.md).

Pengujian regresi MES ↔ JSON tersedia di [tests/ChoiceRoundTrip](tests/ChoiceRoundTrip/README.md). Verifikasi pada 1.456 file mencakup 255 choice dalam 103 file, round-trip, alur opcode/label, dan perbandingan byte non-choice dengan versi sebelumnya.
