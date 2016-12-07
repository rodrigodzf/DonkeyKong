"""
freesound fetcher
"""
try:
	import pyext
except:
	print "ERROR: This script must be loaded by the PD/Max pyext external"

import os
import sys
import freesound
from ffmpy import FFmpeg

class ex1(pyext._class):
	"""Example of a simple class which receives messages and prints to the console"""
	def __init__(self):
		self.path = os.path.dirname(os.path.abspath(__file__))


	# print "Script arguments: ", sys.argv

	# number of inlets and outlets
	_inlets = 1
	_outlets = 1

	def convert_wav(self, wav):
		"""
		This converts sounds
		"""
		ff = FFmpeg( inputs={wav: None}, outputs={wav: '-ar 44100'})
		print ff.cmd
		ff.run()

	def query_1(self, *args):
		"""
		This fetches sounds
		"""
		if len(args) != 2:
			print "invalid number of arguments, need query and number of items"
			return

		keyword = args[0]
		num_items = args[1]
		print "Fetching {0} sounds with query {1!s}".format(num_items, keyword)
		print "Saving in {0}".format(self.path)

		client = freesound.FreesoundClient()
		client.set_token("55217501132ecb9ada5da26888c7f0eac8ce1f45", "token")

		results = client.text_search(query=keyword, fields="id,name,previews")
		i = 0
		for sound in results:
			if i < num_items:
				sound.retrieve_preview(self.path, sound.name)
				self.convert_wav(self.path + "/" + sound.name)
				print sound.name
				i += 1
			else:
				break

		self._outlet(1, 1)

	def delete_files_1(self):
		"""
		This deletes files
		"""
		print "Deleting tmp files"
		filelist = [f for f in os.listdir(self.path) if f.endswith(".mp3") or f.endswith(".wav")]
		for f in filelist:
			print self.path + "/" + f
			os.remove(self.path + "/" + f)
		self._outlet(1, 1)

	def set_path_1(self, *args):
		"""Set Path"""
		self.path = str(args[0])
		print self.path

def num(*args):   # variable argument list
	"""Return the number of arguments"""
	print args[0]
	print args[1]
	return len(args)
