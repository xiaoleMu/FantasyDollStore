#!/usr/bin/env python
import os
import zipfile
from shutil import copyfile

targetDir='../../../../../../../_Game/TabTale/GPGS'
targetFile=os.path.join(targetDir,'MainLibProj.aar')

def zipdir(path, ziph):
    # ziph is zipfile handle
    for root, dirs, files in os.walk(path):
        for file in files:
	    if file.endswith('.meta'):
	        continue
            if file.endswith('.py'):
                continue;
            if file.endswith('.jar_'):
		oldFile=file
                file = file.replace('.jar_','.jar')
                os.rename(os.path.join(root,oldFile),os.path.join(root,file))
            if os.path.isdir(file):
                zipdir(os.path.join(path,file),ziph)
            else:
                ziph.write(os.path.join(root, file))
            if file.endswith('.jar'):
		oldFile=file
                file = file.replace('.jar','.jar_')
		os.rename(os.path.join(root,oldFile),os.path.join(root,file))



if __name__ == '__main__':

    # MainLibProj was moved to an internal game folder instead of under psdk folders
    try:
        os.remove('../MainLibProj.aar')
    except OSError:
        pass

    if not os.path.exists(targetDir):
        os.makedirs(targetDir)

    zipf = zipfile.ZipFile(targetFile, 'w')
    zipdir('./', zipf)
    zipf.close()
