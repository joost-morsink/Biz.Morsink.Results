using System.Globalization;

namespace Biz.Morsink.Results.Errors
{
    public readonly struct Error
    {
#if NET6_0_OR_GREATER
        public Error(ErrorKey key, string? code, IErrorMessage message)
#else
        public Error(ErrorKey key, string code, IErrorMessage message)
#endif
        {
            Key = key;
            Code = code;
            Message = message;
        }
        
#if NET6_0_OR_GREATER
        public Error(ErrorKey key, string? code, ErrorMessage message)
#else
        public Error(ErrorKey key, string code, ErrorMessage message)
#endif
        {
            Key = key;
            Code = code;
            Message = message;
        }
        public ErrorKey Key { get; }
#if NET6_0_OR_GREATER
        public string? Code { get; }
#else
        public string Code { get; }
#endif
        public IErrorMessage Message { get; }

        public Error Prefix(object prefix)
            => new Error(Key.Prefix(prefix), Code, Message);
        public Error Prefix(params object[] prefix)
            => new Error(Key.Prefix(prefix), Code, Message);

        public override string ToString()
            => ToString(CultureInfo.CurrentCulture);
        public string ToString(CultureInfo culture)
            => Code == null
                ? $"{Key}: {Message.GetMessage(culture)}"
                : $"[{Code}] {Key}: {Message.GetMessage(culture)}";
    }
}
