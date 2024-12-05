

import json
import random
import random
import re
import random

def tao_cau_hoi_dat_ban():
    """Tạo câu hỏi đặt bàn và thực thể liên quan (không có nhãn KHOẢNG_THỜI_GIAN)."""
    so_ban = random.randint(1, 5) if random.random() > 0.5 else None  # Có thể có số bàn hoặc không có
    gio = random.randint(1, 20) if random.random() > 0.5 else None
    ngay = random.randint(1, 28) if random.random() > 0.5 else None
    thang = random.randint(1, 12) if random.random() > 0.5 else None

    # Danh sách nhãn còn lại
    all_labels = ["SỐ_BÀN", "GIỜ", "NGÀY", "THÁNG"]
    missing_labels = random.sample(all_labels, random.randint(0, 3))  # Có thể thiếu từ 0 đến 3 nhãn

    # Tạo câu hỏi theo mẫu, chỉ bao gồm nhãn không bị thiếu
    cau_hoi_parts = [random.choice(["Tôi muốn đặt", "Đặt", "Muốn đặt"])]
    missing_info = []  # Danh sách các nhãn thiếu

    # Xử lý số bàn
    if "SỐ_BÀN" not in missing_labels:
        if so_ban is None:
            missing_info.append("SỐ_BÀN")
            cau_hoi_parts.append("bàn")
        else:
            cau_hoi_parts.append(f"{so_ban} bàn")
    else:
        missing_info.append("SỐ_BÀN")
        cau_hoi_parts.append("bàn")

    # Xử lý giờ
    if "GIỜ" not in missing_labels:
        if gio is not None:
            cau_hoi_parts.append(f"lúc {gio} giờ")
        else:
            missing_info.append("GIỜ")
    else:
        missing_info.append("GIỜ")

    # Xử lý ngày
    if "NGÀY" not in missing_labels:
        if ngay is not None:
            cau_hoi_parts.append(f"ngày {ngay}")
        else:
            missing_info.append("NGÀY")
    else:
        missing_info.append("NGÀY")

    # Xử lý tháng
    if "THÁNG" not in missing_labels:
        if thang is not None:
            cau_hoi_parts.append(f"tháng {thang}")
        else:
            missing_info.append("THÁNG")
    else:
        missing_info.append("THÁNG")

    cau_hoi = " ".join(cau_hoi_parts)

    # Danh sách các thực thể cần xác định
    entities = []
    i = 0  # Chỉ số để duyệt qua câu hỏi

    # Xác định thực thể SỐ_BÀN
    if "SỐ_BÀN" not in missing_labels and so_ban is not None:
        so_ban_str = str(so_ban)
        start = cau_hoi.find(so_ban_str, i)
        if start != -1:
            entities.append({"start": start, "end": start + len(so_ban_str), "label": "SỐ_BÀN"})
            i = start + len(so_ban_str)

    # Xác định thực thể GIỜ
    if "GIỜ" not in missing_labels and gio is not None:
        gio_str = str(gio)
        start = cau_hoi.find(gio_str, i)
        if start != -1:
            entities.append({"start": start, "end": start + len(gio_str), "label": "GIỜ"})
            i = start + len(gio_str)

    # Xác định thực thể NGÀY
    if "NGÀY" not in missing_labels and ngay is not None:
        ngay_str = str(ngay)
        start = cau_hoi.find(ngay_str, i)
        if start != -1:
            entities.append({"start": start, "end": start + len(ngay_str), "label": "NGÀY"})
            i = start + len(ngay_str)

    # Xác định thực thể THÁNG
    if "THÁNG" not in missing_labels and thang is not None:
        thang_str = str(thang)
        start = cau_hoi.find(thang_str, i)
        if start != -1:
            entities.append({"start": start, "end": start + len(thang_str), "label": "THÁNG"})
            i = start + len(thang_str)

    # Trả về câu hỏi, các thực thể đã nhận diện, và nhãn bị thiếu
    return cau_hoi, entities, missing_info

