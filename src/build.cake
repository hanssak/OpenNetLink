///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Git&version=1.1.0
#addin nuget:?package=Cake.Json&version=6.0.1
#addin nuget:?package=Newtonsoft.Json&version=13.0.1
#addin nuget:?package=Cake.Prompt&version=1.0.15
#addin nuget:?package=Cake.FileHelpers&version=4.0.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var customName = Argument("customName", "");
var configuration = Argument("configuration", "Release");
var setNetwork = Argument<bool>("setNetwork", true);
var isFull = Argument<bool>("isFull", true);	//false로 하면, 설치파일은 만들지 않는다.
var isPatch = Argument<bool>("isPatch", true);	//false로 하면, 패치파일은 만들지 않는다.
var isLightPatch = Argument<bool>("isLightPatch", false);
var isEnc = Argument<bool>("isEnc", true);
var deleteNetLink = Argument<bool>("deleteNetLink", false);		//true로 하면, 기존 NetLink Unintall.exe를 붙여넣기 한 후, 기존 NetLink를 삭제한다.
var isSilent = Argument<bool>("isSilent", false);				//true로 하면, Silent 모드
var startAuto = Argument<bool>("startAuto", true);				//false 하면, 설치 완료 후 자동 실행 안됨
var isSilentShowAll = Argument<bool>("isSilentShowAll", false);	//true로 하면, Silent / Show 모드 설치파일 모두 만듬
var regCrxForce = Argument<bool>("regCrxForce", false);					//true로 하면, NetPos가 "IN"인 Case
var patchAppEnv = Argument<bool>("patchAppEnv", false);					//true로 하면, patch때에 AppEnvSetting.json 파일을 덮어씌우는 동작함(win)
var inkFileName = Argument("inkFileName", "OpenNetLink");      // 바탕화면 Ink 파일 이름 설정 
var isPatchSilent = Argument<bool>("isPatchSilent", true);		// false로 하면 패치파일의 설치과정을 UI View로 변경함(사용자가 직접 여러번 클릭해줘야함.)
var regStartProgram = Argument<bool>("regStartProgram", true);		// 시작프로그램에 등록 여부(Window)
var isUpdateCheck = Argument<bool>("isUpdateCheck", false);				//false 하면 업데이트 체크 안함
var useMakeConfig = Argument<bool>("useMakeConfig", false);		//Json 형식으로된 MakeConfig.json을 로드하여 지정된 속성을 처리

var isPatchInstaller = false;
var nacLoginType ="0" ;		//0:none / 1:Genian NAC
var nacLoginEncryptKey ="";	//NAC 사용 시 전달되는 인증정보 암호화에 사용하는 Key 
var networkFlag = "NONE"; //NONE일 경우 패키지명에 networkflag는 비어진 상태로 나타남
// var customName = "NONE";
var storageName ="NONE";
var disableCertAutoUpdate =false;	//윈도우 버전 최초 설치 시, 로컬 보안 정책 > '인증서 자동 업데이트 사용안함' 설정 (default : false)
var AppProps = new AppProperty(Context,
								"./OpenNetLinkApp/Directory.Build.props", 				// Property file path of the build directory
								 "../", 													// Path of the Git Local Repository
								"./OpenNetLinkApp/wwwroot/conf/AppVersion.json",		// Version file Path 
								 "./openNetLinkApp/ReleaseNote.md");						// Release Note of Patch File

var MakeProps = new MakeProperty(Context, useMakeConfig, "./OpenNetLinkApp/wwwroot/SiteProfile/MakeConfig.json");

string PackageDirPath 		= "NONE";
string ReleaseNoteDirPath 	= "NONE";
// string PackageZipFile 		= String.Format("OpenNetLink-{0}-{1}.hz", AppProps.AppUpdatePlatform, AppProps.PropVersion.ToString());
string siteProfilePath = "./OpenNetLinkApp/wwwroot/SiteProfile";
string publishInitJsonDirPath ="NONE";	//암호화가 필요한 json 초기 파일 경로
///////////////////////////////////////////////////////////////////////////////
// CLASSES
///////////////////////////////////////////////////////////////////////////////
public class MakeProperty
{
	ICakeContext Context{get;}
	public JObject FileObj;


	public MakeProperty(ICakeContext context, bool useMakeConfig, string makeConfigFile)
	{
		if(useMakeConfig == false)	return;		
		Context = context;		
		FileObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(makeConfigFile));
	}

	public JObject GetStorageValue(string storageName)
	{
		if(FileObj == null) return null;

		foreach(JObject storage in FileObj["STORAGE"])
		{
			if(storage["STORAGE_NAME"].ToString() == storageName)
				return storage;
		}
		return null;
	}
	public JObject GetAgentValue(string storageName, string agentName)
	{
		JObject storage = GetStorageValue(storageName);		
		if(storage == null) return null;

		foreach(JObject agent in storage["AGENT"])
		{
			if(agent["AGENT_NAME"].ToString() == agentName)
				return agent;
		}
		return null;
	}

	public string GetLinkFileName(string storageName, string agentName)
	{
		JObject agent= GetAgentValue(storageName, agentName);
		if(agent == null || agent["LNK_FILE_NAME"] == null || agent["LNK_FILE_NAME"].ToString() == "")	
			return "OpenNetLink";
		else
			return agent["LNK_FILE_NAME"].ToString();
	}
}

public class AppProperty
{
    ICakeContext Context { get; }
	public string Platform {get;set;}

