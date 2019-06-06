#!/usr/bin/python

import os, sys, getopt
import json
import glob
import urllib
import boto3
from distutils.version import StrictVersion
import shutil


from uninstall_psdk import delete_psdk
from pprint import pprint
from distutils.dir_util import copy_tree
#from appsDbReader import get_appsdb_json
from xml.etree import ElementTree as ET
import zipfile

installReferenceDict = {
	"appsFlyer"		: "PSDKAppsFlyer",
	"core"			: "PSDKCore",
	"installer"		: "PSDKInstaller",
	"locationMgr"	: "PSDKMonetization",
	"banners"		: "PSDKBanners",
	"gameLevelData"	: "PSDKGameLevelData",
	"rewardedAds"	: "PSDKRewardedAds",
	"splash"		: "PSDKSplash",
	"referrals"		: "PSDKGoogleAnalytics",
	"gsdkSocialNetworks"	: "PSDKSocial",
	"crashMonitoringTool"	: "PSDKCrashTool",
	"analytics"		: "PSDKAnalytics",
	"billing"		: "PSDKBilling",
	"rateUs"		: "PSDKRateUs",
	"share"			: "PSDKShare",
	"singular"     		: "PSDKSingular",
	"crossDevicePersistency": "PSDKCrossDevicePersistency",
	"nativeCampaign"        : "PSDKNativeCampaign"
}



def main(argv):
	currentScriptDirectory=os.path.dirname(os.path.abspath(__file__))
	unityProjectDirectory=os.path.join(currentScriptDirectory,'..','..')
	psdkBranch=None
	unityExePath=None
	store=None
	for n in range(len(argv)):
		if (argv[n] == '-psdk.branch' and n+1 < len(argv)):
			psdkBranch=argv[n+1]
		if (argv[n] == '-unityExePath' and n+1 < len(argv)):
			unityExePath=argv[n+1]
		if (argv[n] == '-store' and n+1 < len(argv)):
			store=argv[n+1]


	if (psdkBranch == None):
		print __file__, ": No PSDK branch specified, not installing PSDK !"
		return

	if (psdkBranch.lower() == 'nosdk'):
		print __file__, ": NoSDK specified, not installing PSDK !"
		return

	if (psdkBranch.lower() == 'master'):
		psdkBranch='0.0.0.0'

	if (unityExePath != None and unityExePath.startswith('/Applications/Unity_4')):
		print __file__, ": Unity 4 not supported for PSDK automatic installtion !"
		return

	if (len(psdkBranch.split('.')) < 3):
		print __file__, ": PSDK branch too short " + psdkBranch + " , not installing PSDK !"
		return

	print "psdkBranch=" + psdkBranch
	
	if (store.lower() == 'apple'):
		store='ios'

	streamingAssetsDirectory = os.path.join(unityProjectDirectory,'Assets','StreamingAssets')

	psdkJson = read_json_file(os.path.join(streamingAssetsDirectory,'psdk_' + store + '.json'))

	if (psdkJson == None):
	    psdkJson = read_json_file(os.path.join(unityProjectDirectory,'Assets','Plugins','Android','assets','psdk_' + store + '.json'))

        if (psdkJson == None):
            psdkJson = read_json_file(os.path.join(unityProjectDirectory,'assets','Plugins','Android','assets','psdk_' + store + '.json'))

	if (psdkJson == None):
		print "Error reading ",os.path.join(streamingAssetsDirectory,'psdk_'+store+'.json')," !!"
		return -1

	psdkPkgsToInstall={ "core":True }
	set_psdk_pkgs_to_install(psdkJson, psdkPkgsToInstall)
	install_psdk_pkgs(psdkPkgsToInstall,psdkBranch,unityProjectDirectory)
	install_psdkobbdownloader_if_needed(store=store,psdkJson=psdkJson,psdkVersion=psdkBranch,unityProjectDirectory=unityProjectDirectory)


def mount_tab_share_repo():
	os.system("mkdir -p /Volumes/REPO; mount_smbfs smb://guest@192.168.200.8/REPO /Volumes/REPO");

def get_installed_psdk_version(unityProjectDirectory,serviceName='core'):
	streamingAssetsDirectory = os.path.join(unityProjectDirectory,'Assets','StreamingAssets')
	psdkVersionFilePath = os.path.join(streamingAssetsDirectory,"psdk","versions",installReferenceDict[serviceName]+".unitypackage.version.txt");
        installedPsdkVersion=None
        try:
                fileContent=None
                with open(psdkVersionFilePath,'r') as f:
                        fileContent=f.read().replace('\n', '')
                if fileContent:
                        #print("---------------------------- \n{}\n --------------------------\n".format(fileContent))
			installedPsdkVersion=fileContent.replace('\n', '').replace(' ', '')
                f.close()
        except:
                pass

        return installedPsdkVersion


