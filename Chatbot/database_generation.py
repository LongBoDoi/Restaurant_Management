import mysql.connector
from mysql.connector import Error
from faker import Faker
import uuid
import random
from datetime import datetime, timedelta
import string
import json

class DatabaseManager:
    def __init__(self):
        self.faker = Faker()

    def create_connection(self):
        """
        Establish a connection to the MySQL database with error handling
        """
        try:
            connection = mysql.connector.connect(
                host='localhost',
                database='chatbotdatabase',
                user='root',
                password='123456'
            )
            if connection.is_connected():
                print("Successfully connected to MySQL database")
                return connection
        except Error as e:
            print(f"Database connection error: {e}")
            return None

    def execute_query(self, query, values=None):
        """
        Execute a SQL query with robust error handling
        """
        connection = None
        try:
            connection = self.create_connection()
            if connection:
                cursor = connection.cursor()
                if values:
                    cursor.execute(query, values)
                else:
                    cursor.execute(query)
                connection.commit()
                return cursor
        except Error as e:
            print(f"Query execution error: {e}")
        finally:
            if connection and connection.is_connected():
                connection.close()

    def create_tables(self):
        """
        Create all necessary tables for the restaurant database
        """
        tables = {
    "Customer": """
        CREATE TABLE IF NOT EXISTS Customer (
            CustomerID CHAR(36) PRIMARY KEY, 
            CustomerName VARCHAR(255) NOT NULL,  
            Email VARCHAR(255) NOT NULL UNIQUE,  
            PhoneNumber VARCHAR(50) NOT NULL,  
            Preferences JSON,  
            LoyaltyPoint INT DEFAULT 0  
        )
    """,
    "Employee": """
        CREATE TABLE IF NOT EXISTS Employee (
            EmployeeID CHAR(36) PRIMARY KEY, 
            EmployeeName VARCHAR(255) NOT NULL, 
            Email VARCHAR(255) NOT NULL UNIQUE,  
            Role ENUM('Manager', 'Waiter', 'Cook', 'Cashier') NOT NULL,  
            Schedule JSON  
        )
    """,
    "Menu": """
        CREATE TABLE IF NOT EXISTS Menu (
            MenuItemID CHAR(36) PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            Description TEXT,
            Price DECIMAL(10, 2) NOT NULL,
            Category VARCHAR(50) NOT NULL,
            OutOfStock BOOL NOT NULL DEFAULT FALSE
        )
    """,
    "Inventory": """
        CREATE TABLE IF NOT EXISTS Inventory (
            InventoryID CHAR(36) PRIMARY KEY,  
            Name VARCHAR(255) NOT NULL,  
            Quantity INT NOT NULL,  
            Unit VARCHAR(50) NOT NULL,  
            ReorderLevel INT NOT NULL  
        )
    """,
    "Order": """
        CREATE TABLE IF NOT EXISTS `Order` (
            OrderID CHAR(36) PRIMARY KEY,
            CustomerID CHAR(36),
            CustomerName VARCHAR(255) NULL,
            TableNumber INT NOT NULL,
            OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
            TotalAmount DECIMAL(10, 2) NOT NULL,
            Status ENUM('Pending', 'Processing', 'Completed', 'Cancelled') NOT NULL,
            SpecialRequest TEXT,
            FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
        )
    """,
    "OrderDetail": """
        CREATE TABLE IF NOT EXISTS OrderDetail (
            OrderDetailID CHAR(36) PRIMARY KEY,
            OrderID CHAR(36),
            MenuItemID CHAR(36),
            Quantity INT NOT NULL,
            Price DECIMAL(10, 2) NOT NULL,
            Note TEXT,
            FOREIGN KEY (OrderID) REFERENCES `Order`(OrderID),
            FOREIGN KEY (MenuItemID) REFERENCES Menu(MenuItemID)
        )
    """,
    "Reservation": """
        CREATE TABLE IF NOT EXISTS Reservation (
            ReservationID CHAR(36) PRIMARY KEY,
            CustomerID CHAR(36),
            EmployeeID CHAR(36),
            ReservationDate DATETIME NOT NULL,
            ReservationInfo TEXT,
            Status ENUM('Pending', 'Confirmed', 'Cancelled') NOT NULL,
            GuestCount INT NOT NULL,
            Note TEXT,
            FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
            FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
        )
    """,
    "Feedback": """
        CREATE TABLE IF NOT EXISTS Feedback (
            FeedbackID CHAR(36) PRIMARY KEY,
            CustomerID CHAR(36),
            CustomerName VARCHAR(255) NOT NULL,
            OrderID CHAR(36),
            Content TEXT NOT NULL,
            Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
            FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
            FOREIGN KEY (OrderID) REFERENCES `Order`(OrderID)
        )
    """,
    "ChatbotConversation": """
        CREATE TABLE IF NOT EXISTS ChatbotConversation (
            ConversationID CHAR(36) PRIMARY KEY,
            CustomerID CHAR(36),
            Topic VARCHAR(255),
            IsHelpful BOOL,
            FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
        )
    """,
    "ChatbotConversationDetail": """
        CREATE TABLE IF NOT EXISTS ChatbotConversationDetail (
            ConversationDetailID CHAR(36) PRIMARY KEY,
            ConversationID CHAR(36),
            Sender INT NOT NULL,
            Message TEXT,
            Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (ConversationID) REFERENCES ChatbotConversation(ConversationID)
        )
    """,
    "Recommendation": """
        CREATE TABLE IF NOT EXISTS Recommendation (
            RecommendationID CHAR(36) PRIMARY KEY,
            CustomerID CHAR(36),
            MenuItemID CHAR(36),
            ConfidenceScore DECIMAL(5, 2) NOT NULL,
            FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
            FOREIGN KEY (MenuItemID) REFERENCES Menu(MenuItemID)
        )
    """,
    "DemandForecast": """
        CREATE TABLE IF NOT EXISTS DemandForecast (
            ForecastID CHAR(36) PRIMARY KEY,
            MenuItemID CHAR(36),
            ForecastDate DATETIME NOT NULL,
            PredictedQuantity INT NOT NULL,
            ConfidenceLevel DECIMAL(5, 2) NOT NULL,
            FOREIGN KEY (MenuItemID) REFERENCES Menu(MenuItemID)
        )
    """
}

        for table_name, ddl in tables.items():
            try:
                self.execute_query(ddl)
                print(f"Table '{table_name}' created or already exists.")
            except Error as e:
                print(f"Error creating table {table_name}: {e}")

    def generate_random_preferences(self):
        """Generate random customer preferences"""
        preferences = {
            "allergies": random.choice(["None", "Peanuts", "Dairy", "Gluten"]),
            "spicy_level": random.choice(["Mild", "Medium", "Hot", "Extra Hot"]),
            "diet": random.choice(["Vegan", "Vegetarian", "Non-Vegetarian"])
        }
        return json.dumps(preferences)

    def insert_customer(self):
        """Insert a random customer record"""
        query = """
            INSERT INTO Customer 
            (CustomerID, CustomerName, Email, PhoneNumber, Preferences, LoyaltyPoint)
            VALUES (%s, %s, %s, %s, %s, %s)
        """
        values = (
            str(uuid.uuid4()),
            self.faker.name(), 
            self.faker.email(), 
            self.faker.phone_number(), 
            self.generate_random_preferences(), 
            random.randint(0, 1000)
        )
        self.execute_query(query, values)
        print("Customer inserted successfully")

    def insert_employee(self):
        """Insert a random employee record"""
        def random_schedule():
            days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']
            return json.dumps({
                "day": random.choice(days),
                "hours": f"{random.randint(8, 10)}am - {random.randint(5, 8)}pm"
            })

        query = """
            INSERT INTO Employee 
            (EmployeeID, EmployeeName, Email, Role, Schedule)
            VALUES (%s, %s, %s, %s, %s)
        """
        roles = ['Manager', 'Waiter', 'Cook', 'Cashier']
        values = (
            str(uuid.uuid4()),
            self.faker.name(), 
            self.faker.email(), 
            random.choice(roles), 
            random_schedule()
        )
        self.execute_query(query, values)
        print("Employee inserted successfully")

    def insert_menu_items(self):
        """Insert menu items with predefined Vietnamese cuisine"""
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
    {"name": "Khoai tây chiên", "description": "Món khoai tây chiên giòn, ăn kèm với sốt ketchup hoặc mayonnaise.", "category": "Side Dish"},
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

        query = """
            INSERT INTO Menu 
            (MenuItemID, Name, Description, Price, Category, OutOfStock)
            VALUES (%s, %s, %s, %s, %s, %s)
        """
        for item in menu_items:
            values = (
                str(uuid.uuid4()),
                item["name"], 
                item["description"], 
                round(random.uniform(5, 50), 2),
                item["category"], 
                False
            )
            self.execute_query(query, values)
        print("Menu items inserted successfully")

    def insert_inventory_items(self, num_items=5):
        """Insert random inventory items"""
        units = ['kg', 'g', 'litre', 'ml', 'piece']
        query = """
            INSERT INTO Inventory 
            (InventoryID, Name, Quantity, Unit, ReorderLevel)
            VALUES (%s, %s, %s, %s, %s)
        """
        for _ in range(num_items):
            values = (
                str(uuid.uuid4()),
                self.faker.word(), 
                random.randint(0, 100), 
                random.choice(units), 
                random.randint(0, 20)
            )
            self.execute_query(query, values)
        print("Inventory items inserted successfully")

def main():
    db_manager = DatabaseManager()
    
    # Create tables
    db_manager.create_tables()
    
    # Insert sample data
    for _ in range(5):  # Insert 5 of each type
        db_manager.insert_customer()
        db_manager.insert_employee()
    
    db_manager.insert_menu_items()
    db_manager.insert_inventory_items()

if __name__ == "__main__":
    main()