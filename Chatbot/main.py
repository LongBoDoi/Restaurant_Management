import os
from langchain_community.utilities import SQLDatabase
from groq import Groq
from flask import Flask, request, jsonify, json
from flask_cors import CORS
import re
import requests
import pytz
from datetime import datetime, timedelta
import ast

# CONFIG
host_ip = "14.225.254.152"

api_key = os.getenv("GROQ_API_KEY", "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")
mysql_uri = f"mysql+pymysql://gudfud_chatbot:gudfudchatbot2712@{host_ip}:3306/restaurant_management"
backend_url = f"http://{host_ip}:7198/api"
model = "gemma2-9b-it"
token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VybmFtZSI6Imd1ZGZ1ZC1jaGF0Ym90IiwiZXhwIjo0OTAyNTYwMjkxLCJpc3MiOiJtbF9pc3N1ZXIiLCJhdWQiOiJtbF9hdWRpZW5jZSJ9.Ql1eDsJHN2dp193Y_uHAOlZCpMOgXEXsmtsgt1TQ05g"

client = Groq(
    api_key=api_key
)
db = SQLDatabase.from_uri(mysql_uri)
# --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

def getCurrentDate(timezone):
    timezone = int(timezone)
    timezone = f"-{timezone}" if timezone > 0 else f"+{timezone * -1}"

    local_tz = pytz.timezone(f"Etc/GMT{timezone}")

    now = datetime.now(local_tz).strftime("%Y-%m-%d %H:%M:%S")

    return now

def convertToUtc(timestamp, timezone):
    timezone = int(timezone)
    timezone = f"-{timezone}" if timezone > 0 else f"+{timezone * -1}"

    timestamp = datetime.strptime(timestamp, "%Y-%m-%d %H:%M:%S")
    local_tz = pytz.timezone(f"Etc/GMT{timezone}")

    local_date = local_tz.localize(timestamp)
    utc_date = local_date.astimezone(pytz.utc)

    return utc_date.strftime("%Y-%m-%dT%H:%M:%SZ")

def convertToLocalTime(timestamp, timezone):
    timezone = int(timezone)
    
    offset = timedelta(hours=timezone)

    timestamp = datetime.strptime(timestamp, "%Y-%m-%dT%H:%M:%SZ")
    return (timestamp + offset)

def getOpeningTimeData(timezone):
    day_mapping = {
        0: "Sunday",
        1: "Monday",
        2: "Tuesday",
        3: "Wednesday",
        4: "Thursday",
        5: "Friday",
        6: "Saturday",
        7: "Holiday"
    }

    opening_hours = db.run("select settingvalue from setting where settingkey = 'OpeningTimes'")
    parsed_result = ast.literal_eval(opening_hours)
    lst_opening_hours = json.loads(parsed_result[0][0])

    for hour_setting in lst_opening_hours:
        hour_setting['StartTime'] = convertToLocalTime(hour_setting['StartTime'], timezone).strftime("%H:%M")
        hour_setting['EndTime'] = convertToLocalTime(hour_setting['EndTime'], timezone).strftime("%H:%M")
        hour_setting['Days'] = [day_mapping.get(num, "Invalid") for num in hour_setting['Days']]

    print(lst_opening_hours)
    return lst_opening_hours

app = Flask(__name__)
CORS(app)

@app.route('/createNewConversation', methods=['POST'])
def createNewConversation():
    data = request.get_json()

    timezone = data['timezone']
    lst_opening_hours = getOpeningTimeData(timezone)

    settings = db.run("select * from setting where settingkey in ('RestaurantName', 'SocialMediaUrls', 'RestaurantAddress', 'RestaurantPhoneNumber')")
    
    return jsonify([
        {
            "role": "system",
            "content": f"You are a chatbot for a restaurant management system. You should always answer the user's quesions in Vietnamese using 'tôi' and 'bạn' only and use a friendly tone."
        },
        {
            "role": "system",
            "content": f"Here is the database schema of the menu items: {db.get_table_info(table_names=['menuitem'])}. Answer naturally and display each item in each line using <br> HTML tag, and embolden the item name with <b></b> tags."
        },
        {
            "role": "system",
            "content": f"Here is the database schema of the reservations: {db.get_table_info(table_names=['reservation'])}"
        },
        {
            "role": "system",
            "content": f"""Here is the data for the opening times of the restaurant: {lst_opening_hours}. Use it when the user asks for opening hours of the restaurant."""
        },
        {
            'role': 'system',
            'content': f"""This is the schema of some information of the restaurant: `{settings}`. Use it when the user wants to ask about the information of the restaurant like addres, phone number, social media links.
            For the social media, you should answer based on the Link properties from the json value of the record with the SettingKey 'SocialMediaUrls'
            If the data is empty, you should answer that the restaurant hasn't config the information yet."""
        },
        {
            "role": "system",
            "content": "If the user asks about the information of the restaurant, use the restaurant data to generate answer. If you do not have the data, say that you do not have the data required for the information."
        },
        {
            "role": "assistant",
            "content": "Xin chào. Tôi là chatbot của nhà hàng. Tôi có thể giúp gì cho bạn hôm nay?"
        }
    ])