	public string PropsFile { get; }
	public string GitRepoPath { get; }
	public string VersionFile { get; }
	public string AppEnvFile { get; }
	//public string NetworkFile { get; }
	public string ReleaseNoteFile {get;}
	private JObject VersionJObj { get; set;}
	private JObject AppEnvJObj { get; }
	//private JObject NetworkJobj { get; }
	private string _updateSvcIp = "";
	
	public AppProperty(ICakeContext context, string propsFile, string gitRepoPath, string versionFile, string releaseNoteFile)
	{
		Context = context;

		PropsFile = propsFile;
		GitRepoPath = gitRepoPath;
		VersionFile = versionFile;
		// AppEnvFile = appEnvFile;
		// NetworkFile = networkFile;
		ReleaseNoteFile = releaseNoteFile;

		VersionJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(VersionFile));
		// AppEnvJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(AppEnvFile));
		// NetworkJobj = JsonAliases.ParseJsonFromFile(Context, new FilePath(NetworkFile));
	}

	public void ReloadVersionFile()
	{
		VersionJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(VersionFile));
	}

	public string InstallerRootDirPath
	{
		get {
			return $"artifacts/installer/{Platform}";
		}
	}

	public Version PropVersion {
		get {
			var readedVersion = XmlPeekAliases.XmlPeek(Context, PropsFile, "//Version");
			return new Version(readedVersion);
		}
		set {
			var version = value.ToString();
			XmlPokeAliases.XmlPoke(Context, PropsFile, "//Version", version);
		}
	}
	public string PropConfiguration {
		get {
			var configuration = XmlPeekAliases.XmlPeek(Context, PropsFile, "//Configuration");
			return configuration;
		}
		set {
			var configuration = value;
			XmlPokeAliases.XmlPoke(Context, PropsFile, "//Configuration", configuration);
			XmlPokeAliases.XmlPoke(Context, PropsFile, "//AssemblyConfiguration", configuration);
			
		}
	}
	public string PropCommitId {
		get {
			var commitId = XmlPeekAliases.XmlPeek(Context, PropsFile, "//CommitId");
			return commitId;
		}
		set {
			var commitId = value;
			XmlPokeAliases.XmlPoke(Context, PropsFile, "//CommitId", commitId);
			
		}
	}

	public GitCommit GitLastCommit {
		get {
			return GitAliases.GitLogTip(Context, GitRepoPath);
		}
	}
	public string GitLastTagName {
		get {
			var TagNameList = GitAliases.GitTags(Context, GitRepoPath);
			return TagNameList[TagNameList.Count -1].ToString();
		}
	}
	public int GitCommitCount {
		get {
			var commits = GitAliases.GitLog(Context, GitRepoPath, int.MaxValue);
			return commits.Count;
		}
	}
	public ICollection<GitCommit> GitLastTag {
		get {
			return GitAliases.GitLogTag(Context, GitRepoPath, GitLastTagName);
		}
	}
	public string GitLastShaId {
		get {
			return GitAliases.GitLogTip(Context, GitRepoPath).Sha;
		}
	}
	public string GitLastShaIdPretty {
		get {
			return GitAliases.GitLogTip(Context, GitRepoPath).Sha.Substring(0,7);
		}
	}

	public string AppSWVersion {
		get {
			return VersionJObj["SWVersion"].ToString();
		}
		set {
			VersionJObj["SWVersion"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(VersionFile), VersionJObj);
		}
	}
	public string AppEnvUpdateSvnIP {
		get {
			//return AppEnvJObj["UpdateSvcIP"].ToString();
			return _updateSvcIp;
		}
		set {
			_updateSvcIp = value;
			//AppEnvJObj["UpdateSvcIP"] = value;
			//JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(AppEnvFile), AppEnvJObj);
		}
	}
	public string AppSWCommitId {
		get {
			return VersionJObj["SWCommitId"].ToString();
		}
		set {
			VersionJObj["SWCommitId"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(VersionFile), VersionJObj);
		}
	}
	public string AppUpdatePlatform {
		get {
			return VersionJObj["UpdatePlatform"].ToString();
		}
		set {
			VersionJObj["UpdatePlatform"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(VersionFile), VersionJObj);
		}
	}

	public string AppLastUpdated {
		get {
			return VersionJObj["LastUpdated"].ToString();
		}
		set {
			VersionJObj["LastUpdated"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(VersionFile), VersionJObj);
		}
	}

	public string AppEnvUpdatePort {
		get {
			return "3439";
		}
	}
}

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Version")
    .Does(() => {
    /*
	var propsFile = "./OpenNetLinkApp/Directory.Build.props";
	var readedVersion = XmlPeek(propsFile, "//Version");
	var currentVersion = new Version(readedVersion);
	*/
	//var currentVersion = AppProps.PropVersion;
	//var semVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build, currentVersion.Revision + 1);
	//var semVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build, currentVersion.Revision);
	//var CurAppEnvSWVersion = AppProps.AppSWVersion;
	 var Commit = AppProps.GitLastCommit;
	 var ShaId = AppProps.GitLastShaIdPretty;
	
	//AppProps.PropVersion = semVersion;
	AppProps.PropCommitId = ShaId;
	//AppProps.AppSWVersion = semVersion.ToString();
	AppProps.AppSWCommitId = ShaId;

	Information($"- Last Commit Log: [{Commit.ToString()}]");
	Information($"- CommitId: [{AppProps.PropCommitId}]");
	Information($"- Version: [{AppProps.PropVersion}]");
	Information($"- AppEnv SWCommitId: [{AppProps.AppSWCommitId}]");
	Information($"- AppEnv SWVersion: [{AppProps.AppSWVersion}]");
});

Task("Clean")
//    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    DotNetCoreClean("./OpenNetLinkApp/OpenNetLinkApp.csproj");
});

