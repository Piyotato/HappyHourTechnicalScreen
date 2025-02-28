# Technical Screen for Happy Hour Games

## Q1: Energy System

I have attempted this in C#. The function required is really just as follows:

```
public int GetEnergy()
{
    var deltaTime = CurrentTime - lastUpdateTimeStamp;
    var deltaEnergy = (int)(deltaTime.Ticks / TimeForOneEnergy.Ticks);

    if (deltaEnergy <= 0) return energy;

    energy = Math.Min(MaxEnergy, energy + deltaEnergy);
    lastUpdateTimeStamp = CurrentTime;
    return energy;
}
```

This function is part of an `EnergyManager` class. `CurrentTime` is a property of type `DateTime`; `TimeForOneEnergy` is of type `TimeSpan`; `MaxEnergy` is of type `int`.

In this repository, I have mocked up a database using a `Dictionary` keyed by `playerId` that further simulates the environment that this function might be used. The code has also been unit-tested using NUnit (unit tests are in this repository as well).

Some main assumptions I made about the behaviour of this system:
1. There's a realistic driver providing the `CurrentTime` value, and it will always provide values that doesn't cause `deltaEnergy` to overflow. In my test environment, I have it such that I can manually adjust it.
2. The energy for a new player is initialized to be max.
