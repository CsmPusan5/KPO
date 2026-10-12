public class ChargeBattery : IThing, IEnergyConsumer
{
    private readonly string _inventoryNumber;
    private readonly string _name;
    private readonly int _easyScore;
    private readonly double _energyUsage; 

    public ChargeBattery(string inventoryNumber, string name, int easyScore, double energyUsage)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Инвентарный номер обязателен", nameof(inventoryNumber));

        if (easyScore < 1 || easyScore > 10)
            throw new ArgumentOutOfRangeException(nameof(easyScore), "Оценка простоты должна быть от 1 до 10");

        if (energyUsage < 0)
            throw new ArgumentOutOfRangeException(nameof(energyUsage), "Расход энергии не может быть отрицательным");

        _inventoryNumber = inventoryNumber;
        _name = name;
        _easyScore = easyScore;
        _energyUsage = energyUsage;
    }

    public string GetInventoryNumber() => _inventoryNumber;
    public string GetName() => _name;

    public int GetEasyScore() => _easyScore;
    public bool IsEasyForBeginner() => _easyScore >= 6;

    public double GetEnergyUsage() => _energyUsage;

    public void Use()
    {
        Console.WriteLine($"Баттарея {_inventoryNumber} заряжает");
    }
}