def tao_cau_hoi_trang_thai_order():
    """Tạo câu hỏi kiểm tra trạng thái order và thực thể liên quan."""
    mon_an = [
    {"name": "Cơm chiên"},
    {"name": "Phở bò"},
    {"name": "Bún chả"},
    {"name": "Bánh mì"},
    {"name": "Gỏi cuốn"},
    {"name": "Trà sữa"},
    {"name": "Cà phê"},
    {"name": "Bánh cuốn"},
    {"name": "Bánh bao"},
    {"name": "Bánh xèo"},
    {"name": "Bánh tét"},
    {"name": "Bánh đúc"},
    {"name": "Bánh ướt"},
    {"name": "Bánh chưng"},
    {"name": "Bánh rán"},
    {"name": "Bánh bột lọc"},
    {"name": "Bánh bông lan"},
    {"name": "Bánh cam"},
    {"name": "Bánh nậm"},
    {"name": "Bánh pía"},
    {"name": "Bánh khọt"},
    {"name": "Cháo gà"},
    {"name": "Lẩu hải sản"},
    {"name": "Chả giò"},
    {"name": "Nem nướng"},
    {"name": "Cơm tấm"},
    {"name": "Sữa chua mít"},
    {"name": "Chè thập cẩm"},
    {"name": "Bún riêu"},
    {"name": "Mì xào bò"},
    {"name": "Soda chanh"},
    {"name": "Súp cua"},
    {"name": "Bánh canh cua"},
    {"name": "Hủ tiếu Nam Vang"},
    {"name": "Bò kho"},
    {"name": "Bánh crepe"},
    {"name": "Hamburger"},
    {"name": "Khoai tây chiên"},
    {"name": "Cơm gà Hải Nam"},
    {"name": "Bánh flan"},
    {"name": "Chè bắp"},
    {"name": "Chè chuối"},
    {"name": "Kem chiên"},
    {"name": "Xúc xích nướng"},
    {"name": "Pizza"},
    {"name": "Cá viên chiên"},
    {"name": "Há cảo"},
    {"name": "Xíu mại"},
    {"name": "Sinh tố"},
    {"name": "Nước dừa"},
    {"name": "Chả quế"},
    {"name": "Bún cá"},
    {"name": "Nem nướng Nha Trang"},
    {"name": "Bún bò Huế"},
    {"name": "Bánh mì nướng Lạng Sơn"},
    {"name": "Phở cuốn"},
    {"name": "Mì vằn thắn"},
    {"name": "Bún riêu cua"},
    {"name": "Sữa đậu"},
    {"name": "Trà chanh"}
]
    
    # Lựa chọn ngẫu nhiên món ăn từ danh sách
    mon = random.choice(mon_an)["name"]
    
    # Tạo câu hỏi kiểm tra trạng thái order
    cau_hoi = f"{mon} của tôi đã được chuẩn bị xong chưa?"
    
    # Tính toán start và end cho thực thể
    start = cau_hoi.find(mon)
    entities = [{"start": start, "end": start + len(mon), "label": "MÓN_ĂN"}]
    
    return cau_hoi, entities


def tao_cau_hoi_goi_mon():
    """Tạo câu hỏi gọi món và thực thể liên quan."""
    mon_an = [
    {"name": "Cơm chiên"},
    {"name": "Phở bò"},
    {"name": "Bún chả"},
    {"name": "Bánh mì"},
    {"name": "Gỏi cuốn"},
    {"name": "Trà sữa"},
    {"name": "Cà phê"},
    {"name": "Bánh cuốn"},
    {"name": "Bánh bao"},
    {"name": "Bánh xèo"},
    {"name": "Bánh tét"},
    {"name": "Bánh đúc"},
    {"name": "Bánh ướt"},
    {"name": "Bánh chưng"},
    {"name": "Bánh rán"},
    {"name": "Bánh bột lọc"},
    {"name": "Bánh bông lan"},
    {"name": "Bánh cam"},
    {"name": "Bánh nậm"},
    {"name": "Bánh pía"},
    {"name": "Bánh khọt"},
    {"name": "Cháo gà"},
    {"name": "Lẩu hải sản"},
    {"name": "Chả giò"},
    {"name": "Nem nướng"},
    {"name": "Cơm tấm"},
    {"name": "Sữa chua mít"},
    {"name": "Chè thập cẩm"},
    {"name": "Bún riêu"},
    {"name": "Mì xào bò"},
    {"name": "Soda chanh"},
    {"name": "Súp cua"},
    {"name": "Bánh canh cua"},
    {"name": "Hủ tiếu Nam Vang"},
    {"name": "Bò kho"},
    {"name": "Bánh crepe"},
    {"name": "Hamburger"},
    {"name": "Khoai tây chiên"},
    {"name": "Cơm gà Hải Nam"},
    {"name": "Bánh flan"},
    {"name": "Chè bắp"},
    {"name": "Chè chuối"},
    {"name": "Kem chiên"},
    {"name": "Xúc xích nướng"},
    {"name": "Pizza"},
    {"name": "Cá viên chiên"},
    {"name": "Há cảo"},
    {"name": "Xíu mại"},
    {"name": "Sinh tố"},
    {"name": "Nước dừa"},
    {"name": "Chả quế"},
    {"name": "Bún cá"},
    {"name": "Nem nướng Nha Trang"},
    {"name": "Bún bò Huế"},
    {"name": "Bánh mì nướng Lạng Sơn"},
    {"name": "Phở cuốn"},
    {"name": "Mì vằn thắn"},
    {"name": "Bún riêu cua"},
    {"name": "Sữa đậu"},
    {"name": "Trà chanh"}
]



    so_luong_mon = random.randint(1, 4)
    mon_goi = random.sample(mon_an, so_luong_mon)

    # Tạo câu hỏi với danh sách món ăn và số lượng
    cau_hoi_parts = [f"{random.randint(1, 3)} {mon['name']}" for mon in mon_goi]
    cau_hoi_1 = f"Tôi muốn gọi {', '.join(cau_hoi_parts[:-1])} và {cau_hoi_parts[-1]}."
    cau_hoi_2 = f"Cho tôi gọi {', '.join(cau_hoi_parts[:-1])} và {cau_hoi_parts[-1]}."
    cau_hoi = random.choice([cau_hoi_1, cau_hoi_2])  # Sửa lại để chọn ngẫu nhiên giữa hai câu hỏi
    # Duyệt câu từ đầu đến cuối để xác định thực thể
    entities = []
    i = 0  # Chỉ số để duyệt qua câu hỏi

    for mon in cau_hoi_parts:
        # Phân tách số lượng và tên món ăn
        parts = mon.split(maxsplit=1)
        so_luong = parts[0]
        ten_mon = parts[1] if len(parts) > 1 else ""

        # Xác định thực thể SỐ_LƯỢNG
        start_so_luong = cau_hoi.find(so_luong, i)
        if start_so_luong != -1:
            entities.append({
                "start": start_so_luong,
                "end": start_so_luong + len(so_luong),
                "label": "SỐ_LƯỢNG"
            })
            i = start_so_luong + len(so_luong)  # Cập nhật chỉ số i

        # Xác định thực thể MÓN_ĂN
        start_mon = cau_hoi.find(ten_mon, i)
        if start_mon != -1:
            entities.append({
                "start": start_mon,
                "end": start_mon + len(ten_mon),
                "label": "MÓN_ĂN"
            })
            i = start_mon + len(ten_mon)  # Cập nhật chỉ số i

    return cau_hoi, entities


