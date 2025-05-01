

import random
import json
from datetime import datetime, timedelta

def tao_cau_hoi_dat_ban():
    """Tạo câu hỏi đặt bàn và thực thể liên quan (không có nhãn KHOẢNG_THỜI_GIAN)."""
    so_nguoi = random.randint(1, 10) if random.random() > 0.3 else None  # Có thể có số bàn hoặc không có
    gio = random.randint(1, 20) if random.random() > 0.3 else None
    ngay = random.randint(1, 28) if random.random() > 0.3 else None
    thang = random.randint(1, 12) if random.random() > 0.3 else None

    # Danh sách nhãn còn lại
    all_labels = ["SỐ_NGƯỜI", "GIỜ", "NGÀY", "THÁNG"]
    missing_labels = random.sample(all_labels, random.randint(0, 3))  # Có thể thiếu từ 0 đến 3 nhãn

    # Tạo câu hỏi theo mẫu, chỉ bao gồm nhãn không bị thiếu
    #cau_hoi_parts = [random.choice(["Tôi muốn đặt", "Đặt", "Muốn đặt"])]
        # Đảm bảo cau_hoi_parts là một danh sách
    cau_hoi_parts = []  # Khởi tạo là danh sách thay vì chuỗi
    #cau_hoi_parts = random.choice(["Tôi muốn đặt bàn", "Làm ơn đặt bàn giúp tôi", "Có thể đặt bàn", "Xin đặt bàn"])
    cau_hoi_parts.append(random.choice(["Tôi muốn đặt bàn", "Làm ơn đặt bàn giúp tôi", "Có thể đặt bàn", "Xin đặt bàn"]))
    #cau_hoi_parts = random.choice(["Tôi muốn đặt bàn", "Làm ơn đặt bàn giúp tôi", "Có thể đặt bàn", "Xin đặt bàn"])
    missing_info = []  # Danh sách các nhãn thiếu

    # Xử lý số bàn
    if "SỐ_NGƯỜI" not in missing_labels:
        if so_nguoi is None:
            missing_info.append("SỐ_NGƯỜI")
            #cau_hoi_parts.append("NGƯỜI")
        else:
            cau_hoi_parts.append(f"{so_nguoi} người")
    else:
        missing_info.append("SỐ_BÀN")
        #cau_hoi_parts.append("bàn")

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
    if "SỐ_NGƯỜI" not in missing_labels and so_nguoi is not None:
        so_ban_str = str(so_nguoi)
        start = cau_hoi.find(so_ban_str, i)
        if start != -1:
            entities.append({"start": start, "end": start + len(so_ban_str), "label": "SỐ_NGƯỜI"})
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




'''def tao_cau_hoi_dat_ban():
    """Tạo câu hỏi đặt bàn thực tế với thực thể liên quan, một số thực thể có thể bị bỏ qua."""
    # Lấy thông tin ngẫu nhiên, có thể bị bỏ qua
    so_nguoi = random.randint(1, 10) if random.random() > 0.5 else None
    gio = random.randint(6, 22) if random.random() > 0.5 else None
    ngay = random.randint(1, 28) if random.random() > 0.5 else None
    thang = random.randint(1, 12) if random.random() > 0.5 else None

    # Tạo phần đầu câu hỏi
    cau_hoi_dau = random.choice(["Tôi muốn đặt bàn", "Làm ơn đặt bàn giúp tôi", "Có thể đặt bàn", "Xin đặt bàn"])

    # Tạo các phần còn lại của câu hỏi (chỉ thêm nếu thông tin có sẵn)
    cau_hoi_parts = []
    if so_nguoi:
        cau_hoi_parts.append(f"{so_nguoi} người")
    if gio:
        cau_hoi_parts.append(f"lúc {gio} giờ")
    if ngay:
        cau_hoi_parts.append(f"ngày {ngay}")
    if thang:
        cau_hoi_parts.append(f"tháng {thang}")

    # Đảo vị trí các phần còn lại
    random.shuffle(cau_hoi_parts)

    # Ghép phần đầu và các phần còn lại thành câu hỏi hoàn chỉnh
    cau_hoi = " ".join([cau_hoi_dau] + cau_hoi_parts).strip()

    # Xác định các thực thể (entities)
    entities = []
    used_positions = set()  # Lưu các vị trí đã được sử dụng

    def add_entity(value, label):
        """Thêm thực thể vào danh sách nếu không bị trùng vị trí."""
        if value is not None:
            str_value = str(value)
            start = cau_hoi.find(str_value)
            end = start + len(str_value)
            # Kiểm tra vị trí đã sử dụng
            if start != -1 and (start, end) not in used_positions:
                entities.append({"start": start, "end": end, "label": label})
                used_positions.add((start, end))  # Đánh dấu vị trí đã sử dụng

    # Thêm các thực thể
    add_entity(so_nguoi, "SỐ_NGƯỜI")
    add_entity(gio, "GIỜ")
    add_entity(ngay, "NGÀY")
    add_entity(thang, "THÁNG")

    # Trả về câu hỏi và danh sách thực thể
    return cau_hoi, entities'''


