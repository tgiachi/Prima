using Prima.UOData.Id;

namespace Prima.Tests;

[TestFixture]
public class SerialTests
{

    [Test]
    public void TestGeneratedSerials()
    {
        var newSerial = Serial.RandomSerial();

        Assert.That(newSerial.IsValid, Is.True);
    }

}
