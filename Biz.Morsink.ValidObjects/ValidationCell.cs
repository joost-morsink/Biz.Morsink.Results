using System.Diagnostics.CodeAnalysis;

namespace Biz.Morsink.ValidObjects;

public class ValidationCell<TVo, TDto> : IHasStateVersion, IValidationCell<TVo, TDto> where TVo : class
{
    private readonly IObjectValidator<TVo, TDto> _validator;
    private Lazy<Result<TVo, ErrorList>> _validObject;
    private TDto _value;
    private StateValue _lastEvaluatedStateVersion = StateValue.Zero;
    private readonly StateVersion _stateVersion = new();
    
    public ValidationCell(IObjectValidator<TVo, TDto> validator, TVo validObject)
    {
        _validator = validator;
        _value = validator.GetDto(validObject);
        _validObject = GetLazyValid(validObject);
    }

    public ValidationCell(IObjectValidator<TVo, TDto> validator, TDto value)
    {
        _validator = validator;
        _value = value;
        _validObject = GetLazyValidation();
    }

    private Result<TVo,ErrorList> ValidateNow()
    { 
        if(_validObject.IsValueCreated && _lastEvaluatedStateVersion != GetStateVersion())
            _validObject = GetLazyValidation();
        return _validObject.Value;
    }
    public bool IsValid => ValidateNow().IsSuccess;
    public ErrorList Errors => ValidateNow().Switch(x => default, x => x);
    public virtual TDto Value
    {
        get => _value;
        set  {
            if(!ReferenceEquals(_value, value))
            {
                _value = value;
                ResetValidationCheck();
            }
        }
    }
    
    [DisallowNull]
    public virtual TVo? ValidObject
    {
        get => ValidateNow().Switch(x => (TVo?) x, x => null);
        set 
        {             
            _validObject = new Lazy<Result<TVo, ErrorList>>(() => value);
            var _ = _validObject.Value;
            Value = _validator.GetDto(value);
        }
    }

    protected void ResetValidationCheck()
    {
        _stateVersion.Next();
        if(_validObject.IsValueCreated)
            _validObject = GetLazyValidation();
    }

    private Lazy<Result<TVo, ErrorList>> GetLazyValidation()
        => new (() =>
        {
            _lastEvaluatedStateVersion = GetStateVersion();
            return _validator.TryCreate(_value);
        });

    private Lazy<Result<TVo, ErrorList>> GetLazyValid(TVo validObject)
    {
        _value = _validator.GetDto(validObject);
        _stateVersion.Next();
        _lastEvaluatedStateVersion = GetStateVersion();
        var res = new Lazy<Result<TVo, ErrorList>>(() => validObject);
        var _ = res.Value;
        return res;
    }
    
    public static implicit operator TDto(ValidationCell<TVo, TDto> cell)
        => cell.Value;

    public static implicit operator TVo?(ValidationCell<TVo, TDto> cell)
        => cell.ValidObject;

    public Result<TVo, ErrorList> AsResult()
        => ValidateNow();
    
    public virtual StateValue GetStateVersion()
        => _stateVersion.Value;
}