def tao_cau_hoi_goi_y_mon_an():

    # Mapping thể loại từ tiếng Anh sang tiếng Việt
    category_mapping = {
        "Main Course": "món chính",
        "Appetizer": "khai vị",
        "Beverage": "đồ uống",
        "Dessert": "tráng miệng",
    }

    categories = list(category_mapping.values())
    prices = [20000, 25000, 30000, 35000, 40000, 50000, 200000]

    # Chọn ngẫu nhiên thể loại và giá
    category = random.choice(categories) if random.random() > 0.5 else None
    max_price = random.choice(prices) if random.random() > 0.5 else None

    # Đảm bảo luôn có ít nhất một điều kiện
    if category is None and max_price is None:
        category = random.choice(categories)

    # Tạo câu hỏi với các điều kiện
    '''if category and max_price:
        cau_hoi = f"Bạn có thể gợi ý {category} với giá dưới {max_price} đồng không?"
    elif category:
        cau_hoi = f"Tôi muốn tìm món thuộc thể loại {category}."
    else:  # Chỉ có giá
        cau_hoi = f"Tôi muốn tìm món với giá dưới {max_price} đồng."'''
    if category and max_price:
        cau_hoi_templates = [
        f"Bạn có thể gợi ý {category} với giá dưới {max_price} đồng không?",
        f"Tôi muốn tìm một món {category} có giá không quá {max_price} đồng.",
        f"Tôi đang cần món {category}, giá dưới {max_price} đồng. Bạn có thể giúp tôi không?",
        f"Có món {category} nào giá dưới {max_price} đồng không?",
        f"Tôi muốn gọi món {category} và giá không vượt quá {max_price} đồng, bạn có thể gợi ý không?",
    ]
        cau_hoi = random.choice(cau_hoi_templates)
    elif category:
        cau_hoi_templates = [
        f"Tôi muốn tìm món thuộc thể loại {category}.",
        f"Bạn có thể gợi ý vài món trong nhóm {category} không?",
        f"Món ăn thuộc thể loại {category} nào đang phổ biến?",
        f"Có món {category} nào ngon bạn muốn giới thiệu không?",
        f"Tôi đang cần tìm một số món {category}, bạn có thể gợi ý không?",
    ]
        cau_hoi = random.choice(cau_hoi_templates)
    else:  # Chỉ có giá
        cau_hoi_templates = [
            f"Tôi muốn tìm món với giá dưới {max_price} đồng.",
            f"Có món nào giá không quá {max_price} đồng mà ngon không?",
            f"Bạn có thể gợi ý món ăn có giá dưới {max_price} đồng không?",
            f"Tôi đang tìm món ăn với ngân sách khoảng {max_price} đồng, bạn có ý tưởng gì không?",
            f"Với giá dưới {max_price} đồng, tôi nên chọn món nào?",
        ]
        cau_hoi = random.choice(cau_hoi_templates)


    # Hàm tìm thủ công vị trí bắt đầu và kết thúc của chuỗi con
    def find_offsets(text, subtext):
        start = text.find(subtext)
        if start != -1:
            end = start + len(subtext)
            return start, end
        raise ValueError(f"'{subtext}' not found in '{text}'")

    # Xác định thực thể
    entities = []
    if category:
        try:
            start_category, end_category = find_offsets(cau_hoi, category)
            entities.append({"start": start_category, "end": end_category, "label": "THỂ_LOẠI"})
        except ValueError as e:
            print(f"Error locating category: {e}")
    if max_price:
        try:
            start_price, end_price = find_offsets(cau_hoi, str(max_price))
            entities.append({"start": start_price, "end": end_price, "label": "GIÁ"})
        except ValueError as e:
            print(f"Error locating price: {e}")

    return cau_hoi, entities





