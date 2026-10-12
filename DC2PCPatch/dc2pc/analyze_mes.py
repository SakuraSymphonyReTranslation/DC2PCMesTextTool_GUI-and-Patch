import os, json, glob

base_dir = os.path.dirname(os.path.abspath(__file__))
tr_dir = os.path.join(base_dir, "dc2pcjson_test_Translated")
mes_dir = os.path.join(base_dir, "MES")
jp_dir = os.path.join(base_dir, "MES - JP")

def hex_dump(data):
    return " ".join(f"{b:02X}" for b in data)

for jf in glob.glob(os.path.join(tr_dir, "*.json")):
    with open(jf, 'r', encoding='utf-8-sig') as f:
        try:
            data = json.load(f)
            for item in data:
                if 'dalam' in item.get('message', '').lower():
                    print(f"Match found in: {os.path.basename(jf)}")
                    print(f"Index: {item.get('index')}")
                    print(f"Message: {item.get('message')}")
                    
                    basename = os.path.basename(jf).replace(".json", ".mes")
                    tr_mes = os.path.join(mes_dir, basename)
                    jp_mes = os.path.join(jp_dir, basename)
                    
                    if os.path.exists(tr_mes):
                        with open(tr_mes, 'rb') as tf:
                            raw = tf.read()
                            idx = raw.find(b'S')
                            if idx != -1:
                                print(f"Hex dump from TR MES around 'S':")
                                print(hex_dump(raw[idx:idx+32]))
                    
                    if os.path.exists(jp_mes):
                        print(f"Original JP MES size: {os.path.getsize(jp_mes)}")

        except Exception as e:
            pass
