import datetime
import hashlib
import hmac
import json
import os
import plistlib
import zipfile
import subprocess
import shutil


def s3_sign(key, msg):
    return hmac.new(key, msg.encode('utf-8'), hashlib.sha256).digest()


def get_s3_signature_key(key, date_stamp, region_name, service_name):
    k_date = s3_sign(('AWS4' + key).encode('utf-8'), date_stamp)
    k_region = s3_sign(k_date, region_name)
    k_service = s3_sign(k_region, service_name)
    k_signing = s3_sign(k_service, 'aws4_request')

    return k_signing


def download_s3_file(path, access_key, secret_key, region):
    install_and_import('requests', '2.18.1')

    empty_body_sha256 = 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855'
    method = 'GET'
    service = 's3'
    host = 's3.amazonaws.com'

    t = datetime.datetime.utcnow()
    amzdate = t.strftime('%Y%m%dT%H%M%SZ')
    datestamp = t.strftime('%Y%m%d')

    canonical_uri = '/' + path
    canonical_querystring = ''
    canonical_headers = 'host:' + host + '\n' + 'x-amz-content-sha256:' + empty_body_sha256 + '\n' + 'x-amz-date:' + \
                        amzdate + '\n'
    signed_headers = 'host;x-amz-content-sha256;x-amz-date'
    payload_hash = hashlib.sha256('').hexdigest()
    canonical_request = method + '\n' + canonical_uri + '\n' + canonical_querystring + '\n' + canonical_headers + \
                        '\n' + signed_headers + '\n' + payload_hash
    algorithm = 'AWS4-HMAC-SHA256'
    credential_scope = datestamp + '/' + region + '/' + service + '/' + 'aws4_request'
    string_to_sign = algorithm + '\n' + amzdate + '\n' + credential_scope + '\n' + hashlib.sha256(
        canonical_request).hexdigest()

    signing_key = get_s3_signature_key(secret_key, datestamp, region, service)
    signature = hmac.new(signing_key, string_to_sign.encode('utf-8'), hashlib.sha256).hexdigest()
    authorization_header = algorithm + ' ' + 'Credential=' + access_key + '/' + credential_scope + ', ' + \
                           'SignedHeaders=' + signed_headers + ', ' + 'Signature=' + signature
    headers = {'x-amz-date': amzdate, 'Authorization': authorization_header, 'x-amz-content-sha256': empty_body_sha256,
               'Host': host}

    request_url = 'https://' + host + '/' + path
    response = requests.get(request_url, headers=headers)

    return response


DIR_PATH = os.path.dirname(os.path.abspath(__file__))
PSDK_SCRIPTS_DIR_PATH = os.path.join(DIR_PATH, '.psdk_scripts')
BUILD_CONFIG_JSON = None
PSDK_JSON = None

ACCESS_KEY = os.environ.get('AWS_ACCESS_KEY_ID')
SECRET_KEY = os.environ.get('AWS_SECRET_ACCESS_KEY')
REGION = 'us-east-1'


def install_and_import(package, version=None):
    import importlib
    import site
    import pip

    package_version = package
    if version is not None:
        package_version = package_version + '==' + version
    pip.main(['install', '--user', package_version])
    site.addsitedir(site.getusersitepackages())
    globals()[package] = importlib.import_module(package)


def read_psdk_version():
    psdk_version_file_path = os.path.join(DIR_PATH, 'Data/Raw/psdk/versions/PSDKCore.unitypackage.version.txt')
    if not os.path.isfile(psdk_version_file_path):
        print 'Failed to find the PSDKCore.unitypackage.version.txt file from: ' + psdk_version_file_path
        return None

    with open(psdk_version_file_path, 'r') as psdk_version_file:
        psdk_version = psdk_version_file.read().strip()

    return psdk_version


def get_psdk_version():
    info_plist_file_path = os.path.join(DIR_PATH, 'Info.plist')
    if os.path.isfile(info_plist_file_path):
        info_plist = plistlib.readPlist(info_plist_file_path)
        if 'CFPSDKVersion' in info_plist:
            return info_plist['CFPSDKVersion']

    return read_psdk_version()