Task("PkgClean")
    .Does(() =>
{
	if(DirectoryExists(AppProps.InstallerRootDirPath)) {
    	DeleteDirectory(AppProps.InstallerRootDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
		Information($"- Delete: [{AppProps.InstallerRootDirPath}]");
	}

	/*
	if(DirectoryExists(ReleaseNoteDirPath)) {
 		DeleteDirectory(ReleaseNoteDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
		Information($"- Delete: [{ReleaseNoteDirPath}]");
	}

	if(FileExists(PackageZipFile)) { 
 	 	DeleteFile(PackageZipFile);
	 	Information($"- Delete: [{PackageZipFile}]");
	}
	*/
});

Task("Build")
    .IsDependentOn("Version")
    .Does(() => {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = "Debug",
		};
		AppProps.PropConfiguration = "Debug";
        DotNetCoreBuild("./OpenNetLinkApp/OpenNetLinkApp.csproj", settings);
});

Task("Debug")
    .IsDependentOn("Version")
    .Does(() => {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = "Debug",
		};
		AppProps.PropConfiguration = "Debug";
        DotNetCoreBuild("./OpenNetLinkApp/OpenNetLinkApp.csproj", settings);
});

Task("Release")
    .IsDependentOn("Version")
    .Does(() => {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = "Release",
		};
		AppProps.PropConfiguration = "Release";
        DotNetCoreBuild("./OpenNetLinkApp/OpenNetLinkApp.csproj", settings);
});

Task("MakeHashSqlScript")
	.Does(()=> {
		//해시 생성 sql 문 생성 (Arg : 1 + [OS])
		var arg = $"1 {AppProps.Platform}";
		

		if(AppProps.Platform == "windows")
		{
			using(var process = StartAndReturnProcess("./HashTool/MD5HashUtility.exe", new ProcessSettings{ Arguments = arg }))    {
				process.WaitForExit();
			}
		}
		else if(AppProps.Platform == "debian" || AppProps.Platform == "redhat")
		{
			using(var process = StartAndReturnProcess("./HashToolLinux/MD5HashUtility", new ProcessSettings
												{ Arguments = new ProcessArgumentBuilder()
												.Append(arg)
												}))
			{
				process.WaitForExit();
			}
		}
		else if(AppProps.Platform == "mac")
		{
			using(var process = StartAndReturnProcess("./HashToolOSX/MD5HashUtility", new ProcessSettings
												{ Arguments = new ProcessArgumentBuilder()
												.Append(arg)
												}))
			{
				process.WaitForExit();
			}
		}
		else
		{
			throw new Exception(String.Format("[Err] Not Support Platform : {0}", AppProps.Platform));
		}

	});

Task("EncryptConfig")
	.Does(()=> {
		//OP 파일 암호화 처리 (Arg : 2 + [publish 생성 OS폴더명]
		var arg = $"2 {AppProps.Platform}";
		var UtilityFilePath ="";
		if(AppProps.Platform == "windows")	
			UtilityFilePath = "./HashTool/MD5HashUtility.exe";
		else if(AppProps.Platform == "debian" || AppProps.Platform == "redhat")
			UtilityFilePath = "./HashToolLinux/MD5HashUtility";
		else if(AppProps.Platform == "mac")
			UtilityFilePath = "./HashToolOSX/MD5HashUtility";
		else
			throw new Exception(String.Format("[Err] Not Support Platform : {0}", AppProps.Platform));
		
		using(var process = StartAndReturnProcess(UtilityFilePath, new ProcessSettings
												{ Arguments = new ProcessArgumentBuilder()
												.Append(arg)
												}))
		{
			process.WaitForExit();
		}

			
		// if(AppProps.Platform == "windows")
		// {
		// 	using(var process = StartAndReturnProcess("./HashTool/MD5HashUtility.exe", new ProcessSettings{ Arguments = arg }))    {
		// 		process.WaitForExit();
		// 	}
		// }
		// else if(AppProps.Platform == "debian" || AppProps.Platform == "redhat")
		// {
		// 	using(var process = StartAndReturnProcess("./HashToolLinux/MD5HashUtility", new ProcessSettings
		// 										{ Arguments = new ProcessArgumentBuilder()
		// 										.Append(arg)
		// 										}))
		// 	{
		// 		process.WaitForExit();
		// 	}
		// }
		// else if(AppProps.Platform == "mac")
		// {
		// 	using(var process = StartAndReturnProcess("./HashToolOSX/MD5HashUtility", new ProcessSettings
		// 										{ Arguments = new ProcessArgumentBuilder()
		// 										.Append(arg)
		// 										}))
		// 	{
		// 		process.WaitForExit();
		// 	}
		// }
		// else
		// {
		// 	throw new Exception(String.Format("[Err] Not Support Platform : {0}", AppProps.Platform));
		// }

	});

Task("EncryptInitDirectory")
	.Does(()=> {
		//지정 폴더 내 모든 파일 암호화 처리 (Arg : 3 + 폴더경로)
		var arg = $"3 {publishInitJsonDirPath}";
		var UtilityFilePath ="";
		if(AppProps.Platform == "windows")	
			UtilityFilePath = "./HashTool/MD5HashUtility.exe";
		else if(AppProps.Platform == "debian" || AppProps.Platform == "redhat")
			UtilityFilePath = "./HashToolLinux/MD5HashUtility";
		else if(AppProps.Platform == "mac")
			UtilityFilePath = "./HashToolOSX/MD5HashUtility";
		else
			throw new Exception(String.Format("[Err] Not Support Platform : {0}", AppProps.Platform));
		
		using(var process = StartAndReturnProcess(UtilityFilePath, new ProcessSettings
												{ Arguments = new ProcessArgumentBuilder()
												.Append(arg)
												}))
		{
			process.WaitForExit();
		}
	});

