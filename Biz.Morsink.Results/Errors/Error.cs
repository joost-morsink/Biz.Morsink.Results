namespace Biz.Morsink.Results.Errors;

public readonly struct Error
{
    public Error(ErrorKey key, string? code, IErrorMessage message)
    {
        Key = key;
        Code = code;
        Message = message;
    }
    public Error(ErrorKey key, string? code, ErrorMessage message)
    {
        Key = key;
        Code = code;
        Message = message;
    }
    public ErrorKey Key { get; }
    public string? Code { get; }
    public IErrorMessage Message { get; }

    public Error Prefix(object prefix)
        => new (Key.Prefix(prefix), Code, Message);
    public Error Prefix(params object[] prefix)
        => new (Key.Prefix(prefix), Code, Message);

    public override string ToString()
        => ToString(CultureInfo.CurrentCulture);
    public string ToString(CultureInfo culture)
        => Code == null
            ? $"{Key}: {Message.GetMessage(culture)}"
            : $"[{Code}] {Key}: {Message.GetMessage(culture)}";
}
