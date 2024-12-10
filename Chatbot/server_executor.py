import mysql.connector
from mysql.connector import Error
import uuid
from topic_classification import test_ner_model as tnm
from datetime import datetime
import random
import json 
import math
from flask import session
from database_util import execute_query, connect_to_database
# Đường dẫn đến dữ liệu huấn luyện và mô hình NER
data_path = 'ner_training_data.json'
model_path = "ner_model"

# Kết nối cơ sở dữ liệu MySQL


def xu_ly_hoi_thoai_dat_ban(entities, miss_info):
    """
    Xử lý các thực thể để lấy thông tin đặt bàn.
    Nếu thiếu thông tin, trả về thông báo yêu cầu người dùng cung cấp đủ thông tin.
    """
    if miss_info:
        return None, f"Đề nghị bạn đặt bàn theo cú pháp: Tôi muốn đặt x bàn lúc y giờ ngày m tháng n. Thiếu thông tin: {', '.join(miss_info)}"

    # Duyệt qua các thực thể để trích xuất thông tin
    so_nguoi = gio = ngay = thang = None
    for entity in entities:
        label = entity['label']
        text = entity['text']
        if label == 'SỐ_NGƯỜI':
            so_nguoi = int(text)
        elif label == 'GIỜ':
            gio = int(text)
        elif label == 'NGÀY':
            ngay = int(text)
        elif label == 'THÁNG':
            thang = int(text)

    # Kiểm tra thông tin có đầy đủ không
    if not all([so_nguoi, gio, ngay, thang]):
        return None, "Thông tin không đầy đủ. Đề nghị bạn kiểm tra lại cú pháp đặt bàn."

    return (so_nguoi, gio, ngay, thang), None


def xu_ly_yeu_cau_dat_ban(so_nguoi, gio, ngay, thang, note=None, customer_id=None, updateStatus=True):
    """
    Xử lý yêu cầu đặt bàn, kiểm tra bàn trống, và thêm thông tin đặt bàn mà không cần quản lý số bàn (reservationInfo).
    """
    TOTAL_TABLES = 20  # Tổng số bàn trong nhà hàng

    # Lấy EmployeeID ngẫu nhiên
    query_employee = "SELECT EmployeeID FROM Employee ORDER BY RAND() LIMIT 1"
    employee_result = execute_query(query_employee)
    employee_id = employee_result[0]['EmployeeID'] if employee_result else None

    if not employee_id:
        return "Không thể xử lý yêu cầu vì không tìm thấy nhân viên."

    # Truy vấn kiểm tra tổng số bàn đã đặt tại thời gian yêu cầu
    query_kiem_tra = """
    SELECT COUNT(*) AS so_ban_su_dung
    FROM Reservation
    WHERE 
        Status = 'Confirmed' AND
        DATE(ReservationDate) = %s AND
        HOUR(ReservationDate) = %s
    """
    param_kiem_tra = (datetime(2024, thang, ngay), gio)
    result = execute_query(query_kiem_tra, param_kiem_tra)

    if result:
        so_ban_da_dat = result[0]['so_ban_su_dung']
        so_ban_con_lai = TOTAL_TABLES - so_ban_da_dat
        so_ban = math.ceil(so_nguoi / 6)
        if so_ban_con_lai >= so_ban:
            if updateStatus:
                # Thêm thông tin đặt bàn một lần
                reservation_date = datetime(2024, thang, ngay, gio, 0)
                reservation_id = str(uuid.uuid4())
                total_guests = so_ban * 6  # Giả sử mỗi bàn có 6 khách
                insert_query = """
                INSERT INTO Reservation (ReservationID, CustomerID, EmployeeID, ReservationDate, Status, GuestCount, Note)
                VALUES (%s, %s, %s, %s, %s, %s, %s)
                """
                insert_params = (
                    reservation_id, customer_id, employee_id,
                    reservation_date, 'Confirmed', total_guests, note
                )
                execute_query(insert_query, insert_params)

            return f"Đã xác nhận đặt {so_ban} bàn lúc {gio} giờ ngày {ngay}/{thang}. Chúng tôi đã giữ chỗ cho bạn."
        else:
            return f"Chỉ còn {so_ban_con_lai} bàn trống vào lúc đó. Bạn có muốn chọn thời gian khác không?"

    return "Không thể xác nhận đặt bàn. Vui lòng thử lại sau."




def xu_ly_hoi_thoai_goi_mon(entities):
    """
    Xử lý thông tin gọi món từ các thực thể đã nhận diện.
    """
    mon_an = []
    so_luong = []

    if not entities:  # Kiểm tra nếu không có thực thể nào
        return [], "Bạn hãy đặt lại món"  # Trả về danh sách trống và lỗi

    current_quantity = 1  # Mặc định số lượng nếu không có thực thể `SỐ_LƯỢNG`

    for entity in entities:
        if entity['label'] == 'SỐ_LƯỢNG':
            # Chuyển đổi text thành số lượng (ép kiểu nếu cần)
            try:
                current_quantity = int(entity['text'])
            except ValueError:
                current_quantity = 1  # Mặc định là 1 nếu không hợp lệ
        elif entity['label'] == 'MÓN_ĂN':
            # Thêm món ăn vào danh sách và gắn số lượng hiện tại
            mon_an.append(entity['text'])
            so_luong.append(current_quantity)
            current_quantity = 1  # Reset số lượng về mặc định sau mỗi món ăn

    # Kiểm tra nếu không có món ăn nào được xác định
    if not mon_an:
        return [], "Không tìm thấy món ăn nào trong yêu cầu của bạn."

    # Trả về danh sách món ăn và số lượng
    return {'mon_an': mon_an, 'so_luong': so_luong}, None


