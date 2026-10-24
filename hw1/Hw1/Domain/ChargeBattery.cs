namespace Hw1.Domain;

public class ChargeBattery : Thing, IEnergyConsumer
{
    private readonly double _energyUsage;

    public ChargeBattery(string inventoryNumber, string name, int easyScore, double energyUsage)
        : base(inventoryNumber, name, easyScore)
    {
        if (energyUsage < 0)
            throw new ArgumentOutOfRangeException(nameof(energyUsage), "Расход энергии не может быть отрицательным");

        _energyUsage = energyUsage;
    }

    public double GetEnergyUsage() => _energyUsage;

    public override void Use()
        => Console.WriteLine($"Батарея {GetInventoryNumber()} заряжает");
}