Task("PubCrossflatform")
	.IsDependentOn("Version")
	.Does(() => {	
	
	//AppUpdatePlatform 은 Deploy 시 활용
	AppProps.AppUpdatePlatform = AppProps.Platform;

	var runtime ="win-x64";

	if(AppProps.Platform == "windows")
		runtime = "win-x64";
	else if(AppProps.Platform == "debian" || AppProps.Platform == "redhat")
		runtime = "linux-x64";
	else if(AppProps.Platform == "mac")
		runtime = "osx-x64";

	var settings = new DotNetCorePublishSettings
	{
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = runtime,
		OutputDirectory = $"./artifacts/{AppProps.AppUpdatePlatform}/published"
	};
	
	if(DirectoryExists(settings.OutputDirectory)) {
		DeleteDirectory(settings.OutputDirectory, new DeleteDirectorySettings { Force = true, Recursive = true });
	}		

	if(AppProps.Platform == "windows")
	{
		//Window 에 한하여 추가 작업
		String strWebViewLibPath 			= "./OpenNetLinkApp/Library/WebView2Loader.dll";
		if(FileExists(strWebViewLibPath)) { DeleteFile(strWebViewLibPath); }

		String strWebWindowNativeLibPath 	= "./OpenNetLinkApp/Library/WebWindow.Native.dll";
		if(FileExists(strWebWindowNativeLibPath)) { DeleteFile(strWebWindowNativeLibPath); }

		String strNetLinkUninstallPath = "./OpenNetLinkApp/Library/NetLink.Uninstall/uninstall.exe";
		if(FileExists(strNetLinkUninstallPath) & deleteNetLink.ToString().ToUpper() == "FALSE") { DeleteFile(strNetLinkUninstallPath); }
	}
	else
	{
		String strNetLinkUninstallDir = "./OpenNetLinkApp/Library/NetLink.Uninstall";		
		if(DirectoryExists(strNetLinkUninstallDir)) {
			DeleteDirectory(strNetLinkUninstallDir, new DeleteDirectorySettings { Force = true, Recursive = true });
		}
	}

    DotNetCorePublish("./OpenNetLinkApp", settings);
	DotNetCorePublish("./PreviewUtil", settings);

	if(AppProps.Platform != "mac")
		DotNetCorePublish("./ContextTransferClient", settings);
    
	//생성전 과거 해시 SQL문 삭제 
	String hashSqlFile ="./OpenNetLinkApp/VersionHash.txt";
	if(FileExists(hashSqlFile))	DeleteFile(hashSqlFile);

	Information($"Start MD5HashUtility - isEnc : {isEnc} / Platforms : {AppProps.AppUpdatePlatform}");

	//해시 SQL문 생성
	RunTarget("MakeHashSqlScript");

	if(!FileExists(hashSqlFile))
		throw new Exception(String.Format("[Err] Failed to create hash information file. : {0}", hashSqlFile));
});

