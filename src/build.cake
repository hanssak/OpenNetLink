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
	string PackageDirPath 	= String.Format("artifacts/packages/{0}/{1}", AppProps.AppEnvUpdatePlatform, AppProps.PropVersion.ToString());
	
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

	string title 			= String.Format("OpenNetLink v{0}", AppProps.PropVersion.ToString());
	string PackageDirPath 	= String.Format("artifacts/packages/{0}/{1}", AppProps.AppEnvUpdatePlatform, AppProps.PropVersion.ToString());
	string PackagePath 		= String.Format("{0}/{1}.md", PackageDirPath, AppProps.PropVersion.ToString());
	System.IO.Directory.CreateDirectory(PackageDirPath);

	// Write File
	using(StreamWriter writer = new StreamWriter(PackagePath)){
		writer.WriteLine("# "+title);
		writer.WriteLine("");
		foreach (var tag in AppProps.GitLastTag)
		{
			writer.WriteLine(tag.Message);
		}
	};
});

Task("Appcast")
    .IsDependentOn("CreateReleaseNote")
	.Does(() =>
{
	string url = String.Format("https://{0}/updatePlatform/{1}/{2}/", AppProps.AppEnvUpdateSvnIP, AppProps.AppEnvUpdatePlatform, AppProps.PropVersion.ToString());
	string GeneratorPath = String.Format("./Appcasts/Generator/SelfContain/{0}/generate_appcast", AppProps.AppEnvUpdatePlatform);
	string PackagePath = String.Format("artifacts/packages/{0}/{1}/", AppProps.AppEnvUpdatePlatform, AppProps.PropVersion.ToString());

	if(!DirectoryExists(PackagePath)) {
		throw new Exception(String.Format("[Error] Not Found Directory : {0}", PackagePath));
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
	
	Information("url: {0}\n GeneratorPath: {1} \n", url, GeneratorPath);

	using(var process = StartAndReturnProcess(GeneratorPath
						, new ProcessSettings { 
							Arguments = new ProcessArgumentBuilder()
											.Append("--product-name").AppendQuoted("opennetlink")
											.Append("--file-extract-version").AppendQuoted(AppProps.PropVersion.ToString())
											.Append("--appcast-output-directory").AppendQuoted(PackagePath)
											.Append("--os").AppendQuoted(strOS)
											.Append("--ext").AppendQuoted(strEXT)
											.Append("--key-path").AppendQuoted("Appcasts/Generator/keys")
											.Append("--binaries").AppendQuoted(PackagePath)
											.Append("--base-url").AppendQuoted(url)
											
											// * README: when creating --change-log-path 
											// 1. There must be a directory version before the package.
											// 2. The version directory and md file should be the same name. 
											.Append("--change-log-path").AppendQuoted(PackagePath)
											.Append("--change-log-url").AppendQuoted(url)
							}))
	{
		process.WaitForExit();
		Information("Exit code: {0}", process.GetExitCode());
	}
});


Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
