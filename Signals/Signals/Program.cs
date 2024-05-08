
// There are two types of signals.  Top level signals can be assigned values.  Here we create one
// called counter which we originally a assign a value of 4
var counter = new Signal<int>(4);

// this can be changed to a five etc.
counter.Set(5);

// and read back
Console.WriteLine(counter.Get());   // prints 5



// The other type of signals are computed ones,  which are based on other signals
var isEven = SignalBuilder.DependsOn(counter).ComputedBy(counterSignal => counterSignal.Get() % 2 == 0);

// The javascript version looks a bit nicer,  but it's harder to work out what this counter depends on.
//var isEven = SignalBuilder.ComputedBy(() => counter.Get() % 2 == 0);

Console.WriteLine(isEven.Get());   // prints false as counter is 5

counter.Set(6);
Console.WriteLine(isEven.Get());   // prints true as counter is 6



// Computed signals can also be based on other computed signals
var parity = SignalBuilder.DependsOn(isEven).ComputedBy(isEvenSignal => isEvenSignal.Get() ? "Even" : "Odd");
Console.WriteLine(parity.Get());   // prints Even as isEven is true

counter.Set(9);
Console.WriteLine(parity.Get());   // prints odd as isEven is now false


// or on multiple signals
var firstName = new Signal<string>("David");
var surname = new Signal<string>("Betteridge");
var fullname = SignalBuilder.DependsOn(firstName, surname).ComputedBy((f,l) => $"{f.Get()} {l.Get()}");

Console.WriteLine(fullname.Get());

// Effects can be added to signals which are automatically triggered when a value changes
counter.AddEffect((previous, current) => Console.WriteLine($"Counter changed {previous} to {current}"));
isEven.AddEffect((previous, current) => Console.WriteLine($"IsEven changed {previous} to {current}"));
parity.AddEffect((previous, current) => Console.WriteLine($"Parity changed {previous} to {current}"));

counter.Set(10);
// Counter changed 9 to 10
// IsEven changed False to True 
// Parity changed Odd to Even

counter.Set(12);
// Counter changed 10 to 12


// Not only aren't the other two effect triggered, but also parity isn't even computed.
parity = SignalBuilder.DependsOn(isEven).ComputedBy(isEvenSignal =>
{
    Console.WriteLine("Compute parity");
    return isEvenSignal.Get() ? "Even" : "Odd";
});

// This changes the value for isEven so parity is computed
counter.Set(13);
Console.WriteLine(parity.Get());

// This doesn't change the value for isEven so parity isn't computed
counter.Set(15);
Console.WriteLine(parity.Get());

Console.WriteLine("Done");






bool CompareOrderedLists<T>(IList<T> lhs, IList<T> rhs) where T : notnull
{
    if (lhs.Count != rhs.Count)
        return false;

    for (var i = 0; i < lhs.Count; i++)
    {
        if (!lhs[i].Equals(rhs[i]))
            return false;
    }

    return true;
}

var data = new Signal<List<People>>([new People("David", 48)]).UsingEquality(CompareOrderedLists); 

var adults = SignalBuilder.DependsOn(data)
                          .ComputedBy(value => value.Get().Where(p => p.Age > 18).ToList())
                          .UsingEquality(CompareOrderedLists);

var numberOfAdults = SignalBuilder.DependsOn(adults)
                                  .ComputedBy(value =>
                                  {
                                      Console.WriteLine("Counting adults");
                                      return value.Get().Count;
                                  });

numberOfAdults.AddEffect((oldCount, newCount) => 
    Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));

Console.WriteLine(adults.Get().Count);

data.Set([new People("David", 48), new People("Rebecca",48)]);
data.Set([new People("David", 48), new People("Rebecca",48)]);

Console.WriteLine(adults.Get().Count);
Console.WriteLine(numberOfAdults.Get());

data.Set([new People("David", 48), new People("Rebecca",48)]);


record People(string Name, int Age);

