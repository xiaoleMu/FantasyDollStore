import argparse
import os


def validate_google_play_services_version(store):
    if store != 'google' and store != 'amazon':
        return True

    script_dir_path = os.path.dirname(os.path.abspath(__file__))
    unity_project_dir_path = os.path.join(script_dir_path, '..', '..', '..', '..', '..', '..')
    android_dir_path = os.path.join(unity_project_dir_path, 'Assets/Plugins/Android')

    if not os.path.isdir(android_dir_path):
        return True

    is_found = False
    for root, dir_names, file_names in os.walk(android_dir_path):
        for file_name in file_names:
            if not file_name.endswith(".xml"):
                continue

            file_path = os.path.join(root, file_name)
            if 'google_play_services_version' in open(file_path).read():
                print 'Error - found "google_play_services_version" key in file: ' + os.path.abspath(file_path)
                is_found = True

    if is_found:
        print 'Please remove all the "google_play_services_version" keys in the file paths mentioned above. ' \
              'this metadata key can cause a mismatch and throw an error in runtime.'
        return False

    return True


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('-store')
    args, unknown = parser.parse_known_args()

    if not validate_google_play_services_version(args.store):
        return -1
    return 0


if __name__ == '__main__':
    exit(main())
