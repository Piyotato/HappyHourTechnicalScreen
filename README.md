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

## Q2: CI/CD Pipeline

Designing a CI/CD Pipeline starts with designing the CI. First of all, we decide on a DevOps platform. Personally, I'm used to using GitLab for my game development so let's use that. I'm also going to tailor my response to a CI/CD pipeline for a game project. For simplicity, I'll just describe the broad strokes. The CI/CD will have 3 stages:

**1. Testing & Static Analysis**

In this stage, we will run the test for the project. Namely, we need:
* Unit tests to check component functionality;
* Integration tests to make sure our parts work together;
* Regression tests to detect reappearing bugs;
* Data serialization/deserialization tests, especially if the game is data-driven;
* Some unit performance tests, which are important in the context of games.

Note that this involves having already written comprehensive tests for the critical features in the project. Without comprehensive tests, it's risky to do CI/CD.

We will also run some static analysis, e.g. linting for code style and quality checks, and security scans for vulnerabilities. I'm not super familiar with static analysis but other important analyzers can be added here.

**2. Building**

The building process in this step depends on the platform. For Unity, we'll have to make standalone builds for each of our target platforms (e.g. Android, iOS, PC, Mac). We can add build validation scripts to this step as well, to make sure the build contains the expected files and doesn't have any broken/missing assets or broken references.

**3. Deploying**

Now that the game has been built, it can be uploaded to a test environment for testers and QA. We will also have automated tests in this test environment:
* For Unity, I haven't tried it yet but I believe Play Mode tests can be set up to test builds.
* Smoke tests can check if the build correctly starts up etc.
* Network (and multiplayer tests for games) can be automated as well if required.
* Performance tests, e.g. frame rate, memory usage etc.
* UI tests can be automated with frameworks like the [Unity UI Test Automation Framework](https://github.com/taphos/unity-uitest).
