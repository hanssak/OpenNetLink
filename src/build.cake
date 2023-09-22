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
var sitename = Argument("sitename", "hanssak");
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


var isPatchInstaller = false;
var networkFlag = "NONE"; //NONE일 경우 패키지명에 networkflag는 비어진 상태로 나타남
var customName = "NONE";
var storageName ="NONE";

var AppProps = new AppProperty(Context,
								"./OpenNetLinkApp/Directory.Build.props", 				// Property file path of the build directory
								 "../", 													// Path of the Git Local Repository
								"./OpenNetLinkApp/wwwroot/conf/AppVersion.json",		// Version file Path 
								// "./OpenNetLinkApp/wwwroot/conf/AppEnvSetting.json",		// Env file Path of the Application env settings
								// "./OpenNetLinkApp/wwwroot/conf/NetWork.json",			// Network file Path of the Network settings
								 "./openNetLinkApp/ReleaseNote.md");						// Release Note of Patch File

string PackageDirPath 		= "NONE";
string ReleaseNoteDirPath 	= "NONE";
// string PackageZipFile 		= String.Format("OpenNetLink-{0}-{1}.hz", AppProps.AppUpdatePlatform, AppProps.PropVersion.ToString());
string siteProfilePath = "./OpenNetLinkApp/wwwroot/SiteProfile";
///////////////////////////////////////////////////////////////////////////////
// CLASSES
///////////////////////////////////////////////////////////////////////////////

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
	// public string NetworkFromName {
	// 	get {
	// 		return NetworkJobj["NETWORKS"][0]["FROMNAME"].ToString();
	// 	}
	// 	set {
	// 		NetworkJobj["NETWORKS"][0]["FROMNAME"] = value;
	// 		JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
	// 	}
	// }

	// public string NetworkToName {
	// 	get {
	// 		return NetworkJobj["NETWORKS"][0]["TONAME"].ToString();
	// 	}
	// 	set {
	// 		NetworkJobj["NETWORKS"][0]["TONAME"] = value;
	// 		JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
	// 	}
	// }

	// public string NetworkIPAddress {
	// 	get {
	// 		return NetworkJobj["NETWORKS"][0]["IPADDRESS"].ToString();
	// 	}
	// 	set {
	// 		NetworkJobj["NETWORKS"][0]["IPADDRESS"] = value;
	// 		JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
	// 	}
	// }

	// public string NetworkPort {
	// 	get {
	// 		return NetworkJobj["NETWORKS"][0]["PORT"].ToString();
	// 	}		
	// }

	// public string NetworkPos {
	// 	get {
	// 		return NetworkJobj["NETWORKS"][0]["NETPOS"].ToString();
	// 	}	
	// 	set {
	// 		NetworkJobj["NETWORKS"][0]["NETPOS"] = value;
	// 		JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
	// 	}	
	// }

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

