from flask import Flask, request, jsonify, session
import uuid
import random
from server_executor import queryExecution, execute_query
from topic_classification import test_ner_model as tnm
from flask_cors import CORS
from datetime import datetime

#data_path = 'training_data.json'
model_path = "ner_model_sample"
app = Flask(__name__)
CORS(app)  
# Thiết lập secret_key
app.secret_key = '123456'  # Hoặc sử dụng chuỗi ngẫu nhiên
# Ví dụ:
# import os
# app.secret_key = os.urandom(24)

# Tạo mới conversationID và lưu vào cơ sở dữ liệu
@app.route('/CreateNewConversation', methods=['POST'])
def create_new_conversation():
    conversation_id = str(uuid.uuid4())  # Tạo conversationID mới
    
    # Lấy thông tin topic, IsHelpful (ngẫu nhiên)
    #topic, _, _ = tnm(model_path, user_input)  # Giả sử bạn có cách xác định topic mặc định
    is_helpful = random.choice([True, False])
        
        # Lưu thông tin cuộc trò chuyện vào bảng ChatbotConversation
    insert_query = """
            INSERT INTO ChatbotConversation (ConversationID, Topic, IsHelpful)
            VALUES (%s, %s, %s)
    """
    execute_query(insert_query, (conversation_id, None, is_helpful))
    
    return jsonify({
        'Success': True,
        'Data': {
            'ConversationID': conversation_id,
            'Topic': None,
            'IsHelpful': False
        }
    })     
        
@app.route('/SendChatbotMessage', methods=['POST'])
def sendNewMessage():
    data = request.get_json()
    conversation_id = data.get('ConversationID')
    user_input = data.get('Message')

    new_guid = str(uuid.uuid4())
    data['ConversationDetailID']= new_guid

    insert_detail_query = """
            INSERT INTO ChatbotConversationDetail (ConversationDetailID, ConversationID, Sender, Message, Timestamp)
            VALUES (%s, %s, %s, %s, now());
        """
    params = (new_guid, conversation_id, "1", user_input)
    execute_query(insert_detail_query, params)

    return jsonify({
        'Success': True,
        'Data': data
    })

@app.route('/GetNewChatbotResponse', methods=['GET'])
def getNewChatbotResponse():
    # Nhận dữ liệu từ Vue.js
    user_input = request.args.get('user_input', '')
    conversation_id = request.args.get('conversation_id', '')
    
    if not user_input:
        return jsonify({'error': 'Missing required parameters'}), 400
    
    try:
        # Kiểm tra xem đã có conversation_id trong session chưa
        
        # conversation_id = session['conversation_id']  # Lấy conversation_id từ session

        # Gọi hàm queryExecution với conversation_id
        result = queryExecution(user_input, conversation_id)
        return jsonify({'Success': True, 'Data': result})
    
    except Exception as e:
        return jsonify({'Success': False, 'ErrorMsg': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port= 9000)
    
