import numpy as np
from random import shuffle
import cv2
import os


def get_image_label(filename):
    category = filename.split('_')[0]
    if category == "car":
        return np.array([1, 0, 0, 0])
    elif category == "motorbike":
        return np.array([0, 1, 0, 0])
    elif category == "boat":
        return np.array([0, 0, 1, 0])
    elif category == "plane":
        return np.array([0, 0, 0, 1])


def preprocess_data():
    img_paths = os.listdir('../assets')
    training_data = []
    for category in img_paths:
        for img in os.listdir("../assets/" + category):
            print("Processing", img)
            path = "../assets/" + category + "/" + img
            imgData = cv2.imread(path, 0)
            imgData = cv2.resize(imgData, (64, 64))
            training_data.append([np.array(imgData), get_image_label(img)])

    shuffle(training_data)
    mid = len(training_data) / 4
    np.save('./data/train-set.npy', training_data[int(mid):])
    np.save('./data/test-set.npy', training_data[:int(mid)])


preprocess_data()
