
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


//var target = Argument("target", "Default");
//Task("Default")
//	.Does(() => {
//		DotNetCoreBuild("../OpenNetLink.sln");
//	});
//RunTarget(target);

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
		//	foreach (var item in TagNameList)
		//	{
		//		System.Console.WriteLine(item);	
		//	}
			return TagNameList[TagNameList.Count -1].ToString();
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

	public string AppEnvSWCommitId {
		get {
			return AppEnvJObj["SWCommitId"].ToString();
		}
		set {
			AppEnvJObj["SWCommitId"] = value;
			JsonAliases.SerializeJsonToPrettyFile<JObject>(Context, new FilePath(AppEnvFile), AppEnvJObj);
		}
	}
}

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

Task("PubWin10")
    .IsDependentOn("Version")
    .Does(() => {

	var settings = new DotNetCorePublishSettings
	{
		Framework = "net5.0",
		Configuration = "Release",
		Runtime = "win10-x64",
		OutputDirectory = "./artifacts/win10/published"
	};

    DotNetCorePublish("./OpenNetLinkApp", settings);
    DotNetCorePublish("./PreviewUtil", settings);
    DotNetCorePublish("./ContextTransferClient", settings);
});

Task("PkgWin10")
    .IsDependentOn("Version")
    .Does(() => {
	MakeNSIS("./OpenNetLink.nsi");
});

Task("Default")
    //.IsDependentOn("PubWin10");
	//.IsDependentOn("PkgWin10");
	.Does(() => {
		MakeNSIS("./OpenNetLink.nsi");
});
RunTarget(target);