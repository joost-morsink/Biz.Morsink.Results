using System.Collections.Immutable;
using System.Diagnostics.Tracing;
namespace Biz.Morsink.ValidObjects.Test;

public class ValidObjectTest
{
    private readonly Person.Dto validPersonDto = new ()
    {
        FirstName = "Pietje", LastName = "Puk", Age = 44,
        Addresses = ImmutableList.Create(new Address.Dto
        {
            Street = "Kalverstraat",
            HouseNumber = "1",
            ZipCode = "1111AA",
            City = "Amsterdam"
        })
    };
    [Test]
    public void ValidationFailureTest()
    {
        var dto = validPersonDto.Addresses[0] with { ZipCode = "11111AA" };
        dto.TryCreate().Should().BeFailure().Which.Should().HaveCount(1)
            .And.Contain(e => e.Key.ToString() == nameof(Address.ZipCode));
    }
    [Test]
    public void ValidationTest()
    {
        var dto = validPersonDto.Addresses[0];
        dto.TryCreate().Should().BeSuccess().Which.Should().Match<Address>(
            a => a.Street.Value == dto.Street
                && a.HouseNumber.Value == dto.HouseNumber
                && a.ZipCode.Value == dto.ZipCode
                && a.City.Value == dto.City);
    }
    [Test]
    public void TestTest()
    {
        var dto = validPersonDto with { Tags = validPersonDto.Tags.Add("") };
        dto.TryCreate().Should().BeFailure();
    }
    [Test]
    public void NestedValidationFailureTest()
    {
        var dto = validPersonDto with
        {
            Addresses = validPersonDto.Addresses.SetItem(0, validPersonDto.Addresses[0] with { City = "" })
        };
        dto.TryCreate().Should().BeFailure().Which.Should().HaveCount(1)
            .And.Contain(e => e.Key.ToString().EndsWith(nameof(Address.City)));
    }
    [Test]
    public void NestedValidationTest()
    {
        var dto = validPersonDto;
        var person = dto.TryCreate();
        person.Should().BeSuccess().Which.Should().Match<Person>(
            p => p.FirstName.Value == dto.FirstName
                && p.LastName.Value == dto.LastName
                && p.Age.Value == dto.Age
                && p.Addresses.Count == 1);
        var address= person.Select(p => p.Addresses[0]);
        address.Should().BeSuccess().Which.Should().Match<Address>(a =>
            !ReferenceEquals(a.GetDto(), dto.Addresses[0])
            && a.GetDto().Equals(dto.Addresses[0]));
    }
}
