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

def generate_sample():

    # choose a random order
    xform_order = random.choice(xform_orders)

    # generate three angles
    # angles = 80.0*(np.random.random(size=3)*2.0 - 1.0)        
    angles = [0,0,0]
    for n in range(3):
        angles[n] = np.random.random() * 10.0 + 0.0
        if random.choice([True,False]):
            angles[n] *= -1.0

    # compose to create rotate matrix
    matrix = R.from_euler(xform_order, angles, degrees=True)

    # actual vector is concatenation of flattened matrix + angles
    x = np.concatenate((matrix.as_dcm().flatten(), np.array([0.08*(ang+random.random()*0.5) for ang in angles])))
    return (x, xform_orders.index(xform_order))

def generate_samples(n):
    x, y = list(zip(*[generate_sample() for n in range(n)]))
    return np.array(x), keras.utils.to_categorical(y)

x_train, y_train = generate_samples(10000)
x_test, y_test = generate_samples(1000)

model = Sequential()
model.add(Dense(32, activation='relu', input_dim=12))
# model.add(Dropout(0.01))
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