#!/usr/bin/python
import fnmatch
import os
from distutils.version import LooseVersion

import sys


def main(argv):
    unity_exe_path = None
    for n in range(len(argv)):
        if argv[n] == '-unityExePath' and n + 1 < len(argv):
            unity_exe_path = argv[n + 1]

    try:
            unity_version = unity_exe_path.split('/')[2].split('_')[1]
    except:
        print "probably unity 4"
        return 0

    if LooseVersion(unity_version) < LooseVersion('5.6.4'):
        return 0

    print "Change *.gradle_aar files to *.aar"
    for root, dirnames, filenames in os.walk(os.path.join('..', '..')):
        filenames = [f for f in filenames if not f[0] == '.']
        dirnames[:] = [d for d in dirnames if not d[0] == '.']
        for filename in fnmatch.filter(filenames, '*.gradle_aar'):
            name = os.path.splitext(filename)[0]
            aar_file = os.path.join(root, ''.join([name, '.aar']))
            file_path = os.path.join(root, filename)
            print 'Rename the file: %s to: %s' % (file_path, aar_file)
            os.rename(os.path.join(root, filename), aar_file)


if __name__ == "__main__":
    main(sys.argv[1:])