def install_psdk_pkgs(psdkPkgsToInstall, psdkBranch, unityProjectDirectory):
	print "Mounting REPO"
	mount_tab_share_repo()
	remotePsdkPath = get_remote_psdk_path(psdkBranch)
	remotePsdkVersion = os.path.basename(remotePsdkPath.rstrip('/'))
        needInstallation=False
        for pkgToInstall in psdkPkgsToInstall:
	    installedPsdkVersion = get_installed_psdk_version(unityProjectDirectory,pkgToInstall);
	    if installedPsdkVersion:
	        print "installedPsdkVersion "+pkgToInstall+":\t" + installedPsdkVersion + ", remotePsdkVersion:" + remotePsdkVersion
	    if (installedPsdkVersion != remotePsdkVersion):
		needInstallation=True
        if not needInstallation:
            print "no need to install psdk, remote and installed are the same " + installedPsdkVersion + " !"
            return
        print "Injecting PSDK " + remotePsdkVersion
	print "Deleting PSDK"
	delete_psdk(unityProjectDirectory)
	print "installing:",psdkPkgsToInstall.keys()," from branch:",  psdkBranch
        sshfilepath = os.path.join('/Volumes','REPO','id_rsa_tabshare')
        for service in psdkPkgsToInstall:
                if psdkPkgsToInstall[service]:
                        if service in installReferenceDict:
                                zipFilePath = "tabtale@192.168.200.8:" + os.path.join(remotePsdkPath,installReferenceDict[service]+".unity.zip").replace('/Volumes','/share')
                                print 'extracting ' + zipFilePath + ' -> ' + unityProjectDirectory
                                unzipCommand = "rm -f " + unityProjectDirectory + "/psdk_tmp_pkg.zip ; date ; scp " + zipFilePath + " " + unityProjectDirectory + "/psdk_tmp_pkg.zip ; date ; unzip -o " + unityProjectDirectory + "/psdk_tmp_pkg.zip  -d " + unityProjectDirectory + " ; date ";
                                print unzipCommand
                                os.system(unzipCommand);
                                #with zipfile.ZipFile(zipFilePath, 'r') as zipToExtract:
                                #       zipToExtract.extractall(unityProjectDirectory)


def get_remote_psdk_path(psdkBranch):
    # checking if its a minor or major branch
    minorBranch=None
    mount_tab_share_repo();
    # looking for the exact psdkBranch as major branch
    majorBranchPath = os.path.join('/Volumes','REPO','unity','PublishingSDK',psdkBranch)
    if not os.path.exists(majorBranchPath):
        majorBranch='.'.join(psdkBranch.split('.')[0:len(psdkBranch.split('.'))-1])
        # trying to treat psdk branch as minor branch, e.g. looking for major branch of minor branch
        majorBranchPath = os.path.join('/Volumes','REPO','unity','PublishingSDK',majorBranch)
        if not os.path.exists(majorBranchPath):
            # psdkBrnach is not a major or minor branch, it means we need to find a major branch
            majorVersionsNumbers = []
            refs = {}
            majorBranchPath=None
            for b in glob.glob(os.path.join(os.path.join('/Volumes','REPO','unity','PublishingSDK'), psdkBranch + '*/')):
               lastNumber = int(b[b.rfind('.')+1:b.rfind('/')])
               majorVersionsNumbers.append(lastNumber)
               refs[str(lastNumber)]=b
            majorBranchPath=refs[str(sorted(majorVersionsNumbers,key=int)[-1])]

        if not majorBranchPath:
            #fail build
            print "Didn't find major branch path for psdkBranch ",psdkBranch,"!!! failure !"
            return None

    minorBranchPath=os.path.join('/Volumes','REPO','unity','PublishingSDK',majorBranchPath,psdkBranch)
    if not os.path.exists(minorBranchPath):
        minorBranchPath=None
        #find latest minor verison
        minorVersionsNumbers = []
        refs={}
        for b in glob.glob(os.path.join(majorBranchPath, '*/')):
            lastNumber = int(b[b.rfind('.')+1:b.rfind('/')])
            minorVersionsNumbers.append(lastNumber)
            refs[str(lastNumber)]=b
        minorBranchPath = refs[str(sorted(minorVersionsNumbers,key=int)[-1])]
    return minorBranchPath



def set_psdk_pkgs_to_install(psdkJson, psdkPkgsToInstall):
	for service in installReferenceDict:
		if service in psdkJson:
			if "included" in psdkJson[service]:
				if psdkJson[service]["included"]:
					psdkPkgsToInstall[service] = True;


def read_json_file(json_file_path):
	jsonData=None
	try:
		jsonTxt=None
		with open(json_file_path,'r') as f:
			jsonTxt=f.read().replace('\n', '')
		if jsonTxt:
			#print("---------------------------- \n{}\n --------------------------\n".format(jsonTxt))
			jsonData=json.loads(jsonTxt)
		f.close()
	except:
		pass

	return jsonData

