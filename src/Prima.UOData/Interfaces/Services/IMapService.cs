using Orion.Core.Server.Interfaces.Services.Base;
using Prima.UOData.Data.Map;

namespace Prima.UOData.Interfaces.Services;

public interface IMapService : IOrionService, IOrionStartService
{
    public CityInfo[] GetAvailableStartingCities();
}