def detect_missing_info(topic, entities):
    """Xác định thông tin còn thiếu."""
    required_entities = {
        "đặt bàn": ["SỐ_BÀN", "GIỜ", "NGÀY", "THÁNG"],
        "trạng thái order": ["MÓN_ĂN"],
        "gọi món": ["MÓN_ĂN", "SỐ_LƯỢNG"]
    }

    missing_info = []
    for entity in required_entities.get(topic, []):
        if not any(ent["label"] == entity for ent in entities):
            missing_info.append(entity)

    return missing_info


def generate_ner_data(data_path, num_samples=5000):
    """Tạo dữ liệu huấn luyện NER."""
    data = []
    '''topic = "đặt bàn"
    #topic = "gọi món"
    #topic = "trạng thái order"
    for _ in range(num_samples):
        cau_hoi, entities, missing_info = tao_cau_hoi_dat_ban()
        #cau_hoi, entities = tao_cau_hoi_trang_thai_order()
        #cau_hoi, entities = tao_cau_hoi_goi_mon()
        #missing_info = detect_missing_info(topic,entities)
        entity_detected = [{"start": ent["start"], "end": ent["end"], "label": ent["label"]} for ent in entities]

        # Lọc missing_info sao cho không chứa những nhãn đã có trong entity_detected
        #missing_info = detect_missing_info(topic, entities)        #entity_detected = [{"start": ent["start"], "end": ent["end"], "label": ent["label"]} for ent in entities]
        data.append({
                "topic": topic,
                "user_query": cau_hoi,
                "missing_info": missing_info,
                "entity_detected": entity_detected
            })
    with open(data_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=4)
    print(f"Dữ liệu đã được lưu tại: {data_path}")
    '''
    for _ in range(num_samples):
        topic = random.choice(["đặt bàn", "trạng thái order", "gọi món"])

        if topic == "đặt bàn":
            cau_hoi, entities, missing_info = tao_cau_hoi_dat_ban()
        elif topic == "trạng thái order":
            cau_hoi, entities = tao_cau_hoi_trang_thai_order()
        elif topic == "gọi món":
            cau_hoi, entities = tao_cau_hoi_goi_mon()

        # Xác định thông tin còn thiếu
        missing_info = detect_missing_info(topic, entities)

        # Chuẩn bị output
        entity_detected = [{"start": ent["start"], "end": ent["end"], "label": ent["label"]} for ent in entities]

        data.append({
            "topic": topic,
            "user_query": cau_hoi,
            "missing_info": missing_info,
            "entity_detected": entity_detected
        })
    with open(data_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=4)
    print(f"Dữ liệu đã được lưu tại: {data_path}")

    # Lưu dữ liệu vào file JSON


if __name__ == "__main__":
    generate_ner_data("training_data.json", num_samples=50000) 
    print("Dữ liệu đã được tạo.")

