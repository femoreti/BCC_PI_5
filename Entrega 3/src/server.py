import os
from flask import Flask
from flask import request
import train
import json
import numpy as np
import live_training

app = Flask(__name__)

@app.route("/")
def health_check():
    return "STATUS: Server running\nSERVICES: OK"

@app.route("/predict")
def predict_input():
    imgName = request.args.get("q")
    path = "../predictions/"+ imgName
    if not os.path.exists(path):
        return "404"

    predictions = train.GetPrediction(path)
    return str(predictions)

@app.route("/update/<int:itemPath>")
def update_dataset(itemPath):
	with open("./datasetReport.json", "r+") as f:
		data = json.load(f)
		data['categories'][itemPath] += 1
		f.seek(0)
		f.truncate()
		json.dump(data, f)

	live_training.watch_dataset_changes()
	return "OK"
	
if __name__ == "__main__":
    app.run()