def xu_ly_yeu_cau_goi_mon(mon_an, so_luong, special_request=None, updateStatus=True, customerID=None, customer_name=None):
    """
    Xử lý yêu cầu gọi món:
    - Truy vấn giá từng món từ bảng Menu.
    - Tạo đơn hàng mới và lưu vào bảng `Order` và `OrderDetails`.
    """
    if not mon_an or not so_luong:
        return "Danh sách món ăn hoặc số lượng không hợp lệ."

    # Khởi tạo tổng đơn hàng
    sum_don_hang = 0
    order_details = []
    table_number = random.randint(1, 20)

    # Duyệt qua từng món ăn trong danh sách
    for i in range(len(mon_an)):
        mon = mon_an[i]
        quantity = so_luong[i]
        param = (mon,)

        # Truy vấn giá của món ăn từ bảng Menu
        query = """
        SELECT Price, MenuItemID, OutOfStock FROM Menu WHERE Name = %s
        """
        result = execute_query(query, param)

        if not result:
            return f"Không tìm thấy món ăn {mon} trong menu."

        price = result[0]['Price']
        menu_item_id = result[0]['MenuItemID']
        out_of_stock = result[0]['OutOfStock']

        # Kiểm tra xem món có hết hàng không
        if out_of_stock:
            return f"Món {mon} hiện đã hết hàng, vui lòng chọn món khác."

        # Cập nhật tổng đơn hàng
        sum_don_hang += price * quantity

        # Lưu thông tin chi tiết đơn hàng
        order_details.append({
            'MenuItemID': menu_item_id,
            'Quantity': quantity,
            'Price': price,
            'Note': f"Ordered {quantity} of {mon}"
        })

    # Tạo OrderID ngẫu nhiên
    order_id = str(uuid.uuid4())

    # Lấy thời gian hiện tại làm OrderDate
    order_date = datetime.now().strftime('%Y-%m-%d %H:%M:%S')

    if updateStatus:
        # Cập nhật bảng `Order`
        order_query = """
        INSERT INTO `Order` (OrderID, CustomerID, CustomerName, TableNumber, OrderDate, TotalAmount, Status, SpecialRequest)
        VALUES (%s, %s, %s, %s, %s, %s, 'Pending', %s)
        """
        order_params = (order_id, customerID, customer_name, table_number, order_date, sum_don_hang, special_request)
        execute_query(order_query, order_params)

        # Cập nhật bảng `OrderDetails`
        for detail in order_details:
            order_detail_id = str(uuid.uuid4())
            order_detail_query = """
            INSERT INTO OrderDetail (OrderDetailID, OrderID, MenuItemID, Quantity, Price, Note)
            VALUES (%s, %s, %s, %s, %s, %s)
            """
            order_detail_params = (
                order_detail_id, order_id, detail['MenuItemID'],
                detail['Quantity'], detail['Price'], detail['Note']
            )
            execute_query(order_detail_query, order_detail_params)

    return f"Đặt món thành công. Tổng đơn hàng là {sum_don_hang:.2f} VND."

def xu_ly_hoi_thoai_goi_y(entities):
    the_loai = []  # Khởi tạo là danh sách rỗng
    gia = []       # Khởi tạo là danh sách rỗng
    
    if not entities:  # Kiểm tra nếu không có thực thể nào
        return [], "Bạn hãy nhập lại yêu cầu"

    for entity in entities:
        if entity['label'] == 'THỂ_LOẠI':
            the_loai.append(entity['text'])
        elif entity['label'] == 'GIÁ':
            gia.append(int(entity['text']))

    # Trả về kết quả với cả thể loại và giá nếu có
    result = {}
    
    result['the_loai'] = the_loai
    result['gia'] = gia


    # Kiểm tra nếu không có thông tin nào được xác định
    if not result:
        return [], "Không tìm thấy thông tin cụ thể trong yêu cầu của bạn."

    return result, None

def xu_ly_yeu_cau_goi_y(the_loai, gia):
    category_mapping = {
        "món chính": "Main Course",
        "khai vị": "Appetizer",
        "đồ uống": "Beverage",
        "tráng miệng": "Dessert",
    }
    mapped_category = category_mapping.get(the_loai, the_loai)
    if gia and the_loai:
        query = "SELECT Name FROM MENU WHERE PRICE <= %s and Category = %s"
        param = (gia,mapped_category)
        result = execute_query(query,param)
    elif the_loai:
        query = "SELECT Name FROM MENU WHERE Category = %s"
        param = (mapped_category,)
        result = execute_query(query,param)
    elif gia:
        query = "SELECT Name FROM MENU WHERE PRICE <= %s"
        param = (gia,)
        result = execute_query(query,param)
    
    if result == None:
        return "Trong thực đơn không có món nào như vậy"
    response = "Đây là một số món ăn theo yêu cầu của bạn: "
    for item in result:
        response = response + item['Name'] + ", "
    return response

    
