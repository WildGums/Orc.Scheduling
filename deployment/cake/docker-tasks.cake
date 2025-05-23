#l "docker-variables.cake"

#addin "nuget:?package=Cake.Docker&version=1.3.0"

//-------------------------------------------------------------

public class DockerImagesProcessor : ProcessorBase
{
    public DockerImagesProcessor(BuildContext buildContext)
        : base(buildContext)
    {
        
    }

    public override bool HasItems()
    {
        return BuildContext.DockerImages.Items.Count > 0;
    }

    public string GetDockerRegistryUrl(string projectName)
    {
        // Allow per project overrides via "DockerRegistryUrlFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(BuildContext, projectName, "DockerRegistryUrlFor", BuildContext.DockerImages.DockerRegistryUrl);
    }

    public string GetDockerRegistryUserName(string projectName)
    {
        // Allow per project overrides via "DockerRegistryUserNameFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(BuildContext, projectName, "DockerRegistryUserNameFor", BuildContext.DockerImages.DockerRegistryUserName);
    }

    public string GetDockerRegistryPassword(string projectName)
    {
        // Allow per project overrides via "DockerRegistryPasswordFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(BuildContext, projectName, "DockerRegistryPasswordFor", BuildContext.DockerImages.DockerRegistryPassword);
    }

    private string GetDockerImageName(string projectName)
    {
        var name = projectName.Replace(".", "-");
        return name.ToLower();
    }

    private string GetDockerImageTag(string projectName, string version)
    {
        var dockerRegistryUrl = GetDockerRegistryUrl(projectName);

        var tag = string.Format("{0}/{1}:{2}", dockerRegistryUrl, GetDockerImageName(projectName), version);
        return tag.TrimStart(' ', '/').ToLower();
    }

    private string[] GetDockerImageTags(string projectName)
    {
        var dockerTags = new List<string>();

        var versions = new List<string>();

        versions.Add(BuildContext.General.Version.NuGet);

        foreach (var version in new [] 
                                {
                                    BuildContext.General.Version.MajorMinor,
                                    BuildContext.General.Version.Major
                                })
        {
            var additionalTag = version;

            if (BuildContext.General.IsAlphaBuild)
            {
                additionalTag += "-alpha";
            }

            if (BuildContext.General.IsBetaBuild)
            {
                additionalTag += "-beta";
            }

            versions.Add(additionalTag);
        }

        foreach (var version in versions)
        {
            dockerTags.Add(GetDockerImageTag(projectName, version));
        }

        if (BuildContext.General.IsAlphaBuild)
        {
            dockerTags.Add(GetDockerImageTag(projectName, "latest-alpha"));
        }

        if (BuildContext.General.IsBetaBuild)
        {
            dockerTags.Add(GetDockerImageTag(projectName, "latest-beta"));
        }

        if (BuildContext.General.IsOfficialBuild)
        {
            dockerTags.Add(GetDockerImageTag(projectName, "latest-stable"));
            dockerTags.Add(GetDockerImageTag(projectName, "latest"));
        }

        return dockerTags.ToArray();
    }

    private void ConfigureDockerSettings(AutoToolSettings dockerSettings)
    {
        var engineUrl = BuildContext.DockerImages.DockerEngineUrl;
        if (!string.IsNullOrWhiteSpace(engineUrl))
        {
            CakeContext.Information("Using remote docker engine: '{0}'", engineUrl);

            dockerSettings.ArgumentCustomization = args => args.Prepend($"-H {engineUrl}");
            //dockerSettings.BuildArg = new [] { $"DOCKER_HOST={engineUrl}" };
        }
    }

    public override async Task PrepareAsync()
    {
        if (!HasItems())
        {
            return;
        }

        // Check whether projects should be processed, `.ToList()` 
        // is required to prevent issues with foreach
        foreach (var dockerImage in BuildContext.DockerImages.Items.ToList())
        {
            foreach (var imageTag in GetDockerImageTags(dockerImage))
            {
                CakeContext.Information(imageTag);
            }

            if (!ShouldProcessProject(BuildContext, dockerImage))
            {
                BuildContext.DockerImages.Items.Remove(dockerImage);
            }
        }        
    }

