import os

cwd = os.getcwd()

for file in os.listdir('Traces'):
	print file
	os.rename(cwd + '/Traces/' + file, cwd + '/Traces/' + file.replace('trace', '').replace('.log', ''))