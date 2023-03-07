namespace Biz.Morsink.ValidObjects;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public class ValidObjectAttribute : Attribute
{
    public bool CellDtos { get; set; }
}