import random
import math
import numpy as np
from itertools import permutations
from scipy.spatial.transform import Rotation as R

import keras
from keras.models import Sequential
from keras.layers import Dense, Dropout, Activation, BatchNormalization
from keras.optimizers import SGD, Adam
from keras.preprocessing.image import ImageDataGenerator

# this is the complete list of all rotation orders
xform_orders = [''.join(list(tuple)) for tuple in list(permutations({'x','y','z'}))]

def standardize(x):
    x -= np.mean(x)
    x /= np.std(x)

def random_vector(n, mag):
    return 2.0 * mag * (0.5-np.random.random(size=n))

def generate_sample():

    # choose a random order
    xform_order = random.choice(xform_orders)

    # generate three angles
    angles = random_vector(3, 10.0)

    # reject if any absolute angle < 0.1 degree
    if (min(abs(angles)) < 0.01):
        return generate_sample()

    # compose to create rotate matrix
    matrix = R.from_euler(xform_order, angles, degrees=True)

    # actual vector is concatenation of flattened matrix + angles
    error_angles = angles + random_vector(3, 0.1)
    # experimenting with sin/cos as inputs
    sincos_angles = [scang for angle in error_angles for scang in [math.sin(angle), math.cos(angle)]] 
    x = np.concatenate((matrix.as_dcm().flatten(), np.array(error_angles)))
    standardize(x)
    return (x, xform_orders.index(xform_order))

def generate_samples(n):
    x, y = list(zip(*[generate_sample() for n in range(n)]))
    return np.array(x), keras.utils.to_categorical(y)

x_train, y_train = generate_samples(10000)
x_test, y_test = generate_samples(1000)

model = Sequential()
model.add(Dense(32, activation='relu', input_dim=x_train.shape[1]))
model.add(Dense(16, activation='relu'))
model.add(Dense(16, activation='relu'))
model.add(Dense(6, activation='softmax'))
model.summary()

sgd = SGD(lr=0.1, decay=1e-6, momentum=0.9)
adam = Adam(lr=0.001, decay=0.0)
model.compile(loss='categorical_crossentropy', optimizer=adam, metrics=['accuracy']) 

model.fit(x_train, y_train, epochs=500, batch_size=512)
score = model.evaluate(x_test, y_test, batch_size=512)
print(score)