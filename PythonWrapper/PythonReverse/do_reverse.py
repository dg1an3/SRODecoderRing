def do_reverse(request):
	assert(type(request) is list)
	return reversed(request)

if __name__ == '__main__':
	import sys
	# deserialize request
	in_file = open(sys.argv[1], 'r')
	request = in_file.readlines()
	in_file.close()

	response = do_reverse(request)

	# serialize response
	print ('writing result to ', sys.argv[2])
	out_file = open(sys.argv[2],'w')
	out_file.writelines(response)
	out_file.close()