@app.route('/getNewResponse', methods=['POST'])
def getNewResponse():
    data = request.get_json()

    conversation = data['conversation']
    user_msg = data['message']
    timezone = data['timezone']

    current_date = getCurrentDate(timezone)

    tables = db.run('SELECT * FROM `table` t where t.`Status` = 0')
    if not tables:
        table_prompt = "you answer that there is no available seat in the restaurant, please comeback another time."
    else:
        table_prompt = f"""here is the data of the available tables: {tables}. The user's question can be like 'Nhà hàng còn bàn trống không?' or 'còn chỗ ngồi không?', and you should answer like `There are currently n tables available for m people` with n is
        the number of record from the data, and m is the total number of SeatCount column. If the user asks if there is enough seats for n people, you should compute the total seats from those tables and check with n."""

    conversation.append({
        'role': 'system',
        'content': f"""Answer this question from the user in Vietnamese: `{user_msg}`. Do not use any other special characters in your response.
        If it is about available seats, {table_prompt}.
        If it is about booking reservations,
        ask for all the required information including name, phone number, number of people, date and time and special request (if they have any).
        When all the information is provided, confirm all the information once again with the user before proceed. If the user confirms, generate your answer returning the data in JSON using PascalCase for the properties in this format: `ReservationDataJson: the_json_data`, and your whole answer should be enclosed in grave accents and should contain no other text.
        Also the json data should only contains the properties for the information above, do not include the other properties from the reservation table schema.
        The properties should be enclosed in double quotes and must match with their name in the reservation table from the database.
        The current date of the user is {current_date}. Use it to get the data for the Reservation Date that the user wants and return it in this format: 'YYYY-MM-DD HH:mm:SS'.
        For example, your answer should looks like this: `ReservationDataJson: {{"CustomerName": name, "CustomerPhoneNumber": phone_number, etc..}}`.
        Remember that the Status column should always be 1."""
    })

    chat_completion = client.chat.completions.create(
        messages=conversation,
        model=model
    )

    new_response = chat_completion.choices[0].message.content
    reservation_data_match = re.search(r'ReservationDataJson:\s*(\{.*\})', new_response)
    if (reservation_data_match):
        print("Đã match dữ liệu đặt bàn")
        reservation_data_json = reservation_data_match.group(1)
        reservation_data = json.loads(reservation_data_json)

        reservation_data['ReservationDate'] = convertToUtc(reservation_data['ReservationDate'], timezone)

        response = requests.post(url=f"{backend_url}/Reservation/CreateCustomerReservation", json=reservation_data, headers={
            "Authorization": f"Bearer {token}"
        })

        if response.status_code == 200:
            response_data = response.json()

            if (response_data['Success']):
                return {
                    'role': 'assistant',
                    'content': "Đặt bàn thành công. Nhân viên của chúng tôi sẽ sớm liên hệ với bạn để xác nhận thông tin."
                }
            else:
                
                print(f"Response text: '{response.text}'")
                return {
                    'role': 'assistant',
                    'content': "Có lỗi xảy ra trong quá trình đặt bàn. Vui lòng thử lại sau."
                }
        else:
            print(f"Response text: '{response.text}'")
            return {
                'role': 'assistant',
                'content': "Có lỗi xảy ra trong quá trình đặt bàn. Vui lòng thử lại sau."
            }

    return {
        'role': 'assistant',
        'content': repr(new_response.rstrip())
    }

if __name__ == '__main__':
    app.run(port=5005, host="0.0.0.0")

