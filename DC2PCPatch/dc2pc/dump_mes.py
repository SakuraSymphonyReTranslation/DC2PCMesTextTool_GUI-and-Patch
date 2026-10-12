import os

base_dir = os.path.dirname(os.path.abspath(__file__))
tr_mes = os.path.join(base_dir, "MES", "0ind_1216_a1_skr.mes")
jp_mes = os.path.join(base_dir, "MES - JP", "0ind_1216_a1_skr.mes")

def hex_dump(data):
    return " ".join(f"{b:02X}" for b in data)

if os.path.exists(jp_mes):
    with open(jp_mes, 'rb') as f:
        jp_raw = f.read()
        print("JP MES Header (64 bytes):")
        print(hex_dump(jp_raw[:64]))
    
if os.path.exists(tr_mes):
    with open(tr_mes, 'rb') as f:
        tr_raw = f.read()
        print("\nTR MES Header (64 bytes):")
        print(hex_dump(tr_raw[:64]))

        idx = tr_raw.find(b'\x53\x00\x00\x00\xE4\x81\x00\x00')
        if idx != -1:
            print(f"\nFound 'Sangat' at offset {idx}")
            print("Surrounding 32 bytes:")
            print(hex_dump(tr_raw[idx-16:idx+16]))
