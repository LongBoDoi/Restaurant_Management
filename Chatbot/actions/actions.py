from typing import Any, Dict, List

from rasa_sdk import Action, Tracker
from rasa_sdk.executor import CollectingDispatcher
import requests  # if you're making an HTTP call
import logging
import dateparser
from rasa_sdk.forms import FormValidationAction
from datetime import datetime, timedelta
from rasa_sdk.events import SlotSet
from pytz import timezone as pytz_timezone, utc
import re

authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySUQiOiJkMDkyOWFlZi0xYTViLTQ0ZjYtOTYyZC0wMWY3ZjliYjJiMmIiLCJVc2VybmFtZSI6ImFkbWluIiwiVXNlclR5cGUiOiIwIiwiZXhwIjo0OTAyMjA1ODExLCJpc3MiOiJtbF9pc3N1ZXIiLCJhdWQiOiJtbF9hdWRpZW5jZSJ9.4PyLxWfy0_yU9HCtoNkgFnDTrV3JgQnDoNJjL_42zN8"
server_url = "http://host.docker.internal:7198/api"

logger = logging.getLogger(__name__)

def getTimezoneInt(tracker: Tracker, multiplier: int = -1) -> int:
    timezone = tracker.get_slot("timezone")
    if not timezone:
        # Default to GMT+7 if no timezone is set
        timezone = "7"

    timezone = int(timezone) * multiplier

    return int(timezone)

def getTimezone(tracker: Tracker, multiplier: int = -1) -> pytz_timezone:
    timezone = tracker.get_slot("timezone")
    if not timezone:
        # Default to GMT+7 if no timezone is set
        timezone = "7"

    timezone = int(timezone) * multiplier

    if timezone > 0:
        timezone = f"+{int(timezone)}"

    return pytz_timezone(f"Etc/GMT{timezone}")

def getTimezoneStr(tracker: Tracker, multiplier: int = -1) -> str:
    timezone = tracker.get_slot("timezone")
    if not timezone:
        # Default to GMT+7 if no timezone is set
        timezone = "7"

    timezone = int(timezone) * multiplier

    if timezone > 0:
        timezone = f"+{int(timezone)}"

    return f"Etc/GMT{timezone}"

class ActionGreetWithTimezone(Action):
    def name(self) -> str:
        return "action_greet_with_timezone"

    def run(self, dispatcher: CollectingDispatcher, tracker: Tracker, domain: dict):
        # Get the timezone entity from the user's message
        timezone = tracker.get_slot("timezone")
        
        # If timezone is provided, store it and greet the user
        if timezone:
            dispatcher.utter_message(text="Xin chào! Tôi là chatbot của nhà hàng. Tôi có thể hỗ trợ bạn đặt bàn hoặc cung cấp thông tin về thực đơn. Bạn cần giúp gì hôm nay?")
            return [SlotSet("timezone", timezone)]
        
        # Default greeting if no timezone is provided
        return []

