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


public interface ISignal
{
    public void MarkAsSuspect();
    
    public int Version { get; } 
}
public interface IComputeSignal : ISignal
{
    void EnsureNodeIsComputed();
}

public abstract class BaseSignal<T> : ISignal
{
    // Who depends on this signal
    private List<IComputeSignal> _children = [];

    // Value the last time this signal was calculated
    protected T Value = default!;
    
    // How many times has the value of this signal changed
    public int Version { get; set; } 
    
    // We are suspect when a node somewhere above us in the graph has changed
    protected bool IsSuspect = true;
  
    // Optional method to call when the value of this signal changes
    protected Action<T,T>? Effect;

    public abstract T Get();

    public void HasEffect(Action<T, T> effect)
    {
        Effect = effect;
    }
    
    public void AddChild(IComputeSignal signal)
    {
        _children.Add(signal);
    }

    public void MarkAsSuspect()
    {
        if (!IsSuspect)
        {
            IsSuspect = true;
            foreach (var child in _children)
                child.MarkAsSuspect();
        }
    }
}


public class Signal<T> : BaseSignal<T>
{
    public void Set(T value)
    {
        if (Value is null || !Value.Equals(value))
        {
            // Top level signal has changed, so we need to mark all children as suspect
            MarkAsSuspect();

            // Update our value and bump the version
            var oldValue = Value;
            Value = value;
            IsSuspect = false;
            Version++;
            
            Effect?.Invoke(oldValue, value);
        }
    }
   
    public override T Get()
    {
        // Top level signal, so we know it's always correct
        return Value;
    }
}

public abstract class ReadOnlySignal<T> : BaseSignal<T>, IComputeSignal
{

    protected class SignalWithVersion(ISignal signal, int version)
    {
        public int Version { get; set; } = version;
        public ISignal Signal { get; set; } = signal;
    }

    // Who do we depend on?
    protected List<SignalWithVersion> Parents = [];
    
    protected void AddParent(ISignal signal)
    {
        Parents.Add(new SignalWithVersion(signal, signal.Version));
    }
    
    protected abstract void Compute();
    
    public override T Get()
    {
        EnsureNodeIsComputed();
        return Value;
    }
    
    public void EnsureNodeIsComputed()
    {
        if (!IsSuspect) return;  // All good
        
        // Make sure our parents are good
        foreach (var parent in Parents)
        {
            if (parent.Signal is IComputeSignal computeSignal)
            {
                computeSignal.EnsureNodeIsComputed();
            }
        }

        // If any of our parents have changed,  then we need to update
        var updateNeeded = false;
        foreach (var parent in Parents)
        {
            if (parent.Version != parent.Signal.Version)
            {
                updateNeeded = true;
                parent.Version = parent.Signal.Version;
            }
        }

        if (updateNeeded)
        {
            var oldValue = Value;
            
            Compute();

            if (oldValue == null || !oldValue.Equals(Value))
                Version++;
        }
        
        IsSuspect = false;
    }
    
}

public class ReadOnlySignal1<T, T1> : ReadOnlySignal<T>
{
    private readonly Func<BaseSignal<T1>, T> _func;

    public ReadOnlySignal1(BaseSignal<T1> counter1, Func<BaseSignal<T1>, T> func)
    {
        _func = func;

        counter1.AddChild(this);
        AddParent(counter1);

        IsSuspect = true;
    }
    
    protected override void Compute()
    {
        Value = _func((BaseSignal<T1>)Parents[0].Signal);
    }
}

public static class SignalBuilder
{
    public static ReadOnlySignalBuilder1<T1> DependsOn<T1>(BaseSignal<T1> counter1)
    {
        return new ReadOnlySignalBuilder1<T1>(counter1);
    }
}

public class ReadOnlySignalBuilder1<T1>(BaseSignal<T1> counter1)
{
    public BaseSignal<TResult> ComputedBy<TResult>(Func<BaseSignal<T1>, TResult> func)
    {
        return new ReadOnlySignal1<TResult, T1>(counter1, func);
    }
}

record People(string Name, int Age);

