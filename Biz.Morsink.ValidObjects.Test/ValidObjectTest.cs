using System.Collections.Immutable;
namespace Biz.Morsink.ValidObjects.Test;


public class ValidObjectTest
{
    [Test]
    public void ValidationFailureTest()
    {
        var dto = new Address.Dto("Kalverstraat", "1", "11111AA", "Amsterdam");
        dto.TryCreate().Should().BeFailure().Which.Should().HaveCount(1)
            .And.Contain(e => e.Key.ToString() == nameof(Address.ZipCode));
    }
    [Test]
    public void ValidationTest()
    {
        var dto = new Address.Dto("Kalverstraat", "1", "1111AA", "Amsterdam");
        dto.TryCreate().Should().BeSuccess().Which.Should().Match<Address>(
            a => a.Street.Value == dto.Street
                && a.HouseNumber.Value == dto.HouseNumber
                && a.ZipCode.Value == dto.ZipCode
                && a.City.Value == dto.City);
    }
    [Test]
    public void NestedValidationFailureTest()
    {
        var dto = new Person.Dto("Pietje", "Puk", 44, 
            ImmutableList.Create(
                new Address.Dto("Kalverstraat", "1", "1111AA", "")));
        dto.TryCreate().Should().BeFailure().Which.Should().HaveCount(1)
            .And.Contain(e => e.Key.ToString().EndsWith(nameof(Address.City)));
    }
    [Test]
    public void NestedValidationTest()
    {
        var dto = new Person.Dto("Pietje", "Puk", 44,
            ImmutableList.Create(
                new Address.Dto("Kalverstraat", "1", "1111AA", "Amsterdam")));
        dto.TryCreate().Should().BeSuccess().Which.Should().Match<Person>(
            p => p.FirstName.Value == dto.FirstName
                && p.LastName.Value == dto.LastName
                && p.Age.Value == dto.Age
                && p.Addresses.Count == 1);
    }
}
