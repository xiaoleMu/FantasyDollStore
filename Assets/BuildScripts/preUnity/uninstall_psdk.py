#!/usr/bin/python

import os, sys, getopt
import glob


test = False

def main(argv):
	currentScriptDirectory=os.path.dirname(os.path.abspath(__file__))
	unityProjectDirectory=os.path.join(currentScriptDirectory,'..','..')
	delete_psdk(unityProjectDirectory)
	
def delete_psdk(unityProjectDirectory):
    android_manifest_path = os.path.join("Assets","Plugins","Android","AndroidManifest.xml").lower()
    assets_tabtale_psdk = os.path.join("Assets","TabTale","PSDK").lower()
    assets_psdk = os.path.join("Assets","PSDK").lower()
    assets_tabtale_unitypackages_psdk = os.path.join("Assets","TabTale","UnityPackages","PSDK").lower()
    for files_to_delete_file in glob.glob(unityProjectDirectory + "/Assets/TabTale/PSDK/services/*/FileSet/*.files.txt") + glob.glob(os.path.abspath(__file__) + "/../../Assets/PSDK/FileSet/*.files.txt") + glob.glob(unityProjectDirectory + "/Assets/TabTale/UnityPackages/PSDK/services/*/FileSet/*.files.txt"):
        try:
            with open(files_to_delete_file,'r') as ftd:
                for fileToDelete in ftd:
                    try:
                        f = os.path.join(unityProjectDirectory , fileToDelete.replace('\n', ''))
                        if f.lower().endswith(android_manifest_path):
                            continue
                        if (not fileToDelete.lower().startswith(assets_tabtale_psdk)) and (not  fileToDelete.lower().startswith(assets_psdk)) and (not  fileToDelete.lower().startswith(assets_tabtale_unitypackages_psdk)):
                            continue
							
                        if os.path.exists(f):
                            print "rm ", f
                            if not test:
                                os.remove(f)
                            if f.endswith(".aar"):
                                g = f.replace(".aar",".gradle_aar")
                                if os.path.exists(g):
                                    print "rm ", g
                                    if not test:
                                        os.remove(g)
                    except:
                        pass
                ftd.close()
                try:
                    print "rm ",files_to_delete_file
                    if not test:
                        os.remove(files_to_delete_file)
                except:
                  pass
        except:
            pass
	
    for fileToDelete in glob.glob(os.path.join(unityProjectDirectory , "Assets/TabTale/PSDK/services/*/Editor/iOS/*framework")) + glob.glob(os.path.join(unityProjectDirectory , "Assets/TabTale/UnityPackages/PSDK/services/*/Editor/iOS/*framework")):
        try:
            print "rmdir ",fileToDelete
            if not test:
                os.rmdir(fileToDelete)
        except:
            pass

if __name__ == "__main__":
   exit(main(sys.argv[1:]))