// Computed Signals - Expression
// BaseSignal
//
// public record Effect<T>(Action<T, T> Action);
//
// public abstract class BaseSignal<T>
// {
//     protected T Value;
//     private List<Effect<T>> _effects = new();
//     
//     public abstract T Get();
//     
//     public Effect<T> AddEffect(Action<T, T> action)
//     {
//         var newEffect = new Effect<T>(action);
//         _effects.Add(newEffect);
//         return newEffect;
//     }
//
//     public bool RemoveEffect(Effect<T> existingEffect)
//     {
//         return _effects.Remove(existingEffect);
//     }
//
//     protected void FireEffects(T previous)
//     {
//         foreach (var effect in _effects)
//         {
//             effect.Action(previous, Value);
//         }
//     }
//     
// }
//
// public class Signal<T> : BaseSignal<T>
// {
//     public override T Get()
//     {
//         return Value;
//     }
//
//     /// <returns>The previous value of the signal</returns>
//     public T Set(T newValue)
//     {
//         var previous = Value;
//         Value = newValue;
//         if (previous is null || !previous.Equals(Value))
//             FireEffects(previous);
//         return previous;
//     }
// }
//
// public class ComputedSignal<T> : BaseSignal<T>
// {
//     public ComputedSignal(BaseSignal<T1> depends1, Func<BaseSignal<T1>,T> fn)
//     {
//         
//     }
// }






















//
//
// var counter = new Signal<int>(4);
// counter.AddEffect((previous, current) => Console.WriteLine($"Changed from {previous} to {current}"));
//
//
// var isEven = SignalBuilder.DependsOn(counter).ComputedBy(v => v.Get() % 2 == 0);
// isEven.AddEffect((previous, current) => Console.WriteLine($"isEven Changed from {previous} to {current}"));
//
//
//
//
//
// var parity = SignalBuilder.DependsOn(isEven).ComputedBy(v =>
// {dd
//     Console.WriteLine("Computed");
//     return v.Get() ? "Even" : "Odd";
// });
//
// counter.Set(6);
// Console.WriteLine(parity.Get());
//
// counter.Set(7);
// Console.WriteLine(parity.Get());
//
// counter.Set(9);
// Console.WriteLine(parity.Get());
//
//
//
//
//
//






// var counter = new Signal<int>(0);
// counter.HasEffect((from,to) => Console.WriteLine($"Counter changed from {from} to {to}."));
// var isEven = SignalBuilder.DependsOn(counter).ComputedBy(c => c.Get() % 2 == 0);
//
// Console.WriteLine(counter.Get());
// Console.WriteLine(isEven.Get());
//
// var parity = SignalBuilder.DependsOn(isEven).ComputedBy(i =>
// {
//     Console.WriteLine("Computing parity");
//     return i.Get() ? "Even" : "Odd";
// });
// parity.HasEffect((from,to) => Console.WriteLine($"Parity changed from {from} to {to}."));
//
// counter.Set(counter.Get() + 1);
// counter.Set(counter.Get() + 1);
// counter.Set(counter.Get() + 2);
// counter.Set(counter.Get() + 4);
// counter.Set(counter.Get() + 1);

public interface ISignal
{
    public void MarkAsSuspect();
    
    public int Version { get; } 
}
public interface IComputeSignal : ISignal
{
    void EnsureNodeIsComputed();
    void FireEffects();
}

public abstract class BaseSignal<T> : ISignal
{
    // Who depends on this signal
    protected readonly List<IComputeSignal> Children = [];

    // Value the last time this signal was calculated
    protected T Value = default!;
    
    // How many times has the value of this signal changed
    public int Version { get; set; } 
    
    // We are suspect when a node somewhere above us in the graph has changed
    protected bool IsSuspect = true;
  
    // Optional method to call when the value of this signal changes
    protected Action<T,T>? Effect;
    protected Func<T, T, bool>? _comparer;

    public abstract T Get();

    public void AddEffect(Action<T, T> effect)
    {
        Effect = effect;
    }
    
    public void AddChild(IComputeSignal signal)
    {
        Children.Add(signal);
    }

    public void MarkAsSuspect()
    {
        if (!IsSuspect)
        {
            IsSuspect = true;
            foreach (var child in Children)
                child.MarkAsSuspect();
        }
    }
}


public class Signal<T> : BaseSignal<T>
{
    public Signal(T initialValue)
    {
        Value = initialValue;
        IsSuspect = false;
    }
    public void Set(T value)
    {
        var changed = _comparer is null ? (Value is null || !Value.Equals(value)) : !_comparer(Value, value);
        
        if (changed)
        {
            // Top level signal has changed, so we need to mark all children as suspect
            MarkAsSuspect();

            // Update our value and bump the version
            var oldValue = Value;
            Value = value;
            IsSuspect = false;
            Version++;
            
            Effect?.Invoke(oldValue, value);
            
            foreach (var child in Children)
                child.FireEffects();
        }
    }
   
