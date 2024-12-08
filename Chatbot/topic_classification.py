import json
import spacy
from spacy.training import Example
from tqdm import tqdm
import random
from database_util import execute_query

def load_training_data(data_path):
    """
    Tải dữ liệu huấn luyện từ file JSON.
    """
    with open(data_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    training_data = []
    topic_mapping = {}  # Lưu topic để sử dụng sau
    for record in data:
        user_query = record["user_query"].lower()
        
        # Tạo entities từ dữ liệu
        entities = [(ent["start"], ent["end"], ent["label"]) for ent in record["entity_detected"]]
        
        # Lưu topic riêng biệt để không ảnh hưởng đến dữ liệu huấn luyện
        topic_mapping[user_query] = record["topic"]
        
        # Tạo dữ liệu huấn luyện
        training_data.append((user_query, {"entities": entities}))

    return training_data, topic_mapping

def train_ner_model(data_path, model_output_dir, iterations=5):
    """
    Huấn luyện mô hình NER từ dữ liệu JSON.
    """
    # Tải dữ liệu huấn luyện và topic mapping
    training_data, _ = load_training_data(data_path)

    # Tạo mô hình trống SpaCy (ngôn ngữ tiếng Việt)
    nlp = spacy.blank("vi")  

    # Thêm thành phần NER vào pipeline
    if "ner" not in nlp.pipe_names:
        ner = nlp.add_pipe("ner", last=True)
    else:
        ner = nlp.get_pipe("ner")

    # Thêm nhãn thực thể vào NER
    for _, annotations in training_data:
        for ent in annotations["entities"]:
            ner.add_label(ent[2])

    # Huấn luyện mô hình
    optimizer = nlp.begin_training()
    print("Đang huấn luyện mô hình NER...")
    for i in range(iterations):
        random.shuffle(training_data)
        losses = {}
        for text, annotations in tqdm(training_data, desc=f"Iteration {i + 1}/{iterations}"):
            example = Example.from_dict(nlp.make_doc(text), annotations)
            nlp.update([example], drop=0.5, losses=losses)
        print(f"Iteration {i + 1}: Losses {losses}")

    # Lưu mô hình đã huấn luyện
    nlp.to_disk(model_output_dir)
    print(f"Mô hình NER đã được lưu tại: {model_output_dir}")

def predict_topic(entities_detected):
    """
    Kiểm tra sự xuất hiện của các thực thể để xác định topic.
    Dựa vào các nhãn của thực thể đã nhận diện.
    """
    detected_labels = {ent["label"] for ent in entities_detected}

    # Đối với "Đặt bàn", cần ít nhất 2 nhãn trong nhóm {SỐ_BÀN, GIỜ, NGÀY, THÁNG, KHOẢNG_THỜI_GIAN}
    đặt_bàn_labels = {"SỐ_BÀN", "GIỜ", "NGÀY", "THÁNG"}
    #nếu có ít nhất 2 nhãn trùng khớp
    if len(detected_labels & đặt_bàn_labels) >= 1:
        return "đặt bàn"
    
    # Đối với "Gọi món", cần có SỐ_LƯỢNG và MÓN_ĂN
    if "SỐ_LƯỢNG" in detected_labels and "MÓN_ĂN" in detected_labels:
        return "gọi món"
    
    # Đối với "Trạng thái order", cần có MÓN_ĂN
    if "MÓN_ĂN" in detected_labels:
        # Kiểm tra xem món ăn có tồn tại trong cơ sở dữ liệu không
        if not check_menu_exists(entities_detected):
            return "Unknown"  # Trả về unknown nếu món ăn không tồn tại
        return "trạng thái order"
    
    return "Unknown"

def check_menu_exists(entities_detected):
    """
    Kiểm tra xem món ăn có tồn tại trong cơ sở dữ liệu không.
    """
    for ent in entities_detected:
        if ent["label"] == "MÓN_ĂN":
            # Thực hiện truy vấn để kiểm tra món ăn
            query = "SELECT COUNT(*) FROM MenuItem WHERE Name = %s"
            result = execute_query(query, (ent["text"],))
            if result and result[0]['COUNT(*)'] > 0:
                return True
    return False

def test_ner_model(model_path, test_sentence):
    """
    Kiểm tra mô hình NER đã huấn luyện và in kết quả theo định dạng yêu cầu.
    """
    nlp = spacy.load(model_path)
    doc = nlp(test_sentence)

    # Lấy các thực thể đã phát hiện
    entities_detected = []
    for ent in doc.ents:
        entities_detected.append({"text": ent.text, "label": ent.label_})

    # Dự đoán topic từ các nhãn thực thể đã nhận diện
    predicted_topic = predict_topic(entities_detected)

    # Cập nhật all_entity_labels với thông tin về các thực thể theo từng topic
    all_entity_labels = {
    "đặt bàn": {"SỐ_BÀN", "GIỜ", "NGÀY", "THÁNG"},
    "gọi món": {"SỐ_LƯỢNG", "MÓN_ĂN"},
    "trạng thái order": {"MÓN_ĂN"}
}

    # Lấy nhãn của các thực thể đã phát hiện
    detected_labels = {ent["label"] for ent in entities_detected}

    # Tìm các thực thể thiếu
    missing_info = []
    if predicted_topic == "đặt bàn":
    # Lấy nhãn cần thiết cho "đặt bàn"
        required_labels = all_entity_labels["đặt bàn"]
    # Kiểm tra xem mỗi nhãn có bị thiếu không
        for label in required_labels:
            if label not in detected_labels:
                missing_info.append(label)  # Các nhãn thiếu là sự khác biệt giữa yêu cầu và nhãn đã phát hiện
        

    # In kết quả theo định dạng yêu cầu
    return predicted_topic, entities_detected, missing_info
# Hàm in ra kết quả test:
def print_model_results(user_input):
    predicted_topic, entities_detected, missing_info = test_ner_model('./ner_model_sample',user_input)#load_training_data("ner_training_data.json")[1])
    print(f"Test Sentence: {user_input}")
    print(f"Predicted Topic: {predicted_topic}")
    print("Entities Detected:")
    for ent in entities_detected:
        print(f"  - Text: {ent['text']}, Label: {ent['label']}")
    print(f"Missing Info: {missing_info}")

if __name__ == "__main__":
    # Đường dẫn tới file dữ liệu và thư mục lưu mô hình
    data_file = "training_data.json"  # File JSON chứa dữ liệu huấn luyện
    output_dir = "./ner_model_sample"

    # Huấn luyện mô hình
    training_data, topic_mapping = load_training_data(data_file)  # Lưu topic_mapping để kiểm tra
    #train_ner_model(data_file, output_dir)
    print("model created")

    # Kiểm tra mô hình đã huấn luyện
    model_path = "./ner_model_sample"  # Đường dẫn tới mô hình đã huấn luyện
    #test_sentence = "Món gỏi cuốn của tôi đã xong chưa"
    test_sentence = "hello"
    print(test_ner_model(model_path,test_sentence))
    #while(test_sentence != "quit"):
        #print(test_ner_model(model_path, test_sentence))
        #print_model_results(test_sentence)
        #print(test_ner_model(model_path,test_sentence))
        #load_training_data(test_sentence)
        #test_sentence = input("Nhập câu hỏi muốn test: ")
    #test_ner_model(model_path, test_sentence, topic_mapping)'''
