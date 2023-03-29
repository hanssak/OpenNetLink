///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Git&version=1.1.0
#addin nuget:?package=Cake.Json&version=6.0.1
#addin nuget:?package=Newtonsoft.Json&version=13.0.1
#addin nuget:?package=Cake.Prompt&version=1.0.15
#addin nuget:?package=Cake.FileHelpers&version=4.0.0
//#addin nuget:?package=Cake.Core=3.0.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var sitename = Argument("sitename", "hanssak");
var configuration = Argument("configuration", "Release");
var setNetwork = Argument<bool>("setNetwork", true);
var isPatch = Argument<bool>("isPatch", false);
var isLightPatch = Argument<bool>("isLightPatch", false);
var networkFlag = "NONE"; //NONE일 경우 패키지명에 networkflag는 비어진 상태로 나타남
var customName = "KRX";
var customFileUiName = "K-Link";	// site에서 요구하는 파일이름(OpenNetLink => K-Link)
var isWithSilence = Argument<bool>("isWithSilence", false);
var AppProps = new AppProperty(Context, 
								"./OpenNetLinkApp/Directory.Build.props", 				// Property file path of the build directory
								"../", 													// Path of the Git Local Repository
								"./OpenNetLinkApp/wwwroot/conf/AppVersion.json",		// Version file Path 
								"./OpenNetLinkApp/wwwroot/conf/AppEnvSetting.json",		// Env file Path of the Application env settings
								"./OpenNetLinkApp/wwwroot/conf/NetWork.json",			// Network file Path of the Network settings
								"./openNetLinkApp/ReleaseNote.md");						// Release Note of Patch File

string PackageDirPath 		= String.Format("artifacts/installer/{0}/packages", AppProps.AppUpdatePlatform);
string ReleaseNoteDirPath 	= String.Format("artifacts/installer/{0}/release_note", AppProps.AppUpdatePlatform);
string PackageZipFile 		= String.Format("OpenNetLink-{0}-{1}.hz", AppProps.AppUpdatePlatform, AppProps.PropVersion.ToString());
///////////////////////////////////////////////////////////////////////////////
// CLASSES
///////////////////////////////////////////////////////////////////////////////
public class AppProperty
{
    ICakeContext Context { get; }
	public string PropsFile { get; }
	public string GitRepoPath { get; }
	public string VersionFile { get; }
	public string AppEnvFile { get; }
	public string NetworkFile { get; }
	public string ReleaseNoteFile {get;}
	private JObject VersionJObj { get; }
	private JObject AppEnvJObj { get; }
	private JObject NetworkJobj { get; }
	