def queryExecution(user_input, conversation_id=None, customerID=None):
    """
    Hàm xử lý hội thoại chatbot và trả về toàn bộ chi tiết hội thoại:
    - Lưu thông tin vào bảng ChatbotConversation và ChatbotConversationDetail.
    - Trả về đoạn hội thoại với các thông tin chi tiết.
    """
    # Tạo ConversationID mới nếu chưa có
    if conversation_id is None:
        conversation_id = str(uuid.uuid4())  # Tạo mới ConversationID
        is_helpful = random.choice([True, False])  # Lấy ngẫu nhiên giá trị IsHelpful
        topic, entities, miss_info = tnm(model_path, user_input)
        
        # Thêm cuộc trò chuyện mới vào bảng ChatbotConversation
        insert_conversation_query = """
            INSERT INTO ChatbotConversation (ConversationID, CustomerID, Topic, IsHelpful)
            VALUES (%s, %s, %s, %s);
        """
        params = (conversation_id, customerID, topic, is_helpful)
        execute_query(insert_conversation_query, params)

    # Hàm cục bộ để ghi thông điệp vào bảng ChatbotConversationDetail
    conversation_detail_id = str(uuid.uuid4())
    timestamp = datetime.now()
    def update_conversation_table(message, sender):
            insert_detail_query = """
            INSERT INTO ChatbotConversationDetail (ConversationDetailID, ConversationID, Sender, Message, Timestamp)
            VALUES (%s, %s, %s, %s, %s);
        """
            params = (conversation_detail_id, conversation_id, sender, message, timestamp)
            execute_query(insert_detail_query, params)
       
            

    # Ghi câu hỏi của người dùng vào bảng
    #update_conversation_table(user_input, sender="1")  # "1" là người dùng

    # Xử lý và phân loại chủ đề
    topic, entities, miss_info = tnm(model_path, user_input)

    # Xử lý dựa trên chủ đề
    if topic == "đặt bàn":
        result, error_message = xu_ly_hoi_thoai_dat_ban(entities, miss_info)
        if error_message:
            #update_conversation_table(error_message, sender="0")  # "0" là chatbot
            response = error_message

        # Xử lý yêu cầu đặt bàn nếu thông tin đầy đủ
        elif result:
            so_ban, gio, ngay, thang= result
            response = xu_ly_yeu_cau_dat_ban(so_ban, gio, ngay, thang)

    elif topic == "gọi món":
        result, error_message = xu_ly_hoi_thoai_goi_mon(entities)
        if error_message:
            #update_conversation_table(error_message, sender="0")
            response = error_message
        elif result:
        # Xử lý yêu cầu gọi món
            mon_an = result["mon_an"]
            so_luong = result["so_luong"]
            response = xu_ly_yeu_cau_goi_mon(mon_an, so_luong)
    elif topic == "gợi ý món ăn":
        result,error_message = xu_ly_hoi_thoai_goi_y(entities)
        if error_message:
            response = error_message
        elif result:
            the_loai = result.get('the_loai', [])
            gia = result.get('gia', [])
            if the_loai:
                the_loai_value = the_loai[0]  # Lấy phần tử đầu tiên nếu có
            else:
                the_loai_value = None  # Hoặc một giá trị mặc định khác

            if gia:
                gia_value = gia[0]  # Lấy phần tử đầu tiên nếu có
            else:
                gia_value = None
            response = xu_ly_yeu_cau_goi_y(the_loai_value,gia_value)
    else:
        response = "Hiện tại tôi chưa hỗ trợ chủ đề này. Vui lòng thử lại." 

    #print(response)

    # Ghi phản hồi của chatbot vào bảng
    update_conversation_table(response, sender="0")

    return {
        'ConversationDetailID': conversation_detail_id,
        'ConversationID': conversation_id,
        'Sender': 0,
        'Message': response,
        'Timestamp': timestamp
    }


def main():
     # Giả sử có thông tin gọi món từ người dùng
    user_input = "gọi 1 bún cá, 1 bánh ướt"
    #user_input = "gợi ý món ăn tráng miệng" #giá dưới 35000 đồng"
    topic, entities, miss_info = tnm(model_path,user_input)
    result, err_mess = xu_ly_hoi_thoai_goi_y(entities)
    the_loai = result.get('the_loai', [])
    gia = result.get('gia', [])
    if the_loai:
        the_loai_value = the_loai[0]  # Lấy phần tử đầu tiên nếu có
    else:
        the_loai_value = None  # Hoặc một giá trị mặc định khác

    if gia:
        gia_value = gia[0]  # Lấy phần tử đầu tiên nếu có
    else:
        gia_value = None
    print(queryExecution(user_input))

if __name__ == "__main__":
    main()
