import os

base_dir = os.path.dirname(os.path.abspath(__file__))
tr_mes = os.path.join(base_dir, "MES", "0ind_1216_a1_skr.mes")

def hex_dump(data):
    return " ".join(f"{b:02X}" for b in data)

if os.path.exists(tr_mes):
    with open(tr_mes, 'rb') as f:
        raw = f.read()
        
        idx = raw.find(b'Sangat')
        if idx != -1:
            print("Found plaintext Sangat!")
            print(hex_dump(raw[idx:idx+16]))
        
        cipher = bytes([c - 0x20 for c in b'Sangat'])
        idx = raw.find(cipher)
        if idx != -1:
            print("Found encrypted Sangat (DC2PC 0x20 KEY) at offset", idx)
            print(hex_dump(raw[idx-4:idx+20]))
            print("Opcode:", hex(raw[idx-1]))
