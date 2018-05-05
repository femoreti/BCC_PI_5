import os
from providers.imageProvider import DownloadImage
import time
import json

fileName = os.getcwd() + "/datasetCategories.json"
jsonData = json.load(open(fileName))

for category in jsonData["categories"]:
    for i in range(0, category["total"]):
        fileName = category["name"] + "-" + str(len(os.listdir(os.getcwd() + "/assets/raw"))) + ".jpg"
        print("Starting download...")
        DownloadImage(category["name"], fileName)
        time.sleep(3)