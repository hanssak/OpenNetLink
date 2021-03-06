///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Git&version=0.22.0
#addin nuget:?package=Cake.Json&version=5.2.0
#addin nuget:?package=Newtonsoft.Json&version=11.0.2

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var sitename = Argument("sitename", "hanssak");
var configuration = Argument("configuration", "Release");

var AppProps = new AppProperty(Context, 
								"./OpenNetLinkApp/Directory.Build.props", 				// Property file path of the build directory
								"../", 													// Path of the Git Local Repository
								"./OpenNetLinkApp/wwwroot/conf/AppEnvSetting.json");	// Env file Path of the Application env settings

string PackageDirPath 		= String.Format("artifacts/installer/{0}/packages", AppProps.AppEnvUpdatePlatform);
string ReleaseNoteDirPath 	= String.Format("artifacts/installer/{0}/release_note", AppProps.AppEnvUpdatePlatform);
string PackageZipFile 		= String.Format("OpenNetLink-{0}-{1}.hz", AppProps.AppEnvUpdatePlatform, AppProps.PropVersion.ToString());
///////////////////////////////////////////////////////////////////////////////
// CLASSES
///////////////////////////////////////////////////////////////////////////////
public class AppProperty
{
    ICakeContext Context { get; }
	public string PropsFile { get; }
	public string GitRepoPath { get; }
	public string AppEnvFile { get; }
	private JObject AppEnvJObj { get; }
    public AppProperty(ICakeContext context, string propsFile, string gitRepoPath, string appEnvFile)
    {
        Context = context;
		PropsFile = propsFile;
		GitRepoPath = gitRepoPath;
		AppEnvFile = appEnvFile;
		AppEnvJObj = JsonAliases.ParseJsonFromFile(Context, new FilePath(AppEnvFile));
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

	public string AppEnvSWVersion {
		get {
			return AppEnvJObj["SWVersion"].ToString();
		}
		set {
			AppEnvJObj["SWVersion"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(AppEnvFile), AppEnvJObj);
		}
	}
	public string AppEnvUpdateSvnIP {
		get {
			return AppEnvJObj["UpdateSvcIP"].ToString();
		}
	}
	public string AppEnvSWCommitId {
		get {
			return AppEnvJObj["SWCommitId"].ToString();
		}
		set {
			AppEnvJObj["SWCommitId"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(AppEnvFile), AppEnvJObj);
		}
	}
	public string AppEnvUpdatePlatform {
		get {
			return AppEnvJObj["UpdatePlatform"].ToString();
		}
		set {
			AppEnvJObj["UpdatePlatform"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(AppEnvFile), AppEnvJObj);
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
	var CurAppEnvSWVersion = AppProps.AppEnvSWVersion;
	var Commit = AppProps.GitLastCommit;
	var ShaId = AppProps.GitLastShaIdPretty;

	AppProps.PropVersion = semVersion;
	AppProps.PropCommitId = ShaId;
	AppProps.AppEnvSWVersion = semVersion.ToString();
	AppProps.AppEnvSWCommitId = ShaId;

	Information($"- Last Commit Log: [{Commit.ToString()}]");
	Information($"- CommitId: [{AppProps.PropCommitId}]");
	Information($"- Version: [{currentVersion} -> {AppProps.PropVersion}]");
	Information($"- AppEnv SWCommitId: [{AppProps.AppEnvSWCommitId}]");
	Information($"- AppEnv SWVersion: [{CurAppEnvSWVersion} -> {AppProps.AppEnvSWVersion}]");
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

Task("PubDebian")
    .IsDependentOn("Version")
    .Does(() => {

	AppProps.AppEnvUpdatePlatform = "debian";
	PackageDirPath 	= String.Format("artifacts/installer/{0}/packages", AppProps.AppEnvUpdatePlatform);
	var settings = new DotNetCorePublishSettings {
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "linux-x64",
		OutputDirectory = "./artifacts/debian/published"
	};
	
	if(DirectoryExists(settings.OutputDirectory)) {
		DeleteDirectory(settings.OutputDirectory, new DeleteDirectorySettings { 
		Force = true, Recursive = true });
	}

    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);
    DotNetCorePublish("./ContextTransferClient", settings);
});

Task("PkgDebian")
    .IsDependentOn("PubDebian")
    .Does(() => {

	using(var process = StartAndReturnProcess("./PkgDebian.sh", new ProcessSettings{ Arguments = AppProps.PropVersion.ToString() }))
	{
		process.WaitForExit();
		Information("Package Debin: Exit code: {0}", process.GetExitCode());
	}
});

Task("PubWin10")
    .IsDependentOn("Version")
    .Does(() => {

	AppProps.AppEnvUpdatePlatform = "windows";
	PackageDirPath 	= String.Format("artifacts/installer/{0}/packages", AppProps.AppEnvUpdatePlatform);
	var settings = new DotNetCorePublishSettings
	{
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "win10-x64",
		OutputDirectory = "./artifacts/windows/published"
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

});

Task("PkgWin10")
    .IsDependentOn("PubWin10")
    .Does(() => {
	if(DirectoryExists(PackageDirPath)) {
		DeleteDirectory(PackageDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
	}
	System.IO.Directory.CreateDirectory(PackageDirPath);
	
	MakeNSIS("./OpenNetLink.nsi", new MakeNSISSettings {
		Defines = new Dictionary<string, string>{
			{"PRODUCT_VERSION", AppProps.PropVersion.ToString()}
		}
	});

});

Task("PubOSX")
    .IsDependentOn("Version")
    .Does(() => {
	AppProps.AppEnvUpdatePlatform = "mac";
	PackageDirPath 		= String.Format("artifacts/installer/{0}/packages", AppProps.AppEnvUpdatePlatform);
	var settings = new DotNetCorePublishSettings {
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "osx-x64",
		OutputDirectory = "./artifacts/osx/published"
	};
    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);
});

Task("PkgOSX")
    .IsDependentOn("PubOSX")
    .Does(() => {

	if(DirectoryExists(PackageDirPath)) {
		DeleteDirectory(PackageDirPath, new DeleteDirectorySettings { Force = true, Recursive = true });
	}
	System.IO.Directory.CreateDirectory(PackageDirPath);

	using(var process = StartAndReturnProcess("./MacOSAppLayout/PkgAndNotarize.sh", new ProcessSettings{ Arguments = AppProps.PropVersion.ToString() }))
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
		writer.WriteLine("# "+Title);
		writer.WriteLine("");
		foreach (var tag in AppProps.GitLastTag)
		{
			writer.WriteLine(tag.Message);
		}
	};
});

Task("Appcast")
	.IsDependentOn("Install-NetSparkleUpdater.Tools.AppCastGenerator")
    .IsDependentOn("CreateReleaseNote")
	.Does(() =>
{
	string InstallerOsPath	= String.Format("artifacts/installer/{0}", AppProps.AppEnvUpdatePlatform);
	string PackagesURL 		= String.Format("https://{0}/updatePlatform/{1}/packages/", AppProps.AppEnvUpdateSvnIP, AppProps.AppEnvUpdatePlatform);
	string ReleaseNoteURL 	= String.Format("https://{0}/updatePlatform/{1}/release_note/", AppProps.AppEnvUpdateSvnIP, AppProps.AppEnvUpdatePlatform);

	if(!DirectoryExists(PackageDirPath)) {
		throw new Exception(String.Format("[Error] Not Found Directory : {0}", PackageDirPath));
	}

	// default OS
	string strEXT 	= "exe";
	string strOS 	= "windows";
	if(AppProps.AppEnvUpdatePlatform.Equals("mac")) { 
		strOS 	= "mac";
		strEXT 	= "pkg";
	}
	if(AppProps.AppEnvUpdatePlatform.Equals("debian")) { 
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
	if(AppProps.AppEnvUpdatePlatform.Equals("mac")) { 
		PackagePath = String.Format("{0}/OpenNetLinkApp-{1}.pkg", PackageDirPath, AppProps.PropVersion.ToString());
	}
	else if(AppProps.AppEnvUpdatePlatform.Equals("debian")) { 
		PackagePath = String.Format("{0}/opennetlink_{1}_amd64.deb", PackageDirPath, AppProps.PropVersion.ToString());
	}
	else if(AppProps.AppEnvUpdatePlatform.Equals("windows")) { 
		PackagePath = String.Format("{0}/OpenNetLinkSetup_v{1}.exe", PackageDirPath, AppProps.PropVersion.ToString());
	}
	else {
		throw new Exception(String.Format("[Err] Not Support Platform : {0}", AppProps.AppEnvUpdatePlatform));
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
