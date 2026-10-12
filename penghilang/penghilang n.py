import os
import json
import shutil
import re
from datetime import datetime

# regex ruby: {reading／KANJI} atau {reading/KANJI} → KANJI
RUBY_PATTERN = re.compile(r'[｛{][^／/]+[／/]([^｝}]+)[｝}]')

# Hapus 「」quotes Jepang (opsional, uncomment jika perlu)
# JP_QUOTE_PATTERN = re.compile(r'[「」『』]')

def clean_for_dc2pc(text):
    """Bersihkan teks untuk Da Capo 2 Plus Communication.
    
    PENTING: Jangan hapus \u3000 (full-width space)!
    DC2PC engine membaca 2 byte per karakter, dan \u3000 digunakan
    sebagai pemisah kata yang ter-encode sebagai 2 byte di Shift-JIS.
    """
    # 1. Hapus ruby text, pertahankan kanji/terjemahan
    text = RUBY_PATTERN.sub(r'\1', text)
    
    # 2. Hapus \r (carriage return) tapi pertahankan \n (line break)
    #    karena \n bisa dipakai untuk line break di text box
    text = text.replace('\r', '')
    
    # 3. Hapus leading/trailing whitespace per baris
    lines = text.split('\n')
    cleaned_lines = []
    for line in lines:
        # Hapus leading \u3000 saja (sering ada di awal kalimat Jepang)
        line = line.lstrip('\u3000')
        # Hapus spasi biasa di awal/akhir
        line = line.strip()
        if line:  # Abaikan baris kosong
            cleaned_lines.append(line)
    
    text = '\n'.join(cleaned_lines)
    
    return text

def clean_json_properly():
    current_dir = os.path.dirname(os.path.abspath(__file__))
    timestamp = datetime.now().strftime('%H%M%S')
    backup_folder = os.path.join(current_dir, f"backup_json_{timestamp}")

    valid_extensions = ('.json',)

    os.makedirs(backup_folder, exist_ok=True)
    
    total_cleaned = 0
    total_errors = 0

    for root, _, files in os.walk(current_dir):
        # Skip backup folders
        if 'backup_json' in root:
            continue
            
        for filename in files:
            if not filename.lower().endswith(valid_extensions):
                continue
            if filename == os.path.basename(__file__):
                continue

            file_path = os.path.join(root, filename)
            rel_path = os.path.relpath(file_path, current_dir)
            backup_path = os.path.join(backup_folder, rel_path)

            os.makedirs(os.path.dirname(backup_path), exist_ok=True)

            try:
                # 1. Backup
                shutil.copy2(file_path, backup_path)

                # 2. Baca JSON
                with open(file_path, 'r', encoding='utf-8-sig') as f:
                    data = json.load(f)

                # 3. Bersihkan - DC2PC format: list of {name?, message}
                modified = False
                if isinstance(data, list):
                    for item in data:
                        if isinstance(item, dict):
                            for key in ('message', 'name'):
                                if key in item and isinstance(item[key], str):
                                    original = item[key]
                                    cleaned = clean_for_dc2pc(original)
                                    if cleaned != original:
                                        item[key] = cleaned
                                        modified = True

                # 4. Tulis ulang JSON rapi
                if modified:
                    with open(file_path, 'w', encoding='utf-8') as f:
                        json.dump(data, f, ensure_ascii=False, indent=2)
                    total_cleaned += 1
                    print(f"[OK] Dibersihkan: {rel_path}")
                else:
                    print(f"[--] Sudah bersih: {rel_path}")

            except Exception as e:
                total_errors += 1
                print(f"[ERROR] {rel_path}: {str(e)}")

    print(f"\nTotal: {total_cleaned} file dibersihkan, {total_errors} error")

if __name__ == "__main__":
    print("=== Pembersih JSON untuk Da Capo 2 Plus Communication ===")
    print("- Menghapus ruby text {reading/kanji} → kanji")
    print("- Menghapus leading fullwidth space (　) di awal baris")
    print("- MEMPERTAHANKAN fullwidth space di tengah kalimat")
    print("=" * 55)
    print()
    clean_json_properly()
    print("\n" + "=" * 55)
    input("Selesai! Tekan ENTER untuk keluar...")

