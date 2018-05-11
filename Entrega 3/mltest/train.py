import tflearn
from tflearn.layers.conv import conv_2d, max_pool_2d
from tflearn.layers.core import input_data, fully_connected, dropout
from tflearn.layers.estimator import regression
import tensorflow as tf
import numpy as np
import cv2

imgSize = 64
imgFilterSize = 8
#
train = np.load("train-set.npy")
train_data = np.array([i[0] for i in train]).reshape(-1, imgSize, imgSize, 1)
train_labels = [i[1] for i in train]

test = np.load("test-set.npy")
test_data = np.array([i[0] for i in test]).reshape(-1, imgSize, imgSize, 1)
test_labels = [i[1] for i in test]

#test_car = cv2.imread("../assets/TEST1.jpg", 0)
#test_car = cv2.resize(test_car, (imgSize, imgSize))
#test_car = np.array(test_car).reshape(-1, imgSize, imgSize, 1)

def cnn():
    tf.reset_default_graph()

    convnet = input_data([None, imgSize, imgSize, 1], name="input")

    # First hidden layer
    convnet = conv_2d(convnet, 32, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Second hidden layer
    convnet = conv_2d(convnet, 64, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Third hidden layer
    convnet = conv_2d(convnet, 128, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Fifth hidden layer
    convnet = conv_2d(convnet, 64, imgFilterSize, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Sixth hidden layer
#    convnet = conv_2d(convnet, 32, imgFilterSize, activation="relu")
#    convnet = max_pool_2d(convnet, 5)

    # First fully connected layer
    convnet = fully_connected(convnet, 1024, activation="relu")

    # Second fully connected layer
    convnet = fully_connected(convnet, 4, activation="softmax")
    convnet = regression(convnet, optimizer="adam", loss="categorical_crossentropy", name="targets")

    model = tflearn.DNN(convnet)
    #model.load('./mlgame.tflearn')
    #print(model.predict(test_car))
    #print(test_labels[212])
    model.fit({"input": train_data}, {"targets": train_labels}, n_epoch=10,
              validation_set=({"input": test_data}, {"targets": test_labels}),
              snapshot_epoch=True, show_metric=True)

    model.save('./mlgame.tflearn')

cnn()
