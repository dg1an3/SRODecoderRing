# run: jupyter nbconvert --to python train_rotation_order.ipynb --template=python_script.tpl
#!/usr/bin/env python
# coding: utf-8

# ## Dependencies
# * standard libraries: random, math, itertools
# * numpy scipy
# * keras / tensorflow
import random
import math
from itertools import permutations
import numpy as np
from scipy.spatial.transform import Rotation as R
import keras
from keras.models import Sequential
from keras.layers import Dense, Dropout, Activation, BatchNormalization
from keras.optimizers import SGD, Adam
from keras.preprocessing.image import ImageDataGenerator

# ## Model
all_rotation_orders = [''.join(axes) for axes in permutations({'x','y','z'})]
all_rotation_orders
def random_vector(n, mag):
    return 2.0 * mag * (0.5-np.random.random(size=n))
random_vector(10,5.0)

# define a function to generate a single sample from the model.
# 
# sample is a tuple of:
# * np.array with 9 matrix elements + 3 rotation angles
# * string corresponding to chosen rotation order (element of all_rotation_orders)
def generate_sample_from_model():
    rotation_order = random.choice(all_rotation_orders)  # choose a rotation order
    angles = random_vector(3, 10.0)   # generate three random angles (in degrees)
    if (min(abs(angles)) < 0.1):     # reject any angles that are too small
        return generate_sample_from_model()
    matrix = R.from_euler(rotation_order, angles, degrees = True)  # generate matrix
    noise_angles = angles + random_vector(3, 0.1)    
    x = np.concatenate((matrix.as_dcm().flatten(), 
                        np.array(noise_angles)))
                        # np.array(np.cos(noise_angles)), 
                        # np.array(np.sin(noise_angles))))                         
    return (x, rotation_order)
generate_sample_from_model()

# define a function to generate multiple samples, suitable for training or testing.
# 
# for number N, sample is a tuple:
#  * np.array Nx12 of x vectors (standardized zero mean)
#  * np.array Nx6 one-hot vectors
def generate_samples(n):
    [xs, rotation_orders] = list(zip(*[generate_sample_from_model() for _ in range(n)]))
    xs = np.array(xs)
    xs -= xs.mean(0)    # standardize along sample dimension
    xs /= xs.std(0)
    ys = [all_rotation_orders.index(rotation_order) for rotation_order in rotation_orders]
    return xs, keras.utils.to_categorical(ys)
generate_samples(3)

# generate training and testing data from the model
x_train, y_train = generate_samples(10000)
x_test, y_test = generate_samples(1000)
x_train.shape, y_train.shape

# now create an MLP as the model
model = Sequential()
model.add(Dense(64, activation='relu', input_dim = x_train.shape[1]))
model.add(Dense(32, activation='relu'))
model.add(Dense(32, activation='relu'))
model.add(Dense(6, activation='softmax'))
model.summary()

# set up the optimizer and compile the model
adam = Adam(lr=0.001, decay=1e-8)
model.compile(loss='categorical_crossentropy', optimizer=adam, metrics=['accuracy'])

# now fit the data using the training samples
model.fit(x_train, y_train, epochs=200, batch_size=256)
score = model.evaluate(x_test, y_test, batch_size=256)
print(score)
model.save('rotation_order.h5')
import h5py
f = h5py.File('rotation_order.h5')
f
[(k, grp) for k in f.keys() for grp in f[k]]
type(f)
import pprint
f.visititems(lambda name, obj: pprint.pprint(obj))
# f['model_weights']['dense_1'].name

