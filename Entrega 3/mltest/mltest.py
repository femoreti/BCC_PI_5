import numpy as np
from random import shuffle
import cv2
import os


def get_image_label(filename):
    category = filename.split('-')[0]
    if category == "car":
        return np.array([1, 0])
    elif category == "bike":
        return np.array([0, 1])


def preprocess_data():
    images = os.listdir('../assets/raw')
    training_data = []
    for img in images:
        print("Processing", img)
        path = "../assets/raw/" + img
        imgData = cv2.imread(path, 0)
        imgData = cv2.resize(imgData, (25, 25))
        training_data.append([np.array(imgData), get_image_label(img)])

    shuffle(training_data)
    np.save('train-set.npy', training_data[:6])
    np.save('test-set.npy', training_data[6:])


preprocess_data()