def install_psdkobbdownloader_if_needed(store,psdkJson,psdkVersion,unityProjectDirectory):
	if not store:
		print  "Didn't get store paremeter,!!! failure !"
		return -1

	if not psdkJson:
		print  "Didn't get store psdkJson,!!! failure !"
		return -1

	if not psdkVersion:
		print  "Didn't get store psdkJson,!!! failure !"
		return -1

	if not store == "google":
		return

	if not "remoteAssets" in psdkJson:
		return

	if not "mode" in psdkJson["remoteAssets"]:
		return

	if  not str(psdkJson["remoteAssets"]["mode"]).lower() == "obb":
		return

	# Download PSDKObbDownloader version for this psdk.
	s3_client = boto3.client("s3","us-east-1")
	bucketName = 'com.tabtale.repo'
	psdkMajor=None
	psdkMinor=None
	if (len(psdkVersion.split(".")) > 4):
		psdkMajor='.'.join(psdkVersion.split(".")[:-1])
		psdkMinor=psdkVersion
	else:
		psdkMajor=psdkVersion
		result = s3_client.list_objects(Bucket=bucketName, Prefix="psdk/" + psdkVersion + "/", Delimiter="/")
		versionMap = {}
		psdkBuildVersionList = []
		for prefix in result['CommonPrefixes']:
			psdkBuildPath =  str(prefix['Prefix'])
			psdkBuildVersion = psdkBuildPath.split('/')[-2]
			versionMap[psdkBuildVersion] = psdkBuildPath
			psdkBuildVersionList.append(psdkBuildVersion)
		psdkBuildVersionList = sorted(psdkBuildVersionList, key = lambda s: map(int, s.split('.')))
		psdkMinor = psdkBuildVersionList[-1]

	lastPath = "psdk/" + psdkMajor + "/" + psdkMinor  + "/buildConfig.json"
	obj = s3_client.get_object(Bucket=bucketName, Key=lastPath)
	jsonData = json.loads(obj['Body'].read())
	psdkObbDownloaderVersion =  jsonData["service"]["psdkobbdownloader"]["version"];
        #getting local installed version of PSDKDownloader
        local_PsdkDownloader_version = local_psdkobbdownloader_installed_version(unityProjectDirectory=unityProjectDirectory)
        if local_PsdkDownloader_version:
            if (compare_versions(psdkObbDownloaderVersion,local_PsdkDownloader_version) > 0):
                print "local PSDKObbDownloader version is bigger than being installed  " + local_PsdkDownloader_version + " > " + psdkObbDownloaderVersion + ", not installing PSDKObbDownloader"
                return
        fileToDownload = 'unity/psdk/artifacts/release/com/publishingsdk/psdkobbdownloader/'+psdkObbDownloaderVersion+'/PSDKObbDownloader.zip'
        cache_path = os.path.join('/tmp','TTBuilder','cache','psdk','psdkobbdownloader',psdkObbDownloaderVersion)
        localFile = os.path.join(cache_path,'PSDKObbDownloader.zip')
        if not os.path.exists(localFile):
            if not os.path.exists(cache_path):
                os.makedirs(cache_path);
            s3_client.download_file(bucketName,fileToDownload,localFile)
        local_psdk_obb_downloader_folder_path = os.path.join(unityProjectDirectory,"Assets","TabTale","PSDK","services","PSDKObbDownloader")
        if os.path.exists(local_psdk_obb_downloader_folder_path):
            print 'deleting ' + local_psdk_obb_downloader_folder_path
            shutil.rmtree(local_psdk_obb_downloader_folder_path)
	print 'extracting ' + localFile + ' -> ' + unityProjectDirectory
	unzipCommand = "unzip -o " + localFile + " -d " + unityProjectDirectory;
	print unzipCommand
	os.system(unzipCommand);


def local_psdkobbdownloader_installed_version(unityProjectDirectory):
    #Assets/TabTale/PSDK/services/PSDKObbDownloader/.PSDKObbDownloader.version.txt 
    local_psdkdownloader_version_file_path = os.path.join(unityProjectDirectory,'Assets','TabTale','PSDK','services','PSDKObbDownloader','.PSDKObbDownloader.version.txt')
    if not os.path.exists(local_psdkdownloader_version_file_path):
        return None
    with open(local_psdkdownloader_version_file_path,'r') as f:
        return f.read().replace('\n', '').replace(' ','')
    
    

def compare_versions(ver1, ver2):
    if ver1 == ver2:
        return 0;

    ver_list = [ ver1, ver2 ]
    ver_list = sorted(ver_list, key = lambda s: map(int, s.split('.')))
    if ver_list[1] == ver2:
        return 1;
    return -1
    



if __name__ == "__main__":
   #exit(install_psdkobbdownloader_if_needed(store="google",psdkJson=json.loads('{"remoteAssets": {"mode":"OBB"}}',encoding='ascii'),psdkVersion="4.7.1.0",unityProjectDirectory='/tmp'))
   exit(main(sys.argv[1:]))
