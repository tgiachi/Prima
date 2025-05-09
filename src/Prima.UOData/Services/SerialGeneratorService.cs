using Microsoft.Extensions.Logging;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Services;

namespace Prima.UOData.Services;

public class SerialGeneratorService : ISerialGeneratorService
{
    private readonly ILogger _logger;

    public SerialGeneratorService(ILogger<SerialGeneratorService> logger)
    {
        _logger = logger;
    }

    public Serial GenerateSerial<TEntity>()
    {
        throw new NotImplementedException();
    }

    public void GenerateSerial<TEntity>(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