def get_build_config_json():
    global BUILD_CONFIG_JSON

    if BUILD_CONFIG_JSON is not None:
        return BUILD_CONFIG_JSON

    psdk_minor_version = get_psdk_version()
    if not psdk_minor_version:
        print 'Failed to get psdk version from Info.plist'
        return None

    psdk_major_version = '.'.join(psdk_minor_version.split('.')[:-1])
    build_config_s3_file_path = 'com.tabtale.repo/psdk/' + psdk_major_version + '/' + psdk_minor_version + '/buildConfig.json'
    build_config_file_path = os.path.join(PSDK_SCRIPTS_DIR_PATH, 'buildConfig.json')
    response = download_s3_file(build_config_s3_file_path, ACCESS_KEY, SECRET_KEY, REGION)
    if response.status_code >= 400:
        print 'Failed to download: ' + build_config_s3_file_path + ' error: ' + response.content
        return None

    with open(build_config_file_path, 'wb') as handler:
        handler.write(response.content)

    with open(build_config_file_path) as build_config_file:
        BUILD_CONFIG_JSON = json.load(build_config_file)

    return BUILD_CONFIG_JSON


def get_psdk_json():
    global PSDK_JSON

    if PSDK_JSON is not None:
        return PSDK_JSON

    psdk_json_file_path = os.path.join(DIR_PATH, 'Data/Raw/psdk_ios.json')
    if not os.path.isfile(psdk_json_file_path):
        print 'Failed to read psdk.json. The file %s does not exists' % psdk_json_file_path
        return None

    with open(psdk_json_file_path) as psdk_json_file:
        PSDK_JSON = json.load(psdk_json_file)

    return PSDK_JSON


def download_scripts():
    if os.path.isdir(PSDK_SCRIPTS_DIR_PATH):
        shutil.rmtree(PSDK_SCRIPTS_DIR_PATH)
    os.makedirs(PSDK_SCRIPTS_DIR_PATH)

    build_config_json = get_build_config_json()
    if build_config_json is None:
        return False

    psdk_json = get_psdk_json()
    if psdk_json is None:
        return False

    xcode_scripts_json = build_config_json['xcode_scripts']
    if 'buildConfig' in psdk_json and 'xcode_scripts' in psdk_json['buildConfig']:
        xcode_scripts_json = dict(xcode_scripts_json.items() + psdk_json['buildConfig']['xcode_scripts'].items())

    scripts_s3_dir_path = 'com.tabtale.repo/psdk/scripts/xcode'

    for script_name, values in xcode_scripts_json.iteritems():
        version = values['version']
        script_file_name = script_name + '-' + version + '.zip'
        script_s3_file_path = os.path.join(scripts_s3_dir_path, script_name, version, script_file_name)
        script_file_path = os.path.join(PSDK_SCRIPTS_DIR_PATH, script_file_name)

        response = download_s3_file(script_s3_file_path, ACCESS_KEY, SECRET_KEY, REGION)
        if response.status_code >= 400:
            print 'Failed to download: ' + script_s3_file_path + ' error: ' + response.content
            return False
        with open(script_file_path, 'wb') as handler:
            handler.write(response.content)

        zip_ref = zipfile.ZipFile(script_file_path, 'r')
        zip_ref.extractall(PSDK_SCRIPTS_DIR_PATH)
        zip_ref.close()

    return True


def setup_scripts():
    for file_name in BUILD_CONFIG_JSON['xcode_scripts']:
        script_file_path = os.path.join(PSDK_SCRIPTS_DIR_PATH, os.path.splitext(file_name)[0], 'script.py')
        cmd = ['python', script_file_path, '--setup', '-project-dir-path', os.path.dirname(os.path.realpath(__file__))]
        return_code = subprocess.call(cmd)
        if return_code != 0:
            print 'Failed to setup the script: ' + script_file_path
            return False

    return True


def main():
    if not download_scripts():
        return -1

    if not setup_scripts():
        return -1

    return 0


if __name__ == "__main__":
    exit(main())
