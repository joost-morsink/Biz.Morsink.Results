using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Biz.Morsink.ValidObjects;

public class ValidationCell<TVo, TDto> : IValidationCell<TVo, TDto>, INotifyPropertyChanged
    where TVo : class
{
    private readonly IObjectValidator<TVo, TDto> _validator;
    private Lazy<Result<TVo, ErrorList>> _validObject;
    private TDto _value;
    
    public ValidationCell(IObjectValidator<TVo, TDto> validator, TVo validObject)
    {
        _validator = validator;
        _value = default!; // GetLazyValid will set it
        _validObject = GetLazyValid(validObject);
    }

    public ValidationCell(IObjectValidator<TVo, TDto> validator, TDto value)
    {
        _validator = validator;
        _value = default!; // SetValue will set it
        SetValue(value, true);
        _validObject = GetLazyValidation();
    }

    private Result<TVo, ErrorList> ValidateNow()
    { 
        return _validObject.Value;
    }
    public bool IsValid => ValidateNow().IsSuccess;
    public ErrorList Errors => ValidateNow().Switch(x => default, x => x);
    public virtual TDto Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<TDto>.Default.Equals(_value, value))
                SetValue(value);
        }
    }

    protected void SetValue(TDto value, bool direct = false)
    {
        if(_value is INotifyPropertyChanged npc)
            npc.PropertyChanged -= OnValueChanged;
        
        if (direct)
            _value = value;
        else
            SetField(ref _value, value);
        
        if(value is INotifyPropertyChanged npc2)
            npc2.PropertyChanged += OnValueChanged;
    }

    protected virtual void OnValueChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Value));
    }
    
    [DisallowNull]
    public virtual TVo? ValidObject
    {
        get => ValidateNow().Switch(x => (TVo?) x, x => null);
        set
        {
            _validObject = GetLazyValid(value);
            OnPropertyChanged();
        }
    }

    protected void ResetValidationCheck()
    {
        if(_validObject.IsValueCreated)
            _validObject = GetLazyValidation();
    }

    private Lazy<Result<TVo, ErrorList>> GetLazyValidation()
        => new (() =>
        {
            return _validator.TryCreate(_value);
        });

    private Lazy<Result<TVo, ErrorList>> GetLazyValid(TVo validObject)
    {
        SetValue(_validator.GetDto(validObject));

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
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if(propertyName != nameof(ValidObject))
            ResetValidationCheck();
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}