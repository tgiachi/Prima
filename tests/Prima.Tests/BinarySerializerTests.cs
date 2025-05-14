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
    private string _fileName;
    private IPersistenceManager _persistenceManager;

    private const int MaxSize = 1_000_000;

    [OneTimeSetUp]
    public void Setup()
    {
        _fileName =
            Path.Combine("/tmp/", "mobiles.bin");

        if (File.Exists(_fileName))
        {
            File.Delete(_fileName);
        }

        _persistenceManager = new PersistenceManager(NullLogger<PersistenceManager>.Instance);
        _persistenceManager.RegisterEntitySerializer(new BinaryItemSerializer());
        _persistenceManager.RegisterEntitySerializer(new BinaryMobileSerializer());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        if (File.Exists(_fileName))
        {
            File.Delete(_fileName);
        }
    }

    [Test]
    public async Task CreateFile_Test()
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

        await _persistenceManager.SaveToFileAsync(listOfMobile, _fileName);
    }


    [Test]
    public async Task ReadFile_Test()
    {
        var listOfMobile = await _persistenceManager.DeserializeAsync<MobileEntity>(_fileName);

        Assert.That(listOfMobile, Is.Not.Null);
        Assert.That(listOfMobile.Count, Is.EqualTo(MaxSize));
        Assert.That(listOfMobile[0].Id, Is.EqualTo(Serial.Parse("1")));
        Assert.That(listOfMobile[0].Name, Is.EqualTo("TEST"));
        Assert.That(listOfMobile[0].IsPlayer, Is.EqualTo(true));
        Assert.That(listOfMobile[0].Hue, Is.EqualTo(20));
        Assert.That(listOfMobile[0].Position, Is.EqualTo(new Point3D(10, 10, 10)));
    }
}
