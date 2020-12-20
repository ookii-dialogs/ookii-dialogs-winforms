#tool "nuget:?package=NuGet.CommandLine&version=5.8.0"
#addin "nuget:?package=Cake.MinVer&version=0.2.0"

var target       = Argument<string>("target", "pack");
var buildVersion = MinVer(s => s.WithTagPrefix("v").WithDefaultPreReleasePhase("preview"));

Task("clean")
    .Does(() =>
{
    CleanDirectory("./build/artifacts");
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
    CleanDirectories("./test/**/bin");
    CleanDirectories("./test/**/obj");
});

Task("restore")
    .IsDependentOn("clean")
    .Does(() =>
{
    NuGetRestore("./Ookii.Dialogs.WinForms.sln", new NuGetRestoreSettings
    {
        NoCache = true,
    });
});

Task("build")
    .IsDependentOn("restore")
    .Does(() =>
{
    var assemblyVersionInfoFile = FilePath.FromString("./src/Ookii.Dialogs.WinForms/Properties/AssemblyVersionInfo.cs");
    var assemblyVersionInfoBytes = System.IO.File.ReadAllBytes(assemblyVersionInfoFile.FullPath);

    try
    {
        CreateAssemblyInfo(assemblyVersionInfoFile, new AssemblyInfoSettings
        {
            Version = buildVersion.AssemblyVersion,
            FileVersion = buildVersion.FileVersion,
            InformationalVersion = buildVersion.PackageVersion,
        });

        MSBuild("./Ookii.Dialogs.WinForms.sln", new MSBuildSettings
        {
            Configuration = "Debug",
            ToolVersion = MSBuildToolVersion.VS2019,
        }.WithTarget("Rebuild"));

        MSBuild("./Ookii.Dialogs.WinForms.sln", new MSBuildSettings
        {
            Configuration = "Release",
            ToolVersion = MSBuildToolVersion.VS2019,
        }.WithTarget("Rebuild"));
    }
    finally
    {
        System.IO.File.WriteAllBytes(assemblyVersionInfoFile.FullPath, assemblyVersionInfoBytes);
    }
});

Task("pack")
    .IsDependentOn("build")
    .Does(() =>
{
    var releaseNotes = $"https://github.com/augustoproiete/ookii-dialogs-winforms/releases/tag/v{buildVersion.PackageVersion}";

    NuGetPack("./src/Ookii.Dialogs.WinForms/Ookii.Dialogs.WinForms.nuspec", new NuGetPackSettings
    {
        Version = buildVersion.PackageVersion,
        OutputDirectory = "./build/artifacts",
        ReleaseNotes = new[] { releaseNotes },
    });
});

Task("publish")
    .IsDependentOn("pack")
    .Does(context =>
{
    var url =  context.EnvironmentVariable("NUGET_URL");
    if (string.IsNullOrWhiteSpace(url))
    {
        context.Information("No NuGet URL specified. Skipping publishing of NuGet packages");
        return;
    }

    var apiKey =  context.EnvironmentVariable("NUGET_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        context.Information("No NuGet API key specified. Skipping publishing of NuGet packages");
        return;
    }

    var nugetPushSettings = new DotNetCoreNuGetPushSettings
    {
        Source = url,
        ApiKey = apiKey,
    };

    foreach (var nugetPackageFile in GetFiles("./build/artifacts/*.nupkg"))
    {
        DotNetCoreNuGetPush(nugetPackageFile.FullPath, nugetPushSettings);
    }
});

RunTarget(target);
