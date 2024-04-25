var data = new Signal<List<People>>();

var adults = SignalBuilder.DependsOn(data)
                          .ComputedBy(value => value.Get().Where(p => p.Age > 18).ToList());

var numberOfAdults = SignalBuilder.DependsOn(adults)
                                  .ComputedBy(value => value.Get().Count);

numberOfAdults.HasEffect((oldCount, newCount) => 
    Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));

data.Set([new People("David", 48)]);
Console.WriteLine(adults.Get().Count);

data.Set([new People("David", 48), new People("Rebecca",48)]);
data.Set([new People("David", 48), new People("Rebecca",48)]);

Console.WriteLine(adults.Get().Count);
Console.WriteLine(numberOfAdults.Get());


public interface ISignal<T>
{
    public T Get();
    void HasEffect(Action<T, T> action);
}


public abstract class BaseSignal
{
    protected List<BaseSignal> _children = [];

    protected bool _isDirty = false;
    
    public void AddChild(BaseSignal signal)
    {
        _children.Add(signal);
    }

    protected virtual void MarkAsDirty()
    {
        if (!_isDirty)
        {
            _isDirty = true;
            foreach (var child in _children)
                child.MarkAsDirty();
        }
    }
    
}


public class Signal<T> : BaseSignal, ISignal<T>
{
    private T _value = default!;
    private Action<T,T> _action;

    public void Set(T value)
    {
        if (_value is null || !_value.Equals(value))
        {
            MarkAsDirty();
            _value = value;
            _isDirty = false;
        }
    }
    
    public T Get()
    {
        return _value;
    }

    public void HasEffect(Action<T, T> action)
    {
        _action = action;
    }
}

public class ReadOnlySignal1<T, T1> : BaseSignal, ISignal<T>
{
    private T _value;
    private readonly ISignal<T1> _counter1;
    private readonly Func<ISignal<T1>, T> _func;
    private Action<T, T>? _action;

    public ReadOnlySignal1(ISignal<T1> counter1, Func<ISignal<T1>, T> func)
    {
        _counter1 = counter1;
        _func = func;

        ((BaseSignal)_counter1).AddChild(this);

        _isDirty = true;
    }
    
    public T Get()
    {
        if (_isDirty)
        {
            _value = _func(_counter1);
            _isDirty = false;
        }

        return _value;
    }
    
    protected override void MarkAsDirty()
    {
        base.MarkAsDirty();
        if (_action != null)
        {
            var currentValue = _value;
            var newValue = Get();
            if (currentValue is null || !currentValue.Equals(newValue))
            {
                _action(currentValue,newValue);
            }
        }
    }
    
    public void HasEffect(Action<T, T> action)
    {
        _action = action;
    }
}

public static class SignalBuilder
{
    public static ReadOnlySignalBuilder1<T1> DependsOn<T1>(ISignal<T1> counter1)
    {
        return new ReadOnlySignalBuilder1<T1>(counter1);
    }
}

public class ReadOnlySignalBuilder1<T1>(ISignal<T1> counter1)
{
    public ISignal<TResult> ComputedBy<TResult>(Func<ISignal<T1>, TResult> func)
    {
        return new ReadOnlySignal1<TResult, T1>(counter1, func);
    }
}

record People(string Name, int Age);

