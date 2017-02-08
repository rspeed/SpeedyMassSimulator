#!/usr/bin/env python

import sys, os, io
import argparse
import logging

try:
	from configparser import RawConfigParser
except ImportError:
	from ConfigParser import RawConfigParser


# build-configs
# Generates part cfg files based on a template.

LOG = logging.getLogger(__name__)
LOG.addHandler(logging.StreamHandler())
LOG.setLevel(logging.INFO)


class BuildPartConfigs(object):
	def __init__(self, parts, template, prefix):
		self.prefix = prefix
		self.parts = RawConfigParser()

		# Loaded the part data file
		try:
			with io.open(parts) as parts_file:
				self.parts.readfp(parts_file)
				LOG.info("{} loaded.".format(parts))
		except IOError:
			logging.error("Couldn't read the part data file: {}.".format(parts))
			raise
		except ValueError:
			logging.error("Couldn't parse the part data file: {}.".format(parts))
			raise


		# Load the part template file
		try:
			LOG.debug("Reading {}.".format(template))
			with io.open(template, 'r') as template_cfg:
				# Read the template into memory
				self.template = template_cfg.readlines()
				LOG.info("{} loaded.".format(template))
		except IOError:
			LOG.error("Couldn't read part template file: {}".format(template))
			raise


	def build_all(self):
		LOG.debug("Building all part configurations.")

		for name in self.parts.sections():

			self.build_config(name)

		LOG.info("Done.")


	def build_config(self, name):
		LOG.debug("Starting build for {}.".format(name))

		part = dict(self.parts.items(name))
		part_cfg_path = os.path.join(self.prefix, 'Parts/{}.cfg'.format(name))

		try:
			LOG.debug("Reading {}.".format(part_cfg_path))

			with io.open(part_cfg_path, 'w+') as part_cfg:
				# Use python's built-in string formatting to customize the part config and write it to the file.
				for template_line in self.template:
					part_cfg.write(template_line.format(name=name, **part))
		except IOError:
			LOG.error("Unable to write to {}.".format(part_cfg_path))
			raise

		LOG.info("Built {}".format(part_cfg_path))


def main():
	parser = argparse.ArgumentParser(
		prog='python build-configs',
		formatter_class=argparse.RawDescriptionHelpFormatter,
		description="Generates KSP part .cfg files based on a template."
	)

	parser.add_argument('--prefix', default='build', help="directory where the part configurations will be placed")

	verbose=parser.add_mutually_exclusive_group()
	verbose.add_argument('--quiet', action="store_true", help="suppress output except for errors")
	verbose.add_argument('--verbose', action="store_true", help="show verbose output")

	parser.add_argument('parts', help="path to the parts file")
	parser.add_argument('template', help="path to the part template file")

	args = parser.parse_args()

	if args.quiet:
		LOG.setLevel(logging.ERROR)
	elif args.verbose:
		LOG.setLevel(logging.DEBUG)

	builder = BuildPartConfigs(args.parts, args.template, args.prefix)
	builder.build_all()


if __name__ == '__main__':
	main()
