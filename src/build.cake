///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Version")
    .Does(() => {
	var propsFile = "./OpenNetLinkApp/Directory.Build.props";
	var readedVersion = XmlPeek(propsFile, "//Version");
	var currentVersion = new Version(readedVersion);

	var semVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build + 1, currentVersion.Revision);
	var version = semVersion.ToString();

	XmlPoke(propsFile, "//Version", version);
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
        DotNetCoreBuild("./OpenNetLinkApp/OpenNetLinkApp.csproj");
});

Task("Debug")
    .IsDependentOn("Version")
    .Does(() => {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = "Debug",
		};
        DotNetCoreBuild("./OpenNetLinkApp/OpenNetLinkApp.csproj", settings);
});

Task("Release")
    .IsDependentOn("Version")
    .Does(() => {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = "Release",
		};
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