Task("PkgCrossflatform")
.Does(()=>{
	//SetFileName
	
	if(isFull.ToString().ToUpper() == "TRUE")
	{
		if(customName == "")
			customName = Prompt("Custom Name : ");			
	}
		

	var LastUpdatedTime = DateTime.Now.ToString(@"yyyy\/MM\/dd H\:mm");

	if(DirectoryExists(AppProps.InstallerRootDirPath)) {
		DeleteDirectory(AppProps.InstallerRootDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
	}
	System.IO.Directory.CreateDirectory(AppProps.InstallerRootDirPath);

	//[빌드 전] Site 공통 파일 적용 (ex.HSText, 인증서 등)
	Information($"Copy [Site Unit] Common Files"); 

	//CopyFiles($"{siteProfilePath}/HSText.xml", "./OpenNetLinkApp/wwwroot/conf");
	CopyFiles($"{siteProfilePath}/Sparkling.service", "./OpenNetLinkApp/wwwroot/conf");
	CopyFiles($"{siteProfilePath}/postgresql.crt", "./OpenNetLinkApp/wwwroot/conf");

	//[빌드 전] 스토리지 공유 단위별 공통 파일 적용 (ex.OP, AppVersion, Release Note 등)
	Information($"Copy [Storage Unit] Common Files");

	string lastAppSWVersion ="";
	foreach(var storageUnit in System.IO.Directory.GetDirectories(siteProfilePath))
	{
		var storageUnitInfo =new DirectoryInfo(storageUnit);
		string unitName = storageUnitInfo.Name;

		if(unitName.Substring(0, 1) == ".") 
			continue;

		PackageDirPath =$"{AppProps.InstallerRootDirPath}/{unitName}/packages";
		ReleaseNoteDirPath = $"{AppProps.InstallerRootDirPath}/{unitName}/release_note";		
		System.IO.Directory.CreateDirectory(PackageDirPath);
		System.IO.Directory.CreateDirectory(ReleaseNoteDirPath);
		
		CopyFiles($"{storageUnit}/AppVersion.json", "./OpenNetLinkApp/wwwroot/conf");
		
		//Get AppVersion and Set Derectory.Build.props
		AppProps.ReloadVersionFile();							
		
		Information($"{lastAppSWVersion} - {AppProps.AppSWVersion}");

		if(lastAppSWVersion != AppProps.AppSWVersion)	//AppVersion이 이전에 빌드한 스토리지의 버전과 동일하다면, Publish 생략 (해시 파일 통일)
		{
			lastAppSWVersion = AppProps.AppSWVersion;
			AppProps.PropVersion = new Version(AppProps.AppSWVersion);
			AppProps.AppLastUpdated = LastUpdatedTime;

			Information($"AppPorps.PropVersion : {AppProps.PropVersion}");		
			
			//현재 스토리지 기준 Publish
			RunTarget("PubCrossflatform");		

			//publish 이후에, Edge 따로 백업 처리	
			if(DirectoryExists($"./artifacts/edge"))		
				DeleteDirectory($"./artifacts/edge", new DeleteDirectorySettings {Force = true, Recursive = true });
			if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/edge"))	
				CopyDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/edge", $"./artifacts/edge");					
		    //UpdateListList.txt는 잠시 백업
			if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp/UpdateFileList.txt"))
				CopyFiles($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp/UpdateFileList.txt", $"{storageUnit}");
		}
		CopyFile($"{storageUnit}/ReleaseNote.md", $"{ReleaseNoteDirPath}/{AppProps.PropVersion.ToString()}.md");				
		CopyFiles("./OpenNetLinkApp/VersionHash.txt", $"{AppProps.InstallerRootDirPath}/{unitName}");
		
		//published 경로에 설정 관련 json 파일 Copy 전 기존 파일 삭제
		DeleteFiles($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/AppOPsetting_*.json");				
		if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/NetWork.json"))
			DeleteFile($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/NetWork.json");			
		// if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/AppEnvSetting.json"))
		// 	DeleteFile($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/AppEnvSetting.json");
		if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/SqlQuery.xml"))
			DeleteFile($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/SqlQuery.xml");
		if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/HSText.xml"))
			DeleteFile($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/HSText.xml");

		//Delete Default SiteProfile
		if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/SiteProfile"))		
			DeleteDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/SiteProfile", new DeleteDirectorySettings {Force = true, Recursive = true });

		//Delete Log Directory
		if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/Log"))		
			DeleteDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/Log", new DeleteDirectorySettings {Force = true, Recursive = true });
			
		// window에 한하여 필요없는 파일들 배포전에 제거
		if(AppProps.Platform == "windows")
		{
			DeleteFiles("./artifacts/windows/published/*.so");
			DeleteFiles("./artifacts/windows/published/*.pdb");

			var files = GetFiles("./artifacts/windows/published/*.so.*");
			foreach(var file in files)
			{
				String strSearchFile = (String)file.FullPath;
				//Information("File: {0}", strSearchFile);

				int nIdex = strSearchFile.LastIndexOf('.');
				if (nIdex > 0)
				{
					String strItem = strSearchFile.Substring(nIdex+1);

					int n=0;
					bool isNumeric = int.TryParse(strItem, out n);
					if (isNumeric)
					{
						Information("File-Deleted: {0}", strSearchFile);
						DeleteFile(strSearchFile);
					}
				}		
			}
		}
		
		//[빌드 후] 에이전트 별 파일 적용 (ex.Network.json, AppEnvSetting 등)
		Information($"Copy [Agent Unit] Files - " + storageUnit);
		
		publishInitJsonDirPath = $"./artifacts/{AppProps.Platform}/published/wwwroot/conf/Init";	
		//설치파일 생성
		if(isFull.ToString().ToUpper() == "TRUE")
		{
			//UpdateFileList 삭제
			if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp"))
				DeleteDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp", new DeleteDirectorySettings {Force = true, Recursive = true });

			foreach(var agentUnit in System.IO.Directory.GetDirectories(storageUnit))
			{
				var agentUnitInfo = new DirectoryInfo(agentUnit);
				string AgentName= agentUnitInfo.Name;
				
				if(AgentName.Substring(0, 1) == ".")
					continue;
				
				if(DirectoryExists(publishInitJsonDirPath))		
					DeleteDirectory(publishInitJsonDirPath, new DeleteDirectorySettings {Force = true, Recursive = true });
				
				if(isEnc.ToString().ToUpper() == "TRUE")
				{
					//설치 패키지: OPSetting.json  / HSText.xml / Network.json / SqlQuery.xml을 Init에 생성
					System.IO.Directory.CreateDirectory(publishInitJsonDirPath);
					CopyFiles($"{siteProfilePath}/HSText.xml", publishInitJsonDirPath);
					CopyFiles($"{storageUnit}/AppOPsetting*.json", publishInitJsonDirPath);		
					CopyFiles($"{storageUnit}/SqlQuery.xml", publishInitJsonDirPath);		
					// CopyFiles($"{agentUnit}/AppEnvSetting.json", publishInitJsonDirPath);
					CopyFiles($"{agentUnit}/NetWork.json", publishInitJsonDirPath);				
					
					RunTarget("EncryptInitDirectory");	// Init 폴더(publishInitJsonDirPath)에 존재하는 파일 암호화 처리				
				}
				else
				{
					CopyFiles($"{siteProfilePath}/HSText.xml", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");	
					CopyFiles($"{storageUnit}/AppOPsetting*.json", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");						
					CopyFiles($"{storageUnit}/SqlQuery.xml", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");	
					CopyFiles($"{agentUnit}/NetWork.json", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");										
				}
				
				Information($"Make Agent Installer {AgentName}");

				networkFlag = AgentName.ToUpper();			
				storageName = unitName.ToUpper();
								
				CopyFiles($"{agentUnit}/AppEnvSetting.json", $"./artifacts/{AppProps.AppUpdatePlatform}/published/wwwroot/conf");
				CopyFiles($"{agentUnit}/NetWork.json", $"./artifacts/{AppProps.AppUpdatePlatform}/published/wwwroot/conf");

				//Check Op & Set SGNac.exe
				if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/Library/SGNacAgent"))		
					DeleteDirectory($"./artifacts/{AppProps.Platform}/published/Library/SGNacAgent", new DeleteDirectorySettings {Force = true, Recursive = true });
				nacLoginType ="0";
				nacLoginEncryptKey ="";
				disableCertAutoUpdate =false;
				
				if(AppProps.Platform == "windows")	//SGNAC.exe는 Window만 필요 시 배포
				{	
					//OP파일은 Plain Text인 SiteProfile에서 참고
					var opFiles = GetFiles($"{storageUnit}/AppOPsetting_*_{AgentName}.json");
					foreach(var opFile in opFiles)
					{
						String strOPFile = (String)opFile.FullPath;
						JObject OPJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(strOPFile));	
						if(nacLoginType == "0")
						{
							if(OPJObj["NACLoginType"] != null && OPJObj["NACLoginType"].ToString() != "" && OPJObj["NACLoginType"].ToString() != "0")
							{
								if(OPJObj["NACLoginEncryptKey"] == null || OPJObj["NACLoginEncryptKey"].ToString() == "")
									throw new Exception(String.Format("[Err] NACLoginType 사용 시, 인증상태 정보 암호화를 위한 키 'NACLoginEncryptKey'를 설정해주세요."));
								
								//Add SGNac.exe
								CopyFiles("./OpenNetLinkApp/Library/SGNacAgent/SGNac.exe", $"./artifacts/{AppProps.Platform}/published");
								nacLoginType = OPJObj["NACLoginType"].ToString();
								nacLoginEncryptKey = OPJObj["NACLoginEncryptKey"].ToString();
							}
						}

						if(disableCertAutoUpdate == false)
						{
							if(OPJObj["bDisableCertAutoUpdate"] != null && OPJObj["bDisableCertAutoUpdate"].ToString() != "" && OPJObj["bDisableCertAutoUpdate"].ToString().ToUpper() != "FALSE")
								disableCertAutoUpdate = true;
						}
					}

					//LightPatch 다음에 설치 생성 시, edge 없으면 복원하여 처리
					if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/edge") == false)	
					{
						if(DirectoryExists($"./artifacts/edge") == false)
							throw new Exception(String.Format($"[Err] 설치본을 위해 복원할 edge 폴더가 없습니다. Copy to [artifacts/edge] -> [artifacts/{AppProps.Platform}/published/wwwroot/edge]"));
						
						CopyDirectory($"./artifacts/edge", $"./artifacts/{AppProps.Platform}/published/wwwroot/edge");		
					}						
				}
				isPatchInstaller=false;
				if(useMakeConfig == true)
					inkFileName = MakeProps.GetLinkFileName(unitName, AgentName);
				RunTarget("MakeInstaller");		
			}
		}

		//패치파일 생성
		if(isPatch.ToString().ToUpper() == "TRUE")
		{
			if(DirectoryExists(publishInitJsonDirPath))		
				DeleteDirectory(publishInitJsonDirPath, new DeleteDirectorySettings {Force = true, Recursive = true });

			//UpdateFileList 이동
			if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp") == false)						
				System.IO.Directory.CreateDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp");
			CopyFiles($"{storageUnit}/UpdateFileList.txt", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf/temp");			

			if(isEnc.ToString().ToUpper() == "TRUE")
			{
				//패치 패키지의 경우,  OPSetting.json / HSText.xml / SqlQuery.xml을 Init에 생성
				System.IO.Directory.CreateDirectory(publishInitJsonDirPath);
				CopyFiles($"{siteProfilePath}/HSText.xml", publishInitJsonDirPath);			
				CopyFiles($"{storageUnit}/AppOPsetting*.json", publishInitJsonDirPath);		
				CopyFiles($"{storageUnit}/SqlQuery.xml", publishInitJsonDirPath);		
				
				RunTarget("EncryptInitDirectory");	// Init 폴더에 존재하는 파일 암호화 처리
			}
			else
			{
				CopyFiles($"{siteProfilePath}/HSText.xml", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");	
				CopyFiles($"{storageUnit}/AppOPsetting*.json", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");					
				CopyFiles($"{storageUnit}/SqlQuery.xml", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");	
			}

			//Light Patch 버전일 땐, edge 폴더 배포전에 제거
			if(isLightPatch.ToString().ToUpper().Equals("TRUE"))
			{
				if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/edge")) 
					DeleteDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/edge", new DeleteDirectorySettings { Force = true, Recursive = true });
			}
			
			if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/NetWork.json"))
				DeleteFile($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/NetWork.json");		

			if (patchAppEnv.ToString().ToUpper() == "FALSE")
			{
				if(FileExists($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/AppEnvSetting.json"))
					DeleteFile($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/AppEnvSetting.json");
			}
			nacLoginType ="0";
			disableCertAutoUpdate = false;
			isPatchInstaller=true;
			isSilent= isPatchSilent;			
			// 패치는 시작프로그램 등 설정하지 않으므로 제외
			// if(useMakeConfig == true)
			// 	inkFileName = MakeProps.GetLinkFileName(unitName, AgentName);
			RunTarget("MakeInstaller");		
		}
	}
});

Task("MakeInstaller")
	.Does(() => {

	//Agent 설치대상 별로 exe 파일 생성 (IN/EX ...)
	if(AppProps.Platform == "windows")
	{

		if (isSilentShowAll.ToString().ToUpper() == "TRUE")
		{
			Information("Package windows: Silent Mode !");
			MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
				Defines = new Dictionary<string, string>{
					{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
					{"IS_PATCH", isPatchInstaller.ToString().ToUpper()},
					{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
					{"NETWORK_FLAG", networkFlag.ToUpper()},
					{"CUSTOM_NAME", customName.ToUpper()},
					{"OUTPUT_DIRECTORY", PackageDirPath},
					{"DELETE_NETLINK", deleteNetLink.ToString().ToUpper()},
					{"IS_SILENT", "TRUE"},
					{"STARTAUTO", startAuto.ToString().ToUpper()},
					{"STORAGE_NAME", storageName.ToUpper()},
					{"UPDATECHECK", isUpdateCheck.ToString().ToUpper()},
					{"REG_CRX", regCrxForce.ToString().ToUpper()},
					{"PATCH_APPENV", patchAppEnv.ToString().ToUpper()},
					{"INK_NAME", $"\"{inkFileName}\""},
					{"NAC_LOGIN_TYPE", nacLoginType.ToString()},
					{"NAC_LOGIN_ENCRYPTKEY", nacLoginEncryptKey.ToString()},
					{"DISABLE_CERT_AUTOUPDATE", disableCertAutoUpdate.ToString().ToUpper()},
					{"REG_STARTPROGRAM", regStartProgram.ToString().ToUpper()},
				}
			});			

			Information("Package windows: Show Mode!");
			MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
				Defines = new Dictionary<string, string>{
					{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
					{"IS_PATCH", isPatchInstaller.ToString().ToUpper()},
					{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
					{"NETWORK_FLAG", networkFlag.ToUpper()},
					{"CUSTOM_NAME", customName.ToUpper()},
					{"OUTPUT_DIRECTORY", PackageDirPath},
					{"DELETE_NETLINK", deleteNetLink.ToString().ToUpper()},
					{"IS_SILENT", "FALSE"},
					{"STARTAUTO", startAuto.ToString().ToUpper()},
					{"STORAGE_NAME", storageName.ToUpper()},
					{"UPDATECHECK", isUpdateCheck.ToString().ToUpper()},
					{"REG_CRX", regCrxForce.ToString().ToUpper()},
					{"PATCH_APPENV", patchAppEnv.ToString().ToUpper()},
					{"INK_NAME", $"\"{inkFileName}\""},
					{"NAC_LOGIN_TYPE", nacLoginType.ToString()},
					{"NAC_LOGIN_ENCRYPTKEY", nacLoginEncryptKey.ToString()},
					{"DISABLE_CERT_AUTOUPDATE", disableCertAutoUpdate.ToString().ToUpper()},
					{"REG_STARTPROGRAM", regStartProgram.ToString().ToUpper()},
				}
			});
		}
		else
		{
			MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
				Defines = new Dictionary<string, string>{
					{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
					{"IS_PATCH", isPatchInstaller.ToString().ToUpper()},
					{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
					{"NETWORK_FLAG", networkFlag.ToUpper()},
					{"CUSTOM_NAME", customName.ToUpper()},
					{"OUTPUT_DIRECTORY", PackageDirPath},
					{"DELETE_NETLINK", deleteNetLink.ToString().ToUpper()},
					{"IS_SILENT", isSilent.ToString().ToUpper()},
					{"STARTAUTO", startAuto.ToString().ToUpper()},
					{"STORAGE_NAME", storageName.ToUpper()},
					{"UPDATECHECK", isUpdateCheck.ToString().ToUpper()},
					{"REG_CRX", regCrxForce.ToString().ToUpper()},
					{"PATCH_APPENV", patchAppEnv.ToString().ToUpper()},
					{"INK_NAME", $"\"{inkFileName}\""},
					{"NAC_LOGIN_TYPE", nacLoginType.ToString()},
					{"NAC_LOGIN_ENCRYPTKEY", nacLoginEncryptKey.ToString()},
					{"DISABLE_CERT_AUTOUPDATE", disableCertAutoUpdate.ToString().ToUpper()},
					{"REG_STARTPROGRAM", regStartProgram.ToString().ToUpper()},
				}
			});			
		}
	}
	else if(AppProps.Platform == "debian")
	{
		using(var process = StartAndReturnProcess("./PkgDebian.sh", new ProcessSettings
												{ Arguments = new ProcessArgumentBuilder()
												.Append(AppProps.PropVersion.ToString())
												.Append(isPatchInstaller.ToString().ToUpper())
												.Append(networkFlag.ToUpper()) 
												.Append(customName.ToUpper())
												.Append(PackageDirPath)//$5	
												.Append(isUpdateCheck.ToString().ToUpper())	//$6
												.Append(startAuto.ToString().ToUpper())//$7
												.Append(storageName.ToUpper())//$8									
												.Append(regCrxForce.ToString().ToUpper())//$9
																								})
												
		)
		{
			process.WaitForExit();
			Information("Package Debin: Exit code: {0}", process.GetExitCode());
		}
	}
	else if(AppProps.Platform == "redhat")
	{
		using(var process = StartAndReturnProcess("./PkgRedhat.sh", new ProcessSettings
													{ Arguments = new ProcessArgumentBuilder()
													.Append(AppProps.PropVersion.ToString())
													.Append(isPatchInstaller.ToString().ToUpper())
													.Append(networkFlag.ToUpper()) 
													.Append(customName.ToUpper())
													.Append(PackageDirPath) //$5
													.Append(isUpdateCheck.ToString().ToUpper())	//$6
													.Append(startAuto.ToString().ToUpper()) //$7
													.Append(storageName.ToUpper())//$8
													
													})
		)
		{
			process.WaitForExit();
			Information("Package Redhat: Exit code: {0}", process.GetExitCode());
		}
	}
	else if( AppProps.Platform == "mac" )
	{
		using(var process = StartAndReturnProcess("./MacOSAppLayout/PkgAndNotarize.sh", new ProcessSettings
													{ Arguments = new ProcessArgumentBuilder()
													.Append(AppProps.PropVersion.ToString())
													.Append(isPatchInstaller.ToString().ToUpper())
													.Append(networkFlag.ToUpper()) 
													.Append(customName.ToUpper())	//$4
													.Append(PackageDirPath)	//$5
													.Append(isUpdateCheck.ToString().ToUpper())	//$6
													.Append(startAuto.ToString().ToUpper())//$7
													.Append(storageName.ToUpper())//$8
													.Append(regCrxForce.ToString().ToUpper())//$9
													})
		)
		{
			process.WaitForExit();
			Information("Package osx: Exit code: {0}", process.GetExitCode());
		}
	}
});

Task("PkgWin10")
    .Does(() => {
		AppProps.Platform = "windows";
		RunTarget("PkgCrossflatform");
});

Task("PkgOSX")	
    .Does(() => {
		AppProps.Platform = "mac";
		RunTarget("PkgCrossflatform");	
});

Task("PkgDebian")
    .Does(() => {
	AppProps.Platform = "debian";
	RunTarget("PkgCrossflatform");
});

Task("PkgRedhat")
    .Does(() => {
		AppProps.Platform = "redhat";
		RunTarget("PkgCrossflatform");

});

Task("Deploy")
	//.IsDependentOn("CreateReleaseNote")
	.IsDependentOn("Install-DotnetCompressor")
    .Does(() => {

	//패치 파일만드는 Platform 은 마지막 Appversion의 Platform 보고 판단
	AppProps.Platform = AppProps.AppUpdatePlatform;
	//스토리피 별 패치 파일 생성
	foreach(var storageUnit in System.IO.Directory.GetDirectories(AppProps.InstallerRootDirPath))
	{
		var storageUnitInfo =new DirectoryInfo(storageUnit);
		string unitName = storageUnitInfo.Name;

		if(unitName.Substring(0, 1) == ".") 
			continue;

		if(DirectoryExists($"{storageUnit}/patch")) {
			DeleteDirectory($"{storageUnit}/patch", new DeleteDirectorySettings { Force = true, Recursive = true });
		}
		System.IO.Directory.CreateDirectory($"{storageUnit}/patch/{AppProps.Platform}/packages");
		System.IO.Directory.CreateDirectory($"{storageUnit}/patch/{AppProps.Platform}/release_note");

		foreach(var exeFile in GetFiles($"{storageUnit}/packages/*.*"))
		{	
			string exeName = System.IO.Path.GetFileName(exeFile.FullPath);
			
			if(exeName.Substring(0, 1) == "." || exeName.Substring(0, 1) == "[" || exeName.Substring(0,1) == "#")
				continue;
			
			//패치 exe 파일만 patch 폴더로 Copy
			CopyFiles(exeFile.FullPath, $"{storageUnit}/patch/{AppProps.Platform}/packages");
		}

		string[] packages = System.IO.Directory.GetFiles($"{storageUnit}/patch/{AppProps.Platform}/packages");
		if(packages.Length != 1)
			throw new Exception(String.Format("[Err] No patch file or 2 or more patch files exist. : {0}", $"{storageUnit}/patch/{AppProps.Platform}/packages"));

		CopyFiles($"{storageUnit}/release_note/*.md", $"{storageUnit}/patch/{AppProps.Platform}/release_note");
				
		string PackagePath = packages[0];
		string PackagefileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(PackagePath);
		string PackageZipFile 		= $"{storageUnit}/{PackagefileNameWithoutExtension}.hz";
		
		if(!FileExists($"{siteProfilePath}/.HzCompressSeed"))
			throw new Exception($"[Err] Not Found Hz Compress Password : {siteProfilePath}/.HzCompressSeed");

		string Password = FileReadText($"{siteProfilePath}/.HzCompressSeed");

		Information($"Make hz File - {PackageZipFile}");
		// 압축 command: dcomp zip c -p [password] -b artifacts/packages/ -o test.zip
		using(var process = StartAndReturnProcess("dcomp"
							, new ProcessSettings { 
								Arguments = new ProcessArgumentBuilder()
												.Append("zip")
												.Append("c")
												.Append("-p").AppendQuoted(Password)
												.Append("-b").AppendQuoted($"{storageUnit}/patch")
												.Append("-o").AppendQuoted(PackageZipFile)
								}))
		{
			process.WaitForExit();
		}
		
		DeleteDirectory($"{storageUnit}/patch", new DeleteDirectorySettings { Force = true, Recursive = true });
	}
});


Task("Install-NetSparkleUpdater.Tools.AppCastGenerator")
    .Does(() => {

	// command: dotnet tool install --global NetSparkleUpdater.Tools.AppCastGenerator --version 2.0.8
	using(var process = StartAndReturnProcess("dotnet"
						, new ProcessSettings { 
							Arguments = new ProcessArgumentBuilder()
											.Append("tool")
											.Append("install")
											.Append("--global")
											.Append("NetSparkleUpdater.Tools.AppCastGenerator")
											.Append("--version").AppendQuoted("2.0.8")
							}))
	{
		process.WaitForExit();
	//	Information("Exit code: {0}", process.GetExitCode());
	}
});

Task("Install-DotnetCompressor")
    .Does(() => {
	// command: dotnet tool install --global dotnet-compressor 
	using(var process = StartAndReturnProcess("dotnet"
						, new ProcessSettings { 
							Arguments = new ProcessArgumentBuilder()
											.Append("tool")
											.Append("install")
											.Append("--global")
											.Append("dotnet-compressor")
											.Append("--version").AppendQuoted("1.2.0")
							}))
	{
		process.WaitForExit();
	}
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);

