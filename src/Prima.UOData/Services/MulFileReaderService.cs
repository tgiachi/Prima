using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Config;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Mul;
using Server;

namespace Prima.UOData.Services;

public class MulFileReaderService : IMulFileReaderService
{
    private readonly ILogger _logger;

    private readonly PrimaServerConfig _primaServerConfig;

    private readonly IProcessQueueService _processQueueService;


    public MulFileReaderService(
        ILogger<MulFileReaderService> logger, PrimaServerConfig primaServerConfig, IProcessQueueService processQueueService
    )
    {
        _logger = logger;
        _primaServerConfig = primaServerConfig;
        _processQueueService = processQueueService;


        if (string.IsNullOrEmpty(_primaServerConfig.Shard.UoDirectory))
        {
            _logger.LogError("UoDirectory is not set in the config file.");
            throw new Exception("UoDirectory is not set in the config file.");
        }

        UoFiles.ScanForFiles(_primaServerConfig.Shard.UoDirectory);
    }


    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        Skills.Reload();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}