def tao_cau_hoi_goi_mon():
    """Tạo câu hỏi gọi món thực tế hơn với các thực thể không trùng lặp."""
    menu_items = [
    {"name": "Cơm chiên", "description": "Món cơm chiên với trứng, rau củ và các loại thịt như gà hoặc tôm.", "category": "Main Course"},
    {"name": "Phở bò", "description": "Món phở truyền thống với nước dùng đậm đà, bánh phở và thịt bò thái lát mỏng.", "category": "Main Course"},
    {"name": "Bún chả", "description": "Món bún với thịt nướng (chả) và nước mắm pha chua ngọt, ăn kèm với rau sống.", "category": "Main Course"},
    {"name": "Bánh mì", "description": "Một loại bánh mì với nhiều loại nhân phong phú như thịt nguội, pate, rau củ.", "category": "Appetizer"},
    {"name": "Gỏi cuốn", "description": "Cuốn bánh tráng với tôm, rau sống và bún, ăn kèm với nước chấm đậm đà.", "category": "Appetizer"},
    {"name": "Trà sữa", "description": "Đồ uống ngọt, gồm trà và sữa, có thể thêm các loại topping như trân châu.", "category": "Beverage"},
    {"name": "Cà phê", "description": "Cà phê đen hoặc cà phê sữa, được pha chế theo phong cách Việt Nam.", "category": "Beverage"},
    {"name": "Bánh cuốn", "description": "Món bánh cuốn mỏng, thường ăn kèm với thịt băm, nấm và nước mắm.", "category": "Appetizer"},
    {"name": "Bánh bao", "description": "Bánh bao nhồi với thịt heo, trứng và các loại gia vị.", "category": "Appetizer"},
    {"name": "Bánh xèo", "description": "Món bánh mỏng, giòn với nhân tôm, thịt và giá đỗ, thường ăn kèm với rau sống.", "category": "Main Course"},
    {"name": "Bánh tét", "description": "Món bánh nếp gói trong lá chuối, nhân đậu xanh và thịt lợn, thường ăn vào dịp Tết.", "category": "Main Course"},
    {"name": "Bánh đúc", "description": "Món bánh làm từ bột gạo, có thể ăn với nước mắm hoặc nước cốt dừa.", "category": "Main Course"},
    {"name": "Bánh ướt", "description": "Món bánh mỏng, mềm, ăn kèm với thịt nướng, chả, rau sống và nước mắm.", "category": "Main Course"},
    {"name": "Bánh chưng", "description": "Món bánh truyền thống làm từ gạo nếp, đậu xanh và thịt lợn, gói trong lá dong.", "category": "Main Course"},
    {"name": "Bánh rán", "description": "Món bánh rán ngọt với nhân đậu xanh hoặc đậu đỏ, chiên giòn và phủ đường.", "category": "Dessert"},
    {"name": "Bánh bột lọc", "description": "Món bánh bột lọc trong suốt, nhân tôm thịt, ăn kèm với nước mắm.", "category": "Appetizer"},
    {"name": "Bánh bông lan", "description": "Bánh ngọt mềm, nhẹ, có thể ăn kèm với kem hoặc trái cây.", "category": "Dessert"},
    {"name": "Bánh cam", "description": "Món bánh chiên giòn, nhân đậu xanh hoặc mè, hình dáng giống quả cam.", "category": "Dessert"},
    {"name": "Bánh nậm", "description": "Món bánh gói trong lá chuối, làm từ bột gạo và nhân tôm thịt.", "category": "Main Course"},
    {"name": "Bánh pía", "description": "Bánh ngọt có lớp vỏ mềm, nhân đậu xanh và durian hoặc sầu riêng.", "category": "Dessert"},
    {"name": "Bánh khọt", "description": "Bánh nhỏ, giòn, có nhân tôm và rau, ăn kèm với nước mắm chua ngọt.", "category": "Appetizer"},
    {"name": "Cháo gà", "description": "Cháo nấu từ thịt gà, hành, và tiêu, ăn nóng.", "category": "Main Course"},
    {"name": "Lẩu hải sản", "description": "Món lẩu với nước dùng cay, hải sản tươi và rau xanh.", "category": "Main Course"},
    {"name": "Chả giò", "description": "Món cuốn chiên giòn, nhân thịt băm, miến và rau củ.", "category": "Appetizer"},
    {"name": "Nem nướng", "description": "Thịt heo xay nướng thơm, ăn kèm với bánh tráng và nước chấm.", "category": "Main Course"},
    {"name": "Cơm tấm", "description": "Món cơm với sườn nướng, bì, chả trứng và nước mắm.", "category": "Main Course"},
    {"name": "Sữa chua mít", "description": "Món tráng miệng từ sữa chua, mít và topping đa dạng.", "category": "Dessert"},
    {"name": "Chè thập cẩm", "description": "Món chè truyền thống với nhiều loại topping và nước cốt dừa.", "category": "Dessert"},
    {"name": "Bún riêu", "description": "Món bún với nước dùng chua cay, gạch cua và cà chua.", "category": "Main Course"},
    {"name": "Mì xào bò", "description": "Mì xào với thịt bò, rau cải và sốt đậm đà.", "category": "Main Course"},
    {"name": "Soda chanh", "description": "Đồ uống giải khát với soda và chanh tươi.", "category": "Beverage"},
    {"name": "Súp cua", "description": "Món súp với thịt cua, rau củ và gia vị.", "category": "Main Course"},
    {"name": "Bánh canh cua", "description": "Món bánh canh với thịt cua, nước dùng ngọt và rau sống.", "category": "Main Course"},
    {"name": "Hủ tiếu Nam Vang", "description": "Món hủ tiếu với thịt, tôm, và nước dùng đậm đà.", "category": "Main Course"},
    {"name": "Bò kho", "description": "Món bò kho với thịt bò mềm, nước sốt đậm đà, ăn kèm với bánh mì.", "category": "Main Course"},
    {"name": "Bánh crepe", "description": "Bánh crepe với nhân ngọt hoặc mặn, có thể ăn kèm với trái cây hoặc kem.", "category": "Dessert"},
    {"name": "Hamburger", "description": "Món hamburger với thịt bò, rau củ và sốt đặc biệt.", "category": "Main Course"},
    {"name": "Khoai tây chiên", "description": "Món khoai tây chiên giòn, ăn kèm với sốt ketchup hoặc mayonnaise.", "category": "Main Course"},
    {"name": "Cơm gà Hải Nam", "description": "Món cơm gà Hải Nam với gà luộc mềm, cơm nấu từ nước luộc gà và gia vị.", "category": "Main Course"},
    {"name": "Bánh flan", "description": "Món bánh flan mềm, mịn với lớp caramel ngọt.", "category": "Dessert"},
    {"name": "Chè bắp", "description": "Món chè làm từ ngô, sữa và đường, có vị ngọt thanh.", "category": "Dessert"},
    {"name": "Chè chuối", "description": "Món chè làm từ chuối và nước cốt dừa, vị ngọt và béo.", "category": "Dessert"},
    {"name": "Kem chiên", "description": "Món kem chiên giòn với lớp vỏ ngoài giòn tan và nhân kem lạnh bên trong.", "category": "Dessert"},
    {"name": "Xúc xích nướng", "description": "Xúc xích được nướng thơm, có thể ăn kèm với bánh mì hoặc rau.", "category": "Appetizer"},
    {"name": "Pizza", "description": "Món pizza với lớp đế giòn, phủ nhiều loại nhân như thịt, phô mai, rau củ.", "category": "Main Course"},
    {"name": "Cá viên chiên", "description": "Món cá viên chiên giòn, ăn kèm với nước chấm đặc biệt.", "category": "Appetizer"},
    {"name": "Há cảo", "description": "Món há cảo hấp với nhân tôm thịt, vỏ bánh mềm mịn.", "category": "Appetizer"},
    {"name": "Xíu mại", "description": "Món xíu mại từ thịt heo, ăn kèm với cơm hoặc bánh mì.", "category": "Main Course"},
    {"name": "Sinh tố", "description": "Đồ uống làm từ trái cây tươi xay nhuyễn, có thể thêm sữa hoặc đá.", "category": "Beverage"},
    {"name": "Nước dừa", "description": "Nước uống từ quả dừa tươi, mát lạnh và ngọt tự nhiên.", "category": "Beverage"},
    {"name": "Chả quế", "description": "Món chả quế thơm lừng với thịt heo, quế và gia vị.", "category": "Appetizer"},
    {"name": "Bún cá", "description": "Món bún với cá chiên giòn, nước dùng đậm đà.", "category": "Main Course"},
    {"name": "Nem nướng Nha Trang", "description": "Nem nướng từ thịt heo xay, nướng thơm, ăn kèm với bánh tráng.", "category": "Appetizer"},
    {"name": "Bún bò Huế", "description": "Món bún bò với nước dùng cay, thịt bò và giò heo.", "category": "Main Course"},
    {"name": "Bánh mì nướng Lạng Sơn", "description": "Bánh mì nướng với mỡ hành, ăn kèm với thịt và rau.", "category": "Appetizer"},
    {"name": "Phở cuốn", "description": "Món phở cuốn với thịt bò, rau sống và nước mắm.", "category": "Appetizer"},
    {"name": "Mì vằn thắn", "description": "Món mì với nước dùng ngọt thanh, thịt, tôm và rau củ.", "category": "Main Course"},
    {"name": "Bún riêu cua", "description": "Món bún với nước dùng cua, rau và gia vị.", "category": "Main Course"},
    {"name": "Sữa đậu", "description": "Đồ uống từ sữa đậu nành, ngọt thanh và bổ dưỡng.", "category": "Beverage"},
    {"name": "Trà chanh", "description": "Đồ uống giải khát với trà và nước chanh tươi.", "category": "Beverage"}
]


    # Số lượng món ăn trong yêu cầu
    so_luong_mon = random.randint(1, 3)
    mon_goi = random.sample(menu_items, so_luong_mon)  # Chọn ngẫu nhiên các món ăn

    # Tạo câu hỏi gọi món
    cau_hoi_parts = []
    for mon in mon_goi:
        so_luong = random.randint(1, 5)  # Số lượng mỗi món
        cau_hoi_parts.append(f"{so_luong} {mon['name']}")  # Ghép số lượng và tên món

    # Ghép các món thành câu hỏi
    if len(cau_hoi_parts) > 1:
        cau_hoi = random.choice([
            f"Tôi muốn gọi {', '.join(cau_hoi_parts[:-1])} và {cau_hoi_parts[-1]}.",
            f"Cho tôi gọi {', '.join(cau_hoi_parts[:-1])} với {cau_hoi_parts[-1]}.",
        ])
    else:
        cau_hoi = f"Tôi muốn gọi {cau_hoi_parts[0]}."

    # Tạo danh sách thực thể (entities)
    entities = []
    for part in cau_hoi_parts:
        so_luong, ten_mon = part.split(maxsplit=1)  # Tách số lượng và tên món

        # Tìm vị trí số lượng
        start_sl = cau_hoi.find(so_luong)
        end_sl = start_sl + len(so_luong)

        # Tìm vị trí tên món
        start_mon = cau_hoi.find(ten_mon, end_sl)
        end_mon = start_mon + len(ten_mon)

        # Chỉ thêm thực thể nếu không có xung đột
        if all(not (start_sl <= ent["end"] and end_sl >= ent["start"]) for ent in entities):
            entities.append({"start": start_sl, "end": end_sl, "label": "SỐ_LƯỢNG"})
        if all(not (start_mon <= ent["end"] and end_mon >= ent["start"]) for ent in entities):
            entities.append({"start": start_mon, "end": end_mon, "label": "MÓN_ĂN"})

    return cau_hoi, entities