    public override T Get()
    {
        // Top level signal, so we know it's always correct
        return Value;
    }
    
    public Signal<T> UsingEquality(Func<T, T, bool> comparer)
    {
        _comparer = comparer;
        return this;
    }
}

public abstract class ComputedSignal<T> : BaseSignal<T>, IComputeSignal
{

    protected class SignalWithVersion(ISignal signal, int version)
    {
        public int Version { get; set; } = version;
        public ISignal Signal { get; } = signal;
    }

    // Who do we depend on?
    protected List<SignalWithVersion> Parents = [];
    
    protected void AddParent(ISignal signal)
    {
        Parents.Add(new SignalWithVersion(signal, -1));
    }
    
    protected abstract void Compute();
    
    public override T Get()
    {
        EnsureNodeIsComputed();
        return Value;
    }
    
    public void FireEffects()
    {
        if (Effect != null)
        {
            var oldValue = Value;
            EnsureNodeIsComputed();
            
            var changed = _comparer is null ? (Value is null || !Value.Equals(oldValue)) : !_comparer(Value, oldValue);
            if (changed)
            {
                Effect(oldValue, Value);
            }
            else
            {
                // We didn't change,  so don't need to examine our children
                return;
            }
        }
        // We have changed,  so maybe our children have as well
        foreach (var child in Children)
        {
            child.FireEffects();
        }
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
            var changed = _comparer is null ? (Value is null || !Value.Equals(oldValue)) : !_comparer(Value, oldValue);
            if (changed)
                Version++;
        }
        
        IsSuspect = false;
    }
    
    public BaseSignal<T> UsingEquality(Func<T, T, bool> comparer)
    {
        _comparer = comparer;
        return this;
    }
}

public class ComputedSignal1<T, T1> : ComputedSignal<T>
{
    private readonly Func<BaseSignal<T1>, T> _func;

    public ComputedSignal1(BaseSignal<T1> counter1, Func<BaseSignal<T1>, T> func)
    {
        _func = func;

        counter1.AddChild(this);
        AddParent(counter1);

        EnsureNodeIsComputed();
    }
    
    protected override void Compute()
    {
        Value = _func((BaseSignal<T1>)Parents[0].Signal);
    }
}


public class ComputedSignal2<T, T1, T2> : ComputedSignal<T>
{
    private readonly Func<BaseSignal<T1>, BaseSignal<T2>, T> _func;

    public ComputedSignal2(BaseSignal<T1> counter1, BaseSignal<T2> counter2, Func<BaseSignal<T1>, BaseSignal<T2>, T> func)
    {
        _func = func;

        counter1.AddChild(this);
        AddParent(counter1);

        counter2.AddChild(this);
        AddParent(counter2);
        
        EnsureNodeIsComputed();
    }
    
    protected override void Compute()
    {
        Value = _func((BaseSignal<T1>)Parents[0].Signal, (BaseSignal<T2>)Parents[1].Signal);
    }
}

public static class SignalBuilder
{
    public static ReadOnlySignalBuilder1<T1> DependsOn<T1>(BaseSignal<T1> counter1)
    {
        return new ReadOnlySignalBuilder1<T1>(counter1);
    }
    
    public static ReadOnlySignalBuilder2<T1,T2> DependsOn<T1,T2>(BaseSignal<T1> counter1, BaseSignal<T2> counter2)
    {
        return new ReadOnlySignalBuilder2<T1,T2>(counter1, counter2);
    }
}

public class ReadOnlySignalBuilder1<T1>(BaseSignal<T1> counter1)
{
    public ComputedSignal<TResult> ComputedBy<TResult>(Func<BaseSignal<T1>, TResult> func)
    {
        return new ComputedSignal1<TResult, T1>(counter1, func);
    }
}

public class ReadOnlySignalBuilder2<T1,T2>(BaseSignal<T1> counter1, BaseSignal<T2> counter2)
{
    public ComputedSignal<TResult> ComputedBy<TResult>(Func<BaseSignal<T1>, BaseSignal<T2>, TResult> func)
    {
        return new ComputedSignal2<TResult, T1, T2>(counter1, counter2, func);
    }
}