class ValidateDatBanForm(FormValidationAction):
    def name(self) -> str:
        return "validate_dat_ban_form"

    def validate_ngay(self, slot_value: str, dispatcher, tracker: Tracker, domain: dict):
        """Validate and convert the 'ngay' slot to YYYY-MM-DD format."""
        if slot_value:
            tz = getTimezone(tracker)
            today = datetime.now(tz)
            logger.info(f"Ngày hiện tại: {today}")
            weekdays = {
                "thứ 2": 1,
                "thứ hai": 1,
                "thứ 3": 2,
                "thứ ba": 2,
                "thứ 4": 3,
                "thứ tư": 3,
                "thứ 5": 4,
                "thứ năm": 4,
                "thứ 6": 5,
                "thứ sáu": 5,
                "thứ 7": 6,
                "thứ bảy": 6,
                "chủ nhật": 0
            }

            if "tuần này" in slot_value:
                for day, weekday in weekdays.items():
                    if day in slot_value.lower():
                        current_weekday = today.weekday()

                        delta_days = weekday - current_weekday

                        return {"ngay" : (today + timedelta(days=delta_days)).strftime("%Y-%m-%d")}

            if "tuần sau" in slot_value:
                for day, weekday in weekdays.items():
                    if day in slot_value.lower():
                        current_weekday = today.weekday()
                        
                        delta_days = weekday - current_weekday + 7

                        return {"ngay": (today + timedelta(days=delta_days)).strftime("%Y-%m-%d")}
                    
            if ("mai" in slot_value and not "ngày" in slot_value):
                return {"ngay": (today + timedelta(days=1)).strftime("%Y-%m-%d")}

            # Parse the natural language date into a datetime object
            parsed_date = dateparser.parse(
                slot_value,
                languages=["vi"],
                settings={"TIMEZONE": getTimezoneStr(tracker, multiplier=1), "TO_TIMEZONE": getTimezoneStr(tracker, multiplier=1)}
            )  # Vietnamese language
            if parsed_date:
                # Convert to YYYY-MM-DD format
                logger.info(f"Ngày đã parse: {parsed_date}")
                formatted_date = parsed_date.strftime("%Y-%m-%d")
                return {"ngay": formatted_date}
            else:
                logger.error(f"dateparser không thể phân tích ngày: {slot_value}")

                dispatcher.utter_message(text="Xin lỗi, tôi không hiểu ngày bạn cung cấp.")
                return {"ngay": None}
        
        dispatcher.utter_message(text="Xin lỗi, tôi không hiểu ngày bạn cung cấp.")    
        return {"ngay": None}
    
    def validate_thoi_gian(self, slot_value: str, dispatcher, tracker: Tracker, domain: dict):
        selected_date = tracker.get_slot("ngay")
        if not selected_date:
            tz = getTimezone(tracker)
            today = datetime.now(tz)
            selected_date = today.strftime("%Y-%m-%d")

        # Preprocess the text to make it more parseable by dateparser
        time_pattern = r"(?P<hour>\d{1,2})(?:[:h]|\s*giờ)(?P<minute>\d{1,2})?\s*(?P<period>sáng|chiều|tối|AM|PM|trưa)?"

        # Match the input against the pattern
        match = re.match(time_pattern, slot_value.lower().strip())
        if match:
            # Extract hour, minute, and period
            hour = int(match.group("hour"))
            minute = int(match.group("minute")) if match.group("minute") else 0
            period = match.group("period")

            # Normalize the period to AM/PM
            logger.info(f"Period: {period}")
            if period in ["sáng", "AM"]:
                period = "AM"
            elif period in ["chiều", "tối", "PM"]:
                period = "PM"
            elif period == "trưa":
                period = "PM" if hour >= 12 else "AM"

            # Convert to 24-hour format if needed
            if period == "PM" and hour < 12:
                hour += 12
            elif period == "AM" and hour == 12:
                hour = 0

            return {"thoi_gian": f"{selected_date} {hour:02d}:{minute:02d}:00"}
        
        dispatcher.utter_message(text="Xin lỗi, tôi không hiểu thời gian bạn cung cấp. Bạn có thể nói cụ thể hơn như '10 giờ sáng' hoặc '8 giờ tối' không?")
        return {"thoi_gian": None}
    
    def validate_so_nguoi(self, slot_value: str, dispatcher: CollectingDispatcher, tracker: Tracker, domain: dict):
        """Validate and convert the 'so_nguoi' slot."""
        # Try parsing as a digit
        if slot_value.isdigit():
            return {"so_nguoi": int(slot_value)}

        # Try parsing as a Vietnamese text-based number
        text_to_number = {
            "một": 1,
            "hai": 2,
            "ba": 3,
            "bốn": 4,
            "năm": 5,
            "sáu": 6,
            "bảy": 7,
            "tám": 8,
            "chín": 9,
            "mười": 10,
            "mười một": 11,
            "mười hai": 12,
            "mười ba": 13,
            "mười bốn": 14,
            "mười lăm": 15,
            "hai mươi": 20,
            "hai mươi mốt": 21,
            "ba mươi": 30,
            "bốn mươi": 40,
            "năm mươi": 50,
            "một trăm": 100,
        }

        # Normalize the input and check if it matches the mapping
        slot_value = slot_value.lower().strip()
        parsed_number = text_to_number.get(slot_value, None)
        if parsed_number:
            return {"so_nguoi": parsed_number}

        # If invalid, ask the user to re-enter
        dispatcher.utter_message(text="Số người không hợp lệ. Vui lòng nhập lại.")
        return {"so_nguoi": None}
    
    def validate_yeu_cau(self, slot_value: str, dispatcher: CollectingDispatcher, tracker: Tracker, domain: dict):
        """Validate the 'yeu_cau' slot."""
        # List of phrases indicating no special request
        no_request_phrases = [
            "không",
            "ko"
        ]

        # Normalize the input (convert to lowercase and strip whitespace)
        normalized_value = slot_value.lower().strip()

        # Check if the input matches any of the "no request" phrases
        if normalized_value in no_request_phrases:
            return {"yeu_cau": ""}  # Set the slot to an empty string

        # Otherwise, keep the user's input as the slot value
        return {"yeu_cau": slot_value}

