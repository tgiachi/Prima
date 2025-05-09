using Prima.UOData.Id;

namespace Prima.UOData.Interfaces.Services;

public interface ISerialGeneratorService
{
    Serial GenerateSerial<TEntity>();

    void GenerateSerial<TEntity>(TEntity entity);

}
