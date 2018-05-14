import os
from flask import Flask
from flask import request

from src.train import PredictInput

app = Flask(__name__)

@app.route("/")
def health_check():
    return "STATUS: Server running\nSERVICES: OK"

@app.route("/predict")
def predict_input():
    path = request.args.get("q")
    if not os.path.exists(path):
        return "404"

    test = PredictInput(path)
    return test

if __name__ == "__main__":
    app.run()

