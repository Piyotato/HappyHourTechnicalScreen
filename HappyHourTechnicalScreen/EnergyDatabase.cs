namespace HappyHourTechnicalScreen;

public class EnergyDatabase
{
    private readonly Func<DateTime> currentTimeGetter;

    /// <summary>
    /// Mockup database.
    /// </summary>
    private readonly Dictionary<string, EnergyManager> energyManagers = new();

    public readonly int MaxEnergy;
    private readonly TimeSpan timeForOneEnergy;

    public EnergyDatabase(int maxEnergy, TimeSpan timeForOneEnergy, Func<DateTime> currentTimeGetter)
    {
        MaxEnergy = maxEnergy;
        this.timeForOneEnergy = timeForOneEnergy;
        this.currentTimeGetter = currentTimeGetter;
    }

    public DateTime CurrentTime => currentTimeGetter();

    /// <summary>
    /// Gets the energy manager for a given player, or creates a new one if it does not exist.
    /// By default, the energy will be full.
    /// </summary>
    private EnergyManager GetOrCreate(string playerId)
    {
        if (energyManagers.TryGetValue(playerId, out var manager)) return manager;

        EnergyManager newManager = new(this, MaxEnergy, currentTimeGetter());
        energyManagers[playerId] = newManager;
        return newManager;
    }

    /// <summary>
    /// Gets the energy for a given player id at a given time.
    /// Assumptions: queries will only increase in time.
    /// </summary>
    public int GetEnergy(string playerId)
    {
        return GetOrCreate(playerId).GetEnergy();
    }

    public bool TrySpendEnergy(string playerId, int energy)
    {
        return GetOrCreate(playerId).TrySpendEnergy(energy);
    }

    /// <summary>
    /// Manages energy for a given player.
    /// </summary>
    private class EnergyManager
    {
        private readonly EnergyDatabase parentDatabase;
        private int energy;
        private DateTime lastUpdateTimeStamp;
        private TimeSpan timeStored;

        public EnergyManager(EnergyDatabase parentDatabase, int initialEnergy, DateTime lastUpdateTimeStamp)
        {
            this.parentDatabase = parentDatabase;
            this.lastUpdateTimeStamp = lastUpdateTimeStamp;
            energy = initialEnergy;
        }

        private TimeSpan TimeForOneEnergy => parentDatabase.timeForOneEnergy;
        private DateTime CurrentTime => parentDatabase.CurrentTime;
        private int MaxEnergy => parentDatabase.MaxEnergy;
        
        public int GetEnergy()
        {
            timeStored += CurrentTime - lastUpdateTimeStamp;
            var deltaEnergy = (int)(timeStored.Ticks / TimeForOneEnergy.Ticks);
            timeStored -= deltaEnergy * TimeForOneEnergy;

            energy = Math.Min(MaxEnergy, energy + deltaEnergy);
            if (energy == MaxEnergy) timeStored = TimeSpan.Zero;
            lastUpdateTimeStamp = CurrentTime;
            return energy;
        }

        /// <summary>
        /// Tries to spend a given amount of energy.
        /// Does not spend if there isn't enough.
        /// </summary>
        /// <returns>Whether energy was successfully spent.</returns>
        public bool TrySpendEnergy(int amountToSpend)
        {
            if (amountToSpend <= 0) return false;
            if (GetEnergy() < amountToSpend) return false;
            energy -= amountToSpend;
            return true;
        }
    }
}