using Microsoft.Extensions.Logging.Abstractions;
using Orion.Core.Server.Data.Directories;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Types;
using Prima.Server.Services;
using Prima.UOData.Entities;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Services;

namespace Prima.Tests;

[TestFixture]
public class WorldManagerServiceTests
{
    private IWorldManagerService _worldManagerService;

    [SetUp]
    public void Setup()
    {
        var tmpDirectory = Path.GetTempPath();

        _worldManagerService = new WorldManagerService(
            new NullLogger<WorldManagerService>(),
            new PersistenceManager(new NullLogger<PersistenceManager>()),
            new DirectoriesConfig(tmpDirectory, Enum.GetNames<DirectoryType>()),
            new PrimaServerConfig()
        );
    }


    [Test]
    public void TestAddMobiles()
    {
        var mobile = _worldManagerService.GenerateWorldEntity<MobileEntity>();
        var mobile2 = _worldManagerService.GenerateWorldEntity<MobileEntity>();
        var mobile3 = _worldManagerService.GenerateWorldEntity<MobileEntity>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(mobile, Is.Not.Null);
            Assert.That(mobile2, Is.Not.Null);
            Assert.That(mobile3, Is.Not.Null);
        }
    }

    [Test]
    public void TestAddItems()
    {
        var item = _worldManagerService.GenerateWorldEntity<ItemEntity>();
        var item2 = _worldManagerService.GenerateWorldEntity<ItemEntity>();
        var item3 = _worldManagerService.GenerateWorldEntity<ItemEntity>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(item.Id, Is.GreaterThanOrEqualTo(Serial.ItemOffset));
            Assert.That(item2.Id, Is.GreaterThanOrEqualTo(Serial.ItemOffset));
            Assert.That(item3.Id, Is.GreaterThanOrEqualTo(Serial.ItemOffset));
        }
    }
}