class ActionSubmitBooking(Action):
    def name(self) -> str:
        return "action_submit_booking"

    def run(self, dispatcher: CollectingDispatcher,
            tracker: Tracker,
            domain: Dict[str, Any]) -> List[Dict[str, Any]]:

        # Get slot values
        so_nguoi = tracker.get_slot("so_nguoi")
        thoi_gian = tracker.get_slot("thoi_gian")
        ten = tracker.get_slot("ten")
        sdt = tracker.get_slot("so_dien_thoai")
        yeu_cau = tracker.get_slot("yeu_cau")

        # Parse ngay and thoi_gian into a datetime object
        
        thoi_gian = dateparser.parse(thoi_gian)
        logger.info(f"Parsed DateTime: {thoi_gian}")

        tz = getTimezone(tracker)
        local_datetime = tz.localize(thoi_gian)  # Localize to the user's timezone
        
        logger.info(f"Local DateTime: {local_datetime}")

        # Convert to UTC
        utc_datetime = local_datetime.astimezone(utc)
        logger.info(f"UTC DateTime: {utc_datetime}")

        # Example: Prepare data
        booking_data = {
            "GuestCount": so_nguoi,
            "ReservationDate": utc_datetime.strftime("%Y-%m-%dT%H:%M:%S.%fZ"),  # Format to ISO 8601
            "CustomerName": ten,
            "CustomerPhoneNumber": sdt,
            "Note": yeu_cau,
        }

        # Example: API call (replace with your real API endpoint)
        response = requests.post(
            f"{server_url}/Reservation/CreateCustomerReservation",
            json=booking_data,
            headers={
                "Authorization": f"Bearer {authToken}",
            },
            timeout=5  # seconds
        )

        logger.info(f"Response data: {response.json()}")
        if response.status_code == 200 and response.json().get("Success"):
            dispatcher.utter_message(text="Đặt bàn thành công! Nhân viên của chúng tôi sẽ liên hệ với bạn để xác nhận.")

            thoi_gian_hien_thi = local_datetime.strftime("%H:%M %d/%m/%Y")
            yeu_cau = f"- Yêu cầu: {yeu_cau}\n" if yeu_cau else ""
            dispatcher.utter_message(text="Thông tin đặt bàn của bạn:\n"
                                        f"- Tên: {ten}\n"
                                        f"- Số điện thoại: {sdt}\n"
                                        f"- Số người: {so_nguoi}\n"
                                        f"- Thời gian: {thoi_gian_hien_thi}\n"
                                        f"{yeu_cau}")
        else:
            dispatcher.utter_message(text="Xin lỗi, có lỗi xảy ra khi đặt bàn. Vui lòng thử lại sau.")
        # try:
            
        # except Exception as e:
        #     print(f"Error: {e}")
        #     dispatcher.utter_message(text="Xin lỗi, không thể kết nối tới hệ thống đặt bàn.")

        return []