    public AppProperty(ICakeContext context, string propsFile, string gitRepoPath, string versionFile, string appEnvFile, string networkFile, string releaseNoteFile)
    {
        Context = context;
		PropsFile = propsFile;
		GitRepoPath = gitRepoPath;
		VersionFile = versionFile;
		AppEnvFile = appEnvFile;
		NetworkFile = networkFile;
		ReleaseNoteFile = releaseNoteFile;
		VersionJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(VersionFile));
		AppEnvJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(AppEnvFile));
		NetworkJobj = JsonAliases.ParseJsonFromFile(Context, new FilePath(NetworkFile));
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
			return AppEnvJObj["UpdateSvcIP"].ToString();
		}
		set {
			AppEnvJObj["UpdateSvcIP"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(AppEnvFile), AppEnvJObj);
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
	public string AppEnvUpdatePort {
		get {
			return "3439";
		}
	}
	public string NetworkFromName {
		get {
			return NetworkJobj["NETWORKS"][0]["FROMNAME"].ToString();
		}
		set {
			NetworkJobj["NETWORKS"][0]["FROMNAME"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
		}
	}

	public string NetworkToName {
		get {
			return NetworkJobj["NETWORKS"][0]["TONAME"].ToString();
		}
		set {
			NetworkJobj["NETWORKS"][0]["TONAME"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
		}
	}

	public string NetworkIPAddress {
		get {
			return NetworkJobj["NETWORKS"][0]["IPADDRESS"].ToString();
		}
		set {
			NetworkJobj["NETWORKS"][0]["IPADDRESS"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
		}
	}

	public string NetworkPort {
		get {
			return NetworkJobj["NETWORKS"][0]["PORT"].ToString();
		}		
	}

	public string NetworkPos {
		get {
			return NetworkJobj["NETWORKS"][0]["NETPOS"].ToString();
		}	
		set {
			NetworkJobj["NETWORKS"][0]["NETPOS"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(NetworkFile), NetworkJobj);
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

	var currentVersion = AppProps.PropVersion;
	//var semVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build, currentVersion.Revision + 1);
	var semVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build, currentVersion.Revision);
	var CurAppEnvSWVersion = AppProps.AppSWVersion;
	var Commit = AppProps.GitLastCommit;
	var ShaId = AppProps.GitLastShaIdPretty;

	AppProps.PropVersion = semVersion;
	AppProps.PropCommitId = ShaId;
	AppProps.AppSWVersion = semVersion.ToString();
	AppProps.AppSWCommitId = ShaId;

	Information($"- Last Commit Log: [{Commit.ToString()}]");
	Information($"- CommitId: [{AppProps.PropCommitId}]");
	Information($"- Version: [{currentVersion} -> {AppProps.PropVersion}]");
	Information($"- AppEnv SWCommitId: [{AppProps.AppSWCommitId}]");
	Information($"- AppEnv SWVersion: [{CurAppEnvSWVersion} -> {AppProps.AppSWVersion}]");
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
	if(DirectoryExists(PackageDirPath)) {
    	DeleteDirectory(PackageDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
		Information($"- Delete: [{PackageDirPath}]");
	}

	if(DirectoryExists(ReleaseNoteDirPath)) {
 		DeleteDirectory(ReleaseNoteDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
		Information($"- Delete: [{ReleaseNoteDirPath}]");
	}

	if(FileExists(PackageZipFile)) { 
 		DeleteFile(PackageZipFile);
		Information($"- Delete: [{PackageZipFile}]");
	}
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

Task("SetFileName")
	//patch가 아닐 경우만 실행
	.WithCriteria(!isPatch)
	.Does(()=>{
		//customName = Prompt("Custom Name : ");
		//customFileUiName = Prompt("Site Require File Name : ");	// site에서요구한 이름 입력받아처리할때 사용
		networkFlag = Prompt("Network Flag (IN/CN/EX/NCI) : ");
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
    
     using(var process = StartAndReturnProcess("./HashTool/MD5HashUtility.exe"))
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
	Information("5");
	
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
    
	if(networkFlag.ToString().ToUpper().Equals("ALL"))
	{
	
		var Folders = GetDirectories("./json/*");
		//var dirPath = Directory("./json");
		//var subDirs = dirPath.GetDirectories();
		foreach(var folderA in Folders)
		{
			String strSearchFolder = (String)folderA.FullPath;
			String strNetPosFolderName = "";
			int nIdex = strSearchFolder.LastIndexOf('/');
			if (nIdex > 0)
			{
				//Information("Folder-Name : GetNetPos!!!");
				strNetPosFolderName = strSearchFolder.Substring(nIdex+1);
			}			
			
			Information("NetPos/FolderName : {0}, FullPath: {1}", strNetPosFolderName, strSearchFolder);
			strSearchFolder += "/*.json";
			
			// NetPos 별로 json 파일 복사 및 nsis make 동작
			var jsonfiles = GetFiles(strSearchFolder);
			foreach(var file in jsonfiles)
			{
				String strSrcFile = (String)file.FullPath;
				String strTargetFile = "./artifacts/windows/published/wwwroot/conf/" + file.GetFilename();
				Information("Json-File-Copy, Src  : {0}, Dest : {1}", strSrcFile, strTargetFile);

				DeleteFile(strTargetFile);
				CopyFile(strSrcFile, strTargetFile);
				
				// json 파일복사 확인
				//Console.ReadKey();
			}			
			
			MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
				Defines = new Dictionary<string, string>{
					{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
					{"IS_PATCH", isPatch.ToString().ToUpper()},
					{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
					{"NETWORK_FLAG", strNetPosFolderName.ToUpper()},
					{"CUSTOM_NAME", customName.ToUpper()},					
					{"CUSTOM_FILE_NAME", customFileUiName.ToUpper()},
					{"IS_WITH_SILENCE", "FALSE"}
				}
			});
			Information("NetPos : {0}, NSIS-Make-Done !!!", strNetPosFolderName);
			
			if(isWithSilence.ToString().ToUpper().Equals("TRUE"))
			{
				MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
					Defines = new Dictionary<string, string>{
						{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
						{"IS_PATCH", isPatch.ToString().ToUpper()},
						{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
						{"NETWORK_FLAG", strNetPosFolderName.ToUpper()},					
						{"CUSTOM_NAME", customName.ToUpper()},	
						{"CUSTOM_FILE_NAME", customFileUiName.ToUpper()},						
						{"IS_WITH_SILENCE", isWithSilence.ToString().ToUpper()}
					}
				});
				Information("NetPos : {0}, NSIS(silence)-Make-Done !!!", strNetPosFolderName);				
			}

			// 설치파일 생성확인
			// Console.ReadKey();
			
		}

		/*int nIdex = strSearchFile.LastIndexOf('.');
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
		}*/
		
	}
	else
	{
	
		MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
			Defines = new Dictionary<string, string>{
				{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
				{"IS_PATCH", isPatch.ToString().ToUpper()},
				{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
				{"NETWORK_FLAG", networkFlag.ToUpper()},
				{"CUSTOM_NAME", customName.ToUpper()}			
			}
		});
		
		if(isWithSilence.ToString().ToUpper().Equals("TRUE"))
		{
			MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
				Defines = new Dictionary<string, string>{
					{"PRODUCT_VERSION", AppProps.PropVersion.ToString()},
					{"IS_PATCH", isPatch.ToString().ToUpper()},
					{"IS_LIGHT_PATCH", isLightPatch.ToString().ToUpper()},						
					{"NETWORK_FLAG", networkFlag.ToUpper()},
					{"CUSTOM_NAME", customName.ToUpper()},			
					{"IS_WITH_SILENCE", isWithSilence.ToString().ToUpper()}
				}
			});	
		}	
	
	}
	
	
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
							}))
	{
		process.WaitForExit();
	}
});


Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
