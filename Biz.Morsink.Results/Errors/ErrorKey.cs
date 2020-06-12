﻿using System.Collections.Immutable;
using System.Linq;

namespace Biz.Morsink.Results.Errors
{
    public struct ErrorKey
    {
        private readonly ImmutableStack<object> _parts;
        public ImmutableStack<object> Parts => _parts ?? ImmutableStack<object>.Empty;
        private ErrorKey(ImmutableStack<object> parts)
        {
            _parts = parts;
        }
        public ErrorKey(params object[] parts)
        {
            var p = ImmutableStack<object>.Empty;
            for (int i = parts.Length - 1; i >= 0; i--)
                p = p.Push(parts[i]);
            _parts = p;
        }
        public ErrorKey(object part)
        {
            _parts = ImmutableStack.Create(part);
        }
        public ErrorKey Prefix(params object[] parts)
        {
            var p = Parts;
            for (int i = parts.Length - 1; i >= 0; i--)
                p = p.Push(parts[i]);
            return new ErrorKey(p);
        }
        public ErrorKey Prefix(object part)
            => new ErrorKey(Parts.Push(part));

        public override string ToString()
            => string.Join(".", Parts.Select(PartToString));

        private static string PartToString(object part)
        {
            return part is IErrorKeyProvider ekp ? ekp.ErrorKey : part.ToString();
        }
        public static implicit operator ErrorKey(string key)
            => new ErrorKey(key);
    }
}
