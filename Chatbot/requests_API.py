import requests

token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySUQiOiI1YzMxYWU4OS0zNzJjLTRmMmEtYWFlMy1mZTg3Yjc2NmI0ZDciLCJVc2VybmFtZSI6ImFkbWluIiwiVXNlclR5cGUiOiIwIiwiZXhwIjoyMDQ5MjAzMDQ5LCJpc3MiOiJtbF9pc3N1ZXIiLCJhdWQiOiJtbF9hdWRpZW5jZSJ9.hmn39GiYsIpA-p8lpKhNXZmNu0FBECnlKQdgOhk_XSE"
headers = { 'Authorization': f'Bearer {token}'}

def getMenuItems():
    response = requests.get('http://api.gudfud.online:7198/api/MenuItem/GetDataPaging', headers=headers, params={
        'page': 1,
        'itemsPerPage': -1
    })
    if response.status_code == 200: # Parse the JSON response 
        response_data = response.json()
        return response_data['Data']['Data']
    return []