    public override async Task UpdateInfoAsync()
    {
        if (!HasItems())
        {
            return;
        }

        // Doesn't seem neccessary yet
        // foreach (var dockerImage in BuildContext.DockerImages.Items)
        // {
        //     Information("Updating version for docker image '{0}'", dockerImage);

        //     var projectFileName = GetProjectFileName(BuildContext, dockerImage);

        //     TransformConfig(projectFileName, new TransformationCollection 
        //     {
        //         { "Project/PropertyGroup/PackageVersion", VersionNuGet }
        //     });
        // }        
    }

    public override async Task BuildAsync()
    {
        if (!HasItems())
        {
            return;
        }
        
        foreach (var dockerImage in BuildContext.DockerImages.Items)
        {
            BuildContext.CakeContext.LogSeparator("Building docker image '{0}'", dockerImage);

            var projectFileName = GetProjectFileName(BuildContext, dockerImage);
            
            var msBuildSettings = new MSBuildSettings 
            {
                Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
                ToolVersion = MSBuildToolVersion.Default,
                Configuration = BuildContext.General.Solution.ConfigurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            ConfigureMsBuild(BuildContext, msBuildSettings, dockerImage, "build");

            // Always disable SourceLink
            msBuildSettings.WithProperty("EnableSourceLink", "false");

            RunMsBuild(BuildContext, dockerImage, projectFileName, msBuildSettings, "build");
        }        
    }

    public override async Task PackageAsync()
    {
        if (!HasItems())
        {
            return;
        }

        // The following directories are being created, ready for docker images to be used:
        // ./output => output of the publish step
        // ./config => docker image and config files, in case they need to be packed as well

        foreach (var dockerImage in BuildContext.DockerImages.Items)
        {
            if (!ShouldPackageProject(BuildContext, dockerImage))
            {
                CakeContext.Information("Docker image '{0}' should not be packaged", dockerImage);
                continue;
            }

            BuildContext.CakeContext.LogSeparator("Packaging docker image '{0}'", dockerImage);

            var projectFileName = GetProjectFileName(BuildContext, dockerImage);
            var dockerImageSpecificationDirectory = System.IO.Path.Combine(".", "deployment", "docker", dockerImage);
            var dockerImageSpecificationFileName = System.IO.Path.Combine(dockerImageSpecificationDirectory, dockerImage);

            var outputRootDirectory = System.IO.Path.Combine(BuildContext.General.OutputRootDirectory, dockerImage, "output");

            CakeContext.Information("1) Preparing ./config for package '{0}'", dockerImage);

            // ./config
            var confTargetDirectory = System.IO.Path.Combine(outputRootDirectory, "conf");
            CakeContext.Information("Conf directory: '{0}'", confTargetDirectory);

            CakeContext.CreateDirectory(confTargetDirectory);

            var confSourceDirectory = string.Format("{0}/*", dockerImageSpecificationDirectory);
            CakeContext.Information("Copying files from '{0}' => '{1}'", confSourceDirectory, confTargetDirectory);

            CakeContext.CopyFiles(confSourceDirectory, confTargetDirectory, true);

            BuildContext.CakeContext.LogSeparator();

            CakeContext.Information("2) Preparing ./output using 'dotnet publish' for package '{0}'", dockerImage);

            // ./output
            var outputDirectory = System.IO.Path.Combine(outputRootDirectory, "output");
            CakeContext.Information("Output directory: '{0}'", outputDirectory);

            var msBuildSettings = new DotNetMSBuildSettings();

            ConfigureMsBuildForDotNet(BuildContext, msBuildSettings, dockerImage, "pack");

            msBuildSettings.WithProperty("ConfigurationName", BuildContext.General.Solution.ConfigurationName);
            msBuildSettings.WithProperty("PackageVersion", BuildContext.General.Version.NuGet);

            // Disable code analyses, we experienced publish issues with mvc .net core projects
            msBuildSettings.WithProperty("RunCodeAnalysis", "false");

            var publishSettings = new DotNetPublishSettings
            {
                MSBuildSettings = msBuildSettings,
                OutputDirectory = outputDirectory,
                Configuration = BuildContext.General.Solution.ConfigurationName,
                //NoBuild = true
            };

            CakeContext.DotNetPublish(projectFileName, publishSettings);

            BuildContext.CakeContext.LogSeparator();

            CakeContext.Information("3) Using 'docker build' to package '{0}'", dockerImage);

            // docker build ..\..\output\Release\platform -f .\Dockerfile

            // From the docs (https://docs.microsoft.com/en-us/azure/app-service/containers/tutorial-custom-docker-image#use-a-docker-image-from-any-private-registry-optional), 
            // we need something like this:
            // docker tag <azure-container-registry-name>.azurecr.io/mydockerimage
            var dockerRegistryUrl = GetDockerRegistryUrl(dockerImage);

            // Note: to prevent all output & source files to be copied to the docker context, we will set the
            // output directory as context (to keep the footprint as small as possible)

            var dockerSettings = new DockerImageBuildSettings
            {
                NoCache = true, // Don't use cache, always make sure to fetch the right images
                File = dockerImageSpecificationFileName,
                //Platform = "linux",
                Tag = GetDockerImageTags(dockerImage)
            };

            ConfigureDockerSettings(dockerSettings);

            CakeContext.Information("Docker files source directory: '{0}'", outputRootDirectory);

            CakeContext.DockerBuild(dockerSettings, outputRootDirectory);

            BuildContext.CakeContext.LogSeparator();
        }        
    }

    public override async Task DeployAsync()
    {
        if (!HasItems())
        {
            return;
        }

        foreach (var dockerImage in BuildContext.DockerImages.Items)
        {
            if (!ShouldDeployProject(BuildContext, dockerImage))
            {
                CakeContext.Information("Docker image '{0}' should not be deployed", dockerImage);
                continue;
            }

            BuildContext.CakeContext.LogSeparator("Deploying docker image '{0}'", dockerImage);

            var dockerRegistryUrl = GetDockerRegistryUrl(dockerImage);
            var dockerRegistryUserName = GetDockerRegistryUserName(dockerImage);
            var dockerRegistryPassword = GetDockerRegistryPassword(dockerImage);
            var dockerImageName = GetDockerImageName(dockerImage);

            if (string.IsNullOrWhiteSpace(dockerRegistryUrl))
            {
                throw new Exception("Docker registry url is empty, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to some default public registry");
            }

            // Note: we are logging in each time because the registry might be different per container
            CakeContext.Information("Logging in to docker @ '{0}'", dockerRegistryUrl);

            var dockerLoginSettings = new DockerRegistryLoginSettings
            {
                Username = dockerRegistryUserName,
                Password = dockerRegistryPassword
            };

            ConfigureDockerSettings(dockerLoginSettings);

            CakeContext.DockerLogin(dockerLoginSettings, dockerRegistryUrl);

            try
            {
                foreach (var dockerImageTag in GetDockerImageTags(dockerImage))
                {
                    CakeContext.Information("Pushing docker images with tag '{0}' to '{1}'", dockerImageTag, dockerRegistryUrl);

                    var dockerImagePushSettings = new DockerImagePushSettings
                    {
                    };

                    ConfigureDockerSettings(dockerImagePushSettings);

                    CakeContext.DockerPush(dockerImagePushSettings, dockerImageTag);

                    await BuildContext.Notifications.NotifyAsync(dockerImage, string.Format("Deployed to Docker"), TargetType.DockerImage);
                }
            }
            finally
            {
                CakeContext.Information("Logging out of docker @ '{0}'", dockerRegistryUrl);

                var dockerLogoutSettings = new DockerRegistryLogoutSettings
                {
                };

                ConfigureDockerSettings(dockerLogoutSettings);

                CakeContext.DockerLogout(dockerLogoutSettings, dockerRegistryUrl);
            }
        }        
    }

    public override async Task FinalizeAsync()
    {

    }
}
