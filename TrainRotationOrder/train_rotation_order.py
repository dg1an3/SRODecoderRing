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
from keras import backend as K
print(K.backend())

# ## Model
all_rotation_orders = [''.join(axes) for axes in permutations({'x','y','z'})]
print(all_rotation_orders)
def random_vector(n, mag):
	return 2.0 * mag * (0.5-np.random.random(size=n))

print(random_vector(10,5.0))

def generate_sample_from_model():
	"""
	define a function to generate a single sample from the model.
	sample is a tuple of:
	* np.array with 9 matrix elements + 3 rotation angles
	* string corresponding to chosen rotation order (element of all_rotation_orders)
	"""
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

print(generate_sample_from_model())

def generate_samples(n, mean=None, std=None):
	"""
	define a function to generate multiple samples, suitable for training or testing.
	for number N, sample is a tuple:
	* np.array Nx12 of x vectors (standardized zero mean)
	* np.array Nx6 one-hot vectors
	"""
	[xs, rotation_orders] = list(zip(*[generate_sample_from_model() for _ in range(n)]))
	xs = np.array(xs)
	if mean is None:
		mean = xs.mean(0)
	if std is None:
		std = xs.std(0)
	xs -= mean    # standardize along sample dimension
	xs /= std
	ys = [all_rotation_orders.index(rotation_order) for rotation_order in rotation_orders]
	return xs, mean, std, keras.utils.to_categorical(ys)

print(generate_samples(3))

# generate training and testing data from the model
x_train, mean, std, y_train = generate_samples(10000)
x_test, _, _, y_test = generate_samples(1000, mean, std)
print(x_train.shape, y_train.shape)

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
import json
import csv
with open('model_config.json', 'w') as modelfile:  
    json.dump(model.get_config(), modelfile)
def rows_of(array):
    if len(array.shape) > 1:
        for n in range(array.shape[0]):
            yield (n, array[n])
    else:
        yield (0, array)
        
with open('weights.csv', 'w', newline='') as csvfile:
    weightswriter = csv.writer(csvfile, delimiter=',', quoting=csv.QUOTE_MINIMAL)
    for layer in model.layers:
        for weight_variable in layer.weights:
            weight_values = K.batch_get_value(weight_variable)
            for (row_num, row_array) in rows_of(weight_values):
                if len(weight_values.shape) > 1:
                    row_count = weight_values.shape[0]
                    col_count = weight_values.shape[1]
                else:
                    row_count = 1
                    col_count = weight_values.shape[0]                    
                csv_row = [layer.name, weight_variable.name,
                           row_count, col_count, row_num] + list(row_array)
                weightswriter.writerow(csv_row)
# !jupyter nbconvert --to python train_rotation_order.ipynb --template=python_script.tpl

