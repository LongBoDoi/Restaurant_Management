from flask import Flask, request, jsonify, session
import logging
from flask_cors import CORS
from server_executor import queryExecution

app = Flask(__name__)
CORS(app, origins=["https://your-frontend-url.com"])  # Restrict CORS
app.secret_key = '123456'  # Replace with a more secure key

# Set up logging
logging.basicConfig(level=logging.ERROR)

@app.route('/GetNewChatbotResponse', methods=['GET'])
def getNewChatbotResponse():
    user_input = request.args.get('message', '')
    conversation_id = request.args.get('conversationID', '')
    
    if not user_input:
        return jsonify({'error': 'Missing required parameters'}), 400
    
    try:
        result = queryExecution(user_input, conversation_id)
        print(result)
        return jsonify({'Success': True, 'Data': result})
    except Exception as e:
        app.logger.error(f"Error: {str(e)}")
        return jsonify({'Success': False, 'ErrorMsg': str(e)}), 500

if __name__ == '__main__':
    app.run(port=9000)