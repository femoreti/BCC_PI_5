import tflearn
from tflearn.layers.conv import conv_2d, max_pool_2d
from tflearn.layers.core import input_data, fully_connected, dropout
from tflearn.layers.estimator import regression
import tensorflow as tf
import numpy as np
import cv2

imgSize = 64
imgFilterSize = 5

train = np.load("./data/train-set.npy", allow_pickle=True)
train_data = np.array([i[0] for i in train]).reshape(-1, imgSize, imgSize, 1)
train_labels = [i[1] for i in train]

test = np.load("./data/test-set.npy", allow_pickle=True)
test_data = np.array([i[0] for i in test]).reshape(-1, imgSize, imgSize, 1)
test_labels = [i[1] for i in test]

def GetModel():
    tf.reset_default_graph()

    convnet = input_data([None, imgSize, imgSize, 1], name="input")

    # First hidden layer
    convnet = conv_2d(convnet, 32, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 8)

    # Second hidden layer
    convnet = conv_2d(convnet, 64, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 8)

    # Third hidden layer
    convnet = conv_2d(convnet, 128, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 8)

    # Fourth hidden layer
    convnet = conv_2d(convnet, 64, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 8)

    # First fully connected layer
    convnet = fully_connected(convnet, 1024, activation="relu")

    # Second fully connected layer
    convnet = fully_connected(convnet, 4, activation="softmax")
    convnet = regression(convnet, optimizer="adam", loss="categorical_crossentropy", name="targets")

    return tflearn.DNN(convnet)

def TrainModel():
    model = GetModel()

    model.fit({"input": train_data}, {"targets": train_labels}, n_epoch=10,
              validation_set=({"input": test_data}, {"targets": test_labels}),
              snapshot_epoch=True, show_metric=True)

    model.save('./saved-models/mlgame.tflearn')

def PredictInput(imgPath):
    model = GetModel()
    model.load('./saved-models/mlgame.tflearn')
    img = cv2.imread(imgPath, 0)
    img = cv2.resize(img, (imgSize, imgSize))
    return model.predict(np.array(img).reshape(-1, 64, 64, 1))

def	GetPrediction(imgPath):
    result = PredictInput(imgPath)
    highIndex = -1
    for x in range(0,4):
        if(highIndex == -1):
            highIndex = 0
        if(result[0][x] > result[0][highIndex]):
            highIndex = x

    obj = ""
    if(highIndex == 0):
        obj = "Carro"
    elif(highIndex == 1):
        obj = "Moto"
    elif(highIndex == 2):
        obj = "Barco"
    elif(highIndex == 3):
        obj = "Avião"

    print(result[0])
    print("Isso é um:",obj,"com %.2f" % (result[0][highIndex] * 100),"% de certeza.")

    return highIndex

