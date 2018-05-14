import os
from flask import Flask
from flask import request
import train
import numpy as np

app = Flask(__name__)

@app.route("/")
def health_check():
    return "STATUS: Server running\nSERVICES: OK"

@app.route("/predict")
def predict_input():
    imgName = request.args.get("q")
    path = "../assets/"+ imgName
    if not os.path.exists(path):
        return "404"

    predictions = train.PredictInput(path)
    return np.array_str(predictions)

if __name__ == "__main__":
    app.run()
