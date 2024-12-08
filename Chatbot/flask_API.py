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


@app.route('/GetNewChatbotResponse', methods=['GET'])
def getNewChatbotResponse():
    # Nhận dữ liệu từ Vue.js
    user_input = request.args.get('message', '')
    conversation_id = request.args.get('conversationID', '')
    print(user_input)
    print(conversation_id)
    
    if not user_input:
        return jsonify({'error': 'Missing required parameters'}), 400
    
    try:
        # Kiểm tra xem đã có conversation_id trong session chưa
        
        # conversation_id = session['conversation_id']  # Lấy conversation_id từ session

        # Gọi hàm queryExecution với conversation_id
        result = queryExecution(user_input, conversation_id)
        print(result)
        return jsonify({'Success': True, 'Data': result})
    
    except Exception as e:
        return jsonify({'Success': False, 'ErrorMsg': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port= 9000)
    
