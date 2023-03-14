using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;

namespace Biz.Morsink.ValidObjects.Mutable;

public abstract class AbstractMutable<T, D> : ValidationCell<T, D>, INotifyPropertyChanged
    where T : class, IValidObject<T, D>
    where D : class, IDto<T, D>
{

    protected AbstractMutable(T validObject, Action<D, D>? updateDto = null) : base(ObjectValidator.For<T,D>(), validObject)
    {
        _updateDto = updateDto ?? ((_,_) => { });
    }

    protected AbstractMutable(D value, Action<D, D>? updateDto = null) : base(ObjectValidator.For<T,D>(), value)
    {
        _updateDto = updateDto ?? ((_,_) => { });
    }

    private Action<D, D> _updateDto;
    
    public override D Value
    {
        get => base.Value;
        set
        {
            if (!ReferenceEquals(base.Value, value))
            {
                var old = base.Value;
                base.Value = value;
                _updateDto(old, value);
            }
        }
    }
}