class ActionFetchMenu(Action):
    def name(self) -> str:
        return "action_fetch_menu"

    def run(self, dispatcher: CollectingDispatcher, tracker: Tracker, domain: dict):
        # Example: Fetch menu from an API or database
        today = datetime.now(getTimezone(tracker))
        startToday = today.replace(hour=0, minute=0, second=0, microsecond=0)
        endToday = today.replace(hour=23, minute=59, second=59)
        logger.info(f"Start today UTC: {startToday}")
        logger.info(f"End today UTC: {endToday}")

        response = requests.get(f"{server_url}/Dashboard/GetPopularMenuItems",
                                headers={
                                    "Authorization": f"Bearer {authToken}",
                                },
                                params={
                                    "fromDate": startToday.strftime("%Y-%m-%dT%H:%M:%S.%fZ"),
                                    "toDate": endToday.strftime("%Y-%m-%dT%H:%M:%S.%fZ"),
                                    "timeFilter": 0,
                                    "timeZone": getTimezoneInt(tracker, multiplier=1),
                                })

        if response.status_code == 200 and response.json().get("Success"):
            menu_items = response.json().get("Data", [])
            if menu_items:
                # Format the menu items for display
                logger.info(f"Menu items: {menu_items}")
                menu_text = "\n".join([f"- {item['MenuItem']['Name']} ({int(item['MenuItem']['Price']):,} đ)" for item in menu_items])
                dispatcher.utter_message(text=f"Các món phổ biến hôm nay bao gồm:\n{menu_text}")
            else:
                suggest_item_response = requests.get(f"{server_url}/MenuItem/GetDataPaging",
                                                    headers={
                                                        "Authorization": f"Bearer {authToken}",
                                                    },
                                                    params={
                                                        "page": 1,
                                                        "itemsPerPage": 5,
                                                        "sort": "[{Direction: 'RANDOM'}]"
                                                    })
                if suggest_item_response.status_code == 200 and suggest_item_response.json().get("Success"):
                    suggest_items = suggest_item_response.json().get("Data").get("Data", [])
                    if suggest_items:
                        # Format the menu items for display
                        menu_text = "\n".join([f"- {item['Name']}: {int(item['MenuItem']['Price']):,}đ" for item in suggest_items])
                        dispatcher.utter_message(text=f"Hôm nay chúng tôi chưa nhận order, bạn có thể thử các món sau:\n{menu_text}")
                    else:
                        dispatcher.utter_message(text="Xin lỗi, hiện tại nhà hàng chưa thiết lập thực đơn.")

        return []

class ActionCheckAvailableSeats(Action):
    def name(self) -> str:
        return "action_check_available_seats"

    def run(self, dispatcher: CollectingDispatcher, tracker: Tracker, domain: dict):
        response = requests.get(f"{server_url}/Table/GetAll",
                                headers={
                                    "Authorization": f"Bearer {authToken}",
                                },
                                params={
                                    "filter": "[{Name: 'Status', Value: 0, Operator: '=='}]",
                                })

        if response.status_code == 200 and response.json().get("Success"):
            available_tables = response.json().get("Data", [])
            if len(available_tables) > 0:
                dispatcher.utter_message(text=f"Hiện tại nhà hàng có {len(available_tables)} bàn trống, đủ chỗ cho {sum(table['SeatCount'] for table in available_tables)} người.")
            else:
                dispatcher.utter_message(text="Tiếc quá, hiện tại nhà hàng không còn bàn nào trống.")
        else:
            dispatcher.utter_message(text="Xin lỗi, có lỗi xảy ra khi kiểm tra chỗ ngồi.")

        return []