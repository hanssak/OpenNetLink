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
	var semVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build + 1, currentVersion.Revision);
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

	var settings = new DotNetCorePublishSettings
	{
		Framework = "netcoreapp3.1",
		Configuration = "Release",
		Runtime = "linux-x64",
		OutputDirectory = "./OpenNetLinkApp/artifacts/"
	};

    DotNetCorePublish("./OpenNetLinkApp", settings);
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);