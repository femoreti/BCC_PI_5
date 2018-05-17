import json
import os
import train
import preprocess

def get_total_new_files():
	with open("./datasetReport.json") as f:
		data = json.load(f)
		total = 0
		for item in data["categories"]:
			total += item
		
		return total

def retrain_model():
	preprocess.preprocess_data()
	#exec(open("./preprocess.py").read())
	train.TrainModel()	

def watch_dataset_changes():
	if get_total_new_files() > 10:
		reset_dataset_report()
		retrain_model()

def reset_dataset_report():
	with open("./datasetReport.json", "r+") as f:
		data = json.load(f)
		for i in range(0, 4):
			data["categories"][i] = 0
		f.seek(0)
		f.truncate()
		json.dump(data, f)


