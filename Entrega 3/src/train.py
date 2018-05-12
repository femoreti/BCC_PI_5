import tflearn
from tflearn.layers.conv import conv_2d, max_pool_2d
from tflearn.layers.core import input_data, fully_connected, dropout
from tflearn.layers.estimator import regression
import tensorflow as tf
import numpy as np
import cv2

imgSize = 64
imgFilterSize = 8

train = np.load("./data/train-set.npy")
train_data = np.array([i[0] for i in train]).reshape(-1, imgSize, imgSize, 1)
train_labels = [i[1] for i in train]

test = np.load("./data/test-set.npy")
test_data = np.array([i[0] for i in test]).reshape(-1, imgSize, imgSize, 1)
test_labels = [i[1] for i in test]

#test_car = cv2.imread("../assets/TEST1.jpg", 0)
#test_car = cv2.resize(test_car, (imgSize, imgSize))
#test_car = np.array(test_car).reshape(-1, imgSize, imgSize, 1)

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

    # Sixth hidden layer
    #convnet = conv_2d(convnet, 32, imgFilterSize, activation="relu")
    #convnet = max_pool_2d(convnet, 5)

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

def PredictInput(img):
    model = GetModel()
    model.load('./saved-models/mlgame.tflearn')
    return model.predict(img)

TrainModel()
