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
all_rotation_orders = [''.join(list(tuple)) for tuple in list(permutations({'x','y','z'}))]

def standardize(x):
    x -= np.mean(x)
    x /= np.std(x)

def random_vector(n, mag):
    return 2.0 * mag * (0.5-np.random.random(size=n))

def generate_sample_from_model():
    """
    """
    # choose a random order
    rotation_order = random.choice(all_rotation_orders)

    # generate three angles
    angles = random_vector(3, 10.0)

    # reject if any absolute angle < 0.1 degree
    if (min(abs(angles)) < 0.1):
        return generate_sample_from_model()

    # compose to create rotate matrix
    matrix = R.from_euler(rotation_order, angles, degrees=True)

    # actual vector is concatenation of flattened matrix + angles
    error_angles = angles + random_vector(3, 0.1)
    x = np.concatenate((matrix.as_dcm().flatten(), np.array(error_angles)))
    standardize(x)
    return (x, rotation_order)

def generate_samples(n):
    xs, rotation_orders = list(zip(*[generate_sample_from_model() for n in range(n)]))
    ys = [all_rotation_orders.index(rotation_order) for rotation_order in rotation_orders]
    return np.array(xs), keras.utils.to_categorical(ys)

x_train, y_train = generate_samples(10000)
x_test, y_test = generate_samples(1000)

model = Sequential()
model.add(Dense(32, activation='relu', input_dim=x_train.shape[1]))
model.add(Dense(16, activation='relu'))
model.add(Dense(16, activation='relu'))
model.add(Dense(6, activation='softmax'))
model.summary()

adam = Adam(lr=0.001, decay=1e-8)
model.compile(loss='categorical_crossentropy', optimizer=adam, metrics=['accuracy']) 

model.fit(x_train, y_train, epochs=500, batch_size=256)
score = model.evaluate(x_test, y_test, batch_size=256)
print(score)