# Signals

## Set-able Signals
There are two types of signals.  Top level signals can be assigned values.  Here we create one
called **counter** which we initially a assign a value of 4

`counter = new Signal(4);`

this can be changed to a five etc using `Set`.

`counter.Set(5);`

and read back using `Get`

`counter.Get();   // prints 5`


## Computed Signals

The second type of signals are computed ones,  these are based on other signals

`isEven = SignalBuilder.DependsOn(counter).ComputedBy(counterSignal => counterSignal.Get() % 2 == 0);`

`isEven.Get();   // prints false as counter is 5`

`counter.Set(6);`

`isEven.Get();   // prints true as counter is 6`

---


The proposed Javascript version looks a bit nicer,  but it's harder to work out what this signal depends on.

`var isEven = SignalBuilder.ComputedBy(() => counter.Get() % 2 == 0);`


---

Computed signals can also be based on other computed signals

`parity = SignalBuilder.DependsOn(isEven).ComputedBy(isEvenSignal => isEvenSignal.Get() ? "Even" : "Odd");`

`parity.Get();   // prints Even as isEven is true`

`counter.Set(9);`

`parity.Get();   // prints odd as isEven is now false`

---
or on multiple signals

`firstName = new Signal("David");`

`surname = new Signal("Betteridge");`

`fullname = SignalBuilder.DependsOn(firstName, surname).ComputedBy((f,l) => $"{f.Get()} {l.Get()}");`

---

Effects can be added to signals which are automatically triggered when a value changes

`counter.AddEffect((previous, current) => Console.WriteLine($"Counter changed {previous} to {current}"));`

`isEven.AddEffect((previous, current) => Console.WriteLine($"IsEven changed {previous} to {current}"));`

`parity.AddEffect((previous, current) => Console.WriteLine($"Parity changed {previous} to {current}"));`

``` csharp
counter.Set(10);
// Counter changed 9 to 10
// IsEven changed False to True
// Parity changed Odd to Even
```

``` csharp
counter.Set(12);
// Counter changed 10 to 12
```

---

## Lazy 

In the previous example not only aren't the other two effects triggered, but parity isn't even computed.

``` csharp
parity = SignalBuilder.DependsOn(isEven).ComputedBy(v =>
{
    Console.WriteLine("Compute parity");
    return v.Get() ? "Even" : "Odd";
});

// This changes the value for isEven so parity is computed
counter.Set(13);
parity.Get();

// This doesn't change the value for isEven so parity isn't computed
counter.Set(15);
parity.Get();
```

## Challenge

Attempt to implement the Signals API in a language of your choosing.
1. The solution should only trigger effects if the data has really changed.
2. Signals should only be evaluated if needed.
3. Try to provide a nice API for your users.


## Extension

So far we have only looked at a signals based on a primitive type (integer).  In the real world we need to
cater for arrays/lists of classes.  For example:

``` csharp

record People(string Name, int Age);

var data = new Signal<List<People>>([new People("David", 48)]); 

var adults = SignalBuilder.DependsOn(data)
                          .ComputedBy(value => value.Get().Where(p => p.Age > 18).ToList());

var numberOfAdults = SignalBuilder.DependsOn(adults)
                                  .ComputedBy(value =>
                                  {
                                      Console.WriteLine("Counting adults");
                                      return value.Get().Count;
                                  });

numberOfAdults.AddEffect((oldCount, newCount) => 
    Console.WriteLine($"Number of adults changed from {oldCount} to {newCount}"));

```

The **numberOfAdults** signal only needs to be computed if **adults** changes.

Hint: In order to get this to work,  you many have to supply your own Equality function.