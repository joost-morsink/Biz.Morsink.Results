namespace Biz.Morsink.ValidObjects;

[AttributeUsage(AttributeTargets.Method)]
public class ValidationMethodAttribute : Attribute
{
    public ValidationMethodAttribute() { }
    public ValidationMethodAttribute(string message) { }
}