def detect_missing_info(topic, entities):
    """Xác định thông tin còn thiếu."""
    required_entities = {
        "đặt bàn": ["SỐ_NGƯỜI", "GIỜ", "NGÀY", "THÁNG"],
        "gọi món": ["MÓN_ĂN", "SỐ_LƯỢNG"],
        "gợi ý món ăn": ["THỂ_LOẠI", "GIÁ"]  # Sử dụng định dạng chuẩn "THỂ_LOẠI"
    }

    # Chuyển đổi tên thực thể từ entities để đồng nhất
    detected_labels = [ent["label"].upper() for ent in entities]

    missing_info = []
    for entity in required_entities.get(topic.lower(), []):
        if entity not in detected_labels:
            missing_info.append(entity)

    return missing_info

def generate_ner_data(data_path, num_samples=5000):
    """Tạo dữ liệu huấn luyện NER."""
    data = []
    for _ in range(num_samples):
        topic = random.choice(["đặt bàn", "gợi ý món ăn", "gọi món"])

        if topic == "đặt bàn":
            cau_hoi, entities, missing_info = tao_cau_hoi_dat_ban()
        elif topic == "gợi ý món ăn":
            cau_hoi, entities = tao_cau_hoi_goi_y_mon_an()
        elif topic == "gọi món":
            cau_hoi, entities = tao_cau_hoi_goi_mon()

        missing_info = detect_missing_info(topic, entities)

        data.append({
            "topic": topic,
            "user_query": cau_hoi,
            "missing_info": missing_info,
            "entity_detected": entities
        })

        
    with open(data_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=4)
    print(f"Dữ liệu đã được lưu tại: {data_path}")

if __name__ == "__main__":
    generate_ner_data("ner_training_data.json", num_samples=45000)
    print("Dữ liệu đã được tạo.")

