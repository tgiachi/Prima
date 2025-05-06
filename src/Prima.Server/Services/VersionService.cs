using System.Reflection;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Options;

namespace Prima.Server.Services;

public class VersionService : IVersionService
{
    private readonly AppContextData<PrimaServerOptions, PrimaServerConfig> _appContextData;
    private readonly ITextTemplateService _templateService;

    public VersionService(AppContextData<PrimaServerOptions, PrimaServerConfig> appContextData, ITextTemplateService templateService)
    {
        _appContextData = appContextData;
        _templateService = templateService;

        var versionInfo = GetVersionInfo();

        _templateService.AddVariable("version", versionInfo.Version);
        _templateService.AddVariable("codename", versionInfo.CodeName);
        _templateService.AddVariable("commit", versionInfo.GitHash);
        _templateService.AddVariable("branch", versionInfo.Branch);
        _templateService.AddVariable("commit_date", versionInfo.BuildDate);


    }

    public VersionInfoData GetVersionInfo()
    {
        var version = typeof(VersionService).Assembly.GetName().Version;

        var codename = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "Codename")
            ?.Value;

        return new VersionInfoData(
            _appContextData.AppName,
            codename ?? "Unknown",
            version.ToString(),
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch,
            ThisAssembly.Git.CommitDate
        );
    }
}
