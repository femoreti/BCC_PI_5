import tflearn
from tflearn.layers.conv import conv_2d, max_pool_2d
from tflearn.layers.core import input_data, fully_connected, dropout
from tflearn.layers.estimator import regression
import tensorflow as tf
import numpy as np

train = np.load("train-set.npy")
train_data = np.array([i[0] for i in train]).reshape(-1, 25, 25, 1)
train_labels = [i[1] for i in train]

test = np.load("test-set.npy")
test_data = np.array([i[0] for i in test]).reshape(-1, 25, 25, 1)
test_labels = [i[1] for i in test]

def cnn():
    tf.reset_default_graph()

    convnet = input_data([None, 25, 25, 1], name="input")

    # First hidden layer
    convnet = conv_2d(convnet, 32, 5, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Second hidden layer
    convnet = conv_2d(convnet, 64, 5, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Third hidden layer
    convnet = conv_2d(convnet, 128, 5, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Fifth hidden layer
    convnet = conv_2d(convnet, 64, 5, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # Sixth hidden layer
    convnet = conv_2d(convnet, 32, 5, activation="relu")
    convnet = max_pool_2d(convnet, 5)

    # First fully connected layer
    convnet = fully_connected(convnet, 1024, activation="relu")

    # Second fully connected layer
    convnet = fully_connected(convnet, 2, activation="softmax")
    convnet = regression(convnet, optimizer="adam", loss="categorical_crossentropy", name="targets")

    model = tflearn.DNN(convnet)
    model.fit({"input": train_data}, {"targets": train_labels}, n_epoch=10,
              validation_set=({"input": test_data}, {"targets": test_labels}),
              snapshot_epoch=True, show_metric=True)

cnn()