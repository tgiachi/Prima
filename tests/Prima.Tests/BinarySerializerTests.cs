using Microsoft.Extensions.Logging.Abstractions;
using Prima.Server.Services;
using Prima.UOData.Data.Geometry;
using Prima.UOData.Entities;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Serializers.Binary;
using Prima.UOData.Types;

namespace Prima.Tests;

[TestFixture]
public class BinarySerializerTests
{
    private string _mobileFileName;
    private string _itemFileName;
    private IPersistenceManager _persistenceManager;

    private const int MaxSize = 10_000;

    [OneTimeSetUp]
    public void Setup()
    {
        _mobileFileName =
            Path.Combine("/tmp/", "mobiles.bin");

        _itemFileName =
            Path.Combine("/tmp/", "items.bin");

        if (File.Exists(_itemFileName))
        {
            File.Delete(_itemFileName);
        }

        if (File.Exists(_mobileFileName))
        {
            File.Delete(_mobileFileName);
        }

        _persistenceManager = new PersistenceManager(NullLogger<PersistenceManager>.Instance);
        _persistenceManager.RegisterEntitySerializer(new BinaryItemSerializer());
        _persistenceManager.RegisterEntitySerializer(new BinaryMobileSerializer());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        if (File.Exists(_mobileFileName))
        {
            File.Delete(_mobileFileName);
        }
    }

    [Test]
    public async Task CreateFileMobile_Test()
    {
        var listOfMobile = new List<MobileEntity>();

        for (int i = 0; i < MaxSize; i++)
        {
            var serial = Serial.Parse("1");
            var mobile = new MobileEntity
            {
                Id = serial,
                Name = $"TEST",
                IsPlayer = true,
                Hue = 20,
                Position = new Point3D(10, 10, 10),
                Direction = Direction.North
            };
            listOfMobile.Add(mobile);
        }

        await _persistenceManager.SaveToFileAsync(listOfMobile, _mobileFileName);
    }


    [Test]
    public async Task CreateFileItem_Test()
    {
        var listOfItem = new List<ItemEntity>();

        for (int i = 0; i < MaxSize; i++)
        {
            var serial = Serial.Parse("1");
            var item = new ItemEntity
            {
                Id = serial,
                Name = $"TEST",
                Hue = 20,
                Position = new Point3D(10, 10, 10)
            };
            listOfItem.Add(item);
        }

        await _persistenceManager.SaveToFileAsync(listOfItem, _itemFileName);
    }


    [Test]
    public async Task ReadFileMobile_Test()
    {
        var listOfMobile = await _persistenceManager.DeserializeAsync<MobileEntity>(_mobileFileName);

        Assert.That(listOfMobile, Is.Not.Null);
        Assert.That(listOfMobile.Count, Is.EqualTo(MaxSize));
        Assert.That(listOfMobile[0].Id, Is.EqualTo(Serial.Parse("1")));
        Assert.That(listOfMobile[0].Name, Is.EqualTo("TEST"));
        Assert.That(listOfMobile[0].IsPlayer, Is.EqualTo(true));
        Assert.That(listOfMobile[0].Hue, Is.EqualTo(20));
        Assert.That(listOfMobile[0].Position, Is.EqualTo(new Point3D(10, 10, 10)));
    }

    [Test]
    public async Task ReadFileItem_Test()
    {
        var listOfItem = await _persistenceManager.DeserializeAsync<ItemEntity>(_itemFileName);

        Assert.That(listOfItem, Is.Not.Null);
        Assert.That(listOfItem.Count, Is.EqualTo(MaxSize));
        Assert.That(listOfItem[0].Id, Is.EqualTo(Serial.Parse("1")));
        Assert.That(listOfItem[0].Name, Is.EqualTo("TEST"));
        Assert.That(listOfItem[0].Hue, Is.EqualTo(20));
        Assert.That(listOfItem[0].Position, Is.EqualTo(new Point3D(10, 10, 10)));
    }
}
