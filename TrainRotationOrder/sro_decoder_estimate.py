import sys
import numpy as np
from keras.models import load_model

def estimate(input):
	model = load_model('sro_decoder_model/sro_decoder_mlp_model.h5')
	model.summary()
	return model.predict(input)

if __name__ == '__main__':
	input_strings = sys.argv[1].split('/')
	input_array = np.zeros((1,12))
	input_array = np.array([[float(val) for val in input_strings]])
	result_array = estimate(input_array)
	result_vals = [str(val) for val in result_array[0]]
	print('/'.join(result_vals))
