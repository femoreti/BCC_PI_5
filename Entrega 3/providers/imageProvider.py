import requests
import shutil
import os

def DownloadImage(query, fileName):
    url = "https://source.unsplash.com/200x200/?" + query
    response = requests.get(url, stream = True)
    if response.status_code == 200:
        filePath = os.getcwd() + "/assets/" + fileName
        print("Saving " + query + " image to: " + filePath)
        with open(filePath, "wb") as f:
            response.raw.decode_content = True
            shutil.copyfileobj(response.raw, f)