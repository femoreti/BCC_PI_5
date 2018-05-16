import json
import os
import train

def get_total_new_files():
	with open("./datasetReport.json") as f:
		data = json.load(f)
		total = 0
		for item in data["categories"]:
			total += item
		
		return total

def retrain_model():
	exec(open("./preprocess.py").read())
	train.TrainModel()	

def watch_dataset_changes():
	if get_total_new_files() > 10:
		retrain_model()