/*
Task("SetFileName")
	//patch가 아닐 경우만 실행
	.WithCriteria(!isPatch)
	.Does(()=>{
		customName = Prompt("Custom Name : ");	
		networkFlag = Prompt("Network Flag (IN/CN/EX) : ");		
		AppProps.NetworkPos = networkFlag.ToUpper();
	});

Task("SetNetwork")
	.WithCriteria(setNetwork)
	.WithCriteria(!isPatch)
	.Does(() => {		
		Information($"Current Network infomation : {AppProps.NetworkIPAddress} ({AppProps.NetworkFromName} -> {AppProps.NetworkToName}) / Update IP : {AppProps.AppEnvUpdateSvnIP}");
			
		AppProps.NetworkIPAddress = Prompt("IPAddress : ");
		AppProps.AppEnvUpdateSvnIP = $"{AppProps.NetworkIPAddress}:{AppProps.AppEnvUpdatePort}";
		AppProps.NetworkFromName = Prompt($"{AppProps.NetworkIPAddress} - From Name : ");
		AppProps.NetworkToName = Prompt($"{AppProps.NetworkIPAddress} - To Name : ");	

		Information($"Change Complete : {networkFlag} - {AppProps.NetworkIPAddress} ({AppProps.NetworkFromName} -> {AppProps.NetworkToName})");						
	});


Task("PubDebian")
    .IsDependentOn("Version")
    .Does(() => {

	AppProps.AppUpdatePlatform = "debian";
	PackageDirPath 	= String.Format("artifacts/installer/{0}/packages", AppProps.AppUpdatePlatform);
	var settings = new DotNetCorePublishSettings {
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "linux-x64",
		OutputDirectory = $"./artifacts/{AppProps.AppUpdatePlatform}/published"
	};
	
	if(DirectoryExists(settings.OutputDirectory)) {
		DeleteDirectory(settings.OutputDirectory, new DeleteDirectorySettings { 
		Force = true, Recursive = true });
	}

    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);
    DotNetCorePublish("./ContextTransferClient", settings);

	// 필요할때에 추가로 개발예정
    	using(var process = StartAndReturnProcess("./HashToolLinux/MD5HashUtility"))
        {
			process.WaitForExit();
			//Information("Package linux: Exit code: {0}", process.GetExitCode());
		}
});

Task("PkgDebian")
	.IsDependentOn("SetFileName")
	.IsDependentOn("SetNetwork")
    .IsDependentOn("PubDebian")
    .Does(() => {

	using(var process = StartAndReturnProcess("./PkgDebian.sh", new ProcessSettings{ 
		Arguments = new ProcessArgumentBuilder()
			.Append(AppProps.PropVersion.ToString())
			.Append(isPatch.ToString().ToUpper())
			.Append(networkFlag.ToUpper()) 
			.Append(customName.ToUpper())
		}))
	{
		process.WaitForExit();
		Information("Package Debin: Exit code: {0}", process.GetExitCode());
	}
});


Task("PubRedhat")
    .IsDependentOn("Version")
    .Does(() => {

	AppProps.AppUpdatePlatform = "redhat";
	PackageDirPath 	= String.Format("artifacts/installer/{0}/packages", AppProps.AppUpdatePlatform);
	var settings = new DotNetCorePublishSettings {
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "linux-x64",
		OutputDirectory = $"./artifacts/{AppProps.AppUpdatePlatform}/published"
	};
	
	if(DirectoryExists(settings.OutputDirectory)) {
		DeleteDirectory(settings.OutputDirectory, new DeleteDirectorySettings { 
		Force = true, Recursive = true });
	}

    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);
    DotNetCorePublish("./ContextTransferClient", settings);

	// 필요할때에 추가로 개발예정
    //	using(var process = StartAndReturnProcess("./HashToolLinux/MD5HashUtility"))
    //        {
	//	process.WaitForExit();
	//	Information("Package linux: Exit code: {0}", process.GetExitCode());
	//}
});

Task("PkgRedhat")
	.IsDependentOn("SetFileName")
	.IsDependentOn("SetNetwork")
    .IsDependentOn("PubRedhat")
    .Does(() => {

	using(var process = StartAndReturnProcess("./PkgRedhat.sh", new ProcessSettings{ 
		Arguments = new ProcessArgumentBuilder()
			.Append(AppProps.PropVersion.ToString())
			.Append(isPatch.ToString().ToUpper())
			.Append(networkFlag.ToUpper()) 
			.Append(customName.ToUpper())
		}))
	{
		process.WaitForExit();
		Information("Package Redhat: Exit code: {0}", process.GetExitCode());
	}
});

Task("PubWin10")
    .IsDependentOn("Version")
    .Does(() => {

	AppProps.AppUpdatePlatform = "windows";
	PackageDirPath 	= String.Format("artifacts/installer/{0}/packages", AppProps.AppUpdatePlatform);
	var settings = new DotNetCorePublishSettings
	{
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "win-x64",
		OutputDirectory = $"./artifacts/{AppProps.AppUpdatePlatform}/published"
	};

	String strWebViewLibPath 			= "./OpenNetLinkApp/Library/WebView2Loader.dll";
	if(FileExists(strWebViewLibPath)) { DeleteFile(strWebViewLibPath); }

	String strWebWindowNativeLibPath 	= "./OpenNetLinkApp/Library/WebWindow.Native.dll";
	if(FileExists(strWebWindowNativeLibPath)) { DeleteFile(strWebWindowNativeLibPath); }

	if(DirectoryExists(settings.OutputDirectory)) {
		DeleteDirectory(settings.OutputDirectory, new DeleteDirectorySettings { Force = true, Recursive = true });
	}		

    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);
    DotNetCorePublish("./ContextTransferClient", settings);
    
     using(var process = StartAndReturnProcess("./HashTool/MD5HashUtility.exe", new ProcessSettings{ Arguments = "1 windows" }))
     {
		process.WaitForExit();
		Information("Package windows: Exit code: {0}", process.GetExitCode());
     }

     using(var process = StartAndReturnProcess("./HashTool/MD5HashUtility.exe", new ProcessSettings{ Arguments = "2 windows" }))
     {
		process.WaitForExit();
		Information("Package windows: Exit code: {0}", process.GetExitCode());
     }
});

Task("PkgWin10")
	.IsDependentOn("SetFileName")
	.IsDependentOn("SetNetwork")
    .IsDependentOn("PubWin10")
    .Does(() => {
	if(DirectoryExists(PackageDirPath)) {
		DeleteDirectory(PackageDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
	}	

	System.IO.Directory.CreateDirectory(PackageDirPath);

	// window 쪽에 필요없는 파일들 배포전에 제거
	DeleteFiles("./artifacts/windows/published/*.so");
	DeleteFiles("./artifacts/windows/published/*.pdb");

	//Light Patch 버전일 땐, edge 폴더 배포전에 제거
	if(isPatch.ToString().ToUpper().Equals("TRUE"))
	{
		if(isLightPatch.ToString().ToUpper().Equals("TRUE"))
		{
			if(DirectoryExists("./artifacts/windows/published/wwwroot/edge")) 
			{
				DeleteDirectory("./artifacts/windows/published/wwwroot/edge", new DeleteDirectorySettings { Force = true, Recursive = true });
			}
		}
	}
	
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
            
	MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
		Defines = new Dictionary<string, string>{
			{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
			{"IS_PATCH", isPatch.ToString().ToUpper()},
			{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
			{"NETWORK_FLAG", networkFlag.ToUpper()},
			{"CUSTOM_NAME", customName.ToUpper()}			
		}
	});
});

Task("PubOSX")
	.IsDependentOn("Version")
    .Does(() => {
	AppProps.AppUpdatePlatform = "mac";
	PackageDirPath 		= String.Format("artifacts/installer/{0}/packages", AppProps.AppUpdatePlatform);
	var settings = new DotNetCorePublishSettings {
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "osx-x64",
		OutputDirectory = $"./artifacts/{AppProps.AppUpdatePlatform}/published"
	};
    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);

	using(var process = StartAndReturnProcess("./HashToolOSX/MD5HashUtility"))
             {
		process.WaitForExit();
		Information("Package mac: Exit code: {0}", process.GetExitCode());
	 }
});

Task("PkgOSX")	
	.IsDependentOn("SetFileName")
	.IsDependentOn("SetNetwork")
    .IsDependentOn("PubOSX")
    .Does(() => {

	if(DirectoryExists(PackageDirPath)) {
		DeleteDirectory(PackageDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
	}

	System.IO.Directory.CreateDirectory(PackageDirPath);

	using(var process = StartAndReturnProcess("./MacOSAppLayout/PkgAndNotarize.sh", new ProcessSettings{ 
		Arguments = new ProcessArgumentBuilder()
			.Append(AppProps.PropVersion.ToString())
			.Append(isPatch.ToString().ToUpper())
			.Append(networkFlag.ToUpper()) 
			.Append(customName.ToUpper())
		}))
	{
		process.WaitForExit();
		Information("Package osx: Exit code: {0}", process.GetExitCode());
	}
});

Task("CreateReleaseNote")
	.Does(() =>
{
	Information("CreateReleaseNote v{0}", AppProps.PropVersion.ToString());

	string Title 			= String.Format("OpenNetLink v{0}", AppProps.PropVersion.ToString());
	string ReleaseNotePath 	= String.Format("{0}/{1}.md", ReleaseNoteDirPath, AppProps.PropVersion.ToString());
	if(DirectoryExists(ReleaseNoteDirPath)) { DeleteDirectory(ReleaseNoteDirPath, new DeleteDirectorySettings { Force = true, Recursive = true }); }
	System.IO.Directory.CreateDirectory(ReleaseNoteDirPath);

	// Write File
	using(StreamWriter writer = new StreamWriter(ReleaseNotePath)){
		

		if(FileExists(AppProps.ReleaseNoteFile))
		{
			foreach(var line in FileReadLines(AppProps.ReleaseNoteFile))
			{
				writer.WriteLine(line);
			}
			
		}
		else
		{
			writer.WriteLine("# "+Title);
			writer.WriteLine("");
			foreach (var tag in AppProps.GitLastTag)
			{
				writer.WriteLine(tag.Message);
			}
		}
	};
});

Task("Appcast")
	.IsDependentOn("Install-NetSparkleUpdater.Tools.AppCastGenerator")
    .IsDependentOn("CreateReleaseNote")
	.Does(() =>
{
	string InstallerOsPath	= String.Format("artifacts/installer/{0}", AppProps.AppUpdatePlatform);
	string PackagesURL 		= String.Format("https://{0}/updatePlatform/{1}/packages/", AppProps.AppEnvUpdateSvnIP, AppProps.AppUpdatePlatform);
	string ReleaseNoteURL 	= String.Format("https://{0}/updatePlatform/{1}/release_note/", AppProps.AppEnvUpdateSvnIP, AppProps.AppUpdatePlatform);

	if(!DirectoryExists(PackageDirPath)) {
		throw new Exception(String.Format("[Error] Not Found Directory : {0}", PackageDirPath));
	}

	// default OS
	string strEXT 	= "exe";
	string strOS 	= "windows";
	if(AppProps.AppUpdatePlatform.Equals("mac")) { 
		strOS 	= "mac";
		strEXT 	= "pkg";
	}
	if(AppProps.AppUpdatePlatform.Equals("debian")) { 
		strOS 	= "linux";
		strEXT 	= "deb";
	}

	using(var process = StartAndReturnProcess("netsparkle-generate-appcast"
						, new ProcessSettings { 
							Arguments = new ProcessArgumentBuilder()
											.Append("--product-name").AppendQuoted("opennetlink")
											.Append("--file-extract-version").AppendQuoted(AppProps.PropVersion.ToString())
											.Append("--appcast-output-directory").AppendQuoted(InstallerOsPath)
											.Append("--os").AppendQuoted(strOS)
											.Append("--ext").AppendQuoted(strEXT)
											.Append("--key-path").AppendQuoted("Appcasts/Generator/keys")
											.Append("--binaries").AppendQuoted(PackageDirPath)
											.Append("--base-url").AppendQuoted(PackagesURL)
											
											// * README: when creating --change-log-path 
											// 1. There must be a directory version before the package.
											// 2. The version directory and md file should be the same name. 
											.Append("--change-log-path").AppendQuoted(ReleaseNoteDirPath)
											.Append("--change-log-url").AppendQuoted(ReleaseNoteURL)
							}))
	{
		process.WaitForExit();
		// Information("Exit code: {0}", process.GetExitCode());
	}
});


Task("Deploy")
	.IsDependentOn("CreateReleaseNote")
	.IsDependentOn("Install-DotnetCompressor")
    .Does(() => {

	string PackagePath;
	if(AppProps.AppUpdatePlatform.Equals("mac")) { 
		PackagePath = String.Format("{0}/OpenNetLink-Mac-{1}.pkg", PackageDirPath, AppProps.PropVersion.ToString());
	}
	else if(AppProps.AppUpdatePlatform.Equals("debian")) { 
		PackagePath = String.Format("{0}/OpenNetLink-Debian-{1}.deb", PackageDirPath, AppProps.PropVersion.ToString());
	}
	else if(AppProps.AppUpdatePlatform.Equals("windows")) { 
		PackagePath = String.Format("{0}/OpenNetLink-Windows-{1}.exe", PackageDirPath, AppProps.PropVersion.ToString());
	}
	else {
		throw new Exception(String.Format("[Err] Not Support Platform : {0}", AppProps.AppUpdatePlatform));
	}

	if(!FileExists(PackagePath)) {
		throw new Exception(String.Format("[Err] Not Found Package : {0}", PackagePath));
	}

	string Password = "%hsckconfigseed$";
	// 압축 command: dcomp zip c -p %hsckconfigseed$ -b artifacts/packages/ -o test.zip
	using(var process = StartAndReturnProcess("dcomp"
						, new ProcessSettings { 
							Arguments = new ProcessArgumentBuilder()
											.Append("zip")
											.Append("c")
											.Append("-p").AppendQuoted(Password)
											.Append("-b").AppendQuoted("artifacts/installer")
											.Append("-o").AppendQuoted(PackageZipFile)
							}))
	{
		process.WaitForExit();
		//Information("Result: {0} - Deploy Package Zip : {1}", process.GetExitCode(), PackagePath);
	}

});
*/

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
		customName = Prompt("Custom Name : ");			

	var LastUpdatedTime = DateTime.Now.ToString(@"yyyy\/MM\/dd H\:mm");

	if(DirectoryExists(AppProps.InstallerRootDirPath)) {
		DeleteDirectory(AppProps.InstallerRootDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
	}
	System.IO.Directory.CreateDirectory(AppProps.InstallerRootDirPath);

	//[빌드 전] Site 공통 파일 적용 (ex.HSText, 인증서 등)
	Information($"Copy [Site Unit] Common Files"); 

	CopyFiles($"{siteProfilePath}/HSText.xml", "./OpenNetLinkApp/wwwroot/conf");
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
		}
		
		CopyFile($"{storageUnit}/ReleaseNote.md", $"{ReleaseNoteDirPath}/{AppProps.PropVersion.ToString()}.md");				
		CopyFiles("./OpenNetLinkApp/VersionHash.txt", $"{AppProps.InstallerRootDirPath}/{unitName}");
		
		//storage의 OP 파일 published 경로에 적용 (+ OP 설정파일 암호화)
		DeleteFiles($"./artifacts/{AppProps.Platform}/published/wwwroot/conf/AppOPsetting_*.json");		
		CopyFiles($"{storageUnit}/AppOPsetting*.json", $"./artifacts/{AppProps.Platform}/published/wwwroot/conf");		
		
		if(isEnc.ToString().ToUpper() == "TRUE")
			RunTarget("EncryptConfig");

		//Delete Default SiteProfile
		if(DirectoryExists($"./artifacts/{AppProps.Platform}/published/wwwroot/SiteProfile"))		
			DeleteDirectory($"./artifacts/{AppProps.Platform}/published/wwwroot/SiteProfile", new DeleteDirectorySettings {Force = true, Recursive = true });
			
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
		Information($"Copy [Agent Unit] Files");

		//설치파일 생성
		if(isFull.ToString().ToUpper() == "TRUE")
		{
			foreach(var agentUnit in System.IO.Directory.GetDirectories(storageUnit))
			{
				var agentUnitInfo = new DirectoryInfo(agentUnit);
				string AgentName= agentUnitInfo.Name;
				
				if(AgentName.Substring(0, 1) == ".")
					continue;

				Information($"Make Agent Installer {AgentName}");

				networkFlag = AgentName.ToUpper();			
				storageName = unitName.ToUpper();
				
				if(AppProps.Platform == "windows")
				{
					JObject AppEnvJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath($"{agentUnit}/AppEnvSetting.json"));
					for(int i = 0; i < AppEnvJObj["strForwardUrl"].Count(); i++)
					{
						//AppEnvJObj["strForwardUrl"][i] = "file:\\\\\\C:\\HANSSAK\\OpenNetLink\\wwwroot\\Web\\WebLinkInfo.html";
						AppEnvJObj["strForwardUrl"][i] = "";
					}
					JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath($"{agentUnit}/AppEnvSetting.json"), AppEnvJObj);

				}
				else if(AppProps.Platform == "mac")
				{
					JObject AppEnvJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath($"{agentUnit}/AppEnvSetting.json"));
					for(int i = 0; i < AppEnvJObj["strForwardUrl"].Count(); i++)
					{
						AppEnvJObj["strForwardUrl"][i] = "file:/Applications/OpenNetLinkApp.app/Contents/MacOS/wwwroot/Web/WebLinkInfo.html";
					}
					JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath($"{agentUnit}/AppEnvSetting.json"), AppEnvJObj);
				}
				else
				{
					JObject AppEnvJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath($"{agentUnit}/AppEnvSetting.json"));
					for(int i = 0; i < AppEnvJObj["strForwardUrl"].Count(); i++)
					{
						AppEnvJObj["strForwardUrl"][i] = "file:/opt/hanssak/opennetlink/wwwroot/Web/WebLinkInfo.html";
					}
					JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath($"{agentUnit}/AppEnvSetting.json"), AppEnvJObj);
				}

				CopyFiles($"{agentUnit}/AppEnvSetting.json", $"./artifacts/{AppProps.AppUpdatePlatform}/published/wwwroot/conf");
				CopyFiles($"{agentUnit}/NetWork.json", $"./artifacts/{AppProps.AppUpdatePlatform}/published/wwwroot/conf");
				
				isPatchInstaller=false;
				RunTarget("MakeInstaller");		
			}
		}

		//패치파일 생성
		if(isPatch.ToString().ToUpper() == "TRUE")
		{
			//Light Patch 버전일 땐, edge 폴더 배포전에 제거
			if(isLightPatch.ToString().ToUpper().Equals("TRUE"))
			{
				if(DirectoryExists("./artifacts/windows/published/wwwroot/edge")) 
				{
					DeleteDirectory("./artifacts/windows/published/wwwroot/edge", new DeleteDirectorySettings { Force = true, Recursive = true });
				}
			}
			
			if(FileExists("./artifacts/windows/published/wwwroot/conf/NetWork.json"))
				DeleteFile("./artifacts/windows/published/wwwroot/conf/NetWork.json");			
			if(FileExists("./artifacts/windows/published/wwwroot/conf/AppEnvSetting.json"))
				DeleteFile("./artifacts/windows/published/wwwroot/conf/AppEnvSetting.json");
			
			isPatchInstaller=true;
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
					{"REG_CRX", regCrxForce.ToString().ToUpper()},
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
					{"REG_CRX", regCrxForce.ToString().ToUpper()},
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
					{"REG_CRX", regCrxForce.ToString().ToUpper()},
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
												.Append(storageName.ToUpper())//$6	
												.Append(regCrxForce.ToString().ToUpper())//$7
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
													.Append(storageName.ToUpper())//$6	
													})
		)
		{
			process.WaitForExit();
			Information("Package Redhat: Exit code: {0}", process.GetExitCode());
		}
	}
	// else if( AppProps.Platform == "mac" && (networkFlag == "IN"))
	else if( AppProps.Platform == "mac" )
	{
		using(var process = StartAndReturnProcess("./MacOSAppLayout/PkgAndNotarize.sh", new ProcessSettings
													{ Arguments = new ProcessArgumentBuilder()
													.Append(AppProps.PropVersion.ToString())
													.Append(isPatchInstaller.ToString().ToUpper())
													.Append(networkFlag.ToUpper()) 
													.Append(customName.ToUpper())
													.Append(PackageDirPath)	//$5 Output
													.Append(storageName.ToUpper())//$6
													.Append(regCrxForce.ToString().ToUpper())//$7														
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

