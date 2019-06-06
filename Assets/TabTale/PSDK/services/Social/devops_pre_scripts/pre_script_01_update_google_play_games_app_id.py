#!/usr/bin/python

import os
import sys
import re
import json

currentScriptDirectory = os.path.dirname(os.path.abspath(__file__))
projectDirectory = os.path.join(currentScriptDirectory, '..', '..','..','..','..','..')
google_play_games_settings_file = os.path.join(projectDirectory, 'ProjectSettings','GooglePlayGameSettings.txt')


def get_google_play_games_app_id(bundle_id, store):
    if bundle_id is None:
        print "BundleId is null !!!"
        return None

    if store is None:
        print "store is null !!!"
        return None

    if store != 'google':
        return None 
    
    #print os.path.join(projectDirectory, 'appsDbJson.' + str(bundle_id) + '.txt')
    apps_db_json = read_json_file(os.path.join(projectDirectory, 'appsDbJson.' + str(bundle_id) + '.txt'))
    if apps_db_json is None:
        print __file__, ": Error: failure: apps db json local file is missing: appsDbJson." + str(
            bundle_id) + ".txt , not updating google play games settings !"
        return None

    google_play_games_app_id = "googlePlayGamesAppId"
    if google_play_games_app_id not in apps_db_json[store]:
        print __file__, ":  apps db field " + google_play_games_app_id + " not exist in appsDbJson for bundleId " + str(
            bundle_id) + " , not updating google ap id !"
        return None

    return apps_db_json[store][google_play_games_app_id]


def update_google_play_games_settings_file(google_app_id, bundle_id,filepath):
    with open(filepath, "r+") as f:
        data = f.read()
        output = re.sub("proj.AppId=.*","proj.AppId="+google_app_id,data)
        output = re.sub("and.BundleId=.*","and.BundleId="+bundle_id,output)
        #print replaced
        f.seek(0)
    	f.write(output)
        f.truncate()

def log(*args):
    print("UnityPreBuild: " + args[0])


def log_error():
    print("UnityPreBuild ERROR: ")


def main(argv):
    bundle_id = None
    store = None
    for n in range(len(argv)):
        if argv[n] == '-bundleId' and n + 1 < len(argv):
            bundle_id = argv[n + 1]
        if argv[n] == '-store' and n + 1 < len(argv):
            store = argv[n + 1]

    if store == "ios":
        store = "apple"

    if os.path.isfile(google_play_games_settings_file):
        google_app_id = get_google_play_games_app_id(bundle_id, store)
        if google_app_id:
            print "updating google app id: " + google_app_id
            update_google_play_games_settings_file(google_app_id=google_app_id, bundle_id=bundle_id, filepath=google_play_games_settings_file)


def read_json_file(json_file_path):
    json_data = None
    try:
        with open(json_file_path, 'r') as f:
            json_txt = f.read().replace('\n', '')
        if json_txt:
            json_data = json.loads(json_txt)
            
        f.close()
    except Exception as err:
        print(__file__ + " Exception:", err)

    return json_data
if __name__ == "__main__":
    main(sys.argv)
