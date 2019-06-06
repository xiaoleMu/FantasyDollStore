import json
import os
import shutil
import sys
import argparse
from subprocess import call

reload(sys)
sys.setdefaultencoding('utf8')




def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('-project-folder')
    args, unknown = parser.parse_known_args()
    return adding_1024_icon_to_xcode_project(args.project_folder)




def adding_1024_icon_to_xcode_project(project_folder):
    print "adding_1024_icon_to_xcode_project " + project_folder
    os.chdir(project_folder)
    appIconFolderPath = os.path.join(project_folder,"Unity-iPhone","Images.xcassets","AppIcon.appiconset")
    if not os.path.exists(appIconFolderPath):
        print "not unity, quiting"
        return 0
    iconContentJsonFile = os.path.join(appIconFolderPath,"Contents.json")
    content=None
    jsonTxt=None
    with open(iconContentJsonFile,'r') as f:
        jsonTxt=f.read().replace('\n', '')
    if jsonTxt:
        content=json.loads(jsonTxt)
    f.close()
    if not content:
        print "error, didn't manage to read icon Contents.json file"
        return -1
    updateFile=None
    iconFileName = "Icon-2048.png"
    for index in range(0,len(content["images"])):
        if content["images"][index]["size"]=="1024x1024":
            if "filename" in content["images"][index]:
                # 1024x1024 icon exist, nothing to do
                print "filename already exist for 1024x1024 icon, nothing to do"
                return 0
            updateFile=True
            content["images"][index]["filename"] = iconFileName

    if not updateFile:
        print "didn't find 1024x1024 slot"
        content["images"].append({"filename": "Icon-2048.png","idiom": "ios-marketing","scale": "1x","size": "1024x1024"})
        updateFile=True

    with open(iconContentJsonFile , 'w+') as f:
        f.write(json.dumps(content, indent=4, sort_keys=True))
        f.close()
    iconDestFile = os.path.join(appIconFolderPath,iconFileName)
    if os.path.exists(iconDestFile):
        print "icon already exist at path " + iconDestFile
        return 0
    iconSourceFile = os.path.join("..","BuildResources","icons","1024x1024.png")
    if os.path.exists(iconSourceFile):
        shutil.copy(iconSourceFile,iconDestFile)
        print "ipad 1024x1024 icon copied from " + iconSourceFile
        return 0

    iconSourceFile=os.path.join(appIconFolderPath,"Icon-180.png")
    call(["sips", "-z","1024","1024",iconSourceFile,"--out",iconDestFile])
    print "icon 1024x1024 genrated from " + iconSourceFile
    return 0

if __name__ == "__main__":
    exit(main())
