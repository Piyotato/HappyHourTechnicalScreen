using NUnit.Framework;

namespace HappyHourTechnicalScreen;

[TestFixture]
public class EnergyDatabaseTests
{
    [SetUp]
    public void SetUp()
    {
        CurrentTime = DateTime.Now;
        db = new EnergyDatabase(5, TimeSpan.FromMinutes(30), () => CurrentTime);
    }

    private const string Player1 = "player1";
    private const string Player2 = "player2";
    private EnergyDatabase db;

    private DateTime CurrentTime { get; set; }

    [Test]
    public void NewPlayer_HasMaxEnergy()
    {
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(5));
        Assert.That(db.GetEnergy(Player2), Is.EqualTo(5));
    }

    [Test]
    public void SpendEnergy_SpendsWhenEnoughEnergy()
    {
        db.TrySpendEnergy(Player1, 1);

        Assert.That(db.GetEnergy(Player1), Is.EqualTo(4));
    }

    [Test]
    public void SpendEnergy_DoesNotSpendWhenNotEnoughEnergy()
    {
        db.TrySpendEnergy(Player1, 1);
        var hasSpentEnergy = db.TrySpendEnergy(Player1, 5);

        Assert.That(hasSpentEnergy, Is.False);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(4));
    }

    [Test]
    public void GetEnergy_RefillsOverTime()
    {
        db.TrySpendEnergy(Player1, 5);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(0));

        CurrentTime += TimeSpan.FromMinutes(60);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(2));

        CurrentTime += TimeSpan.FromMinutes(15);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(2));

        CurrentTime += TimeSpan.FromMinutes(15);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(3));
    }

    [Test]
    public void Energy_DoesNotExceedMax_And_DoesNotRefillAfterMax()
    {
        CurrentTime += TimeSpan.FromHours(1000);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(5));

        db.TrySpendEnergy(Player1, 3);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(2));

        CurrentTime += TimeSpan.FromMinutes(60);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(4));
    }

    [Test]
    public void EnergyManager_WorksWithMultiplePlayers()
    {
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(5));
        Assert.That(db.GetEnergy(Player2), Is.EqualTo(5));

        db.TrySpendEnergy(Player1, 3);
        db.TrySpendEnergy(Player2, 4);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(2));
        Assert.That(db.GetEnergy(Player2), Is.EqualTo(1));

        CurrentTime += TimeSpan.FromMinutes(60);
        Assert.That(db.GetEnergy(Player1), Is.EqualTo(4));
        Assert.That(db.GetEnergy(Player2), Is.EqualTo(3));
    }
}