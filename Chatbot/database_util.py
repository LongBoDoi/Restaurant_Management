import mysql.connector
from mysql.connector import Error

def connect_to_database():
    """
    Kết nối đến cơ sở dữ liệu MySQL và trả về đối tượng kết nối.
    """
    try:
        connection = mysql.connector.connect(
            host='localhost',        # Thay thế bằng host của bạn
            database='restaurant_management',  # Thay thế bằng tên cơ sở dữ liệu của bạn
            user='root',        # Thay thế bằng tên người dùng của bạn
            password='123456'   # Thay thế bằng mật khẩu của bạn
        )
        if connection.is_connected():
            return connection
    except Error as e:
        print(f"Lỗi kết nối cơ sở dữ liệu: {e}")
        return None

# Hàm thực thi truy vấn SQL
def execute_query(query, params=None):
    """
    Thực thi truy vấn SQL và trả về kết quả.
    """
    connection = connect_to_database()
    if connection is None:
        return None

    cursor = connection.cursor(dictionary=True)
    try:
        cursor.execute(query, params)

        # Nếu là SELECT, trả về kết quả
        if query.strip().lower().startswith('select'):
            return cursor.fetchall()

        # Nếu là INSERT, UPDATE, DELETE, chỉ cần commit và không cần trả về kết quả
        connection.commit()
        return None

    except Error as e:
        print(f"Lỗi khi thực thi truy vấn: {e}")
        connection.rollback()  # Quay lại nếu có lỗi
        return None
    finally:
        cursor.close()
        connection.close()